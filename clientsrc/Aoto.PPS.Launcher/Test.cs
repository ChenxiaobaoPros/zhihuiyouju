using System;
using System.Threading;
using Aoto.PPS.Core.Application;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.Configuration;
using Aoto.PPS.Peripheral;
using Aoto.PPS.QMS.Application;
using log4net;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Aoto.PPS.Launcher
{
    static class Test
    {
        static ITellerService tellerService;
        private static ILog log = LogManager.GetLogger(typeof(Test));
        static IAdminService adminService;
        static IBusTypeService busTypeService;
        static ISettingService settingService;
        static ITicketService ticketService;
        static ISystemService systemService;
        static IPrinter thermalPrinter;
        static PeripheralManager peripheralManager;

        [STAThread]
        static void Main(string[] args)
        {
            string dir = Config.App.Peripheral.Value<string>("dir");
            string libPath = Path.Combine(Config.PeripheralAbsolutePath, dir, "lib");
            log.InfoFormat("libDir = {0}", libPath);
            string dllPath = Path.Combine(Config.PeripheralAbsolutePath, dir);
            log.InfoFormat("dllDir = {0}", dllPath);

            string envPath = Environment.GetEnvironmentVariable("PATH") + ";" + dllPath + ";" + libPath;
            Environment.SetEnvironmentVariable("PATH", envPath);
            log.InfoFormat("EnvironmentVariable PATH = {0}", envPath);

            peripheralManager = AutofacContainer.ResolveNamed<PeripheralManager>("peripheralManager");
            peripheralManager.ICCardReaderWriter.Cancelled = false;

            JObject jo = new JObject();
            jo["timeout"] = 864000000;
            peripheralManager.ICCardReaderWriter.ReadAsync(jo);

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            //tellerService = AutofacContainer.ResolveNamed<ITellerService>("tellerService");
            //adminService = AutofacContainer.ResolveNamed<IAdminService>("adminService");
            //busTypeService = AutofacContainer.ResolveNamed<IBusTypeService>("busTypeService");
            //settingService = AutofacContainer.ResolveNamed<ISettingService>("settingService");
            //ticketService = AutofacContainer.ResolveNamed<ITicketService>("ticketService");
            //systemService = AutofacContainer.ResolveNamed<ISystemService>("systemService");
            //thermalPrinter = AutofacContainer.ResolveNamed<IPrinter>("thermalPrinter");

            //ticketService.AptcallTicket(jo);

            //ticketService.TransferTicket(jo);

            //ticketService.Signin(jo);
   /*
            JObject jo = new JObject();
            JArray jcerts = new JArray();
            JObject joCert = new JObject();
            joCert["certNo"] = "32010319811029101X";
            joCert["certTypeId"] = 1;

            jcerts.Add(joCert);

            JObject jtkt = new JObject();
            jtkt["buzTypeId"] = 1;
            jo["ticket"] = jtkt;

            JObject joCustomer = new JObject();
            joCustomer["certs"] = jcerts;
            jo["customer"] = joCustomer;

            ticketService.GetTicket(jo);
 */

            //int a = settingService.GetDateTypeIdByToday();
            //TestLogin();

            //ISchedulerFactory sf = new StdSchedulerFactory();
            //IScheduler sched = sf.GetScheduler();

            //sched.Start();

            //IScriptInvoker scriptInvoker = AutofacContainer.ResolveNamed<IScriptInvoker>("scriptInvoker");
            //ISystemService systemService = AutofacContainer.ResolveNamed<ISystemService>("systemService");

            //TestGetBusTypes();


            Thread.Sleep(300000);

            //sched.Shutdown(true);
            //IReader m = AutofacContainer.ResolveNamed<IReader>("magneticCardReaderWriter");
            //IReader idCardReader = AutofacContainer.ResolveNamed<IReader>("idCardReader");
            //IReader icCardReader = AutofacContainer.ResolveNamed<IReader>("icCardReaderWriter");
            //IIndicateorLight light = AutofacContainer.ResolveNamed<IIndicateorLight>("indicateorLight");
            //IReader t = AutofacContainer.ResolveNamed<IReader>("twoDimensionalCodeScanner");
            //IReader s = AutofacContainer.ResolveNamed<IReader>("suctionCardReaderWriter");
            //IPrinter needlePrinter = AutofacContainer.ResolveNamed<IPrinter>("needlePrinter");
            //PeripheralManager peripheralManager = AutofacContainer.ResolveNamed<PeripheralManager>("peripheralManager");

            //TestCreateTeller();
            //TestGetTeller();
            //TestGetTellers();
            //TestRomveTeller();
        }

        static void TestGetBusTypes()
        {
            JObject jo = new JObject();
            busTypeService.GetBusTypes(jo);
        }

        static void TestGetBusType()
        {
            JObject jo = new JObject();
            JObject busType = new JObject();
            jo["busType"] = busType;
            busType["busTypeId"] = 1;
            busTypeService.GetBusType(jo);
        }

        static void TestLogin()
        {
            JObject jo = new JObject();
            JObject admin = new JObject();
            jo["admin"] = admin;
            admin["username"] = "admin";
            admin["password"] = "123456";
            adminService.Login(jo);
        }

        static void TestGetTellers()
        {
            JObject jo = new JObject();
            JObject tel = new JObject();
            jo["teller"] = tel;

            tellerService.GetTellers(jo);
        }

        static void TestGetTeller()
        {
            JObject jo = new JObject();
            JObject tel = new JObject();
            jo["teller"] = tel;

            tel["rowId"] = 1;
            tellerService.GetTeller(jo);
        }

        static void TestCreateTeller()
        {
            JObject jo = new JObject();
            JObject tel = new JObject();
            jo["teller"] = tel;

            //tel["rowId"] = 10;
            tel["telNo"] = "0003";
            tel["telName"] = "张山";
            tel["pwd"] = "0001";
            tel["gender"] = 1;
            tel["phone"] = "13770548366";
            tel["email"] = "zs@szaoto.aoto.com.cn";
            tel["photoPath"] = "a/b/c/d.jpg";
            tel["starLevel"] = 5;
            tel["left"] = false;
            tellerService.CreateTeller(jo);
        }

        static void TestUpdateTeller()
        {
            JObject jo = new JObject();
            JObject tel = new JObject();
            jo["teller"] = tel;

            tel["rowId"] = 1;
            tel["telNo"] = "0002";
            tel["telName"] = "张山2";
            tel["pwd"] = "00012";
            tel["gender"] = 2;
            tel["phone"] = "13770548367";
            tel["email"] = "ddd@szaoto.aoto.com.cn";
            tel["photoPath"] = "dda/b/c/d.jpg";
            tel["starLevel"] = 4;
            tel["left"] = true;
            tellerService.UpdateTeller(jo);
        }

        static void TestRomveTeller()
        {
            JObject jo = new JObject();
            JObject tel = new JObject();
            jo["teller"] = tel;

            tel["rowId"] = 1;
            tellerService.RemoveTeller(jo);
        }
    }
}
