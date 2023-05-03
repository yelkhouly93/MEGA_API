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
    public class ExtraFoodCS
    {
        public int StationID { get; set; }
        public string IPID { get; set; }

        public string OrderID { get; set; }
        public string DoctorID { get; set; }

        DBHelper DB = new DBHelper("Reception");

        public List<ExtraFoodModel> ViewMain()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@StationID", StationID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_EXTRA_FOOD_VIEW", sqlParam);
                DataTable dt = ds.Tables[0];

                List<ExtraFoodModel> li = (
                    from DataRow s in dt.Rows
                    orderby Convert.ToUInt16(s["dispatched"].ToString()) ascending
                    select new ExtraFoodModel
                    {
                        sOrderNo = s["sOrderNo"].ToString(),
                        OrderNo = s["OrderNo"].ToString(),
                        Prefix = s["prefix"].ToString(),
                        PIN = s["PIN"].ToString(),
                        IPID = s["IPID"].ToString(),
                        Dispatch = s["dispatched"].ToString(),
                        RegistrationNo = s["RegistrationNo"].ToString(),
                        PatientName = s["PatientName"].ToString(),
                        BedName = s["BedName"].ToString(),
                        //BedID = s["BedID"].ToString(),
                        DateTime = s["DateTime"].ToString(),
                        OperatorName = s["OperatorName"].ToString(),
                        StationName = s["StationName"].ToString(),
                        IssueAuthoritycode = s["issueauthoritycode"].ToString()
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
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_EXTRA_FOOD_DETAIL", sqlParam);
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
        public string SaveOrder(List<ExtraFood> model,List<ExtraFood> canfood, string OperatorId)
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
                string sCan=null;
                if (canfood != null)
                {
                    DataTable dtCan = new DataTable();
                    dtCan.Columns.AddRange(new[] {
                    new DataColumn("ServiceID", typeof(int))
                });
                    foreach (var item in canfood)
                    {
                        DataRow newRow = dtCan.NewRow();
                        newRow["ServiceID"] = item.ID;
                        dtCan.Rows.Add(newRow);
                    }
                    dtCan.TableName = "Data";
                    dtCan.WriteXml(swCan);
                    sCan = swCan.ToString();
                }

                
                SqlParameter[] sqlParam = new SqlParameter[5];
                sqlParam[0] = new SqlParameter("@OPERATORID", OperatorId);
                sqlParam[1] = new SqlParameter("@ipid", IPID);
                sqlParam[2] = new SqlParameter("@XML", sw.ToString());
                sqlParam[3] = new SqlParameter("@OrderID", OrderID);
                sqlParam[4] = new SqlParameter("@XMLCAN", sCan);

                dl.ExecuteSQLDS("WARDS.WARDS_EXTRA_FOOD_SAVE", sqlParam);
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

                dl.ExecuteSQLDS("WARDS.WARDS_EXTRA_FOOD_CANCEL", sqlParam);
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
