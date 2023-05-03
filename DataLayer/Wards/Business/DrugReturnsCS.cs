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
    public class DrugReturnsCS
    {
        public int StationID { get; set; }
        public string IPID { get; set; }

        public string DrugOrderID { get; set; }
        public string OrderID { get; set; }
        public string DoctorID { get; set; }
        DBHelper DB = new DBHelper("Reception");
        public List<DrugReturnModel> ViewMain()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@StationID", StationID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_DRUG_RETURN_VIEW", sqlParam);
                DataTable dt = ds.Tables[0];

                List<DrugReturnModel> li = (
                    from DataRow s in dt.Rows
                    orderby s["OrderNo"].ToString() descending
                    select new DrugReturnModel
                    {
                        sOrderNo = s["sOrderNo"].ToString(),
                        OrderNo = s["OrderNo"].ToString(),
                        Prefix = s["prefix"].ToString(),
                        PIN = s["PIN"].ToString(),
                        DrugOrderID = s["DrugOrderId"].ToString(),
                        IPID = s["IPID"].ToString(),
                        Status = s["Status"].ToString(),
                        RegistrationNo = s["Registrationno"].ToString(),
                        PatientName = s["PatientName"].ToString(),
                        BedName = s["BedNo"].ToString(),
                        DateTime = s["DateTime"].ToString(),
                        OperatorName = s["Operator"].ToString(),
                        StationID = s["StationId"].ToString(),
                        stationslno = s["stationslno"].ToString(),
                        IssueAuthoritycode = s["IssueAuthoritycode"].ToString(),
                        DoctorID = s["DoctorID"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<DrugModel> ViewSelectedDrug()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[2];
                sqlParam[0] = new SqlParameter("@DrugOrderID", DrugOrderID);
                sqlParam[1] = new SqlParameter("@OrderID", OrderID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_DRUG_RETURN_SELECTED", sqlParam);
                DataTable dt = ds.Tables[0];
                int i = 1;
                List<DrugModel> li = (
                    from DataRow s in dt.Rows
                    orderby s["Name"].ToString() ascending
                    select new DrugModel
                    {
                        Row = i++,
                        OrderID = s["OrderID"].ToString(),
                        ServiceID = s["ID"].ToString(),
                        BatchID = s["Batchid"].ToString(),
                        DrugOrderID = s["DrugOrderId"].ToString(),
                        DrugName = s["Name"].ToString(),
                        UnitID = s["UnitID"].ToString(),
                        UnitName = s["Units"].ToString(),
                        Batchno = s["BatchnoDrug"].ToString(),
                        Remarks = s["Remarks"].ToString(),
                        Quantity = s["Quantity"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<DrugModel> ViewDrugOrder()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@DrugOrderID", DrugOrderID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_DRUG_RETURN_ORDER", sqlParam);
                DataTable dt = ds.Tables[0];

                List<DrugModel> li = (
                    from DataRow s in dt.Rows
                    orderby s["Name"].ToString() ascending
                    select new DrugModel
                    {
                        OrderID = s["OrderID"].ToString(),
                        ServiceID = s["ID"].ToString(),
                        BatchID = s["Batchid"].ToString(),
                        DrugOrderID = s["DrugOrderId"].ToString(),
                        DrugName = s["Name"].ToString(),
                        UnitID = s["UnitID"].ToString(),
                        UnitName = s["Units"].ToString(),
                        Batchno = s["BatchnoDrug"].ToString(),
                        Quantity = s["Quantity"].ToString()

                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<OrderNoList> OrderNoList()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@IPID", IPID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_DRUG_RETURN_ORDERNO", sqlParam);
                DataTable dt = ds.Tables[0];

                List<OrderNoList> li = (
                    from DataRow s in dt.Rows
                    orderby s["stationslno"].ToString() ascending
                    select new OrderNoList
                    {
                        Prefix = s["prefix"].ToString(),
                        OrderID = s["orderid"].ToString(),
                        StationSLNo = s["stationslno"].ToString(),
                        PrescriptionID = s["prescriptionid"].ToString()

                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public string CancelOrder()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@ORDERID", OrderID);
                dl.ExecuteSQLDS("WARDS.WARDS_DRUG_RETURN_CANCEL", sqlParam);
                return "Cancel Successfull!";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public string SaveOrder(List<DrugModel> drug, string OperatorId)
        {
            try
            {

                DataTable dtRet = new DataTable();
                dtRet.Columns.AddRange(new[] {
                    new DataColumn("ServiceID", typeof(string)),
                    new DataColumn("Quantity", typeof(string)),
                    new DataColumn("BatchID", typeof(string)),
                    new DataColumn("Remarks", typeof(string)),

                });

                foreach (var item in drug)
                {
                    DataRow newRow = dtRet.NewRow();
                    newRow["ServiceID"] = item.ServiceID;
                    newRow["Quantity"] = item.Quantity;
                    newRow["BatchID"] = item.BatchID;
                    newRow["Remarks"] = item.Remarks;
                    dtRet.Rows.Add(newRow);
                }

                System.IO.StringWriter sw = new System.IO.StringWriter();
                dtRet.TableName = "Data";
                dtRet.WriteXml(sw);

                SqlParameter[] sqlParam = new SqlParameter[6];
                sqlParam[0] = new SqlParameter("@ORDERID_PARAM", OrderID);
                sqlParam[1] = new SqlParameter("@IPID", IPID);
                sqlParam[2] = new SqlParameter("@OperatorID", OperatorId);
                sqlParam[3] = new SqlParameter("@DrugOrderId", DrugOrderID);
                sqlParam[4] = new SqlParameter("@DoctorID", DoctorID);
                sqlParam[5] = new SqlParameter("@XML", sw.ToString());

                dl.ExecuteSQLDS("WARDS.WARDS_DRUG_RETURN_SAVE", sqlParam);
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
    