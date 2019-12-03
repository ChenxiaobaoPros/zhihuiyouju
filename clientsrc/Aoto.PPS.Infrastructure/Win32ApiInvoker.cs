using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Aoto.PPS.Infrastructure.Utils;

namespace Aoto.PPS.Infrastructure
{
    public class Win32ApiInvoker
    {
        private Win32ApiInvoker()
        {

        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        #region 开关机重启

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TokPriv1Luid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }

        public const int SE_PRIVILEGE_ENABLED = 2;
        public const int TOKEN_QUERY = 8;
        public const int TOKEN_ADJUST_PRIVILEGES = 32;
        public const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        public const int EWX_LOGOFF = 0;
        public const int EWX_SHUTDOWN = 1;
        public const int EWX_REBOOT = 2;
        public const int EWX_FORCE = 4;
        public const int EWX_POWEROFF = 8;
        public const int EWX_FORCEIFHUNG = 16;
        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GetCurrentProcess();
        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);
        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool ExitWindowsEx(int flg, int rea);
        public static void DoExitWin(int flg)
        {
            IntPtr currentProcess = GetCurrentProcess();
            IntPtr zero = IntPtr.Zero;
            bool flag = OpenProcessToken(currentProcess, 40, ref zero);
            TokPriv1Luid tokPriv1Luid;
            tokPriv1Luid.Count = 1;
            tokPriv1Luid.Luid = 0L;
            tokPriv1Luid.Attr = 2;
            flag = LookupPrivilegeValue(null, "SeShutdownPrivilege", ref tokPriv1Luid.Luid);
            flag = AdjustTokenPrivileges(zero, false, ref tokPriv1Luid, 0, IntPtr.Zero, IntPtr.Zero);
            flag = ExitWindowsEx(flg, 0);
        }

        #endregion

        [DllImport("kernel32.dll")]
        public extern static IntPtr LoadLibrary(string path);

        [DllImport("kernel32.dll")]
        public extern static IntPtr GetProcAddress(IntPtr lib, string funcName);

        [DllImport("kernel32.dll")]
        public extern static bool FreeLibrary(IntPtr lib);

        [StructLayout(LayoutKind.Sequential)]
        public struct LASTINPUTINFO
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwTime;
        }

        [DllImport("user32.dll")]
        public static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("Kernel32.dll")]
        public static extern bool SetLocalTime(ref SystemTime sysTime);

        [StructLayout(LayoutKind.Sequential)]
        public struct SystemTime
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMiliseconds;
        }
    }
}
