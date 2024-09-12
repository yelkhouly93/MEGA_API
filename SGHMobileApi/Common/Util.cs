using DataLayer.Data;
using Newtonsoft.Json;
using Otsdc;
using SmartBookingService.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

using RazorPDF;
using SelectPdf;

namespace SGHMobileApi.Common
{
    public static class Util
    {
        public static List<int> OasisBranches = new List<int> { 9 };
        public static List<int> UaeBranches = new List<int> { 301, 302, 303, 304 , 305 , 306 , 307 , 308 , 309 , 310 };

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static IPAddress GetLocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

        }

        public static string GetSexID(string sexType)
        {
            switch (sexType)
            {
                case "F":
                    return "1";
                case "M":
                    return "2";
                case "O":
                    return "3";
                case "U":
                    return "4";
            }
            return "2";
        }

        //public static string SendTestSMS(string mobileNumber, string message)
        //{
        //    String MobileNo = "";

        //    string SMSTestingNumber = "";

        //    SMSTestingNumber = mobileNumber;

        //    //String SMS_Message = "This is test message to check SMS service";
        //    String SMS_Message = message;

        //    //FARAZ//var orc = new OtsdcRestClient("vmkyAM0o9ibI3kfKb0YUnizkGzsB0");
        //    ICollection<KeyValuePair<String, String>> param = new Dictionary<String, String>();

        //    String SMSServiceAPI_Send = ConfigurationManager.AppSettings["SMSServiceAPI_SendSMS"].ToString();
        //    String SMSAppSid = ConfigurationManager.AppSettings["SMSServiceAppSid"].ToString();
        //    String SMSSender = ConfigurationManager.AppSettings["SMSServiceSenderName"].ToString();



        //    try
        //    {
        //        MobileNo = SMSTestingNumber;

        //        string mobileno = SetValidMobileNumber(MobileNo);

        //        if (mobileno != "0")
        //        {
        //            var orc = new OtsdcRestClient("6RLajGRQc3RaveM8cXdeOJ0amNMATy");
        //            var sendSmsMessageResult = orc.SendSmsMessage(mobileno, SMS_Message, SMSSender);

        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return "success";
        //}


        public static string SendTestSMS(string mobileNumber, string message)
        {

            string MobileNo = "";

            string SMSTestingNumber = "";

            SMSTestingNumber = mobileNumber;//ConfigurationManager.AppSettings["SMSTestingNumber"].ToString();

            //String SMS_Message = "This is test message to check SMS service";
            string SMS_Message = message;//ConfigurationManager.AppSettings["SMSTestingText"].ToString();

            //FARAZ//var orc = new OtsdcRestClient("vmkyAM0o9ibI3kfKb0YUnizkGzsB0");
            ICollection<KeyValuePair<String, String>> param = new Dictionary<String, String>();

            string SMSServiceAPI_Send = ConfigurationManager.AppSettings["SMSServiceAPI_SendSMS"].ToString();
            string SMSUserName = ConfigurationManager.AppSettings["SMSServiceUserName"].ToString();
            string SMSPass = ConfigurationManager.AppSettings["SMSServiceUserPass"].ToString();
            string SMSSender = ConfigurationManager.AppSettings["SMSServiceSenderName"].ToString();
            string SMSPriority = ConfigurationManager.AppSettings["SMSServicePriority"].ToString();
            string SMSContryCode = ConfigurationManager.AppSettings["SMSServiceCountryCode"].ToString();



            try
            {
                MobileNo = SMSTestingNumber;

                string mobileno = SetValidMobileNumber(MobileNo);

                if (mobileno != "0")
                {
                    //FARAZ//var sendSmsMessageResult = orc.SendSmsMessage(mobileno, SMS_Message);
                    param.Add(new KeyValuePair<String, String>("user", SMSUserName));
                    param.Add(new KeyValuePair<String, String>("pwd", SMSPass));
                    param.Add(new KeyValuePair<String, String>("senderid", SMSSender));
                    param.Add(new KeyValuePair<String, String>("msgtext", SMS_Message));
                    param.Add(new KeyValuePair<String, String>("mobileno", mobileno));
                    param.Add(new KeyValuePair<String, String>("priority", SMSPriority));
                    param.Add(new KeyValuePair<String, String>("CountryCode", SMSContryCode));

                    clWebClient myRequest = new clWebClient(SMSServiceAPI_Send, "POST", param);

                    string sResponse = myRequest.GetResponse();
                    //XmlDocument xResponse = new XmlDocument();
                    //xResponse.LoadXml(sResponse);
                    string[] s = sResponse.Split(new string[] { "," }, StringSplitOptions.None);
                    //xResponse.SelectSingleNode("/SendSMS/Code").InnerText // TGet the "Code" node of XML
                }
            }
            catch (Exception ex)
            {
                //Logger.LogError(ex);
                if (ex.Message == "No sufficient balance, check you pre-paid messages or top up your wallet and try again")
                {
                    //var getBal = GetBalance();
                }
            }

            return "success";
        }


