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
    public class BloodRequestCS
    {
        public int StationID { get; set; }
        public string IPID { get; set; }

        public string BloodOrderID { get; set; }
        public string OrderID { get; set; }
        public string DoctorID { get; set; }

        DBHelper DB = new DBHelper("Reception");

        public List<BloodRequest> ViewMain()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@StationID", StationID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_BLOOD_REQUEST_VIEW", sqlParam);
                DataTable dt = ds.Tables[0];

                List<BloodRequest> li = (
                    from DataRow s in dt.Rows
                    orderby Convert.ToUInt16(s["OrderNo"].ToString()) descending
                    select new BloodRequest
                    {
                        sOrderNo = s["sOrderNo"].ToString(),
                        OrderNo = s["OrderNo"].ToString(),
                        Prefix = s["prefix"].ToString(),
                        PIN = s["PIN"].ToString(),
                        BloodOrderID = s["ID"].ToString(),
                        IPID = s["IPID"].ToString(),
                        Status = s["STATUS"].ToString(),
                        RegistrationNo = s["RegistrationNo"].ToString(),
                        PatientName = s["PatientName"].ToString(),
                        //BedName = s["BedName"].ToString(),
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
        public BloodRequest ViewDetail()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@OrderID", OrderID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_BLOOD_REQUEST_DETAIL", sqlParam);
                DataTable dt = ds.Tables[0];                
                BloodRequest br = new BloodRequest();

                if (OrderID != "0")
                {
                    DataRow s = dt.Rows[0];
                    br.TypeofRequest = s["reqtype"].ToString();
                    br.TypeofTransfusion = s["transtype"].ToString();
                    br.Donor = s["replace"].ToString();
                    br.WBC = s["wbc"].ToString();
                    br.RBC = s["rbc"].ToString();
                    br.HB = s["hb"].ToString();
                    br.PCV = s["pcv"].ToString();
                    br.Platelet = s["platelet"].ToString();
                    br.Others = s["others"].ToString();
                    br.PT = s["pt"].ToString();
                    br.PTTK = s["pttk"].ToString();
                    br.Diagnosis = s["ICDDescription"].ToString();
                }

                DataTable dt2 = ds.Tables[2];
                List<BloodDetail> bloodlist = (
                   from DataRow ss in dt2.Rows
                   select new BloodDetail
                   {                       
                       Name = ss["name"].ToString(),
                       Code = ss["Code"].ToString(),
                       ComponentID = ss["id"].ToString()

                   }).ToList();
                br.BloodList = bloodlist;
                return br;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<BloodDetail> ViewSelected()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@OrderID", OrderID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_BLOOD_REQUEST_DETAIL", sqlParam);
                DataTable dt = ds.Tables[0];               
                DataTable dt2 = ds.Tables[1];
                List<BloodDetail> bloodlist = (
                   from DataRow ss in dt2.Rows
                   select new BloodDetail
                   {
                       Name = ss["name"].ToString(),
                       Code = ss["Code"].ToString(),
                       ComponentID = ss["componentid"].ToString(),
                       Quantity = ss["quantity"].ToString(),
                       RequiredDate= ss["RDATETIME"].ToString(),
                       Remarks = ss["remarks"].ToString()

                   }).ToList();

                return bloodlist;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public string SaveOrder(BloodRequest model, List<BloodDetail> blood, string OperatorId)
        {
            try
            {
                DataTable dtRet = new DataTable();
                dtRet.Columns.AddRange(new[] {
                    new DataColumn("componentid", typeof(string)),                    
                    new DataColumn("Quantity", typeof(string)),
                    new DataColumn("Remarks", typeof(string)),
                    new DataColumn("RDATETIME", typeof(string))
                });

                foreach (var item in blood)
                {
                    DataRow newRow = dtRet.NewRow();
                    newRow["componentid"] = item.ComponentID;
                    newRow["Quantity"] = item.Quantity;
                    newRow["Remarks"] = item.Remarks;
                    newRow["RDATETIME"] = item.RequiredDate;
                    dtRet.Rows.Add(newRow);
                }

                System.IO.StringWriter sw = new System.IO.StringWriter();
                dtRet.TableName = "Data";
                dtRet.WriteXml(sw);

                SqlParameter[] sqlParam = new SqlParameter[19];
                sqlParam[0] = new SqlParameter("@OPERATORID", OperatorId);
                sqlParam[1] = new SqlParameter("@ipid", model.IPID);
                sqlParam[2] = new SqlParameter("@doctorid", model.Docid);

                sqlParam[3] = new SqlParameter("@transtype", model.TypeofTransfusion);
                sqlParam[4] = new SqlParameter("@reqtype", model.TypeofRequest);
                sqlParam[5] = new SqlParameter("@wbc", model.WBC);
                sqlParam[6] = new SqlParameter("@rbc", model.RBC);
                sqlParam[7] = new SqlParameter("@hb", model.HB);
                sqlParam[8] = new SqlParameter("@pcv", model.PCV);
                sqlParam[9] = new SqlParameter("@platelet", model.Platelet);
                sqlParam[10] = new SqlParameter("@others", model.Others);
                sqlParam[11] = new SqlParameter("@earlierdetct", model.EarlierDefect);
                sqlParam[12] = new SqlParameter("@demand", "0");
                sqlParam[13] = new SqlParameter("@ireplace", model.Donor);
                sqlParam[14] = new SqlParameter("@Clinicaldetails", model.Diagnosis);
                sqlParam[15] = new SqlParameter("@pt", model.PT);
                sqlParam[16] = new SqlParameter("@pttk", model.PTTK);

                sqlParam[17] = new SqlParameter("@XML", sw.ToString());
                sqlParam[18] = new SqlParameter("@OrderID", model.BloodOrderID);

                dl.ExecuteSQLDS("WARDS.WARDS_BLOOD_REQUEST_SAVE", sqlParam);
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
