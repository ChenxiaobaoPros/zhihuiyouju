using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Aoto.EMS.Infrastructure.Configuration;

namespace Aoto.EMS.Infrastructure.Utils
{
    /// <summary>
    /// 加密类
    /// </summary>
    public class CodeRegister
    {

        private static byte[] IV = { 0x12, 0x21, 0x08, 0x14, 0xC3, 0xCC, 0x04, 0xE8 };//加密IV向量

        /// <summary>
        /// 加密DES
        /// </summary>
        /// <param name="Key">加密key</param>
        /// <param name="str">数据</param>
        /// <returns></returns>
        public static String Encrypt(String Key, String str)
        {
            return EncryptByDES(Key, str);
        }

        private static String EncryptByDES(String Key, String str)
        {
            byte[] bKey = Encoding.UTF8.GetBytes(Key.Substring(0, 8));
            byte[] bIV = IV;
            byte[] bStr = Encoding.UTF8.GetBytes(str);
            try
            {
                DESCryptoServiceProvider desc = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, desc.CreateEncryptor(bKey, bIV), CryptoStreamMode.Write);
                cStream.Write(bStr, 0, bStr.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 解密DES
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="DecryptStr"></param>
        /// <returns></returns>
        public static String Decrypt(String Key, String DecryptStr)
        {
            return DecryptByDES(Key, DecryptStr);
        }

        private static String DecryptByDES(String Key, String DecryptStr)
        {
            try
            {
                byte[] bKey = Encoding.UTF8.GetBytes(Key.Substring(0, 8));
                byte[] bIV = IV;
                byte[] bStr = Convert.FromBase64String(DecryptStr);
                DESCryptoServiceProvider desc = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, desc.CreateDecryptor(bKey, bIV), CryptoStreamMode.Write);
                cStream.Write(bStr, 0, bStr.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        public static string GetMd5(string md5)
        {
            return EncryptByMD5(md5);
        }

        private static string EncryptByMD5(string md5)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] value, hash;
            value = System.Text.Encoding.UTF8.GetBytes(md5);
            hash = md.ComputeHash(value);
            md.Clear();
            string temp = "";
            for (int i = 0, len = hash.Length; i < len; i++)
            {
                temp += hash[i].ToString("X").PadLeft(4, '0');
            }
            return temp;
        }

        /// <summary>
        /// 机器码效验
        /// </summary>
        /// <returns></returns>
        public static bool VerifyDeviceCode(ref string stateCode)
        {
            bool VerifySign = false;
            VerifySign = VerifyMachineCode(ref stateCode);
            return VerifySign;
        }

        /// <summary>
        /// 机器码效验
        /// </summary>
        /// <returns></returns>
        public static bool VerifyDeviceCode(string keyPath)
        {
            bool VerifySign = false;
            VerifySign = VerifyMachineCode(keyPath);
            return VerifySign;
        }

        private static bool VerifyMachineCode(ref string stateCode)
        {
            bool VerifySign = false;
            try
            {
                if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), @"TempPPF\protectppf.dat")))
                {
                    return VerifySign;
                }
                using (StreamReader objInput = new StreamReader(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), @"TempPPF\protectppf.dat"), System.Text.Encoding.UTF8))
                {
                    string contents = CodeRegister.Decrypt("aoto0587", objInput.ReadToEnd().Trim());

                    contents = contents.Substring(contents.IndexOf("_") + 1, contents.Length - contents.IndexOf("_") - 1);

                    //string[] split = System.Text.RegularExpressions.Regex.Split(contents, "&", RegexOptions.None);

                    string curMachineCode = CodeRegister.GetMd5("南京奥拓电子" + DeviceInfo.Instance().CpuID + DeviceInfo.Instance().MacAddress + DeviceInfo.Instance().DiskID + DeviceInfo.Instance().SystemType);

                    if (contents.Contains(curMachineCode))
                    {

                        if (DateTime.Compare(DateTime.Now, DateTime.Parse(contents.Replace(curMachineCode + "_", ""))) < 0)
                        {
                            VerifySign = true;
                            stateCode = "激活成功";
                        }
                    }

                }

            }
            catch
            {
                //不记录日志
                VerifySign = false;
            }
            finally
            {
                if (!VerifySign)
                {
                    stateCode = "激活失败";
                    File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), @"TempPPF\protectppf.dat"));
                }
            }
            return VerifySign;
        }

        private static bool VerifyMachineCode(string keypath)
        {
            bool VerifySign = false;
            try
            {
                using (StreamReader objInput = new StreamReader(Path.Combine(keypath, ""), System.Text.Encoding.UTF8))
                {
                    string[] strContents = objInput.ReadToEnd().Trim().TrimEnd('&').Split('&');

                    string deviceInfo = DeviceInfo.Instance().CpuID + DeviceInfo.Instance().MacAddress + DeviceInfo.Instance().DiskID + DeviceInfo.Instance().SystemType;

                    string curMachineCode = CodeRegister.GetMd5("南京奥拓电子" + deviceInfo);

                    foreach (string var in strContents)
                    {
                        string contents = CodeRegister.Decrypt("aoto0587", var);

                        contents = contents.Substring(contents.IndexOf("_") + 1, contents.Length - contents.IndexOf("_") - 1);
                        if (contents.Contains(curMachineCode))
                        {

                            if (DateTime.Compare(DateTime.Now, DateTime.Parse(contents.Replace(curMachineCode + "_", ""))) < 0)
                            {
                                if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), @"TempPPF\protectppf.dat")))
                                {

                                    File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), @"TempPPF\protectppf.dat"));
                                }
                                using (Stream strSall = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), @"TempPPF\protectppf.dat"), FileMode.Create, FileAccess.Write))
                                {
                                    using (StreamWriter strWirtSall = new StreamWriter(strSall, UTF8Encoding.UTF8))
                                    {
                                        strWirtSall.Write(var);
                                    }
                                }

                                VerifySign = true;
                                break;
                            }
                        }

                    }

                }

            }
            catch
            {
                //不记录日志
                VerifySign = false;
            }

            return VerifySign;
        }

    }
}
