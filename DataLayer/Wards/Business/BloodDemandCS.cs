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
    public class BloodDemandCS
    {
        public int StationID { get; set; }
        public string IPID { get; set; }

        public string BloodOrderID { get; set; }
        public string OrderID { get; set; }
        public string DoctorID { get; set; }
        public string bedid { get; set; }
        DBHelper DB = new DBHelper("Reception");

        public List<BloodDemand> ViewMain()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[2];
                sqlParam[0] = new SqlParameter("@StationID", StationID);
                sqlParam[1] = new SqlParameter("@IPID", IPID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_BLOOD_DEMAND_VIEW", sqlParam);
                DataTable dt = ds.Tables[0];

                List<BloodDemand> li = (
                    from DataRow s in dt.Rows
                    orderby s["DEMAND"].ToString() ascending
                    select new BloodDemand
                    {
                        sOrderNo = s["sOrderNo"].ToString(),
                        OrderNo = s["OrderNo"].ToString(),
                        Prefix = s["prefix"].ToString(),
                        PIN = s["PIN"].ToString(),
                        BloodOrderID = s["ID"].ToString(),
                        IPID = s["IPID"].ToString(),
                        Demand = s["DEMAND"].ToString(),
                        RegistrationNo = s["RegistrationNo"].ToString(),
                        PatientName = s["PatientName"].ToString(),
                        BedName = s["BedName"].ToString(),
                        BedID = s["BedID"].ToString(),
                        DateTime = s["DateTime"].ToString(),
                        OperatorName = s["OperatorName"].ToString(),
                        StationName = s["StationName"].ToString(),
                        IssueAuthoritycode = s["issueauthoritycode"].ToString(),
                        Docid = s["DoctorID"].ToString()
                        
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<BloodDetail> ViewBlood()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@orderid", BloodOrderID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_BLOOD_DEMAND_SELECTED", sqlParam);
                DataTable dt = ds.Tables[0];

                List<BloodDetail> li = (
                    from DataRow s in dt.Rows
                    select new BloodDetail
                    {
                        ComponentID = s["componentid"].ToString(),
                        Quantity = s["quantity"].ToString(),
                        Remarks = s["remarks"].ToString(),
                        Name = s["name"].ToString(),
                        Code = s["code"].ToString(),
                        DemandQuantity = s["quantity"].ToString(),
                        PrevQuantity = s["DEMANDQTY"].ToString(),
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public string SaveOrder(List<BloodDetail> blood, string OperatorId)
        {
            try
            {
                DataTable dtRet = new DataTable();
                dtRet.Columns.AddRange(new[] {
                    new DataColumn("componentid", typeof(string)),
                    new DataColumn("OQTY", typeof(string)),
                    new DataColumn("Quantity", typeof(string)),
                    new DataColumn("Remarks", typeof(string))
                });

                foreach (var item in blood)
                {
                    DataRow newRow = dtRet.NewRow();
                    newRow["componentid"] = item.ComponentID;
                    newRow["OQTY"] = item.Quantity;
                    newRow["Quantity"] = item.DemandQuantity;
                    newRow["Remarks"] = item.Remarks;
                    dtRet.Rows.Add(newRow);
                }

                System.IO.StringWriter sw = new System.IO.StringWriter();
                dtRet.TableName = "Data";
                dtRet.WriteXml(sw);

                SqlParameter[] sqlParam = new SqlParameter[4];
                sqlParam[0] = new SqlParameter("@OPERATORID", OperatorId);
                sqlParam[1] = new SqlParameter("@BLOODORDERID", BloodOrderID);
                sqlParam[2] = new SqlParameter("@ipid", IPID);
                sqlParam[3] = new SqlParameter("@XML", sw.ToString());

                dl.ExecuteSQLDS("WARDS.WARDS_BLOOD_DEMAND_SAVE", sqlParam);
                return "Record Successfully Saved!";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }

        /************Blood Return*****************/

        public List<BloodDemand> ViewMainRet()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[2];
                sqlParam[0] = new SqlParameter("@StationID", StationID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_BLOODRETURN_VIEW", sqlParam);
                DataTable dt = ds.Tables[0];

                List<BloodDemand> li = (
                    from DataRow s in dt.Rows
                    orderby s["orderno"].ToString() descending
                    select new BloodDemand
                    {
                        sOrderNo = s["sOrderNo"].ToString(),
                        PIN = s["PIN"].ToString(),
                        OrderNo = s["orderno"].ToString(),
                        PatientName = s["PatientName"].ToString(),
                        DateTime = s["odatetime"].ToString(),
                        OperatorName = s["operatorname"].ToString(),
                        Demand = s["ReturnStatus"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<BloodDetail> ViewBloodRet()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@IPID", IPID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_BLOODRETURN_ORDER", sqlParam);
                DataTable dt = ds.Tables[0];

                List<BloodDetail> li = (
                    from DataRow s in dt.Rows
                    select new BloodDetail
                    {
                        Code = s["BagNumber"].ToString(),
                        Quantity = s["Volume"].ToString(),
                        RequiredDate = s["EXPIRYDATE"].ToString(),
                        Name = s["bloodname"].ToString(),
                        ComponentID = s["ID"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public string SaveOrderRet(List<BloodDetail> blood, string OperatorId,string ORDERID)
        {
            try
            {
                DataTable dtRet = new DataTable();
                dtRet.Columns.AddRange(new[] {
                    new DataColumn("ID", typeof(string))
                });

                foreach (var item in blood)
                {
                    DataRow newRow = dtRet.NewRow();
                    newRow["ID"] = item.ComponentID;
                    dtRet.Rows.Add(newRow);
                }

                System.IO.StringWriter sw = new System.IO.StringWriter();
                dtRet.TableName = "Data";
                dtRet.WriteXml(sw);

                SqlParameter[] sqlParam = new SqlParameter[3];
                sqlParam[0] = new SqlParameter("@OPERATORID", OperatorId);
                sqlParam[1] = new SqlParameter("@ORDERID", ORDERID);
                sqlParam[2] = new SqlParameter("@XML", sw.ToString());

                dl.ExecuteSQLDS("WARDS.WARDS_BLOODRETURN_SAVE", sqlParam);
                return "Record Successfully Saved!";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }


    }
}
