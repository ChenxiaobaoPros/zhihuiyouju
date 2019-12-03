using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.ComponentModel;
using Aoto.PPS.Infrastructure.Configuration;
using log4net;
using Newtonsoft.Json.Linq;

namespace Aoto.PPS.Peripheral.Default
{
    public class NeedlePrinter : IPrinter
    {
        /*
        private enum StatusCode
        {
            [Description("Error status")]
            ErrorStatus = 0,
            [Description("Self-printing mode")]
            SelfprintingMode = 1,
            [Description("Busy")]
            Busy = 2,
            [Description("Wait")]
            Wait = 3,
            [Description("空闲")]//Idle
            Idle = 4,
            [Description("Not ready to print")]
            NotReadyToPrint = 5,
            [Description("Ink drying")]
            InkDrying = 6,
            [Description("Cleaning")]
            Cleaning = 7,
            [Description("Factory shipping status")]
            FactoryShippingStatus = 8,
            [Description("Motor energization")]
            MotorEnergization = 9,
            [Description("Shutting down")]
            ShuttingDown = 10,
            [Description("Waiting for paper initialization start trigger")]
            WaitingForPaperInitializationStartTrigger = 11,
            [Description("Paper initializing")]
            PaperInitializing = 12,
            [Description("Converting Black ink type")]
            ConvertingBlackInkType = 13,
            [Description("Waiting for the heater to adjust to the setting temperature")]
            WaitingForTheHeaterToAdjustToTheSettingTemperature = 14,
            [Description("Initializing printer startup")]
            InitializingPrinterStartup = 15,
        }

        private enum ErrCode
        {
            [Description("Fatal error")]
            FatalError = 0x00,
            [Description("Interface ot selected")]
            InterfaceNotSelected = 0x01,
            [Description("Cover open")]
            CoverOpen = 0x02,
            [Description("Release lever position error")]
            ReleaseLeverPositionError = 0x03,
            [Description("Paper jam")]
            PaperJam = 0x04,
            [Description("Out of ink (*1)")]
            OutOfInk = 0x05,
            [Description("未装载纸")]//Out of paper
            OutOfPaper = 0x06,
            [Description("Initial setting")]
            InitialSetting = 0x07,
            [Description("Unknown error")]
            UnknownError = 0x08,
            [Description("Paper change unfinished error")]
            PaperChangeUnfinishedError = 0x09,
            [Description("Paper size error (SIDM)")]
            PaperSizeError = 0x0A,
            [Description("Ribbon error")]
            RibbonError = 0x0B,
            [Description("Paper size/type/path error (SNIJ)")]
            PaperSizeOrTypeOrPathError = 0x0C,
            [Description("Paper thickness lever position error")]
            PaperRhicknessLeverPositionError = 0x0D,
            [Description("Paper eject error")]
            PaperEjectError = 0x0E,
            [Description("SIMM copy error")]
            SIMMCopyError = 0x0F,
            [Description("Maintenance tank full (Left) (*2)")]
            MaintenanceTankFull = 0x10,
            [Description("Waiting for reset during tear-off")]
            WaitingForResetDuringTearOff = 0x11,
            [Description("Duplicate feed error")]
            DuplicateFeedError = 0x12,
            [Description("Head hot error")]
            HeadHotError = 0x13,
            [Description("Paper cut error")]
            PaperCutError = 0x14,
            [Description("Presser lever release error")]
            PresserLeverReleaseError = 0x15,
            [Description("Cleaning impossible error")]
            CleaningImpossibleError = 0x16,
            [Description("Paper recognition error")]
            PaperRecognitionError = 0x17,
            [Description("Paper skewed-Reload paper error")]
            PaperSkewedReloadPaperError = 0x18,
            [Description("Cleaning count overrun error")]
            CleaningCountOverrunError = 0x19,
            [Description("Ink cover open")]
            InkCoverOpen = 0x1A,
            [Description("Ink cartridge error (LFP)")]
            InkCartridgeError = 0x1B,
            [Description("Cutter error (Fatal Error)")]
            CutterError = 0x1C,
            [Description("Cutter jam error (Recoverable)")]
            CutterJamError = 0x1D,
            [Description("Ink color error")]
            InkColorError = 0x1E,
            [Description("Cutter cover open error")]
            CutterCoverOpenError = 0x1F,
            [Description("Ink lever release")]
            InkLeverRelease = 0x20,
            [Description("Maintenance tank out")]
            MaintenanceTankOut = 0x22,
            [Description("Ink cartridge combination error")]
            InkCartridgeCombinationError = 0x23,
            [Description("Command error (LFP)")]
            CommandError = 0x24,
            [Description("Rear cover open error")]
            RearCoverOpenError = 0x25,
            [Description("Multi-sensor gain error")]
            MultisensorGainError = 0x26,
            [Description("Automatic adjustment impossible error")]
            AutomaticAdjustmentImpossibleError = 0x27,
            [Description("Cleaning failure error")]
            CleaningFailureError = 0x28,
            [Description("No paper tray error")]
            NoPaperTrayError = 0x29,
            [Description("Maintenance tank out (Right)")]
            MaintenanceTankOutRight = 0x2D,
            [Description("Maintenance tank full (Right) (*2)")]
            MaintenanceTankFullRight = 0x2E,
            [Description("Maintenance cartridge cover open error")]
            MaintenanceCartridgeCoverOpenError = 0x36,
            [Description("Maintenance tank full (Center) (*2)")]
            MaintenanceTankFullCenter = 0x39,
            [Description("Maintenance tank out (Center)")]
            MaintenanceTankOutCenter = 0x3A,
            [Description("Cannot open ink cover")]
            CannotOpenInkCover = 0x3B,
            [Description("Not enough maintenance ink capacity")]
            NotEnoughMaintenanceInkCapacity = 0x3C,
            [Description("Not enough ink")]
            NotEnoughInk = 0x3D,
            [Description("TakeUpError")]
            TakeUpError = 0x3E,
            [Description("Remove roll pape")]
            RemoveRollPaper = 0x3F,
            [Description("Maintenance cover open error for service personnel")]
            MaintenanceCoverOpenErrorForServicePersonnel = 0x40,
            [Description("Maintenance error (tube at end of life)")]
            MaintenanceError = 0x41,
            [Description("ACCEL error")]
            ACCELError = 0x42
        }
        */

