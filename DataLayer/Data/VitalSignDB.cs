using DataLayer.Common;
using DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace DataLayer.Data
{
	public class VitalSignDB
	{
		CustomDBHelper DB = new CustomDBHelper("RECEPTION");

        public DataTable GET_Patient_VitalSign_GraphData(string Lang, int PatientMRN, int BranchID, string Source)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", Lang),
                    new SqlParameter("@RegistrationNo", PatientMRN),
                    new SqlParameter("@BranchID", BranchID),
                    new SqlParameter("@Source", Source),                    
                    new SqlParameter("@Er_Status", SqlDbType.Int),
                    new SqlParameter("@msg", SqlDbType.NVarChar, 1000)
                };
            DB.param[6].Direction = ParameterDirection.Output;
            DB.param[7].Direction = ParameterDirection.Output;

            var ReturnDataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_PatientVitals_SP]");
            return ReturnDataTable;
        }


        public DataTable GET_Patient_My_VitalSign_DT(string Lang, int PatientMRN, int BranchID)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", Lang),
                    new SqlParameter("@RegistrationNo", PatientMRN),
                    new SqlParameter("@BranchID", BranchID)                    
                };

            var ReturnDataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_PatientMyVitals_SP]");
            return ReturnDataTable;
        }

        public DataTable GET_Patient_My_VitalSign_Detail_DT(string Lang, int PatientMRN, int BranchID ,  string VitalSign)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", Lang),
                    new SqlParameter("@RegistrationNo", PatientMRN),
                    new SqlParameter("@BranchID", BranchID),
                    new SqlParameter("@Vital", VitalSign)
                };

            var ReturnDataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_PatientMyVitals_Details_SP]");
            return ReturnDataTable;
        }

    }
}
