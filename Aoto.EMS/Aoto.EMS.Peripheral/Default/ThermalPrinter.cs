using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using Aoto.EMS.Common;
using Aoto.EMS.Infrastructure.ComponentModel;
using Aoto.EMS.Infrastructure.Configuration;
using log4net;
using Newtonsoft.Json.Linq;

namespace Aoto.EMS.Peripheral
{
    public class ThermalPrinter : IPrinter
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        protected struct StructPrinterDefaults
        {
            [MarshalAs(UnmanagedType.LPTStr)]
            public String pDatatype;

            public IntPtr pDevMode;

            [MarshalAs(UnmanagedType.I4)]
            public int DesiredAccess;
        }

        protected struct PrinterInfo2
        {
            public string PServerName;
            public string PPrinterName;
            public string PShareName;
            public string PPortName;
            public string PDriverName;
            public string PComment;
            public string PLocation;
            public IntPtr PDevMode;
            public string PSepFile;
            public string PPrintProcessor;
            public string PDatatype;
            public string PParameters;
            public IntPtr PSecurityDescriptor;
            public UInt32 Attributes;
            public UInt32 Priority;
            public UInt32 DefaultPriority;
            public UInt32 StartTime;
            public UInt32 UntilTime;
            public UInt32 Status;
            public UInt32 CJobs;
            public UInt32 AveragePpm;
        }

        [FlagsAttribute]
        protected enum PrinterStatus
        {
            PRINTER_STATUS_BUSY = 0x00000200,
            PRINTER_STATUS_DOOR_OPEN = 0x00400000,
            PRINTER_STATUS_ERROR = 0x00000002,
            PRINTER_STATUS_INITIALIZING = 0x00008000,
            PRINTER_STATUS_IO_ACTIVE = 0x00000100,
            PRINTER_STATUS_MANUAL_FEED = 0x00000020,
            PRINTER_STATUS_NO_TONER = 0x00040000,
            PRINTER_STATUS_NOT_AVAILABLE = 0x00001000,
            PRINTER_STATUS_OFFLINE = 0x00000080,
            PRINTER_STATUS_OUT_OF_MEMORY = 0x00200000,
            PRINTER_STATUS_OUTPUT_BIN_FULL = 0x00000800,
            PRINTER_STATUS_PAGE_PUNT = 0x00080000,
            PRINTER_STATUS_PAPER_JAM = 0x00000008,
            PRINTER_STATUS_PAPER_OUT = 0x00000010,
            PRINTER_STATUS_PAPER_PROBLEM = 0x00000040,
            PRINTER_STATUS_PAUSED = 0x00000001,
            PRINTER_STATUS_PENDING_DELETION = 0x00000004,
            PRINTER_STATUS_PRINTING = 0x00000400,
            PRINTER_STATUS_PROCESSING = 0x00004000,
            PRINTER_STATUS_TONER_LOW = 0x00020000,
            PRINTER_STATUS_USER_INTERVENTION = 0x00100000,
            PRINTER_STATUS_WAITING = 0x20000000,
            PRINTER_STATUS_WARMING_UP = 0x00010000
        }

        //[DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern long SetDefaultPrinter(string pszPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        protected static extern bool OpenPrinter(string printer, out IntPtr handle, ref StructPrinterDefaults pDefault);

        [DllImport("winspool.drv")]
        protected static extern bool ClosePrinter(IntPtr handle);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        protected static extern bool GetPrinter(IntPtr handle, Int32 level, IntPtr buffer, Int32 size, out Int32 sizeNeeded);

        //[DllImport("winspool.drv", EntryPoint = "EnumJobsA")]
        //public static extern bool EnumJobs(IntPtr hPrinter, uint FirstJob, uint NoJobs, uint Level, IntPtr pJob, uint cdBuf, out uint pcbNeeded, out uint pcReturned);