        public static string CallServicePost(string Url, ICollection<KeyValuePair<String, String>> param)
        {
            try
            {
                clWebClient myRequest = new clWebClient(Url, "POST", param);

                string sResponse = myRequest.GetResponse();
                
                string[] s = sResponse.Split(new string[] { "," }, StringSplitOptions.None);
                //xResponse.SelectSingleNode("/SendSMS/Code").InnerText // TGet the "Code" node of XML

            }
            catch (Exception ex)
            {
                //Logger.LogError(ex);
                if (ex.Message == "No sufficient balance, check you pre-paid messages or top up your wallet and try again")
                {
                    //var getBal = GetBalance();
                }
            }

            return "success";
        }

        public static string CallServiceGet(string Url, ICollection<KeyValuePair<String, String>> param)
        {
            try
            {

                clWebClient myRequest = new clWebClient(Url, "GET", param);


                string sResponse = myRequest.GetResponse();
              
                string[] s = sResponse.Split(new string[] { "," }, StringSplitOptions.None);
              

            }
            catch (Exception ex)
            {
                //Logger.LogError(ex);
                if (ex.Message == "No sufficient balance, check you pre-paid messages or top up your wallet and try again")
                {
                    //var getBal = GetBalance();
                }
            }

            return "success";
        }


        public static string SendSMS_Cairo(string mobileNumber, string message)
        {
            //String MobileNo = "";

            //string SMSTestingNumber = "";

            //SMSTestingNumber = mobileNumber;//ConfigurationManager.AppSettings["SMSTestingNumber"].ToString();

            //String SMS_Message = "This is test message to check SMS service";
            string SMS_Message = message;//ConfigurationManager.AppSettings["SMSTestingText"].ToString();

            //FARAZ//var orc = new OtsdcRestClient("vmkyAM0o9ibI3kfKb0YUnizkGzsB0");
            ICollection<KeyValuePair<String, String>> param = new Dictionary<String, String>();

            string SMSServiceAPI_Send = ConfigurationManager.AppSettings["SMSServiceAPI_SendSMS_Cairo"].ToString();


            try
            {
                string mobileno = SetValidMobileNumber_Cairo(mobileNumber);

                if (mobileno != "0")
                {
                    //FARAZ//var sendSmsMessageResult = orc.SendSmsMessage(mobileno, SMS_Message);
                    param.Add(new KeyValuePair<String, String>("message", SMS_Message));
                    param.Add(new KeyValuePair<String, String>("phone", mobileno));
                    //param.Add(new KeyValuePair<String, String>("msg_id", "22"));

                    

                    clWebClient myRequest = new clWebClient(SMSServiceAPI_Send, "POST", param);

                    string sResponse = myRequest.GetResponse();
                    //XmlDocument xResponse = new XmlDocument();
                    //xResponse.LoadXml(sResponse);
                    string[] s = sResponse.Split(new string[] { "," }, StringSplitOptions.None);
                    //xResponse.SelectSingleNode("/SendSMS/Code").InnerText // TGet the "Code" node of XML
                }
            }
            catch (Exception ex)
            {
                //Logger.LogError(ex);
                if (ex.Message == "No sufficient balance, check you pre-paid messages or top up your wallet and try again")
                {
                    //var getBal = GetBalance();
                }
            }

            return "success";
        }


