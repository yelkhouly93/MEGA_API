using DataLayer.Common;
using DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;



namespace DataLayer.Data
{
    public class Login2DB
    {
        private readonly CustomDBHelper _db = new CustomDBHelper("RECEPTION");
        private readonly EncryptDecrypt_New util = new EncryptDecrypt_New();
        public UserInfo ValidateLoginUser(string lang, int hospitalId, string pCellNo, string registrationNo, string nationalId, ref int activationNo, ref int erStatus, ref string msg)
        {

            _db.param = new[]
            {
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@PCellNo", pCellNo),
                new SqlParameter("@RegistrationNo", registrationNo),
                new SqlParameter("@NationalId", nationalId),
                new SqlParameter("@ACtivationNo", SqlDbType.Int),
                new SqlParameter("@Er_Status", SqlDbType.Int),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 200)
            };
            _db.param[4].Direction = ParameterDirection.Output;
            _db.param[5].Direction = ParameterDirection.Output;
            _db.param[6].Direction = ParameterDirection.Output;

            var userInfoModel = _db.ExecuteSPAndReturnDataTable("DBO.[Validate_User2_SP]").ToListObject<UserInfo2Model>();
            var userInfo = MapUserInfoModelToUserInf(userInfoModel);


            //// AHsna New Testing ONLY  ************ Start *****************
            //CustomDBHelper _db2 = new CustomDBHelper("RECEPTION");
            //_db2.param = new SqlParameter[]
            //    {
            //        new SqlParameter("@Lang", lang),
            //        new SqlParameter("@BranchId", hospitalId),
            //        new SqlParameter("@RegistrationNo ", registrationNo),
            //        new SqlParameter("@Er_Status", SqlDbType.Int),
            //        new SqlParameter("@Msg", SqlDbType.VarChar, 100)
            //    };
            //_db2.param[3].Direction = ParameterDirection.Output;
            //_db2.param[4].Direction = ParameterDirection.Output;


            //var DataDT = _db2.ExecuteSPAndReturnDataTable("[DBO].[Get_PatientData_SP]");
            //var userInfoModel2 = DataDT.ToListObject<UserInfo2Model_New>();
            //var userInfo2 = MapUserInfoModelToUserInf_New(userInfoModel2);
            //// AHsna New Testing ONLY  ************ END *****************





            erStatus = Convert.ToInt32(_db.param[5].Value);
            msg = _db.param[6].Value.ToString();

            if (erStatus == 0)
                activationNo = Convert.ToInt32(_db.param[4].Value);
            
            return userInfo;

        }


