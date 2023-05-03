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
    public class PatientTransCS
    {
        public int StationID { get; set; }
        public string IPID { get; set; }

        public string OrderID { get; set; }
        public string DoctorID { get; set; }

        DBHelper DB = new DBHelper("Reception");

        public List<PatientRefModel> ViewMain()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@StationID", StationID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_PATIENTTRANS_VIEW", sqlParam);
                DataTable dt = ds.Tables[0];

                List<PatientRefModel> li = (
                    from DataRow s in dt.Rows                    
                    select new PatientRefModel
                    {
                        sOrderNo = s["sOrderNo"].ToString(),
                        OrderNo = s["OrderNo"].ToString(),
                        PIN = s["PIN"].ToString(),
                        PatientName = s["PatientName"].ToString(),
                        DateTime = s["DateTime"].ToString(),
                        RefDoctor = s["OPERATOR"].ToString(),
                        Dispatch = s["trans"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<ExtraFood> ViewSelectedFood()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@OrderID", OrderID);
                DataSet ds = dl.ExecuteSQLDS("WARDS_EXTRA_FOOD_DETAIL", sqlParam);
                DataTable dt = ds.Tables[0];
                int i = 1;
                List<ExtraFood> li = (
                    from DataRow s in dt.Rows
                    orderby s["Name"].ToString() ascending
                    select new ExtraFood
                    {
                        Row = i++,
                        Name = s["Name"].ToString(),
                        ID = s["Id"].ToString(),
                        Quantity = Convert.ToInt16(s["Quantity"].ToString()),
                        Units = "NILS"
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public string SaveOrder(string OperatorId, string newBed, string remarks)
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[5];
                sqlParam[0] = new SqlParameter("@OPERATORID", OperatorId);
                sqlParam[1] = new SqlParameter("@ipid", IPID);
                sqlParam[2] = new SqlParameter("@OrderID", OrderID);

                sqlParam[3] = new SqlParameter("@newbedid", newBed);
                sqlParam[4] = new SqlParameter("@remarks", remarks);

                dl.ExecuteSQLDS("WARDS.WARDS_PATIENTTRANS_SAVE", sqlParam);
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
    }
}
