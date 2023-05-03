using DataLayer.Common;
using DataLayer.Model;
using DataLayer.Reception.Business;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DataLayer.Reception.Business
{
    public class LoggerDB
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");
        Response res = new Response();

        public void RequestAndResponseLogging(LogMetadata logMetadata)
        {
            StringBuilder query = new StringBuilder();

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@RequestContent", logMetadata.RequestContent),
                new SqlParameter("@RequestContentType", logMetadata.RequestContentType),
                new SqlParameter("@RequestMethod", logMetadata.RequestMethod),
                new SqlParameter("@RequestUri", logMetadata.RequestUri),
                new SqlParameter("@RequestTimestamp", logMetadata.RequestTimestamp),
                new SqlParameter("@ResponseContent", logMetadata.ResponseContent),
                new SqlParameter("@ResponseContentType", logMetadata.ResponseContentType),
                new SqlParameter("@ResponseStatusCode", logMetadata.ResponseStatusCode),
                new SqlParameter("@ResponseStatusMessage", logMetadata.ResponseStatusMessage),
                new SqlParameter("@ResponseTimestamp", logMetadata.ResponseTimestamp)
            };

            DB.ExecuteNonQuerySP("dbo.API_LogRequestResponse_SP");

        }
    }



    public class ApiLogging
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");
        Response res = new Response();
        public void InsertLog(ApiLog apiLog)
        {
            StringBuilder query = new StringBuilder();

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Host", apiLog.Host),
                new SqlParameter("@Headers", apiLog.Headers),
                new SqlParameter("@StatusCode", apiLog.StatusCode),
                new SqlParameter("@RequestBody", apiLog.RequestBody),
                new SqlParameter("@RequestedMethod", apiLog.RequestedMethod),
                new SqlParameter("@UserHostAddress", apiLog.UserHostAddress),
                new SqlParameter("@Useragent", apiLog.Useragent),
                new SqlParameter("@AbsoluteUri", apiLog.AbsoluteUri),
                new SqlParameter("@RequestType", apiLog.RequestType)
            };

            DB.ExecuteNonQuerySP("dbo.API_V3_Logging");
        }
    }

}
