using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace SmartBookingService.Common
{
    public static class TokenGenerator
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const long EPOCH_SECONDS = 62167219200;
        static UTF8Encoding encoder = new UTF8Encoding();
        static string serialized = string.Empty;

        public static string AppId()
        {
            string AppId = ConfigurationManager.AppSettings["MobileWebApi_AppId"].ToString();
            return AppId;
        }

        public static string ApplicationKey()
        {
            string ApplicationKey = ConfigurationManager.AppSettings["MobileWebApi_ApplicationKey"].ToString();
            return ApplicationKey;
        }

        public static string GenerateToken(int userId, string expiresAt, long expiresInSecs)
        {
            string appID = AppId();
            string key = ApplicationKey();
            string userName = "user" + userId.ToString();
            string macHex = string.Empty;

            if ((appID != null) && (key != null) && (userName != null))
            {
                string expires = "";

                // Check if using expiresInSecs or expiresAt
                if (expiresInSecs > 0)
                {
                    TimeSpan timeSinceEpoch = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
                    expires = (Math.Floor(timeSinceEpoch.TotalSeconds) + EPOCH_SECONDS + expiresInSecs).ToString();
                }
                else if (expiresAt != null)
                {
                    try
                    {
                        TimeSpan epochToExpires = DateTime.Parse(expiresAt).ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
                        expires = (Math.Floor(epochToExpires.TotalSeconds + EPOCH_SECONDS)).ToString();
                    }
                    catch (Exception e)
                    {
                        log.Error("\nException caught in expiresAt time calculation. Time format probably invalid. Should look like so: 2055-10-27T10:54:22Z");
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    log.Error("\nExiting! Neither expiresInSecs or expiresAt was set.");
                }

                log.Info("Setting key           : " + key);
                log.Info("Setting appId         : " + appID);
                log.Info("Setting userName      : " + userName);
                log.Info("Setting expiresInSecs : " + expiresInSecs);
                if (expiresAt != null)
                    log.Info("Setting expiresAt     : " + expiresAt);
                log.Info("Expirey time          : " + expires);
                string jid = userName + "@" + appID;
                string body = "provision" + "\0" + jid + "\0" + expires + "\0" + "";

                
                var hmacsha = new HMACSHA384(encoder.GetBytes(key));
                byte[] mac = hmacsha.ComputeHash(encoder.GetBytes(body));

                // Get the hex version of the mac
                macHex = BytesToHex(mac);

                serialized = body + '\0' + macHex;

                log.Info("\nGenerated token:\n" + Convert.ToBase64String(encoder.GetBytes(serialized)));

            }
            return Convert.ToBase64String(encoder.GetBytes(serialized));
        }
        private static string BytesToHex(byte[] bytes)
        {
            var hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

    }
}