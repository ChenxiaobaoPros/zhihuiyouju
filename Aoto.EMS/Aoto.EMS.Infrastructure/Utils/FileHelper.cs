using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Aoto.EMS.Infrastructure.Utils
{
    /// <summary>
    /// 文件帮助类
    /// </summary>
    public class FileHelper
    {
        string[] format = { "*.jpg", "*.doc", "*.exe", "*.pdf", "*.xls","*.form","*.txt","*.json" };
        #region 读取文件 +string ReadFile(string path)
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public string ReadFile(string path)
        {
            //FileStream sFile = new FileStream(path, FileMode.Open);
            //byte[] byData = new byte[sFile.Length];
            //sFile.Read(byData, 0, byData.Length);
            //string fileContent = System.Text.Encoding.UTF8.GetString(byData);
            //return fileContent;
            string content = String.Empty;
            StreamReader reader = new StreamReader(path, GetFileEncodeType(path));
            return reader.ReadToEnd();
        }
        #endregion

        #region 删除文件夹下面所有文件 void deleteAll(string delpath, string geshi)
        /// <summary>  
        /// 删除文件夹下面所有文件  
        /// </summary>  
        /// <param name="oldpngPath">待删除文件路径</param>  
        /// <param name="newpngPath">删除到的新目录</param>  
        /// <param name="geshi">操作文件的格式 例如:*.png、*.xml</param>  
        public void deleteAll(string delpath, string geshi)
        {
            string[] files = Directory.GetFiles(delpath, geshi);
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }
        #endregion

        #region 複製文件夹下面所有文件 +void copyALl(string oldpngPath, string newpngPath, string geshi)
        /// <summary>  
        /// 複製文件夹下面所有文件
        /// </summary>  
        /// <param name="oldpngPath">待复制文件路径</param>  
        /// <param name="newpngPath">复制到的新目录</param>  
        /// <param name="geshi">操作文件的格式 例如:*.png、*.xml</param>  
        public void copyALl(string oldpngPath, string newpngPath, string geshi)
        {
            string[] files = Directory.GetFiles(oldpngPath, geshi);
            foreach (string file in files)
            {
                string otherFile = Path.Combine(oldpngPath, Path.GetFileName(file));
                File.Copy(file, newpngPath + "\\" + Path.GetFileName(file));
            }
        }
        #endregion

        #region 移动文件夹下的所有文件   void moveAll(string olderpath, string topath)
        /// <summary>  
        /// 移动文件夹下的所有文件  
        /// </summary>  
        /// <param name="olderpath">待移动的文件目录</param>  
        /// <param name="topath">新目录</param>  
        public void moveAll(string olderpath, string topath)
        {
            string[] files = Directory.GetFiles(olderpath, format[0]);
            foreach (string file in files)
            {
                File.Move(file, topath); //移动文件  
            }
        }
        #endregion

        #region 獲取一個文件夾下面所有文件 +string[] GetAllFiles(string path, string geshi)
        /// <summary>
        /// 獲取一個文件夾下面所有文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="geshi">文件類型</param>
        /// <returns></returns>
        public string[] GetAllFiles(string path, string geshi)
        {
            string[] files = Directory.GetFiles(path, geshi);
            return files;
        }
        #endregion

        #region 複製單個文件 + void copyOneFile(string oldfilePath, string newfilePath)
        /// <summary>
        /// 複製單個文件
        /// </summary>
        /// <param name="oldfilePath">文件原來的全路徑</param>
        /// <param name="newfilePath">文件新的全路徑</param>
        public void copyOneFile(string oldfilePath, string newfilePath, string newFileName)
        {
            if (!Directory.Exists(newfilePath))
            {
                Directory.CreateDirectory(newfilePath);
            }
            File.Copy(oldfilePath, newfilePath + newFileName);

        }
        #endregion

        #region 移動一個文件 + void MoveOneFile(string oldfilePath, string newfilePath)
        /// <summary>
        /// 移動一個文件
        /// </summary>
        /// <param name="oldfilePath">文件原來的全路徑</param>
        /// <param name="newfilePath">文件新的全路徑</param>
        public void MoveOneFile(string oldfilePath, string newfilePath, string newFileName)
        {
            if (!Directory.Exists(newfilePath))
            {
                Directory.CreateDirectory(newfilePath);
            }
            File.Move(oldfilePath, newfilePath + newFileName);

        }
        #endregion

        #region 刪除一個文件 +void DeleteOneFile(string oldfilePath)
        /// <summary>
        /// 刪除一個文件 
        /// </summary>
        /// <param name="oldfilePath">文件路徑</param>
        public void DeleteOneFile(string oldfilePath)
        {

            File.Delete(oldfilePath);

        }
        #endregion

        #region 讀取目錄的時候進行的提示
        public void ReadFileLogOnly(string path)
        {
            Console.WriteLine(string.Format("正在進入{0}目錄中", path));
            if (!Directory.Exists(path))
            {
                Console.WriteLine(String.Format("文件目錄{0}不存在", path));
                //return;
                //Directory.CreateDirectory(erpLogPath);
            }
            else
            {
                Console.WriteLine(String.Format("文件目錄{0}訪問正常", path));
            }
        }
        public void ReadFileLogAndCreate(string path)
        {
            Console.WriteLine(string.Format("正在進入{0}目錄", path));
            if (!Directory.Exists(path))
            {
                Console.WriteLine(String.Format("文件目錄{0}不存在，正在進行創建", path));

                Directory.CreateDirectory(path);
                Console.WriteLine(String.Format("文件目錄{0}創建成功", path));
            }
            else
            {
                Console.WriteLine(String.Format("文件目錄{0}訪問正常", path));
            }
        }
        #endregion

        #region 獲取問價的編碼方式 +System.Text.Encoding GetFileEncodeType(string filename)
        /// <summary>
        /// 獲取問價的編碼方式
        /// </summary>
        /// <param name="filename">文件路徑</param>
        /// <returns>文件的編碼方式</returns>
        public System.Text.Encoding GetFileEncodeType(string filename)
        {
            System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
            Byte[] buffer = br.ReadBytes(2);
            if (buffer[0] >= 0xEF)
            {
                if (buffer[0] == 0xEF && buffer[1] == 0xBB)
                {
                    return System.Text.Encoding.UTF8;
                }
                else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                {
                    return System.Text.Encoding.BigEndianUnicode;
                }
                else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                {
                    return System.Text.Encoding.Unicode;
                }
                else
                {
                    return System.Text.Encoding.Default;
                }
            }
            else
            {
                return System.Text.Encoding.Default;
            }

        }
        #endregion


        /// <summary>  
        /// 从文件中读取信息  
        /// </summary>  
        /// <param name="path">文件路径</param>  
        /// <returns></returns>  
        public static string GetInfoFromFile(string path)
        {
            StringBuilder str = new StringBuilder();
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(path, Encoding.Default);
                while (!reader.EndOfStream)
                {
                    str.Append(reader.ReadLine());
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                reader.Close();
            }
            return str.ToString();
        }

        /// <summary>  
        /// 返回指定目录中的文件名称  
        /// </summary>  
        /// <param name="folderPath"></param>  
        /// <returns></returns>  
        public static string[] GetFileNameFromFolder(string folderPath)
        {
            try
            {
                return Directory.GetFiles(folderPath);
            }
            catch (DirectoryNotFoundException e)
            {
                return null;
            }
        }

        /// <summary>  
        /// 往文件中写数据  
        /// </summary>  
        /// <param name="path">路径</param>  
        /// <param name="info">数据</param>  
        /// <returns></returns>  
        public static bool WriteToFile(string path, string info)
        {
            if (!Directory.Exists(path))
            {//创建路径  
                // Directory.CreateDirectory(path);  
                FileInfo file = new FileInfo(path);
                FileStream f = file.Create();
                f.Close();
            }
            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter(path, false, Encoding.GetEncoding("gb2312"));
                writer.Write(info);
            }
            catch
            {
                return false;
            }
            finally
            {
                writer.Close();
            }
            return true;
        }
    }
}
