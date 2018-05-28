using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace FileDistributionService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        ///    
        public static DateTime applicationStartDate;
        static void Main()
        {
            DateTime d = DateTime.Now;
            applicationStartDate = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, 00);


           
             FileDistributionTaskScheduler fileDistributionTaskScheduler = new FileDistributionTaskScheduler();
             fileDistributionTaskScheduler.TaskHandler();

            /*
           ServiceBase[] ServicesToRun;
           ServicesToRun = new ServiceBase[]
           {
               new Service()
           };
           ServiceBase.Run(ServicesToRun);
          */


        }
    }
}
