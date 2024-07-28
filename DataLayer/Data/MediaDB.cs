using DataLayer.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DataLayer.Data
{
    public class MediaDB
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");

        public DataTable GetNewsAnnoucementDT(string lang, string hospitalID, int ContentTypeID)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalID),
                    new SqlParameter("@ContentTypeId", ContentTypeID)
                };
            var DtResults
                = DB.ExecuteSPAndReturnDataTable("DBO.[Get_NewsAndNotifications_SP]");

            return DtResults;

        }


        public DataTable GetNewsAnnoucementDT_V4(string lang, string hospitalID, int ContentTypeID , string CountryId)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalID),
                    new SqlParameter("@ContentTypeId", ContentTypeID)
                };
            var DtResults
                = DB.ExecuteSPAndReturnDataTable("DBO.[Get_NewsAndNotifications_SP]");

            return DtResults;

        }

        public DataTable GetMediaDT(string lang, string hospitalID, int ContentTypeID)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalID),
                    new SqlParameter("@ContentTypeId", ContentTypeID)
                };
            var DtResults
                = DB.ExecuteSPAndReturnDataTable("DBO.[Get_Media_SP]");

            return DtResults;

        }
    }
}