        public static string SendSMS_UAE(int hospitalId, string mobileNumber, string message)
        {
            string SMS_Message = message;
            ICollection<KeyValuePair<String, String>> param = new Dictionary<String, String>();
            string SMSServiceAPI_Send = ConfigurationManager.AppSettings["SMSServiceAPI_SendSMS_" + hospitalId.ToString()].ToString();
            string sResponse = string.Empty;


            try
            {
                string mobileno = SetValidMobileNumber_UAE(mobileNumber);

                if (mobileno != "0")
                {

                    SMSServiceAPI_Send = SMSServiceAPI_Send.Replace("{mobile}", mobileno);
                    SMSServiceAPI_Send = SMSServiceAPI_Send.Replace("{SMSContent}", message);

                    param.Add(new KeyValuePair<String, String>("message", SMS_Message));

                    log.Info(SMSServiceAPI_Send);
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    clWebClient myRequest = new clWebClient(SMSServiceAPI_Send, "GET", param);

                    sResponse = myRequest.GetResponse();
                   
                    string[] s = sResponse.Split(new string[] { "," }, StringSplitOptions.None);

                    return sResponse;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "No sufficient balance, check you pre-paid messages or top up your wallet and try again")
                {
                    
                }
                return ex.Message;
            }

            return sResponse;
        }



        public static string CleanNumber(string phone)
        {
            Regex digitsOnly = new Regex(@"[^\d]");
            return digitsOnly.Replace(phone, "");
        }

        public static string SetValidMobileNumber(string mobileNumber)
        {
            // remove all non-numeric characters
            string _mobileNumber = CleanNumber(mobileNumber);


            // trim any leading zeros
            _mobileNumber = _mobileNumber.TrimStart(new char[] { '0' });

            // check for this in case they've entered 44 (0)xxxxxxxxx or similar
            //if (_mobileNumber.StartsWith("05"))
            //{
            //    _mobileNumber = _mobileNumber.Remove(2, 1);
            //}

            // add country code if they haven't entered it

            if (_mobileNumber.StartsWith("+"))
            {
                _mobileNumber = _mobileNumber.Remove(0, 1);
            }

            if (!_mobileNumber.StartsWith("966"))
            {
                _mobileNumber = "966" + _mobileNumber;
            }
            // check if it's the right length
            if (_mobileNumber.Length != 12)
            {
                return "0";
                //   return false;
            }
            //return true;
            return _mobileNumber;
        }

        public static string SetValidMobileNumber_Cairo(string mobileNumber)
        {
            // remove all non-numeric characters
            string _mobileNumber = CleanNumber(mobileNumber);


            // trim any leading zeros
            //_mobileNumber = _mobileNumber.TrimStart(new char[] { '0' });

            // check for this in case they've entered 44 (0)xxxxxxxxx or similar
            //if (_mobileNumber.StartsWith("05"))
            //{
            //    _mobileNumber = _mobileNumber.Remove(2, 1);
            //}

            // add country code if they haven't entered it

            if (_mobileNumber.StartsWith("+20"))
            {
                _mobileNumber = _mobileNumber.Remove(0, 1);
            }

            if (_mobileNumber.StartsWith("+"))
            {
                _mobileNumber = _mobileNumber.Remove(0, 1);
            }

            if (!_mobileNumber.StartsWith("01") && _mobileNumber.Length == 9)
            {
                _mobileNumber = "01" + _mobileNumber;
            }

            if (!_mobileNumber.StartsWith("0") && _mobileNumber.Length == 10)
            {
                _mobileNumber = "0" + _mobileNumber;
            }

            // check if it's the right length
            if (_mobileNumber.Length != 11)
            {
                return "0";
                //   return false;
            }
            //return true;
            return _mobileNumber;
        }


        public static string SetValidMobileNumber_UAE(string mobileNumber)
        {
            // remove all non-numeric characters
            string _mobileNumber = CleanNumber(mobileNumber);

            if (_mobileNumber.StartsWith("+"))
            {
                _mobileNumber = _mobileNumber.Remove(0, 1);
            }

            if (!_mobileNumber.StartsWith("971"))
            {
                _mobileNumber = "971" + _mobileNumber;
            }

            // check if it's the right length
            if (_mobileNumber.Length != 12)
            {
                return "0";
                //   return false;
            }
            //return true;
            return _mobileNumber;
        }


        //public static byte[] GetFile()
        //{
        //    String RemoteFtpPath = "ftp://130.1.2.133:21/001_JEDDAH/LAB/LabResult_0_845949_2169619_369.pdf";
        //    String LocalDestinationPath = "LabResult_0_845949_2169619_369.pdf";
        //    String Username = "ftpUserCX";
        //    String Password = "cXu$3r4fTP";
        //    Boolean UseBinary = true; // use true for .zip file or false for a text file
        //    Boolean UsePassive = false;

