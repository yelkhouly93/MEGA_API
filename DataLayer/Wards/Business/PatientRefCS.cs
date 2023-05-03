using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
using DataLayer.Wards.Model;
namespace DataLayer.Wards.Business
{
    public class PatientRefCS
    {
        public int StationID { get; set; }
        public string IPID { get; set; }

        public string OrderID { get; set; }
        public string DoctorID { get; set; }
        public string PaperID { get; set; }
            
        DBHelper DB = new DBHelper("Reception");

        public List<PatientRefModel> ViewMain()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@StationID", StationID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_PATIENTREF_VIEW", sqlParam);
                DataTable dt = ds.Tables[0];

                List<PatientRefModel> li = (
                    from DataRow s in dt.Rows
                    orderby Convert.ToDateTime(s["datetime"].ToString()) descending
                    select new PatientRefModel
                    {
                        sOrderNo = s["sOrderNo"].ToString(),
                        OrderNo = s["OrderNo"].ToString(),
                        Prefix = s["prefix"].ToString(),
                        PIN = s["PIN"].ToString(),
                        RegistrationNo = s["RegistrationNo"].ToString(),
                        PatientName = s["PatientName"].ToString(),
                        DoctorID = s["DoctorID"].ToString(),
                        DoctorName = s["DocName"].ToString(),
                        RefDoctor = s["RefDocID"].ToString(),
                        RefDoctorName = s["RefDocNAme"].ToString(),
                        RefDate = s["datetime"].ToString(),
                        RefReason = s["reason"].ToString(),
                        IPID = s["IPID"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public string SaveOrder(string OperatorId, string DoctorID, string RefDocID, string Reason,string station)
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[8];
                sqlParam[0] = new SqlParameter("@OPERATORID", OperatorId);
                sqlParam[1] = new SqlParameter("@ipid", IPID);
                sqlParam[2] = new SqlParameter("@OrderID", OrderID);

                sqlParam[3] = new SqlParameter("@DoctorID", DoctorID);
                sqlParam[4] = new SqlParameter("@RefDocID", RefDocID);
                sqlParam[5] = new SqlParameter("@Reason", Reason);
                sqlParam[6] = new SqlParameter("@stationid", station);
                sqlParam[7] = new SqlParameter("@PaperID", PaperID);

                dl.ExecuteSQLDS("WARDS.WARDS_PATIENTREF_SAVE_P", sqlParam);
                return "Record Successfully Saved!";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public string CancelOrder(string OperatorId)
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[2];
                sqlParam[0] = new SqlParameter("@OPERATORID", OperatorId);
                sqlParam[1] = new SqlParameter("@OrderID", OrderID);

                dl.ExecuteSQLDS("WARDS.WARDS_PATIENTREF_CANCEL", sqlParam);
                return "Record Successfully Deleted!";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }

        public List<PatientRefModel> ViewMainTransfer()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@StationID", StationID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_PRIMARYTRANSFER_VIEW", sqlParam);
                DataTable dt = ds.Tables[0];

                List<PatientRefModel> li = (
                    from DataRow s in dt.Rows                    
                    select new PatientRefModel
                    {
                        sOrderNo = s["sOrderNo"].ToString(),
                        OrderNo = s["OrderNo"].ToString(),                        
                        PIN = s["PIN"].ToString(),
                        RegistrationNo = s["registrationno"].ToString(),
                        PatientName = s["PatientName"].ToString(),
                        DoctorID = s["OldConsultantID"].ToString(),
                        DoctorName = s["PrimaryCons"].ToString(),
                        RefDoctor = s["NewConsultantID"].ToString(),
                        RefDoctorName = s["ChangedTo"].ToString(),
                        Operator = s["OPERATOR"].ToString(),
                        Prefix = s["trans"].ToString(),
                        DateTime = s["DateTime"].ToString(),
                        RefReason = s["Reason"].ToString(),
                        IPID = s["IPID"].ToString(),
                        BedNo = s["BedNo"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public string SaveOrderTransfer(string id,string OldCOnsultantId, string NewCOnsultantId,  string Reason, string OperatorId)
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[7];
                sqlParam[0] = new SqlParameter("@id", id);
                sqlParam[1] = new SqlParameter("@ipid", IPID);
                sqlParam[2] = new SqlParameter("@OldCOnsultantId", OldCOnsultantId);
                sqlParam[3] = new SqlParameter("@NewCOnsultantId", NewCOnsultantId);
                sqlParam[4] = new SqlParameter("@intimatedby", OperatorId);
                sqlParam[5] = new SqlParameter("@Reason", Reason);
                sqlParam[6] = new SqlParameter("@stationid", StationID);

                dl.ExecuteSQLDS("WARDS.WARDS_PRIMARYTRANSFER_SAVE", sqlParam);
                return "Record Successfully Saved!";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public string CancelOrderTransfer(string id)
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];                
                sqlParam[0] = new SqlParameter("@id", id);
                dl.ExecuteSQLDS("WARDS.WARDS_PRIMARYTRANSFER_CANCEL", sqlParam);
                return "Record Successfully Deleted!";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }

        public List<PatientRefModel> ViewMainDischargeIntimate()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@StationID", StationID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_DISCHARGEINTIMATE_VIEW", sqlParam);
                DataTable dt = ds.Tables[0];

                List<PatientRefModel> li = (
                    from DataRow s in dt.Rows
                    select new PatientRefModel
                    {
                        sOrderNo = s["sOrderNo"].ToString(),
                        OrderNo = s["ID"].ToString(),
                        PIN = s["PIN"].ToString(),
                        RegistrationNo = s["registrationno"].ToString(),
                        PatientName = s["PatientName"].ToString(),
                        DoctorName = s["OPERATOR"].ToString(),
                        DateTime = s["DateTime"].ToString(),
                        IPID = s["IPID"].ToString(),
                        BedID = s["BedID"].ToString(),
                        BedNo = s["Bed"].ToString(),
                        Dispatch = s["trans"].ToString(),

                        Remarks = s["Remarks"].ToString(),
                        RefReason = s["DischargeReasonId"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public string SaveOrderDischargeIntimate(string DISCHARGEREASONID, string remarks, string OperatorId)
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[5];
                sqlParam[0] = new SqlParameter("@ipid", IPID);
                sqlParam[1] = new SqlParameter("@DISCHARGEREASONID", DISCHARGEREASONID);
                sqlParam[2] = new SqlParameter("@remarks", remarks);
                sqlParam[3] = new SqlParameter("@OPERATORID", OperatorId);
                sqlParam[4] = new SqlParameter("@stationid", StationID);
                dl.ExecuteSQLDS("WARDS.WARDS_DISCHARGEINTIMATE_SAVE", sqlParam);
                return "Record Successfully Saved!";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public string CancelOrderDischargeIntimate(string OperatorId)
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[2];
                sqlParam[0] = new SqlParameter("@ipid", IPID);
                sqlParam[1] = new SqlParameter("@OPERATORID", OperatorId);
                dl.ExecuteSQLDS("WARDS.WARDS_DISCHARGEINTIMATE_CANCEL", sqlParam);
                return "Record Successfully Deleted!";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
    }
}
