using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DataLayer.Common;
using DataLayer.Model;

using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace DataLayer.Data
{
	public class AppReminder
	{
		private readonly CustomDBHelper _db = new CustomDBHelper("RECEPTION");


        public DataTable GetWaterReminder_List(string Lang, int hospitalId, int registrationNo)
        {
            _db.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@PatientMRN", registrationNo)
            };

            var dt = _db.ExecuteSPAndReturnDataTable("DBO.[Get_WaterReminderList_SP]");

            return dt;
        }

        public void SaveWaterReminder(string Lang,int BranchID, int RegistrationNo, int ReminderHour, DateTime FromTime, DateTime ToTime,string Source,  ref int errStatus, ref string errMessage)
        {
            _db.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@BranchId", BranchID),
                new SqlParameter("@PatientMRN", RegistrationNo),
                new SqlParameter("@ReminderHour", ReminderHour),
                new SqlParameter("@FromTime", FromTime),
                new SqlParameter("@ToTime", ToTime),
                new SqlParameter("@Source", Source),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 1000)
            };

            _db.param[7].Direction = ParameterDirection.Output;
            _db.param[8].Direction = ParameterDirection.Output;

            _db.ExecuteSPAndReturnDataTable("Save_APP_WaterRemninder_SP");

            errStatus = Convert.ToInt32(_db.param[7].Value);
            errMessage = _db.param[8].Value.ToString();

            return ;
        }

        public DataTable UpdateAppRating(int RatingID, decimal StarRate, string Comments, ref int errStatus, ref string errMessage)
        {
            _db.param = new SqlParameter[]
            {

                new SqlParameter("@RatingID", RatingID),
                new SqlParameter("@StarRate", StarRate),
                new SqlParameter("@Comments", Comments),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 500)
            };

            _db.param[3].Direction = ParameterDirection.Output;
            _db.param[4].Direction = ParameterDirection.Output;

            var dataTable = _db.ExecuteSPAndReturnDataTable("Save_APP_ScreenRating_SP");

            errStatus = Convert.ToInt32(_db.param[3].Value);
            errMessage = _db.param[4].Value.ToString();

            return dataTable;
        }

        public void CancelWaterReminder(string Lang, int BranchID, int RegistrationNo,string Source, ref int errStatus, ref string errMessage)
        {
            _db.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@BranchId", BranchID),
                new SqlParameter("@PatientMRN", RegistrationNo),
                new SqlParameter("@Source", Source),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 1000)
            };

            _db.param[4].Direction = ParameterDirection.Output;
            _db.param[5].Direction = ParameterDirection.Output;

            _db.ExecuteSPAndReturnDataTable("Cancel_APP_WaterRemninder_SP");

            errStatus = Convert.ToInt32(_db.param[4].Value);
            errMessage = _db.param[5].Value.ToString();

            return ;
        }

    }
}
