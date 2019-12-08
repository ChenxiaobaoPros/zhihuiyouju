using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Aoto.EMS.Infrastructure.Configuration
{
    public enum OperationCommand
    {
        None = 0,
        UsbUpdate = 60,
        Shutdown = 61,
        LogicalOn = 62,
        LogicalOff = 63,
        AppRestart = 64,
        Restart = 65,
        UpdateAfterRestart = 66,
        UploadLog = 67
    }

    public enum PageCommand
    {
        /// <summary>
        /// 增
        /// </summary>
        Add=31,
        /// <summary>
        /// 删
        /// </summary>
        Delete=32,
        /// <summary>
        /// 改
        /// </summary>
        Update=33,
        /// <summary>
        /// 查
        /// </summary>
        Select = 34,
        /// <summary>
        /// 
        /// </summary>
        Adjust = 35,
           /// <summary>
        /// 
        /// </summary>
        Kazhejudge = 36


    }

    public static class AppState
    {
        public static bool Online = Config.App.Online;
        public static int CurrentStyleId = 0;
        public static OperationCommand Command = OperationCommand.None;

        /// <summary>
        /// 打印状态
        /// </summary>
        public static string PrintStatus = "0000";
        /// <summary>
        /// 外设读卡开启状态， true-开启，false-关闭
        /// </summary>
        public static bool PerSign = false;
        //public static string WelcomeUrl = Path.Combine(Config.AppRoot, "web\\qms\\html\\admin\\index.html");
        //public static string MaintenanceUrl = Path.Combine(Config.AppRoot, "web\\maintenance.html");
        public static string WelcomeUrl = Path.Combine(Config.AppRoot, "duogong\\html\\index.html");

        static AppState()
        {

        }

        public static string ToJsonString()
        {
            return "{\"online\":" + Online.ToString().ToLower() + ",\"currentStyleId\":" + CurrentStyleId + "}";
        }
    }
}
