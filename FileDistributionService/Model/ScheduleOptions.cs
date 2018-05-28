using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDistributionService.Model
{
    public class ScheduleOptions
    {
        public ScheduleMode  ScheduleMode { get; set; }
        public string ScheduledTime { get; set; }
        public int IntervalMinutes { get; set; }

        public string IntervalStartTime { get; set; }
        public string IntervalEndTime { get; set; }

    }
}
