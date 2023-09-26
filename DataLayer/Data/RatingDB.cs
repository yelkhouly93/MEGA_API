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
    public class RatingDB
    {
        private readonly CustomDBHelper _db = new CustomDBHelper("RECEPTION");

        public DataTable GetUserRating_List(string Lang, int hospitalId, int registrationNo )
        {
            _db.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@BranchId", hospitalId),                
                new SqlParameter("@RegistrationNo", registrationNo)                
            };
            //_db.param[3].Direction = ParameterDirection.Output;
            //_db.param[4].Direction = ParameterDirection.Output;            

            var dt = _db.ExecuteSPAndReturnDataTable("DBO.[Get_UserRatingList_SP]");

            //erStatus = Convert.ToInt32(_db.param[3].Value);
            //msg = _db.param[4].Value.ToString();            

            return dt;
        }

        public DataTable SaveAppRating(string Lang, int RatingID,int BranchID,int RegistrationNo, string ScreenName, decimal StarRate, string Comments,bool Compeleted,int ServiceRecordID, DateTime ServiceDate, ref int errStatus, ref string errMessage)
        {
            _db.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@RatingID", RatingID),
                new SqlParameter("@BranchId", BranchID),
                new SqlParameter("@RegistrationNo", RegistrationNo),
                new SqlParameter("@ScreenName", ScreenName),
                new SqlParameter("@StarRate", StarRate),
                new SqlParameter("@Comments", Comments),
                new SqlParameter("@Compeleted", Compeleted),
                new SqlParameter("@ServiceRecordID", ServiceRecordID),
                new SqlParameter("@ServiceDate", ServiceDate),                
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200)
            };

            _db.param[10].Direction = ParameterDirection.Output;
            _db.param[11].Direction = ParameterDirection.Output;

            var dataTable = _db.ExecuteSPAndReturnDataTable("Save_APP_ScreenRating_SP");

            errStatus = Convert.ToInt32(_db.param[10].Value);
            errMessage = _db.param[11].Value.ToString();

            return dataTable;
        }

        public DataTable UpdateAppRating(int RatingID, decimal StarRate, string Comments,ref int errStatus, ref string errMessage)
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

    }
}
