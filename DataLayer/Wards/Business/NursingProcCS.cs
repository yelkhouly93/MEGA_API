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
    public class NursingProcCS
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
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@StationID", StationID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_NURSE_PROC_VIEW", sqlParam);
                DataTable dt = ds.Tables[0];

                List<Patient> li = (
                    from DataRow s in dt.Rows
                    orderby s["OrderNo"].ToString() ascending
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
                        OperatorName = s["Nurse"].ToString()
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
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@OrderID", OrderID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_NURSE_PROC_SELECTED", sqlParam);
                DataTable dt = ds.Tables[0];
                int i = 1;
                List<ItemCode> li = (
                    from DataRow s in dt.Rows
                    orderby s["Code"].ToString() ascending
                    select new ItemCode
                    {
                        Row = i++,
                        ID = s["ServiceID"].ToString(),
                        Code = s["Code"].ToString(),
                        Description = s["name"].ToString(),
                        Quantity = s["quantity"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public string SaveOrder(List<ItemCode> model, List<ItemCode> can, string OperatorId)
        {
            try
            {
                DataTable dtRet = new DataTable();
                dtRet.Columns.AddRange(new[] {
                    new DataColumn("ServiceID", typeof(int)),                    
                    new DataColumn("Quantity", typeof(int))
                });
                foreach (var item in model)
                {
                    DataRow newRow = dtRet.NewRow();
                    newRow["ServiceID"] = item.ID;
                    newRow["Quantity"] = item.Quantity;
                    dtRet.Rows.Add(newRow);
                }
                System.IO.StringWriter sw = new System.IO.StringWriter();
                dtRet.TableName = "Data";
                dtRet.WriteXml(sw);

                System.IO.StringWriter swCan = new System.IO.StringWriter();
                string sCan = null;
                if (can != null)
                {
                    DataTable dtCan = new DataTable();
                    dtCan.Columns.AddRange(new[] {
                    new DataColumn("ServiceID", typeof(int)),
                    new DataColumn("Quantity", typeof(int))
                });
                    foreach (var item in can)
                    {
                        DataRow newRow = dtCan.NewRow();
                        newRow["ServiceID"] = item.ID;
                        newRow["Quantity"] = item.Quantity;
                        dtCan.Rows.Add(newRow);
                    }
                    dtCan.TableName = "Data";
                    dtCan.WriteXml(swCan);
                    sCan = swCan.ToString();
                }

                SqlParameter[] sqlParam = new SqlParameter[6];
                sqlParam[0] = new SqlParameter("@OrderID", OrderID);
                sqlParam[1] = new SqlParameter("@ipid", IPID);
                sqlParam[2] = new SqlParameter("@DoctorID", DoctorID);
                sqlParam[3] = new SqlParameter("@OPERATORID", OperatorId);
                sqlParam[4] = new SqlParameter("@XML", sw.ToString());
                sqlParam[5] = new SqlParameter("@XMLCAN", sCan);
                dl.ExecuteSQLDS("WARDS.WARDS_NURSE_PROC_SAVE", sqlParam);
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
                sqlParam[0] = new SqlParameter("@OrderID", OrderID);
                sqlParam[1] = new SqlParameter("@CanceledBy", OperatorId);

                dl.ExecuteSQLDS("WARDS.WARDS_NURSE_PROC_CANCEL", sqlParam);
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
