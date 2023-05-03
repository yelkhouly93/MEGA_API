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
    public class InvestigationCS
    {
        public int StationID { get; set; }
        public string IPID { get; set; }

        public string OrderID { get; set; }
        public string DoctorID { get; set; }
        public string SampleID { get; set; }
        
        DBHelper DB = new DBHelper("Reception");

        public List<Patient> ViewMain()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@StationID", StationID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_INVESTIGATION_VIEW", sqlParam);
                DataTable dt = ds.Tables[0];

                List<Patient> li = (
                    from DataRow s in dt.Rows
                    orderby Convert.ToUInt16(s["OrderNo"].ToString()) descending
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
                        OperatorName = s["Nurse"].ToString(),

                        //InvExecuted = s["Executed"].ToString(),
                        //InvStat = s["Stat"].ToString(),
                        //InvPriority = s["priority"].ToString(),
                        //InvBedStat = s["BedStat"].ToString(),
                        //InvPatientStat = s["PatientStatus"].ToString(),
                        //InvPhlem = s["Phlebotomy"].ToString(),
                        //InvTestDone = s["ToBeDoneAt"].ToString(),
                        //InvTestDate = s["ToBeDoneBY"].ToString()

                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<LaboratoryTest> ViewSelected(string br)
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[2];
                sqlParam[0] = new SqlParameter("@OrderID", OrderID);
                sqlParam[1] = new SqlParameter("@Break", br);

                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_INVESTIGATION_SELECTED", sqlParam);
                DataTable dt = ds.Tables[0];
                int i = 1;
                List<LaboratoryTest> li = (
                    from DataRow s in dt.Rows
                    orderby s["TestName"].ToString() ascending
                    select new LaboratoryTest
                    {
                        Row = i++,
                        ID = s["testid"].ToString(),
                        Code = s["Code"].ToString(),
                        Description = s["TestName"].ToString(),
                        Profile = s["TestName"].ToString(),
                        Section = s["StationName"].ToString(),
                        Sample = s["Sample"].ToString(),
                        CollectedBy = s["collectedby"].ToString(),
                        TestDoneBy = s["testdoneby"].ToString(),
                        VerifiyBy = s["verifiedby"].ToString(),
                        Remarks = s["Comments"].ToString(),

                        DestID = s["DestStID"].ToString(),
                        SampleID = s["SampleID"].ToString(),
                        ProfileID = s["ProfileID"].ToString()

                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }

        public string SaveOrder(List<LaboratoryTest> model, InvestigationDetail det, string OperatorId)
        {
            try
            {
                DataTable dtRet = new DataTable();
                dtRet.Columns.AddRange(new[] {
                    new DataColumn("ServiceID", typeof(int)),                    
                    new DataColumn("DestStID", typeof(int)),
                    new DataColumn("SampleID", typeof(int)),
                    new DataColumn("ProfileID", typeof(int))
                });
                foreach (var item in model)
                {
                    DataRow newRow = dtRet.NewRow();
                    newRow["ServiceID"] = item.ID;
                    newRow["DestStID"] = item.DestID;
                    newRow["SampleID"] = item.SampleID;
                    newRow["ProfileID"] = item.ProfileID;
                    dtRet.Rows.Add(newRow);
                }

                System.IO.StringWriter sw = new System.IO.StringWriter();
                dtRet.TableName = "Data";
                dtRet.WriteXml(sw);

                SqlParameter[] sqlParam = new SqlParameter[13];
                sqlParam[0] = new SqlParameter("@OrderID", OrderID);
                sqlParam[1] = new SqlParameter("@ipid", IPID);
                sqlParam[2] = new SqlParameter("@DoctorID", DoctorID);
                sqlParam[3] = new SqlParameter("@OPERATORID", OperatorId);

                sqlParam[4] = new SqlParameter("@Remarks", det.Remarks);
                sqlParam[5] = new SqlParameter("@DateTime", det.DateTime);
                sqlParam[6] = new SqlParameter("@ToBeDoneBY", det.ToBeDoneBY);
                sqlParam[7] = new SqlParameter("@Priority", det.Priority);
                sqlParam[8] = new SqlParameter("@ToBeDoneAt", det.ToBeDoneAt);

                sqlParam[9] = new SqlParameter("@ExStatus", det.ExStatus);
                sqlParam[10] = new SqlParameter("@Phlebotomy", det.Phlebotomy);
                sqlParam[11] = new SqlParameter("@PatientStatus", det.PatientStatus);

                sqlParam[12] = new SqlParameter("@XML", sw.ToString());
                dl.ExecuteSQLDS("WARDS.WARDS_INVESTIGATION_SAVE", sqlParam);
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
                sqlParam[1] = new SqlParameter("@OperatorId", OperatorId);

                dl.ExecuteSQLDS("WARDS.WARDS_INVESTIGATION_CANCEL", sqlParam);
                return "Record Successfully Deleted!";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public string UpdateOrder(string OperatorId)
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[4];
                sqlParam[0] = new SqlParameter("@OrderID", OrderID);
                sqlParam[1] = new SqlParameter("@SampleID", SampleID);
                sqlParam[2] = new SqlParameter("@CollectedBy", OperatorId);
                sqlParam[3] = new SqlParameter("@CollectedAt", StationID);

                dl.ExecuteSQLDS("WARDS.WARDS_INVESTIGATION_UPDATE", sqlParam);
                return "Record Successfully Updated!";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
    }
}