        public UserInfo_New ValidateLoginUser_New(string lang, int hospitalId, string pCellNo, string registrationNo, string nationalId, ref int erStatus, ref string msg,string ApiSources="MobileApp")
        {
            // AHsna New Testing ONLY  ************ Start *****************
            CustomDBHelper _db2 = new CustomDBHelper("RECEPTION");
            _db2.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalId),
                    new SqlParameter("@RegistrationNo ", registrationNo),
                    new SqlParameter("@Er_Status", SqlDbType.Int),
                    new SqlParameter("@Msg", SqlDbType.NVarChar, 200)
                };
            _db2.param[3].Direction = ParameterDirection.Output;
            _db2.param[4].Direction = ParameterDirection.Output;


            string DB_SP_Name = "[DBO].[Get_PatientData_SP]";

            if (ApiSources.ToLower() == "saleforce")
                DB_SP_Name = "[SF].[Get_PatientData_SP]";


            var DataDT = _db2.ExecuteSPAndReturnDataTable(DB_SP_Name);
            var userInfoModel2 = DataDT.ToListObject<UserInfo2Model_New>();
            var userInfo2 = MapUserInfoModelToUserInf_New(userInfoModel2);
            // AHsna New Testing ONLY  ************ END *****************





            erStatus = Convert.ToInt32(_db2.param[3].Value);
            msg = _db2.param[4].Value.ToString();

            //if (erStatus == 0)
            //    activationNo = Convert.ToInt32(_db.param[4].Value);
            //activationNo = 0;

            return userInfo2;

        }


        private static UserInfo MapUserInfoModelToUserInf(List<UserInfo2Model> userInfoModel)
        {
            UserInfo userInfo = new UserInfo();
            if (userInfoModel == null || userInfoModel.Count <= 0) return userInfo;
            userInfo.address = userInfoModel[0].PAddress;
            userInfo.birthday = userInfoModel[0].DOB;
            userInfo.email = userInfoModel[0].PEmail;
            userInfo.family_name = userInfoModel[0].FamilyName;
            userInfo.first_name = userInfoModel[0].FirstName;
            userInfo.gender = userInfoModel[0].GenderId;
            userInfo.hospital_id = userInfoModel[0].Branch_Id;
            userInfo.id = userInfoModel[0].PatientId;
            userInfo.last_name = userInfoModel[0].LastName;
            userInfo.marital_status_id = userInfoModel[0].MaritalStatusId;
            userInfo.middle_name = userInfoModel[0].MiddleName;
            userInfo.name = userInfoModel[0].Name;
            userInfo.national_id = userInfoModel[0].NationalId;
            userInfo.phone = userInfoModel[0].PCellno;
            userInfo.registration_no = userInfoModel[0].RegistrationNo.ToString();
            userInfo.title_id = userInfoModel[0].TitleId;
            userInfo.title_id = userInfoModel[0].TitleId;
            userInfo.name_ar = userInfoModel[0].name_ar;

            return userInfo;
        }

        private static UserInfo_New MapUserInfoModelToUserInf_New(List<UserInfo2Model_New> userInfoModel)
        {
            UserInfo_New userInfo = new UserInfo_New();
            if (userInfoModel == null || userInfoModel.Count <= 0) return userInfo;
            userInfo.address = userInfoModel[0].PAddress;
            userInfo.birthday = userInfoModel[0].DOB;
            userInfo.email = userInfoModel[0].PEmail;
            userInfo.family_name = userInfoModel[0].FamilyName;
            userInfo.first_name = userInfoModel[0].FirstName;
            userInfo.gender = userInfoModel[0].Gender;
            userInfo.gender_ar = userInfoModel[0].Gender_ar;
            userInfo.hospital_id = userInfoModel[0].Branch_Id;
            userInfo.id = userInfoModel[0].PatientId;
            userInfo.national_id =  userInfoModel[0].PatientId;
            userInfo.last_name = userInfoModel[0].LastName;
            userInfo.marital_status_id = userInfoModel[0].MaritalStatusId;
            userInfo.middle_name = userInfoModel[0].MiddleName;
            userInfo.name = userInfoModel[0].PatientFullName;
            //userInfo.national_id = userInfoModel[0].NationalId;
            userInfo.phone = userInfoModel[0].PCellno;
            userInfo.registration_no = userInfoModel[0].RegistrationNo.ToString();
            userInfo.title_id = userInfoModel[0].Title;            
            userInfo.name_ar = userInfoModel[0].name_ar;
            userInfo.FamilyMembersCount = userInfoModel[0].FamilyMembersCount;


            userInfo.Age= userInfoModel[0].Age;
            userInfo.BloodGroup = userInfoModel[0].BloodGroup;
            userInfo.Weight = userInfoModel[0].pWeight;
            userInfo.Height = userInfoModel[0].pHeight;


            userInfo.NationalityId = userInfoModel[0].NationalityId;
            userInfo.Nationality = userInfoModel[0].Nationality;


            userInfo.image_url  = userInfoModel[0].image_url;

            userInfo.IsCash = userInfoModel[0].IsCash;

            userInfo.Branch_Id  = userInfoModel[0].Branch_Id;
            userInfo.Branch_Name  = userInfoModel[0].Branch_Name;

            userInfo.Branch_Name_ar = userInfoModel[0].Branch_AR;


            return userInfo;
        }


		public DataTable ValidateLoginUser_List(string Lang, int hospitalId, string pCellNo, string nationalId, int registrationNo, string Source, ref int erStatus, ref string msg, ref string ACtivationNo, bool IsEncrypt = true)
		{
			//var temp = util.Encrypt("Megamind", true);
			//var temp1 = util.Encrypt("Megamind", false);
			//GetDependentPatientDataTable

			_db.param = new SqlParameter[]
			{
				new SqlParameter("@Lang", Lang),
				new SqlParameter("@BranchId", hospitalId),
				new SqlParameter("@PcellNo", pCellNo),
				new SqlParameter("@NationalId", nationalId),
				new SqlParameter("@RegistrationNo", registrationNo),
				new SqlParameter("@Er_Status", SqlDbType.Int),
				new SqlParameter("@Msg", SqlDbType.NVarChar, 500),
				new SqlParameter("@ACtivationNo", SqlDbType.VarChar, 100),
				new SqlParameter("@Source", Source)
			};
			_db.param[5].Direction = ParameterDirection.Output;
			_db.param[6].Direction = ParameterDirection.Output;
			_db.param[7].Direction = ParameterDirection.Output;

			var dt = _db.ExecuteSPAndReturnDataTable("DBO.[Validate_User3_SP]");

			erStatus = Convert.ToInt32(_db.param[5].Value);
			msg = _db.param[6].Value.ToString();
			ACtivationNo = _db.param[7].Value.ToString();

			if (erStatus != 1 && IsEncrypt)
			{
				dt = Encrpt_LoginUserList_dt(dt);
			}

			return dt;
		}
		
        // New Copy Changes Due to Dammam
		public List<login_check_modal>  login_check(string Lang, int hospitalId, string pCellNo, string nationalId, int registrationNo, string Source, ref int erStatus, ref string msg, bool IsEncrypt = true)
        {
            //login_check_modal

            _db.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@PcellNo", pCellNo),
                new SqlParameter("@NationalId", nationalId),
                new SqlParameter("@RegistrationNo", registrationNo),
                new SqlParameter("@Er_Status", SqlDbType.Int),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 500),                
                new SqlParameter("@Source", Source)
            };
            _db.param[5].Direction = ParameterDirection.Output;
            _db.param[6].Direction = ParameterDirection.Output;            

            var dt = _db.ExecuteSPAndReturnDataTable("DBO.[Validate_User_V4_SP]").ToListObject<login_check_modal>();

            erStatus = Convert.ToInt32(_db.param[5].Value);
            msg = _db.param[6].Value.ToString();
            //if (erStatus != 1 && IsEncrypt)
            //{
            //    dt = Encrpt_LoginUserList_dt(dt);
            //}
            //List < login_check_modal >
            return dt;
        }



        public List<UserLoginInfoList> ValidateLoginUser_List_Encypt(string Lang, int hospitalId, string pCellNo, string nationalId, int registrationNo, ref int erStatus, ref string msg, ref string ACtivationNo)
        {
            //GetDependentPatientDataTable
            _db.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@PcellNo", pCellNo),
                new SqlParameter("@NationalId", nationalId),
                new SqlParameter("@RegistrationNo", registrationNo),
                new SqlParameter("@Er_Status", SqlDbType.Int),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 500),
                new SqlParameter("@ACtivationNo", SqlDbType.VarChar, 100)
            };
            _db.param[5].Direction = ParameterDirection.Output;
            _db.param[6].Direction = ParameterDirection.Output;
            _db.param[7].Direction = ParameterDirection.Output;

            var dt = _db.ExecuteSPAndReturnDataTable("DBO.[Validate_User3_SP]").ToListObject_ENCYPT<UserLoginInfoList>();


            erStatus = Convert.ToInt32(_db.param[5].Value);
            msg = _db.param[5].Value.ToString();
            ACtivationNo = _db.param[7].Value.ToString();

            //if (erStatus != 1)
            //{
            //    dt = Encrpt_LoginUserList_dt(dt);
            //}

            return dt;
        }



        public DataTable Encrpt_LoginUserList_dt(DataTable dtobj)
        {
            var ReturnObj = new DataTable();

            ReturnObj.Columns.Add(new DataColumn() { ColumnName = "Branch_EN", DataType = typeof(string) });
            ReturnObj.Columns.Add(new DataColumn() { ColumnName = "Branch_AR", DataType = typeof(string) });
            ReturnObj.Columns.Add(new DataColumn() { ColumnName = "PatientId", DataType = typeof(string) });
            ReturnObj.Columns.Add(new DataColumn() { ColumnName = "PatientName_EN", DataType = typeof(string) });
            ReturnObj.Columns.Add(new DataColumn() { ColumnName = "PatientName_AR", DataType = typeof(string) });
            ReturnObj.Columns.Add(new DataColumn() { ColumnName = "PatientCellNo", DataType = typeof(string) });
            ReturnObj.Columns.Add(new DataColumn() { ColumnName = "PEMail", DataType = typeof(string) });
            ReturnObj.Columns.Add(new DataColumn() { ColumnName = "Registrationno", DataType = typeof(string) });
            ReturnObj.Columns.Add(new DataColumn() { ColumnName = "BranchID", DataType = typeof(string) });
            ReturnObj.Columns.Add(new DataColumn() { ColumnName = "PatientCellNo2", DataType = typeof(string) });
            ReturnObj.Columns.Add(new DataColumn() { ColumnName = "PatientFullName", DataType = typeof(string) });
            ReturnObj.Columns.Add(new DataColumn() { ColumnName = "DOB", DataType = typeof(string) });
            ReturnObj.Columns.Add(new DataColumn() { ColumnName = "image_url", DataType = typeof(string) });



            if (dtobj != null)
            {
                //int i = 0;
                foreach (DataRow dr in dtobj.Rows) // search whole table
                {
                    string strBranch_EN = dr["Branch_EN"].ToString();
                    string strBranch_AR = dr["Branch_AR"].ToString();
                    string StrRegNumber = dr["Registrationno"].ToString();                    

                    var tempDR = ReturnObj.NewRow();
                    tempDR["Branch_EN"] = util.Encrypt( dr["Branch_EN"].ToString() , true) ;
                    tempDR["Branch_AR"] = util.Encrypt(dr["Branch_AR"].ToString(), true);
                    tempDR["Registrationno"] = util.Encrypt(dr["Registrationno"].ToString(), true);
                    tempDR["BranchID"] = util.Encrypt(dr["BranchID"].ToString(), true);

                    tempDR["PatientId"] = dr["PatientId"].ToString();
                    tempDR["PatientName_EN"] = dr["PatientName_EN"].ToString();
                    tempDR["PatientCellNo"] = dr["PatientCellNo"].ToString();

                    
                    if (dtobj.Rows.Count == 1)                    
                        tempDR["PatientCellNo2"] = dr["PatientCellNo2"].ToString();

                    if (dtobj.Rows.Count > 1)
                    {
                        tempDR["DOB"] = dr["DOB"].ToString();
                        tempDR["image_url"] = dr["image_url"].ToString();
                    }
                    tempDR["PatientFullName"] = "";

                    ReturnObj.Rows.Add(tempDR);
                }

            }


            //for (int i = 0; i < dtobj.Rows.Count; i++)
            //{
            //    string strBranch_EN = dtobj.Rows[i].Field<string>("Branch_EN");
            //    string strBranch_AR = dtobj.Rows[i].Field<string>("Branch_AR");
            //    string StrRegNumber = dtobj.Rows[i].Field<string>("Registrationno");

            //    ReturnObj.Rows[i].SetField<string>("Registrationno") = StrRegNumber;
            //}

            return ReturnObj;
        }


        //public List <UserInfo2Model> LoginUserinfoList(string lang, string pCellNo, string registrationNo, string nationalId,  ref int erStatus, ref string msg)
        //{
        //    _db.param = new[]
        //    {   
        //        new SqlParameter("@PCellNo", pCellNo),
        //        new SqlParameter("@RegistrationNo", registrationNo),
        //        new SqlParameter("@NationalId", nationalId),
        //        new SqlParameter("@Er_Status", SqlDbType.Int),
        //        new SqlParameter("@Msg", SqlDbType.VarChar, 100)
        //    };
        //    _db.param[3].Direction = ParameterDirection.Output;
        //    _db.param[4].Direction = ParameterDirection.Output;

        //    var userInfoModel = _db.ExecuteSPAndReturnDataTable("DBO.[Search_Patient_list_SP]").ToListObject<UserInfo2Model>();
        //    var userInfo = MapUserInfoModelToUserInf(userInfoModel);
        //    erStatus = Convert.ToInt32(_db.param[5].Value);
        //    msg = _db.param[6].Value.ToString();
        //    return userInfoModel;

        //}



        public void CheckLoginUser_Mobile(string lang,int hospitalId, string pCellNo, ref int erStatus, ref string msg, ref string ACtivationNo)
        {   

            _db.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@PcellNo", pCellNo),
                new SqlParameter("@Er_Status", SqlDbType.Int),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 1000),
                new SqlParameter("@ACtivationNo", SqlDbType.VarChar, 100)
            };
            _db.param[3].Direction = ParameterDirection.Output;
            _db.param[4].Direction = ParameterDirection.Output;
            _db.param[5].Direction = ParameterDirection.Output;

            _db.ExecuteSP("DBO.[Validate_UserMobile_SP]");

            erStatus = Convert.ToInt32(_db.param[3].Value);
            msg = _db.param[4].Value.ToString();
            ACtivationNo = _db.param[5].Value.ToString();

        }


        public string GetVerificaitonCode2Resend(int hospitalId, string patientRegNo, string patientPhone, int OTP_Type)
        {

            _db.param = new SqlParameter[]
            {
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@RegistratioNo", patientRegNo),
                new SqlParameter("@CellNo", patientPhone),
                new SqlParameter("@VerificationCodeReason", OTP_Type)
                
            };

            DataTable dt = _db.ExecuteSPAndReturnDataTable("dbo.Get_VerificationCode2Resend_SP");

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0].ItemArray.GetValue(0).ToString();
            }
            else
            {
                return null;
            }
        }



        public UserInfo_New ValidatePatientPassword(string Lang, string PatientPwd, string nationalId, ref int erStatus, ref string msg)
        {
            _db.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@PatientPwd", PatientPwd),                
                new SqlParameter("@NationalId", nationalId),                
                new SqlParameter("@Er_Status", SqlDbType.Int),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 1000)                
            };
            _db.param[3].Direction = ParameterDirection.Output;
            _db.param[4].Direction = ParameterDirection.Output;

            var dt = _db.ExecuteSPAndReturnDataTable("DBO.[Validate_Patient_PWD_SP]");

            
            var userInfoModel2 = dt.ToListObject<UserInfo2Model_New>();
            var userInfo2 = MapUserInfoModelToUserInf_New(userInfoModel2);
            // AHsna New Testing ONLY  ************ END *****************


            erStatus = Convert.ToInt32(_db.param[3].Value);
            msg = _db.param[4].Value.ToString();


            return userInfo2;
        }



    }



}
