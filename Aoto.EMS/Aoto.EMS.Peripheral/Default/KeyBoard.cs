using Aoto.EMS.Infrastructure;
using Aoto.EMS.Infrastructure.ComponentModel;
using Aoto.EMS.Infrastructure.Configuration;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;

namespace Aoto.EMS.Peripheral
{
    public class KeyBoard : IKeyBoard
    {
        #region 托管接口
        //
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SUNSON_OpenCom(int nPortNo, int nTimeOut);
        //
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SUNSON_GetVersionNo(StringBuilder ReturnInfo);
        //
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SUNSON_CloseCom();
        //
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SUNSON_UseEppPlainTextMode(int PlaintextLength, int AutoEnd, StringBuilder ReturnInfo);
        //
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SUNSON_ResetEpp(StringBuilder ReturnInfo);
        //
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SUNSON_ScanKeyPress(StringBuilder ucValue);
        //
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SUNSON_LoadUserKey(int ucKeyId, int ucDecryptKeyId, int KeyAttribute, int ucKeyLen, StringBuilder KeyValue, StringBuilder ReturnInfo);
        //
        //int WINAPI	SUNSON_CloseEppPlainTextMode(unsigned char *ReturnInfo);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SUNSON_CloseEppPlainTextMode(StringBuilder ucValue);
        //
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SUNSON_GetPin(int ucPinMinLen, int ucPinMaxLen, int AutoReturnFlag, StringBuilder ReturnInfo);
        //
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SUNSON_GetPinBlock(int UserKeyId, int JM_mode, int padchar, int ucCardLen, String ucCardNumber, byte[] PinBlockResult);
        //
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int SUNSON_DataCompute(int KeyId, int JM_mode, int SF_mode, int padchar, int datalen, byte[] data, StringBuilder DataResult);
        #endregion

        private SUNSON_OpenCom sUNSON_OpenCom;
        private SUNSON_GetVersionNo sUNSON_GetVersionNo;

        private SUNSON_CloseCom sUNSON_CloseCom;
        private SUNSON_UseEppPlainTextMode sUNSON_UseEppPlainTextMode;
        private SUNSON_ResetEpp sUNSON_ResetEpp;
        private SUNSON_ScanKeyPress sUNSON_ScanKeyPress;
        private SUNSON_LoadUserKey sUNSON_LoadUserKey;
        private SUNSON_CloseEppPlainTextMode sUNSON_CloseEppPlainTextMode;
        private SUNSON_GetPin sUNSON_GetPin;
        private SUNSON_GetPinBlock sUNSON_GetPinBlock;
        private SUNSON_DataCompute sUNSON_DataCompute;


        private IScriptInvoker scriptInvoker;
        private static readonly ILog log = LogManager.GetLogger("keyBoard");
        private IntPtr intPtr;
        private RunAsyncCaller asyncCaller;
        public event RunCompletedEventHandler RunCompletedEvent;
        private int ComNumber = 6;
        private StringBuilder Rec = new StringBuilder(100); //返回值
        int nOpend = 0;

        private string SecretKey = "12345678ABCDEFFF";  //秘钥


        protected string name;
        protected string dll;
        protected int timeout;

        protected bool enabled;
        protected bool isBusy;
        protected bool cancelled;

        public bool Cancelled { get { return cancelled; } set { cancelled = value; } }
        public bool Enabled { get { return enabled; } }
        public bool IsBusy { get { return isBusy; } }

