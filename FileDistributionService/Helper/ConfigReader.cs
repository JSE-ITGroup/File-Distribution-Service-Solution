using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using log4net;

namespace FileDistributionService.Helper
{
    class ConfigReader
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ScheduledTask GetJobConfig(string fileName)
        {
           
            if (!File.Exists(fileName))
            {
                log.Error(string.Format("File Does not Exist at: {0}", fileName));
                return null;
            }
            string xmlfileContents = File.ReadAllText(fileName);
            XmlDocument xdoc = new XmlDocument();
            try
            {
                xdoc.LoadXml(xmlfileContents);
            }
            catch (Exception exp)
            {
                log.Error(exp);
                return null;

            }
            XElement jobElement = XElement.Parse(xmlfileContents, LoadOptions.None);



            ScheduledTask scheduledTask = new ScheduledTask();



            string job = jobElement.AncestorsAndSelf().First().Name.ToString().ToUpper();
            string jobName = jobElement.Descendants().First().Attribute("JobName") != null
                ? jobElement.Descendants().First().Attribute("JobName").Value.ToUpper()
                : "";

            string description = jobElement.Descendants().First().Attribute("JobDescription") != null
                ? jobElement.Descendants().First().Attribute("JobDescription").Value.ToUpper()
                : "";
            string activeStatus = jobElement.Descendants().First().Attribute("JobActive") != null
                 ? jobElement.Descendants().First().Attribute("JobActive").Value.ToUpper()
                : "";

            //Get Data Sources
            var datasources = from ds in jobElement.Elements("job").Elements("dataSources")
                              select ds;

            var datadestination = from ds in jobElement.Elements("job").Elements("destinationProperty")
                                  select ds;

            var scheduleoption = from ds in jobElement.Elements("job").Elements("schedulerProperty")
                                 select ds;


            var dataSourceXelement = datasources as XElement[] ?? datasources.ToArray();
            if (!dataSourceXelement.Any() || datasources == null)
            {
                log.Error("No data sources were provided in Config File");
                return null;
            }

            var datadestinationXElements = datadestination as XElement[] ?? datadestination.ToArray();
            if (!datadestinationXElements.Any() || datadestination == null)
            {
                log.Error("No data delivery options were provided in Config File");
                return null;
            }

            var scheduleoptionxElements = scheduleoption as XElement[] ?? scheduleoption.ToArray();
            if (!scheduleoptionxElements.Any() || scheduleoption == null)
            {
                log.Error("No Scheduling options were provided in Config File");
                return null;
            }

            List<Dictionary<string, object>> dataSourceDictionary = new List<Dictionary<string, object>>();

            //Get Data Souces
            foreach (var item in dataSourceXelement.Elements("dataSource").ToList())
            {
                Dictionary<string, object> dataElement = new Dictionary<string, object>();
                if (item.HasAttributes)
                {
                    if (item.Attribute("RelativeOrAbsoluteFilePath") != null)
                    {
                        dataElement[item.Attribute("RelativeOrAbsoluteFilePath").Name.ToString()] = item.Attribute("RelativeOrAbsoluteFilePath").Value;
                    }

                }
                foreach (var element in item.Elements().ToList())
                {


                    if (element.HasElements)
                    {
                        Dictionary<string, object> outputFormatDictionary = new Dictionary<string, object>();
                       List<Dictionary<string, object>> processExecutionList = new List<Dictionary<string, object>>();
                        Dictionary<string, object> process = null;

                        if (element.Name.ToString().ToUpper().Equals("OUTPUTFILEFORMAT"))
                        {

                            foreach (var outputFileFormatElement in element.Elements().ToList())
                            {
                                if (outputFormatDictionary.ContainsKey(outputFileFormatElement.Name.ToString()))
                                {
                                    log.Warn(string.Format("The Key: {0} already exists in Output File Format Dictionary", outputFileFormatElement.Name.ToString()));
                                    continue;
                                }

                                outputFormatDictionary[outputFileFormatElement.Name.ToString()] = outputFileFormatElement.Value;
                            }
                        }
                        if (element.Name.ToString().ToUpper().Equals("PROCESSEXECUTION"))
                        {
                              foreach (var processExecutionElements in element.Elements().ToList())
                                {
                                process = new Dictionary<string, object>();

                                if (processExecutionElements.HasElements)
                                    {
                                        if (processExecutionElements.HasAttributes)
                                        {
                                            if (processExecutionElements.Attribute("Type") != null)
                                            {
                                                process[processExecutionElements.Attribute("Type").Name.ToString()] =
                                                    processExecutionElements.Attribute("Type").Value;
                                            }
                                        }

                                        foreach (var innerelement in processExecutionElements.Elements().ToList())
                                        {
                                            if (process.ContainsKey(innerelement.Name.ToString()))
                                            {
                                                log.Warn(
                                                    string.Format(
                                                        "The Key: {0} already exists in Output File Format Dictionary",
                                                        innerelement.Name.ToString()));
                                                continue;
                                            }
                                            process[innerelement.Name.ToString()] = innerelement.Value;
                                           
                                        }
                                    }
                                processExecutionList.Add(process);
                            }

                        }

                        if (outputFormatDictionary.Count > 0)
                        {
                            dataElement["OutputFileFormat"] = outputFormatDictionary;
                        }
                        if (processExecutionList.Count > 0)
                        {
                            dataElement["ProcessExecution"] = processExecutionList;
                        }
                    }
                    if (!element.HasElements)
                    {
                        if (dataElement.ContainsKey(element.Name.ToString()))
                        {
                            log.Warn(string.Format("The Key: {0} already exists in Data Source Dictionary", element.Name.ToString()));
                            continue;
                        }

                        dataElement[element.Name.ToString()] = element.Value;
                    }
                    

                }


                dataSourceDictionary.Add(dataElement);
            }

            //Get Data Delivery Option Task
            var dataDestinationDictionary = new Dictionary<string, object>();

            
            var firstOrDefault = datadestinationXElements.FirstOrDefault();
            if (firstOrDefault != null && firstOrDefault.HasAttributes)
            {
                if (firstOrDefault.Attribute("RelativeOrAbsoluteFilePath") != null)
                {
                    dataDestinationDictionary[firstOrDefault.Attribute("RelativeOrAbsoluteFilePath").Name.ToString()] =
                        firstOrDefault.Attribute("RelativeOrAbsoluteFilePath").Value;
                }
            }
            
        
            foreach (var element in datadestinationXElements.Elements().ToList())
            {
                if (dataDestinationDictionary.ContainsKey(element.Name.ToString()))
                {
                    log.Warn(string.Format("The Key: {0} already exists in Destination Property Dictionary", element.Name.ToString()));
                    continue;
                    //break
                }
                dataDestinationDictionary[element.Name.ToString()] = element.Value;
                
               
            }

            //Get Schedule Task
            var dataScheduleDictionary= new Dictionary<string, object>();
            foreach (var element in scheduleoptionxElements.Elements().ToList())
            {
                if (dataScheduleDictionary.ContainsKey(element.Name.ToString()))
                {
                    log.Warn(string.Format("The Key: {0} already exists in Schedule Options Dictionary", element.Name.ToString()));
                    continue;
                }
                dataScheduleDictionary[element.Name.ToString()] = element.Value;
                /*  if (element.HasAttributes)
                  {
                      if (element.Attribute("RelativeOrAbsoluteFilePath") != null)
                      {
                          dataDestinationDictionary[element.Attribute("RelativeOrAbsoluteFilePath").Name.ToString()] = element.Attribute("RelativeOrAbsoluteFilePath").Value;
                      }

                  }*/

                //Check For Attributes
                if (element.HasAttributes)
                {
                    if (element.Attribute("RepeatEvery") != null)
                    {
                        dataScheduleDictionary[element.Attribute("RepeatEvery").Name.ToString()] = element.Attribute("RepeatEvery").Value;
                    }
                    if (element.Attribute("DayNumberOfTheWeek") != null)
                    {
                        dataScheduleDictionary[element.Attribute("DayNumberOfTheWeek").Name.ToString()] = element.Attribute("DayNumberOfTheWeek").Value;
                    }
                }


                if (element.HasElements)
                {
                    if (element.Name.ToString().ToUpper().Equals("SPECIFICTIMETRIGGER"))
                    {
                       
                            dataScheduleDictionary[element.Name.ToString()] = element.Value;
                    
                    }
                }
            }


            scheduledTask.JobActive = activeStatus.Trim().ToUpper().Equals("Y") ? true : false;
            scheduledTask.JobName = jobName;
            scheduledTask.Description = description;
            scheduledTask.DataSourceDictionary = dataSourceDictionary;
            scheduledTask.DataDeliveryDictionary = dataDestinationDictionary;
            scheduledTask.scheduleOptionsDictionary = dataScheduleDictionary;

            return scheduledTask;
        }
    }
}
