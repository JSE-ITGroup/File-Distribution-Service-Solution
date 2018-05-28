using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using DataAccessUtility;
using log4net;
using FileDistributionService.DataAccessLayer;
using FileDistributionService.Model;
using FileDistributionService.Helper;
using System.Configuration;
using System.Diagnostics;
using System.Threading;


namespace FileDistributionService
{
    class FileDistributionTaskScheduler
    {
        static ManualResetEvent resetEvent = new ManualResetEvent(false);

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<ScheduledTask> _scheduledTaskList;

        private string message;
        public List<ScheduledTask> GetScheduledTasks()
        {
            List<ScheduledTask> scheduledTasks = new List<ScheduledTask>();
            const string configSectionName = "JobFileLocationConfig";


            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var sectionKey = configuration.Sections.Keys;
            var section = configuration.GetSection(configSectionName);
            var appSettings = section as AppSettingsSection;
            if (appSettings == null)
            {
                log.Error("Config Section for AppSettings was not Found");
                return null;
            }

            var jobPath = appSettings.Settings["JobConfigPath"];


            string basedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, jobPath.Value);

            foreach (string file in Directory.GetFiles(basedPath, "*.cfg"))
            {


                var scheduledTask = ConfigReader.GetJobConfig(file);
                if (scheduledTask == null)
                {
                    continue;
                }
                if (scheduledTask.JobActive)
                {
                    scheduledTasks.Add(scheduledTask);
                }

            }


            return scheduledTasks;
        }

