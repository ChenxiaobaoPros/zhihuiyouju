using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace Aoto.EMS.Infrastructure.Configuration
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public class DeviceInfo
    {
        /// <summary>
        /// 设备CpuID
        /// </summary>
        public string CpuID;
        /// <summary>
        /// 设备网卡信息
        /// </summary>
        public string MacAddress;
        /// <summary>
        /// 硬盘ID
        /// </summary>
        public string DiskID;
        /// <summary>
        /// 设备IP
        /// </summary>
        //public string IpAddress;
        /// <summary>
        /// 登陆用户名
        /// </summary>
        //public string LoginUserName;
        /// <summary>
        /// 计算机名
        /// </summary>
        //public string ComputerName;
        /// <summary>
        /// 设备系统类型
        /// </summary>
        public string SystemType;
        /// <summary>
        /// 物理内存
        /// </summary>
        //public string TotalPhysicalMemory; //单位：M

        private static DeviceInfo _instance;

        public static DeviceInfo Instance()
        {
            if (_instance == null)
                _instance = new DeviceInfo();
            return _instance;
        }

        protected DeviceInfo()
        {
            CpuID = GetCpuID();
            MacAddress = GetMacAddress();
            DiskID = GetDiskID();
            //IpAddress = GetIPAddress();
            //LoginUserName = GetUserName();
            SystemType = GetSystemType();
            //TotalPhysicalMemory = GetTotalPhysicalMemory();
            //ComputerName = GetComputerName();
        }

        /// <summary>
        /// 获取设备cpuID
        /// </summary>
        /// <returns></returns>
        string GetCpuID()
        {
            try
            {
                //获取CPU序列号代码
                string cpuInfo = "";//cpu序列号
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }
                moc = null;
                mc = null;
                return cpuInfo;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }

        /// <summary>
        /// 获取设备网卡信息
        /// </summary>
        /// <returns></returns>
        string GetMacAddress()
        {
            try
            {
                //获取网卡Mac地址
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac = mo["MacAddress"].ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return mac;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }

        /// <summary>
        /// 获取设备IP地址
        /// </summary>
        /// <returns></returns>
        string GetIPAddress()
        {
            try
            {
                //获取IP地址
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        //st=mo["IpAddress"].ToString();
                        System.Array ar;
                        ar = (System.Array)(mo.Properties["IpAddress"].Value);
                        st = ar.GetValue(0).ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }

        /// <summary>
        /// 获取设备硬盘ID
        /// </summary>
        /// <returns></returns>
        string GetDiskID()
        {
            try
            {
                //获取硬盘ID
                String HDid = "";
                ManagementClass mc = new ManagementClass("Win32_DiskDrive");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    HDid = (string)mo.Properties["Model"].Value;
                }
                moc = null;
                mc = null;
                return HDid;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }

        /// 
        /// 操作系统的登录用户名
        /// 
        /// 
        string GetUserName()
        {
            try
            {
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {

                    st = mo["UserName"].ToString();

                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }

        /// 
        /// PC类型
        /// 
        /// 
        string GetSystemType()
        {
            try
            {
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {

                    st = mo["SystemType"].ToString();

                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }

        /// 
        /// 物理内存
        /// 
        /// 
        string GetTotalPhysicalMemory()
        {
            try
            {

                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {

                    st = mo["TotalPhysicalMemory"].ToString();

                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }

        /// 
        /// 计算机名称
        /// 
        /// 
        string GetComputerName()
        {
            try
            {
                return System.Environment.GetEnvironmentVariable("ComputerName");
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }

    }
}