        //    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(RemoteFtpPath);
        //    request.Method = WebRequestMethods.Ftp.DownloadFile;
        //    request.KeepAlive = true;
        //    request.UsePassive = UsePassive;
        //    request.UseBinary = UseBinary;

        //    request.Credentials = new NetworkCredential(Username, Password);

        //    FtpWebResponse response = (FtpWebResponse)request.GetResponse();

        //    Stream responseStream = response.GetResponseStream();
        //    StreamReader reader = new StreamReader(responseStream);

        //    //return responseStream;
        //    byte[] buffer = new byte[204800];

        //    using (FileStream writer = new FileStream(LocalDestinationPath, FileMode.Create))
        //    {

        //        long length = response.ContentLength;
        //        int bufferSize = 204800;
        //        int readCount;

        //        readCount = responseStream.Read(buffer, 0, bufferSize);
        //        while (readCount > 0)
        //        {
        //            writer.Write(buffer, 0, readCount);
        //            readCount = responseStream.Read(buffer, 0, bufferSize);
        //        }
        //    }

        //    reader.Close();
        //    response.Close();

        //    return buffer;
        //}


        public static byte[] DownloadFileFTP(string _filename, string _filetype)
        {
            string ftphost = Convert.ToString(WebConfigurationManager.AppSettings["FTP_IP"]);
            string ftpuser = Convert.ToString(WebConfigurationManager.AppSettings["FTP_User"]);
            string ftppass = Convert.ToString(WebConfigurationManager.AppSettings["FTP_Pass"]);

            string inputfilepath = @"C:\Temp\" + _filename;
            string ftpfilepath = "/" + _filetype + "/" + _filename;

            string ftpfullpath = "ftp://" + ftphost + ftpfilepath;

            using (WebClient request = new WebClient())
            {
                request.Credentials = new NetworkCredential(ftpuser, ftppass);
                byte[] fileData = request.DownloadData(ftpfullpath);

                using (FileStream file = File.Create(inputfilepath))
                {
                    file.Write(fileData, 0, fileData.Length);
                    file.Close();
                }
                //MessageBox.Show("Download Complete");

                return fileData;
            }
        }

        public static void SendMail(String StrSubject, String strBody, string toEmail)
        {

            //  MailMessage message = new MailMessage(fromEmail, toEmail);
            using (MailMessage message = new MailMessage())
            {
                //bool SentEmail = false;
                string fromEmail = ConfigurationManager.AppSettings["FromEmail"].ToString();
                string emailSever = ConfigurationManager.AppSettings["EmailServer"].ToString();

                message.From = new MailAddress(fromEmail, "Video Call Alert Notification");
                String[] ToEmails = toEmail.Split(',');
                foreach (String email in ToEmails)
                {
                    if (email.Trim() != "")
                    {
                        if (email.Contains("sghgroup.net"))
                        {
                            //SentEmail = true;
                            message.To.Add(email);
                        }

                    }
                }
                
                message.IsBodyHtml = true;
                message.Subject = StrSubject;
                message.Body = strBody + " <br/> <br/> <br/>";
                using (SmtpClient smtp = new SmtpClient(emailSever))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential("HRIS@sghgroup.net", "sghjadm322");
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Port = 25;
                    try
                    {
                        smtp.Send(message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    
                }
            }
        }

        public static string GetUniqID()
        {
            var ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            double t = ts.TotalMilliseconds / 1000;

            int a = (int)Math.Floor(t);
            int b = (int)((t - Math.Floor(t)) * 1000000);

            return a.ToString("x8") + b.ToString("x5");
        }

        public static string GenerateVideoCallUrl(string lang, int hospitaId, int scheduleDayId, int patientId, DateTime timeTo, string doctorName,ref string errMessage, ref int errStatus)
        {
            string generateVideoToken = TokenGenerator.GenerateToken(patientId, timeTo.ToString(), 0);
            string roomKey = Util.GetUniqID();

            string videoUrl = "https://static.vidyo.io/latest/connector/VidyoConnector.html?host=prod.vidyo.io&autoJoin=1&resourceId=" + roomKey + "&displayName=" + doctorName + "&token=" + generateVideoToken;

            PatientDB _patientDb = new PatientDB();
            _patientDb.UpdateVideoCallURL(lang, hospitaId, scheduleDayId, videoUrl, ref errMessage, ref errStatus);

            return videoUrl;

        }

        public static bool CheckCallingAccess(string ClientId , string API)
        {
            if (ClientId == "26620230-971B-4D0E-998A-D2C67FAEC96A")
                return true;
            
            return false;
        }

        public static string GetNewName()
        {
            string sName = Convert.ToString(DateTime.Now.Ticks);
            return (sName);
        }
        public static string ConvertURL_to_PDF(string HTMLURL , string TestID)
        {
            string sFileName = GetNewName() + "_" + TestID;            
            string sUrl = HTMLURL;
            //for testing Pexera 
            //sUrl = "https://130.1.11.136:443//Integrator.aspx?uname=his&pwd=His12345&StudyUID=1.2.840.4892943.343.20220111124110430.1341&AccNo=SGHJ267673&StudyID=SGHJ267673&ZFP=True&Branch=Jeddah";

            string url = sUrl;
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                ConfigurationManager.AppSettings["pdf_page_size"].ToString(), true);
            PdfPageOrientation pdfOrientation =
                (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation),
                ConfigurationManager.AppSettings["pdf_orientation"].ToString(), true);

