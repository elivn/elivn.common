using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using JWT.Serializers;
using Kasca.Common.Authrization;
using Kasca.Common.ComModels;
using Kasca.Common.ComUtils;
using Kasca.Common.Plugs.CachePlug;
using Kasca.Common.Plugs.LogPlug;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kasca.Common.Plugs.JWTPlug
{
    /// <summary>
    /// 
    /// </summary>
    public class JWTPlug
    {
        /// <summary>
        ///  JWT生成token的密钥
        /// </summary>
        public static string JWT_TOKEN_SECRET { get; } = ConfigUtil.GetSection("JWTTOKEN:JWT_TOKEN_SECRET")?.Value;

        /// <summary>
        ///  JWT生成token的密钥
        /// </summary>
        public static int JWT_TOKEN_Expir { get; } = Convert.ToInt32(ConfigUtil.GetSection("JWTTOKEN:JWT_TOKEN_Expir")?.Value);

        static string ClientType = "Kasca_PC_System";

        /// <summary>
        /// 生成 token
        /// </summary>
        /// <param name="jwtPayload"></param>
        /// <param name="Expire"></param>
        /// <returns></returns>
        public static string CreateJwtToken(SysUserJwtPayloadMo jwtPayload, int Expire)
        {
            var provider = new UtcDateTimeProvider();
            var now = provider.GetNow();
            var secondsSinceEpoch = UnixEpoch.GetSecondsSince(now.AddMinutes(Expire));
            var payload = new Dictionary<string, object>
            {
                { "exp", secondsSinceEpoch },
                { "SysUserId", jwtPayload.SysUserId },
                { "Username", jwtPayload.Username },
                { "SysRoleType", jwtPayload.SysRoleType },
                { "IdentityType", jwtPayload.IdentityType },
                { "IdentityId", jwtPayload.IdentityId },
                { "IP", jwtPayload.IP },
                { "LoginLogoutTimeId", jwtPayload.LoginLogoutTimeId},
                { "ClientType", jwtPayload.ClientType}
            };

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var tokenBody= encoder.Encode(payload, JWT_TOKEN_SECRET);
            var token = $"Bearer {tokenBody}";

            //将生成的 token 写入到缓存中
            TokenMo tokenEntity = new TokenMo
            {
                TokenValue = token,
                SysUserId = jwtPayload.SysUserId,
                Expires = DateTime.Now.AddMinutes(Expire)
            };

            string json = JsonConvert.SerializeObject(tokenEntity);
            var pcFlag = "";
            if (ClientType == jwtPayload.ClientType)
                pcFlag = ClientType;
            var result = StackRedisPlug.Instance.Set($"JwtToken{pcFlag}{jwtPayload.SysUserId}", tokenBody, TimeSpan.FromMinutes(Expire));
            if (result == false)
                LogUtil.Error($"插入缓存信息失败{token}", "CreateJwtToken", "JWTPlug");
            return token;
        }

        /// <summary>
        /// 验证 token
        /// </summary>
        /// <returns></returns>
        public static ResultMo<SysUserJwtPayloadMo> ValidJwtToken()
        {
            ResultMo<SysUserJwtPayloadMo> AR = new ResultMo<SysUserJwtPayloadMo>();
            try
            {
                var token = Kasca.Common.Authrization.MemberShiper.AppAuthorize?.Token;
                if (string.IsNullOrEmpty(token))
                {
                    return new ResultMo<SysUserJwtPayloadMo>(Common.ComModels.Enums.ResultTypes.UnAuthorize, "未登录");
                }
                if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer"))
                {
                    return new ResultMo<SysUserJwtPayloadMo>(Common.ComModels.Enums.ResultTypes.UnAuthorize, "授权码错误");
                }

              
                token = token.Substring("Bearer ".Length).Trim();

                IJsonSerializer serializer = new JsonNetSerializer();
                var provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);

                var json = decoder.Decode(token, JWT_TOKEN_SECRET, verify: true);
                var SysUserJwtPayloadMo = JsonConvert.DeserializeObject<SysUserJwtPayloadMo>(json);
                var pcFlag = "";
                if (ClientType == SysUserJwtPayloadMo.ClientType)
                    pcFlag = ClientType;
                //校验码是否被注销
                if (StackRedisPlug.Instance.KeyExists($"JwtToken{pcFlag}{SysUserJwtPayloadMo.SysUserId}") == false)
                {
                    return new ResultMo<SysUserJwtPayloadMo>(Common.ComModels.Enums.ResultTypes.UnAuthorize, "用户登录超时");
                }
                if (!StackRedisPlug.Instance.Get<string>($"JwtToken{pcFlag}{SysUserJwtPayloadMo.SysUserId}").Equals(token) )
                {
                    return new ResultMo<SysUserJwtPayloadMo>(Common.ComModels.Enums.ResultTypes.UnAuthorize, "用户登录过期");
                }
                return new ResultMo<SysUserJwtPayloadMo>(SysUserJwtPayloadMo);
            }
            catch (TokenExpiredException)
            {
                return new ResultMo<SysUserJwtPayloadMo>(Common.ComModels.Enums.ResultTypes.UnAuthorize, "用户登录超时");
            }
            catch (SignatureVerificationException)
            {
                return new ResultMo<SysUserJwtPayloadMo>(Common.ComModels.Enums.ResultTypes.UnAuthorize, "用户授权码错误");
            }
            catch(Exception error)
            {
                LogUtil.Error(string.Concat("错误信息：", error.Message, "详细信息：", error.StackTrace), "ValidJwtToken");
                return new ResultMo<SysUserJwtPayloadMo>(Common.ComModels.Enums.ResultTypes.UnAuthorize, "身份验证未知错误");
            }
        }

        /// <summary>
        /// 验证 token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static ResultMo<SysUserJwtPayloadMo> ValidJwtToken(string token)
        {
            ResultMo<SysUserJwtPayloadMo> AR = new ResultMo<SysUserJwtPayloadMo>();
            try
            {
                if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer"))
                {
                    return new ResultMo<SysUserJwtPayloadMo>(Common.ComModels.Enums.ResultTypes.UnAuthorize, "授权码错误");
                }

                token = token.Substring("Bearer ".Length).Trim();

                IJsonSerializer serializer = new JsonNetSerializer();
                var provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);

                var json = decoder.Decode(token, JWT_TOKEN_SECRET, verify: true);
                var SysUserJwtPayloadMo = JsonConvert.DeserializeObject<SysUserJwtPayloadMo>(json);
                var pcFlag = "";
                if (ClientType == SysUserJwtPayloadMo.ClientType)
                    pcFlag = ClientType;
                //校验码是否被注销
                if (StackRedisPlug.Instance.KeyExists($"JwtToken{pcFlag}{SysUserJwtPayloadMo.SysUserId}") == false)
                {
                    return new ResultMo<SysUserJwtPayloadMo>(Common.ComModels.Enums.ResultTypes.UnAuthorize, "用户登录超时");
                }
                if (!StackRedisPlug.Instance.Get<string>($"JwtToken{pcFlag}{SysUserJwtPayloadMo.SysUserId}").Equals(token))
                {
                    return new ResultMo<SysUserJwtPayloadMo>(Common.ComModels.Enums.ResultTypes.UnAuthorize, "用户登录过期");
                }
                return new ResultMo<SysUserJwtPayloadMo>(SysUserJwtPayloadMo);
            }
            catch (TokenExpiredException)
            {
                return new ResultMo<SysUserJwtPayloadMo>(Common.ComModels.Enums.ResultTypes.UnAuthorize, "用户登录超时");
            }
            catch (SignatureVerificationException)
            {
                return new ResultMo<SysUserJwtPayloadMo>(Common.ComModels.Enums.ResultTypes.UnAuthorize, "用户授权码错误");
            }
        }
    }
}
