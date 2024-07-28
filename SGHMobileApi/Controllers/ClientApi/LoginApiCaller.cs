using DataLayer.Common;
using DataLayer.Model;
using Newtonsoft.Json.Linq;
using RestClient;
using SGHMobileApi.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;

namespace SmartBookingService.Controllers.ClientApi
{
    public class LoginApiCaller
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");
        private readonly EncryptDecrypt_New util = new EncryptDecrypt_New();

        public UserInfo ValidateLoginUserByApi(string lang, int hospitalID, string PCellNo, string RegistrationNo, string NationalId, ref int activationNo, ref int Er_Status, ref string Msg)
        {

            HttpStatusCode status;
            UserInfo _userInfo = new UserInfo();
            accountData _accData = new accountData();
            string apiBasic = ConfigurationManager.AppSettings["MobileWebApi_BasicURL_" + hospitalID.ToString()].ToString();
            string RegistrationUrl = apiBasic + ConfigurationManager.AppSettings["MobileWebApi_RetreivePatientInfo_" + hospitalID.ToString()].ToString();

            var apiUserName = ConfigurationManager.AppSettings["MobileWebApi_UserName"].ToString();
            var apiPassword = ConfigurationManager.AppSettings["MobileWebApi_Password"].ToString();
            var apiFixedPatientId = ConfigurationManager.AppSettings["MobileWebApi_FixedPatientId"].ToString();

            if (RegistrationNo != "" && RegistrationNo != null)
            {
                _accData = new accountData() { idNumber = RegistrationNo, idType = "MRN" };
            }
            else
            {
                _accData = new accountData() { idNumber = NationalId, idType = "ID" };
            }

            patientData _data = RestUtility.CallService<patientData>(RegistrationUrl, null, _accData, "POST", apiUserName, apiPassword, out status) as patientData;

            var _userInfoModel = new List<UserInfo2Model>();

            DB.param = new SqlParameter[]
                {
                new SqlParameter("@BranchId", hospitalID),
                new SqlParameter("@PCellNo", _data != null ? _data.MOBILE_NO : ""),
                new SqlParameter("@RegistrationNo", _data != null ? _data.MRN.ToString() : "0"),
                new SqlParameter("@NationalId", NationalId),
                new SqlParameter("@PatientId", _data != null ? _data.PATIENT_ID : 0),
                new SqlParameter("@ACtivationNo", SqlDbType.Int),
                new SqlParameter("@Er_Status", status == HttpStatusCode.OK ? 0 : 1),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 500)
                };
            DB.param[5].Direction = ParameterDirection.Output;
            DB.param[6].Direction = ParameterDirection.InputOutput;
            DB.param[7].Direction = ParameterDirection.Output;

            _userInfoModel = DB.ExecuteSPAndReturnDataTable("dbo.Generate_OTP").ToListObject<UserInfo2Model>();

            Er_Status = Convert.ToInt32(DB.param[6].Value);
            Msg = DB.param[7].Value.ToString();

            if (Er_Status == 0)
                activationNo = Convert.ToInt32(DB.param[5].Value);

            if (status == HttpStatusCode.OK)
            {
                _userInfo = MapUserInfoModelToUserInf(_data);
            }

