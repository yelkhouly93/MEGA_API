using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
using DataLayer.Wards.Model;

namespace DataLayer.Wards
{
    public class Global
    {

        public int StationID { get; set; }
        public int UserID { get; set; }
        public string IPID { get; set; }
        public string PIN { get; set; }

        public string Employee { get; set; }
        public string EmployeeID { get; set; }
        public string DivisionID { get; set; }
        public string DepartmentID { get; set; }

        DBHelper DB = new DBHelper("Reception");
        
        public List<Patient> InpatientList()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[2];
                sqlParam[0] = new SqlParameter("@STATIONID", StationID);
                sqlParam[1] = new SqlParameter("@IPID", IPID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_PATIENT", sqlParam);
                DataTable dt = ds.Tables[0];

                List<Patient> li = (from DataRow s in dt.Rows
                                    orderby s["Bed"].ToString() ascending
                                    select new Patient
                                    {
                                        PIN = s["PIN"].ToString(),
                                        RegNo = s["RegistrationNo"].ToString(),
                                        PatientName = s["PatientName"].ToString(),
                                        IPID = s["IPID"].ToString(),

                                        BedNo = s["Bed"].ToString(),
                                        Age = s["Age"].ToString(),
                                        Sex = s["Sex"].ToString(),
                                        StationName = s["StationName"].ToString(),
                                        BloodGroup = s["BloodGroup"].ToString(),
                                        DoctorID = s["DoctorID"].ToString(),
                                        CompanyID = s["CompanyID"].ToString(),
                                        CompanyName = s["CompanyName"].ToString(),
                                        Drug = s["Allergy"].ToString(),
                                        Package = s["Package"].ToString(),
                                        Diagnosis = s["ICDDescription"].ToString(),
                                        DateTime = s["DateTime"].ToString()

                                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }

        public List<Patient> PatientLook(string id)
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@code", id);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_PATIENT_PARAM", sqlParam);
                DataTable dt = ds.Tables[0];

                List<Patient> li = (from DataRow s in dt.Rows
                                    orderby s["PatientName"].ToString() ascending
                                    select new Patient
                 {
                     PIN = s["PIN"].ToString(),
                     RegNo = s["RegistrationNo"].ToString(),
                     PatientName = s["PatientName"].ToString(), 
                     Age = s["Age"].ToString(),
                     Sex = s["Sex"].ToString()

                 }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<Patient> InpatientLook(string id)
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@code", id);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_PATIENT_IP", sqlParam);
                DataTable dt = ds.Tables[0];

                List<Patient> li = (from DataRow s in dt.Rows
                                    orderby s["BedNo"].ToString() ascending
                                    select new Patient
                                    {
                                        IPID = s["ipid"].ToString(),
                                        PIN = s["PIN"].ToString(),
                                        RegNo = s["RegistrationNo"].ToString(),
                                        PatientName = s["PatientName"].ToString(),
                                        DateTime = s["AdmitDatetime"].ToString(),
                                        DischargeDate = s["DischargeDatetime"].ToString(),
                                        CompanyName = s["CompanyName"].ToString(),
                                        BedName = s["BedNo"].ToString()

                                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public Patient PatientDetail()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[2];
                sqlParam[0] = new SqlParameter("@STATIONID", StationID);
                sqlParam[1] = new SqlParameter("@IPID", IPID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_PATIENT", sqlParam);
              
                Patient li = new Patient();

                if (ds.Tables[0].Rows.Count != 0)
                {
                    DataTable dt = ds.Tables[0];
                    DataRow s = dt.Rows[0];
                    li = new Patient()
                             {
                                 PIN = s["PIN"].ToString(),
                                 RegNo = s["RegistrationNo"].ToString(),
                                 PatientName = s["PatientName"].ToString(),
                                 BedNo = s["Bed"].ToString(),
                                 Age = s["Age"].ToString(),
                                 Sex = s["Sex"].ToString(),
                                 StationName = s["StationName"].ToString(),
                                 BloodGroup = s["BloodGroup"].ToString(),
                                 DoctorID = s["DoctorID"].ToString(),
                                 DoctorName = s["DocName"].ToString(),
                                 Drug = s["Allergy"].ToString(),
                                 Package = s["Package"].ToString(),
                                 CompanyID = s["CompanyID"].ToString(),
                                 CompanyName = s["CompanyName"].ToString(),
                                 Diagnosis = s["ICDDescription"].ToString(),
                                 DateTime = s["DateTime"].ToString(),
                                 AdmitDateTime = s["AdmitDateTime"].ToString()
                             };
                }
                return li ?? new Patient();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }

        public List<Doctor> DoctorList()
        {
            try
            {
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_DOCTOR", null);
                DataTable dt = ds.Tables[0];

                List<Doctor> li = (from DataRow s in dt.Rows
                                   orderby s["EmpCode"].ToString() ascending
                                   select new Doctor
                                             {
                                                 ID = s["ID"].ToString(),
                                                 EmpCode = s["EmpCode"].ToString(),
                                                 EmpName = s["EmpName"].ToString()

                                             }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public string GetStationName(string id)
        {
            try
            {

                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@id", id);
                
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_STATIONNAME", sqlParam);
                return ds.Tables[0].Rows[0]["Name"].ToString();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
       

        public List<ItemCode> DrugList()
        {
            try
            {
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_DRUG_ITEMS", null);
                DataTable dt = ds.Tables[0];

                List<ItemCode> li = (
                    from DataRow s in dt.Rows
                    orderby s["Id"].ToString() ascending
                    select new ItemCode
                    {
                        Code = s["Id"].ToString(),
                        ID = s["ItemID"].ToString(),
                        Description = s["Description"].ToString(),
                        Description2 = s["Description2"].ToString(),
                        UnitName = s["UnitName"].ToString(),

                        Dose = s["Dose"].ToString(),
                        DoseName = s["DoseName"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<LaboratoryTest> TestCodeList(string prof)
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@Profile", prof);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_TEST_ITEMS", sqlParam);
                DataTable dt = ds.Tables[0];

                List<LaboratoryTest> li = (
                    from DataRow s in dt.Rows
                    orderby s["Code"].ToString() ascending
                    select new LaboratoryTest
                    {
                        Code = s["Code"].ToString(),
                        ID = s["ID"].ToString(),
                        Description = s["Name"].ToString(),
                        DestID = s["STATIONID"].ToString(),
                        SampleID = s["SampleID"].ToString(),
                        ProfileID = s["Profile"].ToString(),
                        Remarks = s["DepartmentID"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<ItemCode> GenericListC()
        {
            try
            {
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_DRUG_GENERIC", null);
                DataTable dt = ds.Tables[0];

                List<ItemCode> li = (
                    from DataRow s in dt.Rows
                    orderby s["GenericCode"].ToString() ascending
                    select new ItemCode
                    {
                        Code = s["GenericCode"].ToString(),
                        ID = s["Id"].ToString(),
                        Description = s["Description"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public string DrugList(string id)
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@id", id);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_DRUG_STOCK", sqlParam);

                return ds.Tables[0].Rows[0]["qoh"].ToString();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<ExtraFood> ExtraFoodList()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_EXTRAFOOD", sqlParam);
                DataTable dt = ds.Tables[0];

                List<ExtraFood> li = (
                    from DataRow s in dt.Rows
                    orderby s["name"].ToString() ascending
                    select new ExtraFood
                    {
                        ID = s["id"].ToString(),
                        Name = s["name"].ToString(),
                        Code = s["Code"].ToString()

                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }

        public List<ItemCode> OtherProcedureList()
        {
            try
            {
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_OTHERPROC_ITEMS", null);
                DataTable dt = ds.Tables[0];

                List<ItemCode> li = (
                    from DataRow s in dt.Rows
                    orderby s["code"].ToString() ascending
                    select new ItemCode
                    {
                        Code = s["code"].ToString(),
                        ID = s["Id"].ToString(),
                        Description = s["name"].ToString(),
                        GenericID = s["DepartmentID"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<ItemCode> OtherProcedureSelect(string id)
        {
            try
            {

                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@id", id);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_OTHERPROC_PARAM", sqlParam);
                DataTable dt = ds.Tables[0];

                List<ItemCode> li = (
                    from DataRow s in dt.Rows
                    orderby s["code"].ToString() ascending
                    select new ItemCode
                    {
                        Code = s["code"].ToString(),
                        ID = s["Id"].ToString(),
                        Description = s["name"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<ItemCode> NursingProcedureList()
        {
            try
            {
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_NURSEPROC_ITEMS", null);
                DataTable dt = ds.Tables[0];

                List<ItemCode> li = (
                    from DataRow s in dt.Rows
                    orderby s["code"].ToString() ascending
                    select new ItemCode
                    {
                        Code = s["code"].ToString(),
                        ID = s["Id"].ToString(),
                        Description = s["name"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }


        public List<ItemList> FrequencyList()
        {
            try
            {
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_FREQUENCY", null);
                DataTable dt = ds.Tables[0];

                List<ItemList> li = (
                    from DataRow s in dt.Rows
                    select new ItemList
                    {
                        ID = s["Id"].ToString(),
                        Name = s["Description"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<ItemList> DurationList()
        {
            try
            {
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_DURATION", null);
                DataTable dt = ds.Tables[0];

                List<ItemList> li = (
                    from DataRow s in dt.Rows
                    select new ItemList
                    {
                        ID = s["Id"].ToString(),
                        Name = s["Description"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<ItemList> RouteofAdminList()
        {
            try
            {
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_ROUTEOFADMIN", null);
                DataTable dt = ds.Tables[0];

                List<ItemList> li = (
                    from DataRow s in dt.Rows
                    select new ItemList
                    {
                        ID = s["Id"].ToString(),
                        Name = s["Description"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        public List<ItemList> PatientStatusList()
        {
            try
            {
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_PATIENTSTATUS", null);
                DataTable dt = ds.Tables[0];

                List<ItemList> li = (
                    from DataRow s in dt.Rows
                    select new ItemList
                    {
                        ID = s["Id"].ToString(),
                        Name = s["name"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }

        public List<ItemList> UnitList(string id)
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@itemid", id);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_UNIT", sqlParam);
                DataTable dt = ds.Tables[0];

                List<ItemList> li = (
                    from DataRow s in dt.Rows
                    orderby s["Slno"].ToString() descending
                    select new ItemList
                    {
                        ID = s["ID"].ToString(),
                        Name = s["Name"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }

        public List<ItemList> MedicalEquipmentList()
        {
            try
            {
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_MEDEQUIP", null);
                DataTable dt = ds.Tables[0];

                List<ItemList> li = (
                    from DataRow s in dt.Rows
                    select new ItemList
                    {
                        ID = s["ItemId"].ToString(),
                        Name = s["EquipmentName"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }


        public List<ItemList> DischargeReasonList()
        {
            try
            {
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_DISCHARGE_REASON", null);
                DataTable dt = ds.Tables[0];

                List<ItemList> li = (
                    from DataRow s in dt.Rows
                    orderby s["ID"].ToString() ascending
                    select new ItemList
                    {
                        ID = s["ID"].ToString(),
                        Name = s["NAME"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
        //public List<OrderNo> DrugOrder(string id)
        //{
        //    try
        //    {
        //        SqlParameter[] sqlParam = new SqlParameter[1];
        //        sqlParam[0] = new SqlParameter("@TYPE", id);
        //        DataSet ds = dl.ExecuteSQLDS("WARDS_LOOKUP_DRUG_LIST", sqlParam);
        //        DataTable dt = ds.Tables[0];

        //        List<ItemCode> li = (
        //            from DataRow s in dt.Rows
        //            orderby s["Description"].ToString() ascending
        //            select new ItemCode
        //            {
        //                ID = s["Id"].ToString(),
        //                Name = s["Description"].ToString()
        //            }).ToList();
        //        return li;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
        //        //return false;
        //    }
        //}

        public List<BedList> ShowBedList()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@StationID", StationID);

                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_BEDLIST", sqlParam);
                DataTable dt = ds.Tables[0];
                int i = 1;
                List<BedList> li = (
                    from DataRow s in dt.Rows
                    orderby s["Name"].ToString() ascending
                    select new BedList
                    {
                        BedName = s["Name"].ToString(),
                        PatientName = s["PatientName"].ToString(),
                        IPID = s["IPID"].ToString(),
                        RegNo = s["RegistrationNo"].ToString(),
                        PIN = s["PIN"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }

        public List<ItemList> StationListCS()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[2];
                sqlParam[0] = new SqlParameter("@MODULEID", StationID);
                sqlParam[1] = new SqlParameter("@USERID", UserID);

                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_USER_STATION", sqlParam);
                DataTable dt = ds.Tables[0];
                int i = 1;
                List<ItemList> li = (
                    from DataRow s in dt.Rows
                    orderby s["Name"].ToString() ascending
                    select new ItemList
                    {
                        ID = s["id"].ToString(),
                        Name = s["Name"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }

        public List<ItemList> TransferStationListCS()
        {
            try
            {
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_TRANSFER_STATION", null);
                DataTable dt = ds.Tables[0];
                int i = 1;
                List<ItemList> li = (
                    from DataRow s in dt.Rows
                    orderby s["Name"].ToString() ascending
                    select new ItemList
                    {
                        ID = s["id"].ToString(),
                        Name = s["Name"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }


        public List<ItemList> DischargeReasonListCS()
        {
            try
            {
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_DISCHARGREASON", null);
                DataTable dt = ds.Tables[0];
                int i = 1;
                List<ItemList> li = (
                    from DataRow s in dt.Rows
                    orderby s["Name"].ToString() ascending
                    select new ItemList
                    {
                        ID = s["id"].ToString(),
                        Name = s["Name"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }

        public List<ItemList> TransferBedListCS(string id)
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@ID", id);

                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_LOOKUP_TRANSFER_BED", sqlParam);
                DataTable dt = ds.Tables[0];
                int i = 1;
                List<ItemList> li = (
                    from DataRow s in dt.Rows
                    orderby s["Name"].ToString() ascending
                    select new ItemList
                    {
                        ID = s["id"].ToString(),
                        Name = s["name"].ToString(),
                        Type = s["BedType"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }

        public List<ItemList> UserList(string id)
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@code", id);

                DataSet ds = dl.ExecuteSQLDS("WARDS.LIST_EMPLOYEE", sqlParam);
                DataTable dt = ds.Tables[0];
                int i = 1;
                List<ItemList> li = (
                    from DataRow s in dt.Rows
                    orderby s["Name"].ToString() ascending
                    select new ItemList
                    {
                        ID = s["EmployeeID"].ToString(),
                        Name = s["Name"].ToString()
                    }).ToList();
                return li;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }

        public Boolean IsUserValid(string Username, string Password)
        {
            try
            {
                EncryptDecrypt enc = new EncryptDecrypt();
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@Username", Username);
                DataSet ds = dl.ExecuteSQLDS("WARDS.EMPLOYEE_LOGIN", sqlParam);

                if (ds.Tables[0].Rows.Count != 0)
                {
                    if (string.Compare(Password ?? "", DecryptPassword(ds.Tables[0].Rows[0]["password"].ToString()), false) == 0)
                    {
                        this.Employee = ds.Tables[0].Rows[0]["name"].ToString();
                        this.EmployeeID = ds.Tables[0].Rows[0]["id"].ToString();
                        this.DivisionID = ds.Tables[0].Rows[0]["divisionid"].ToString();
                        this.DepartmentID = ds.Tables[0].Rows[0]["DepartmentID"].ToString();
                        return true; 
                    }
                    else
                    {
                        this.Employee = "";
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public string DecryptPassword(string StringToDecrypt)
        {
            double dblCountLength;
            int intLengthChar;
            string strCurrentChar;
            double dblCurrentChar;
            int intCountChar;
            int intRandomSeed;
            int intBeforeMulti;
            int intAfterMulti;
            int intSubNinetyNine;
            int intInverseAsc;
            string decrypt = "";

            for (dblCountLength = 0; dblCountLength < @StringToDecrypt.Length; dblCountLength++)
            {
                intLengthChar = int.Parse(@StringToDecrypt.Substring((int)dblCountLength, 1));
                strCurrentChar = @StringToDecrypt.Substring((int)(dblCountLength + 1), intLengthChar);
                dblCurrentChar = 0;
                for (intCountChar = 0; intCountChar < strCurrentChar.Length; intCountChar++)
                {
                    dblCurrentChar = dblCurrentChar + (Convert.ToInt32(char.Parse(strCurrentChar.Substring(intCountChar, 1))) - 33) * (Math.Pow(93, (strCurrentChar.Length - (intCountChar + 1))));
                }

                intRandomSeed = int.Parse(dblCurrentChar.ToString().Substring(2, 2));
                intBeforeMulti = int.Parse(dblCurrentChar.ToString().Substring(0, 2) + dblCurrentChar.ToString().Substring(4, 2));
                intAfterMulti = intBeforeMulti / intRandomSeed;
                intSubNinetyNine = intAfterMulti - 99;
                intInverseAsc = 256 - intSubNinetyNine;
                decrypt += Convert.ToChar(intInverseAsc);
                dblCountLength = dblCountLength + intLengthChar;
            }
            return decrypt;
        }

        public ItemList CheckStation(string id, string stationID)
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[2];
                sqlParam[0] = new SqlParameter("@EmpId", id);
                sqlParam[1] = new SqlParameter("@StationID", stationID);
                DataSet ds = dl.ExecuteSQLDS("WARDS.WARDS_USER_STATION_ASSIGN", sqlParam);
                ItemList model = new ItemList();
                if (ds.Tables[0].Rows.Count != 0)
                {
                    model.ID = ds.Tables[0].Rows[0]["StationID"].ToString();
                    model.Name = ds.Tables[0].Rows[0]["Name"].ToString();
                }
                else
                {
                    model.ID = "0";
                    model.Name = "";
                }
                return model;   
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }

        public string GetVersion()
        {
            try
            {
                DataSet ds = dl.ExecuteSQL_String("select * from WARDS_VERSION order by ID desc");
                return ds.Tables[0].Rows[0]["VersionName"].ToString();

            }
            catch (Exception ex)
            {
                return "";
            }
        }

    }
}


