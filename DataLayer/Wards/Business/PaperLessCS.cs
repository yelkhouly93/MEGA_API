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
    public class PaperLessQueueModel
    {
        public string ID { get; set; }
        public string ModuleID { get; set; }
        public string PIN { get; set; }
        public string IPID { get; set; }
        public string AdmitDateTime { get; set; }
        public string VisitDate { get; set; }
        public string PatientName { get; set; }
        public string DoctorID { get; set; }
        public string DocName { get; set; }
        public string EmpCode { get; set; }
        public string BedName { get; set; }
        public string StationName { get; set; }
        public string Remarks { get; set; }
    }

    public class PaperLessReferralModel
    {
        public string DoctorID { get; set; }
        public string RefDocID { get; set; }
        public string Reason { get; set; }
        public string ConsultNote { get; set; }
        public string TransferType { get; set; }
        public string EmpID { get; set; }
        public string EmpCode { get; set; }
        public string DoctorName { get; set; }
        public string RefEmpID { get; set; }
        public string RefEmpCode { get; set; }
        public string RefName { get; set; }
        public string TransDatetime { get; set; }        
    }

    public class PaperLessInvModel
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string DepartmentID { get; set; }
        public string DeptName { get; set; }
    }
    public class PaperLessCS
    {

        DBHelper DB = new DBHelper("WARDS");
        public string ID { get; set; }
        public string UserID { get; set; }
        public string IPID { get; set; }
        public string StationID { get; set; }
        DBHelper DB = new DBHelper("Reception");

        public List<PaperLessQueueModel> PaperLessQueueCS()
        {
            try
            {
                DB.param = new SqlParameter[]{
                     new SqlParameter("ID", ID),
                     new SqlParameter("UserID", UserID),
                     new SqlParameter("IPID", IPID),
                     new SqlParameter("StationID", StationID)
                };
                return DB.ExecuteSPAndReturnDataTable("WARDS.PAPER_VIEW_DATE_WARDS").DataTableToList<PaperLessQueueModel>();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
            }
        }

        public PaperLessReferralModel ConsultLoad()
        {
            try
            {
                DB.param = new SqlParameter[]{
                     new SqlParameter("ID", ID)
                };
                return DB.ExecuteSPAndReturnDataTable("WARDS.PAPER_CONSULT_VIEW").DataTableToModel<PaperLessReferralModel>();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
            }
        }
        public List<LaboratoryTest> InvestigationLoad()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@ID", ID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.PAPER_INVESTIGATION_VIEW_WARDS", sqlParam);
                DataTable dt = ds.Tables[0];

                List<LaboratoryTest> li = (
                    from DataRow s in dt.Rows
                    orderby s["Code"].ToString() ascending
                    select new LaboratoryTest
                    {
                        Code = s["Code"].ToString(),
                        ID = s["ID"].ToString(),
                        Description = s["Description"].ToString(),
                        DestID = s["DestID"].ToString(),
                        SampleID = s["SampleID"].ToString(),
                        ProfileID = s["Profile"].ToString(),
                        Remarks = s["DepartmentID"].ToString()
                    }).ToList();
                return li;

            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
            }
        }

    }
}