        public FileDistributionTaskScheduler(List<ScheduledTask> scheduledTaskList)
        {
            _scheduledTaskList = scheduledTaskList;
        }
        public FileDistributionTaskScheduler()
        {
            _scheduledTaskList = null;
        }
        //Call from Services
        public void TaskHandler()
        {
            log.Info("File Distribution Service Task Handler has been Initiated");
            if (_scheduledTaskList == null || _scheduledTaskList.Count < 1)
            {
                List<ScheduledTask> taskList = GetScheduledTasks();

                if (taskList == null || taskList.Count < 1)
                {
                    message = string.Format("There are no available Task for Processing at this time");
                    log.Info(message);
                    return;
                }
                else
                {
                    message = string.Format("{0} Job(s) retrieved for Processing", taskList.Count);
                    log.Info(message);
                    foreach (var t in taskList)
                    {
                        message = string.Format("Job Name: {0} Job Description: {1}", t.JobName, t.Description);
                        log.Info(message);
                    }

                }
                _scheduledTaskList = taskList;
            }

            foreach (var task in _scheduledTaskList)
            {
                DateTime currentDateTime = DateTime.Now;
                DateTime scheduleDateTime = DateTime.MinValue;
                DateTime startDateTime = DateTime.MinValue;
                DateTime endDateTime = DateTime.MinValue;
                List<DateTime> scheduleStartDateTimes = new List<DateTime>();

                var executejob = false;
                //Condition to Validate Schedule Time
                Dictionary<string, object> scheduleOptionDictionary = new Dictionary<string, object>();
                scheduleOptionDictionary = task.scheduleOptionsDictionary;
                if (scheduleOptionDictionary == null || scheduleOptionDictionary.Count < 1)
                {
                    log.Info(string.Format("No scheduling Options have been configured for Job:{0} ", task.JobName));
                    continue;
                }


                var scheduleMode = (ScheduleMode)Convert.ToInt16(HelperMethods.DictionaryLookUp(scheduleOptionDictionary, "ScheduleMode"));
                var intervalStartime = (string)HelperMethods.DictionaryLookUp(scheduleOptionDictionary, "IntervalStartime");
                var intervalEndTime = (string)HelperMethods.DictionaryLookUp(scheduleOptionDictionary, "IntervalEndTime");
                var intervalMinutes = (int)Convert.ToInt16(HelperMethods.DictionaryLookUp(scheduleOptionDictionary, "IntervalMinutes"));
                var specificStartTime = (string)HelperMethods.DictionaryLookUp(scheduleOptionDictionary, "SpecificTimeTrigger");
                var repeatEvery = (RepeatEvery)Convert.ToInt32(HelperMethods.DictionaryLookUp(scheduleOptionDictionary, "RepeatEvery")); //1st,2nd,3rd,4th,lst
                var dayNumberOfTheWeek = (string)HelperMethods.DictionaryLookUp(scheduleOptionDictionary, "DayNumberOfTheWeek"); //1.....7
                string [] rundays = new string[7];
                if (dayNumberOfTheWeek == null)
                {
                    rundays =  new []{ "2","3","4","5","6"};
                }
                else
                {
                    rundays = dayNumberOfTheWeek.Split(',');
                }
                 
                

                startDateTime = DateTime.Today.Add(TimeSpan.Parse(intervalStartime));
                endDateTime = DateTime.Today.Add(TimeSpan.Parse(intervalEndTime));

               
                message = string.Format("Job Name: {0} \r\n Schedule Mode: {1}  \r\n Interval StartTime: {2} \r\n Interval EndTime: {3} \r\n Interval in Minutes: {4} \r\n Specific StartTime: {5} \r\n Run Days : {6}",
                                        task.JobName, scheduleMode, intervalStartime, intervalEndTime, intervalMinutes, specificStartTime, dayNumberOfTheWeek);

                log.Info(message);


                var appStartTime = DateTime.Today.Add(TimeSpan.Parse(Properties.Settings.Default.ServiceStartTime));
                var appEndTime = DateTime.Today.Add(TimeSpan.Parse(Properties.Settings.Default.ServiceEndTime));


                
                             if (!(DateTime.Compare(appStartTime, DateTime.Now) < 0 && (DateTime.Compare(DateTime.Now,appEndTime) < 0)))
                             {
                                 log.Warn(string.Format("The current time  does not fall within  specific Start Time : {0} and End Time: {1} for Application", appStartTime, appEndTime));
                                 break;
                             }

                /*
                   if ((DateTime.Now > appStartTime && DateTime.Now <= appEndTime))
                                         {
                                             log.Warn(string.Format("The current time  does not fall within  specific Start Time : {0} and End Time: {1} for Application", appStartTime, appEndTime));
                                             break;
                                         }
                                        */

                try
                {
                    
                switch (scheduleMode)
                {
                    case ScheduleMode.Daily:
                        {
                            //Get list of days from RunDays
                            var runDay = rundays.Where(a => a.Equals(((int)DateTime.Now.DayOfWeek + 1).ToString()));

                            if (startDateTime <= endDateTime)
                            {
                                if (runDay.Any())
                                {
                                    scheduleDateTime = DateTime.Today.Add(TimeSpan.Parse(specificStartTime));
                                    scheduleStartDateTimes.Add(scheduleDateTime);
                                }
                            }
                            break;
                        }
                    case ScheduleMode.Interval:
                        {
                                //Use Current Date/Time
                                //StartDateTime = Program.applicationStartDate;
                                //Get list of days from RunDays

                                var runDay = rundays.Where(a => a.Equals(((int)DateTime.Now.DayOfWeek + 1).ToString()));

                                if (runDay.Any())
                            {

                                scheduleStartDateTimes.Add(startDateTime);
                                scheduleDateTime = startDateTime;
                                while (startDateTime <= endDateTime)
                                {
                                    //Set Hour to Friday
                                    scheduleDateTime = scheduleDateTime.AddMinutes(intervalMinutes);
                                    scheduleStartDateTimes.Add(scheduleDateTime);
                                    startDateTime = scheduleDateTime;
                                }
                            }
                            break;

                        }

                    case ScheduleMode.Now:
                        {
                                
                                scheduleStartDateTimes.Add(DateTime.Now);
                            break;
                        }

                    case ScheduleMode.Weekly:
                        {
                            var runDay = rundays.Where(a => a.Equals(((int)DateTime.Now.DayOfWeek + 1).ToString()));
                                if (runDay.Any())
                            {
                                scheduleDateTime = DateTime.Today.Add(TimeSpan.Parse(specificStartTime));
                                scheduleStartDateTimes.Add(scheduleDateTime);
                            }
                            break;

                        }
                    case ScheduleMode.Monthly:
                        {
                             var runDay = rundays.Where(a => a.Equals(((int)DateTime.Now.DayOfWeek + 1).ToString()));
                                if (runDay.Any())
                            {
                                scheduleDateTime = DateTime.Today.Add(TimeSpan.Parse(specificStartTime));
                                scheduleStartDateTimes.Add(scheduleDateTime);
                            }
                            break;
                        }
                    case ScheduleMode.Quarterly:
                        {
                            var runDay = rundays.Where(a => a.Equals(((int)DateTime.Now.DayOfWeek + 1).ToString()));
                                if (runDay.Any())
                            {
                                scheduleDateTime = DateTime.Today.Add(TimeSpan.Parse(specificStartTime));
                                scheduleStartDateTimes.Add(scheduleDateTime);
                            }
                            break;
                        }
                    default:
                        break;
                }

               

                int day = DateTime.Now.Day;
                int hrs = DateTime.Now.Hour;
                int minute = DateTime.Now.Minute;

                var startdate = from t in scheduleStartDateTimes.Where(a => a.Day == day && a.Hour == hrs && a.Minute == minute && a.Second <= 59) select t;
                if (startdate.Any())
                {
                    executejob = true;

                }


                if (!executejob)
                {
                    log.Info(string.Format("No Job Schdeuled for the current Date Time: {0}", DateTime.Now));
                    continue;
                }
                ExecuteTaskWrapper(task);
                resetEvent.WaitOne();

                }
                catch (Exception exp)
                {
                    log.Error(exp);

                }


            }
            //
        }

