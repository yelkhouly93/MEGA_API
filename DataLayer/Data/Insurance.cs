//using AgoraIO.Media;
//using AgoraIO.Media;
using DataLayer.Common;
using DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace DataLayer.Data
{
	public class InsuranceDB
	{
		CustomDBHelper DB = new CustomDBHelper("RECEPTION");
        public DataTable GetPatientInSuranceApprovalStatus_DT(string Lang, int BranchID, int RegistrationNo, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@BranchId", BranchID),
                new SqlParameter("@RegistrationNo", RegistrationNo),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200)
            };
            DB.param[3].Direction = ParameterDirection.Output;
            DB.param[4].Direction = ParameterDirection.Output;

            var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.Get_PatientUCAFStatus_SP");
            try
            {
                if (DB.param[3].Value != null)
                    errStatus = Convert.ToInt32(DB.param[3].Value);

                errMessage = DB.param[4].Value.ToString();
            }
            catch (Exception ex)
            {
                errStatus = 0;
            }


            return dataTable;
        }

        public DataTable GetPatientInsuranceInfo_DT(int hospitalId, int registrationNo, ref int erStatus, ref string msg)
        {
            try
            {
                DB.param = new SqlParameter[]
                {
                    new SqlParameter("@BranchId", hospitalId),
                    new SqlParameter("@RegistrationNo", registrationNo),
                    new SqlParameter("@status", SqlDbType.Int),
                    new SqlParameter("@msg", SqlDbType.NVarChar, 2000)
                };
                DB.param[2].Direction = ParameterDirection.Output;
                DB.param[3].Direction = ParameterDirection.Output;

                var dt = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_PatientInsuranceInfo_SP]");

                erStatus = Convert.ToInt32(DB.param[2].Value);
                msg = DB.param[3].Value.ToString();
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;

        }

    }
}
