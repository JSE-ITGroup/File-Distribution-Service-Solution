using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDistributionService
{
    public enum DataSourceType
    {
        Database = 1,
        File = 2,
        Ftp = 3,
        Email = 4,
    }
    public enum DataSourceDbProvider
    {
      
        MsSql = 1,
        OleDb = 3,
        Odbc = 4,
    }

    public enum AccessSecurityType
    {
        None = 0,
        Ssl = 1

    }
    public enum DataRetrievalMethod
    {
        SqlQuery = 0,
        Other = 1

    }
    public enum DataConnectionType
    {
        ConnectionString = 0,
        Other = 1

    }

    
    public enum FileExtenson
    {
        dat = 1,
        txt = 2,
        csv = 3,
        xls = 4,
        xlsx = 5,
        xml = 6,
        Other =7

    }
    public enum FileNameTimeStampOptions
    {
        UseYearMonthDay = 1,
        UseHourMintuesMilliSecond = 2,
        UseYearMonthDayWithHourMintuesMilliSecond = 3,
        None = 4


    }

    public enum DeliveryMethod
    {
       
        FtpOrSftp =1,
        Email = 2,
        Database =3,
        DropBox=4,
        Other =5
    }

    public enum ScheduleMode
    {
        Daily =0,
        Interval=1,
        Weekly =2,
        Monthly=3,
        Now =4,
        Quarterly =5
    }

    public enum RepeatEvery
    {
        First = 0,
        Second = 1,
        Third = 2,
        Fourth = 3,
        Last = 4,
        
    }

    
}
