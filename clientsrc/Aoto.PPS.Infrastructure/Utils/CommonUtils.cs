using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using System.Collections.Generic;
using Aoto.PPS.Infrastructure.Configuration;
using System.Management;
using System.Drawing;

namespace Aoto.PPS.Infrastructure.Utils
{
    public class CommonUtils
    {
        /// <summary>
        /// 获取主机地址
        /// </summary>
        /// <returns></returns>
        public static string GetHostAddresses()
        {
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress add in ipEntry.AddressList)
            {
                if (AddressFamily.InterNetwork == add.AddressFamily)
                {
                    return add.ToString();
                }
            }
            return String.Empty;
        }

        //public static byte[] Encrypt(byte[] original, String key)
        //{
        //    Aes aes = Aes.Create("AES");
        //    aes.Mode = CipherMode.ECB;
        //    aes.Padding = PaddingMode.PKCS7;
        //    aes.KeySize = 128;
        //    aes.Key = Encoding.ASCII.GetBytes(key);
        //    ICryptoTransform transform = aes.CreateEncryptor();

        //    return transform.TransformFinalBlock(original, 0, original.Length);
        //}

        //public static byte[] Decrypt(byte[] encrypted, string key)
        //{
        //    Aes aes = Aes.Create("AES");
        //    aes.Mode = CipherMode.ECB;
        //    aes.KeySize = 128;
        //    aes.Padding = PaddingMode.PKCS7;
        //    aes.Key = Encoding.ASCII.GetBytes(key);
        //    ICryptoTransform transform = aes.CreateDecryptor();

        //    return transform.TransformFinalBlock(encrypted, 0, encrypted.Length);
        //}

        public static byte[] StrToHexByte(string data)
        {
            data = data.Replace(" ", "");
            if ((data.Length % 2) != 0)
            {
                data += " ";
            }

            byte[] bytes = new byte[data.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(data.Substring(i * 2, 2), 16);
            }

            return bytes;
        }

        public static string ByteToHexStr(byte[] bytes)
        {
            StringBuilder s = new StringBuilder();

            if (bytes != null)
            {
                foreach (byte b in bytes)
                {
                    s.Append(b.ToString("X2"));
                }
            }

            return s.ToString();
        }
        /// <summary>
        /// 数据加密
        /// </summary>
        /// <param name="original"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] original, String key)
        {
            using (RijndaelManaged rm = new RijndaelManaged())
            {
                rm.Key = Encoding.ASCII.GetBytes(key);
                rm.Mode = CipherMode.ECB;
                rm.Padding = PaddingMode.PKCS7;

                return rm.CreateEncryptor().TransformFinalBlock(original, 0, original.Length);
            }
        }
        /// <summary>
        /// 数据解密
        /// </summary>
        /// <param name="encrypted"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] encrypted, string key)
        {
            using (RijndaelManaged rm = new RijndaelManaged())
            {
                rm.Key = Encoding.ASCII.GetBytes(key);
                rm.Mode = CipherMode.ECB;
                rm.Padding = PaddingMode.PKCS7;

                return rm.CreateDecryptor().TransformFinalBlock(encrypted, 0, encrypted.Length);
            }
        }
        /// <summary>
        /// 获取枚举描述信息
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum value)
        {
            Type enumType = value.GetType();
            string name = Enum.GetName(enumType, value);

            if (name != null)
            {
                FieldInfo fieldInfo = enumType.GetField(name);

                if (fieldInfo != null)
                {
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute), false) as DescriptionAttribute;

                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 本地图片转base64字符串
        /// </summary>
        /// <param name="filepath">本地图片路径</param>
        /// <returns>字符串</returns>
        public static string ImageToBase64String(string filepath)
        {
            if (File.Exists(filepath))
            {
                Bitmap bmp = new Bitmap(filepath);
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            else
            {
                return String.Empty;
            }
        }

        ///<summary>
        /// 通过WMI读取系统信息里的网卡MAC
        ///</summary>
        ///<returns></returns>
        public static string GetMacByWMI()
        {
            List<string> macs = new List<string>();
            try
            {
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"])
                    {
                        mac = mo["MacAddress"].ToString();
                        return mac;
                        // macs.Add(mac);
                    }
                }
                moc = null;
                mc = null;
            }
            catch
            {
            }

            return "";
        }
    }
}
