using System;
using System.Text;
using System.Net.Sockets;
using log4net;
using System.Net;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.Configuration;
using System.Diagnostics;

namespace Aoto.CQMS.Common.Sockets
{
    /// <summary>
    /// Socket请求处理线程类，处理主机发出的命令请求
    /// </summary>
    public class SocketAcceptRequest
    {
        private readonly Socket _client;

        private static readonly ILog Log = LogManager.GetLogger("SocketServer");
        
        public SocketAcceptRequest(Socket client)
        {
            _client = client;
        }
        
        /// <summary>
        /// 处理线程
        /// </summary>
        public void ThreadProc()
        {
            var sRemoteIP = "";
            try
            {
                //远程ip地址
                sRemoteIP = ((IPEndPoint)_client.RemoteEndPoint).Address.ToString();

                int iPackageLen = 4;   //包长度 4 和后台定义
                byte[] recvTemp = new byte[iPackageLen];//缓冲数组
                const int iRecvTimeout = 3; //接收超时时间（秒）
                var dtOverTime = DateTime.Now.AddSeconds(iRecvTimeout);
                _client.ReceiveTimeout = iRecvTimeout * 1000;// 设置超时
                iPackageLen = _client.Receive(recvTemp);// 接收数据

                if (iPackageLen == 0) return;
                Array.Reverse(recvTemp);
                var iDataLen = BitConverter.ToInt32(recvTemp, 0);
                var recvAll = new byte[iDataLen];

                recvTemp = new byte[1024];
                var iRecvLen = _client.Receive(recvTemp);
                Array.Copy(recvTemp, 0, recvAll, 0, iDataLen <= iRecvLen ? iDataLen : iRecvLen);

                var iTotalLen = iRecvLen;
                while (iTotalLen < iDataLen && iRecvLen > 0 && dtOverTime > DateTime.Now)
                {

                    iRecvLen = _client.Receive(recvTemp);
                    if (iDataLen - iTotalLen > iRecvLen)
                    {
                        Array.Copy(recvTemp, 0, recvAll, iTotalLen, iRecvLen);
                    }
                    else
                    {
                        Array.Copy(recvTemp, 0, recvAll, iTotalLen, iDataLen - iTotalLen);
                    }
                    iTotalLen += iRecvLen;
                }

                if (iTotalLen < iDataLen)//包不对
                {
                    Log.Error(String.Format("[SocketAcceptRequest.ThreadProc]\t[IP={0}]\t数据长度不足", sRemoteIP));
                }
                else//收到数据正确
                {
                    string sDataLen = Encoding.UTF8.GetString(recvAll);
                    var jsonObjDoc = JObject.Parse(sDataLen);
                    //解析指令
                    switch (jsonObjDoc["robot"]["head"].Value<string>("tradeCode"))
                    {
                        case "qmssign"://查询排队机
                            string result = GlobalVariable2ICBC.ICBC_QMSSIGN.ToString();//缓存中取数据
                            var targetObj = JsonConvert.DeserializeObject<JsonObj.QmssignJson.Root>(result);
                            Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss")+"收到查询排队机请求，内容为\n" + sDataLen);
                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "收到查询排队机请求，内容为\n" + sDataLen);
                            #region ::::: 填值 :::::
                            targetObj.robot.head.tradeCode = "qmssign";
                            targetObj.robot.head.channel = jsonObjDoc["robot"]["head"].Value<string>("channel");
                            targetObj.robot.head.orgCode = jsonObjDoc["robot"]["head"].Value<string>("orgCode");
                            targetObj.robot.head.robotID = jsonObjDoc["robot"]["head"].Value<string>("robotID");
                            targetObj.robot.body.ticketPrintJson = string.Empty;
                            #endregion

                            string responseJsonQmssign = new JavaScriptSerializer().Serialize(targetObj);
                            Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "返回消息，内容为\n" + responseJsonQmssign);
                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "返回消息，内容为\n" + responseJsonQmssign);
                            SendMessBySocket(_client, responseJsonQmssign);
                            break;
                        case "authentication"://远程鉴权
                        case "custgetseq"://远程取号
                            var requestTemp = JsonConvert.DeserializeObject<JsonObj.CustgetseqJson.RequestJsonObject.Root>(sDataLen);
                            Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "收到取号请求，内容为\n" + sDataLen);
                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "收到取号请求，内容为\n" + sDataLen);

                            #region ::::: 封装http发送需要的content :::::
                            IcbcInfos icbcInfo = new IcbcInfos();
                            requestTemp.biom.head.qmsIp = BuzConfig2ICBC.LocalIP;
                            icbcInfo.QmsIp = requestTemp.biom.head.qmsIp;
                            icbcInfo.TradeCode = requestTemp.biom.head.tradeCode;
                            icbcInfo.Content = new JavaScriptSerializer().Serialize(requestTemp);
                            string dataStr = HttpClient.Post("/", icbcInfo);
                            #endregion

                            var response = JsonConvert.DeserializeObject<JsonObj.CustgetseqJson.ResponseJsonObject.Root>(dataStr);

                            #region ::::: 填值 :::::
                            response.robot.head.tradeCode = jsonObjDoc["robot"]["head"].Value<string>("tradeCode");
                            response.robot.head.channel = jsonObjDoc["robot"]["head"].Value<string>("channel");
                            response.robot.head.orgCode = jsonObjDoc["robot"]["head"].Value<string>("orgCode");
                            response.robot.head.robotID = jsonObjDoc["robot"]["head"].Value<string>("robotID");
                            #endregion

                            string responseJsonCustgetseq = new JavaScriptSerializer().Serialize(response);
                            Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "返回消息，内容为\n" + responseJsonCustgetseq);
                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "返回消息，内容为\n" + responseJsonCustgetseq);
                            SendMessBySocket(_client, responseJsonCustgetseq);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("[SocketAcceptRequest.ThreadProc]\t[IP={0}]连接错误:{1}", sRemoteIP, ex.Message));
            }
            finally
            {
                try
                {
                    // 关闭socket连接                 
                    _client.Close();
                }
                catch
                {
                    // ignored
                }
            }
        }

        /// <summary>
        /// 发送消息给后台
        /// </summary>
        private void SendMessBySocket(Socket socket, string sSendData)
        {
            byte[] forwardMsg = Encoding.UTF8.GetBytes(sSendData);//原始数据
            int forwardMsgLen = forwardMsg.Length;
            var msgLenByte = BitConverter.GetBytes(forwardMsgLen);
            Array.Reverse(msgLenByte);
            byte[] allMsg = new byte[4 + forwardMsgLen];//发送总数据

            Array.Copy(msgLenByte, allMsg, 4);
            Array.Copy(forwardMsg, 0, allMsg, 4, forwardMsgLen);

            try
            {
                socket.Send(allMsg);
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("[SocketAcceptRequest.SendMessBySocket]\t数据发送错误:{0}", ex.Message));
            }
        }
    }
}
