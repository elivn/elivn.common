using System;
using Newtonsoft.Json;
using StackExchange.Redis;
using Kasca.Common.Plugs.CachePlug;
using Kasca.Common.ComUtils;
using System.Collections.Generic;
using Fleck;
using Kasca.Common.Plugs.LogPlug;

namespace Kasca.SocketPlug.WebSocket
{
    /// <summary>
    /// redis缓存实现类
    /// </summary>
    public class WebSocketPlug
    {
        /// <summary>
        /// 客户端url以及其对应的Socket对象字典
        /// </summary>
        public static Dictionary<string, IWebSocketConnection> dic_Sockets = new Dictionary<string, IWebSocketConnection>();

        /// <summary>
        /// 证书地址
        /// </summary>
        protected static string m_InitWebSocketName;
        /// <summary>
        /// ssl证书密码
        /// </summary>
        protected static string m_InitWebSocketPwd;

        /// <summary>
        /// ssl证书密码
        /// </summary>
        protected static bool isSSL = false;

        static WebSocketPlug()
        {
            m_InitWebSocketName = ConfigUtil.Configuration.GetSection("WebSocket:InitWebSocketName").Value;
            m_InitWebSocketPwd = ConfigUtil.Configuration.GetSection("WebSocket:InitWebSocketPwd").Value;
            if (ConfigUtil.Configuration.GetSection("WebSocket:isSSL").Value != "0")
            {
                isSSL = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void InitWebSocket()
        {
            try
            {
                WebSocketServer server = null;
                //创建
                if (isSSL)
                {
                    server = new WebSocketServer("wss://0.0.0.0:602");//监听所有的的地址
                    //出错后进行重启
                    server.Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(m_InitWebSocketName, m_InitWebSocketPwd, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet);//
                }
                else
                    server = new WebSocketServer("ws://0.0.0.0:602");//监听所有的的地址

                server.RestartAfterListenError = true;

                //开始监听
                server.Start(socket =>
                {
                    socket.OnOpen = () =>   //连接建立事件
                    {
                        //获取客户端网页的url
                        string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                        dic_Sockets.Add(clientUrl, socket);
                        //后期可增加日志记录信息
                        //  Console.WriteLine(DateTime.Now.ToString() + "|服务器:和客户端网页:" + clientUrl + " 建立WebSock连接！");
                    };
                    socket.OnClose = () =>  //连接关闭事件
                    {
                        string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                        //如果存在这个客户端,那么对这个socket进行移除
                        if (dic_Sockets.ContainsKey(clientUrl))
                        {
                            //注:Fleck中有释放
                            //关闭对象连接 
                            //if (dic_Sockets[clientUrl] != null)
                            //{
                            //dic_Sockets[clientUrl].Close();
                            //}
                            dic_Sockets.Remove(clientUrl);
                        }
                        //  Console.WriteLine(DateTime.Now.ToString() + "|服务器:和客户端网页:" + clientUrl + " 断开WebSock连接！");
                    };
                    socket.OnMessage = message =>  //接受客户端网页消息事件
                    {
                        string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                        //  Console.WriteLine(DateTime.Now.ToString() + "|服务器:【收到】来客户端网页:" + clientUrl + "的信息：\n" + message);
                    };
                });
            }
            catch (Exception ex)
            {
                LogUtil.Error($"执行结果 = { ex.Message }|{ ex.StackTrace}", nameof(WebSocketPlug));
            }
        }


        #region 单例模块

        private static object _lockObj = new object();

        private static WebSocketPlug _instance;

        /// <summary>
        ///   接口请求实例  
        ///  当 DefaultConfig 设值之后，可以直接通过当前对象调用
        /// </summary>
        public static WebSocketPlug Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (_lockObj)
                {
                    if (_instance == null)
                        _instance = new WebSocketPlug();
                }

                return _instance;
            }

        }

        #endregion


    }
}
