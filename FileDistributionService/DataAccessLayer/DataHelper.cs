using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using FileDistributionService.Helper;

namespace FileDistributionService.DataAccessLayer
{
    class DataHelper
    {
        public static  string DataTableToCsv(DataTable dtSource, string fileName)
        {
          
            const bool includeHeader = true;

            if (dtSource == null || dtSource.Rows.Count < 1)
            {
                
               // return null;
            }

            using (StreamWriter writer = new StreamWriter(fileName))
            {
                if (includeHeader)
                {
                    string[] columnNames =
                        dtSource.Columns.Cast<DataColumn>()
                            .Select(column => "\"" + column.ColumnName.Replace("\"", "\"\"") + "\"")
                            .ToArray<string>();
                    writer.WriteLine(String.Join(",", columnNames));
                    writer.Flush();
                }

                foreach (DataRow row in dtSource.Rows)
                {
                    string[] fields =
                        row.ItemArray.Select(field => "\"" + field.ToString().Replace("\"", "\"\"") + "\"")
                            .ToArray<string>();
                    writer.WriteLine(String.Join(",", fields));
                    writer.Flush();
                }
            }
            return fileName;
        
            
        }
        public static string DataTableToXls(DataTable dt, string fileName)
        {
            return "";
        }
        public static string DataTableToTxt(DataTable dt, string fileName)
        {
            return "";
        }
        public static string DataTableToDat(DataTable dt, string fileName)
        {
            return "";
        }
        public static string DataTableToXlsx(DataTable dt, string fileName)
        {
            if(dt.ExportToExcelFile(fileName,false))
            {
                return fileName;
            }
            else
            {
                return null;
            }
            

        }
        public static string DataTableToXml(DataTable dt, string fileName)
        {
            return "";
        }
        public static string DataTableToFlatFile(DataTable dt, string fileName)
        {
            return "";
        }
    }
}
