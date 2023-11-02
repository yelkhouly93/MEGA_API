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

    }
}