        //[DllImport("winspool.drv", EntryPoint = "SetJob")]
        //public static extern int SetJob(IntPtr hPrinter, int JobId, int Level, IntPtr pJob, JOB_CONTROL Command);

        [DllImport("winspool.drv", CharSet = CharSet.Auto)]
        protected static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, IntPtr pDefault);

        private static readonly ILog log = LogManager.GetLogger("printer"); 
        protected PrintDocument printDocument;
        protected RunAsyncCaller printAsyncCaller;
        protected ConcurrentQueue<JObject> queue;
        protected IntPtr ptr;

        protected string name;
        protected string dll;
        protected int timeout;
        protected bool enabled;
        protected bool isBusy;
        protected bool cancelled;

        public bool Cancelled { get { return cancelled; } set { cancelled = value; } }
        public bool Enabled { get { return enabled; } }
        public bool IsBusy { get { return isBusy; } }
        public event RunCompletedEventHandler RunCompletedEvent;

        public ThermalPrinter()
        {
            isBusy = false;
            cancelled = false;

            this.dll = Config.App.Peripheral["thermalPrinter"].Value<string>("dll");
            this.timeout = Config.App.Peripheral["thermalPrinter"].Value<int>("timeout");
            this.enabled = Config.App.Peripheral["thermalPrinter"].Value<bool>("enabled");
            this.name = Config.App.Peripheral["thermalPrinter"].Value<string>("name");

            printAsyncCaller = new RunAsyncCaller(Print);
            queue = new ConcurrentQueue<JObject>();

            printDocument = new PrintDocument();
            printDocument.PrintController = new StandardPrintController();
            printDocument.PrinterSettings.PrinterName = name;
            printDocument.PrinterSettings.DefaultPageSettings.Margins.Left = 30;
            printDocument.PrinterSettings.DefaultPageSettings.Margins.Top = 0;
            //设置边距
            //Margins margin = new Margins(20, 20, 20, 20);
            //pd.DefaultPageSettings.Margins = margin;
            ////纸张设置默认
            PaperSize pageSize = new PaperSize("First custom size", getYc(58), 600);
            printDocument.DefaultPageSettings.PaperSize = pageSize;
            printDocument.PrintPage += new PrintPageEventHandler(this.PrintPage);

            Initialize();
        }

        public virtual void Initialize()
        {
            log.Debug("begin");

            //string dllPath = Path.Combine(Config.AppRoot, dll);
            //ptr = Win32ApiInvoker.LoadLibrary(dllPath);

            //IntPtr api = Win32ApiInvoker.GetProcAddress(ptr, "GetPrinterStatus");
            //getPrinterStatusCaller = (GetPrinterStatusCaller)Marshal.GetDelegateForFunctionPointer(api, typeof(GetPrinterStatusCaller));
            //log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = GetPrinterStatus", ptr);

            log.Debug("end");
        }

        public virtual void Execute(int command)
        {
            log.DebugFormat("begin, args: command = {0}", command);

            //// 退纸
            //if (1 == command)
            //{
            //    int state = GetStatus();

            //    if (11 == state)
            //    {
            //        printData = emptyData;
            //        printDocument.Print();
            //    }
            //}

            log.Debug("end");
        }

        public void PrintAsync(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            if (isBusy)
            {
                jo["result"] = ErrorCode.Busy;
                RunCompletedEvent(this, new RunCompletedEventArgs(jo));
                log.DebugFormat("end, args: jo = {0}", jo);
                return;
            }

            isBusy = true;
            cancelled = false;
            printAsyncCaller.BeginInvoke(jo, new AsyncCallback(Callback), jo);

            log.Debug("end");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 0：禁用；1：正常；2：忙；3：离线；
        /// 10：空闲未装载纸；11：空闲已装载纸，准备就绪
        /// </returns>
        public virtual int GetStatus()
        {
            log.Debug("begin");

            if (!enabled)
            {
                log.DebugFormat("end, return = {0}", StatusCode.Disabled);
                return StatusCode.Disabled;
            }

            int result = StatusCode.Offline;
            IntPtr hPrinter;
            StructPrinterDefaults defaults = new StructPrinterDefaults();

            if (!OpenPrinter(name, out hPrinter, ref defaults))
            {
                log.InfoFormat("end, return = {0}", StatusCode.Offline);
                return StatusCode.Offline;
            }

            uint s = 0x00001000;
            int cbNeeded = 0;
            bool bolRet = GetPrinter(hPrinter, 2, IntPtr.Zero, 0, out cbNeeded);
            if (cbNeeded > 0)
            {
                IntPtr pAddr = Marshal.AllocHGlobal((int)cbNeeded);
                bolRet = GetPrinter(hPrinter, 2, pAddr, cbNeeded, out cbNeeded);
                if (bolRet)
                {
                    PrinterInfo2 Info2 = (PrinterInfo2)Marshal.PtrToStructure(pAddr, typeof(PrinterInfo2));
                    s = Info2.Status;
                }
                Marshal.FreeHGlobal(pAddr);
            }
            ClosePrinter(hPrinter);

            log.InfoFormat("s = {0}", s);

            if ((uint)PrinterStatus.PRINTER_STATUS_PAPER_OUT == (s & (uint)PrinterStatus.PRINTER_STATUS_PAPER_OUT)
                || (uint)PrinterStatus.PRINTER_STATUS_DOOR_OPEN == (s & (uint)PrinterStatus.PRINTER_STATUS_DOOR_OPEN)
                || ((uint)PrinterStatus.PRINTER_STATUS_PAPER_PROBLEM == (s & (uint)PrinterStatus.PRINTER_STATUS_PAPER_PROBLEM)))
            {
                result = 10;
            }
            else if ((uint)PrinterStatus.PRINTER_STATUS_PRINTING == (s & (uint)PrinterStatus.PRINTER_STATUS_PRINTING))
            {
                result = 2;
            }
            else if (0 == s)
            {
                result = 11;
            }
            else
            {
                result = 3;
            }

            log.InfoFormat("result = {0}", result);
            log.Debug("end");
            return result;
        }

        public string GetPrinterStatus(int status)
        {
            string strRet = string.Empty;
            switch (status)
            {
                case 0:
                    strRet = "准备就绪（Ready）";
                    break;
                case 0x00000200:
                    strRet = "忙(Busy）";
                    break;
                case 0x00400000:
                    strRet = "被打开（Printer Door Open）";
                    break;
                case 0x00000002:
                    strRet = "错误(Printer Error）";
                    break;
                case 0x0008000:
                    strRet = "初始化(Initializing）";
                    break;
                case 0x00000100:
                    strRet = "正在输入,输出（I/O Active）";
                    break;
                case 0x00000020:
                    strRet = "手工送纸（Manual Feed）";
                    break;
                case 0x00040000:
                    strRet = "无墨粉（No Toner）";
                    break;
                case 0x00001000:
                    strRet = "不可用（Not Available）";
                    break;
                case 0x00000080:
                    strRet = "脱机（Off Line）";
                    break;
                case 0x00200000:
                    strRet = "内存溢出（Out of Memory）";
                    break;
                case 0x00000800:
                    strRet = "输出口已满（Output Bin Full）";
                    break;
                case 0x00080000:
                    strRet = "当前页无法打印（Page Punt）";
                    break;
                case 0x00000008:
                    strRet = "塞纸（Paper Jam）";
                    break;
                case 0x00000010:
                    strRet = "打印纸用完（Paper Out）";
                    break;
                case 0x00000040:
                    strRet = "纸张问题（Page Problem）";
                    break;
                case 0x00000001:
                    strRet = "暂停（Paused）";
                    break;
                case 0x00000004:
                    strRet = "正在删除（Pending Deletion）";
                    break;
                case 0x00000400:
                    strRet = "正在打印（Printing）";
                    break;
                case 0x00004000:
                    strRet = "正在处理（Processing）";
                    break;
                case 0x00020000:
                    strRet = "墨粉不足（Toner Low）";
                    break;
                case 0x00100000:
                    strRet = "需要用户干预（User Intervention）";
                    break;
                case 0x20000000:
                    strRet = "等待（Waiting）";
                    break;
                case 0x00010000:
                    strRet = "热机中（Warming Up）";
                    break;
                default:
                    strRet = "未知状态（Unknown Status）";
                    break;
            }
            return strRet;
        }

        public void Dispose()
        {
            log.Debug("begin");

            //if (IntPtr.Zero != ptr)
            //{
            //    Win32ApiInvoker.FreeLibrary(ptr);
            //    log.DebugFormat("FreeLibrary: ptr = {0}", ptr);
            //}

            log.Debug("end");
        }

        public void Print(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            int status = GetStatus();

            // 未装载纸
            if (10 == status)
            {
                jo["result"] = 10;
                log.DebugFormat("end, args: jo = {0}", jo);
            }
            // 已装载纸
            else if (11 == status)
            {
                queue.Enqueue((JObject)jo["print"]);
                printDocument.Print();
                jo["result"] = ErrorCode.Success;
                WaitCompleted(jo);
            }
            else
            {
                jo["result"] = status;
            }

            log.DebugFormat("end, args: jo = {0}", jo);
        }

        protected virtual void WaitCompleted(JObject jo)
        {
            int status = 0;
            long start = DateTime.Now.Ticks;
            TimeSpan elapsedSpan = TimeSpan.Zero;

            while (true)
            {
                Thread.Sleep(100);

                status = GetStatus();

                if (2 != status)
                {
                    jo["result"] = ErrorCode.Success;
                    break;
                }

                if (TimeSpan.FromTicks(DateTime.Now.Ticks - start).TotalMilliseconds >= timeout)
                {
                    jo["result"] = ErrorCode.Timeout;
                    break;
                }
            }
        }

        private void Callback(IAsyncResult ar)
        {
            JObject jt = (JObject)ar.AsyncState;

            try
            {
                ((RunAsyncCaller)((AsyncResult)ar).AsyncDelegate).EndInvoke(ar);
            }
            catch (Exception e)
            {
                jt["result"] = ErrorCode.Failure;
                log.Error("ThermalPrinter.Callback Error", e);
            }
            finally
            {
                isBusy = false;
                jt["callback"] = "pintback";
                RunCompletedEvent(this, new RunCompletedEventArgs(jt));
            }
        }

        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            //打印文本
            //SetInvoiceData(ev.Graphics);
            //打印图文
            Bitmap image = QrCodeFactory.CreateQRCode("gfdfbvghf",1);
            GetPrintPicture(image, ev);

            #region 其他打印
            //JObject jo = null;

            //if (!queue.TryDequeue(out jo))
            //{
            //    return;
            //}

            //log.InfoFormat("print data : {0}", jo);

            //StringFormat f = new StringFormat();
            //f.Alignment = StringAlignment.Near;
            //f.LineAlignment = StringAlignment.Near;
            //ev.Graphics.PageUnit = GraphicsUnit.Millimeter;

            //int dpi = jo.Value<int>("dpi");
            //string dfamily = jo.Value<string>("family");
            //string dstyle = jo.Value<string>("style");
            //float dsize = jo.Value<float>("size");

            //float .l、 = 0;
            //float offsetY = 0;
            //JToken jtOffset = null;

            //if (jo.TryGetValue("offset", out jtOffset))
            //{
            //    offsetX = jtOffset.Value<float>("x");
            //    offsetY = jtOffset.Value<float>("y");
            //}

            //float x, y, px, py, width, height, w, h, size, s;
            //JArray items = (JArray)jo["items"];
            //string text, family, path, style, type;

            //foreach (JToken j in items)
            //{
            //    x = j.Value<float>("x");
            //    y = j.Value<float>("y");
            //    text = j.Value<string>("text");
            //    path = j.Value<string>("path");
            //    type = j.Value<string>("type");
            //    log.InfoFormat("type = {0}", type);

            //    if (!String.IsNullOrWhiteSpace(text))
            //    {
            //        family = j.Value<string>("family");
            //        style = j.Value<string>("style");
            //        size = j.Value<float>("size");

            //        family = String.IsNullOrWhiteSpace(family) ? dfamily : family;
            //        style = String.IsNullOrWhiteSpace(style) ? dstyle : style;
            //        size = (0 == size) ? dsize : size;

            //        s = size * 1.0f / dpi * 25.4f;
            //        px = x * 1.0f / dpi * 25.4f - offsetX;
            //        py = y * 1.0f / dpi * 25.4f - offsetY;

            //        //if ("counterNo" == type)
            //        //{
            //        //    string[] arr = text.Split(',');
            //        //    string str = String.Empty;
            //        //    string txt = String.Empty;
            //        //    SizeF sizef = SizeF.Empty;

            //        //    for (int i = 0;i < arr.Length; i ++)
            //        //    {
            //        //        str += " " + arr[i] + ",";
            //        //        sizef = ev.Graphics.MeasureString(str, font);

            //        //        if ((px + sizef.Width) > 70)
            //        //        {
            //        //            txt += str + "\n";
            //        //            str = String.Empty;
            //        //        }
            //        //    }

            //        //    if (!String.IsNullOrEmpty(str))
            //        //    {
            //        //        txt += str;
            //        //    }

            //        //    text = txt.Trim().TrimEnd(',');
            //        //    log.InfoFormat("counterNo print text : {0}", text);
            //        //}

            //        ev.Graphics.DrawString(text, new Font(family, s, GetFontStyle(style), GraphicsUnit.Millimeter), Brushes.Black, new RectangleF(px, py, (74 - px), 0), f);
            //    }
            //    else if (!String.IsNullOrWhiteSpace(path))
            //    {
            //        if (File.Exists(path))
            //        {
            //            px = x * 1.0f / dpi * 25.4f - offsetX;
            //            py = y * 1.0f / dpi * 25.4f - offsetY;
            //            width = j.Value<float>("width");
            //            height = j.Value<float>("height");
            //            w = width * 1.0f / dpi * 25.4f;
            //            h = height * 1.0f / dpi * 25.4f;

            //            ev.Graphics.DrawImage(Image.FromFile(path), px, py, w, h);
            //        }
            //    }
            //}

            //ev.Graphics.Dispose();
            //ev.HasMorePages = false;

            #endregion
        }
        private int getYc(double cm)
        {
            return (int)(cm / 25.4) * 100;
        }
        private void SetInvoiceData(Graphics g)
        {
            Font InvoiceFont = new Font("Arial", 8, FontStyle.Bold);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;//反锯齿
            SolidBrush GrayBrush = new SolidBrush(Color.Gray);
            g.DrawString(GetPrintStr(), InvoiceFont, GrayBrush, 5, 5);
            g.Dispose();
        }
        public string GetPrintStr()
        {

            StringBuilder sb = new StringBuilder();
            string tou = "XXXXX";
            string address = "XXXXXX";
            string address2 = "XXXXXX";
            string saleID = "20160001";
            string item = "项目";
            decimal price = 25.00M;
            int count = 5;
            decimal total = 0.00M;
            decimal fukuan = 500.00M;
            sb.Append("               " + tou + "            \n");
            sb.Append("-----------------------------------------------------------------\n");
            sb.Append("日期:" + DateTime.Now.ToShortDateString() + "  " + "   单号:" + saleID + "\n");
            sb.Append("会员号：13226416738\n");
            sb.Append("-----------------------------------------------------------------\n");

            sb.Append("项目" + "\t" + "数量" + "\t" + "单价" + "\t" + "小计" + "\n");
            for (int i = 0; i < count; i++)
            {
                decimal xiaoji = (i + 1) * price;
                sb.Append(item + (i + 1) + "\t" + (i + 1) + "\t" + price + "\t" + xiaoji);
                total += xiaoji;
                if (i != (count))
                    sb.Append("\n");

            }
            sb.Append("-----------------------------------------------------------------\n");
            sb.Append("数量: " + count + " 合计:   " + total + "\n");
            sb.Append("付款:现金" + " " + fukuan);
            sb.Append(" 现金找零:" + " " + (fukuan - total) + "\n");
            sb.Append("-----------------------------------------------------------------\n");
            sb.Append("地址：" + address + "\n");
            sb.Append(address2 + "\n");
            sb.Append("电话：13*******\n");
            sb.Append("        **谢谢惠顾欢迎下次光临**        ");
            return sb.ToString();

        }
        private FontStyle GetFontStyle(string style)
        {
            FontStyle fontStyle = FontStyle.Regular;

            switch (style)
            {
                case "italic":
                    fontStyle = FontStyle.Italic;
                    break;
                case "bold":
                    fontStyle = FontStyle.Bold;
                    break;
                case "boldAndItalic":
                    fontStyle = FontStyle.Bold | FontStyle.Italic;
                    break;
                case "normal":
                default:
                    fontStyle = FontStyle.Regular;
                    break;
            }

            return fontStyle;
        }
        public static void GetPrintPicture(Bitmap image, PrintPageEventArgs g)
        {
            int height = 5;
            Font font = new Font("宋体", 10f);
            Brush brush = new SolidBrush(Color.Black);
            g.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            int interval = 15;
            int pointX = 5;
            Rectangle destRect = new Rectangle(130, 10, image.Width, image.Height);
            g.Graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
            height += 8;
            RectangleF layoutRectangle = new RectangleF(pointX, height, 260f, 85f);
            g.Graphics.DrawString("资产编号:" + 11111, font, brush, layoutRectangle);

            height += interval;
            layoutRectangle = new RectangleF(pointX, height, 230f, 85f);
            g.Graphics.DrawString("资产名称:" + 22222, font, brush, layoutRectangle);

            height += interval;
            layoutRectangle = new RectangleF(pointX, height, 230f, 85f);
            g.Graphics.DrawString("类    别:" + 33333, font, brush, layoutRectangle);

            height += interval;
            layoutRectangle = new RectangleF(pointX, height, 230f, 85f);
            g.Graphics.DrawString("规格型号:" + 44444, font, brush, layoutRectangle);

            height += interval;
            layoutRectangle = new RectangleF(pointX, height, 230f, 85f);
            g.Graphics.DrawString("生产厂家:" + 55555, font, brush, layoutRectangle);


            height += interval;
            layoutRectangle = new RectangleF(pointX, height, 230f, 85f);
            g.Graphics.DrawString("启用时间:" + 66666, font, brush, layoutRectangle);

            height += interval;
            layoutRectangle = new RectangleF(pointX, height, 230f, 85f);
            g.Graphics.DrawString("资产价格:" + 77777, font, brush, layoutRectangle);

            height += interval;
            layoutRectangle = new RectangleF(pointX, height, 230f, 85f);
            g.Graphics.DrawString("保管单位:" + 88888, font, brush, layoutRectangle);

            //height += interval;
            layoutRectangle = new RectangleF(pointX + 150, height, 230f, 85f);
            g.Graphics.DrawString("保管人:" + 99999, font, brush, layoutRectangle);

            height += interval;
            layoutRectangle = new RectangleF(pointX, height, 230f, 85f);
            g.Graphics.DrawString("存放地点:" + 090909, font, brush, layoutRectangle);

            height += interval;
            layoutRectangle = new RectangleF(pointX, height, 240f, 85f);
            g.Graphics.DrawString("备    注:" + 080808, font, brush, layoutRectangle);

        }
    }
}