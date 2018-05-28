File Distribution Services
This is an application that pulls data from various data source to destribute  using different destination using several delivery methods.
Jobs are defined in an XML files.
Each Job consist of a Datasources section, a data destination section and  scheduling options.
The datasources sections contains one or more datasource.
Each datasource cantains a data an file output format section. The file output format schedule  contains the file name, types or type of file and output directory.
Each job can  have only one destination and one scheduleing option.

Datasource include:
SQl Server Database, MySql Database or Oracle Database using  SQLNative, Odbc or oledb data providers
Delivery methods include:  FTP,SFTP,Email, Drop and Database
The Scheduling section.
Determine if the frequently when the job is to be executed such as Daily, Weekly, Monthly or based on Intervals.
The application  runs as  windows service and  executes a job every five minutes(other based on time defined in config file).


 Dependencies/Libraries
 Winscp.dll support sftp transfers
 SS.net support SSh Connections
 SqlDataUtility the Database Connector.
 Microsoft.Office.Interop.Exce;
 CLose XMl
 Log4net.