        public KeyBoard()
        {
            isBusy = false;
            cancelled = false;
            scriptInvoker = AutofacContainer.ResolveNamed<IScriptInvoker>("scriptInvoker");
            this.dll = Config.App.Peripheral["keyBoard"].Value<string>("dll");
            this.timeout = Config.App.Peripheral["keyBoard"].Value<int>("timeout");
            this.enabled = Config.App.Peripheral["keyBoard"].Value<bool>("enabled");
            this.name = Config.App.Peripheral["keyBoard"].Value<string>("name");

            asyncCaller = new RunAsyncCaller(Read);
            Initialize();
        }
        public void Read(JObject jo)
        {
            while (nOpend > 0)
            {
                StringBuilder cValue = new StringBuilder();
                int ret = sUNSON_ScanKeyPress(cValue);
                /*
                 1、在明文状态下，获取的按键键值为明文键值（0:0x30 1:0x31 2:0x32 3:0x33 
                 4:0x34 5:0x35 6:0x36 7:0x37 8:0x38 9:0x39 00:0x30   取消：0x1b 更正：
                 0x08 确认：0x0D 。对应的 8 个功能键键值依次 0x41 0x42 0x43 0x44 0x45 
                 0x46 0x47 0x48）
                 2、在密文状态下输入的数字键，获取的是字符‘*’号，键值 0x2a。功能键键
                 值与明文状态相同。
                 3、在明文状态下或 PIN 输入状态，当到达指定的字符输入长度时，回送结束键
                 */
                if (ret > 0) //textBox4.Text += cValue[0];
                {
                    switch (cValue[0])
                    {
                        case (char)0x1B: //取消
                            sUNSON_CloseEppPlainTextMode(cValue);
                            break;
                        case (char)0x08: //更正
                            break;
                        case (char)0x0D: //确认
                            break;
                        case '?':
                            sUNSON_CloseEppPlainTextMode(cValue);
                            break;
                        case 'T': //上翻
                            break;
                        case '#': //下翻
                            break;
                        case '.'://0
                            jo["value"] = cValue[0].ToString();
                            jo["model"] = "Text";
                            jo["callback"] = "displayValue";
                            scriptInvoker.ScriptInvoke(jo);
                            break;
                        case (char)0x30://0
                            jo["value"] = cValue[0].ToString();
                            jo["model"] = "Text";
                            jo["callback"] = "displayValue";
                            scriptInvoker.ScriptInvoke(jo);
                            break;
                        case (char)0x31://1
                            jo["value"] = cValue[0].ToString();
                            jo["model"] = "Text";
                            jo["callback"] = "displayValue";
                            scriptInvoker.ScriptInvoke(jo);
                            break;
                        case (char)0x32://2
                            jo["value"] = cValue[0].ToString();
                            jo["model"] = "Text";
                            jo["callback"] = "displayValue";
                            scriptInvoker.ScriptInvoke(jo);
                            break;
                        case (char)0x33://3
                            jo["value"] = cValue[0].ToString();
                            jo["model"] = "Text";
                            jo["callback"] = "displayValue";
                            scriptInvoker.ScriptInvoke(jo);
                            break;
                        case (char)0x34://4
                            jo["value"] = cValue[0].ToString();
                            jo["model"] = "Text";
                            jo["callback"] = "displayValue";
                            scriptInvoker.ScriptInvoke(jo);
                            break;
                        case (char)0x35://5
                            jo["value"] = cValue[0].ToString();
                            jo["callback"] = "displayValue";
                            scriptInvoker.ScriptInvoke(jo);
                            break;
                        case (char)0x36://6
                            jo["value"] = cValue[0].ToString();
                            jo["model"] = "Text";
                            jo["callback"] = "displayValue";
                            scriptInvoker.ScriptInvoke(jo);
                            break;
                        case (char)0x37://7
                            jo["value"] = cValue[0].ToString();
                            jo["model"] = "Text";
                            jo["callback"] = "displayValue";
                            scriptInvoker.ScriptInvoke(jo);
                            break;
                        case (char)0x38://8
                            jo["value"] = cValue[0].ToString();
                            jo["model"] = "Text";
                            jo["callback"] = "displayValue";
                            scriptInvoker.ScriptInvoke(jo);
                            break;
                        case (char)0x39://9
                            jo["value"] = cValue[0].ToString();
                            jo["model"] = "Text";
                            jo["callback"] = "displayValue";
                            scriptInvoker.ScriptInvoke(jo);
                            break;
                        case '*':
                            jo["value"] = cValue[0].ToString();
                            jo["model"] = "CipherText";
                            jo["callback"] = "displayValue";
                            scriptInvoker.ScriptInvoke(jo);
                            break;
                        default:
                            break;
                    }
                }

                Thread.Sleep(50);

            }

        }
        public void ReadAsync(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            if (!enabled)
            {
                return;
            }

            if (isBusy)
            {
                jo["result"] = ErrorCode.Busy;
                log.InfoFormat("end, isBusy = {0}", isBusy);
                return;
            }

            isBusy = true;
            asyncCaller.BeginInvoke(jo, new AsyncCallback(ReadCallback), jo);
            log.Debug("end");
        }

