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
    public class DrugOrderCS
    {
        public int StationID { get; set; }
        public string IPID { get; set; }

        public string OrderID { get; set; }
        public string DoctorID { get; set; }

        DBHelper DB = new DBHelper("Reception");

        public List<Patient> ViewMain()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[2];
                sqlParam[0] = new SqlParameter("@StationID", StationID);
                sqlParam[1] = new SqlParameter("@IPID", IPID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_DRUG_ORDER_VIEW", sqlParam);
                DataTable dt = ds.Tables[0];

                List<Patient> li = (
                    from DataRow s in dt.Rows
                    orderby s["OrderNo"] descending
                    select new Patient                    {

                        sOrderNo = s["sOrderNo"].ToString(),
                        OrderNo = s["OrderNo"].ToString(),
                        Prefix = s["prefix"].ToString(),
                        PIN = s["PIN"].ToString(),
                        IPID = s["IPID"].ToString(),
                        Dispatch = s["Dispatched"].ToString(),
                        RegistrationNo = s["RegistrationNo"].ToString(),
                        PatientName = s["PatientName"].ToString(),
                        BedName = s["BedName"].ToString(),
                        DateTime = s["DateTime"].ToString(),
                        OperatorName = s["OperatorName"].ToString(),
                        StationName = s["StationName"].ToString(),
                        IssueAuthoritycode = s["issueauthoritycode"].ToString(),
                        DoctorID = s["DoctorID"].ToString(),
                        
                        DrugDispatch = s["Dispatched"].ToString(),
                        DrugAck = s["Acknowledged"].ToString(),
                        DrugTakeHome = s["Istakehome"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<ItemCode> ViewSelectedDrug()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@OrderID", OrderID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_DRUG_ORDER_SELECTED", sqlParam);
                DataTable dt = ds.Tables[0];
                int i = 1;
                List<ItemCode> li = (
                    from DataRow s in dt.Rows
                    orderby s["Name"].ToString() ascending
                    select new ItemCode
                    {
                        Row = i++,
                        ID = s["id"].ToString(),
                        Description = s["name"].ToString(),
                        UnitID = s["UnitID"].ToString(),
                        UnitName = s["UnitName"].ToString(),
                        Quantity = s["DispatchQuantity"].ToString(),
                        Remarks = s["Remarks"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public string SaveOrder(List<ItemCode> model, string OperatorId, string DoctorID, string TKHome)
        {
            try
            {
                DataTable dtRet = new DataTable();
                dtRet.Columns.AddRange(new[] {
                    new DataColumn("ServiceID", typeof(int)),                    
                    new DataColumn("Remarks", typeof(string)),
                    new DataColumn("Quantity", typeof(int)),
                    new DataColumn("UnitID", typeof(int))
                });
                foreach (var item in model)
                {
                    DataRow newRow = dtRet.NewRow();
                    newRow["ServiceID"] = item.ID;
                    newRow["Quantity"] = item.Quantity;
                    newRow["Remarks"] = item.Remarks ?? "";
                    newRow["UnitID"] = item.UnitID ?? "0";
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
                sqlParam[4] = new SqlParameter("@Istakehome", TKHome);
                sqlParam[5] = new SqlParameter("@XML", sw.ToString());
                //sqlParam[4] = new SqlParameter("@XMLCAN", sCan);

                dl.ExecuteSQLDS("WARDS.WARDS_DRUG_ORDER_SAVE", sqlParam);
                return "Record Successfully Saved!";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public string Received(string OperatorId)
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[5];
                sqlParam[0] = new SqlParameter("@OrderID", OrderID);
                sqlParam[1] = new SqlParameter("@Operatorid", OperatorId);
                dl.ExecuteSQLDS("WARDS.WARDS_DRUG_ORDER_RECEIVED", sqlParam);
                return "Record Successfully Received!";
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
                sqlParam[0] = new SqlParameter("@OrderID", OrderID);
                sqlParam[1] = new SqlParameter("@OPERATORID", OperatorId);

                dl.ExecuteSQLDS("WARDS.WARDS_DRUG_ORDER_CANCEL", sqlParam);
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
