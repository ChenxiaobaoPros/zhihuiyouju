using System;
using System.Collections.Generic;
using System.Threading;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.ComponentModel;
using Aoto.PPS.Infrastructure.Configuration;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Aoto.PPS.Peripheral
{
    public interface IVoicePlayer : IDisposable
    {
        void Play(string wav);

        void PlayAsync(string xml);

        void PlayAsync(IList<string> list);
    }

    public interface IPeripheral : IDisposable
    {
        bool IsBusy { get; }
        bool Cancelled { get; set; }
        bool Enabled { get; }
        int GetStatus();
        void Initialize();
        event RunCompletedEventHandler RunCompletedEvent;
    }

    public interface ICaller : IReader, IWriter
    {

    }

    public interface IEvaluator : IReader, IWriter
    {

    }
    
    public interface IReader : IPeripheral
    {
        void Read(JObject jo);
        void ReadAsync(JObject jo);
    }

    public interface IWriter : IPeripheral
    {
        void Write(JObject jo);
        void WriteAsync(JObject jo);
    }

    public interface IPrinter : IPeripheral
    {
        void Print(JObject jo);
        void PrintAsync(JObject jo);
        void Execute(int command);
    }

    public class PeripheralManager
    {
        private string peripheralStatus;
        private IReader magneticCardReaderWriter;
        private IReader icCardReaderWriter;
        private IReader idCardReader;
        private IPrinter needlePrinter;
        private IPrinter thermalPrinter;
        private IScriptInvoker scriptInvoker;
        private ICaller caller;
        private IEvaluator evaluator;
        private IWriter barScreen;
        private IWriter compScreen;
        private IVoicePlayer voicePlayer;

       // private IReader mifareCardReader;

        private static readonly ILog log = LogManager.GetLogger("peripheral");
        public static readonly string Dir = Config.App.Peripheral.Value<string>("dir");

        public PeripheralManager()
        {
            scriptInvoker = AutofacContainer.ResolveNamed<IScriptInvoker>("scriptInvoker");
            voicePlayer = AutofacContainer.ResolveNamed<IVoicePlayer>("voicePlayer");
            magneticCardReaderWriter = AutofacContainer.ResolveNamed<IReader>("magneticCardReaderWriter");
            icCardReaderWriter = AutofacContainer.ResolveNamed<IReader>("icCardReaderWriter");
            idCardReader = AutofacContainer.ResolveNamed<IReader>("idCardReader");
            needlePrinter = AutofacContainer.ResolveNamed<IPrinter>("needlePrinter");
            thermalPrinter = AutofacContainer.ResolveNamed<IPrinter>("thermalPrinter");
            evaluator = AutofacContainer.ResolveNamed<IEvaluator>("evaluator");
            barScreen = AutofacContainer.ResolveNamed<IWriter>("barScreen");
            compScreen = AutofacContainer.ResolveNamed<IWriter>("compScreen");
            caller = AutofacContainer.ResolveNamed<ICaller>("caller");

            //mifareCardReader = AutofacContainer.ResolveNamed<IReader>("mifareCardReader");

            magneticCardReaderWriter.RunCompletedEvent += new RunCompletedEventHandler(ReadCardCompletedEvent);
            icCardReaderWriter.RunCompletedEvent += new RunCompletedEventHandler(ReadCardCompletedEvent);
            idCardReader.RunCompletedEvent += new RunCompletedEventHandler(ReadCardCompletedEvent);
            needlePrinter.RunCompletedEvent += new RunCompletedEventHandler(PrintCompletedEvent);

            //mifareCardReader.RunCompletedEvent += new RunCompletedEventHandler(ReadCardCompletedEvent);

        }



        public string PeripheralStatus { get { return peripheralStatus; } set { peripheralStatus = value; } }
        public IReader MagneticCardReaderWriter { get { return magneticCardReaderWriter; } }
        public IReader ICCardReaderWriter { get { return icCardReaderWriter; } }
        public IReader IDCardReader { get { return idCardReader; } }
        public IPrinter NeedlePrinter { get { return needlePrinter; } }
        public ICaller Caller { get { return caller; } }
        public IWriter BarScreen { get { return barScreen; } }
        public IWriter CompScreen { get { return compScreen; } }
        public IVoicePlayer VoicePlayer { get { return voicePlayer; } }
        public IPrinter ThermalPrinter { get { return thermalPrinter; } }
        public IEvaluator Evaluator { get { return evaluator; } }

        private static object lockedObject = new object();
        private static string res = String.Empty;

        private void ReadCardCompletedEvent(object sender, RunCompletedEventArgs e)
        {
            lock (lockedObject)
            {
                JObject jo = (JObject)e.Result;
                int type = jo.Value<int>("type");
                int allCompleted = jo.Value<int>("allCompleted");
                int result = jo.Value<int>("result");

                log.DebugFormat("jo = {0}", jo);

                if (magneticCardReaderWriter == sender)
                {
                    jo["allCompleted"] = allCompleted |= 1;

                    if (ErrorCode.Success == result || ErrorCode.Failure == result)
                    {
                        icCardReaderWriter.Cancelled = true;
                        idCardReader.Cancelled = true;

                        if (ErrorCode.Success == result)
                        {
                            res = jo.ToString(Formatting.None);
                        }
                    }
                }
                else if (icCardReaderWriter == sender)
                {
                    jo["allCompleted"] = allCompleted |= 2;

                    if (ErrorCode.Success == result || ErrorCode.Failure == result)
                    {
                        magneticCardReaderWriter.Cancelled = true;
                        idCardReader.Cancelled = true;

                        if (ErrorCode.Success == result)
                        {
                            res = jo.ToString(Formatting.None);
                        }
                    }
                }
                else if (idCardReader == sender)
                {
                    jo["allCompleted"] = allCompleted |= 4;

                    if (ErrorCode.Success == result || ErrorCode.Failure == result)
                    {
                        magneticCardReaderWriter.Cancelled = true;
                        icCardReaderWriter.Cancelled = true;

                        if (ErrorCode.Success == result)
                        {
                            res = jo.ToString(Formatting.None);
                        }
                    }
                }

                log.DebugFormat("type = {0}", type);
                log.DebugFormat("allCompleted = {0}", allCompleted);
                log.DebugFormat("res = {0}", res);

                if (type == allCompleted)
                {
                    if (String.IsNullOrEmpty(res))
                    {
                        if (ErrorCode.Success == result || ErrorCode.Failure == result || ErrorCode.Timeout == result)
                        {
                            scriptInvoker.ScriptInvoke(jo);
                            log.DebugFormat("ScriptInvoke jo = {0}", jo);
                        }
                        else
                        {
                            log.DebugFormat("no ScriptInvoke");
                        }
                    }
                    else
                    {
                        scriptInvoker.ScriptInvoke(JObject.Parse(res));
                        log.DebugFormat("ScriptInvoke jo = {0}", res);
                        res = String.Empty;
                    }
                }
            }
        }

        private void PrintCompletedEvent(object sender, RunCompletedEventArgs e)
        {
            scriptInvoker.ScriptInvoke((JObject)e.Result);
        }

        public string GetPeripheralStatus()
        {
            string p = String.Empty;
            int state = idCardReader.GetStatus();
            p += state;

            state = magneticCardReaderWriter.GetStatus();
            p += state;

            // 吸卡器
            p += "3";

            //p += "3";
            state = icCardReaderWriter.GetStatus();
            p += state;

            // 针式打印机
            state = needlePrinter.GetStatus();

            if (10 == state || 11 == state)
            {
                p += "1";
            }
            else
            {
                p += state;
            }

            // 号票打印机
            state = thermalPrinter.GetStatus();

            if (10 == state || 11 == state)
            {
                p += "1";
            }
            else
            {
                p += state;
            }
            
            // 二维码扫描仪、密码键盘、摄像头
            p = p + "000";
            peripheralStatus = p;
            return p;
        }

        public void SwipeCard(JObject jo)
        {
            int type = jo.Value<int>("type");

            // 同时刷磁条卡，和IC卡，身份证兼容处理
            switch (type)
            {
                // 001：磁条卡启用
                case 1:
                    magneticCardReaderWriter.Cancelled = false;
                    magneticCardReaderWriter.ReadAsync(jo);
                    break;

                // 010：IC卡启用
                case 2:
                    icCardReaderWriter.Cancelled = false;
                    icCardReaderWriter.ReadAsync(jo);
                    break;

                // 011：磁条卡、IC卡启用
                case 3:
                    magneticCardReaderWriter.Cancelled = false;
                    icCardReaderWriter.Cancelled = false;
                    magneticCardReaderWriter.ReadAsync(jo);
                    icCardReaderWriter.ReadAsync(jo);
                    break;

                // 100：身份证启用
                case 4:
                    idCardReader.Cancelled = false;
                    idCardReader.ReadAsync(jo);
                    break;

                // 101：身份证、磁条卡启用
                case 5:
                    magneticCardReaderWriter.Cancelled = false;
                    idCardReader.Cancelled = false;
                    magneticCardReaderWriter.ReadAsync(jo);
                    idCardReader.ReadAsync(jo);
                    break;

                // 110：IC卡启用、身份证启用
                case 6:
                    icCardReaderWriter.Cancelled = false;
                    idCardReader.Cancelled = false;
                    icCardReaderWriter.ReadAsync(jo);
                    idCardReader.ReadAsync(jo);
                    break;

                // 111：IC卡启用、身份证、磁条卡启用
                case 7:
                    magneticCardReaderWriter.Cancelled = false;
                    icCardReaderWriter.Cancelled = false;
                    idCardReader.Cancelled = false;
                    magneticCardReaderWriter.ReadAsync(jo);
                    icCardReaderWriter.ReadAsync(jo);
                    idCardReader.ReadAsync(jo);
                    break;

                default:
                    break;
            }
        }



        public void Cancel(JObject jo)
        {
            bool isBusy = true;

            do
            {
                idCardReader.Cancelled = true;
                icCardReaderWriter.Cancelled = true;
                magneticCardReaderWriter.Cancelled = true;
                log.Info("set idCardReader, icCardReaderWriter, magneticCardReaderWriter Cancelled = true");

                isBusy = idCardReader.IsBusy || icCardReaderWriter.IsBusy || magneticCardReaderWriter.IsBusy;
                log.InfoFormat("idCardReader.IsBusy = {0}, icCardReaderWriter.IsBusy = {0}, magneticCardReaderWriter.IsBusy = {0}", idCardReader.IsBusy, icCardReaderWriter.IsBusy, magneticCardReaderWriter.IsBusy);
                Thread.Sleep(200);
            }
            while (isBusy);
            log.Info("wait idCardReader, icCardReaderWriter, magneticCardReaderWriter exit loop ok");
        }

        public void Dispose()
        {
            bool isBusy = true;

            do
            {
                idCardReader.Cancelled = true;
                icCardReaderWriter.Cancelled = true;
                magneticCardReaderWriter.Cancelled = true;
                caller.Cancelled = true;
                evaluator.Cancelled = true;
                log.Info("set idCardReader, icCardReaderWriter, magneticCardReaderWriter caller, evaluator Cancelled = true");

                isBusy = idCardReader.IsBusy || icCardReaderWriter.IsBusy || magneticCardReaderWriter.IsBusy || caller.IsBusy || evaluator.IsBusy;
                Thread.Sleep(200);
            }
            while (isBusy);
            log.Info("wait idCardReader, icCardReaderWriter, magneticCardReaderWriter caller, evaluator exit loop ok");

            magneticCardReaderWriter.Dispose();
            icCardReaderWriter.Dispose();
            idCardReader.Dispose();
            needlePrinter.Dispose();
            barScreen.Dispose();
            compScreen.Dispose();
            caller.Dispose();
            //voicePlayer.Dispose();

            Thread.Sleep(2000);
        }
    }
}
