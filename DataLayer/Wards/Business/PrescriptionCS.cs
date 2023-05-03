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
    public class PrescriptionCS
    {
        public int StationID { get; set; }
        public string IPID { get; set; }

        public string OrderID { get; set; }
        public string DoctorID { get; set; }
        public string PresID { get; set; }
        DBHelper DB = new DBHelper("Reception");

        public List<Patient> ViewMain()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@StationID", StationID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_PRESCRIPTION_VIEW", sqlParam);
                DataTable dt = ds.Tables[0];

                List<Patient> li = (
                    from DataRow s in dt.Rows
                    orderby Convert.ToUInt16(s["OrderStatus"].ToString()) ascending, s["sOrderNo"].ToString() descending
                    select new Patient
                    {
                        sOrderNo = s["sOrderNo"].ToString(),
                        OrderNo = s["OrderNo"].ToString(),
                        PIN = s["PIN"].ToString(),
                        IPID = s["IPID"].ToString(),
                        RegistrationNo = s["RegistrationNo"].ToString(),
                        PatientName = s["PatientName"].ToString(),
                        BedName = s["BedName"].ToString(),
                        DateTime = s["DateTime"].ToString(),
                        StationName = s["StationName"].ToString(),
                        DoctorID = s["DoctorID"].ToString(),
                        DoctorName = s["Doctor"].ToString(),
                        OperatorName = s["Nurse"].ToString(),

                        PrescriptionOrderStatus = s["OrderStatus"].ToString(),
                        PrescriptionOrderType = s["Ordertype"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<ItemCode> ViewSelected()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[2];
                sqlParam[0] = new SqlParameter("@Presid", PresID);
                sqlParam[1] = new SqlParameter("@IPID", IPID);

                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_PRESCRIPTION_SELECTED", sqlParam);
                DataTable dt = ds.Tables[0];
                int i = 1;
                List<ItemCode> li = (
                    from DataRow s in dt.Rows
                    orderby s["name"].ToString() ascending
                    select new ItemCode
                    {
                        Row = i++   ,
                        ID = s["itemid"].ToString(),
                        Code = s["itemcode"].ToString(),
                        Description = s["name"].ToString(),

                        Dose = s["Dose"].ToString(),
                        DoseName = s["DoseName"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<ItemCode> ViewSelectedPrescribe()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[2];
                sqlParam[0] = new SqlParameter("@Presid", PresID);
                sqlParam[1] = new SqlParameter("@IPID", IPID);

                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_PRESCRIPTION_PRESCR", sqlParam);
                DataTable dt = ds.Tables[0];
                int i = 1;
                List<ItemCode> li = (
                    from DataRow s in dt.Rows
                    orderby s["name"].ToString() ascending
                    select new ItemCode
                    {
                        Row = i++,
                        ID = s["itemid"].ToString(),
                        Description = s["name"].ToString(),
                        Dose = s["Strength"].ToString(),
                        DoseName = s["strength_unit"].ToString(),

                        Duration = s["DurationNumber"].ToString(),
                        DurationID = s["DurationID"].ToString(),
                        DurationName = s["DurationName"].ToString(),

                        Remarks = s["Remarks"].ToString(),
                        FrequencyID = s["FrequencyID"].ToString(),

                        //Administer = s["RouteofAdminName"].ToString(),
                        RouteofAdminID = s["RouteofAdminID"].ToString(),

                        StartDate = s["StartDateTime"].ToString(),
                        EndDate = s["EndDateTime"].ToString(),

                        Discontinue = s["Discontinued"].ToString(),
                        DiscontinueDate = s["discontinueddatetime"].ToString()

                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public string SaveOrder(List<ItemCode> model, string OperatorId,string ordertype)
        {
            try
            {
                DataTable dtRet = new DataTable();
                dtRet.Columns.AddRange(new[] {
                    new DataColumn("ItemID", typeof(string)),                    
                    new DataColumn("Dose", typeof(string)),
                    new DataColumn("DoseName", typeof(string)),
                    new DataColumn("Remarks", typeof(string)),
                    new DataColumn("RouteofAdminID", typeof(string)),
                    new DataColumn("AdministrationTiming", typeof(string)),
                    //new DataColumn("SqNo", typeof(string)),
                    new DataColumn("DurationID", typeof(string)),
                    new DataColumn("DurationNumber", typeof(string)),
                    new DataColumn("FrequencyID", typeof(string)),
                    //new DataColumn("PerIntakeQty", typeof(string)),
                    new DataColumn("StartDateTime", typeof(string)),
                    new DataColumn("EndDateTime", typeof(string)),
                    //new DataColumn("PerDayQty", typeof(string)),
                    //new DataColumn("TotalQty", typeof(string)),
                    //new DataColumn("PerDoseQty", typeof(string)),
                    //new DataColumn("strength", typeof(string)),
                    //new DataColumn("Period", typeof(string)),
                    //new DataColumn("Updatedperiod", typeof(string)),
                    new DataColumn("Discontinueddatetime", typeof(string))
                });
                foreach (var item in model)
                {
                    DataRow newRow = dtRet.NewRow();
                    newRow["ItemID"] = item.ID;
                    newRow["Dose"] = item.Dose;
                    newRow["DoseName"] = item.DoseName;
                    newRow["Remarks"] = item.Remarks;
                    newRow["RouteofAdminID"] = item.RouteofAdminID;
                    //newRow["AdministrationTiming"] = item.AdministerID;
                    //newRow["SqNo"] = item.Row;
                    newRow["DurationID"] = item.DurationID;
                    newRow["DurationNumber"] = item.Duration;
                    newRow["FrequencyID"] = item.FrequencyID;
                    //newRow["PerIntakeQty"] = 0;
                    newRow["StartDateTime"] = item.StartDate;
                    newRow["EndDateTime"] = item.EndDate;
                    //newRow["PerDayQty"] = item.Quantity;
                    //newRow["TotalQty"] = item.Quantity;
                    //newRow["PerDoseQty"] = item.Quantity;
                    //newRow["strength"] = item.Quantity;
                    //newRow["Period"] = item.Quantity;
                    //newRow["Updatedperiod"] = item.Quantity;
                    newRow["Discontinueddatetime"] = item.DiscontinueDate;
                    newRow["Discontinueddatetime"] = item.DiscontinueDate;
                    dtRet.Rows.Add(newRow);
                }

                System.IO.StringWriter sw = new System.IO.StringWriter();
                dtRet.TableName = "Data";
                dtRet.WriteXml(sw);

                SqlParameter[] sqlParam = new SqlParameter[6];
                sqlParam[0] = new SqlParameter("@OrderID", OrderID);
                sqlParam[1] = new SqlParameter("@ipid", IPID);
                sqlParam[2] = new SqlParameter("@DoctorID", DoctorID);
                sqlParam[3] = new SqlParameter("@OPERATORID", OperatorId);
                sqlParam[4] = new SqlParameter("@OrderType", ordertype);
                sqlParam[5] = new SqlParameter("@XML", sw.ToString());
                dl.ExecuteSQLDS("WARDS.WARDS_PRESCRIPTION_SAVE", sqlParam);
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
                SqlParameter[] sqlParam = new SqlParameter[3];
                sqlParam[0] = new SqlParameter("@OrderID", OrderID);
                sqlParam[1] = new SqlParameter("@CanceledBy", OperatorId);
                sqlParam[2] = new SqlParameter("@station", StationID);

                dl.ExecuteSQLDS("WARDS.WARDS_PRESCRIPTION_CANCEL", sqlParam);
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
