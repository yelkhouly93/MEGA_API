using DataLayer.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DataLayer.Data.DoctorAPI
{
	public class DoctorAPIDB
	{

        CustomDBHelper _db = new CustomDBHelper("RECEPTION");

        public DataTable Validate_DotorInfo (string lang, string hospitalId, string EmployeeID, string NationalID, string ApiSources , ref int erStatus, ref string msg)
        {
            _db.param = new SqlParameter[]
           {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@EmpID", EmployeeID),
                new SqlParameter("@NationalID", NationalID),
                new SqlParameter("@Er_Status", SqlDbType.Int),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 500)
           };

            _db.param[4].Direction = ParameterDirection.Output;
            _db.param[5].Direction = ParameterDirection.Output;

            string DB_SP_Name = "DocApi.[Validate_DoctorInfo]";
            var allDataTableModel = _db.ExecuteSPAndReturnDataTable(DB_SP_Name);
            
            erStatus = Convert.ToInt32(_db.param[4].Value);
            msg = _db.param[5].Value.ToString();

            return allDataTableModel;

        }

        public DataTable GetDoctorInformation(string lang, string hospitalId, string EmployeeID, string ApiSources)
        {
            _db.param = new SqlParameter[]
           {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@EmpID", EmployeeID)                
           };

            string DB_SP_Name = "DocApi.[Get_DoctorsProfile_SP]";
            var allDataTableModel = _db.ExecuteSPAndReturnDataTable(DB_SP_Name);
            
            return allDataTableModel;
        }

    }
}
