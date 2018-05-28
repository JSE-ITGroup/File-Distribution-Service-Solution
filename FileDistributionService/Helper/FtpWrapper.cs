using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using WinSCP;

namespace FileDistributionService.Helper
{
    public class FtpWrapper
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public List<string> messages;

        public FtpWrapper()
        {

        }
       
        public void SftpOrFtpTransfer(Dictionary<string, object> ftpProperty, string fileName)
        {


            var protocalOptions = (ProtocolOptions) Convert.ToInt16(HelperMethods.DictionaryLookUp(ftpProperty, "FtpProtocol"));
            var ftpHost= (string)HelperMethods.DictionaryLookUp(ftpProperty, "FtpHost");
            var portNumber= (int?) Convert.ToInt16(HelperMethods.DictionaryLookUp(ftpProperty, "FtpPort"));
            var userName = (string) HelperMethods.DictionaryLookUp(ftpProperty, "FtpUserName");
            var password = (string)HelperMethods.DictionaryLookUp(ftpProperty,"FtpPassword");
            var passphrase = (string)HelperMethods.DictionaryLookUp(ftpProperty, "FtpPassPhrase");
            var encrypmethod = (EncryptionMethod)Convert.ToInt16(HelperMethods.DictionaryLookUp(ftpProperty, "FtpEncryptionMethod"));
            var auth = (AuthenticationMethod)Convert.ToInt16(HelperMethods.DictionaryLookUp(ftpProperty, "FtpAuthenticationMethod"));
            var remotedir = (string)HelperMethods.DictionaryLookUp(ftpProperty, "FtpRemoteDirectory");
            var transferModeOption = (TransferModeOptions)Convert.ToInt16(HelperMethods.DictionaryLookUp(ftpProperty, "FtpTransferMode"));
            var ftpModeOption = (FtpModeOptions)Convert.ToInt16(HelperMethods.DictionaryLookUp(ftpProperty, "FtpMode"));
            var singleOrMultipleFilesUpload = (SingleFileOrMultipleFilesUpload)Convert.ToInt16(HelperMethods.DictionaryLookUp(ftpProperty, "SingleOrMultipleFilesUpload"));
            var removeSourceFile = (bool)HelperMethods.DictionaryLookUp(ftpProperty, "RemoveSourceFile").Equals("Y")?true:false;
            var sshHostKeyFingerprint = (string)HelperMethods.DictionaryLookUp(ftpProperty,"SshHostKeyFingerprint");
            string sshPrivateKeyPath = null;// Path.Combine(AppDomain.CurrentDomain.BaseDirectory, (string)HelperMethods.DictionaryLookUp(ftpProperty,"SshPrivateKeyFilePath"));
            var tlsHostCertificateFingerprint = (string)HelperMethods.DictionaryLookUp(ftpProperty,"TlsHostCertificateFingerprint");
            var relativeOrAbsoluteFilePath = string.IsNullOrEmpty((string)HelperMethods.DictionaryLookUp(ftpProperty, "RelativeOrAbsoluteFilePath")) ? "R" : (string)HelperMethods.DictionaryLookUp(ftpProperty, "RelativeOrAbsoluteFilePath");





            var requirePrivateKeyPassPhrase = (bool)HelperMethods.DictionaryLookUp(ftpProperty, "RequirePrivateKeyPassPhrase").Equals("Y") ? true : false;
            var requireSshHostKeyFingerprint = (bool)HelperMethods.DictionaryLookUp(ftpProperty, "RequireSshHostKeyFingerprint").Equals("Y") ? true : false;
            var requireSshPrivateKey = (bool)HelperMethods.DictionaryLookUp(ftpProperty, "RequireSshPrivateKey").Equals("Y") ? true : false;
            var requireTlsClientCertificate = (bool)HelperMethods.DictionaryLookUp(ftpProperty, "RequireTlsClientCertificate").Equals("Y") ? true : false;
            var requireTlsHostCertificateFingerprint = (bool)HelperMethods.DictionaryLookUp(ftpProperty, "RequireTlsHostCertificateFingerprint").Equals("Y") ? true : false;

            var path = Path.GetDirectoryName(fileName);
            var filextensionpattern = string.Format("*{0}*", Path.GetExtension(fileName));

            if (relativeOrAbsoluteFilePath.ToUpper().Equals("R"))
            {
                 sshPrivateKeyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, (string)HelperMethods.DictionaryLookUp(ftpProperty, "SshPrivateKeyFilePath"));

            }
            else
            {
                sshPrivateKeyPath =  (string)HelperMethods.DictionaryLookUp(ftpProperty, "SshPrivateKeyFilePath");

            }
            try
            {
                SessionOptions sessionOptions = new SessionOptions();
                sessionOptions.HostName = ftpHost;
                sessionOptions.PortNumber = (int)portNumber;

                if (protocalOptions == ProtocolOptions.Ftp)
                {


                    sessionOptions.Protocol = WinSCP.Protocol.Ftp;
                    sessionOptions.UserName = userName;
                    sessionOptions.Password = password;

                    //Encryption Method
                    if (encrypmethod == EncryptionMethod.None)
                    {
                        sessionOptions.FtpSecure = FtpSecure.None;
                    }
                    else if (encrypmethod == EncryptionMethod.Explicit)
                    {
                        sessionOptions.FtpSecure = FtpSecure.Explicit;
                        if (requireTlsHostCertificateFingerprint)
                        {
                            sessionOptions.TlsHostCertificateFingerprint = tlsHostCertificateFingerprint;
                        }
                    }
                    else if (encrypmethod == EncryptionMethod.Implicit)
                    {
                        sessionOptions.FtpSecure = FtpSecure.Implicit;
                    }


                }
                else if (protocalOptions == ProtocolOptions.Sftp)
                {

                    sessionOptions.Protocol = WinSCP.Protocol.Sftp;
                    sessionOptions.UserName = userName;
                    if (requireSshHostKeyFingerprint)
                    {
                        sessionOptions.SshHostKeyFingerprint = sshHostKeyFingerprint;
                    }

                    if (auth == AuthenticationMethod.Password)
                    {
                        sessionOptions.Password = password;


                    }
                    else if (auth == AuthenticationMethod.PublicKeyWithPassPhrase)
                    {
                        if (requireSshPrivateKey)
                        {
                            sessionOptions.SshPrivateKeyPath = sshPrivateKeyPath;
                        }
                        if (requirePrivateKeyPassPhrase)
                        {
                            sessionOptions.PrivateKeyPassphrase = passphrase;
                        }
                    }

                    else if (auth == AuthenticationMethod.PublicKey)
                    {
                        if (requireSshPrivateKey)
                        {
                            sessionOptions.SshPrivateKeyPath = sshPrivateKeyPath;
                        }

                    }
                }


               

                //Transfer Method
                if (ftpModeOption == FtpModeOptions.Active)
                {
                    sessionOptions.FtpMode = FtpMode.Active;
                }
                else if (ftpModeOption == FtpModeOptions.Passive)
                {
                    sessionOptions.FtpMode = FtpMode.Passive;
                }


                using (Session session = new Session())
                {
                    // Connect
                    try
                    {
                        session.Open(sessionOptions);
                    }
                    catch (Exception exp)
                    {
                        log.Error("Connection Error:",exp);
                        throw;
                    }
                  

                    // Upload files
                    TransferOptions transferOptions = new TransferOptions();
                    if (transferModeOption == TransferModeOptions.Binary)
                    {
                        transferOptions.TransferMode = TransferMode.Binary;

                    }
                    else if (transferModeOption == TransferModeOptions.Ascii)
                    {
                        transferOptions.TransferMode = TransferMode.Ascii;

                    }
                    else if (transferModeOption == TransferModeOptions.Automatic)
                    {
                        transferOptions.TransferMode = TransferMode.Automatic;
                    }


                  

                    //Transfer Objects

                    string[] files = Directory.GetFiles(path, filextensionpattern, SearchOption.AllDirectories);
                    log.Info(string.Format("{0} files: found in output Directory that matches the Pattern: {1}", files.Count(), filextensionpattern));

                    if (!files.Any())
                    {
                        log.Info(string.Format("File: {0} could not be found", fileName));
                        log.Info("There are no files available to be uploaded. Process will be terminated");
                      //  return;
                    }

                        //Iterate Throught Each Files.
                        foreach (var file in files)
                    {


                        if (singleOrMultipleFilesUpload == SingleFileOrMultipleFilesUpload.SingleFile)
                        {
                            if (!file.Equals(fileName))
                            {
                                log.Info("SKIPPING File: " + file);
                                continue;
                            }
                        }

                        TransferOperationResult transferResult;
                        try
                        {
                            transferResult = session.PutFiles(file, remotedir, removeSourceFile, transferOptions);
                            log.Info(string.Format("Uploading File {0} ", file));
                        }
                        catch (Exception e)
                        {

                            log.Error(e.Message);
                            continue;
                        }
                        



                     



                        // Throw on any error
                        transferResult.Check();

                        // Print results
                        messages = new List<string>();
                        foreach (TransferEventArgs transfer in transferResult.Transfers)
                        {
                            log.Info(string.Format("File:  {0} was succesfully Uploaded.", transfer.FileName));
                            messages.Add(string.Format("File:  {0} was succesfully Uploaded.", transfer.FileName));
                        }
                    }
                }


            }

            catch (Exception e)
            {
                log.Error(e.Message);
            }

        }

    }

    public enum ProtocolOptions
    {
        Ftp = 0,
        Sftp = 1,
        Scp = 3,
        WebDav = 2
    }

    public enum FtpModeOptions
    {
        Passive = 1,
        Active = 0
    }
    public enum TransferModeOptions
    {
        Binary = 1,
        Ascii = 2,
        Automatic = 0
    }
    public enum AuthenticationMethod
    {
        None = 0,
        Password = 1,
        PublicKey = 2,
        PublicKeyWithPassPhrase = 3
    }
    public enum EncryptionMethod
    {
        None = 0,
        Implicit = 1,
        Explicit = 2
    }

    public enum SingleFileOrMultipleFilesUpload
    {
        SingleFile = 0,
        MultipleFiles = 1

    }
}
