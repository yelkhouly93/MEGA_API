using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.SqlClient;
using DataLayer.Common;
using SGHMobileApi.Common;


using System.Data;
using System.Globalization;

namespace SGHMobileApi.Common
{
    static public class DataLog_DB
    {

        static public void ADDLOGS_DB(string ApiName, string Description, string UserID, string ApiMethod, string Comments)
        {
            CustomDBHelper DB = new CustomDBHelper("RECEPTION");

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@P1", ApiName),
                new SqlParameter("@P2", UserID),
                new SqlParameter("@P3", ApiMethod),
                new SqlParameter("@P4", Comments)
            };

            DB.ExecuteSP("Save_APIV3Logs_SP");
        }





        static public void SAVE_API_CALL_LOGS_DB(string ApiName, string Method, string URL, string UserAgent, string MessageBody,string UserName,string UserRole)
        {
            CustomDBHelper DB = new CustomDBHelper("RECEPTION");

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@ApiName", ApiName),
                new SqlParameter("@Method", Method),
                new SqlParameter("@URL", URL),
                new SqlParameter("@UserAgent", UserAgent),
                new SqlParameter("@MessageBody", MessageBody),
                new SqlParameter("@UserName", UserName),
                new SqlParameter("@UserRole", UserRole)                
            };

            DB.ExecuteSP("API_CALL_Logging");
        }




        static public bool CheckAccess(string ApiName, string UserID )
        {
            //return false;

            int Er_Count = 1;
            
            CustomDBHelper DB = new CustomDBHelper("RECEPTION");
            
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@ApiName", ApiName),
                new SqlParameter("@UserID", UserID),
                new SqlParameter("@Er_Status", SqlDbType.Int),
                new SqlParameter("@Msg", SqlDbType.VarChar,100)
            };
            DB.param[2].Direction = ParameterDirection.Output;
            DB.param[3].Direction = ParameterDirection.Output;


            DB.ExecuteSP("Chk_UserAccess_SP_APIV3");


            Er_Count = Convert.ToInt32(DB.param[2].Value);
            var msg = DB.param[3].Value.ToString();
            return Er_Count != 1;
            
        }


    }
}