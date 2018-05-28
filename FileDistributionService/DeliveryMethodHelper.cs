using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileDistributionService.Helper;
using FileDistributionService.Model;
using log4net;

namespace FileDistributionService
{
    public class DeliveryMethodHelper
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool ProcessDeliveryMethod(Dictionary<string,object> deliveryMethodDictionary, string fileName)
        {
            log.Info("Destination Process Processing Started");
            bool status = false;
            var securityTypeForDeliveryMethod = (AccessSecurityType)Convert.ToInt16(HelperMethods.DictionaryLookUp(deliveryMethodDictionary, "SecurityTypeForDataSource"));
            var deliveryMethod = (DeliveryMethod)Convert.ToInt16(HelperMethods.DictionaryLookUp(deliveryMethodDictionary, "DeliveryMethod"));


            if (securityTypeForDeliveryMethod == AccessSecurityType.Ssl)
            {
                
            }
            switch (deliveryMethod)
            {

                case DeliveryMethod.FtpOrSftp:
                    {
                        log.Info("FtpOrSftp Destination Process Initiated");
                        status = ProcessSftp(deliveryMethodDictionary, fileName);
                        break;
                    }
               
                case DeliveryMethod.Database:
                    {
                        log.Info("Database Destination Process Initiated");
                        break;
                    }
                case DeliveryMethod.DropBox:
                    {
                        log.Info("Dropbox Destination Process Initiated");
                        break;
                    }
                case DeliveryMethod.Email:
                    {
                        log.Info("Email Destination Process Initiated");
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return status;
        }

        public static bool ProcessSftp(Dictionary<string, object> deliveryMethodDictionary, string fileName)
        {
           FtpWrapper ftpWrapper = new FtpWrapper();
            ftpWrapper.SftpOrFtpTransfer(deliveryMethodDictionary, fileName);


            return true;
        }
    }
}
