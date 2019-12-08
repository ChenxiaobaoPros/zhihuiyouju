using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Aoto.EMS.Infrastructure.Utils
{
    public class CsvUtil
    {
        private static char quotechar = ',';

        public static void WriteCSV(string filePathName, List<string[]> rows, bool append)
        {
            StreamWriter fileWriter = new StreamWriter(filePathName, append, Encoding.Default);
            foreach (string[] cells in rows)
            {
                StringBuilder buffer = new StringBuilder();
                for (int i = 0; i < cells.Length; ++i)
                {
                    string str = cells[i].Replace("\"", "").Trim();
                    if (str == null)
                        str = "";
                    if (str.IndexOf(",") > -1)
                    {
                        str = "\"" + str + "\"";
                    }
                    buffer.Append(str);
                    if (i != cells.Length - 1)
                        buffer.Append(quotechar);
                }
                fileWriter.WriteLine(buffer.ToString());
            }
            fileWriter.Flush();
            fileWriter.Close();
        }

        public static List<string[]> ReadCSV(string filePathName, Encoding encoding)
        {
            StreamReader fileReader = new StreamReader(filePathName, encoding);
            string rowStr = fileReader.ReadLine();
            List<string[]> rowList = new List<string[]>();
            while (rowStr != null)
            {
                List<string> cellVals = getStrCellVal(rowStr);
                string[] cells = new string[cellVals.Count];
                for (int i = 0; i < cellVals.Count; i++)
                {
                    cells[i] = cellVals[i];
                }
                rowList.Add(cells);
                rowStr = fileReader.ReadLine();
            }
            fileReader.Close();
            return rowList;
        }

        private static List<string> getStrCellVal(string rowStr)
        {
            List<string> cellList = new List<string>();
            while (rowStr != null && rowStr.Length > 0)
            {
                string cellVal = "";
                if (rowStr.StartsWith("\""))
                {
                    rowStr = rowStr.Substring(1);
                    int i = rowStr.IndexOf("\",");
                    int j = rowStr.IndexOf("\" ,");
                    int k = rowStr.IndexOf("\"");
                    if (i < 0) i = j;
                    if (i < 0) i = k;
                    if (i > -1)
                    {
                        cellVal = rowStr.Substring(0, i);
                        if ((i + 2) < rowStr.Length)
                            rowStr = rowStr.Substring(i + 2).Trim();
                        else
                            rowStr = "";
                    }
                    else
                    {
                        cellVal = rowStr;
                        rowStr = "";
                    }
                }
                else
                {
                    int i = rowStr.IndexOf(",");
                    if (i > -1)
                    {
                        cellVal = rowStr.Substring(0, i);
                        if ((i + 1) < rowStr.Length)
                            rowStr = rowStr.Substring(i + 1).Trim();
                        else
                            rowStr = "";
                    }
                    else
                    {
                        cellVal = rowStr;
                        rowStr = "";
                    }
                }
                if (cellVal == "") cellVal = " ";
                cellList.Add(cellVal);
            }
            return cellList;
        }
    }
}