            HtmlToPdf converter = new HtmlToPdf();

            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            //converter.Options.WebPageWidth = webPageWidth;
            //converter.Options.WebPageHeight = webPageHeight;
            converter.Options.WebPageWidth = Convert.ToInt32(ConfigurationManager.AppSettings["webPageWidth"].ToString());
            converter.Options.WebPageHeight = Convert.ToInt32(ConfigurationManager.AppSettings["webPageHeight"].ToString());

            PdfDocument doc = converter.ConvertUrl(url);
            string PDFPath = ConfigurationManager.AppSettings["PDFPath"].ToString() + sFileName + ".pdf";



            //// AHSAN NEW LOGIC CHANGE 31-01-2023 FOR SAVING USING FTP
            ////************** NEW LOGIC TO UPLOAD FILE USING FTP **************//            
            //string UploadPath = ConfigurationManager.AppSettings["PDFPath"].ToString();
            //string FileName = sFileName + ".pdf";
            //byte[] fileBytes = doc.Save();
            //string ftpAddress = ConfigurationManager.AppSettings["DocProfile_IP"].ToString();
            //string username = ConfigurationManager.AppSettings["DocProfile_Name"].ToString();
            //string password = ConfigurationManager.AppSettings["DocProfile_string3"].ToString() + ConfigurationManager.AppSettings["DocProfile_string1"].ToString() + ConfigurationManager.AppSettings["DocProfile_string2"].ToString();


            //WebRequest request = WebRequest.Create("ftp://" + ftpAddress + "/" + UploadPath + "/" + FileName);
            //request.Method = WebRequestMethods.Ftp.UploadFile;
            //request.Credentials = new NetworkCredential(username, password);
            //Stream reqStream = request.GetRequestStream();
            //reqStream.Write(fileBytes, 0, fileBytes.Length);
            //reqStream.Close();
            ////************** NEW LOGIC TO UPLOAD FILE USING FTP **************//

            doc.Save(PDFPath);
            doc.Close();





            string pdfurl = ConfigurationManager.AppSettings["pdfurl"].ToString() + sFileName + ".pdf";
            return pdfurl;
        }


        public static string Convert_Base64_to_PDF(string base64String, string TestID)
        {
            string sFileName = GetNewName() + "_" + TestID;
            if (string.IsNullOrWhiteSpace(base64String))
            {
                //throw new ArgumentException("Base64 string cannot be null or empty.", nameof(base64String));
                Console.WriteLine($"Base64 string cannot be null or empty. {nameof(base64String)}");
                return "";
            }


            string PDFPath = ConfigurationManager.AppSettings["PDFPath"].ToString() + sFileName + ".pdf";


            try
            {
                // Decode the Base64 string
                byte[] pdfData = Convert.FromBase64String(base64String);

                // Write the byte array to the specified file path
                File.WriteAllBytes(PDFPath, pdfData);

                string pdfurl = ConfigurationManager.AppSettings["pdfurl"].ToString() + sFileName + ".pdf";
                Console.WriteLine("PDF file saved successfully.");

                return pdfurl;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                //throw; // Re-throw the exception to handle it further up the call stack if needed
                return "";
            }


            return "";


            
        }

        public static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

    }
}