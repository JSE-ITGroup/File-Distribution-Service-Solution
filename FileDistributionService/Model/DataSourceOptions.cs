using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDistributionService.Model
{
    public class DataSourceOptions
    {
        public DataSourceType TypeOfDataSource { get; set; }
        public DataSourceDbProvider DbProviderForDatabaseDataSource { get; set; }
        public AccessSecurityType SecurityTypeForDataSource { get; set; }
        public DataRetrievalMethod DataRetrievalMethod { get; set; }

        public object ValueForDataRetrievalMethod { get; set; }

        public DataConnectionType DataConnectionType { get; set; }

        public object ValueForDataConnectionType { get; set; }

        public OutputFileSettings OutputFileOptions { get; set; }

        public Dictionary<string,object> NoneDatabaseSourceProperty { get; set; }
    }
}
