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
    public class ViewReportCS
    {

        public string Regno { get; set; }
        public string Panic { get; set; }
        public string IPID { get; set; }
        public int StationID { get; set; }

        DBHelper DB = new DBHelper("Reception");
        public List<ResultsView> ViewResult()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[2];
                sqlParam[0] = new SqlParameter("@registrationno", Regno);
                sqlParam[1] = new SqlParameter("@panic", Panic);

                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_RESULTSVIEW_RESULT", sqlParam);
                DataTable dt = ds.Tables[0];
                int i = 1;
                List<ResultsView> li = (
                    from DataRow s in dt.Rows
                    select new ResultsView
                    {
                        OrderNo = s["OrderID"].ToString(),
                        sOrderNo = s["ReqNo"].ToString(),
                        Doctor = s["Doctor"].ToString(),
                        Section = s["Station"].ToString(),
                        TestID = s["TestID"].ToString(),
                        Code = s["Code"].ToString(),
                        TestName = s["Test"].ToString(),
                        OrderDateTime = s["OrderDateTime"].ToString(),

                        TestDoneBy = s["TestDoneBY"].ToString(),
                        VerifiyBy = s["verifiedby"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<ResultsView> OldViewResult()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@registrationno", Regno);

                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_RESULTSVIEW_OLDRESULT", sqlParam);
                DataTable dt = ds.Tables[0];
                int i = 1;
                List<ResultsView> li = (
                    from DataRow s in dt.Rows
                    select new ResultsView
                    {
                        Row = i++,
                        DateCompleted = s["DATETIME_COMPLETED"].ToString(),
                        Doctor = s["DOC_NAME"].ToString(),
                        Code = s["SERVICE_CODE"].ToString(),
                        TestName = s["SERVICE_DESC"].ToString(),
                        PType = s["PATIENT_TYPE"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<ResultsView> Endoscopy1()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@registrationno", Regno);

                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_RESULTSVIEW_ENDOSCOPY", sqlParam);
                DataTable dt = ds.Tables[0];
                int i = 1;
                List<ResultsView> li = (
                    from DataRow s in dt.Rows
                    select new ResultsView
                    {
                        OrderDateTime = s["REPORTDATETIME"].ToString(),
                        BillNo = s["BILLNO"].ToString(),
                        TestName = s["PROCEDURENAME"].ToString(),
                        PType = s["PATIENTTYPE"].ToString(),
                        TestDoneBy = s["TESTDONEBYID"].ToString()                        
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<ResultsView> Endoscopy2()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@registrationno", Regno);

                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_RESULTSVIEW_ENDOSCOPY_2", sqlParam);
                DataTable dt = ds.Tables[0];
                int i = 1;
                List<ResultsView> li = (
                    from DataRow s in dt.Rows
                    select new ResultsView
                    {
                        VisitDate = s["REPORTDATETIME"].ToString(),
                        Room = s["BILLNO"].ToString(),
                        TestName = s["PROCEDURENAME"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<ResultsView> Cath()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@registrationno", Regno);

                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_RESULTSVIEW_CATH", sqlParam);
                DataTable dt = ds.Tables[0];
                int i = 1;
                List<ResultsView> li = (
                    from DataRow s in dt.Rows
                    select new ResultsView
                    {
                        OrderDateTime = s["ORDERDATETIME"].ToString(),
                        TestName = s["ProcedureName"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }


        public List<ResultsView> PatientOrderCS()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@IPID", IPID);

                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_PATIENT_ORDER", sqlParam);
                DataTable dt = ds.Tables[0];
                int i = 1;
                List<ResultsView> li = (
                    from DataRow s in dt.Rows
                    orderby s["row"].ToString() ascending, s["rrow"].ToString() ascending, s["rrrow"].ToString() ascending
                    select new ResultsView
                    {
                        iRow = s["row"].ToString(),
                        iiiRow = s["rrow"].ToString(),
                        iiRow = s["rrrow"].ToString(),

                        PType = s["Title"].ToString(),
                        Section = s["DepartmentName"].ToString(),
                        sOrderNo = s["OrderID"].ToString(),
                        OrderDateTime = s["DateTime"].ToString(),
                        TestName =  s["Name"].ToString(),
                        Room = s["StationName"].ToString(),
                        Qty = s["DispatchQuantity"].ToString(),
                        Unit = s["Unit"].ToString(),
                        OrderStat = s["Status"].ToString(),
                        Operator = s["operator"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }

        public PatientFolder PatientFolderCS()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@Registrationno", Regno);

                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_PATIENTFOLDER_IP", sqlParam);
                DataTable dt = ds.Tables[0];
                
                List<PatientFolderList> IP = (
                    from DataRow s in dt.Rows
                    select new PatientFolderList
                    {
                        DateString = s["Title"].ToString(),
                        IPID = s["IPID"].ToString(),
                        VisitID = s["VisitID"].ToString()
                    }).ToList();

                //DataSet ds2 = dl.ExecuteSQLDS("WARDS.WARDS_PATIENTFOLDER_IP", sqlParam);
                //DataTable dt2 = ds.Tables[0];

                //List<PatientFolderList> IP = (
                //    from DataRow s in dt.Rows
                //    select new PatientFolderList
                //    {
                //        DateString = s["Title"].ToString(),
                //        IPID = s["IPID"].ToString(),
                //        VisitID = s["VisitID"].ToString()
                //    }).ToList();

                PatientFolder pat = new PatientFolder()
                {
                    IPList = IP
                };

                return pat;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }

    }
}
