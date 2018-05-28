using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDistributionService.Model
{
    public class OutputFileSettings
    {
       // public FileType TypeOfFile { get; set; }
        public FileExtenson ExtensionOfFile { get; set; }

        public string FileName { get; set; }
        public string FileOutputPath { get; set; }

        public FileNameTimeStampOptions FileTimeStampOptions { get; set; }
    }
}
