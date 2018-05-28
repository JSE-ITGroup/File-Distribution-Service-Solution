using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using FileDistributionService.Model;

namespace FileDistributionService
{
    public class ScheduledTask
    {

        public int JobId { get; set; }
        public string JobName { get; set; }
        public string Description { get; set; }
        public DataSourceOptions DataSourceOptions { get; set; }
        public List<Dictionary<string, object>> DataSourceDictionary { get; set; }

        public DataDeliveryOptions DataDeliveryOptions { get; set; }
        public Dictionary<string, object> DataDeliveryDictionary { get; set; }
        public ScheduleOptions ScheduleOptions { get; set; }
        public Dictionary<string, object> scheduleOptionsDictionary { get; set; }
        public bool JobActive { get; set; }
        public DateTime LastRunTime { get; set; }
    }

}
