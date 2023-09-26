using DataLayer.Common;
using DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DataLayer.Data
{
	public class AgrementDB
	{
		CustomDBHelper DB = new CustomDBHelper("RECEPTION");


        public DataTable GetAgreementContent(string lang, int BranchId, string AggrementName)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId", BranchId),
                new SqlParameter("@AgreementName", AggrementName)                
            };
            var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.Get_Agreement_Detail_SP");
            return dataTable;
        }

        public void SaveAgrrementAcceptance(int BranchID, string AgrrementName, int MRN, int ActionId,string Source, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchID", BranchID),
                new SqlParameter("@MRN", MRN),
                new SqlParameter("@AgreementName", AgrrementName ),
                new SqlParameter("@ActionID", ActionId),
                new SqlParameter("@Source", Source),                
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200)
            };

            DB.param[5].Direction = ParameterDirection.Output;
            DB.param[6].Direction = ParameterDirection.Output;

            DB.ExecuteNonQuerySP("dbo.SAVE_Agreement_Acceptance_SP");

            errStatus = Convert.ToInt32(DB.param[5].Value);
            errMessage = DB.param[6].Value.ToString();

            //return dataTable;
        }

    }
}