            return _userInfo;

        }

        private UserInfo MapUserInfoModelToUserInf(patientData _userInfoModel)
        {
            UserInfo _userInfo = new UserInfo();

            _userInfo.address = "";
            _userInfo.birthday = Convert.ToDateTime(_userInfoModel.DATE_OF_BIRTH);
            _userInfo.email = _userInfoModel.EMAIL_ADDRESS;
            _userInfo.family_name = _userInfoModel.PAT_NAME_FAMILY;
            _userInfo.first_name = _userInfoModel.PAT_NAME_1;
            _userInfo.gender = Convert.ToInt32(_userInfoModel.SEX != null ? Util.GetSexID(_userInfoModel.SEX.ToString()) : "0");  //getGenderId(_userInfoModel.SEX)
            _userInfo.hospital_id = 10;
            _userInfo.id = _userInfoModel.PATIENT_ID;
            _userInfo.last_name = _userInfoModel.PAT_NAME_3;
            _userInfo.marital_status_id = 0;
            _userInfo.middle_name = _userInfoModel.PAT_NAME_2;
            _userInfo.name = _userInfoModel.PATIENT_NAME;
            _userInfo.national_id = "";
            _userInfo.phone = _userInfoModel.MOBILE_NO;
            //_userInfo.registration_no = _userInfoModel.MRN;
            _userInfo.registration_no = _userInfoModel.MRN.ToString();
            _userInfo.title_id = 0;
            _userInfo.name_ar = _userInfoModel.PATIENT_NAME;

            return _userInfo;
        }



        public List<login_check_modal> ValidateLoginUserByApi_NewDam(string lang, string idValue, string IdType, ref int Er_Status, ref string Msg)
        {

            HttpStatusCode status;
            //patientData_Dam _userInfo = new patientData_Dam();
            accountData_dam _accData = new accountData_dam();		
			string RegistrationUrl = "http://130.11.2.213:30005/getPatientInfo";

            _accData = new accountData_dam() { idValue = idValue, idType = IdType };

            var content = new[]{
                    new KeyValuePair<string, string>("idValue", idValue.ToString()),
                    new KeyValuePair<string, string>("idType", IdType)
            };


            RegistrationUrl = "http://130.11.2.213:30005/getPatientInfo?idValue=" + idValue.ToString() + "&idType=" + IdType.ToString();
            var _NewData = RestUtility.CallAPI<List<patientData_Dam>>(RegistrationUrl, content);


            var _patientData_Dam = new List<patientData_Dam>();
            _patientData_Dam = _NewData as List<patientData_Dam>;

            var _userInfo = MapUserInfoModelToUserInf_NewDamma(_patientData_Dam);

            //if (status == HttpStatusCode.OK)
            //{
            //    //_userInfo = MapUserInfoModelToUserInf(_data);
            //}
            return _userInfo;

        }

        private List<login_check_modal> MapUserInfoModelToUserInf_NewDamma(List<patientData_Dam> _userInfoModel)
        {
            List<login_check_modal> _userInfo = new List<login_check_modal>();

            if (_userInfoModel != null && _userInfoModel.Count > 0)
            {
                for (var i = 0; i < _userInfoModel.Count; i++)
				{
                    login_check_modal _TempModalObj = new login_check_modal();



                    //_TempModalObj.BranchId = util.Encrypt("9", true);  // FOr Dammam Fixed
                    //_TempModalObj.branchID = util.Encrypt("9", true); // FOr Dammam Fixed
                    //_TempModalObj.Branch_AR = util.Encrypt("Dammam Branch", true);// FOr Dammam Fixed
                    //_TempModalObj.Branch_EN = util.Encrypt("Dammam Branch", true);// FOr Dammam Fixed
                    //_TempModalObj.Registrationno = util.Encrypt(_userInfoModel[i].registration_no.ToString(), true);
                    //_TempModalObj.DOB = _userInfoModel[i].birthday.ToString();
                    //_TempModalObj.image_url = "";
                    //_TempModalObj.PatientCellNo = _userInfoModel[i].phone.ToString();
                    //_TempModalObj.PatientCellNo2 = _userInfoModel[i].phone.ToString();
                    ////_TempModalObj.PatientCellNo = "0581178188";
                    ////_TempModalObj.PatientCellNo2 = "0581178188";
                    //_TempModalObj.PatientFullName = _userInfoModel[i].name.ToString();
                    //_TempModalObj.PatientId = _userInfoModel[i].registration_no.ToString();
                    //_TempModalObj.PatientName_AR = _userInfoModel[i].name_ar.ToString();
                    //_TempModalObj.PatientName_EN = _userInfoModel[i].name.ToString();
                    //_TempModalObj.PEMail = _userInfoModel[i].email.ToString();

                    if (_userInfoModel[i].registration_no.ToString() != "" && _userInfoModel[i].registration_no.ToString() != "0")
					{
                        _TempModalObj.BranchId = "9";  // FOr Dammam Fixed
                        _TempModalObj.branchID = "9"; // FOr Dammam Fixed
                        _TempModalObj.Branch_AR = "المستشفى السعودي الألماني الدمام";// FOr Dammam Fixed
                        _TempModalObj.Branch_EN = "Dammam Branch";// FOr Dammam Fixed
                        _TempModalObj.Registrationno = _userInfoModel[i].registration_no.ToString();
                        _TempModalObj.DOB = _userInfoModel[i].birthday.ToString();
                        _TempModalObj.image_url = "";
                        _TempModalObj.PatientCellNo = _userInfoModel[i].phone.ToString();
                        _TempModalObj.PatientCellNo2 = _userInfoModel[i].phone.ToString();

                        //_TempModalObj.PatientFullName = _userInfoModel[i].name.ToString();
                        _TempModalObj.PatientFullName = MaskFullName(_userInfoModel[i].name.ToString());

                        _TempModalObj.PatientId = _userInfoModel[i].registration_no.ToString();
                        _TempModalObj.PatientName_AR = _userInfoModel[i].name_ar.ToString();
                        _TempModalObj.PatientName_EN = _userInfoModel[i].name.ToString();
                        _TempModalObj.PEMail = _userInfoModel[i].email.ToString();
                        _userInfo.Add(_TempModalObj);
                    }
                    
                }
            }
            return _userInfo;
        }

        public static string MaskFullName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                throw new ArgumentNullException(nameof(fullName), "Full name cannot be null or empty.");
            }

            if (fullName.Length <= 6)
            {
                throw new ArgumentException("Full name should be longer than 6 characters.", nameof(fullName));
            }

            int visibleStartLength = 3;
            int visibleEndLength = 3;
            int maskedLength = fullName.Length - visibleStartLength - visibleEndLength;

            string visibleStart = fullName.Substring(0, visibleStartLength);
            string visibleEnd = fullName.Substring(fullName.Length - visibleEndLength, visibleEndLength);
            string maskedPart = new string('*', maskedLength);

            return visibleStart + maskedPart + visibleEnd;
        }


        public UserInfo_New GetPatientDataByApi_NewDam(string lang, string idValue, string IdType, ref int Er_Status, ref string Msg)
        {

            HttpStatusCode status;
            //patientData_Dam _userInfo = new patientData_Dam();
            accountData_dam _accData = new accountData_dam();
            string RegistrationUrl = "http://130.11.2.213:30005/getPatientInfo";

            _accData = new accountData_dam() { idValue = idValue, idType = IdType };

            var content = new[]{
                    new KeyValuePair<string, string>("idValue", idValue.ToString()),
                    new KeyValuePair<string, string>("idType", IdType)
            };


            RegistrationUrl = "http://130.11.2.213:30005/getPatientInfo?idValue=" + idValue.ToString() + "&idType=" + IdType.ToString();
            var _NewData = RestUtility.CallAPI<List<patientData_Dam>>(RegistrationUrl, content);


            var _patientData_Dam = new List<patientData_Dam>();
            _patientData_Dam = _NewData as List<patientData_Dam>;

            var _userInfo = MapUserInfoModelToUserInf_DamNew(_patientData_Dam);

            //if (status == HttpStatusCode.OK)
            //{
            //    //_userInfo = MapUserInfoModelToUserInf(_data);
            //}
            return _userInfo;

        }

        private static UserInfo_New MapUserInfoModelToUserInf_DamNew(List<patientData_Dam> userInfoModel)
        {
            UserInfo_New userInfo = new UserInfo_New();
            if (userInfoModel == null || userInfoModel.Count <= 0) return userInfo;
            userInfo.address = userInfoModel[0].address;
            try
			{
                if (userInfoModel[0].birthday != "")
                    userInfo.birthday = DateTime.Parse(userInfoModel[0].birthday);
            }
            catch (Exception e)
			{

			}
            

            userInfo.email = userInfoModel[0].email;
            userInfo.family_name = userInfoModel[0].family_name;
            userInfo.first_name = userInfoModel[0].first_name;
            userInfo.gender = userInfoModel[0].gender;
            userInfo.gender_ar = userInfoModel[0].gender;
            userInfo.hospital_id = "9";
            userInfo.id = userInfoModel[0].registration_no;
            userInfo.national_id = userInfoModel[0].national_id;
            userInfo.last_name = userInfoModel[0].last_name;
            userInfo.marital_status_id = userInfoModel[0].marital_status_id;
            userInfo.middle_name = userInfoModel[0].middle_name;
            userInfo.name = userInfoModel[0].name;            
            userInfo.phone = userInfoModel[0].phone;
            userInfo.registration_no = userInfoModel[0].registration_no;
            userInfo.title_id = userInfoModel[0].title_id;
            userInfo.name_ar = userInfoModel[0].name_ar;
            userInfo.FamilyMembersCount = 0;


            userInfo.Age = userInfoModel[0].age;
            userInfo.BloodGroup = userInfoModel[0].bloodGroup;
            userInfo.Weight = userInfoModel[0].weight;
            userInfo.Height = userInfoModel[0].height;


            userInfo.NationalityId = userInfoModel[0].nationalityId;
            userInfo.Nationality = userInfoModel[0].nationality;

            if (userInfo.NationalityId == "")
                userInfo.NationalityId = "0";


                userInfo.image_url = "";

            userInfo.IsCash = true;

            userInfo.Branch_Id = "9";
            userInfo.Branch_Name = "SGH Dammam";

            userInfo.Branch_Name_ar = "المستشفى السعودي الألماني الدمام";


            return userInfo;
        }


        // Perscription
        public List<Medical_Perscription_modal> GetPatientPerscriptionByApi_NewDam(string lang, string MRN,  ref int Er_Status, ref string Msg)
        {

            HttpStatusCode status;
            //patientData_Dam _userInfo = new patientData_Dam();
            
            string RegistrationUrl = "http://130.11.2.213:30005/getPharmacyOrders?mrn=" + MRN;
            var _NewData = RestUtility.CallAPI_Perscription<List<Medical_Perscription_Dam>>(RegistrationUrl, null);


            //var _patientPerscriptionData_Dam = new List<Medical_Perscription_Dam>();
            var _patientPerscriptionData_Dam = _NewData as List<Medical_Perscription_Dam>;

            var _MapDATA = MapPerscriptionINfoModel_NewDamma(_patientPerscriptionData_Dam).OrderByDescending(o =>o.Prescription_Date).ToList ();

            //List<Order> SortedList = objListOrder.OrderBy(o => o.OrderDate).ToList();
            //if (status == HttpStatusCode.OK)
            //{
            //    //_userInfo = MapUserInfoModelToUserInf(_data);
            //}
            return _MapDATA;

        }

        private List<Medical_Perscription_modal> MapPerscriptionINfoModel_NewDamma(List<Medical_Perscription_Dam> _APIModal)
        {
            string formatString = "yyyyMMddHHmmss";
            
            
            List<Medical_Perscription_modal> _ListObj = new List<Medical_Perscription_modal>();

            if (_APIModal != null && _APIModal.Count > 0)
            {
                for (var i = 0; i < _APIModal.Count; i++)
                {
                    Medical_Perscription_modal _TempModalObj = new Medical_Perscription_modal();
                    _TempModalObj.Active = 0; // FOr Dammam Fixed
                    _TempModalObj.Age = " ";// FOr Dammam Fixed
                    _TempModalObj.Company = " ";// FOr Dammam Fixed
                    _TempModalObj.Doctor_Name = " ";
                    _TempModalObj.Dosage = _APIModal[i].orders[0].dose + " " + _APIModal[i].orders[0].dispensingUnit;
                    //_TempModalObj.DrugId = _APIModal[i].orders[0].drugCode;
                    _TempModalObj.DrugId = Convert.ToInt32(_APIModal[i].orders[0].orderNo);


                    _TempModalObj.Drug_Name = _APIModal[i].orders[0].drugName;
                    _TempModalObj.Duration = _APIModal[i].orders[0].duration + " " + _APIModal[i].orders[0].durationUom;
                    _TempModalObj.Frequency = _APIModal[i].orders[0].frequency + " / " + _APIModal[i].orders[0].frequencyUom;
                    _TempModalObj.Instructions = " ";
                    _TempModalObj.PrescriptionId = Convert.ToInt32( _APIModal[i].prescriptionNo);
                    DateTime dt = DateTime.ParseExact(_APIModal[i].orders[0].requestDate, formatString, null);
                    
                    _TempModalObj.Prescription_Date = dt.ToString("yyyy-MM-dd");
                    _TempModalObj.Remarks = " ";
                    _TempModalObj.Route = " ";
                    _TempModalObj.Strength = " ";
                    _TempModalObj.UNIFIED_DEPT_NAME = " ";
                    _TempModalObj.Visit_Id = Convert.ToInt32(_APIModal[i].orders[0].orderNo);

                    _ListObj.Add(_TempModalObj);
                }
            }
            return _ListObj;
        }

        public void GenerateOTP_V3 (string hospitalID ,string MOBILE_NO,string MRN,string NationalId,string Source, ref int activationNo, ref int Er_Status, ref string Msg , int CountryId = 0)
		{
            DB.param = new SqlParameter[]
                {
                new SqlParameter("@BranchId", hospitalID),
                new SqlParameter("@PCellNo", MOBILE_NO),
                new SqlParameter("@RegistrationNo", MRN),
                new SqlParameter("@NationalId", NationalId),
                new SqlParameter("@Source", Source),
                new SqlParameter("@ACtivationNo", SqlDbType.Int),
                new SqlParameter("@Er_Status", SqlDbType.Int),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 500),
                new SqlParameter("@CountryID", CountryId)
                };
            DB.param[5].Direction = ParameterDirection.Output;
            DB.param[6].Direction = ParameterDirection.Output;
            DB.param[7].Direction = ParameterDirection.Output;

            DB.ExecuteSPAndReturnDataTable("dbo.Generate_OTP_V3");

            Er_Status = Convert.ToInt32(DB.param[6].Value);
            Msg = DB.param[7].Value.ToString();

            if (Er_Status == 0)
                activationNo = Convert.ToInt32(DB.param[5].Value);
        }


        public void GetNationalityCode(int hospitalID, int UnifiedID, ref int NationalityID)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@BranchId", hospitalID),
                    new SqlParameter("@UnifiedID", UnifiedID),
                    new SqlParameter("@NationalityCode", SqlDbType.Int)                
                };
            DB.param[2].Direction = ParameterDirection.Output;
            DB.ExecuteSPAndReturnDataTable("dbo.Get_NationalityCode_FromUnifiedId_SP");

            NationalityID = Convert.ToInt32(DB.param[2].Value);
        }


        // Appointment
        public List<Patient_Appointment_Modal_ForMobile> GetPatientAppointmentsByApi_NewDam(string lang, string MRN)
        {
            HttpStatusCode status;
            string RegistrationUrl = "http://130.11.2.213:30005/appointmentsHistory?mrn=" + MRN;
            var _NewDataAPI = RestUtility.CallAPI<List<Patient_Appointment_Modal>>(RegistrationUrl, null) as List<Patient_Appointment_Modal>;
            var _NewData = MapPatientAppointments_NewDamma(_NewDataAPI).OrderByDescending(o => o.IsUpComming).ThenBy(o => o.DurationLeft).ThenByDescending(o => o.AppDate).ToList();

            return _NewData;
        }


        private List<Patient_Appointment_Modal_ForMobile> MapPatientAppointments_NewDamma(List<Patient_Appointment_Modal> _APIModal , string lang = "EN")
        {
            List<Patient_Appointment_Modal_ForMobile> _ListObj = new List<Patient_Appointment_Modal_ForMobile>();
            //Doctor_Schedule_days_Modal_ForMobile
            if (_APIModal != null && _APIModal.Count > 0)
            {
                for (var i = 0; i < _APIModal.Count; i++)
                {
                    Patient_Appointment_Modal_ForMobile _TempModalObj = new Patient_Appointment_Modal_ForMobile();
                    var testint = 0;

     //               try
					//{
     //                   testint = Convert.ToInt32(_APIModal[i].DoctorId);

     //               }
     //               catch (Exception ex)
					//{
     //                   continue;
					//}



					try
					{
                        _TempModalObj.AppDate = _APIModal[i].AppDate;
                        _TempModalObj.AppointmentNo = Convert.ToInt32(_APIModal[i].AppointmentNo);
                        _TempModalObj.AppTime = _APIModal[i].AppTime + ":00";
                        if (lang == "AR" || lang == "ar")
                            _TempModalObj.BranchName = "المستشفى السعودي الألماني الدمام";
                        else
                            _TempModalObj.BranchName = "SGH Dammam"; 

                        _TempModalObj.ClinicName = _APIModal[i].ClinicName;
                        _TempModalObj.ClinicName_AR = _APIModal[i].ClinicName_AR;
                        _TempModalObj.DoctorId = Convert.ToInt32(_APIModal[i].DoctorId);
                        _TempModalObj.DoctorName = _APIModal[i].DoctorName;
                        _TempModalObj.DoctorName_AR = _APIModal[i].DoctorName_AR;

                        if (_APIModal[i].AppDate <= System.DateTime.Now.Date)
                        {
                            _TempModalObj.DurationLeft = 0;
                        }
                        else
                        {
                            int dateDifference = (_APIModal[i].AppDate - System.DateTime.Now.Date).Days;
                            _TempModalObj.DurationLeft = dateDifference;
                        }


                        //_TempModalObj.Id = Convert.ToInt32(_APIModal[i].Id);
                        _TempModalObj.Id = Convert.ToInt32(_APIModal[i].AppointmentNo);

                        //_TempModalObj.image_url = _APIModal[i].image_url;
                        _TempModalObj.isPaid = _APIModal[i].isPaid;
                        _TempModalObj.IsUpComming = Convert.ToInt32(_APIModal[i].IsUpComming);
                        _TempModalObj.PatientName = _APIModal[i].PatientName;
                        _TempModalObj.PatientVisited = Convert.ToInt32(_APIModal[i].PatientVisited);
                        _TempModalObj.RegistrationNo = Convert.ToInt32(_APIModal[i].RegistrationNo);
                        //_TempModalObj.VideoCallURL = _APIModal[i].VideoCallURL;


                    }
                    catch (Exception ex)
                    {
                        continue;
                    }




                    _ListObj.Add(_TempModalObj);
                }
            }
            return _ListObj;
        }

        public bool CancelAppointmentApi_NewDammam (string BookingID ,out PostResponse ReturnObject)
		{
            
            HttpStatusCode status;
            string RegistrationUrl = "http://130.11.2.213:30005/cancel";
            BookingID _accData = new BookingID();
            _accData = new BookingID() { bookingID = BookingID.ToString()};
            var _NewData = RestUtility.CallAPI_POST<PostResponse>(RegistrationUrl, _accData, out status);

            ReturnObject = new PostResponse();
            ReturnObject = _NewData as PostResponse;
            if (status == HttpStatusCode.OK)
                return true;

            return false;

        }

        public bool SaveAppointmentApi_NewDammam(string doctorID , string clinicCode,string time,string patient_document_id, string MRN,out PostResponse ReturnObject)
        {
            HttpStatusCode status;
            string RegistrationUrl = "http://130.11.2.213:30005/book";
            BookAppointment _accData = new BookAppointment();
            _accData = new BookAppointment()
            { 
                doctorID = doctorID,
                clinicCode = clinicCode,
                patient_document_id = patient_document_id,
                time=time,
                fileNumber = MRN
            };
            var _NewData = RestUtility.CallAPI_POST<PostResponse>(RegistrationUrl, _accData, out status);
            
            ReturnObject = new PostResponse();
            ReturnObject = _NewData as PostResponse;
            if (status == HttpStatusCode.OK)
                return true;

            return false;

            //if (status == HttpStatusCode.OK)
            //    return true;
            
            //return false;

        }


        public bool ReschudleAppointmentApi_NewDammam(string bookingID, string time, out PostResponse ReturnObject)
        {
            HttpStatusCode status;
            string RegistrationUrl = "http://130.11.2.213:30005/reschedule";
            RescheduleAppoitment _accData = new RescheduleAppoitment();
            _accData = new RescheduleAppoitment()
            {
                bookingID = bookingID,
                time = time
            };
            var _NewData = RestUtility.CallAPI_POST<PostResponse>(RegistrationUrl, _accData, out status);

            ReturnObject = new PostResponse();
            ReturnObject = _NewData as PostResponse;
            if (status == HttpStatusCode.OK)
                return true;

            return false;

            //if (status == HttpStatusCode.OK)
            //    return true;

            //return false;

        }

        public bool PatientAddApi_NewDammam(RegisterPatient2 registerPatient , out PostResponse_AddPatient ReturnObject)
        {
            HttpStatusCode status;
            string RegistrationUrl = "http://130.11.2.213:30005/createPatient";
            CreatePatient _accData = new CreatePatient();

            //ReturnObject = new PostResponse();
            var NationalityCode = 0;
            GetNationalityCode(registerPatient.HospitaId, registerPatient.PatientNationalityId, ref NationalityCode);


            var TempPcell = registerPatient.PatientPhone.ToString();

            // change the the Format to
            TempPcell = TempPcell.Replace("%2B966", "");
            TempPcell = TempPcell.Replace("+", "");

            //TempPcell = TempPcell.Substring(0,3).Replace("966", "");                            
            if (TempPcell.Substring(0, 5) == "00966")
            {
                TempPcell = TempPcell.Substring(5, TempPcell.Length - 5);
            }
            if (TempPcell.Substring(0, 3) == "966")
            {
                TempPcell = TempPcell.Substring(3, TempPcell.Length - 3);
            }
            var FirstChar = TempPcell.Substring(0, 1);
            if (FirstChar != "0")
            {
                TempPcell = "0" + TempPcell;
            }


            string formatString = "yyyy-MM-dd";
            //string DOB = DateTime.ParseExact(, formatString, null).ToString();
            string DOB = registerPatient.PatientBirthday.ToString("yyyy-MM-dd");
            //_TempModalObj.appDate = DateTime.ParseExact(_APIModal[i].appDate, formatString, null).ToString(); ;


            var PGENDER = "M";
            if (registerPatient.PatientGender == 1)
                PGENDER = "F";
            _accData = new CreatePatient()
            {
                familyName = registerPatient.PatientLastName
                ,familyNameAr =""
                ,firstName = registerPatient.PatientFirstName
                ,firstNameAr =""
                ,gender = PGENDER
                ,nationality = NationalityCode.ToString ()
                ,patient_document_id = registerPatient.PatientNationalId .ToString()
                ,phone = TempPcell
                ,dateofbirth = DOB
                ,religion = "8"
                ,secondName = registerPatient.PatientMiddleName
                ,secondNameAr =""
                ,thirdName = ""
                ,thirdNameAr = ""                
            };
            var _NewData = RestUtility.CallAPI_POST<PostResponse_AddPatient>(RegistrationUrl, _accData, out status);

            ReturnObject = new PostResponse_AddPatient();
            ReturnObject = _NewData as PostResponse_AddPatient;

            if (status == HttpStatusCode.OK)
                return true;

            return false;

        }


        //public bool SaveAppointmentApi_NewDammam(string BookingID)
        //{
        //    HttpStatusCode status;
        //    string RegistrationUrl = "http://130.11.2.213:30005/cancel";
        //    BookingID _accData = new BookingID();
        //    _accData = new BookingID() { bookingID = BookingID.ToString() };
        //    var _NewData = RestUtility.CallAPI_POST<Patient_Appointment_Modal>(RegistrationUrl, _accData, out status);
        //    return true;

        //}


        // doctor Schule days
        //Doctor_Schedule_days_Modal
        public List<Doctor_Schedule_days_Modal_ForMobile> GetDoctorSchduleDaysByApi_NewDam(string lang, string DoctorID,string clinicCode , DateTime selectedDate)
        {
            DateTime now = DateTime.Now;
            if (selectedDate >= now)
                now = selectedDate;




            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var fromDate = now.ToString("yyyy-MM-dd") + " 00:00:00";
            var toDate = endDate.ToString("yyyy-MM-dd") + " 23:59:59";

            HttpStatusCode status;
            string APIUrl = "http://130.11.2.213:30005/getDoctorAvaliableDays?doctorID=" + DoctorID + "&clinicCode=" + clinicCode + "&from=" + fromDate + "&to=" + toDate;
            var _NewData = RestUtility.CallAPI<List<Doctor_Schedule_days_Modal>>(APIUrl, null) as List<Doctor_Schedule_days_Modal>;

            var _NewDataForMobile = MapDoctorSchduleDays_NewDamma(_NewData);
            return _NewDataForMobile;
        }

        private List<Doctor_Schedule_days_Modal_ForMobile> MapDoctorSchduleDays_NewDamma(List<Doctor_Schedule_days_Modal> _APIModal)
        {
            List<Doctor_Schedule_days_Modal_ForMobile> _ListObj = new List<Doctor_Schedule_days_Modal_ForMobile>();
            //Doctor_Schedule_days_Modal_ForMobile
            if (_APIModal != null && _APIModal.Count > 0)
            {
                for (var i = 0; i < _APIModal.Count; i++)
                {
                    Doctor_Schedule_days_Modal_ForMobile _TempModalObj = new Doctor_Schedule_days_Modal_ForMobile();
                    _TempModalObj.AvailableSlots = _APIModal[i].AvailableSlots;
                    _TempModalObj.DayBooked = _APIModal[i].DayBooked;
                    _TempModalObj.DepartmentID = _APIModal[i].DepartmentID;
                    _TempModalObj.Doctor_Id = _APIModal[i].Doctor_Id;
                    _TempModalObj.FromTime = _APIModal[i].FromTime;
                    _TempModalObj.Id = _APIModal[i].Id;
                    //_TempModalObj.PageNo = _APIModal[i].PageNo;
                    _TempModalObj.Scheduled_Day = _APIModal[i].Scheduled_Day;
                    _TempModalObj.Schedule_1_From = _APIModal[i].Schedule_1_From;
                    _TempModalObj.Schedule_1_To = _APIModal[i].Schedule_1_To;
                    _TempModalObj.Schedule_2_From= _APIModal[i].Schedule_2_From;
                    _TempModalObj.Schedule_2_To= _APIModal[i].Schedule_2_To;
                    _TempModalObj.SlotTypeId = _APIModal[i].SlotTypeId;
                    _TempModalObj.SlotTypeName = _APIModal[i].SlotTypeName;
                    //_TempModalObj.SNO = _APIModal[i].SNO;
                    //_TempModalObj.Split_Shift= _APIModal[i].Split_Shift;
                    _TempModalObj.TotalSlotCount = _APIModal[i].TotalSlotCount;
                    _TempModalObj.ToTime = _APIModal[i].ToTime;
                    _TempModalObj.NoSchedules = 0;
                    _ListObj.Add(_TempModalObj);
                }
            }
            return _ListObj;
        }

        //AvailableSlots
        // doctor Schule days
        //Doctor_Schedule_days_Modal
        public List<AvailableSlots> GetDoctorSlotsOfDaysByApi_NewDam(string lang, string DoctorID, string clinicCode , DateTime startDate)
        {
            DateTime now = DateTime.Now;
            var fromDate = "";
            var currenttime = DateTime.Now.ToString("HH:mm:ss");
            if (now.Date == startDate.Date)
			{
                fromDate = startDate.ToString("yyyy-MM-dd") + " " + currenttime;

            }
            else
			{
                fromDate = startDate.ToString("yyyy-MM-dd H:mm:ss");
            }


            
            var toDate = startDate.ToString("yyyy-MM-dd") + " 23:59:59";

            
            string APIUrl = "http://130.11.2.213:30005/availability?doctorID=" + DoctorID + "&clinicCode=" + clinicCode + "&from=" + fromDate + "&to=" + toDate;
            var _NewData = RestUtility.CallAPI<List<AvailableSlots>>(APIUrl, null) as List<AvailableSlots>;

            for (int i = 0; i< _NewData.Count; i++  )
			{
                var dt = DateTime.Parse(_NewData[i].time_from);
                _NewData[i].time_from = dt.ToString("HH:mm:ss");

                dt = DateTime.Parse(_NewData[i].time_to);
                _NewData[i].time_to = dt.ToString("HH:mm:ss");

            }





            return _NewData;
        }




        // Lab Results and Readio logi Results
        
        public List<PateintTests> GetPatientLabRadiologyByApi_NewDam(string lang, string MRN, ref int Er_Status, ref string Msg)
        {

            HttpStatusCode status;
            //patientData_Dam _userInfo = new patientData_Dam();

            string RegistrationUrl = "http://130.11.2.213:30005/getLabOrders?mrn=" + MRN;
            var _NewData = RestUtility.CallAPI_Perscription<List<Medical_Perscription_Dam>>(RegistrationUrl, null);
            var _patientLabData_Dam = _NewData as List<Medical_Perscription_Dam>;
            var _MapLabDATA = MapLabRadioINfoModel_NewDamma(_patientLabData_Dam, "LAB").OrderByDescending(o => o.report_date).ToList();

            RegistrationUrl = "http://130.11.2.213:30005/getRadiologyOrders?mrn=" + MRN;
            var _NewDataXray = RestUtility.CallAPI_Perscription<List<Medical_Perscription_Dam>>(RegistrationUrl, null);
            var _patientXRAYData_Dam = _NewDataXray as List<Medical_Perscription_Dam>;
            var _MapXRAYDATA = MapLabRadioINfoModel_NewDamma(_patientXRAYData_Dam, "XRay").OrderByDescending(o => o.report_date).ToList();

            var FinalData = new List<PateintTests>();
            FinalData.AddRange(_MapLabDATA);
            FinalData.AddRange(_MapXRAYDATA);

            return FinalData.OrderByDescending(o => o.report_date).ToList();

        }

        private List<PateintTests> MapLabRadioINfoModel_NewDamma(List<Medical_Perscription_Dam> _APIModal , string ReportType)
        {
            string formatString = "yyyyMMddHHmmss";
            List<PateintTests> _ListObj = new List<PateintTests>();

            if (_APIModal != null && _APIModal.Count > 0)
            {
                for (var i = 0; i < _APIModal.Count; i++)
                {
                    PateintTests _TempModalObj = new PateintTests();
                    _TempModalObj.ftp_path  = "https://cxmw.sghgroup.com.sa/TESTAPI/cs/index2.html"; // FOr Dammam Fixed
                    
                    var temOPID = "OP";
                    
                    if (_APIModal[i].opip == "I")
                        temOPID = "IP";
                    _TempModalObj.opip = temOPID;

                    _TempModalObj.registration_no = Convert.ToInt32(_APIModal[i].fileNumber) ;// FOr Dammam Fixed                    
                    _TempModalObj.report_filename = _APIModal[i].orders[0].dose + " " + _APIModal[i].orders[0].dispensingUnit;
                    
                    //_TempModalObj.report_id = 0; // Currently Fixed For Dammam
                    //_TempModalObj.test_id = 0; // Currently Fixed For Dammam
                    _TempModalObj.report_id = _APIModal[i].orders[0].orderLine; // Currently Fixed For Dammam
                    _TempModalObj.test_id = _APIModal[i].orders[0].orderLine; // Currently Fixed For Dammam


                    _TempModalObj.report_type = ReportType;
                    
                    _TempModalObj.test_name = _APIModal[i].orders[0].procedureName;
                    
                    DateTime dt = DateTime.ParseExact(_APIModal[i].orders[0].requestDate, formatString, null);
                    _TempModalObj.report_date = dt;
                    

                    _ListObj.Add(_TempModalObj);
                }
            }
            return _ListObj;
        }


        // For Family Listing

        public List<PatientFamilyList> GetPatientListsByApi_NewDam(string lang, string idValue, string IdType, ref int Er_Status, ref string Msg)
        {

            HttpStatusCode status;            
            accountData_dam _accData = new accountData_dam();
            string RegistrationUrl = "http://130.11.2.213:30005/getPatientInfo";

            _accData = new accountData_dam() { idValue = idValue, idType = IdType };

            var content = new[]{
                    new KeyValuePair<string, string>("idValue", idValue.ToString()),
                    new KeyValuePair<string, string>("idType", IdType)
            };


            RegistrationUrl = "http://130.11.2.213:30005/getPatientInfo?idValue=" + idValue.ToString() + "&idType=" + IdType.ToString();
            var _NewData = RestUtility.CallAPI<List<patientData_Dam>>(RegistrationUrl, content);


            var _patientData_Dam = new List<patientData_Dam>();
            _patientData_Dam = _NewData as List<patientData_Dam>;

            var _userInfo = MapPatientListToFamilyList(_patientData_Dam , lang);
            return _userInfo;

        }
        public List<PatientFamilyList> MapPatientListToFamilyList(List<patientData_Dam> _InputData, string lang = "EN")
        {
            var NewData = new List<PatientFamilyList>();

            if (_InputData != null && _InputData.Count > 0)
            {
                for (var i = 0; i < _InputData.Count; i++)
                {
                    if (_InputData[i].registration_no != "" && _InputData[i].registration_no != "0")
					{
                        PatientFamilyList _TempModalObj = new PatientFamilyList();
                        _TempModalObj.BranchId = "9"; // FOr Dammam Fixed
                        _TempModalObj.Age = _InputData[i].age;
                        if (lang == "AR" || lang == "ar")
                            _TempModalObj.BranchName = " المستشفى السعودي الألماني الدمام";
                        else
                            _TempModalObj.BranchName = "SGH Dammam";
                        _TempModalObj.DOB = _InputData[i].birthday;
                        _TempModalObj.FamilyName = _InputData[i].family_name;
                        _TempModalObj.FirstName = _InputData[i].first_name;
                        _TempModalObj.Gender = _InputData[i].gender;
                        _TempModalObj.image_url = "";
                        _TempModalObj.LastName = _InputData[i].last_name;
                        _TempModalObj.MaritalStatus = "";
                        _TempModalObj.MiddleName = _InputData[i].middle_name;
                        _TempModalObj.Nationality = _InputData[i].nationality;
						try
						{
                            _TempModalObj.NationalityId = Convert.ToInt32(_InputData[i].nationalityId);
                        }
                        catch(Exception e)
						{

						}
                        

                        _TempModalObj.PatientFullName = _InputData[i].name;
                        _TempModalObj.PatientId = _InputData[i].national_id;
                        _TempModalObj.PCellno = _InputData[i].phone;
                        _TempModalObj.RegistrationNo = _InputData[i].registration_no;

                        NewData.Add(_TempModalObj);
                    }
                    
                }
            }
            return NewData;
        }


        public List<PatientVisit> GetPatientVisitByApi_NewDam(string lang, string MRN, ref int Er_Status, ref string Msg)
        {

            HttpStatusCode status;
            //patientData_Dam _userInfo = new patientData_Dam();

            string RegistrationUrl = "http://130.11.2.213:30005/patientVisits?mrn=" + MRN;
            var _NewData = RestUtility.CallAPI<List<PatientVisit_Dam>>(RegistrationUrl, null);
            var _patientData_Dam = _NewData as List<PatientVisit_Dam>;
            var _MapDATA = MapPatientVisitModel_NewDamma(_patientData_Dam).OrderByDescending(o => o.appDate).ToList();

            return _MapDATA.OrderByDescending(o => o.appDate).ToList();

        }

        private List<PatientVisit> MapPatientVisitModel_NewDamma(List<PatientVisit_Dam> _APIModal)
        {
            string formatString = "yyyy-MM-dd";
            List<PatientVisit> _ListObj = new List<PatientVisit>();

            if (_APIModal != null && _APIModal.Count > 0)
            {
                for (var i = 0; i < _APIModal.Count; i++)
                {
                    PatientVisit _TempModalObj = new PatientVisit();
                    //_TempModalObj.appDate = DateTime.ParseExact(_APIModal[i].appDate, formatString, null).ToString(); ;
                    //_TempModalObj.appointmentType = _APIModal[i].appointmentType;
                    _TempModalObj.id =  Convert.ToInt32( _APIModal[i].id);
                    //_TempModalObj.appointmentNo = Convert.ToInt32( _APIModal[i].appointmentNo);
                    _TempModalObj.appointmentNo = 0;
                    _TempModalObj.registrationNo = Convert.ToInt32(_APIModal[i].registrationNo);
                    _TempModalObj.clinicName = _APIModal[i].clinicName;
                    _TempModalObj.appDate = _APIModal[i].appDate;
                    _TempModalObj.appTime = _APIModal[i].appTime;
                    _TempModalObj.patientVisited = 1;
                    _TempModalObj.patientName = _APIModal[i].patientName;
                    _TempModalObj.doctorName = _APIModal[i].doctorName;
                    _TempModalObj.videoCallUrl = null;
                    _TempModalObj.doctorId = null;                    
                    _TempModalObj.appointmentType = null;
                    _TempModalObj.paid = 1;

                    _TempModalObj.episodeId = Convert.ToInt32(_APIModal[i].id);
                    if (_APIModal[i].appointmentType == "Out-Patient")
                        _TempModalObj.episodeType = "OP";
                    else
                        _TempModalObj.episodeType = "IP";                    
                    
                    _TempModalObj.episodeStatus = "Visited";
                    _ListObj.Add(_TempModalObj);
                }
            }
            return _ListObj;
        }



        public List<PatientInsurance> GetPatientInsuranceByApi_NewDam(string lang, string MRN, ref int Er_Status, ref string Msg)
        {

            HttpStatusCode status;
            //patientData_Dam _userInfo = new patientData_Dam();

            string RegistrationUrl = "http://130.11.2.213:30005/patientInsurance?mrn=" + MRN;
            var _NewData = RestUtility.CallAPI<List<PatientInsurance_Dam>>(RegistrationUrl, null);
            var _patientData_Dam = _NewData as List<PatientInsurance_Dam>;
            var _MapDATA = MapPatientInsuranceModel_NewDamma(_patientData_Dam , MRN);

            return _MapDATA;

        }

        private List<PatientInsurance> MapPatientInsuranceModel_NewDamma(List<PatientInsurance_Dam> _APIModal , string MRN)
        {
            //string formatString = "yyyy-MM-dd";
            List<PatientInsurance> _ListObj = new List<PatientInsurance>();

            if (_APIModal != null && _APIModal.Count > 0)
            {
                for (var i = 0; i < _APIModal.Count; i++)
                {
                    PatientInsurance _TempModalObj = new PatientInsurance();
                    _TempModalObj.category = _APIModal[i].purchaserDesc;
                    _TempModalObj.company = _APIModal[i].policyDesc;
                    _TempModalObj.grade = _APIModal[i].contractDesc;
                    _TempModalObj.idExpiryDate = _APIModal[i].idExpiryDate;
                    _TempModalObj.insuranceId = _APIModal[i].insuranceId;
                    _TempModalObj.policyNo = _APIModal[i].policyNo;
                    _TempModalObj.registrationno = Convert.ToInt32(MRN);

                    //_TempModalObj.id = Convert.ToInt32(_APIModal[i].id);
                    ////_TempModalObj.appointmentNo = Convert.ToInt32( _APIModal[i].appointmentNo);
                    //_TempModalObj.appointmentNo = 0;
                    //_TempModalObj.registrationNo = Convert.ToInt32(_APIModal[i].registrationNo);
                    //_TempModalObj.clinicName = _APIModal[i].clinicName;
                    //_TempModalObj.appDate = _APIModal[i].appDate;
                    //_TempModalObj.appTime = _APIModal[i].appTime;
                    //_TempModalObj.patientVisited = 1;
                    //_TempModalObj.patientName = _APIModal[i].patientName;
                    //_TempModalObj.doctorName = _APIModal[i].doctorName;
                    //_TempModalObj.videoCallUrl = null;
                    //_TempModalObj.doctorId = null;
                    //_TempModalObj.appointmentType = null;
                    //_TempModalObj.paid = 1;

                    //_TempModalObj.episodeId = Convert.ToInt32(_APIModal[i].id);
                    //if (_APIModal[i].appointmentType == "Out-Patient")
                    //    _TempModalObj.episodeType = "OP";
                    //else
                    //    _TempModalObj.episodeType = "IP";

                    //_TempModalObj.episodeStatus = "Visited";


                    _ListObj.Add(_TempModalObj);
                }
            }
            return _ListObj;
        }



    }
}