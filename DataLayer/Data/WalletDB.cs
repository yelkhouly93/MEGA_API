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
	public class WalletDB
	{
		CustomDBHelper DB = new CustomDBHelper("RECEPTION");

        public DataTable GetWalletBalance(string Lang, int PatientMRN, int BranchID)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@PatientMRN", PatientMRN),
                new SqlParameter("@BranchID", BranchID)
            };
            var dataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_Wallet_Balance_SP]");
            return dataTable;
        }

        public DataTable GetWalletTransactionDetails(string Lang, int PatientMRN, int BranchID)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@PatientMRN", PatientMRN),
                new SqlParameter("@BranchID", BranchID)
            };
            var dataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_Wallet_TransactionDetails_SP]");
            return dataTable;
        }

        public DataTable GetWalletBeneficaryList(string Lang, int PatientMRN, int BranchID)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@PatientMRN", PatientMRN),
                new SqlParameter("@BranchID", BranchID)
            };
            var dataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_Wallet_Beneficiary_List_SP]");
            return dataTable;
        }

        public DataTable AddWalletBeneficary(int PatientMRN, int BranchID, int B_MRN,
            int B_BranchID, string B_Name,string B_NameAR,string B_NID, string Source, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {

                new SqlParameter("@PatientMRN", PatientMRN),
                new SqlParameter("@BranchID", BranchID),
                new SqlParameter("@B_Name", B_Name),
                new SqlParameter("@B_Name_ar", B_NameAR),
                new SqlParameter("@B_MRN", B_MRN),
                new SqlParameter("@B_BranchID", B_BranchID),
                new SqlParameter("@B_NID", B_NID),
                new SqlParameter("@Sources", Source),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 500)
            };

            DB.param[8].Direction = ParameterDirection.Output;
            DB.param[9].Direction = ParameterDirection.Output;

            var dataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Save_Wallet_Beneficary_SP]");

            errStatus = Convert.ToInt32(DB.param[8].Value);
            errMessage = DB.param[9].Value.ToString();

            return dataTable;
        }


        public DataTable RemoveWalletBeneficary(string lang, int PatientMRN, int BranchID, int B_ID,
                        string Source, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@PatientMRN", PatientMRN),
                new SqlParameter("@BranchID", BranchID),
                new SqlParameter("@B_ID", B_ID),
                new SqlParameter("@Sources", Source),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 500)
            };

            DB.param[5].Direction = ParameterDirection.Output;
            DB.param[6].Direction = ParameterDirection.Output;

            var dataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Remove_Wallet_Beneficary_SP]");

            errStatus = Convert.ToInt32(DB.param[5].Value);
            errMessage = DB.param[6].Value.ToString();

            return dataTable;
        }

        public DataTable VerifyPatientData(int PatientMRN, int BranchID , string PatientNID)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@PatientNID", PatientNID),
                new SqlParameter("@PatientMRN", PatientMRN),
                new SqlParameter("@BranchID", BranchID)
            };
            var dataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Verify_Patient_Data_SP]");
            return dataTable;
        }

        public DataTable OnlinePaymentConfirmation(string Lang, int BranchId, int PatientMRN, string BillType, string OnlineTrasactionID, string PaidAmount, string PaymentMethod, int TrackID,string Sources ,  ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@BranchId", BranchId),
                new SqlParameter("@PatientMRN", PatientMRN),
                new SqlParameter("@BillType", BillType),                
                new SqlParameter("@OnlineTransactionId", OnlineTrasactionID),
                new SqlParameter("@PaidAmount", PaidAmount),
                new SqlParameter("@PaymentMethod", PaymentMethod),
                new SqlParameter("@Sources", Sources),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 500),
                new SqlParameter("@TrackId", TrackID)
            };
            DB.param[7].Direction = ParameterDirection.Output;
            DB.param[8].Direction = ParameterDirection.Output;

            var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.Save_WalletOnlinePayment_SP");

            errStatus = Convert.ToInt32(DB.param[7].Value);
            errMessage = DB.param[8].Value.ToString();

            return dataTable;
        }

        public DataTable WalletAmountTransfer(string Lang,int BranchId, int PatientMRN, string TransferAmount,int beneficiaryID, string Sources, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@BranchId", BranchId),
                new SqlParameter("@PatientMRN", PatientMRN),
                new SqlParameter("@TransferAmount", TransferAmount),
                new SqlParameter("@BeneficiaryID", beneficiaryID),
                new SqlParameter("@Sources", Sources),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 500)
            };
            DB.param[6].Direction = ParameterDirection.Output;
            DB.param[7].Direction = ParameterDirection.Output;

            var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.Wallet_AmountTransfer_SP");

            errStatus = Convert.ToInt32(DB.param[6].Value);
            errMessage = DB.param[7].Value.ToString();

            return dataTable;
        }

    }
}
