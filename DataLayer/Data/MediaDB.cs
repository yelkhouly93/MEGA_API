using DataLayer.Common;
using DataLayer.Model;
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


        //for Video Call UAE testing

          public VideoCallURL GETVideoURL (string AppointmentID , string HospitalID , string MRN)
		{
            CustomDBHelper _DB = new CustomDBHelper("RECEPTION");

            //var strQry2 = "Select BranchCode  from BAS_Branch_TB where HIS_Id = " + HospitalID; 
            //var str_ID = _DB.ExecuteSQLScalar(strQry2);


            //string strQry = "Select id from VideoCallDetails where AppointmentID = " + AppointmentID + " and BranchID = " + HospitalID;
            //str_ID = _DB.ExecuteSQLScalar(strQry);

            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@BranchID", HospitalID),                    
                    new SqlParameter("@AppointmentID", AppointmentID)
                };
            var DtResults
                = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_VideoCall_INFO_Zoom_SP]").ToModelObject<VideoCallURL>();



            return DtResults;
            
		}


    }
}
