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
    public class MedicalEquipCS
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
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_MEDEQUIP_VIEW", sqlParam);
                DataTable dt = ds.Tables[0];

                List<Patient> li = (
                    from DataRow s in dt.Rows
                    orderby Convert.ToUInt16(s["OrderNo"].ToString()) ascending
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
                        DoctorID = s["DoctorID"].ToString(),
                        OperatorName = s["Nurse"].ToString(),
                        MedEquID = s["MedicalEquipmentID"].ToString(),
                        MedStart = s["StartDateTime"].ToString(),
                        MedEnd = s["EndDateTime"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public string SaveOrder(string OperatorId,string startdate,string enddate,string medid)
        {
            try
            {               

                SqlParameter[] sqlParam = new SqlParameter[7];
                sqlParam[0] = new SqlParameter("@OrderID", OrderID);
                sqlParam[1] = new SqlParameter("@ipid", IPID);
                sqlParam[2] = new SqlParameter("@DoctorID", DoctorID);
                sqlParam[3] = new SqlParameter("@OPERATORID", OperatorId);
                sqlParam[4] = new SqlParameter("@startdate", startdate);
                sqlParam[5] = new SqlParameter("@enddate", enddate);
                sqlParam[6] = new SqlParameter("@MedicalEquipmentID", medid);     
                dl.ExecuteSQLDS("WARDS.WARDS_MEDEQUIP_SAVE", sqlParam);                    
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

                dl.ExecuteSQLDS("WARDS.WARDS_MEDEQUIP_CANCEL", sqlParam);
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

