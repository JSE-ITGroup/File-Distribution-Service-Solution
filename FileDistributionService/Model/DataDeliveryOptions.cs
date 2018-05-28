using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDistributionService.Model
{
    public class DataDeliveryOptions
    {
        public DeliveryMethod DeliveryMethod { get; set; }
        public AccessSecurityType SecurityTypeForDeliveryMethod { get; set; }
        public Dictionary<string,object> DeliveryDestinationProperty { get; set; }

    }
}
