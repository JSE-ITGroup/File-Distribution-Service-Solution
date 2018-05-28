using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessUtility;

namespace FileDistributionService.DataAccessLayer
{
   public  class DataDbAccess
    {
        

        public static DataTable GetSourceData(DataSourceDbProvider dbProvider, string connectionString, string query)
        {
            DbManager dbManager = new DbManager();

            if (dbProvider == DataSourceDbProvider.MsSql)
            {
                dbManager.OpenConnection(connectionString, Common.DataProvider.Mssql);
            }else if (dbProvider == DataSourceDbProvider.Odbc)
            {
                dbManager.OpenConnection(connectionString, Common.DataProvider.Odbc);
            }
            else if (dbProvider == DataSourceDbProvider.OleDb)
            {
                dbManager.OpenConnection(connectionString, Common.DataProvider.OleDb);
            }
           
            var dt = dbManager.GetDataTable(query.Replace("\"",""), Common.ProcedureType.Text, null);
            return dt;
        }
    }
}
