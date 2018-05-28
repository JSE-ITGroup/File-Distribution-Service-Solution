using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace FileDistributionService
{

    public partial class Service : ServiceBase
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private System.Diagnostics.EventLog eventLog;
        private List<ScheduledTask> taskList;
        private FileDistributionTaskScheduler fileDistributionTaskScheduler;
        public Service()
        {
           InitializeComponent();
            eventLog = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("MySource"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "MySource", "MyNewLog");
            }
            eventLog.Source = "MySource";
            eventLog.Log = "MyNewLog";
        }

    protected override void OnStart(string[] args)
    {
            GetTaskList();
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = string.IsNullOrEmpty(Properties.Settings.Default.ServiceTimerFrequency)? 60000: Convert.ToInt32(Properties.Settings.Default.ServiceTimerFrequency); // 60 seconds
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
            log.Info("File Distribution Services has Started");
        }
        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.
            fileDistributionTaskScheduler = new FileDistributionTaskScheduler(taskList);
            fileDistributionTaskScheduler.TaskHandler();
            log.Info(string.Format("Process Running at: {0}", args.SignalTime));
        }

        public void GetTaskList()
        {
            fileDistributionTaskScheduler = new FileDistributionTaskScheduler();
            taskList = fileDistributionTaskScheduler.GetScheduledTasks();

        }
        protected override void OnStop()
        {
            log.Info("File Distribution Services Stopped");
        }
    }
}
