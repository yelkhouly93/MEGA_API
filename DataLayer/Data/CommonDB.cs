using DataLayer.Common;
using DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.Net.Mail;

namespace DataLayer.Data
{
    public class CommonDB
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");

        public Client GetClientInfo(string clientId)
        {
            string sqlQuery = " SELECT  ClientSecretKey, UserName, Password , RM.RoleName as Role , c.id as userid " +
                              " FROM MobileApiClient C inner join ClientRole CR on CR.ClientID = C.Id Inner Join BAS_Role_TB RM on RM.ID = CR.RoleId " +
                              " WHERE ClientSecretKey = '" + clientId + "'";

            var client = DB.ExecuteSQLAndReturnDataTable(sqlQuery.ToString()).DataTableToModel<Client>();
            return client;

        }

        public List<Titles> GetAllTitles(string lang, int hospitalID)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId", hospitalID)
            };

            var _allTitles = new List<Titles>();

            _allTitles = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_Title_SP]").ToListObject<Titles>();

            return _allTitles;

        }

        public List<MaritalStatusCodes> GetAllMaritalStatusCodes(string lang, int hospitalID)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId", hospitalID)
            };

            var _allMaritalStatusCodes = new List<MaritalStatusCodes>();

            _allMaritalStatusCodes = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_MaritalSTatus_SP]").ToListObject<MaritalStatusCodes>();

            return _allMaritalStatusCodes;

        }

        public int GetClinicIdAgainstPhysician(int branchId, int physicianId)
        {

            string sqlQuery = "SELECT DEPARTMENTCODE FROM BAS_OPDDoctors_TB " + 
                         " WHERE Branch_Id =  " + branchId +
                         " AND HIS_Id = " + physicianId;

            int clinicId = Convert.ToInt32(DB.ExecuteSQLScalar(sqlQuery));
            return clinicId;
        }


        public string GetDcotorCodeAgainstPhysician(int branchId, int physicianId)
        {

            string sqlQuery = "SELECT EmployeeCode FROM BAS_OPDDoctors_TB " +
                         " WHERE Branch_Id =  " + branchId +
                         " AND HIS_Id = " + physicianId;

            string empCode = DB.ExecuteSQLScalar(sqlQuery);
            return empCode;
        }
        

        public int GetPateintIdAgainstMrn(int hospitalID, int registrationNo)
        {

            string sqlQuery = " SELECT TOP 1 PatientId " +
                              " FROM PatientVerificationCode_TB " +
                              " WHERE Branch_Id = " + hospitalID +
                              " AND RegistrationNo = " + registrationNo +
                              " AND GETDATE() <= Expiry_DateTime " +
                              " ORDER BY Expiry_DateTime DESC ";

            int patientId = Convert.ToInt32(DB.ExecuteSQLScalar(sqlQuery));
            return patientId;
           

        }


        public void SMSRequest(int BranchId, string SMSSource, string MobileNo, string SMS_Text, ref string errMessage, ref int errStatus)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@SMSSource", SMSSource),
                new SqlParameter("@Branch_Code", BranchId),
                new SqlParameter("@MobileNo", MobileNo),
                new SqlParameter("@SMS_Text", SMS_Text),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200),
                new SqlParameter("@Status", SqlDbType.Int)
            };
            DB.param[4].Direction = ParameterDirection.Output;
            DB.param[5].Direction = ParameterDirection.Output;

            var flag = DB.ExecuteSP("[SMS].[Insert_RealTimeRequestSMS_SP]");

            errStatus = Convert.ToInt32(DB.param[5].Value);
            errMessage = DB.param[4].Value.ToString();
        }



        public DataTable GetGeneralData_DT(int hospitaId, int SpNameId, string  StrParameters, ref int errStatus, ref string errMessage)

        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchId", hospitaId),
                new SqlParameter("@DSNameId", SpNameId),                
                new SqlParameter("@Params ", StrParameters),                
                new SqlParameter("@Er_Status", SqlDbType.Int),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 200)
            };

            DB.param[3].Direction = ParameterDirection.Output;
            DB.param[4].Direction = ParameterDirection.Output;

            var dt = DB.ExecuteSPAndReturnDataTable("[dbo].[GET_GENERALDATA_SP]");

            errStatus = Convert.ToInt32(DB.param[3].Value);
            errMessage = DB.param[4].Value.ToString();

            return dt;
        }


        public DataTable GetSpParameters(string spName)
        {
            DB.param = new SqlParameter[]
            {   
                new SqlParameter("@SPName", spName)
            };

            var  dataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_Title_SP]");

            return dataTable;

        }

        public DataTable GetDynamicDataTable(string spName, DataTable SpParameters)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@SPName", spName)
            };

            var dataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_Title_SP]");

            return dataTable;
        }


        public DataTable GetSPDATA_DT(string spName, SqlParameter[] sqlParameter )
        {
            CustomDBHelper _DB = new CustomDBHelper("RECEPTION");
            _DB.param = sqlParameter;
            var dataTable = _DB.ExecuteSPAndReturnDataTable(spName.ToString());

            return dataTable;
        }
        
        public void SavePatientComments (int TYPEID, int TYPEListID, string Comments, string PMobile , string PName,string PEmail, int CProfileID , int PMRN , int BranchID , int PatientBranchID, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@TYPEID", TYPEID),
                new SqlParameter("@TYPELISTID", TYPEListID),
                new SqlParameter("@Comments", Comments),
                new SqlParameter("@CProfileID", CProfileID),
                new SqlParameter("@PMobile", PMobile),
                new SqlParameter("@PName", PName),
                new SqlParameter("@PEmail", PEmail),
                new SqlParameter("@PMRN", PMRN),
                new SqlParameter("@BranchID", BranchID),
                new SqlParameter("@PBranchID", PatientBranchID),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200),
            };
            DB.param[10].Direction = ParameterDirection.Output;
            DB.param[11].Direction = ParameterDirection.Output;

            var flag = DB.ExecuteSP("dbo.[SAVE_APP_Comment_SP]");

            errStatus = Convert.ToInt32(DB.param[10].Value);
            errMessage = DB.param[11].Value.ToString();
        }

        public void SavePatientComments_V3(int TYPEID, int TYPEListID, string Comments, string PMobile, string PName, string PEmail, int CProfileID, int PMRN, int BranchID, int PatientBranchID,string Sources, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@TYPEID", TYPEID),
                new SqlParameter("@TYPELISTID", TYPEListID),
                new SqlParameter("@Comments", Comments),
                new SqlParameter("@CProfileID", CProfileID),
                new SqlParameter("@PMobile", PMobile),
                new SqlParameter("@PName", PName),
                new SqlParameter("@PEmail", PEmail),
                new SqlParameter("@PMRN", PMRN),
                new SqlParameter("@BranchID", BranchID),
                new SqlParameter("@PBranchID", PatientBranchID),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200),
                new SqlParameter("@Source", Sources)
                
            };
            DB.param[10].Direction = ParameterDirection.Output;
            DB.param[11].Direction = ParameterDirection.Output;

            var flag = DB.ExecuteSP("dbo.[SAVE_APP_Comment_SP]");

            errStatus = Convert.ToInt32(DB.param[10].Value);
            errMessage = DB.param[11].Value.ToString();
        }

        public DataSet GetPatientComments_DT(int MRN, int PBranchId, string Lang)
        {
            DB.param = new SqlParameter[]
            {                
                new SqlParameter("@PMRN", MRN),
                new SqlParameter("@PBranchID", PBranchId),                
                new SqlParameter("@Lang", Lang)                
            };

            var dataTable = DB.ExecuteSPAndReturnDataSet("dbo.[Get_APP_Comments_SP]");
            return dataTable;
        }

        
        public DataTable SaveNewRequest (string lang , int BranchID, int RegistrationNo, int RequestType, string RequestDetails, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId", BranchID),
                new SqlParameter("@RegistrationNo", RegistrationNo),
                new SqlParameter("@RequestType", RequestType),                
                new SqlParameter("@RequestDetails", RequestDetails),                
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200)
            };
            DB.param[5].Direction = ParameterDirection.Output;
            DB.param[6].Direction = ParameterDirection.Output;

            var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.Save_APP_Requests_SP");

            errStatus = Convert.ToInt32(DB.param[5].Value);
            errMessage = DB.param[6].Value.ToString();

            return dataTable;
        }

        public DataTable GetRequestType_DT(string Lang , int Rtype, int typelevel = 2 )
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@RType", Rtype),
                new SqlParameter("@LevelType", typelevel)
            };

            var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.[Get_RTYPE_SubLevel_SP]");
            return dataTable;
        }

        public DataTable GetRequestList_DT(int MRN,int branchId, string Lang)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@MRN", MRN),
                new SqlParameter("@BranchID", branchId),
                new SqlParameter("@Lang", Lang)
            };

            var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.[Get_ReportRequest_SP]");
            return dataTable;
        }


        public List<ReportRequest_List> GetRequestList_V3(int MRN, int branchId, string Lang)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@MRN", MRN),
                new SqlParameter("@BranchID", branchId),
                new SqlParameter("@Lang", Lang)
            };

            var dataTableSet = DB.ExecuteSPAndReturnDataSet("dbo.[Get_ReportRequest_V3_SP]");
            var DataList = MappingReportRequestList(dataTableSet);
            return DataList;
        }

        private List<ReportRequest_List> MappingReportRequestList(DataSet patientDataset)
        {
            try
            {                
                var DataList = new List<ReportRequest_List>();                

                if (patientDataset != null && patientDataset.Tables[0] != null && patientDataset.Tables[0].Rows.Count > 0)
                {
                    var ReportrequestTable = patientDataset.Tables[0].ToListObject<ReportRequest_List>();
                    foreach (var row in ReportrequestTable)
                    {
                        var ReportData = new ReportRequest_List();

                        ReportData.AddDate = row.AddDate;
                        ReportData.BranchID = row.BranchID;
                        ReportData.BranchName= row.BranchName;
                        ReportData.Comments = row.Comments;
                        ReportData.CStatusID = row.CStatusID;
                        ReportData.ID = row.ID;
                        ReportData.IsActive = row.IsActive;
                        ReportData.LastUpdateDate = row.LastUpdateDate;
                        ReportData.RegistrationNo = row.RegistrationNo;
                        ReportData.RequestDetails = row.RequestDetails;
                        ReportData.RequestType = row.RequestType;
                        ReportData.RTypeID = row.RTypeID;
                        ReportData.StatusName = row.StatusName;

                        // NOW Adding Attachment list
                        DataRow[] RRsubDataTableRow = patientDataset.Tables[1].Select("RequestID=" + row.ID);
                        if (RRsubDataTableRow != null)
                        {
                            var AttachList = new List<ReportRequest_attachments>();
                            foreach (DataRow Subrow in RRsubDataTableRow)
                            {
                                var Attatch = new ReportRequest_attachments();

                                Attatch.Attachment_URL = Subrow["Attachment_URL"].ToString();
                                Attatch.ID = (int)Subrow["ID"];
                                Attatch.RequestID = (int)Subrow["RequestID"];
                                Attatch.FileName = Subrow["FileName"].ToString();

                                AttachList.Add(Attatch);
                            }
                            ReportData.Attachments = AttachList;
                        }
                        DataList.Add(ReportData);
                    }
                }

                return DataList;


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }


        public void SENT_OTP(int hospitalId, string pCellNo, string registrationNo, string nationalId,int reason_code ,  ref int activationNo, ref int erStatus, ref string msg)
        {

            DB.param = new[]
            {
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@PCellNo", pCellNo),
                new SqlParameter("@RegistrationNo", registrationNo),
                new SqlParameter("@NationalId", nationalId),
                new SqlParameter("@ACtivationNo", SqlDbType.Int),
                new SqlParameter("@Er_Status", SqlDbType.Int),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 200),
                new SqlParameter("@ReasonType", reason_code),
            };
            DB.param[4].Direction = ParameterDirection.Output;
            DB.param[5].Direction = ParameterDirection.Output;
            DB.param[6].Direction = ParameterDirection.Output;

            var userInfoModel = DB.ExecuteSPAndReturnDataTable("DBO.[SENT_OTP_SP]");
            
            erStatus = Convert.ToInt32(DB.param[5].Value);
            msg = DB.param[6].Value.ToString();

            if (erStatus == 0)
            {
                activationNo = Convert.ToInt32(DB.param[4].Value);
                msg = msg + "OTP has been sent to provided Number";
            }
                

            return;

        }

        public Boolean VerifyOTP_Mobile(int hospitalId,string patientPhone, string VerificationCode , int ReasonCode = 2)
        {

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchId", hospitalId),                
                new SqlParameter("@CellNo", patientPhone),
                new SqlParameter("@VerificationCode", VerificationCode),
                new SqlParameter("@ReasonCode", ReasonCode)
                
            };

            DataTable dt = DB.ExecuteSPAndReturnDataTable("dbo.Verify_OTP_Mobile");

            if (dt != null && dt.Rows.Count > 0)
            {
                var testvalue =  dt.Rows[0].ItemArray.GetValue(0).ToString();
                return true;
            }

            return false;
        }

        public DataTable GetCommentType_DT(string Lang)
        {
            DB.param = new SqlParameter[]
            {              
                new SqlParameter("@Lang", Lang)
            };

            var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.[Get_CTYPE_SP]");
            return dataTable;
        }

        public DataTable GetCommentTypeList_DT(string Lang , int CtypeID)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@CTypeID", CtypeID)

            };

            var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.[Get_CTYPE_List_SP]");
            return dataTable;
        }

        public DataTable GetNotificationList_DT(string Lang, string MRN , string FCMTokken , int hospitalID)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@MRN", MRN),
                new SqlParameter("@APPToken", FCMTokken),
                new SqlParameter("@Branch_Id", hospitalID)
            };

            var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.[Get_APP_Notification_SP]");
            return dataTable;
        }

        public DataTable GetNotificationUnReadCOunt_DT(string Lang, string MRN, string FCMTokken , int HospitalID)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@MRN", MRN),
                new SqlParameter("@BranchID", HospitalID)
            };

            var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.[Get_APP_Notification_UnReadCount_SP]");
            return dataTable;
        }


        public void SaveDeviceRegistration(string PMRN, int BranchID,string APPID,string DeviceType,string MobileModal,string MobileOS,string MobileOsVersion, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@App_ID", APPID),             
                new SqlParameter("@MRN", PMRN),
                new SqlParameter("@BranchID", BranchID),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200),
                new SqlParameter("@DeviceType", DeviceType),
                new SqlParameter("@MobileModal", MobileModal),
                new SqlParameter("@MobileOS", MobileOS),
                new SqlParameter("@MobileOsVersion", MobileOsVersion)
            };
            DB.param[3].Direction = ParameterDirection.Output;
            DB.param[4].Direction = ParameterDirection.Output;

            var flag = DB.ExecuteSP("dbo.[SAVE_APP_DeviceRegistration_SP]");

            errStatus = Convert.ToInt32(DB.param[3].Value);
            errMessage = DB.param[4].Value.ToString();
        }


        public void UpdateNotificationRead(string APPID, int NotificationID, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@App_Token", APPID),
                new SqlParameter("@NotificationID", NotificationID),                
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200),
            };
            DB.param[2].Direction = ParameterDirection.Output;
            DB.param[3].Direction = ParameterDirection.Output;

            var flag = DB.ExecuteSP("dbo.[Save_APP_Notification_read_SP]");

            errStatus = Convert.ToInt32(DB.param[2].Value);
            errMessage = DB.param[3].Value.ToString();
        }



        public string UploadDocImg(HttpPostedFileBase postedFile, string PatientID , int BranchID)
        {
            string ImagePath = "";

            if (postedFile != null)
            {
                //Use Namespace called :  System.IO  
                string FileName = Path.GetFileNameWithoutExtension(postedFile.FileName);

                FileName = PatientID + "_" + BranchID.ToString();
                //To Get File Extension  
                string FileExtension = Path.GetExtension(postedFile.FileName);

                // Complete File Name (Patient MRN _ BranchID)
                FileName = FileName.Trim() + FileExtension;

                //Get Upload path from Web.Config file AppSettings.  
                string UploadPath;                
                UploadPath = ConfigurationManager.AppSettings["USerProfileImage_"].ToString(); // Read Jed if not found in App setting

                //Its Create complete path to store in server.
                ImagePath = UploadPath + FileName;

                try
                {
                    //To copy and save file into server.  
                    postedFile.SaveAs(ImagePath);

                    //UpdateImagePath(FileName, PatientID , BranchID);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    ImagePath = "";
                }



            }
            return ImagePath;
        }

        public int UpdateImagePath(string FileName, string PatientMRN , int BranchID, ref int Status, ref string msg)
        {   
            DB = new CustomDBHelper();
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@FileName", FileName),
                new SqlParameter("@Patient_Id", PatientMRN),
                new SqlParameter("@Branch_Id", BranchID),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 200)

            };
            DB.param[3].Direction = ParameterDirection.Output;
            DB.param[4].Direction = ParameterDirection.Output;

            DB.ExecuteSP("DBO.[Update_Patient_ProfileImage]");
            
            Status = Convert.ToInt32(DB.param[3].Value);
            msg = DB.param[4].Value.ToString();

            return Status;
        }

        public void SendEmail()
        {
            string to = "sfhashmi@megamind-it.com"; //To address    
            string from = "mahsan@megamind-it.com"; //From address    
            MailMessage message = new MailMessage(from, to);

            string mailbody = "New Feedback has been received and assign to you.\n\n Please take suitable action on timely manner.";
            message.Subject = "New Feedback Received";
            message.Body = mailbody;
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("smtp.office365.com", 587); //Gmail smtp    
            System.Net.NetworkCredential basicCredential1 = new
            System.Net.NetworkCredential("mahsan@megamind-it.com", "asdasd@702");
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = basicCredential1;
            try
            {
                client.Send(message);
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetCanelationReason_DT(string Lang, int BranchId, string ApiSources = "MobileApp")
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@BranchId", BranchId)
            };

            string DB_SP_Name = "dbo.Get_AppointmentCancelReason_SP";
            
            if (ApiSources.ToLower() == "saleforce")
                DB_SP_Name = "sf.Get_AppointmentCancelReason_SP";

            var dataTable = DB.ExecuteSPAndReturnDataTable(DB_SP_Name);
            return dataTable;
        }


        public DataTable GetHeardAboutUs_DT(string Lang, int BranchId)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@BranchId", BranchId)
            };

            var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.[Get_AppointmentHearFrom_SP]");
            return dataTable;
        }



        public DataTable SaveMobileUpdateRequest(int BranchID, string NationID, string PatientName, string PatientPhone, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchId", BranchID),
                new SqlParameter("@PatientName", PatientName),
                new SqlParameter("@SaudiIqamaID", NationID),
                new SqlParameter("@PatientPhone", PatientPhone),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200)
            };
            
            DB.param[4].Direction = ParameterDirection.Output;
            DB.param[5].Direction = ParameterDirection.Output;

            var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.SAVE_UpdateMobile_request_SP");

            errStatus = Convert.ToInt32(DB.param[4].Value);
            errMessage = DB.param[5].Value.ToString();

            return dataTable;
        }


        public DataTable GetServiceItemList(int BranchId , string ServiceType, int ServiceID)
        {
            DB.param = new SqlParameter[]
            {                
                new SqlParameter("@BranchId", BranchId),
                new SqlParameter("@ServiceType ", ServiceType),
                new SqlParameter("@ServiceId", ServiceID)
            };

            var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.[Get_ServiceItemsList_SP]");
            return dataTable;
        }

        public DataTable GetCMSUSERLIST_DT( string BranchId , string EMPID)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@EMPId", EMPID),
                new SqlParameter("@BranchId", BranchId)
            };
            var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.[Get_CMS_USERS_TOERP_SP]");
            return dataTable;
        }



        public DataTable Get_AppHelp_DT(string Lang, string ScreenName)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@ScreenName", ScreenName)
            };
            var dataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_App_Help_Details_SP]");
            return dataTable;
        }



        public void InsertUAESMSTABLE(string MobileNumber, string SMSTEXT)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@MobileNumber", MobileNumber),
                new SqlParameter("@SMSText", SMSTEXT)
            };
            DB.ExecuteSP("[dbo].[ADD_UAE_SMSTEXT_SP]");
        }



    }

}