        private void ReadCallback(IAsyncResult ar)
        {
            JObject jo = (JObject)ar.AsyncState;

            try
            {
                ((RunAsyncCaller)((AsyncResult)ar).AsyncDelegate).EndInvoke(ar);
            }
            catch (Exception e)
            {
                jo["result"] = ErrorCode.Failure;
                log.Error("Error", e);
            }
            finally
            {
                isBusy = false;
                int t = jo.Value<int>("timeout");

                if (864000000 == t)
                {
                    if (ErrorCode.Offline != jo.Value<int>("result"))
                    {
                        //scriptInvoker.ScriptInvoke(jo);
                    }

                    if (!cancelled)
                    {
                        Thread.Sleep(200);

                        JObject joo = new JObject();
                        joo["objId"] = jo["objId"];
                        joo["callback"] = jo["callback"];
                        joo["tag"] = jo["tag"];
                        joo["type"] = jo["type"];
                        joo["timeout"] = t;
                        Read(joo);
                    }
                }
                else
                {
                    jo["callback"] = "displayValue";
                    RunCompletedEvent(this, new RunCompletedEventArgs(jo));
                }
            }
        }

        public void Initialize()
        {
            log.Debug("begin");

            if (!enabled)
            {
                return;
            }

            string dllPath = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, dll);

            if (!File.Exists(dllPath))
            {
                dllPath = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, "KeyBoardLib", dll);
            }

            intPtr = Win32ApiInvoker.LoadLibrary(dllPath);
            log.InfoFormat("LoadLibrary: dllPath = {0}, ptr = {1}", dllPath, intPtr);

