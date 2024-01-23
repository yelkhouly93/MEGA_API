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
    public class PaymentDB
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");

        public DataTable GetConsultationAmount(int BranchId, int AppointmentID, string BillType, int OperatorID, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchId", BranchId),
                new SqlParameter("@AppointmentID", AppointmentID),
                new SqlParameter("@BillType", BillType),
                new SqlParameter("@OperatorID", OperatorID),                
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 500)
            };
            DB.param[4].Direction = ParameterDirection.Output;
            DB.param[5].Direction = ParameterDirection.Output;

            var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.Get_ConsultationCashAmount_V2_SP");

            errStatus = Convert.ToInt32(DB.param[4].Value);
            errMessage = DB.param[5].Value.ToString();

            return dataTable;
        }

        //public DataTable PaymentConfirmation_GenerateBill(int BranchId , int AppointmentID, string BillType, int OperatorID, string OnlineTrasactionID , string PaidAmount, string PaymentMethod, ref int errStatus, ref string errMessage)
        //{
        //    DB.param = new SqlParameter[]
        //    {                
        //        new SqlParameter("@BranchId", BranchId),
        //        new SqlParameter("@AppointmentID", AppointmentID),
        //        new SqlParameter("@BillType", BillType),
        //        new SqlParameter("@OperatorID", OperatorID),
        //        new SqlParameter("@OnlineTransactionId", OnlineTrasactionID),
        //        new SqlParameter("@PaidAmount", PaidAmount),
        //        new SqlParameter("@PaymentMethod", PaymentMethod),
        //        new SqlParameter("@status", SqlDbType.Int),
        //        new SqlParameter("@msg", SqlDbType.VarChar, 100)
        //    };
        //    DB.param[7].Direction = ParameterDirection.Output;
        //    DB.param[8].Direction = ParameterDirection.Output;

        //    var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.Save_ConsultationCashBill_SP");

        //    errStatus = Convert.ToInt32(DB.param[7].Value);
        //    errMessage = DB.param[8].Value.ToString();

        //    return dataTable;
        //}

        public SaveBillReturn PaymentConfirmation_GenerateBill(int BranchId, int AppointmentID, string BillType, int OperatorID, string OnlineTrasactionID, string PaidAmount, string PaymentMethod,int TrackID, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchId", BranchId),
                new SqlParameter("@AppointmentID", AppointmentID),
                new SqlParameter("@BillType", BillType),
                new SqlParameter("@OperatorID", OperatorID),
                new SqlParameter("@OnlineTransactionId", OnlineTrasactionID),
                new SqlParameter("@PaidAmount", PaidAmount),
                new SqlParameter("@PaymentMethod", PaymentMethod),                
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 500),
                new SqlParameter("@TrackId", TrackID)
            };
            DB.param[7].Direction = ParameterDirection.Output;
            DB.param[8].Direction = ParameterDirection.Output;

            var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.Save_ConsultationCashBill_SP").ToModelObject<SaveBillReturn>();

            errStatus = Convert.ToInt32(DB.param[7].Value);
            errMessage = DB.param[8].Value.ToString();

            return dataTable;
        }        
        public DataTable GetServicesAmount(int BranchId, int AppointmentID, string BillType, int OperatorID, string Service_ids,string item_Ids,string  department_ids, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchId", BranchId),
                new SqlParameter("@VisitId", AppointmentID),
                new SqlParameter("@BillType", BillType),
                new SqlParameter("@OperatorID", OperatorID),
                new SqlParameter("@ServiceIds", Service_ids),
                new SqlParameter("@DepartmentIds", department_ids),
                new SqlParameter("@ItemIds", item_Ids),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 500)
            };
            DB.param[7].Direction = ParameterDirection.Output;
            DB.param[8].Direction = ParameterDirection.Output;

            var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.Get_ServicesCashAmount_V2_SP");
            try
			{
                errStatus = Convert.ToInt32(DB.param[7].Value);
            }
            catch(Exception ex)
			{
                errStatus = 0;

            }
                

            errMessage = DB.param[8].Value.ToString();

            return dataTable;
        }

        public List<SaveBillReturn> PaymentServicesConfirmation_GenerateBill(int BranchId, int VisitID,int VisitTypeId, string BillType,string ServiceIds,string DepartmentIds,string ItemIds, int OperatorID, string OnlineTrasactionID, string PaidAmount, string PaymentMethod,int TrackID, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchId", BranchId),
                new SqlParameter("@VisitID", VisitID),
                new SqlParameter("@VisitTypeId", VisitTypeId),
                new SqlParameter("@BillType", BillType),
                new SqlParameter("@ServiceIds", ServiceIds),
                new SqlParameter("@DepartmentIds", DepartmentIds),
                new SqlParameter("@ItemIds", ItemIds),                
                new SqlParameter("@OperatorID", OperatorID),
                new SqlParameter("@OnlineTransactionId", OnlineTrasactionID),
                new SqlParameter("@PaidAmount", PaidAmount),
                new SqlParameter("@PaymentMethod", PaymentMethod),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 1000),
                new SqlParameter("@TrackId", TrackID)
            };
            DB.param[11].Direction = ParameterDirection.Output;
            DB.param[12].Direction = ParameterDirection.Output;

            var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.Save_ServicesCashBill_SP").ToListObject<SaveBillReturn>(); 

            errStatus = Convert.ToInt32(DB.param[11].Value);
            errMessage = DB.param[12].Value.ToString();

            return dataTable;
        }

        public List<EInvoiceParam> GetInvoiceInfo(int BranchId,string billNo, bool isCancellation)
        {

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BillNo", billNo),
                new SqlParameter("@IsCancellation", isCancellation),
                new SqlParameter("@BranchId", BranchId)
            };
            
            
            var _paymentBill  = DB.ExecuteSPAndReturnDataTable("[dbo].[ZATCA_Get_OPBill_Information]").ToListObject<EInvoiceParam>();
            
            return _paymentBill;
        }


        public DataTable GetPatientBillList(string lang , int BranchId, int MRN, string fromdate, string todate, string InvoiceType,string EpisodeType = "OP", int EpisodeId = 0)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@MRN", MRN),
                new SqlParameter("@BranchID", BranchId),
                new SqlParameter("@fromdate", fromdate),
                new SqlParameter("@todate", todate),
                new SqlParameter("@InvoiceType", InvoiceType),
                new SqlParameter("@EpisodeType", EpisodeType),
                new SqlParameter("@EpisodeId", EpisodeId)
            };
            var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.Get_Patient_BillList_SP");            
            return dataTable;
        }


        public DataTable PaymentTracking_Generate(string Sources,string PaymentFor, string BranchId, string BillType,string PatientMRN, string TotalAmount, string PaymentMethod,
            string appointmentID ,  string VisitID, string VisitTypeId,
             string ServiceIds, string DepartmentIds, string ItemIds,
            ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@PaymentFor", PaymentFor),
                new SqlParameter("@BranchId", BranchId),
                new SqlParameter("@BillType", BillType),
                new SqlParameter("@PatientMRN", PatientMRN),
                new SqlParameter("@Amount", TotalAmount),
                new SqlParameter("@PaymentMethod", PaymentMethod),
                new SqlParameter("@AppointmentID", appointmentID),
                new SqlParameter("@VisitID", VisitID),
                new SqlParameter("@VisitTypeId", VisitTypeId),                
                new SqlParameter("@ServiceIds", ServiceIds),
                new SqlParameter("@DepartmentIds", DepartmentIds),
                new SqlParameter("@ItemIds", ItemIds),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 1000),
                new SqlParameter("@Sources", Sources)                
            };
            DB.param[12].Direction = ParameterDirection.Output;
            DB.param[13].Direction = ParameterDirection.Output;

            //var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.Save_ServicesCashBill_SP");
            var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.Save_Generate_PaymentTracking_SP");

            errStatus = Convert.ToInt32(DB.param[12].Value);
            errMessage = DB.param[13].Value.ToString();

            return dataTable;
        }


    }
}
