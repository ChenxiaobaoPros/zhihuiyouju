using System;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using log4net;
using System.Threading;

namespace Aoto.CQMS.Common.Sockets
{
    /// <summary>
    /// Socket服务，侦听请求
    /// 单例
    /// </summary>
    public sealed class SocketServer
    {
        #region ::::: 单例 :::::
        /// <summary>
        /// 私有构造函数
        /// </summary>
        private SocketServer() { }

        static SocketServer()
        {
            Instance = new SocketServer();
        }
        
        /// <summary>
        /// 单例
        /// </summary>
        public static SocketServer Instance { get; private set; }
        #endregion

        private Socket _socketServer;

        private static readonly ILog Log = LogManager.GetLogger("SocketServer");
        
        /// <summary>
        /// 开始执行线程
        /// </summary>
        /// <param name="ip">监听ip</param>
        /// <param name="port">监听端口</param>
        public void Start(string ip,string port)
        {
            try
            {
                if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(port))
                {
                    Log.Error("[SocketServer.Start]\tSocket监听无法开启：ip或端口未设置");
                    return;
                }

                IPAddress ipaddress;
                if (!IPAddress.TryParse(ip, out ipaddress))
                {
                    ipaddress = IPAddress.Parse("127.0.0.1");
                    Log.Error("[SocketServer.Start]\tSocket监听开启异常：ip转换错误，使用默认的ip");
                }

                int thePort;
                if (!int.TryParse(port.Trim(), out thePort))
                {
                    thePort = 9999;
                    Log.Error("[SocketServer.Start]\tSocket监听开启异常：端口转换错误，使用默认的端口9999");
                }

                IPEndPoint ipe = new IPEndPoint(ipaddress, thePort);

                _socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socketServer.Bind(ipe);
                _socketServer.Listen(0); //开始tcp端口监听

                //后台运行，主程序退出后线程退出
                var socketTask = new Task(ThreadExecute,TaskCreationOptions.LongRunning);
                socketTask.Start();

            }
            catch (Exception ex)
            {
                Log.Error("[SocketServer.Start]\t遇到错误-开启线程(无参)：" + ex.Message);
            }

        }

        /// <summary>
        /// 处理线程-收到socket命令开线程进行处理
        /// </summary>
        private void ThreadExecute()
        {
            Log.DebugFormat("[SocketServer.ThreadExecute]\tSocket侦听线程启动!!");
            try
            {
                while (true)
                {
                    var client = _socketServer.Accept();
                    var proc = new SocketAcceptRequest(client);
                    Task.Factory.StartNew(proc.ThreadProc);
                }
            }
            catch (Exception ex)
            {
                Log.Error("[SocketServer.ThreadExecute]\t遇到错误-侦听指令线程：" + ex);
            }
        }
    }
}