            uint idcoed = Win32ApiInvoker.GetLastError();
            IntPtr api = Win32ApiInvoker.GetProcAddress(intPtr, "SUNSON_OpenCom");
            sUNSON_OpenCom = (SUNSON_OpenCom)Marshal.GetDelegateForFunctionPointer(api, typeof(SUNSON_OpenCom));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "SUNSON_UseEppPlainTextMode");
            sUNSON_UseEppPlainTextMode = (SUNSON_UseEppPlainTextMode)Marshal.GetDelegateForFunctionPointer(api, typeof(SUNSON_UseEppPlainTextMode));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "SUNSON_GetPin");
            sUNSON_GetPin = (SUNSON_GetPin)Marshal.GetDelegateForFunctionPointer(api, typeof(SUNSON_GetPin));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "SUNSON_ScanKeyPress");
            sUNSON_ScanKeyPress = (SUNSON_ScanKeyPress)Marshal.GetDelegateForFunctionPointer(api, typeof(SUNSON_ScanKeyPress));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "SUNSON_GetVersionNo");
            sUNSON_GetVersionNo = (SUNSON_GetVersionNo)Marshal.GetDelegateForFunctionPointer(api, typeof(SUNSON_GetVersionNo));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "SUNSON_CloseCom");
            sUNSON_CloseCom = (SUNSON_CloseCom)Marshal.GetDelegateForFunctionPointer(api, typeof(SUNSON_CloseCom));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "SUNSON_ResetEpp");
            sUNSON_ResetEpp = (SUNSON_ResetEpp)Marshal.GetDelegateForFunctionPointer(api, typeof(SUNSON_ResetEpp));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "SUNSON_LoadUserKey");
            sUNSON_LoadUserKey = (SUNSON_LoadUserKey)Marshal.GetDelegateForFunctionPointer(api, typeof(SUNSON_LoadUserKey));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "SUNSON_CloseEppPlainTextMode");
            sUNSON_CloseEppPlainTextMode = (SUNSON_CloseEppPlainTextMode)Marshal.GetDelegateForFunctionPointer(api, typeof(SUNSON_CloseEppPlainTextMode));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "SUNSON_GetPinBlock");
            sUNSON_GetPinBlock = (SUNSON_GetPinBlock)Marshal.GetDelegateForFunctionPointer(api, typeof(SUNSON_GetPinBlock));

            api = Win32ApiInvoker.GetProcAddress(intPtr, "SUNSON_DataCompute");
            sUNSON_DataCompute = (SUNSON_DataCompute)Marshal.GetDelegateForFunctionPointer(api, typeof(SUNSON_DataCompute));

            //先判断，关闭
            if (nOpend > 0) sUNSON_CloseCom();
            //打开串口
            nOpend = sUNSON_OpenCom(ComNumber, 9600);

            StringBuilder ReturnInfo = new StringBuilder(100);
            int ret = 0;

            if (ret > 0)
            {
                //成功
            }
        }
        /// <summary>
        /// 开启明文模式
        /// </summary>
        public void TurnOnTextMode()
        {
            if (nOpend == 0)
                return;
            int ret = sUNSON_UseEppPlainTextMode(10, 1, Rec);
            //if (ret > 0)
            //    return;
        }
        //开启密文模式
        public void TurnOnCipherTextMode()
        {
            int ret = sUNSON_GetPin(0, 10, 1, Rec);
        }
        //键盘复位
        public void ResetEpp()
        {
            StringBuilder ReturnInfo = new StringBuilder(100);
            int ret = sUNSON_ResetEpp(ReturnInfo);
            //if (ret > 0)//键盘复位成功
            //else //键盘复位失败/
        }

        public void LoadUserKey()
        {
            //textBox1.Text
            //char textBox1.Text.
            StringBuilder ReturnInfo = new StringBuilder(100);
            StringBuilder Value = new StringBuilder(SecretKey);
            //Value = textBox1.Text;
            int ret = sUNSON_LoadUserKey(0, 65535, 3, 0x08, Value, ReturnInfo);
            //if (ret > 0) textBox4.Text += "秘钥注入成功\r\n";
            //else textBox4.Text += "秘钥注入失败\r\n";
        }
        //解密
        public void Decrypt()
        {
            byte[] ReturnInfo = new byte[100];
            int ret = sUNSON_GetPinBlock(0, 0x36, 0, 12, "123456789012", ReturnInfo);
            if (ret > 0)
            {
                string result = "";//计算结果
                //获取Block成功
                for (int i = 0; i < 8; i++)
                {
                    String aa = "00" + Convert.ToString(ReturnInfo[i], 16);       //必须保证转换后的16进制是2位数
                    result += aa.Substring(aa.Length - 2, 2);
                }

                StringBuilder ReturnInfo1 = new StringBuilder(100);
                //StringBuilder data = new StringBuilder(textBox2.Text);
                byte[] data = strToToHexByte(result);

                ret = sUNSON_DataCompute(0, 0x30, 0x30, 0, data.Length, data, ReturnInfo1);

                string realResult = "";
                if (ret > 0)
                {
                    //解密成功
                    if (ReturnInfo1.Length > 8)
                        realResult = ReturnInfo1.ToString().Substring(0, 8);
                    else
                        realResult = ReturnInfo1.ToString();

                    JObject jo = new JObject();
                    jo["value"] = realResult;
                    jo["model"] = "Decrypt";
                    jo["callback"] = "displayValue";
                    scriptInvoker.ScriptInvoke(jo);

                }
                else
                {
                    //解密失败
                }
            }
            else
            {
                //获取Block失败
            }
        }
        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
    }

}
