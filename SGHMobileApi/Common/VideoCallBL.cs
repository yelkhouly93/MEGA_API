using AgoraIO.Media;
using DataLayer.Common;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace VideoCall.BusinessLayer
{
    public class VideoCallBL
    {
        //private CustomDBHelper _DB = new CustomDBHelper();
        
        public string GenerateVideoURL_Token_Test(string ChannelName, int startTime, int ExpireTime, long UID)
        {
            //string appID = "0fe4ac748cd24a9fb3ba4730ea201df9";
            //string appCertificate = "3a313f24a1f243d19be7ac04ef3856af";
            //int ts = 1645689245;
            //int r = 123456789;
            //long uid = 0;
            //int expiredTs = 1645775645;

            string appID = ConfigurationManager.AppSettings["Agora_APPID"].ToString();
            string appCertificate = ConfigurationManager.AppSettings["Agora_APPCertificate"].ToString();

            int ts = startTime;
            int r = 123456789;
            long uid = UID;
            int expiredTs = ExpireTime;


            string Token = DynamicKey3.generate(appID, appCertificate, ChannelName, ts, r, uid, expiredTs);
            _ = DynamicKey3.generate("", "", "", 0, 0, 0, 0);

            return Token;
        }
    }
}
