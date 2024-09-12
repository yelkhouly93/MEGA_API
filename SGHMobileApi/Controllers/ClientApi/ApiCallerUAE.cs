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
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace SmartBookingService.Controllers.ClientApi
{
    public class ApiCallerUAE
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");
        private readonly EncryptDecrypt_New util = new EncryptDecrypt_New();

        public List<login_check_modal> ValidateLoginUserByApi_NewUAE(string lang, string idValue, string IdType, int BranchID, ref int Er_Status, ref string Msg)
        {

   //         try
			//{
                HttpStatusCode status;
                //accountData_dam _accData = new accountData_dam();
                string RegistrationUrl = "https://app.saudigerman.com/Services/api/GetPatient";

                //// AHSAN TESTING FOR SMS
                //RestUtility.Send_UAE_SMS();


                var BranchName = "";
                BranchName = GetBranchName(BranchID);

                var _patientData_UAE = new List<UAE_Patient_Info>();


                //_accData = new accountData_dam() { idValue = idValue, idType = IdType ,  };
                if (!string.IsNullOrEmpty(BranchName.ToString()))
                {

                    var content = new[]{
                    new KeyValuePair<string, string>("Id", idValue.ToString()),
                    new KeyValuePair<string, string>("idType", IdType),
                    new KeyValuePair<string, string>("Facility", BranchName)
                };

                    var Parameters = "?Id=" + idValue.ToString();
                    Parameters += "&IdType=" + IdType;
                    Parameters += "&Facility=" + BranchName;

                    RegistrationUrl = "https://app.saudigerman.com/Services/api/patient-search-get" + Parameters;

                    var resp = new GenericResponse();

                    var _NewData = RestUtility.CallAPI_POST_UAE<List<UAE_Patient_Info>>(RegistrationUrl, content, out resp, true);

                    _patientData_UAE = _NewData as List<UAE_Patient_Info>;

                    Msg = resp.msg;

                }
                else
                {
                    var content = new[]{
                    new KeyValuePair<string, string>("Id", idValue.ToString()),
                    new KeyValuePair<string, string>("idType", IdType),
                };

                    var Parameters = "?Id=" + idValue.ToString();
                    Parameters += "&IdType=" + IdType;

                    RegistrationUrl = "https://app.saudigerman.com/Services/api/patient-search-get" + Parameters;

                    var resp = new GenericResponse();

                    var _NewData = RestUtility.CallAPI_POST_UAE<List<UAE_Patient_Info>>(RegistrationUrl, content, out resp, true);
                    Msg = resp.msg;
                    _patientData_UAE = _NewData as List<UAE_Patient_Info>;

                }

                var _userInfo = MapUserInfoModelToUserInf_NewUAE(_patientData_UAE, BranchID);

                return _userInfo;
            //}
   //         catch   (Exception ex)
			//{

   //             Er_Status = 0;
   //             Msg = ConfigurationManager.AppSettings["UAE_ERROR_MSG"].ToString() + " " + ex.Message;
   //             //Msg = "UAE site Not Working. Please try again Later.";
   //             return null;
			//}
            
        }

        private List<login_check_modal> MapUserInfoModelToUserInf_NewUAE(List<UAE_Patient_Info> _userInfoModel , int BranchID)
        {
            List<login_check_modal> _userInfo = new List<login_check_modal>();

            var BranchName = GetBranchFullName(BranchID);

            if (_userInfoModel != null && _userInfoModel.Count > 0)
            {
                for (var i = 0; i < _userInfoModel.Count; i++)
                {
                    login_check_modal _TempModalObj = new login_check_modal();

                    if (_userInfoModel[i].registration_no.ToString() != "" && _userInfoModel[i].registration_no.ToString() != "0")
                    {
                        var NewBranchID = BranchID.ToString();
                        if (BranchID == 0)
						{
                            var Hospital_ID = GetBranchID(_userInfoModel[i].hospital_id);
                            NewBranchID = Hospital_ID;
                            BranchName = GetBranchFullName(Convert.ToInt32( Hospital_ID));
                        }
                            

                        _TempModalObj.BranchId = NewBranchID.ToString();
                        _TempModalObj.branchID = NewBranchID.ToString();
                        _TempModalObj.Branch_AR = BranchName;
                        _TempModalObj.Branch_EN = BranchName;
                        _TempModalObj.Registrationno = _userInfoModel[i].registration_no.ToString();
                        //_TempModalObj.DOB = _userInfoModel[i].birthday.ToString();
                        _TempModalObj.DOB = _userInfoModel[i].birthday;
                        _TempModalObj.image_url = "";
                        _TempModalObj.PatientCellNo = _userInfoModel[i].phone.ToString();
                        _TempModalObj.PatientCellNo2 = _userInfoModel[i].phone.ToString();
                        _TempModalObj.PatientFullName = MaskFullName (_userInfoModel[i].name.ToString());
                        _TempModalObj.PatientId = _userInfoModel[i].registration_no.ToString();
                        _TempModalObj.PatientName_AR =  _userInfoModel[i].name_ar.ToString();
                        _TempModalObj.PatientName_EN = MaskFullName(_userInfoModel[i].name.ToString());
                        //_TempModalObj.PEMail = _userInfoModel[i].email.ToString();
                        _TempModalObj.PEMail = "";
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



        public UserInfo_New GetPatientDataByApi_NewUAE(string lang, string idValue, string IdType,int BranchID, ref int Er_Status, ref string Msg)
        {
            HttpStatusCode status;
            accountData_dam _accData = new accountData_dam();
            string RegistrationUrl = "https://app.saudigerman.com/Services/api/GetPatient";

            var BranchName = "";
            BranchName = GetBranchName(BranchID);

            var content = new[]{
                    new KeyValuePair<string, string>("Id", idValue.ToString()),
                    new KeyValuePair<string, string>("idType", IdType),
                    new KeyValuePair<string, string>("Facility", BranchName)
            };

            var Parameters = "?Id=" + idValue.ToString();
            Parameters += "&IdType=" + IdType;
            Parameters += "&Facility=" + BranchName;

            RegistrationUrl = "https://app.saudigerman.com/Services/api/patient-search-get" + Parameters;
            
            var resp = new GenericResponse();
            var _NewData = RestUtility.CallAPI_POST_UAE<List<UAE_Patient_Info>>(RegistrationUrl, content, out resp, true);

            var _patientData_UAE = new List<UAE_Patient_Info>();
            _patientData_UAE = _NewData as List<UAE_Patient_Info>;

            var _userInfo = MapUserInfoModelToUserInf_NewUAE2(_patientData_UAE, BranchID);

            
            return _userInfo;

        }

        private UserInfo_New MapUserInfoModelToUserInf_NewUAE2(List<UAE_Patient_Info> userInfoModel , int BranchID)
        {
            var BranchFullName = GetBranchFullName(BranchID);

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

            //if (BranchID > 300 && BranchID < 400)
                userInfo.CountryId = 3;

            userInfo.email = userInfoModel[0].email;
            userInfo.family_name = userInfoModel[0].family_name;
            userInfo.first_name = userInfoModel[0].first_name;
            userInfo.gender = userInfoModel[0].gender;
            userInfo.gender_ar = userInfoModel[0].gender;
            userInfo.hospital_id = BranchID.ToString();
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
            userInfo.FamilyMembersCount = Convert.ToInt32( userInfoModel[0].familymembersCount);


            userInfo.Age = userInfoModel[0].age;
            userInfo.BloodGroup = userInfoModel[0].bloodgroup;
            userInfo.Weight = userInfoModel[0].weight;
            userInfo.Height = userInfoModel[0].height;


            userInfo.NationalityId = userInfoModel[0].nationalityId;
            userInfo.Nationality = userInfoModel[0].nationality;

            if (userInfo.NationalityId == "")
                userInfo.NationalityId = "0";


            userInfo.image_url = "";
            
            userInfo.IsCash = true;

            userInfo.Branch_Id = BranchID.ToString();
            userInfo.Branch_Name = BranchFullName;

            userInfo.Branch_Name_ar = BranchFullName;

            userInfo.CurrentCity = userInfoModel[0].CurrentCity;
            userInfo.IdType = userInfoModel[0].IdType;
            userInfo.IdExpiry = userInfoModel[0].IdExpiry;        

            return userInfo;
        }


        // Perscription
        public List<Medical_Perscription_modal> GetPatientPerscriptionByApi_NewDam(string lang, string MRN, ref int Er_Status, ref string Msg)
        {

            HttpStatusCode status;
            //patientData_Dam _userInfo = new patientData_Dam();

            string RegistrationUrl = "http://130.11.2.213:30005/getPharmacyOrders?mrn=" + MRN;
            var _NewData = RestUtility.CallAPI_Perscription<List<Medical_Perscription_Dam>>(RegistrationUrl, null);


            //var _patientPerscriptionData_Dam = new List<Medical_Perscription_Dam>();
            var _patientPerscriptionData_Dam = _NewData as List<Medical_Perscription_Dam>;

            var _MapDATA = MapPerscriptionINfoModel_NewDamma(_patientPerscriptionData_Dam).OrderByDescending(o => o.Prescription_Date).ToList();

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
                    _TempModalObj.PrescriptionId = Convert.ToInt32(_APIModal[i].prescriptionNo);
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

        public void GenerateOTP_V3(string hospitalID, string MOBILE_NO, string MRN, string NationalId, string Source, ref int activationNo, ref int Er_Status, ref string Msg)
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
                new SqlParameter("@Msg", SqlDbType.NVarChar, 500)
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

        public string GetNationalityCodeUAE_string(int hospitalID, int UnifiedID)
        {
            //DB.param = new SqlParameter[]
            //    {
            //        new SqlParameter("@BranchId", hospitalID),
            //        new SqlParameter("@UnifiedID", UnifiedID),
            //        new SqlParameter("@NationalityCode", SqlDbType.Int)
            //    };
            //DB.param[2].Direction = ParameterDirection.Output;
            //DB.ExecuteSPAndReturnDataTable("dbo.Get_NationalityCode_FromUnifiedId_UAE_SP");

            //NationalityID = DB.param[2].Value.ToString();
            if (hospitalID == 0)
                return "";

            CustomDBHelper _DB = new CustomDBHelper("RECEPTION");

            var SQL_Qry = "SELECT TOP 1 CONVERT (varchar(5), RIGHT(1000 + HIS_ID, 3)) FROM BAS_Nationality_TB with (nolock) WHERE Branch_Id = '"+ hospitalID .ToString()+ "' AND UnifiedID = '"+ UnifiedID.ToString() + "'  ";
            var EmployeeCode = _DB.ExecuteSQLScalar(SQL_Qry);
            return EmployeeCode;

        }

        // Appointment
        public List<Patient_Appointment_Modal_ForMobile> GetPatientAppointmentsByApi_NewUAE(string lang, string MRN , int BranchID)
        {
            //HttpStatusCode status;
            //string RegistrationUrl = "http://130.11.2.213:30005/appointmentsHistory?mrn=" + MRN;
            //var _NewDataAPI = RestUtility.CallAPI<List<Patient_Appointment_Modal>>(RegistrationUrl, null) as List<Patient_Appointment_Modal>;

            HttpStatusCode status;
            string BaseAPIUrl = "https://app.saudigerman.com/Services/api/";

            var BranchName = "";
            BranchName = GetBranchName(BranchID);

            


            var content = new[]{
					new KeyValuePair<string, string>("hospital_id", BranchName),
					new KeyValuePair<string, string>("patient_reg_no", MRN),
					new KeyValuePair<string, string>("Lang", lang)
			};
			var Parameters = "?hospital_id=" + BranchName.ToString();
            Parameters += "&patient_reg_no=" + MRN;
            Parameters += "&Lang=" + lang.ToUpper();

            BaseAPIUrl = "https://app.saudigerman.com/Services/api/patient-appointments-list" + Parameters;
            var resp = new GenericResponse();
            var _NewDataAPI = RestUtility.CallAPI_POST_UAE<List<UAE_Patient_Appoitment_List>>(BaseAPIUrl, content, out resp , true) as List<UAE_Patient_Appoitment_List>;

            //var _Data_UAE = new List<UAE_Patient_Appoitment_List>();
            //_Data_UAE = _NewDataAPI as List<UAE_Patient_Appoitment_List>;

            var _NewData = MapPatientAppointments_NewUAE(_NewDataAPI, BranchID).OrderByDescending(o => o.IsUpComming).ThenBy(o => o.DurationLeft).ThenByDescending(o => o.AppDate).ToList();

            return _NewData;
        }


        private List<Patient_Appointment_Modal_ForMobile> MapPatientAppointments_NewUAE(List<UAE_Patient_Appoitment_List> _APIModal,int BranchID, string lang = "EN")
        {
            //var BranchName = "";
            //BranchName = GetBranchFullName(BranchID);
            
            var BranchINFO = GetBranchInfoName(BranchID , lang);

            List<Patient_Appointment_Modal_ForMobile> _ListObj = new List<Patient_Appointment_Modal_ForMobile>();
            //Doctor_Schedule_days_Modal_ForMobile
            if (_APIModal != null && _APIModal.Count > 0)
            {
                for (var i = 0; i < _APIModal.Count; i++)
                {
                    Patient_Appointment_Modal_ForMobile _TempModalObj = new Patient_Appointment_Modal_ForMobile();
                    var testint = 0;

                    


                    try
                    {

                        var tempStrDoctorID = _APIModal[i].doctorId;
                        string justNumbers = new String(tempStrDoctorID.Where(Char.IsDigit).ToArray());

                        _TempModalObj.DoctorId = Convert.ToInt32(justNumbers);


                        _TempModalObj.Id = _APIModal[i].id;
                        _TempModalObj.AppDate = _APIModal[i].appDate;
                        _TempModalObj.AppointmentNo = Convert.ToInt32(_APIModal[i].appointmentNo);
                        _TempModalObj.AppTime = _APIModal[i].appTime + ":00";
                        if (lang == "AR" || lang == "ar")
                            _TempModalObj.BranchName = BranchINFO.BranchName;
                        else
                            _TempModalObj.BranchName = BranchINFO.BranchName;

                        _TempModalObj.ClinicName = _APIModal[i].clinicName;                        
                        _TempModalObj.ClinicName_AR = _APIModal[i].clinicName;                        
                        _TempModalObj.DoctorName = _APIModal[i].doctorName;                        
                        _TempModalObj.DoctorName_AR = _APIModal[i].doctorName;
                        if (_APIModal[i].appDate <= System.DateTime.Now.Date)
                        {
                            _TempModalObj.DurationLeft = 0;
                        }
                        else
                        {
                            int dateDifference = (_APIModal[i].appDate- System.DateTime.Now.Date).Days;
                            _TempModalObj.DurationLeft = dateDifference;
                        }
                        _TempModalObj.Id = Convert.ToInt32(_APIModal[i].appointmentNo);
                        _TempModalObj.isPaid = false;
                        _TempModalObj.IsUpComming = Convert.ToInt32(_APIModal[i].isUpComming);
                        _TempModalObj.PatientName = _APIModal[i].patientName;
                        _TempModalObj.PatientVisited = Convert.ToInt32(_APIModal[i].patientVisited);
                        _TempModalObj.RegistrationNo = Convert.ToInt32(_APIModal[i].registrationNo);


                        // AHSAN NEW WORK
                        _TempModalObj.latitude = BranchINFO.latitude;
                        _TempModalObj.longitude = BranchINFO.longitude;




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

        //public bool CancelAppointmentApi_NewDammam(string BookingID, out PostResponse ReturnObject)
        //{

        //    HttpStatusCode status;
        //    string RegistrationUrl = "http://130.11.2.213:30005/cancel";
        //    BookingID _accData = new BookingID();
        //    _accData = new BookingID() { bookingID = BookingID.ToString() };
        //    var resp = new GenericResponse();
        //    var _NewData = RestUtility.CallAPI_POST_UAE<PostResponse>(RegistrationUrl, _accData, out resp);

        //    ReturnObject = new PostResponse();
        //    ReturnObject = _NewData as PostResponse;
        //    if (status == HttpStatusCode.OK)
        //        return true;

        //    return false;

        //}

        public bool SaveAppointmentApi_NewUAE(UAE_BookAppointment bookModel, out AppointmentPostResponse responseOut)
        {
            HttpStatusCode status;
            string BaseAPIUrl = "";
            var BranchName = GetBranchName(bookModel.BranchID);
            var UAEDoctorEmployeeCode = GetDoctorUAEEmployeeCode(bookModel.BranchID, bookModel.PhysicanId.ToString());

            var errStatus = 0;
            var errMessage = "";
            var IdType = "MRN";
            var IdValue = bookModel.PatientId;            
            var userInfo = GetPatientDataByApi_NewUAE("EN", IdValue, IdType, bookModel.BranchID, ref errStatus, ref errMessage);

            var content = new[]{
					new KeyValuePair<string, string>("Facility", BranchName),
					new KeyValuePair<string, string>("AppDate", bookModel.AppDate.ToString("yyyy-MM-dd")),
					new KeyValuePair<string, string>("StartTime", bookModel.StartTime),
					new KeyValuePair<string, string>("EndTime", bookModel.EndTime),
					new KeyValuePair<string, string>("DocId", UAEDoctorEmployeeCode),
					new KeyValuePair<string, string>("PatientId", bookModel.PatientId),
					new KeyValuePair<string, string>("PatientName", userInfo.name),
					new KeyValuePair<string, string>("CreatedBy", bookModel.CreatedBy),
					new KeyValuePair<string, string>("TransType", bookModel.TransType),
					new KeyValuePair<string, string>("IsStandby", bookModel.IsStandby),
					new KeyValuePair<string, string>("Remarks", bookModel.Remarks),
                    new KeyValuePair<string, string>("MobileNumber", userInfo.phone)

            };
			var Parameters = "?Facility=" + BranchName.ToString();
            Parameters += "&AppDate=" + bookModel.AppDate.ToString("yyyy-MM-dd");
            Parameters += "&StartTime=" + bookModel.StartTime;
            Parameters += "&EndTime=" + bookModel.EndTime;
            Parameters += "&DocId=" + UAEDoctorEmployeeCode;
            Parameters += "&PatientId=" + bookModel.PatientId;
            Parameters += "&PatientName=" + userInfo.name;
            Parameters += "&CreatedBy=" + bookModel.CreatedBy;
            Parameters += "&TransType=" + bookModel.TransType;
            Parameters += "&IsStandby=" + bookModel.IsStandby;
            Parameters += "&Remarks=" + bookModel.Remarks;
            Parameters += "&MobileNumber=" + userInfo.phone;

            //AppointmentPostResponse
            BaseAPIUrl = "https://app.saudigerman.com//Services/api/SlotTransactions" + Parameters;
            var resp = new AppointmentPostResponse();
            var _NewData = RestUtility.CallAPI_POST_UAE_Appointment<AppointmentPostResponse>(BaseAPIUrl, content, out resp, true);

            responseOut = resp;

            if (resp.Error.ToUpper() == "FALSE")
                return true;

            return false;

            //if (status == HttpStatusCode.OK)
            //    return true;

            //return false;

        }


        public bool CancelAppointmentApi_NewUAE(UAE_BookAppointment bookModel , out AppointmentPostResponse responseOut)
        {
            HttpStatusCode status;
            string BaseAPIUrl = "";
            var BranchName = GetBranchName(bookModel.BranchID);
            var UAEDoctorEmployeeCode = GetDoctorUAEEmployeeCode(bookModel.BranchID, bookModel.PhysicanId.ToString());

            var errStatus = 0;
            var errMessage = "";
            var IdType = "MRN";
            var IdValue = bookModel.PatientId;
            var userInfo = GetPatientDataByApi_NewUAE("EN", IdValue, IdType, bookModel.BranchID, ref errStatus, ref errMessage);



            var content = new[]{
					new KeyValuePair<string, string>("Facility", BranchName),
                    new KeyValuePair<string, string>("AppDate", bookModel.AppDate.ToString("yyyy-MM-dd")),
                    new KeyValuePair<string, string>("StartTime", bookModel.StartTime),
                    new KeyValuePair<string, string>("EndTime", bookModel.EndTime),
                    new KeyValuePair<string, string>("DocId", UAEDoctorEmployeeCode),
                    new KeyValuePair<string, string>("PatientId", bookModel.PatientId),
                    new KeyValuePair<string, string>("PatientName", userInfo.name),
                    new KeyValuePair<string, string>("CreatedBy", bookModel.CreatedBy),
                    new KeyValuePair<string, string>("TransType", "Cancel"),
					new KeyValuePair<string, string>("IsStandby", bookModel.IsStandby),
					new KeyValuePair<string, string>("Remarks", "M"),
					new KeyValuePair<string, string>("AppId", bookModel.AppId),
                    new KeyValuePair<string, string>("MobileNumber", userInfo.phone),
                    new KeyValuePair<string, string>("CancelReason", "Mobile"),
                    
            };
			//AppointmentPostResponse
			var Parameters = "?Facility=" + BranchName.ToString();
            Parameters += "&AppDate=" + bookModel.AppDate.ToString("yyyy-MM-dd");
            Parameters += "&StartTime=" + bookModel.StartTime;
            Parameters += "&EndTime=" + bookModel.EndTime;
            Parameters += "&DocId=" + UAEDoctorEmployeeCode;
            Parameters += "&PatientId=" + bookModel.PatientId;
            Parameters += "&PatientName=" + userInfo.name;
            Parameters += "&CreatedBy=" + bookModel.CreatedBy;
            Parameters += "&TransType=" + "Cancel";
            Parameters += "&IsStandby=" + bookModel.IsStandby;
            Parameters += "&Remarks=" + "M";
            Parameters += "&AppId=" + bookModel.AppId;
            Parameters += "&MobileNumber=" + userInfo.phone;
            Parameters += "&CancelReason=" + "Mobile";

            //AppointmentPostResponse
            BaseAPIUrl = "https://app.saudigerman.com/Services/api/SlotTransactions" + Parameters;
            var resp = new AppointmentPostResponse();
            var _NewData = RestUtility.CallAPI_POST_UAE_Appointment<AppointmentPostResponse>(BaseAPIUrl, content, out resp, true);


            responseOut = resp;

            if (resp.Error.ToUpper() == "FALSE" && resp.AppId != null)
                return true;

            return false;

            //if (status == HttpStatusCode.OK)
            //    return true;

            //return false;

        }

        public bool ReschduleAppointmentApi_NewUAE(UAE_BookAppointment bookModel, out AppointmentPostResponse responseOut)
        {
            HttpStatusCode status;
            string BaseAPIUrl = "";
            var BranchName = GetBranchName(bookModel.BranchID);
            var UAEDoctorEmployeeCode = GetDoctorUAEEmployeeCode(bookModel.BranchID, bookModel.PhysicanId.ToString());

            var errStatus = 0;
            var errMessage = "";
            var IdType = "MRN";
            var IdValue = bookModel.PatientId;
            var userInfo = GetPatientDataByApi_NewUAE("EN", IdValue, IdType, bookModel.BranchID, ref errStatus, ref errMessage);




            var content = new[]{
                    new KeyValuePair<string, string>("Facility", BranchName),
                    new KeyValuePair<string, string>("AppDate", bookModel.AppDate.ToString("dd-MM-yyyy")),
                    new KeyValuePair<string, string>("StartTime", bookModel.StartTime),
                    new KeyValuePair<string, string>("EndTime", bookModel.EndTime),
                    new KeyValuePair<string, string>("DocId", UAEDoctorEmployeeCode),
                    new KeyValuePair<string, string>("PatientId", bookModel.PatientId),
                    new KeyValuePair<string, string>("PatientName", userInfo.name),
                    new KeyValuePair<string, string>("CreatedBy", bookModel.CreatedBy),
                    new KeyValuePair<string, string>("TransType", "ReSchedule"),
                    new KeyValuePair<string, string>("IsStandby", bookModel.IsStandby),
                    new KeyValuePair<string, string>("Remarks", "M"),
                    new KeyValuePair<string, string>("AppId", bookModel.AppId),
                    new KeyValuePair<string, string>("MobileNumber", userInfo.phone)
            };

            //AppointmentPostResponse
            var Parameters = "?Facility=" + BranchName.ToString();
            Parameters += "&AppDate=" + bookModel.AppDate.ToString("yyyy-MM-dd");
            Parameters += "&StartTime=" + bookModel.StartTime;
            Parameters += "&EndTime=" + bookModel.EndTime;
            Parameters += "&DocId=" + UAEDoctorEmployeeCode;
            Parameters += "&PatientId=" + bookModel.PatientId;
            Parameters += "&PatientName=" + userInfo.name;
            Parameters += "&CreatedBy=" + bookModel.CreatedBy;
            Parameters += "&TransType=" + "ReSchedule";
            Parameters += "&IsStandby=" + bookModel.IsStandby;
            Parameters += "&Remarks=" + "M";
            Parameters += "&AppId=" + bookModel.AppId;
            Parameters += "&MobileNumber=" + userInfo.phone;

            //AppointmentPostResponse
            BaseAPIUrl = "https://app.saudigerman.com/Services/api/SlotTransactions" + Parameters;

            var resp = new AppointmentPostResponse();

            var _NewData = RestUtility.CallAPI_POST_UAE_Appointment<AppointmentPostResponse>(BaseAPIUrl, content, out resp, true);

            responseOut = resp;

            if (resp.Error.ToUpper() == "FALSE")
                return true;

            return false;

            //if (status == HttpStatusCode.OK)
            //    return true;

            //return false;

        }


        //public bool ReschudleAppointmentApi_NewDammam(string bookingID, string time, out PostResponse ReturnObject)
        //{
        //    HttpStatusCode status;
        //    string RegistrationUrl = "http://130.11.2.213:30005/reschedule";
        //    RescheduleAppoitment _accData = new RescheduleAppoitment();
        //    _accData = new RescheduleAppoitment()
        //    {
        //        bookingID = bookingID,
        //        time = time
        //    };
        //    var _NewData = RestUtility.CallAPI_POST_UAE<PostResponse>(RegistrationUrl, _accData, out status);

        //    ReturnObject = new PostResponse();
        //    ReturnObject = _NewData as PostResponse;
        //    if (status == HttpStatusCode.OK)
        //        return true;

        //    return false;

        //    //if (status == HttpStatusCode.OK)
        //    //    return true;

        //    //return false;

        //}

        public bool PatientAddApi_NewUAE(RegisterPatientUAE registerPatientUAE, out RegistrationPostResponse ReturnObject)
        {
            HttpStatusCode status;

            //ReturnObject = new PostResponse();
             
            var NationalityCode = GetNationalityCodeUAE_string(registerPatientUAE.HospitaId, registerPatientUAE.PatientNationalityId);

            if (NationalityCode == "")
                NationalityCode = registerPatientUAE.PatientNationalityId.ToString();


            var TempPcell = registerPatientUAE.PatientPhone.ToString();

            // change the the Format to
            TempPcell = TempPcell.Replace("%2B971", "");
            TempPcell = TempPcell.Replace("+", "");

            //TempPcell = TempPcell.Substring(0,3).Replace("966", "");                            
            if (TempPcell.Substring(0, 5) == "00971")
            {
                TempPcell = TempPcell.Substring(5, TempPcell.Length - 5);
            }
            if (TempPcell.Substring(0, 3) == "971")
            {
                TempPcell = TempPcell.Substring(3, TempPcell.Length - 3);
            }
            var FirstChar = TempPcell.Substring(0, 1);
            //if (FirstChar != "0")
            //{
            //    TempPcell = "0" + TempPcell;
            //}
            string DOB = registerPatientUAE.PatientBirthday.ToString("yyyy-MM-dd");

            string IDExpirydate  = registerPatientUAE.IdExpiry.ToString("yyyy-MM-dd");

            var PGENDER = 1;
            if (registerPatientUAE.PatientGender == 1)
                PGENDER = 2;
            
            string BaseAPIUrl = "";
            var BranchName = GetBranchName(registerPatientUAE.HospitaId);

			var content = new[]{
					new KeyValuePair<string, string>("FirstName", BranchName),
					new KeyValuePair<string, string>("LastName", registerPatientUAE.PatientLastName),
					new KeyValuePair<string, string>("Gender", PGENDER.ToString()),
					new KeyValuePair<string, string>("BirthDate", DOB),
					new KeyValuePair<string, string>("CurrentCity", registerPatientUAE.CurrentCity),
					new KeyValuePair<string, string>("Address", registerPatientUAE.PatientAddress),
					new KeyValuePair<string, string>("NationalityId", NationalityCode.ToString()),
					new KeyValuePair<string, string>("IdType", registerPatientUAE.IdType.ToString()),
					new KeyValuePair<string, string>("IdNumber", registerPatientUAE.PatientNationalId.ToString()),
					new KeyValuePair<string, string>("IdExpiry", IDExpirydate),
					new KeyValuePair<string, string>("Mobile", TempPcell),
					new KeyValuePair<string, string>("FacilityId", BranchName)
			};

			var Parameters = "?FacilityId=" + BranchName.ToString();
            Parameters += "&Mobile=" + TempPcell;
            Parameters += "&IdExpiry=" + IDExpirydate;
            Parameters += "&IdNumber=" + registerPatientUAE.PatientNationalId.ToString();
            Parameters += "&IdType=" + registerPatientUAE.IdType;
            Parameters += "&NationalityId=" + NationalityCode;
            Parameters += "&Address=" + registerPatientUAE.PatientAddress;
            Parameters += "&CurrentCity=" + registerPatientUAE.CurrentCity;
            Parameters += "&BirthDate=" + DOB;
            Parameters += "&Gender=" + PGENDER.ToString();
            Parameters += "&LastName=" + registerPatientUAE.PatientLastName;
            Parameters += "&FirstName=" + registerPatientUAE.PatientFirstName;
            


            BaseAPIUrl = "https://app.saudigerman.com/Services/api/Patient-add" + Parameters;
            var resp = new GenericResponse_NEWUAE_registration();
            var _NewData = RestUtility.CallAPI_POST_UAE_Add_Patient<RegistrationPostResponse>(BaseAPIUrl, content, out resp, true);
            
            ReturnObject = new RegistrationPostResponse();
            //ReturnObject = resp as RegistrationPostResponse;
            ReturnObject.Error = 0;
            if (resp.Error)
            ReturnObject.Error =  1;
            ReturnObject.Message = resp.Message;
            ReturnObject.Mrn = resp.Mrn;


            if (resp.Error == false)
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
        public List<Doctor_Schedule_days_Modal_ForMobile> GetDoctorSchduleDaysByApi_NewUAE(string lang, string DoctorID, int BranchID, DateTime selectedDate)
        {
            HttpStatusCode status;            
            string BaseAPIUrl = "https://app.saudigerman.com/Services/api/";

            var BranchName = "";
            BranchName = GetBranchName(BranchID);
            var UAEDoctorEmployeeCode = GetDoctorUAEEmployeeCode(BranchID, DoctorID);
            DateTime firstDayOfMonth;
            DateTime lastDayOfMonth;

            GetMonthBoundaries(selectedDate,out firstDayOfMonth,out lastDayOfMonth);

            //var string 
			var content = new[]{
					new KeyValuePair<string, string>("hospital_id", BranchName),
					new KeyValuePair<string, string>("StartDate", firstDayOfMonth.ToString("dd-MM-yyyy")+"T00:00:00"),
					new KeyValuePair<string, string>("EndDate", lastDayOfMonth.ToString("dd-MM-yyyy")+"T00:00:00"),
					new KeyValuePair<string, string>("physician_id", UAEDoctorEmployeeCode),
					new KeyValuePair<string, string>("Lang", lang)
			};

			var Parameters = "?hospital_id=" + BranchName.ToString();
            Parameters += "&StartDate=" + firstDayOfMonth.ToString("yyyy-MM-dd")+"T00:00:00";
            Parameters += "&EndDate=" + lastDayOfMonth.ToString("yyyy-MM-dd") + "T00:00:00";
            Parameters += "&physician_id=" + UAEDoctorEmployeeCode;
            Parameters += "&Lang=" + lang.ToUpper();

            BaseAPIUrl = "https://app.saudigerman.com/Services/api/doctor-days-get" + Parameters;
            var resp = new GenericResponse();
            var _NewData = RestUtility.CallAPI_POST_UAE<List<UAE_Doctor_Days>>(BaseAPIUrl, content, out resp, true);

            var _Data_UAE = new List<UAE_Doctor_Days>();
            _Data_UAE = _NewData as List<UAE_Doctor_Days>;

            var _NewDataForMobile = MapDoctorSchduleDays_NewUAE(_Data_UAE , BranchID, DoctorID);
            return _NewDataForMobile;
        }

        private List<Doctor_Schedule_days_Modal_ForMobile> MapDoctorSchduleDays_NewUAE(List<UAE_Doctor_Days> _APIModal, int BranchID , string DoctorId)
        {
            List<Doctor_Schedule_days_Modal_ForMobile> _ListObj = new List<Doctor_Schedule_days_Modal_ForMobile>();
            //Doctor_Schedule_days_Modal_ForMobile
            if (_APIModal != null && _APIModal.Count > 0)
            {
                for (var i = 0; i < _APIModal.Count; i++)
                {
                    Doctor_Schedule_days_Modal_ForMobile _TempModalObj = new Doctor_Schedule_days_Modal_ForMobile();
                    _TempModalObj.AvailableSlots = _APIModal[i].AvailableSlots;
                    
                    _TempModalObj.DepartmentID = _APIModal[i].DepartmentID;
                    _TempModalObj.Doctor_Id = Convert.ToInt32(DoctorId);
                    _TempModalObj.FromTime = _APIModal[i].Fromtime;
                    _TempModalObj.Id = _APIModal[i].Id;                    
                    
                    
                    _TempModalObj.Scheduled_Day = _APIModal[i].Scheduled_day;
                    
                    //var currentDateTime = DateTime.Now.Date;
                    //var TodayDate = currentDateTime.Date;

                    if (_APIModal[i].Scheduled_day.Date < DateTime.Now.Date)
					{
                        _TempModalObj.NoSchedules = 1;
                        _TempModalObj.DayBooked = 1;
                    }
                    else
					{
                        _TempModalObj.NoSchedules = _APIModal[i].noSchedules;
                        _TempModalObj.DayBooked = _APIModal[i].DayBooked;
                    }


                    


                    _TempModalObj.Schedule_1_From = _APIModal[i].schedule_1;
                    
                    _TempModalObj.Schedule_2_From = _APIModal[i].schedule_2;                    
                    _TempModalObj.SlotTypeId = _APIModal[i].SlotTypeId;
                    _TempModalObj.SlotTypeName = _APIModal[i].SlotTypeName;                   
                    
                    _TempModalObj.TotalSlotCount = _APIModal[i].totalSlotCount;
                    _TempModalObj.ToTime = _APIModal[i].ToTime;
                    

                    //_TempModalObj.Schedule_2_To = _APIModal[i].Schedule_2_To;
                    //_TempModalObj.Split_Shift= _APIModal[i].Split_Shift;
                    //_TempModalObj.Schedule_1_To = _APIModal[i].;


                    _ListObj.Add(_TempModalObj);
                }
            }
            return _ListObj;
        }


        public string GetMyDoctorSlots_NewUAE(string lang, int BranchID, string Registration)
        {
            //HttpStatusCode status;
            string BaseAPIUrl = "https://app.saudigerman.com/Services/api/";

            var BranchName = "";
            BranchName = GetBranchName(BranchID);



            var content = new[]{
                    new KeyValuePair<string, string>("hospital_id", BranchName),                    
                    new KeyValuePair<string, string>("patient_reg_no", Registration)                    
            };


            var Parameters = "?hospital_id=" + BranchName.ToString();
            Parameters += "&patient_reg_no=" + Registration;

            BaseAPIUrl = "https://app.saudigerman.com/Services/api/Mydoctors-get" + Parameters;
            var resp = new GenericResponse();
            var _NewData = RestUtility.CallAPI_POST_UAE<List<UAE_My_Docotr>>(BaseAPIUrl, content, out resp, true) as List<UAE_My_Docotr>;
            
            //var ReturntOject =  MapMyDoctorModel_NewUAE(_NewData);
            if (_NewData.Count > 0 )
			{
                var ReturntStr = MapMyDoctorModel_NewUAE(_NewData);
                return ReturntStr;
            }
            return "";
                

            
        }

        
        private string MapMyDoctorModel_NewUAE(List<UAE_My_Docotr> _APIModal)
        {            
            //List<Doctor_Code_N_NearestSlot> _ListObj = new List<Doctor_Code_N_NearestSlot>();
            var strModal = "";
            var strModalEmdCode = "";
            var strModalNear = "";

            if (_APIModal != null && _APIModal.Count > 0)
            {
                for (var i = 0; i < _APIModal.Count; i++)
                {
                    Doctor_Code_N_NearestSlot _TempModalObj = new Doctor_Code_N_NearestSlot();
                    strModalNear += _APIModal[i].nearestAvailableSlot + "~";
                    strModalEmdCode += _APIModal[i].id + "~";
                    //_TempModalObj.empcode = _APIModal[i].id;
                    //_TempModalObj.nearest_slot= _APIModal[i].nearestAvailableSlot;
                    //_ListObj.Add(_TempModalObj);
                }
            }
            strModal = strModalEmdCode.Substring(0, strModalEmdCode.Length-1) + "|" + strModalNear.Substring(0, strModalNear.Length - 1);
            //return _ListObj;
            return strModal;
        }


        //AvailableSlots
        // doctor Schule days
        //Doctor_Schedule_days_Modal
        public List<AvailableSlots> GetDoctorSlotsOfDaysByApi_NewUAE(string lang, string DoctorID, int BranchID, DateTime startDate)
        {
            HttpStatusCode status;
            string BaseAPIUrl = "https://app.saudigerman.com/Services/api/";

            var BranchName = "";
            BranchName = GetBranchName(BranchID);
            var UAEDoctorEmployeeCode = GetDoctorUAEEmployeeCode(BranchID, DoctorID);
            

            var content = new[]{
                    new KeyValuePair<string, string>("hospital_id", BranchName),
                    new KeyValuePair<string, string>("Date", startDate.ToString("yyyy-MM-dd")+ "T00:00:00"),                    
                    new KeyValuePair<string, string>("physician_id", UAEDoctorEmployeeCode)
                    //new KeyValuePair<string, string>("Lang", lang)
            };


            var Parameters = "?hospital_id=" + BranchName.ToString();
            Parameters += "&Date=" + startDate.ToString("yyyy-MM-dd")+ "T00:00:00";            
            Parameters += "&physician_id=" + UAEDoctorEmployeeCode;
          

            BaseAPIUrl = "https://app.saudigerman.com/Services/api/doctor-slots-get" + Parameters;
            var resp = new GenericResponse();
            var _NewData = RestUtility.CallAPI_POST_UAE<List<AvailableSlots>>(BaseAPIUrl, content, out resp, true) as List<AvailableSlots>;

            
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

        private List<PateintTests> MapLabRadioINfoModel_NewDamma(List<Medical_Perscription_Dam> _APIModal, string ReportType)
        {
            string formatString = "yyyyMMddHHmmss";
            List<PateintTests> _ListObj = new List<PateintTests>();

            if (_APIModal != null && _APIModal.Count > 0)
            {
                for (var i = 0; i < _APIModal.Count; i++)
                {
                    PateintTests _TempModalObj = new PateintTests();
                    _TempModalObj.ftp_path = "https://cxmw.sghgroup.net/DoctorsProfile/CS/index2.html"; // FOr Dammam Fixed

                    var temOPID = "OP";

                    if (_APIModal[i].opip == "I")
                        temOPID = "IP";
                    _TempModalObj.opip = temOPID;

                    _TempModalObj.registration_no = Convert.ToInt32(_APIModal[i].fileNumber);// FOr Dammam Fixed                    
                    _TempModalObj.report_filename = _APIModal[i].orders[0].dose + " " + _APIModal[i].orders[0].dispensingUnit;

                    _TempModalObj.report_id = 0; // Currently Fixed For Dammam
                    _TempModalObj.report_type = ReportType;
                    _TempModalObj.test_id = 0; // Currently Fixed For Dammam
                    _TempModalObj.test_name = _APIModal[i].orders[0].procedureName;

                    DateTime dt = DateTime.ParseExact(_APIModal[i].orders[0].requestDate, formatString, null);
                    _TempModalObj.report_date = dt;


                    _ListObj.Add(_TempModalObj);
                }
            }
            return _ListObj;
        }


        // For Family Listing

        public List<PatientFamilyList> GetPatientFamilyListForBooking_NewUAE(string lang, int hospitalID, string MRN,int BookingFor, ref int Er_Status, ref string Msg)
        {

            HttpStatusCode status;

            var BranchName = GetBranchName(hospitalID);
            var BranchNameBookingFor = GetBranchName(BookingFor);

            string BaseAPIUrl = "https://app.saudigerman.com/Services/api/BookingPatientFamily-list-get";
            
            var content = new[]{
                    new KeyValuePair<string, string>("hospital_id", BranchName.ToString()),
                    new KeyValuePair<string, string>("patient_reg_no", MRN),
                    new KeyValuePair<string, string>("Booking_hospital_id", BranchNameBookingFor.ToString())
            };

            var Parameters = "?hospital_id=" + BranchName.ToString();
            Parameters += "&patient_reg_no=" + MRN;
            Parameters += "&Booking_hospital_id=" + BranchNameBookingFor;
           

            //AppointmentPostResponse
            BaseAPIUrl = "https://app.saudigerman.com/Services/api/BookingPatientFamily-list-get" + Parameters;

            
            
            var resp = new GenericResponse();
            var _NewData = RestUtility.CallAPI_POST_UAE<List<UAE_Patient_Info>>(BaseAPIUrl, content, out resp, true);


            var _patientData_Dam = new List<UAE_Patient_Info>();
            _patientData_Dam = _NewData as List<UAE_Patient_Info>;

            var _userInfo = MapPatientListToFamilyList_BookingFor(_patientData_Dam,hospitalID, BookingFor, lang);
            return _userInfo;

        }
        public List<PatientFamilyList> MapPatientListToFamilyList_BookingFor(List<UAE_Patient_Info> _InputData,int BranchID , int BookFor_BranchID, string lang = "EN")
        {
            var BranchFullName = GetBranchFullName(BranchID);
            var BranchNameBookingFor = GetBranchFullName(BookFor_BranchID);

            var NewData = new List<PatientFamilyList>();

            if (_InputData != null && _InputData.Count > 0)
            {
                for (var i = 0; i < _InputData.Count; i++)
                {
                    if (_InputData[i].registration_no != "" && _InputData[i].registration_no != "0")
                    {
                        

                            PatientFamilyList _TempModalObj = new PatientFamilyList();

                       
                            _TempModalObj.CountryId = 3;
                       

                        _TempModalObj.BranchId = BookFor_BranchID.ToString(); 
                        _TempModalObj.Age = _InputData[i].age;
                        if (lang == "AR" || lang == "ar")
                            _TempModalObj.BranchName = BranchNameBookingFor;
                        else
                            _TempModalObj.BranchName = BranchNameBookingFor;
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
                        catch (Exception e)
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




        public List<PatientFamilyList> GetPatientFamilyList_NewUAE(string lang, int hospitalID, string MRN, ref int Er_Status, ref string Msg)
        {

            HttpStatusCode status;

            var BranchName = GetBranchName(hospitalID);

            string BaseAPIUrl = "https://app.saudigerman.com/Services/api/BookingPatientFamily-list-get";

            var content = new[]{
                    new KeyValuePair<string, string>("hospital_id", BranchName.ToString()),
                    new KeyValuePair<string, string>("patient_reg_no", MRN)                    
            };

            var Parameters = "?hospital_id=" + BranchName.ToString();
            Parameters += "&patient_reg_no=" + MRN;
            
            //AppointmentPostResponse
            BaseAPIUrl = "https://app.saudigerman.com/Services/api/patientFamilyProfile-List-Get" + Parameters;

            var resp = new GenericResponse();
            var _NewData = RestUtility.CallAPI_POST_UAE<List<UAE_Patient_Info>>(BaseAPIUrl, content, out resp, true);


            var _patientData_Dam = new List<UAE_Patient_Info>();
            _patientData_Dam = _NewData as List<UAE_Patient_Info>;

            var _userInfo = MapPatientFamilyList_UAE(_patientData_Dam, hospitalID, lang);
            return _userInfo;

        }
      
        public List<PatientFamilyList> MapPatientFamilyList_UAE(List<UAE_Patient_Info> _InputData, int BranchID, string lang = "EN")
        {
            var BranchFullName = GetBranchFullName(BranchID);
            
            var NewData = new List<PatientFamilyList>();

            if (_InputData != null && _InputData.Count > 0)
            {
                for (var i = 0; i < _InputData.Count; i++)
                {
                    if (_InputData[i].registration_no != "" && _InputData[i].registration_no != "0")
                    {
                        PatientFamilyList _TempModalObj = new PatientFamilyList();
                        var Barahcode = _InputData[i].hospital_id.ToString();

                        
                            _TempModalObj.CountryId = 3;


                        var intBranchID = GetBranchID(_InputData[i].hospital_id.ToString());
                        _TempModalObj.BranchId = intBranchID;
                        _TempModalObj.Age = _InputData[i].age;
                        //if (lang == "AR" || lang == "ar")
                        //    _TempModalObj.BranchName = BranchNameBookingFor;
                        //else
                        //    _TempModalObj.BranchName = BranchNameBookingFor;
                        
                        _TempModalObj.BranchName = _InputData[i].hospital_id.ToString();


                        var DBO_date = Convert.ToDateTime(_InputData[i].birthday);
                        var strDBO = DBO_date.ToString("MMM d yyyy");
                        
                        _TempModalObj.DOB = strDBO;



                        _TempModalObj.FamilyName = _InputData[i].family_name;
                        _TempModalObj.FirstName = _InputData[i].first_name;
                        _TempModalObj.Gender = _InputData[i].gender;
                        _TempModalObj.image_url = "";
                        _TempModalObj.LastName = _InputData[i].last_name;
                        _TempModalObj.MaritalStatus = "Single";
                        _TempModalObj.MiddleName = _InputData[i].middle_name;
                        _TempModalObj.Nationality = _InputData[i].nationality;
                        try
                        {
                            var strNAtionalaity = GetUnifiedNationalID(intBranchID, _InputData[i].nationalityId);

                            _TempModalObj.NationalityId = Convert.ToInt32(strNAtionalaity);
                        }
                        catch (Exception e)
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


        private List<PatientVisit_UAE> MapPatientVisitModel_NewUAE(List<PatientVisit_UAE> _APIModal, string MRN)
        {
            string formatString = "yyyy-MM-dd";
            List<PatientVisit_UAE> _ListObj = new List<PatientVisit_UAE>();

            if (_APIModal != null && _APIModal.Count > 0)
            {
                for (var i = 0; i < _APIModal.Count; i++)
                {
                    PatientVisit_UAE _TempModalObj = new PatientVisit_UAE();
                    _TempModalObj.id = _APIModal[i].id;                    
                    _TempModalObj.appointmentNo = 0;
                    _TempModalObj.registrationNo = _APIModal[i].registrationNo;
                    _TempModalObj.clinicName = _APIModal[i].clinicName;
                    _TempModalObj.appDate = _APIModal[i].appDate;
                    _TempModalObj.appTime = _APIModal[i].appTime;
                    _TempModalObj.patientVisited = 1;
                    _TempModalObj.patientName = _APIModal[i].patientName;
                    _TempModalObj.doctorName = _APIModal[i].doctorName;
                    _TempModalObj.videoCallUrl = _APIModal[i].videoCallUrl;
                    _TempModalObj.doctorId = _APIModal[i].doctorId;
                    _TempModalObj.appointmentType = _APIModal[i].appointmentType;
                    _TempModalObj.paid = _APIModal[i].paid;

                    _TempModalObj.episodeId = _APIModal[i].episodeId;                    
                    _TempModalObj.episodeType = _APIModal[i].episodeType;

                    _TempModalObj.episodeStatus = _APIModal[i].episodeStatus;
                    var intBranchID = GetBranchID(_APIModal[i].branchId.ToString());
                    _TempModalObj.branchId= intBranchID;

                    _ListObj.Add(_TempModalObj);
                }
            }
            return _ListObj;
        }


        public List<PatientVisit_UAE> GetPatientVisitByApi_NewUAE(string lang, int hospitalID, string MRN, ref int Er_Status, ref string Msg)
        {
            HttpStatusCode status;
            var BranchName = GetBranchName(hospitalID);
            string BaseAPIUrl = "https://app.saudigerman.com/Services/api/Patient-InsuranceInfo-get";

            var content = new[]{
                    new KeyValuePair<string, string>("hospital_id", BranchName.ToString()),
                    new KeyValuePair<string, string>("patient_reg_no", MRN)
            };

            var Parameters = "?hospital_id=" + BranchName.ToString();
            Parameters += "&patient_reg_no=" + MRN;

            //AppointmentPostResponse
            BaseAPIUrl = "https://app.saudigerman.com/Services/api/patient-visits-get" + Parameters;

            var resp = new GenericResponse();
            var _NewData = RestUtility.CallAPI_POST_UAE<List<PatientVisit_UAE>>(BaseAPIUrl, content, out resp, true);



            var _patientData_UAE = _NewData as List<PatientVisit_UAE>;
            var _MapDATA = MapPatientVisitModel_NewUAE(_patientData_UAE, MRN);

            return _MapDATA;

        }


        public List<PatientInsurance_UAE> GetPatientInsuranceByApi_NewUAE(string lang,int hospitalID, string MRN, ref int Er_Status, ref string Msg)
        {
            HttpStatusCode status;
            var BranchName = GetBranchName(hospitalID);
            string BaseAPIUrl = "https://app.saudigerman.com/Services/api/Patient-InsuranceInfo-get";

            var content = new[]{
                    new KeyValuePair<string, string>("hospital_id", BranchName.ToString()),
                    new KeyValuePair<string, string>("patient_reg_no", MRN)
            };

            var Parameters = "?hospital_id=" + BranchName.ToString();
            Parameters += "&patient_reg_no=" + MRN;

            //AppointmentPostResponse
            BaseAPIUrl = "https://app.saudigerman.com/Services/api/Patient-InsuranceInfo-get" + Parameters;

            var resp = new GenericResponse();
            var _NewData = RestUtility.CallAPI_POST_UAE<List<PatientInsurance_UAE>>(BaseAPIUrl, content, out resp, true);



            var _patientData_UAE = _NewData as List<PatientInsurance_UAE>;
            var _MapDATA = MapPatientInsuranceModel_NewUAE(_patientData_UAE, MRN);

            return _MapDATA;

        }

        private List<PatientInsurance_UAE> MapPatientInsuranceModel_NewUAE(List<PatientInsurance_UAE> _APIModal, string MRN)
        {
            //string formatString = "yyyy-MM-dd";
            List<PatientInsurance_UAE> _ListObj = new List<PatientInsurance_UAE>();

            var NewAPIMOdal = _APIModal.OrderByDescending(od => od.idExpiryDate);

            if (_APIModal != null && _APIModal.Count > 0)
            {
                for (var i = 0; i < _APIModal.Count; i++)
                {
                    PatientInsurance_UAE _TempModalObj = new PatientInsurance_UAE();
                    _TempModalObj.category = _APIModal[0].category;
                    _TempModalObj.company = _APIModal[0].company;
                    _TempModalObj.grade = _APIModal[0].grade;
                    _TempModalObj.idExpiryDate = _APIModal[0].idExpiryDate;
                    _TempModalObj.insuranceId = _APIModal[0].insuranceId;
                    _TempModalObj.policyNo = _APIModal[0].policyNo;
                    _TempModalObj.registrationno = MRN;

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

        private string GetBranchName(int BranchID)
        {
            if (BranchID == 0)
                return "";

            CustomDBHelper _DB = new CustomDBHelper("RECEPTION");

            var SQL_Qry = "  Select BranchCode  from BAS_Branch_TB where HIS_Id = " + BranchID;
            var BranchName = _DB.ExecuteSQLScalar(SQL_Qry);
            return BranchName;
        }
        private string GetBranchFullName(int BranchID)
        {
            if (BranchID == 0)
                return "";

            CustomDBHelper _DB = new CustomDBHelper("RECEPTION");

            var SQL_Qry = "  Select BranchName  from BAS_Branch_TB where HIS_Id = " + BranchID;
            var BranchName = _DB.ExecuteSQLScalar(SQL_Qry);
            return BranchName;
        }


        private BranchInfo GetBranchInfoName(int BranchID , string lang = "EN")
        {
            if (BranchID == 0)
                return null;

            CustomDBHelper _DB = new CustomDBHelper("RECEPTION");


            _DB.param = new SqlParameter[]
                {
                    new SqlParameter("@BranchId", BranchID),
                    new SqlParameter("@Lang", lang)
                };            

            var ModelObj = _DB.ExecuteSPAndReturnDataTable("dbo.Get_HospitalInfo_sp").ToModelObject<BranchInfo>();
            return ModelObj;
        }

        private string GetUnifiedNationalID(string BranchID ,string Nationality)
        {
            //if (BranchID == "")
            //    return "";

            CustomDBHelper _DB = new CustomDBHelper("RECEPTION");

            var SQL_Qry = "  select UnifiedID  from BAS_Nationality_TB where Branch_Id = '"+ BranchID.ToString() + "' and HIS_Id = '"+ Nationality + "'" ;
            var BranchName = _DB.ExecuteSQLScalar(SQL_Qry);
            return BranchName;
        }

        private string GetBranchID(string BranchCode)
        {
            if (BranchCode == "")
                return "";

            CustomDBHelper _DB = new CustomDBHelper("RECEPTION");

            var SQL_Qry = "  Select HIS_ID from BAS_Branch_TB where BranchCode = '"+ BranchCode + "' " ;
            var BranchID = _DB.ExecuteSQLScalar(SQL_Qry);
            return BranchID;
        }

        private string GetDoctorUAEEmployeeCode(int BranchID , string DoctorID)
        {
            if (BranchID == 0)
                return "";

            CustomDBHelper _DB = new CustomDBHelper("RECEPTION");

            var SQL_Qry = "  select EmployeeCode  from BAS_OPDDoctors_TB where Branch_Id = '" + BranchID.ToString()  + "' and HIS_Id  = '"+ DoctorID + "'  " ;
            var EmployeeCode = _DB.ExecuteSQLScalar(SQL_Qry);
            return EmployeeCode;
        }
        public static void GetMonthBoundaries(DateTime SelectedDateTime, out DateTime firstDayOfMonth, out DateTime lastDayOfMonth)
        {            
            var month = SelectedDateTime.Month;
            var year = SelectedDateTime.Year;

            // Get the 1st day of the month (always day 1)
            DateTime first = new DateTime(year, month, 1);

            // Calculate the last day of the month
            DateTime last = first.AddMonths(1).AddSeconds(-1);

            // Set the out parameters
            firstDayOfMonth = first;
            lastDayOfMonth = last;
        }





        public bool UpdatePatientBasicData_NewUAE(int hospitalID, string MRN, string DOB, string MaritalStatus,string Gender,string patient_phone,string patient_Email, ref int Er_Status, ref string Msg)
        {


            var TempPcell = patient_phone.ToString();

            // change the the Format to
            TempPcell = TempPcell.Replace("%2B971", "");
            TempPcell = TempPcell.Replace("+", "");

            //TempPcell = TempPcell.Substring(0,3).Replace("966", "");                            
            if (TempPcell.Substring(0, 5) == "00971")
            {
                TempPcell = TempPcell.Substring(5, TempPcell.Length - 5);
            }
            if (TempPcell.Substring(0, 3) == "971")
            {
                TempPcell = TempPcell.Substring(3, TempPcell.Length - 3);
            }
            var FirstChar = TempPcell.Substring(0, 1);
			//if (FirstChar != "0")
			//{
			//	TempPcell = "0" + TempPcell;
			//}



			HttpStatusCode status;

            var BranchName = GetBranchName(hospitalID);

            string BaseAPIUrl = "https://app.saudigerman.com/Services/api/Patient-BasicData-update";

            var content = new[]{
                    new KeyValuePair<string, string>("hospital_id", BranchName.ToString()),
                    new KeyValuePair<string, string>("patient_reg_no", MRN),
                    new KeyValuePair<string, string>("patient_phone", TempPcell),
                    new KeyValuePair<string, string>("patient_DOB", DOB),
                    new KeyValuePair<string, string>("patient_Gender", Gender),
                    new KeyValuePair<string, string>("Marital_Status", MaritalStatus)
                    //new KeyValuePair<string, string>("patient_Email", MRN)

            };

            var Parameters = "?hospital_id=" + BranchName.ToString();
            Parameters += "&patient_reg_no=" + MRN;
            Parameters += "&patient_phone=" + TempPcell;
            Parameters += "&patient_DOB=" + DOB;
            Parameters += "&patient_Gender=" + Gender;
            Parameters += "&Marital_Status=" + MaritalStatus;

            BaseAPIUrl = "https://app.saudigerman.com/Services/api/Patient-BasicData-update" + Parameters;
            var resp = new GenericResponse();
            var _NewData = RestUtility.CallAPI_POST_UAE<GenericResponse>(BaseAPIUrl, content, out resp, true);

            Er_Status = 0;
            Msg = "Updated Failed. Please try again later.";
            if (resp != null)
			{
                Er_Status = resp.status;
                Msg = resp.msg;
            }
            
            return true;

        }



        public List<PateintTests_New_V4> GetPatientTestList_NewUAE(string lang, int hospitalID, string MRN, ref int Er_Status, ref string Msg)
        {
            HttpStatusCode status;

            var BranchName = GetBranchName(hospitalID);

            string BaseAPIUrl = "https://app.saudigerman.com/Services/api/Test-List-Get";

            var content = new[]{
                    new KeyValuePair<string, string>("hospital_id", BranchName.ToString()),
                    new KeyValuePair<string, string>("patient_reg_no", MRN)
            };

            var Parameters = "?hospital_id=" + BranchName.ToString();
            Parameters += "&patient_reg_no=" + MRN;

            //AppointmentPostResponse
            BaseAPIUrl = "https://app.saudigerman.com/Services/api/Test-List-Get" + Parameters;

            var resp = new GenericResponse();
            var _NewData = RestUtility.CallAPI_POST_UAE<List<LabTest_List_Modal>>(BaseAPIUrl, content, out resp, true);


            var _patientData_Dam = new List<LabTest_List_Modal>();
            _patientData_Dam = _NewData as List<LabTest_List_Modal>;

            var _userInfo = MapLabRadioINfoModel_NewUAE(_patientData_Dam, hospitalID).OrderByDescending(o => o.report_date).ToList(); ;
            return _userInfo;

        }

        private List<PateintTests_New_V4> MapLabRadioINfoModel_NewUAE(List<LabTest_List_Modal> _APIModal, int BranchID)
        {
            string formatString = "yyyy-MM-dd";
            List<PateintTests_New_V4> _ListObj = new List<PateintTests_New_V4>();
			if (_APIModal != null && _APIModal.Count > 0)
			{
				for (var i = 0; i < _APIModal.Count; i++)
				{
                    PateintTests_New_V4 _TempModalObj = new PateintTests_New_V4();

					_TempModalObj.opip = _APIModal[i].opip;
                    _TempModalObj.registration_no = _APIModal[i].registration_no;
                    _TempModalObj.report_filename = _APIModal[i].report_filename;
					_TempModalObj.report_id = _APIModal[i].report_id; 
					_TempModalObj.report_type = _APIModal[i].report_type;
					_TempModalObj.test_id = _APIModal[i].test_id; 
					_TempModalObj.test_name = _APIModal[i].test_name;

					//DateTime dt = DateTime.ParseExact(_APIModal[i].report_date, formatString, null);
                    DateTime ReportdateTime = DateTime.Parse(_APIModal[i].report_date.ToString());
                    _TempModalObj.report_date = ReportdateTime;
                    _TempModalObj.ftp_path = _APIModal[i].ftp_path;


                    _ListObj.Add(_TempModalObj);
				}
			}
			return _ListObj;
        }




		public TestResultMain GetPatientTestResultsList_NewUAE(string lang, int hospitalID, string TestID, ref int Er_Status, ref string Msg)
		{
			HttpStatusCode status;

			var BranchName = GetBranchName(hospitalID);

			string BaseAPIUrl = "";

			var content = new[]{
					new KeyValuePair<string, string>("hospital_id", BranchName.ToString()),
					new KeyValuePair<string, string>("test_id", TestID)
			};

			var Parameters = "?hospital_id=" + BranchName.ToString();
			Parameters += "&test_id=" + TestID;

			//AppointmentPostResponse
			BaseAPIUrl = "https://app.saudigerman.com/Services/api/test-resultdetails-get" + Parameters;

			var resp = new GenericResponse();
			var _NewData = RestUtility.CallAPI_POST_UAE<List<TestResult_details_UAE>>(BaseAPIUrl, content, out resp, true);


			var _patientData_Dam = new List<TestResult_details_UAE>();
			_patientData_Dam = _NewData as List<TestResult_details_UAE>;

			
            var _userInfo = MapTestResultModelToTestResultMain_UAE(_patientData_Dam);           

            return _userInfo;

		}

        private TestResultMain MapTestResultModelToTestResultMain_UAE(List<TestResult_details_UAE> testOrders)
        {  

            TestResultMain testResultMain = new TestResultMain();
            List<TestResultParameter> testParameters = new List<TestResultParameter>();

            testResultMain.testCode = testOrders[0].testCode;
            testResultMain.testName = testOrders[0].testName;
            testResultMain.section = testOrders[0].section;

            testResultMain.sample_name = testOrders[0].sample_name;
            testResultMain.collected_date = testOrders[0].collected_date;


            // For Testing
            var icount = testOrders[0].parameters.Count();
            //var itst = 1;

            if (testOrders.Count > 0)
			{
                foreach (var testParam in testOrders)
				{
                    if (testParam.parameters.Count > 0 )
					{

                        foreach (var param in testParam.parameters)
                        {

                            TestResultParameter parameter = new TestResultParameter();

                            if (param.parameter_name == "Sendout File Result")
                            {
                                continue;
                            }


                            parameter.parameter_name = param.parameter_name ?? "";
                            parameter.result = param.result ?? "";
                            parameter.unit = param.unit ?? "";
                            parameter.range = param.range ?? "";
                            parameter.ResultValueCategory = param.resultValueCategory ?? "N";

                            //parameter.severityID = "N";
                            parameter.rating = param.rating;
                            parameter.severityID = param.rating;

                            if (parameter.result != "")
                            {
                                var tempResult = parameter.result;

                                if (tempResult.Substring(0, 1) == ".")
                                {
                                    parameter.result = "0" + tempResult;
                                }
                                else if (tempResult.EndsWith("."))
                                {
                                    parameter.result = tempResult + "0";
                                }
                            }



                            if (param.rating == null || param.rating.Trim() == "")
                                parameter.severityID = "N";

                            parameter.Weightage = 0;

                            if (parameter.severityID == "N")
                                parameter.Weightage = 0;
                            else if (parameter.severityID == "H")
                                parameter.Weightage = 50;
                            else if (parameter.severityID == "L")
                                parameter.Weightage = 50;
                            else if (parameter.severityID == "P")
                                parameter.Weightage = 100;


                            if (parameter.parameter_name == "RAD. REPORT")
                                parameter.parameter_name = "RADIOLOGY REPORT";

                            parameter.parameter_name = parameter.parameter_name.Replace(":", "").Replace(".", "");




                            testParameters.Add(parameter);

                            //itst += 1;

                        }
                    }
				}                    
			}
            

            testParameters = testParameters.OrderByDescending(o => o.Weightage).ToList();

            testResultMain.parameters = testParameters;

            return testResultMain;

        }



        public LabRad_PDF_UAE GetPatientLabRAD_UAE_PDF(int hospitalID,string MRN, string TestID,string ReportType, ref int Er_Status, ref string Msg)
        {
            HttpStatusCode status;

            var BranchName = GetBranchName(hospitalID);

            string BaseAPIUrl = "";

            var content = new[]{
                    new KeyValuePair<string, string>("FacilityId", BranchName.ToString()),
                    new KeyValuePair<string, string>("PatientId", MRN),
                    new KeyValuePair<string, string>("OrderId", TestID),
                    new KeyValuePair<string, string>("ReportType", ReportType),
                    
            };

            var Parameters = "?FacilityId=" + BranchName.ToString();
            Parameters += "&PatientId=" + MRN;
            Parameters += "&OrderId=" + TestID;
            Parameters += "&ReportType=" + ReportType;

            //AppointmentPostResponse
            BaseAPIUrl = "https://app.saudigerman.com/Services/api/GetReports" + Parameters;

            var resp = new GenericResponse();
            var _NewData = RestUtility.CallAPI_POST_UAE_LABPDF<List<LabRad_PDF_UAE>>(BaseAPIUrl, content, true);


            var _LabRaDData = new List <LabRad_PDF_UAE>();
            _LabRaDData = _NewData as List<LabRad_PDF_UAE>;
            if (_LabRaDData != null)
			{
                var tempObj = new LabRad_PDF_UAE();
                tempObj.Base64 = _LabRaDData[0].Base64;
                tempObj.OrderNumber = _LabRaDData[0].OrderNumber;
                tempObj.PatientId = _LabRaDData[0].PatientId;
                return tempObj;
            }   

            return null;
            
            
            

        }


        

    }
}