        private async void ExecuteTaskWrapper(ScheduledTask task)
        {
            log.Info(string.Format("Fetch Job: {0}", task.JobName));
            try
            {


                var status = await Task.Run(() => ExecuteTask(task));
                if (!status)
                {
                    message =
                        string.Format("Job: {0} was last succesfully run at {1} was not proceesed due to: {2}",
                            task.JobName, DateTime.Now, message);
                    log.Warn(message);
                }
                else
                {
                    message = string.Format("Job: {0} was succesfully processed last Run Time: {1}",
                        task.JobName, DateTime.Now);
                    log.Info(message);
                }
            }
            catch (Exception exp)
            {
                message = string.Format("There was an error processing Job: {0}, last Run Time: {1}", task.JobName, DateTime.Now);
                log.Error(message, exp);
                return;
            }
            resetEvent.Set();
        }

        public bool ExecuteTask(ScheduledTask task)
        {
            message = "";
            var dataSourceOption = task.DataSourceDictionary;
            if (dataSourceOption == null || !dataSourceOption.Any())
            {
                message = string.Format("No data Source Options was configured for Job: {0}", task.JobName);
                // log.Info(message);
                return false;

            }

            foreach (var datasource in dataSourceOption)
            {
                var TypeOfDataSource = (DataSourceType)Convert.ToInt16(HelperMethods.DictionaryLookUp(datasource, "TypeOfDataSource"));
                var securityTypeForDataSource = (AccessSecurityType)Convert.ToInt16(HelperMethods.DictionaryLookUp(datasource, "SecurityTypeForDataSource"));
                var relativeOrAbsoluteFilePath = string.IsNullOrEmpty((string)HelperMethods.DictionaryLookUp(datasource, "RelativeOrAbsoluteFilePath")) ? "R" : (string)HelperMethods.DictionaryLookUp(datasource, "RelativeOrAbsoluteFilePath");


                if (TypeOfDataSource == DataSourceType.Database)
                {



                    string connectionString = (string)HelperMethods.DictionaryLookUp(datasource, "ValueForDataConnectionType");
                    string queryFile = null;

                    if (relativeOrAbsoluteFilePath.ToUpper().Equals("R"))
                    {
                        queryFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                            (string)HelperMethods.DictionaryLookUp(datasource, "ValueForDataRetrievalMethod"));

                    }
                    else
                    {
                        queryFile = (string)HelperMethods.DictionaryLookUp(datasource, "ValueForDataRetrievalMethod");
                    }
                    if (!File.Exists(queryFile))
                    {
                        message = string.Format("Unable to load query files: {0} for Job: {1}", queryFile, task.JobName);
                        return false;
                    }
                    //Get Values for Data Source Property

                    if (datasource.ContainsKey("RequirePreOrPostProcessExecution") && (datasource["RequirePreOrPostProcessExecution"].Equals("1") || datasource["RequirePreOrPostProcessExecution"].Equals("2") || datasource["RequirePreOrPostProcessExecution"].Equals("3")))
                    {
                        if (datasource.ContainsKey("ProcessExecution"))
                        {
                            var executionProcess = datasource["ProcessExecution"];
                            var processes = (List<Dictionary<string, object>>)executionProcess;
                            if (executionProcess != null)
                            {

                                Dictionary<string, object> preProcess =
                                    processes.FirstOrDefault(a => a.ContainsKey("Type") && a.ContainsValue("1"));
                                if (preProcess.Any())
                                    ExternalProcessExecution(preProcess);
                            }

                        }
                    }

                    string query = System.IO.File.ReadAllText(queryFile);
                    var dbProviderForDatabaseDataSource = (DataSourceDbProvider)Convert.ToInt16(HelperMethods.DictionaryLookUp(datasource, "DbProviderForDatabaseDataSource"));

                    if (securityTypeForDataSource == AccessSecurityType.Ssl)
                    {
                        SshWrapper.Connect(null);
                    }
                    log.Info(string.Format("Job Query Retrieved for Job: {0}", task.JobName));

                    DataTable sourceData = null;
                    try
                    {
                        sourceData = DataDbAccess.GetSourceData(dbProviderForDatabaseDataSource, connectionString, query);

                    }
                    catch (Exception exp)
                    {
                        log.Error(exp);
                    }
                  
                    log.Info(string.Format("Record(s) Retrieved: {0} for Job: {1}", sourceData == null ? 0 : sourceData.Rows.Count, task.JobName));

                    if (securityTypeForDataSource == AccessSecurityType.Ssl)
                    {
                        SshWrapper.Disconnect();
                    }

                    if (sourceData == null || sourceData.Rows.Count < 1)
                    {
                        message = string.Format("No data was retrieved from Source: {0}", task.JobName);
                        // return false;
                    }
                    if (!datasource.ContainsKey("OutputFileFormat"))
                    {
                        message = string.Format("No Output File Format Defined for Job: {0}", task.JobName);
                        return false;
                    }
                    var fileName = CreateDataFile((Dictionary<string, object>)datasource["OutputFileFormat"], relativeOrAbsoluteFilePath, sourceData);
                    if (fileName == null)
                    {
                        message = string.Format("Unable to Create data File for Job: {0}", task.JobName);
                        return false;
                    }
                    log.Info(string.Format("Output File created at : {0} for Job: {1} ", fileName, task.JobName));
                    bool status = DeliveryMethodHelper.ProcessDeliveryMethod(task.DataDeliveryDictionary, fileName);
                    if (!status)
                    {
                       // message = string.Format("Unable to Delivery data using Delivery Option: {0} for Job: {1} ",
                           // task.DataDeliveryOptions.DeliveryMethod, task.JobName);
                        //return false;
                    }

                }
                else
                {
                    message = string.Format("No Operation has been defined for Type of Data Source: {0} for Job: {1}",
                        TypeOfDataSource, task.JobName);
                    // log.Info(message);
                }
            }
            return true;
        }

        private bool ExternalProcessExecution(Dictionary<string, object> processDictionary)
        {

            bool useShell = string.IsNullOrEmpty(HelperMethods.DictionaryLookUp(processDictionary, "UseShellInExecution").ToString())
                    ? true
                    : HelperMethods.DictionaryLookUp(processDictionary, "UseShellInExecution").ToString().ToUpper().Equals("Y") ? true : false;


            bool createNoWindow = string.IsNullOrEmpty(HelperMethods.DictionaryLookUp(processDictionary, "CreateNoWindowInExecution").ToString())
                   ? true
                   : HelperMethods.DictionaryLookUp(processDictionary, "CreateNoWindowInExecution").ToString().ToUpper().Equals("Y") ? true : false;

            string executableFileName = (string)Helper.HelperMethods.DictionaryLookUp(processDictionary, "ExecutableFileName");
            string arguments = (string)Helper.HelperMethods.DictionaryLookUp(processDictionary, "ArgumentsforExecution");
            string workingDirecotry = (string)Helper.HelperMethods.DictionaryLookUp(processDictionary, "ExecuteableWorkingDirectory");



            // Use ProcessStartInfo class.
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = createNoWindow;
            startInfo.UseShellExecute = useShell;
            startInfo.FileName = executableFileName;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            if (!string.IsNullOrEmpty(arguments))
            {
                startInfo.Arguments = arguments;
            }
            if (!string.IsNullOrEmpty(workingDirecotry))
            {
                startInfo.WorkingDirectory = workingDirecotry;
            }

            try
            {
                Process[] processes = Process.GetProcesses();

                foreach (Process process in processes)
                {
                    if (process.StartInfo == startInfo)
                    {
                        log.Error(string.Format("Process : {0} is already  and will be terminated", process.Id.ToString()));
                        process.Kill();
                        break;
                    }

                }

                // Start the process with the info we specified.
                // Call WaitForExit and then the using-statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {

                    exeProcess.WaitForExit();
                }
            }
            catch (Exception exp)
            {
                log.Error(string.Format("Pre or Post Failed for Job"), exp);
                return false;
            }
            return true;
        }

        public string CreateDataFile(Dictionary<string, Object> outputFileOptionsDictionary, string relativeOrAbsoluteFilePath, DataTable dt)
        {
            var file = (string)HelperMethods.DictionaryLookUp(outputFileOptionsDictionary, "FileName");
            var fileTimeStampOptions = (FileNameTimeStampOptions)Convert.ToInt16(HelperMethods.DictionaryLookUp(outputFileOptionsDictionary, "FileTimeStampOptions"));
            string fileOutputPath = null;
            if (relativeOrAbsoluteFilePath.ToUpper().Equals("A"))
            {

                fileOutputPath =
                   (string)HelperMethods.DictionaryLookUp(outputFileOptionsDictionary, "FileOutputPath");

            }
            else
            {
                fileOutputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, (string)HelperMethods.DictionaryLookUp(outputFileOptionsDictionary, "FileOutputPath"));
            }
            var extensionOfFile = (FileExtenson)Convert.ToInt16(HelperMethods.DictionaryLookUp(outputFileOptionsDictionary, "ExtensionOfFile"));


            string fileName = "";
            string fulldatestring = DateTime.Now.ToString("yyyyMMdd_hhmmss");
            string yrmnthdatestr = DateTime.Now.ToString("yyyyMMdd");
            string hrsminsecstr = DateTime.Now.ToString("hhmmss");

            if (fileTimeStampOptions == FileNameTimeStampOptions.UseHourMintuesMilliSecond)
            {
                fileName = string.Format("{0}_{1}.{2}", file, hrsminsecstr, extensionOfFile);
            }
            else if (fileTimeStampOptions == FileNameTimeStampOptions.UseYearMonthDay)
            {
                fileName = string.Format("{0}_{1}.{2}", file, yrmnthdatestr, extensionOfFile);
            }
            else if (fileTimeStampOptions == FileNameTimeStampOptions.UseYearMonthDayWithHourMintuesMilliSecond)
            {
                fileName = string.Format("{0}_{1}.{2}", file, fulldatestring, extensionOfFile);
            }
            else
            {
                fileName = string.Format("{0}.{1}", file, extensionOfFile);
            }

            fileName = Path.Combine(fileOutputPath, fileName);
            switch (extensionOfFile)
            {
                case FileExtenson.csv:
                    {

                        DataHelper.DataTableToCsv(dt, fileName);

                        break;
                    }
                case FileExtenson.dat:
                    {
                        DataHelper.DataTableToDat(dt, fileName);
                        break;
                    }
                case FileExtenson.txt:
                    {
                        DataHelper.DataTableToTxt(dt, fileName);
                        break;
                    }
                case FileExtenson.xls:
                    {
                        DataHelper.DataTableToXls(dt, fileName);
                        break;
                    }
                case FileExtenson.xlsx:
                    {
                        DataHelper.DataTableToXlsx(dt, fileName);
                        break;
                    }
                case FileExtenson.Other:
                    {

                        break;
                    }
                default:
                    break;
            }
            return fileName;
        }

    }
}