        //0			获得打印机状态成功
        //1 		Initialize EpsonNetSDK error
        //2 		Can not get printer ID or the printer is no inline
        //3 		Can not open printer handle
        //其他值	Other error
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        private delegate int GetPrinterStatusCaller(int type, string name, ref int statusCode, ref int errorCode);

        private static readonly ILog log = LogManager.GetLogger("printer"); 
        protected PrintDocument printDocument;
        protected RunAsyncCaller printAsyncCaller;
        protected JObject printData;
        protected JObject emptyData;
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

        private GetPrinterStatusCaller getPrinterStatusCaller;

        public NeedlePrinter()
        {
            isBusy = false;
            cancelled = false;

            this.dll = Config.App.Peripheral["needlePrinter"].Value<string>("dll");
            this.timeout = Config.App.Peripheral["needlePrinter"].Value<int>("timeout");
            this.enabled = Config.App.Peripheral["needlePrinter"].Value<bool>("enabled");
            this.name = Config.App.Peripheral["needlePrinter"].Value<string>("name");
            emptyData = JObject.Parse("{\"family\": \"sans-serif\",\"size\": 24,\"dpi\": 200,\"offset\": {\"x\": 1,\"y\": 1},\"items\": [{\"id\": \"receiptNo\",\"text\": \"\",\"x\": 0,\"y\": 0}]}");

            printAsyncCaller = new RunAsyncCaller(Print);

            printDocument = new PrintDocument();
            printDocument.PrintController = new StandardPrintController();
            printDocument.PrinterSettings.PrinterName = name;
            printDocument.PrinterSettings.DefaultPageSettings.Margins.Left = 0;
            printDocument.PrinterSettings.DefaultPageSettings.Margins.Top = 0;
            printDocument.PrintPage += new PrintPageEventHandler(this.PrintPage);

            Initialize();
        }

        public virtual void Initialize()
        {
            log.Debug("begin");

            if (!enabled)
            {
                return;
            }

            string dllPath = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, dll);

            if (!File.Exists(dllPath))
            {
                dllPath = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, "lib", dll);
            }

            ptr = Win32ApiInvoker.LoadLibrary(dllPath);
            log.InfoFormat("LoadLibrary: dllPath = {0}, ptr = {1}", dllPath, ptr);

            IntPtr api = Win32ApiInvoker.GetProcAddress(ptr, "GetPrinterStatus");
            getPrinterStatusCaller = (GetPrinterStatusCaller)Marshal.GetDelegateForFunctionPointer(api, typeof(GetPrinterStatusCaller));
            log.DebugFormat("GetProcAddress: ptr = {0}, entryPoint = GetPrinterStatus", ptr);

