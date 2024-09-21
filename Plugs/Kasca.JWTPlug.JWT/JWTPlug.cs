using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using JWT.Serializers;
using Kasca.CachePlug.Redis;
using Kasca.Common.ComModels;
using Kasca.Common.ComUtils;
using Kasca.Common.Plugs.LogPlug;
using Kasca.JWTPlug.JWT.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Kasca.JWTPlug.JWT
{
    public class JWTPlug
    {
        /// <summary>
        ///  JWT生成token的密钥
        /// </summary>
        public static string JWT_TOKEN_SECRET { get; } = ConfigUtil.GetSection("JWTTOKEN:JWT_TOKEN_SECRET")?.Value;

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
                { "IP", jwtPayload.IP },
                { "LoginLogoutTimeId", jwtPayload.LoginLogoutTimeId},
                { "ClientType", jwtPayload.ClientType}
            };

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var token = "Bearer " + encoder.Encode(payload, JWT_TOKEN_SECRET);

            //将生成的 token 写入到缓存中
            TokenMo tokenEntity = new TokenMo
            {
                TokenValue = token,
                SysUserId = jwtPayload.SysUserId,
                Expires = DateTime.Now.AddMinutes(Expire)
            };

            string json = JsonConvert.SerializeObject(tokenEntity);
           var result=  StackRedisPlug.Instance.Set(token, json, TimeSpan.FromMinutes(Expire));
            if(result==false)
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

                //校验码是否被注销
                if (StackRedisPlug.Instance.KeyExists(token) == false)
                {
                    return new ResultMo<SysUserJwtPayloadMo>(Common.ComModels.Enums.ResultTypes.UnAuthorize, "用户登录超时");
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

                //校验码是否被注销
                if (StackRedisPlug.Instance.KeyExists(token) == false)
                {
                    return new ResultMo<SysUserJwtPayloadMo>(Common.ComModels.Enums.ResultTypes.UnAuthorize, "用户登录超时");
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