            log.Debug("end");
        }

        public virtual void Execute(int command)
        {
            log.DebugFormat("begin, args: command = {0}", command);

            // 退纸
            if (1 == command)
            {
                int state = GetStatus();

                if (11 == state)
                {
                    printData = emptyData;
                    printDocument.Print();
                }
            }

            log.Debug("end");
        }

        public void PrintAsync(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            if (!enabled)
            {
                return;
            }

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
                log.InfoFormat("end, return = {0}", StatusCode.Disabled);
                return StatusCode.Disabled;
            }

            int statusCode = -1;
            int errorCode = -1;
            string error = String.Empty;

            int s = getPrinterStatusCaller(1, name, ref statusCode, ref errorCode);
            log.InfoFormat("invoke {0} -> GetPrinterStatus, args: type = {1}, name = {2}, statusCode = {3}, errorCode = {4}, return = {5}",
                dll, 1, name, statusCode, errorCode, s);

            int result = -1;

            if (0 == s)
            {
                if (0 == statusCode)
                {
                    if (6 == errorCode)
                    {
                        result = 10;
                    }
                }
                else if (4 == statusCode)
                {
                    result = 11;
                }
                else if (2 == statusCode || 3 == statusCode)
                {
                    result = StatusCode.Busy;
                }
                else
                {
                    result = StatusCode.Normal;
                }
            }
            else
            {
                result = StatusCode.Offline;
            }

            log.DebugFormat("end, return = {0}", result);
            return result;
        }

        public void Dispose()
        {
            log.Debug("begin");

            if (!enabled)
            {
                return;
            }

            if (IntPtr.Zero != ptr)
            {
                Win32ApiInvoker.FreeLibrary(ptr);
                log.DebugFormat("FreeLibrary: ptr = {0}", ptr);
            }

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
                printData = (JObject)jo["print"];
                printDocument.Print();
                long start = DateTime.Now.Ticks;

                while (true)
                {
                    Thread.Sleep(500);

                    status = GetStatus();
                    log.InfoFormat("status = {0}", status);

                    if (10 == status)
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
            else
            {
                jo["result"] = ErrorCode.Failure;
            }

            log.DebugFormat("end, args: jo = {0}", jo);
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
                log.Error("Error", e);
            }
            finally
            {
                isBusy = false;
                RunCompletedEvent(this, new RunCompletedEventArgs(jt));
            }
        }

        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            JObject jo = (JObject)printData;

            ev.Graphics.PageUnit = GraphicsUnit.Millimeter;
            StringFormat f = new StringFormat();
            f.Alignment = StringAlignment.Near;
            f.LineAlignment = StringAlignment.Near;
            
            string defaultFamily = jo.Value<string>("family");
            int defaultSize = jo.Value<int>("size");
            int dpi = jo.Value<int>("dpi");
            bool landscape = jo.Value<bool>("landscape");
            int height = 0;

            if (landscape)
            {
                height = jo["image"].Value<int>("height");

                Matrix matrix = new Matrix();
                matrix.Rotate(90);
                ev.Graphics.Transform = matrix;
            }

            float offsetX = 0;
            float offsetY = 0;
            JToken jt = null;

            if (jo.TryGetValue("offset1", out jt))
            {
                offsetX = jt.Value<float>("x");
                offsetY = jt.Value<float>("y");
            }
            else if (jo.TryGetValue("offset2", out jt))
            {
                offsetX = jt.Value<float>("x");
                offsetY = jt.Value<float>("y");
            }
            else if (jo.TryGetValue("offset3", out jt))
            {
                offsetX = jt.Value<float>("x");
                offsetY = jt.Value<float>("y");
            }
            else if (jo.TryGetValue("offset", out jt))
            {
                offsetX = jt.Value<float>("x");
                offsetY = jt.Value<float>("y");
            }
            else if (jo.TryGetValue("backOffset", out jt))
            {
                offsetX = jt.Value<float>("x");
                offsetY = jt.Value<float>("y");
            }

            int x = 0;
            int y = 0;
            string text = String.Empty;
            string family = String.Empty;
            int size = 0;
            int width = 0;

            float px, py, w;
            JArray items = null;

            if (jo.TryGetValue("items1", out jt))
            {
                items = (JArray)jt;
            }
            else if (jo.TryGetValue("items2", out jt))
            {
                items = (JArray)jt;
            }
            else if (jo.TryGetValue("items3", out jt))
            {
                items = (JArray)jt;
            }
            else if (jo.TryGetValue("items", out jt))
            {
                items = (JArray)jt;
            }
            else if (jo.TryGetValue("backItems", out jt))
            {
                items = (JArray)jt;
            }

            bool disabled = false;

            foreach (JObject j in items)
            {
                disabled = j.Value<bool>("disabled");
                text = j.Value<string>("text");

                if (disabled || String.IsNullOrWhiteSpace(text))
                {
                    continue;
                }

                x = j.Value<int>("x");
                y = j.Value<int>("y");
                
                family = j.Value<string>("family");
                size = j.Value<int>("size");
                width = j.Value<int>("width");

                family = String.IsNullOrEmpty(family) ? defaultFamily : family;
                size = (0 == size) ? defaultSize : size;
                w = (0 == width) ? 0 : (width * 1.0f / dpi * 25.4f + 2);

                px = x * 1.0f / dpi * 25.4f - offsetX;

                if (!landscape)
                {
                    py = y * 1.0f / dpi * 25.4f - offsetY;
                }
                else
                {
                    py = (y - height) * 1.0f / dpi * 25.4f - offsetY;
                }

                ev.Graphics.DrawString(text, new Font(family, size * 1.0f / dpi * 25.4f, FontStyle.Regular, GraphicsUnit.Millimeter),
                    Brushes.Black, new RectangleF(px, py, w, 0), f);
            }

            ev.HasMorePages = false;
        }
    }
}