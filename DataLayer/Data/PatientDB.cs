//using AgoraIO.Media;
//using AgoraIO.Media;
using DataLayer.Common;
using DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;



namespace DataLayer.Data
{
    public class PatientDB
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");

        public void SaveOnlinePaymentAgainstBills(string lang, int hospitaId, int registrationNo, decimal amount, string creditCardType, string ccNumber, DateTime? ccValidity, string onlineTransactionId, int hisRefId, int hisRefTypeId, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchId", hospitaId),
                new SqlParameter("@RegistrationNo", registrationNo),
                new SqlParameter("@Amount", amount),
                new SqlParameter("@CREDIT_CARD_TYPE", creditCardType),
                new SqlParameter("@ccnumber", ccNumber),
                new SqlParameter("@ccvalidity", ccValidity),
                new SqlParameter("@OnlineTransactionId", onlineTransactionId),
                new SqlParameter("@HISRefId", hisRefId),
                new SqlParameter("@HISRefTypeId", hisRefTypeId),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200),
            };
            DB.param[9].Direction = ParameterDirection.Output;
            DB.param[10].Direction = ParameterDirection.Output;

            var flag = DB.ExecuteSP("dbo.[Save_OnlinePayment_AgainstBills_SP]");

            errStatus = Convert.ToInt32(DB.param[9].Value);
            errMessage = DB.param[10].Value.ToString();
        }
        
        public void SaveRefillMedication(string lang, int hospitalID, int registrationNo, string iqamaPassportNo, string contactNumber, string selectedSpeciality, int sourceEntryId, ref int Er_Status, ref string Msg)
        {

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Branch_Id ", hospitalID),
                new SqlParameter("@RegistrationNo ", registrationNo),
                new SqlParameter("@IqamaPassportNo", iqamaPassportNo),
                new SqlParameter("@ContactNumber", contactNumber),
                new SqlParameter("@CSVSelectedSpeciality", selectedSpeciality),
                new SqlParameter("@SourceEntryId", sourceEntryId),
                new SqlParameter("@ReturnMessage", SqlDbType.NVarChar, 1000),
                new SqlParameter("@ReturnFlag", SqlDbType.Int)
            };
            DB.param[6].Direction = ParameterDirection.Output;
            DB.param[7].Direction = ParameterDirection.Output;

            var flag = DB.ExecuteSP("[dbo].[SaveMedicineRefillRequest_SP]");

            Er_Status = Convert.ToInt32(DB.param[7].Value);
            Msg = DB.param[6].Value.ToString();

        }

        public List<RegistePatientResponseFailure> RegisterNewPatient(RegisterPatient registerPatient, ref int status, ref string msg, ref string error_type, ref string successType, ref int RegistrationNo, ref int activationNo)
        {
            //new SqlParameter("@skip_duplicate_email", registerPatient.SkipDuplicateEmail),
            //new SqlParameter("@skip_duplicate_phone", registerPatient.SkipDuplicatePhone),
            //new SqlParameter("@activation_num", registerPatient.ActivationNum),
            //new SqlParameter("@skip_send_activation_num", registerPatient.SkipSendActivationNum),

            List<RegistePatientResponseFailure> registerPatientFailure = new List<RegistePatientResponseFailure>();
            DB.param = new SqlParameter[]
            {

                new SqlParameter("@Lang", registerPatient.Lang),
                new SqlParameter("@BranchId", registerPatient.HospitaId),
                new SqlParameter("@UserName", registerPatient.PateintUserName),
                new SqlParameter("@Password", registerPatient.PatientPassword),
                new SqlParameter("@TitleId", registerPatient.PatientTitleId),
                new SqlParameter("@FirstName", registerPatient.PatientFirstName),
                new SqlParameter("@MiddleName", registerPatient.PatientMiddleName),
                new SqlParameter("@LastName", registerPatient.PatientLastName),
                new SqlParameter("@FamilyName", registerPatient.PatientFamilyName),
                new SqlParameter("@CellNo", registerPatient.PatientPhone),
                new SqlParameter("@Email", registerPatient.PatientEmail),
                new SqlParameter("@NationalId", registerPatient.PatientNationalId),
                new SqlParameter("@DOB", registerPatient.PatientBirthday),
                new SqlParameter("@GenderId", registerPatient.PatientGender),
                new SqlParameter("@PAddress", registerPatient.PatientAddress),
                new SqlParameter("@NationalityId", registerPatient.PatientNationalityId),
                new SqlParameter("@MaritalStatusId", registerPatient.PatientMaritalStatusId),
                new SqlParameter("@RegistrationNo", SqlDbType.Int, registerPatient.PatientId),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200),
                new SqlParameter("@ERROR_TYPE", SqlDbType.NVarChar, 200),
                new SqlParameter("@SUCCESS_TYPE", SqlDbType.NVarChar, 200),
                new SqlParameter("@ACtivationNo", SqlDbType.Int)

            };

            DB.param[17].Direction = ParameterDirection.InputOutput;
            DB.param[18].Direction = ParameterDirection.Output;
            DB.param[19].Direction = ParameterDirection.Output;
            DB.param[20].Direction = ParameterDirection.Output;
            DB.param[21].Direction = ParameterDirection.Output;
            DB.param[22].Direction = ParameterDirection.Output;

            var exisitingPatient = DB.ExecuteSPAndReturnDataTable("[dbo].[Save_PatientData_SP]").ToListObject<RegistePatientResponseModel>();

            if (DB.param[17].Value != DBNull.Value)
            {
                RegistrationNo = Convert.ToInt32(DB.param[17].Value);
            }

            if (DB.param[18].Value != DBNull.Value)
            {
                status = Convert.ToInt32(DB.param[18].Value);
            }

            msg = DB.param[19].Value.ToString();
            error_type = DB.param[20].Value.ToString();
            successType = DB.param[21].Value.ToString();

            if (DB.param[22].Value != DBNull.Value)
            {
                activationNo = Convert.ToInt32(DB.param[22].Value);
            }

            if (status == 0 && exisitingPatient != null && exisitingPatient.Count > 0)
            {
                foreach (var item in exisitingPatient)
                {
                    RegistePatientResponseFailure i = new RegistePatientResponseFailure();
                    i.is_you = item.DisplayMsg;
                    i.registration_no = item.RegistrationNo;

                    registerPatientFailure.Add(i);
                }
            }

            return registerPatientFailure;
        }

        public void SaveOnlinePayment(string lang, int hospitaId, int registrationNo, decimal amount, string creditCardType, string ccNumber, DateTime? ccValidity, string onlineTransactionId, int hisRefId, int hisRefTypeId, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchId", hospitaId),
                new SqlParameter("@RegistrationNo", registrationNo),
                new SqlParameter("@Amount", amount),
                new SqlParameter("@CREDIT_CARD_TYPE", creditCardType),
                new SqlParameter("@ccnumber", ccNumber),
                new SqlParameter("@ccvalidity", ccValidity),
                new SqlParameter("@OnlineTransactionId", onlineTransactionId),
                new SqlParameter("@HISRefId", hisRefId),
                new SqlParameter("@HISRefTypeId", hisRefTypeId),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200),
            };
            DB.param[9].Direction = ParameterDirection.Output;
            DB.param[10].Direction = ParameterDirection.Output;

            var flag = DB.ExecuteSP("dbo.[Save_OnlinePayment_Consultation_SP]");

            errStatus = Convert.ToInt32(DB.param[9].Value);
            errMessage = DB.param[10].Value.ToString();
        }

        public ResendVerificationModel ResendVerificaitonCode(string lang, int hospitalId, string patientUserName, string patientPhone)
        {

            DB.param = new SqlParameter[]
            {

                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@UserName", patientUserName),
                new SqlParameter("@CellNo", patientPhone),
            };

            var resendVerification = DB.ExecuteSPAndReturnDataTable("dbo.Get_VerificationCode").ToListObject<ResendVerificationModel>();

            if (resendVerification != null && resendVerification.Count > 0)
            {
                return resendVerification[0];
            }
            else
            {
                return null;
            }

        }

        public string GetVerificaitonCode(int hospitalId, string patientRegNo, string patientPhone)
        {

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@RegistratioNo", patientRegNo),
                new SqlParameter("@CellNo", patientPhone)
            };

            DataTable dt = DB.ExecuteSPAndReturnDataTable("dbo.Get_VerificationCode2_SP");

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0].ItemArray.GetValue(0).ToString();
            }
            else
            {
                return null;
            }
        }



        
        public void ChangePassword(string lang, int hospitalId, string patientId, string patientUserName, string patientOldPassword, string patientNewPassword, ref int Er_Status, ref string Msg, ref string error_type)
        {
            try
            {
                DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@Branch_Id", hospitalId),
                    new SqlParameter("@Patient_Id", patientId),
                    new SqlParameter("@UserName", patientUserName),
                    new SqlParameter("@Old_Password", patientOldPassword),
                    new SqlParameter("@New_Password", patientNewPassword),
                    new SqlParameter("@status", SqlDbType.Int),
                    new SqlParameter("@msg", SqlDbType.NVarChar, 200),
                    new SqlParameter("@ERROR_TYPE", SqlDbType.NVarChar, 200)

                };
                DB.param[6].Direction = ParameterDirection.Output;
                DB.param[7].Direction = ParameterDirection.Output;
                DB.param[8].Direction = ParameterDirection.Output;

                var _patientModel = DB.ExecuteSP("DBO.[Update_ChanegPassword_SP]");

                Er_Status = Convert.ToInt32(DB.param[6].Value);
                Msg = DB.param[7].Value.ToString();
                error_type = DB.param[8].Value.ToString();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<Reservation> CheckPatientVidCallAppointments(string lang, int hospitalID, int registrationNo, int videoCall, ref int Er_Status, ref string Msg)
        {

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId ", hospitalID),
                new SqlParameter("@RegistrationNo", registrationNo)
                //new SqlParameter("@VideoCall", videoCall)
            };


            List<Reservation> _patientVidCallAppointments = new List<Reservation>();
            var patientVideoCallAppointments = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_PatientVideoAppointments_SP]").ToListObject<ReservationModel>();

            _patientVidCallAppointments = MapVidCallAppointsModelToVidCallReservations(patientVideoCallAppointments);

            return _patientVidCallAppointments;

        }

        public List<Reservation> CheckPatientVidCallAppointmentsDue(string lang, int hospitalID, int registrationNo, int videoCall, ref int Er_Status, ref string Msg)
        {

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId ", hospitalID),
                new SqlParameter("@RegistrationNo", registrationNo)
                //new SqlParameter("@VideoCall", videoCall)
            };

            List<Reservation> _patientVidCallAppointments = new List<Reservation>();
            var patientVideoCallAppointments = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_PatientVideoAppointments_SP]").ToListObject<ReservationModel>();

            _patientVidCallAppointments = MapVidCallAppointsModelToVidCallReservations(patientVideoCallAppointments);

            return _patientVidCallAppointments;

        }

        private List<Reservation> MapVidCallAppointsModelToVidCallReservationsDue(List<ReservationModel> patientVideoCallAppointments)
        {
            List<Reservation> _reservationList = new List<Reservation>();
            foreach (var row in patientVideoCallAppointments)
            {
                Reservation _reservation = new Reservation();

                _reservation.clinic_name = row.ClinicName;
                _reservation.date = row.AppDate.ToString("yyyy-MM-dd");
                _reservation.id = row.Id;
                _reservation.patient_attend = row.PatientVisited;
                _reservation.patient_name = row.PatientName;
                _reservation.physician_name = row.DoctorName;
                _reservation.time_from = row.AppTime.ToString();
                _reservation.video_call_url = row.VideoCallUrl;
                _reservation.doctor_id = row.DoctorId;
                _reservation.paid = row.Paid;
                _reservation.appointment_type = row.AppointmentType;
                _reservation.appointment_no = row.AppointmentNo;
                _reservation.registration_no = row.RegistrationNo;
                _reservation.branch_id = row.Branch_Id;
                _reservation.duration = row.Duration;
                _reservation.time_zone = row.TimeZone;
                _reservation.nationional_id = row.NationionalId;
                _reservation.end_date = row.EndDate.ToString("yyyy-MM-dd");
                _reservation.end_time = row.EndTime.ToString();
                _reservation.host2 = row.host2;
                _reservation.autoJoin2 = row.autoJoin2;
                _reservation.resourceId2 = row.resourceId2;
                _reservation.displayName2 = row.displayName2;
                _reservation.token2 = row.token2;
                _reservation.active = row.IsActive;

                _reservationList.Add(_reservation);
            }
            return _reservationList;
        }

        public void MobileAppUpdateRequest(string lang, int hospitalID, string registrationNo, string iqamaNaionalId, string cellNumber, int sourceEntryId, string fileName, ref int Er_Status, ref string Msg)
        {

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Branch_Id", hospitalID),
                new SqlParameter("@RegistrationNo ", registrationNo),
                new SqlParameter("@IqamaPassportNo", iqamaNaionalId),
                new SqlParameter("@ContactNumber", cellNumber),
                new SqlParameter("@ImageFileName", fileName),
                new SqlParameter("@SourceEntryId", sourceEntryId),
                new SqlParameter("@ReturnMessage", SqlDbType.NVarChar, 200),
                new SqlParameter("@ReturnFlag", SqlDbType.Int)
            };

            DB.param[6].Direction = ParameterDirection.Output;
            DB.param[7].Direction = ParameterDirection.Output;

            var _patientModel = DB.ExecuteSP("[dbo].[SaveMobileUpdateRequest_SP]");

            Er_Status = Convert.ToInt32(DB.param[7].Value);
            Msg = DB.param[6].Value.ToString();

        }

        public Patient CheckPatientHISPIN(string lang, int hospitalID, string patientPIN, string pateintNationalId, string patientPhone, string patientEmail, ref int Er_Status, ref string Msg)
        {
            try
            {
                DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalID),
                    new SqlParameter("@RegistrationNo", patientPIN),
                    new SqlParameter("@CellNo", patientPhone),
                    new SqlParameter("@PEmail", patientEmail),
                    new SqlParameter("@NationalId",pateintNationalId),
                    new SqlParameter("@status", SqlDbType.Int),
                    new SqlParameter("@msg", SqlDbType.NVarChar, 200)
                };
                DB.param[6].Direction = ParameterDirection.Output;
                DB.param[7].Direction = ParameterDirection.Output;

                var _patientModel = DB.ExecuteSPAndReturnDataTable("DBO.[Get_PatientDataFromHIS_SP]").ToListObject<PatientModel>();

                Er_Status = Convert.ToInt32(DB.param[6].Value);
                Msg = DB.param[7].Value.ToString();

                Patient _patient = new Patient();
                if (Er_Status > 0)
                {
                    _patient = MapPatinetModelToPatient(_patientModel);
                }

                return _patient;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<PatientPrescription> GetPatientPrescription(string lang, int hospitalId, int resgistrationNo, ref int Er_Status, ref string Msg)
        {
            try
            {
                List<PatientPrescription> _listOfPatientDiagnosis = new List<PatientPrescription>();
                DB.param = new SqlParameter[]
                {
                    new SqlParameter("@BranchId", hospitalId),
                    new SqlParameter("@RegistrationNo ", resgistrationNo),
                    new SqlParameter("@status", SqlDbType.Int),
                    new SqlParameter("@msg", SqlDbType.NVarChar, 200)
                };
                DB.param[2].Direction = ParameterDirection.Output;
                DB.param[3].Direction = ParameterDirection.Output;

                var _patientPrescriptionModel = DB.ExecuteSPAndReturnDataTable("[DBO].[Get_PatientPrescription_SP]").ToListObject<PatientPrescription>();

                _listOfPatientDiagnosis = MapPatientPrescriptionModelToPatientPrescription(_patientPrescriptionModel);

                Er_Status = Convert.ToInt32(DB.param[2].Value);
                Msg = DB.param[3].Value.ToString();

                return _listOfPatientDiagnosis;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<PateintTests> GetPatientTestResults(string lang, int hospitalId, int resgistrationNo, ref int Er_Status, ref string Msg)
        {
            try
            {
                List<PateintTests> _listOfPatientDiagnosis = new List<PateintTests>();
                DB.param = new SqlParameter[]
                {
                    new SqlParameter("@BranchId", hospitalId),
                    new SqlParameter("@RegistrationNo ", resgistrationNo),
                    new SqlParameter("@status", SqlDbType.Int),
                    new SqlParameter("@msg", SqlDbType.NVarChar, 200)
                };
                DB.param[2].Direction = ParameterDirection.Output;
                DB.param[3].Direction = ParameterDirection.Output;

                var _patientTestModel = DB.ExecuteSPAndReturnDataTable("[DBO].[Get_PatientLABnRADResults_SP]").ToListObject<PateintTestsModel>();

                _listOfPatientDiagnosis = MapPatientTestResultsModelToPatientTestResults(_patientTestModel);

                Er_Status = Convert.ToInt32(DB.param[2].Value);
                Msg = DB.param[3].Value.ToString();

                return _listOfPatientDiagnosis;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public PatientData CheckPatientData(string lang, int hospitalID, int patientID, string pateintPhone, string registrationNo, ref int Er_Status, ref string Msg)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId", hospitalID),
                new SqlParameter("@PatientId ", patientID),
                new SqlParameter("@RegistrationNo", registrationNo),
                new SqlParameter("@CellNo ", pateintPhone),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200)
            };
            DB.param[5].Direction = ParameterDirection.Output;
            DB.param[6].Direction = ParameterDirection.Output;

            var patientDataset = DB.ExecuteSPAndReturnDataSet("DBO.[Get_PatientDetails_SP]");

            var patientData = MapPatinetDataModelToPatientData(patientDataset);
            Er_Status = Convert.ToInt32(DB.param[5].Value);
            Msg = DB.param[6].Value.ToString();

            return patientData;

        }

        private PatientData MapPatinetDataModelToPatientData(DataSet patientDataset)
        {
            try
            {
                var patient = new Patient();
                var reservationList = new List<Reservation>();
                var pateitnData = new PatientData();
                
                if (patientDataset != null && patientDataset.Tables[0] != null && patientDataset.Tables[0].Rows.Count > 0)
                {
                    var patientModel = patientDataset.Tables[0].ToListObject<PatientModel>();
                    foreach (var row in patientModel)
                    {
                        patient.address = row.PAddress;
                        patient.birthday = row.DOB.ToString("yyyy-MM-dd");
                        patient.email = row.PEmail;
                        patient.family_name = row.FamilyName;
                        patient.first_name = row.FirstName;
                        patient.gender = row.GenderId;
                        patient.hospital_id = row.Branch_Id;
                        patient.id = row.PatientId;
                        patient.last_name = row.LastName;
                        patient.marital_status_id = row.MaritalStatusId;
                        patient.middle_name = row.MiddleName;
                        patient.name = row.Name;
                        patient.national_id = row.NationalId;
                        patient.phone = row.PCellno;
                        patient.registration_no = row.RegistrationNo;
                        patient.title_id = row.TitleId;
                    }

                    pateitnData.patient = patient;
                }


                if (patientDataset != null && patientDataset.Tables[1] != null && patientDataset.Tables[1].Rows.Count > 0)
                {
                    var _reservationModel = patientDataset.Tables[1].ToListObject<ReservationModel>();
                    foreach (var row in _reservationModel)
                    {
                        Reservation _reservation = new Reservation();

                        _reservation.clinic_name = row.ClinicName;
                        _reservation.date = row.AppDate.ToString("yyyy-MM-dd");
                        _reservation.id = row.Id;
                        _reservation.patient_attend = row.PatientVisited;
                        _reservation.patient_name = row.PatientName;
                        _reservation.physician_name = row.DoctorName;
                        _reservation.time_from = row.AppTime.ToString();
                        _reservation.video_call_url = row.VideoCallUrl;
                        _reservation.doctor_id = row.DoctorId;
                        _reservation.paid = row.Paid;
                        _reservation.appointment_type = row.AppointmentType;
                        _reservation.appointment_no = row.AppointmentNo;
                        _reservation.registration_no = row.RegistrationNo;
                        _reservation.branch_id = row.Branch_Id;
                        _reservation.duration = row.Duration;
                        _reservation.time_zone = row.TimeZone;
                        _reservation.nationional_id = row.NationionalId;
                        _reservation.end_date = row.EndDate.ToString("yyyy-MM-dd");
                        _reservation.end_time = row.EndTime.ToString();
                        _reservation.host2 = row.host2;
                        _reservation.autoJoin2 = row.autoJoin2;
                        _reservation.resourceId2 = row.resourceId2;
                        _reservation.displayName2 = row.displayName2;
                        _reservation.token2 = row.token2;
                        _reservation.active = row.IsActive;

                        _reservation.UpComingAppointment = row.UpComingAppointment; // Ahsan Changes 27/05/2021

                        reservationList.Add(_reservation);
                    }

                    pateitnData.reservations = reservationList;

                }

                return pateitnData;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public void UpdateVideoCallURL(string lang, int hospitaId, int scheduleDayId, string videoCallUrl, ref string  errMessage, ref int errStatus)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId ", hospitaId),
                new SqlParameter("@DoctorScheduleId ", scheduleDayId),
                new SqlParameter("@OnVideoCallUrl", videoCallUrl),
                new SqlParameter("@ReturnMessage", SqlDbType.NVarChar, 200),
                new SqlParameter("@ReturnFlag", SqlDbType.Int)
            };
            DB.param[4].Direction = ParameterDirection.Output;
            DB.param[5].Direction = ParameterDirection.Output;

            var flag = DB.ExecuteSP("[dbo].[Update_Appointment_SP]");

            errStatus = Convert.ToInt32(DB.param[5].Value);
            errMessage = DB.param[4].Value.ToString();
        }

        public void SaveAppointment(string lang, int hospitalID, int clinicId, int physicianId, DateTime selectedDate, int patientID, DateTime timeFrom, DateTime timeTo, int scheduleDayId,int EarlyReminder, int HeardAboutUsId,ref int Er_Status, ref string Msg, ref int IsVideoAppointment, ref string DoctorName)
        {

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId ", hospitalID),
                new SqlParameter("@PatientType ", 3),
                new SqlParameter("@DepartmentId", clinicId),
                new SqlParameter("@DoctorId", physicianId),
                new SqlParameter("@ScheduleDayId", scheduleDayId),
                new SqlParameter("@FromDate", selectedDate),
                new SqlParameter("@FromDateTime", timeFrom),
                new SqlParameter("@ToDateTime", timeTo),
                new SqlParameter("@PatientId", patientID),
                new SqlParameter("@InformEarlierAvailability", EarlyReminder),
                new SqlParameter("@HeardAboutUsId", HeardAboutUsId),                
                new SqlParameter("@ReturnMessage", SqlDbType.NVarChar, 200),
                new SqlParameter("@ReturnFlag", SqlDbType.Int),
                new SqlParameter("@IsVideoAppointment", SqlDbType.Int),
                new SqlParameter("@DoctorName", SqlDbType.NVarChar, 200)
            };
            
            DB.param[12].Direction = ParameterDirection.Output;
            DB.param[13].Direction = ParameterDirection.Output;
            DB.param[14].Direction = ParameterDirection.Output;
            DB.param[15].Direction = ParameterDirection.Output;

            var flag = DB.ExecuteSP("DBO.[Save_Appointment_SP]");

            Msg = DB.param[12].Value.ToString();
            Er_Status = Convert.ToInt32(DB.param[13].Value);
            IsVideoAppointment = Convert.ToInt32(DB.param[14].Value);
            DoctorName = DB.param[15].Value.ToString();

            if (lang == "AR" || lang == "ar")
            {
                if (Msg == "There is already one appointment for same Specialty for same day")
                {
                    Msg = "يوجد بالفعل موعد واحد لنفس التخصص لنفس اليوم";
                }

                if (Er_Status != 0)
                {
                    Msg = "تم حفظ الموعد";
                }


            }

            

        }

        public void SaveAppointment_V2(string lang, int hospitalID, int clinicId, int physicianId, DateTime selectedDate, int patientID, DateTime timeFrom, DateTime timeTo, int scheduleDayId, int EarlyReminder, int HeardAboutUsId,string sources,int SlotType, ref int Er_Status, ref string Msg, ref int IsVideoAppointment, ref string DoctorName , string agent_UserName ="" , string agent_UserID = "")
        {
            string DB_SP_Name = "DBO.[Save_Appointment_V2_SP]";

            if (sources.ToLower() == "saleforce")
			{
                DB.param = new SqlParameter[]
               {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalID),
                    new SqlParameter("@PatientType", 3),
                    new SqlParameter("@DepartmentId", clinicId),
                    new SqlParameter("@DoctorId", physicianId),
                    new SqlParameter("@ScheduleDayId", scheduleDayId),
                    new SqlParameter("@FromDate", selectedDate),
                    new SqlParameter("@FromDateTime", timeFrom),
                    new SqlParameter("@ToDateTime", timeTo),
                    new SqlParameter("@PatientId", patientID),
                    new SqlParameter("@InformEarlierAvailability", EarlyReminder),
                    new SqlParameter("@HeardAboutUsId", HeardAboutUsId),
                    new SqlParameter("@ReturnMessage", SqlDbType.NVarChar, 500),
                    new SqlParameter("@ReturnFlag", SqlDbType.Int),
                    new SqlParameter("@IsVideoAppointment", SqlDbType.Int),
                    new SqlParameter("@DoctorName", SqlDbType.NVarChar, 200),
                    new SqlParameter("@Source", sources),
                    new SqlParameter("@BookingType", SlotType),
                    new SqlParameter("@SF_UserId", agent_UserID),
                    new SqlParameter("@SF_UserName", agent_UserName)
               };

                DB.param[12].Direction = ParameterDirection.Output;
                DB.param[13].Direction = ParameterDirection.Output;
                DB.param[14].Direction = ParameterDirection.Output;
                DB.param[15].Direction = ParameterDirection.Output;

                DB_SP_Name = "SF.[Save_Appointment_V2_SP]";
            }
			else
			{
                DB.param = new SqlParameter[]
               {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalID),
                    new SqlParameter("@PatientType", 3),
                    new SqlParameter("@DepartmentId", clinicId),
                    new SqlParameter("@DoctorId", physicianId),
                    new SqlParameter("@ScheduleDayId", scheduleDayId),
                    new SqlParameter("@FromDate", selectedDate),
                    new SqlParameter("@FromDateTime", timeFrom),
                    new SqlParameter("@ToDateTime", timeTo),
                    new SqlParameter("@PatientId", patientID),
                    new SqlParameter("@InformEarlierAvailability", EarlyReminder),
                    new SqlParameter("@HeardAboutUsId", HeardAboutUsId),
                    new SqlParameter("@ReturnMessage", SqlDbType.NVarChar, 500),
                    new SqlParameter("@ReturnFlag", SqlDbType.Int),
                    new SqlParameter("@IsVideoAppointment", SqlDbType.Int),
                    new SqlParameter("@DoctorName", SqlDbType.NVarChar, 200),
                    new SqlParameter("@Source", sources),
                    new SqlParameter("@BookingType", SlotType)
               };

                DB.param[12].Direction = ParameterDirection.Output;
                DB.param[13].Direction = ParameterDirection.Output;
                DB.param[14].Direction = ParameterDirection.Output;
                DB.param[15].Direction = ParameterDirection.Output;
            }
                

            var flag = DB.ExecuteSP(DB_SP_Name);

            Msg = DB.param[12].Value.ToString();
            Er_Status = Convert.ToInt32(DB.param[13].Value);
            if (Er_Status != 0)
            {
                IsVideoAppointment = Convert.ToInt32(DB.param[14].Value);
                DoctorName = DB.param[15].Value.ToString();
            }
            

            if (lang == "AR" || lang == "ar")
            {
                if (Msg == "There is already one appointment for same Specialty for same day")
                {
                    Msg = "يوجد بالفعل موعد واحد لنفس التخصص لنفس اليوم";
                }

                //if (Er_Status != 0)
                //{
                //    Msg = "تم حفظ الموعد";
                //}
            }
        }

        //public string GenerateVideoURL_Token_Test(string ChannelName, int startTime, int ExpireTime, long UID)
        //{
        //    //string appID = "0fe4ac748cd24a9fb3ba4730ea201df9";
        //    //string appCertificate = "3a313f24a1f243d19be7ac04ef3856af";
        //    //int ts = 1645689245;
        //    //int r = 123456789;
        //    //long uid = 0;
        //    //int expiredTs = 1645775645;

        //    string appID = ConfigurationManager.AppSettings["Agora_APPID"].ToString();
        //    string appCertificate = ConfigurationManager.AppSettings["Agora_APPCertificate"].ToString();

        //    int ts = startTime;
        //    int r = 123456789;
        //    long uid = UID;
        //    int expiredTs = ExpireTime;


        //    string Token = DynamicKey3.generate(appID, appCertificate, ChannelName, ts, r, uid, expiredTs);

        //    return Token;
        //}


        public void SaveAppointment_Nearest_V2(string lang, int hospitalID, int clinicId, int physicianId, DateTime selectedDate, int patientID, DateTime timeFrom, DateTime timeTo, int scheduleDayId, int EarlyReminder, int HeardAboutUsId, string sources, ref int Er_Status, ref string Msg, ref int IsVideoAppointment, ref string DoctorName)
        {

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId ", hospitalID),
                new SqlParameter("@PatientType ", 3),
                new SqlParameter("@DepartmentId", clinicId),
                new SqlParameter("@DoctorId", physicianId),
                new SqlParameter("@ScheduleDayId", scheduleDayId),
                new SqlParameter("@FromDate", selectedDate),
                new SqlParameter("@FromDateTime", timeFrom),
                new SqlParameter("@ToDateTime", timeTo),
                new SqlParameter("@PatientId", patientID),
                new SqlParameter("@InformEarlierAvailability", EarlyReminder),
                new SqlParameter("@HeardAboutUsId", HeardAboutUsId),
                new SqlParameter("@NearestAppBooking", 1),
                new SqlParameter("@ReturnMessage", SqlDbType.NVarChar, 200),
                new SqlParameter("@ReturnFlag", SqlDbType.Int),
                new SqlParameter("@IsVideoAppointment", SqlDbType.Int),
                new SqlParameter("@DoctorName", SqlDbType.NVarChar, 200),
                new SqlParameter("@Source", sources)
            };

            DB.param[13].Direction = ParameterDirection.Output;
            DB.param[14].Direction = ParameterDirection.Output;
            DB.param[15].Direction = ParameterDirection.Output;
            DB.param[16].Direction = ParameterDirection.Output;

            var flag = DB.ExecuteSP("DBO.[Save_Appointment_V2_SP]");

            Msg = DB.param[13].Value.ToString();
            Er_Status = Convert.ToInt32(DB.param[14].Value);
            if (Er_Status != 0)
            {
                IsVideoAppointment = Convert.ToInt32(DB.param[15].Value);
                DoctorName = DB.param[16].Value.ToString();
            }


            if (lang == "AR" || lang == "ar")
            {
                if (Msg == "There is already one appointment for same Specialty for same day")
                {
                    Msg = "يوجد بالفعل موعد واحد لنفس التخصص لنفس اليوم";
                }

                if (Er_Status != 0)
                {
                    Msg = "تم حفظ الموعد";
                }
            }
        }




        public void RescheduleAppointment(string lang, int hospitalID, int clinicId, int physicianId, DateTime selectedDate, int patientID, DateTime timeFrom, DateTime timeTo, int scheduleDayId,int AppointmentID, int EarlyReminder, int HeardAboutUsId,string Sources,int BookType, ref int Er_Status, ref string Msg, ref int IsVideoAppointment, ref string DoctorName,string agent_UserName="",string agent_UserID = "")
        {

            
            

            string DB_SP_Name = "DBO.[Reschedule_Appointment_SP]";

            if (Sources.ToLower() == "saleforce" || Sources == "SaleForce")
			{
                DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalID),
                    new SqlParameter("@PatientType", 3),
                    new SqlParameter("@DepartmentId", clinicId),
                    new SqlParameter("@DoctorId", physicianId),
                    new SqlParameter("@ScheduleDayId", scheduleDayId),
                    new SqlParameter("@FromDate", selectedDate),
                    new SqlParameter("@FromDateTime", timeFrom),
                    new SqlParameter("@ToDateTime", timeTo),
                    new SqlParameter("@PatientId", patientID),
                    new SqlParameter("@AppointmentID", AppointmentID),
                    new SqlParameter("@InformEarlierAvailability", EarlyReminder),
                    new SqlParameter("@HeardAboutUsId", HeardAboutUsId),
                    new SqlParameter("@ReturnMessage", SqlDbType.NVarChar, 200),
                    new SqlParameter("@ReturnFlag", SqlDbType.Int),
                    new SqlParameter("@IsVideoAppointment", SqlDbType.Int),
                    new SqlParameter("@DoctorName", SqlDbType.NVarChar, 200),
                    new SqlParameter("@Sources", Sources),
                    new SqlParameter("@BookingType", BookType),
                    new SqlParameter("@SF_UserId", agent_UserID),
                    new SqlParameter("@SF_UserName", agent_UserName)

                };

                DB_SP_Name = "SF.[Reschedule_Appointment_SP]";
            }
			else
			{
                DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalID),
                    new SqlParameter("@PatientType", 3),
                    new SqlParameter("@DepartmentId", clinicId),
                    new SqlParameter("@DoctorId", physicianId),
                    new SqlParameter("@ScheduleDayId", scheduleDayId),
                    new SqlParameter("@FromDate", selectedDate),
                    new SqlParameter("@FromDateTime", timeFrom),
                    new SqlParameter("@ToDateTime", timeTo),
                    new SqlParameter("@PatientId", patientID),
                    new SqlParameter("@AppointmentID", AppointmentID),
                    new SqlParameter("@InformEarlierAvailability", EarlyReminder),
                    new SqlParameter("@HeardAboutUsId", HeardAboutUsId),
                    new SqlParameter("@ReturnMessage", SqlDbType.NVarChar, 200),
                    new SqlParameter("@ReturnFlag", SqlDbType.Int),
                    new SqlParameter("@IsVideoAppointment", SqlDbType.Int),
                    new SqlParameter("@DoctorName", SqlDbType.NVarChar, 200),
                    new SqlParameter("@Sources", Sources),
                    new SqlParameter("@BookingType", BookType),

                };

            }




            DB.param[13].Direction = ParameterDirection.Output;
            DB.param[14].Direction = ParameterDirection.Output;
            DB.param[15].Direction = ParameterDirection.Output;
            DB.param[16].Direction = ParameterDirection.Output;

            var flag = DB.ExecuteSP(DB_SP_Name);

            Msg = DB.param[13].Value.ToString();
            Er_Status = Convert.ToInt32(DB.param[14].Value);
            IsVideoAppointment = Convert.ToInt32(DB.param[15].Value);
            DoctorName = DB.param[16].Value.ToString();

        }

        public void CancelAppointment(string lang, int hospitalID, int AppointmentID, int RegistrationNo,int CancelResonID,string Sources, ref int Er_Status, ref string Msg , string agent_userid= "",string agent_username = "")
        {

            
            
            string DB_SP_Name = "[dbo].[Cancel_PatientAppointments_SP]";

            if (Sources.ToLower() == "saleforce")
			{
                DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalID),
                    new SqlParameter("@AppointmentId", AppointmentID),
                    new SqlParameter("@RegistrationNo", RegistrationNo),
                    new SqlParameter("@CancelResonID", CancelResonID),
                    new SqlParameter("@ReturnMessage", SqlDbType.NVarChar, 200),
                    new SqlParameter("@ReturnFlag", SqlDbType.Int),
                    new SqlParameter("@Sources", Sources),
                    new SqlParameter("@SF_UserId", agent_userid),
                    new SqlParameter("@SF_UserName", agent_username)
                };

                DB.param[5].Direction = ParameterDirection.Output;
                DB.param[6].Direction = ParameterDirection.Output;

                DB_SP_Name = "[SF].[Cancel_PatientAppointments_SP]";
            
            }
            else
			{
                DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalID),
                    new SqlParameter("@AppointmentId", AppointmentID),
                    new SqlParameter("@RegistrationNo", RegistrationNo),
                    new SqlParameter("@CancelResonID", CancelResonID),
                    new SqlParameter("@ReturnMessage", SqlDbType.NVarChar, 200),
                    new SqlParameter("@ReturnFlag", SqlDbType.Int),
                    new SqlParameter("@Sources", Sources)

                };

                DB.param[5].Direction = ParameterDirection.Output;
                DB.param[6].Direction = ParameterDirection.Output;

            }



            var flag = DB.ExecuteSP(DB_SP_Name);

            Msg = DB.param[5].Value.ToString();
            Er_Status = Convert.ToInt32(DB.param[6].Value);




        }

        public void ConfirmAppointment(string lang, int hospitalID, int AppointmentID, int RegistrationNo,string Sources, ref int Er_Status, ref string Msg)
        {

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId", hospitalID),
                new SqlParameter("@AppointmentId", AppointmentID),
                new SqlParameter("@RegistrationNo", RegistrationNo),
                new SqlParameter("@ReturnMessage", SqlDbType.NVarChar, 200),
                new SqlParameter("@ReturnFlag", SqlDbType.Int),
                new SqlParameter("@Sources", Sources)

            };


            DB.param[4].Direction = ParameterDirection.Output;
            DB.param[5].Direction = ParameterDirection.Output;

            var flag = DB.ExecuteSP("[dbo].[Confirm_PatientAppointments_SP]");

            Msg = DB.param[4].Value.ToString();
            Er_Status = Convert.ToInt32(DB.param[5].Value);




        }

        public List<RegistePatientResponseFailure> RegisterNewPatient_V2(RegisterPatient2 registerPatient,ref int status, ref string msg, ref string error_type, ref string successType, ref int RegistrationNo, ref int activationNo , string ApiSources = "MobileApp")
        {
            //new SqlParameter("@skip_duplicate_email", registerPatient.SkipDuplicateEmail),
            //new SqlParameter("@skip_duplicate_phone", registerPatient.SkipDuplicatePhone),
            //new SqlParameter("@activation_num", registerPatient.ActivationNum),
            //new SqlParameter("@skip_send_activation_num", registerPatient.SkipSendActivationNum),

            List<RegistePatientResponseFailure> registerPatientFailure = new List<RegistePatientResponseFailure>();
            DB.param = new SqlParameter[]
            {

                new SqlParameter("@Lang", registerPatient.Lang),
                new SqlParameter("@BranchId", registerPatient.HospitaId),
                //new SqlParameter("@UserName", registerPatient.PateintUserName),
                //new SqlParameter("@Password", registerPatient.PatientPassword),
                new SqlParameter("@TitleId", registerPatient.PatientTitleId),
                new SqlParameter("@FirstName", registerPatient.PatientFirstName),
                new SqlParameter("@MiddleName", registerPatient.PatientMiddleName),
                new SqlParameter("@LastName", registerPatient.PatientLastName),
                new SqlParameter("@FamilyName", registerPatient.PatientFamilyName),
                new SqlParameter("@CellNo", registerPatient.PatientPhone),
                new SqlParameter("@Email", registerPatient.PatientEmail),
                new SqlParameter("@NationalId", registerPatient.PatientNationalId),
                new SqlParameter("@DOB", registerPatient.PatientBirthday),
                new SqlParameter("@GenderId", registerPatient.PatientGender),
                new SqlParameter("@PAddress", registerPatient.PatientAddress),
                new SqlParameter("@NationalityId", registerPatient.PatientNationalityId),
                new SqlParameter("@MaritalStatusId", registerPatient.PatientMaritalStatusId),
                new SqlParameter("@RegistrationNo", SqlDbType.Int, registerPatient.PatientId),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200),
                new SqlParameter("@ERROR_TYPE", SqlDbType.NVarChar, 200),
                new SqlParameter("@SUCCESS_TYPE", SqlDbType.NVarChar, 200),
                new SqlParameter("@ACtivationNo", SqlDbType.Int),
                new SqlParameter("@skip_duplicate_check", registerPatient.skipDuplicateCheck)

            };

            DB.param[15].Direction = ParameterDirection.InputOutput;
            DB.param[16].Direction = ParameterDirection.Output;
            DB.param[17].Direction = ParameterDirection.Output;
            DB.param[18].Direction = ParameterDirection.Output;
            DB.param[19].Direction = ParameterDirection.Output;
            DB.param[20].Direction = ParameterDirection.Output;

            
            string DB_SP_Name = "[dbo].[Save_PatientData_V2_SP]";

            if (ApiSources.ToLower() == "saleforce")
                DB_SP_Name = "[SF].[Save_PatientData_V2_SP]";

            var exisitingPatient = DB.ExecuteSPAndReturnDataTable(DB_SP_Name).ToListObject<RegistePatientResponseModel>();

            if (DB.param[15].Value != DBNull.Value)
            {
                RegistrationNo = Convert.ToInt32(DB.param[15].Value);
            }

            if (DB.param[16].Value != DBNull.Value)
            {
                status = Convert.ToInt32(DB.param[16].Value);
            }

            msg = DB.param[17].Value.ToString();
            error_type = DB.param[18].Value.ToString();
            successType = DB.param[19].Value.ToString();

            if (DB.param[20].Value != DBNull.Value)
            {
                activationNo = Convert.ToInt32(DB.param[20].Value);
            }

            if (status == 0 && exisitingPatient != null && exisitingPatient.Count > 0)
            {
                foreach (var item in exisitingPatient)
                {
                    RegistePatientResponseFailure i = new RegistePatientResponseFailure();
                    i.is_you = item.DisplayMsg;
                    i.registration_no = item.RegistrationNo;
                    i.name = item.name;
                    i.name_ar = item.name_ar;

                    registerPatientFailure.Add(i);
                }
            }

            return registerPatientFailure;
        }

        public List<RegistePatientResponseFailure> RegisterNewPatient_V5(RegisterPatientUAE registerPatient, ref int status, ref string msg, ref string error_type, ref string successType, ref int RegistrationNo, ref int activationNo, string ApiSources = "MobileApp")
        {
            //new SqlParameter("@skip_duplicate_email", registerPatient.SkipDuplicateEmail),
            //new SqlParameter("@skip_duplicate_phone", registerPatient.SkipDuplicatePhone),
            //new SqlParameter("@activation_num", registerPatient.ActivationNum),
            //new SqlParameter("@skip_send_activation_num", registerPatient.SkipSendActivationNum),

            List<RegistePatientResponseFailure> registerPatientFailure = new List<RegistePatientResponseFailure>();
            DB.param = new SqlParameter[]
            {

                new SqlParameter("@Lang", "EN"),
                new SqlParameter("@BranchId", registerPatient.HospitaId),                
                new SqlParameter("@TitleId", registerPatient.PatientTitleId),
                new SqlParameter("@FirstName", registerPatient.PatientFirstName),
                new SqlParameter("@MiddleName", registerPatient.PatientMiddleName),
                new SqlParameter("@LastName", registerPatient.PatientLastName),
                new SqlParameter("@FamilyName", registerPatient.PatientFamilyName),
                new SqlParameter("@CellNo", registerPatient.PatientPhone),
                new SqlParameter("@Email", registerPatient.PatientEmail),
                new SqlParameter("@NationalId", registerPatient.PatientNationalId),
                new SqlParameter("@DOB", registerPatient.PatientBirthday),
                new SqlParameter("@GenderId", registerPatient.PatientGender),
                new SqlParameter("@PAddress", registerPatient.PatientAddress),
                new SqlParameter("@NationalityId", registerPatient.PatientNationalityId),
                new SqlParameter("@MaritalStatusId", registerPatient.PatientMaritalStatusId),
                new SqlParameter("@RegistrationNo", SqlDbType.Int, registerPatient.PatientId2),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200),
                new SqlParameter("@ERROR_TYPE", SqlDbType.NVarChar, 200),
                new SqlParameter("@SUCCESS_TYPE", SqlDbType.NVarChar, 200),
                new SqlParameter("@ACtivationNo", SqlDbType.Int),
                new SqlParameter("@skip_duplicate_check", registerPatient.skipDuplicateCheck)

            };

            DB.param[15].Direction = ParameterDirection.InputOutput;
            DB.param[16].Direction = ParameterDirection.Output;
            DB.param[17].Direction = ParameterDirection.Output;
            DB.param[18].Direction = ParameterDirection.Output;
            DB.param[19].Direction = ParameterDirection.Output;
            DB.param[20].Direction = ParameterDirection.Output;


            string DB_SP_Name = "[dbo].[Save_PatientData_V2_SP]";

            if (ApiSources.ToLower() == "saleforce")
                DB_SP_Name = "[SF].[Save_PatientData_V2_SP]";

            var exisitingPatient = DB.ExecuteSPAndReturnDataTable(DB_SP_Name).ToListObject<RegistePatientResponseModel>();

            if (DB.param[15].Value != DBNull.Value)
            {
                RegistrationNo = Convert.ToInt32(DB.param[15].Value);
            }

            if (DB.param[16].Value != DBNull.Value)
            {
                status = Convert.ToInt32(DB.param[16].Value);
            }

            msg = DB.param[17].Value.ToString();
            error_type = DB.param[18].Value.ToString();
            successType = DB.param[19].Value.ToString();

            if (DB.param[20].Value != DBNull.Value)
            {
                activationNo = Convert.ToInt32(DB.param[20].Value);
            }

            if (status == 0 && exisitingPatient != null && exisitingPatient.Count > 0)
            {
                foreach (var item in exisitingPatient)
                {
                    RegistePatientResponseFailure i = new RegistePatientResponseFailure();
                    i.is_you = item.DisplayMsg;
                    i.registration_no = item.RegistrationNo;
                    i.name = item.name;
                    i.name_ar = item.name_ar;

                    registerPatientFailure.Add(i);
                }
            }

            return registerPatientFailure;
        }

        public List<RegistePatientResponseFailure> RegisterNewPatient(RegisterPatient2 registerPatient, ref int status, ref string msg, ref string error_type, ref string successType, ref int RegistrationNo, ref int activationNo)
        {
            //new SqlParameter("@skip_duplicate_email", registerPatient.SkipDuplicateEmail),
            //new SqlParameter("@skip_duplicate_phone", registerPatient.SkipDuplicatePhone),
            //new SqlParameter("@activation_num", registerPatient.ActivationNum),
            //new SqlParameter("@skip_send_activation_num", registerPatient.SkipSendActivationNum),

            List<RegistePatientResponseFailure> registerPatientFailure = new List<RegistePatientResponseFailure>();
            DB.param = new SqlParameter[]
            {
                
                new SqlParameter("@Lang", registerPatient.Lang),
                new SqlParameter("@BranchId", registerPatient.HospitaId),
                //new SqlParameter("@UserName", registerPatient.PateintUserName),
                //new SqlParameter("@Password", registerPatient.PatientPassword),
                new SqlParameter("@TitleId", registerPatient.PatientTitleId),
                new SqlParameter("@FirstName", registerPatient.PatientFirstName),
                new SqlParameter("@MiddleName", registerPatient.PatientMiddleName),
                new SqlParameter("@LastName", registerPatient.PatientLastName),
                new SqlParameter("@FamilyName", registerPatient.PatientFamilyName),
                new SqlParameter("@CellNo", registerPatient.PatientPhone),
                new SqlParameter("@Email", registerPatient.PatientEmail),
                new SqlParameter("@NationalId", registerPatient.PatientNationalId),
                new SqlParameter("@DOB", registerPatient.PatientBirthday),
                new SqlParameter("@GenderId", registerPatient.PatientGender),
                new SqlParameter("@PAddress", registerPatient.PatientAddress),
                new SqlParameter("@NationalityId", registerPatient.PatientNationalityId),
                new SqlParameter("@MaritalStatusId", registerPatient.PatientMaritalStatusId),
                new SqlParameter("@RegistrationNo", SqlDbType.Int, registerPatient.PatientId),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200),
                new SqlParameter("@ERROR_TYPE", SqlDbType.NVarChar, 200),
                new SqlParameter("@SUCCESS_TYPE", SqlDbType.NVarChar, 200),
                new SqlParameter("@ACtivationNo", SqlDbType.Int),
                new SqlParameter("@skip_duplicate_check", registerPatient.skipDuplicateCheck)

            };

            DB.param[15].Direction = ParameterDirection.InputOutput;
            DB.param[16].Direction = ParameterDirection.Output;
            DB.param[17].Direction = ParameterDirection.Output;
            DB.param[18].Direction = ParameterDirection.Output;
            DB.param[19].Direction = ParameterDirection.Output;
            DB.param[20].Direction = ParameterDirection.Output;

            var exisitingPatient = DB.ExecuteSPAndReturnDataTable("[dbo].[Save_PatientData_SP]").ToListObject<RegistePatientResponseModel>();

            if (DB.param[15].Value != DBNull.Value)
            {
                RegistrationNo = Convert.ToInt32(DB.param[15].Value);
            }

            if (DB.param[16].Value != DBNull.Value)
            {
                status = Convert.ToInt32(DB.param[16].Value);
            }

            msg = DB.param[17].Value.ToString();
            error_type = DB.param[18].Value.ToString();
            successType = DB.param[19].Value.ToString();

            if (DB.param[20].Value != DBNull.Value)
            {
                activationNo = Convert.ToInt32(DB.param[20].Value);
            }

            if (status == 0 && exisitingPatient != null && exisitingPatient.Count > 0)
            {
                foreach (var item in exisitingPatient)
                {
                    RegistePatientResponseFailure i = new RegistePatientResponseFailure();
                    i.is_you = item.DisplayMsg;
                    i.registration_no = item.RegistrationNo;
                    i.name = item.name;
                    i.name_ar = item.name_ar;

                    registerPatientFailure.Add(i);
                }
            }

            return registerPatientFailure;
        }

        
        private List<PatientBill> MapPatientBillModelToPatientBill(List<PatientBillModel> patientBillModel)
        {
            List<PatientBill> patientBill = new List<PatientBill>();
            foreach (var billModel in patientBillModel)
            {
                PatientBill bill = new PatientBill();
                bill.bill_no = billModel.BillNo;
                bill.total_amount = billModel.TotalAmount;
                bill.vat = billModel.VAT;
                bill.item_code = billModel.ItemCode;
                bill.registration_no = billModel.RegistrationNo;
                bill.item_name = billModel.ItemName;
                bill.qty = billModel.Qty;
                bill.price = billModel.Price;
                bill.paid = billModel.Paid;

                patientBill.Add(bill);

            }

            return patientBill;
        }

        public string VerifyOTP(int hospitalId, string patientRegNo, string patientPhone, string VerificationCode)
        {

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@RegistratioNo", patientRegNo),
                new SqlParameter("@CellNo", patientPhone),
                new SqlParameter("@VerificationCode", VerificationCode),
            };

            DataTable dt = DB.ExecuteSPAndReturnDataTable("dbo.Get_OTPVerified_SP");

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0].ItemArray.GetValue(0).ToString();
            }
            else
            {
                return null;
            }
        }

        public string VerifyOTP_ForRegistation(int hospitalId, string patientRegNo, string patientPhone, string VerificationCode , int ReasonCode)
        {

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@RegistratioNo", patientRegNo),
                new SqlParameter("@CellNo", patientPhone),
                new SqlParameter("@VerificationCode", VerificationCode),
                new SqlParameter("@ReasonCode", ReasonCode)
            };

            DataTable dt = DB.ExecuteSPAndReturnDataTable("dbo.Get_OTPVerified_SP");

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0].ItemArray.GetValue(0).ToString();
            }
            else
            {
                return null;
            }
        }

        public string VerifyOTP_Mobile(string patientPhone, string VerificationCode , int CountryID)
        {

            DB.param = new SqlParameter[]
            {                
                new SqlParameter("@CellNo", patientPhone),
                new SqlParameter("@VerificationCode", VerificationCode),
                new SqlParameter("@CountryID", CountryID)
            };

            DataTable dt = DB.ExecuteSPAndReturnDataTable("dbo.Get_OTPVerified_Mobile_V4_SP");

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0].ItemArray.GetValue(0).ToString();
            }
            else
            {
                return null;
            }
        }

        private Patient MapPatinetModelToPatient(List<PatientModel> _patientModel)
        {
            Patient _patient = new Patient();
            if (_patientModel != null && _patientModel.Count > 0)
            {
                foreach (var row in _patientModel)
                {
                    _patient.address = row.PAddress;
                    _patient.birthday = row.DOB.ToString("yyyy-MM-dd");
                    _patient.email = row.PEmail;
                    _patient.family_name = row.FamilyName;
                    _patient.first_name = row.FirstName;
                    _patient.gender = row.GenderId;
                    _patient.hospital_id = row.Branch_Id;
                    _patient.id = row.PatientId;
                    _patient.last_name = row.LastName;
                    _patient.marital_status_id = row.MaritalStatusId;
                    _patient.middle_name = row.MiddleName;
                    _patient.name = row.Name;
                    _patient.national_id = row.NationalId;
                    _patient.phone = row.PCellno;
                    _patient.registration_no = row.RegistrationNo;
                    _patient.title_id = row.TitleId;
                }
            }

            return _patient;
        }

        public List<PatientDiagnosis> GetPatientDiagnosis(string lang, int hospitalId, int resgistrationNo, ref int Er_Status, ref string Msg , string ApiSources = "MobileApp")
        {
            try
            {
                List<PatientDiagnosis> listOfPatientDiagnosis = new List<PatientDiagnosis>();
                DB.param = new SqlParameter[]
                {
                    new SqlParameter("@BranchId", hospitalId),
                    new SqlParameter("@RegistrationNo ", resgistrationNo),
                    new SqlParameter("@status", SqlDbType.Int),
                    new SqlParameter("@msg", SqlDbType.NVarChar, 200)
                };
                DB.param[2].Direction = ParameterDirection.Output;
                DB.param[3].Direction = ParameterDirection.Output;


                string DB_SP_Name = "[DBO].[Get_PatientDiagnoisis_SP]";

                if (ApiSources.ToLower() == "saleforce")
                    DB_SP_Name = "[SF].[Get_PatientDiagnoisis_SP]";


                var patientDaignosisModel = DB.ExecuteSPAndReturnDataTable(DB_SP_Name).ToListObject<PatientDiagnosisModel>();

                listOfPatientDiagnosis = MapPatientDiagnosisModelToPatientDiagnosis(patientDaignosisModel);

                Er_Status = Convert.ToInt32(DB.param[2].Value);
                Msg = DB.param[3].Value.ToString();

                return listOfPatientDiagnosis;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private List<PatientDiagnosis> MapPatientDiagnosisModelToPatientDiagnosis(List<PatientDiagnosisModel> _patientDaignosisModel)
        {
            try
            {
                List<PatientDiagnosis> _listOfPatientDiagnosis = new List<PatientDiagnosis>();

                if (_patientDaignosisModel != null && _patientDaignosisModel.Count > 0)
                {
                    _patientDaignosisModel.ForEach(p =>
                    {
                        _listOfPatientDiagnosis.Add(new PatientDiagnosis()
                        {
                            visit_id = p.VisitId,
                            visit_type = p.VisitType,
                            diagnosis_desc = p.DiagnosisDesc,
                            diagnosis_datetime = p.DiagnosisDateTime
                        });
                    });
                }

                return _listOfPatientDiagnosis;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private List<PatientPrescription> MapPatientPrescriptionModelToPatientPrescription(List<PatientPrescription> _patientDaignosisModel)
        {
            try
            {
                List<PatientPrescription> _listOfPatientDiagnosis = new List<PatientPrescription>();

                if (_patientDaignosisModel != null && _patientDaignosisModel.Count > 0)
                {
                    _patientDaignosisModel.ForEach(p =>
                    {
                        _listOfPatientDiagnosis.Add(new PatientPrescription()
                        {
                            visit_id = p.visit_id,
                            doctor_name = p.doctor_name,
                            prescription_date = p.prescription_date,
                            drug_name = p.drug_name,
                            route = p.route,
                            duration = p.duration
                        });
                    });
                }

                return _listOfPatientDiagnosis;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<PateintTests> GetPatientTestResultsNew(string lang, int hospitalId, int resgistrationNo, ref int Er_Status, ref string Msg,string ApiSources = "MobileApp" , string EpisodeType = "OP" , int EpisodeID = 0)
        {
            try
            {
                List<PateintTests> _listOfPatientDiagnosis = new List<PateintTests>();
                DB.param = new SqlParameter[]
                {
                    new SqlParameter("@BranchId", hospitalId),
                    new SqlParameter("@RegistrationNo ", resgistrationNo),
                    new SqlParameter("@status", SqlDbType.Int),
                    new SqlParameter("@msg", SqlDbType.NVarChar, 500),
                    new SqlParameter("@EpisodeType", EpisodeType),
                    new SqlParameter("@EpisodeId", EpisodeID)
                };
                DB.param[2].Direction = ParameterDirection.Output;
                DB.param[3].Direction = ParameterDirection.Output;

                string DB_SP_Name = "[dbo].[Get_PatientLABnRADResults_V2_SP]";

                if (ApiSources.ToLower() == "saleforce")
                    DB_SP_Name = "[SF].[Get_PatientLABnRADResults_V2_SP]";

                var _patientTestModel = DB.ExecuteSPAndReturnDataTable(DB_SP_Name).ToListObject<PateintTestsModel>();

                _listOfPatientDiagnosis = MapPatientTestResultsModelToPatientTestResults(_patientTestModel);

                Er_Status = Convert.ToInt32(DB.param[2].Value);
                Msg = DB.param[3].Value.ToString();

                return _listOfPatientDiagnosis;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private List<PateintTests> MapPatientTestResultsModelToPatientTestResults(List<PateintTestsModel> _patientDaignosisModel)
        {
            try
            {
                List<PateintTests> _listOfPatientDiagnosis = new List<PateintTests>();

                if (_patientDaignosisModel != null && _patientDaignosisModel.Count > 0)
                {
                    _patientDaignosisModel.ForEach(p =>
                    {
                        _listOfPatientDiagnosis.Add(new PateintTests()
                        {
                            registration_no = p.RegistrationNo,
                            report_type = p.ReportType,
                            report_date = p.ReportDate,
                            test_name = p.TestName,
                            opip = p.OPIP,
                            report_filename = p.ReportFileName,
                            ftp_path = p.FTPPath,
                            report_id = p.GROUP_ReportId,
                            test_id = p.GROUP_TestId
                        });
                    });
                }

                return _listOfPatientDiagnosis;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<PateintTests_New_V4> GetPatientTestResultsNew_V4(string lang, int hospitalId, int resgistrationNo, ref int Er_Status, ref string Msg, string ApiSources = "MobileApp", string EpisodeType = "OP", int EpisodeID = 0)
        {
            try
            {
                List<PateintTests_New_V4> _listOfPatientDiagnosis = new List<PateintTests_New_V4>();
                DB.param = new SqlParameter[]
                {
                    new SqlParameter("@BranchId", hospitalId),
                    new SqlParameter("@RegistrationNo ", resgistrationNo),
                    new SqlParameter("@status", SqlDbType.Int),
                    new SqlParameter("@msg", SqlDbType.NVarChar, 500),
                    new SqlParameter("@EpisodeType", EpisodeType),
                    new SqlParameter("@EpisodeId", EpisodeID)
                };
                DB.param[2].Direction = ParameterDirection.Output;
                DB.param[3].Direction = ParameterDirection.Output;

                string DB_SP_Name = "[dbo].[Get_PatientLABnRADResults_V2_SP]";

                if (ApiSources.ToLower() == "saleforce")
                    DB_SP_Name = "[SF].[Get_PatientLABnRADResults_V2_SP]";

                var _patientTestModel = DB.ExecuteSPAndReturnDataTable(DB_SP_Name).ToListObject<PateintTestsModel>();

                _listOfPatientDiagnosis = MapPatientTestResultsModelToPatientTestResults_V4(_patientTestModel);

                Er_Status = Convert.ToInt32(DB.param[2].Value);
                Msg = DB.param[3].Value.ToString();

                return _listOfPatientDiagnosis;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private List<PateintTests_New_V4> MapPatientTestResultsModelToPatientTestResults_V4(List<PateintTestsModel> _patientDaignosisModel)
        {
            try
            {
                List<PateintTests_New_V4> _listOfPatientDiagnosis = new List<PateintTests_New_V4>();

                if (_patientDaignosisModel != null && _patientDaignosisModel.Count > 0)
                {
                    _patientDaignosisModel.ForEach(p =>
                    {
                        _listOfPatientDiagnosis.Add(new PateintTests_New_V4()
                        {
                            registration_no = p.RegistrationNo.ToString(),
                            report_type = p.ReportType,
                            report_date = p.ReportDate,
                            test_name = p.TestName,
                            opip = p.OPIP,
                            report_filename = p.ReportFileName,
                            ftp_path = p.FTPPath,
                            report_id = p.GROUP_ReportId.ToString(),
                            test_id = p.GROUP_TestId.ToString()
                        });
                    });
                }

                return _listOfPatientDiagnosis;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private List<Reservation> MapVidCallAppointsModelToVidCallReservations(List<ReservationModel> patientVideoCallAppointments)
        {
            List<Reservation> _reservationList = new List<Reservation>();
            foreach (var row in patientVideoCallAppointments)
            {
                Reservation _reservation = new Reservation();

                _reservation.clinic_name = row.ClinicName;
                _reservation.date = row.AppDate.ToString("yyyy-MM-dd");
                _reservation.id = row.Id;
                _reservation.patient_attend = row.PatientVisited;
                _reservation.patient_name = row.PatientName;
                _reservation.physician_name = row.DoctorName;
                _reservation.time_from = row.AppTime.ToString();
                _reservation.video_call_url = row.VideoCallUrl;
                _reservation.doctor_id = row.DoctorId;
                _reservation.paid = row.Paid;
                _reservation.appointment_type = row.AppointmentType;
                _reservation.appointment_no = row.AppointmentNo;
                _reservation.registration_no = row.RegistrationNo;
                _reservation.branch_id = row.Branch_Id;
                _reservation.duration = row.Duration;
                _reservation.time_zone = row.TimeZone;
                _reservation.nationional_id = row.NationionalId;
                _reservation.end_date = row.EndDate.ToString("yyyy-MM-dd");
                _reservation.end_time = row.EndTime.ToString();
                _reservation.host2 = row.host2;
                _reservation.autoJoin2 = row.autoJoin2;
                _reservation.resourceId2 = row.resourceId2;
                _reservation.displayName2 = row.displayName2;
                _reservation.token2 = row.token2;
                _reservation.active = row.IsActive;

                _reservationList.Add(_reservation);
            }
            return _reservationList;
        }

        public DataTable GetDependentPatientDataTable(int hospitalId, string nationalId,int registrationNo,string relation, ref int erStatus, ref string msg)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@NationalId", nationalId),
                new SqlParameter("@RegistrationNo", registrationNo),
                new SqlParameter("@Relation", relation),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200)
            };

            var dt = DB.ExecuteSPAndReturnDataTable("DBO.[Get_DependentPatientDetails_SP]");
            return dt;

        }

        public DataTable GetPatientAppointmentList(string lang, int hospitalID, int RegistrationNo,string ApiSources = "MobileApp")
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalID),                    
                    new SqlParameter("@RegistrationNo", RegistrationNo)
                };


            string DB_SP_Name = "DBO.Get_PatientAppointments_V2_SP";

            if (ApiSources.ToLower() == "saleforce")
                DB_SP_Name = "SF.Get_PatientAppointments_V2_SP";


            var allClinicsDt
                = DB.ExecuteSPAndReturnDataTable(DB_SP_Name);

            return allClinicsDt;

        }

        public DataTable GetPatientCancelledAppointmentList(string lang, int hospitalID, int RegistrationNo)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalID),
                    new SqlParameter("@RegistrationNo", RegistrationNo)
                };

            var allDataDt
                = DB.ExecuteSPAndReturnDataTable("DBO.[Get_PatientCancelledAppointments_SP]");

            return allDataDt;

        }

        public DataTable GetPatientVisits(string lang, int hospitalID, int RegistrationNo , string ApiSources = "MobileApp")
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalID),
                    new SqlParameter("@RegistrationNo", RegistrationNo)
                };


            string DB_SP_Name = "DBO.Get_PatientVisits_SP";

            if (ApiSources.ToLower() == "saleforce")
                DB_SP_Name = "SF.Get_PatientVisits_SP";


            var allClinicsDt
                = DB.ExecuteSPAndReturnDataTable(DB_SP_Name);

            return allClinicsDt;

        }
        public DataTable GetPatientPrescriptionDT(string lang, int hospitalId, int resgistrationNo, ref int Er_Status, ref string Msg , string ApiSources = "MobileApp" , int EpisodeId = 0, string EpisodeType = "OP")
        {
            try
            {                
                DB.param = new SqlParameter[]
                {
                    new SqlParameter("@BranchId", hospitalId),
                    new SqlParameter("@RegistrationNo", resgistrationNo),
                    new SqlParameter("@status", SqlDbType.Int),
                    new SqlParameter("@msg", SqlDbType.NVarChar, 500),
                    new SqlParameter("@EpisodeType", EpisodeType),
                    new SqlParameter("@EpisodeId", EpisodeId)
                    
                };
                DB.param[2].Direction = ParameterDirection.Output;
                DB.param[3].Direction = ParameterDirection.Output;



                string DB_SP_Name = "[DBO].[Get_PatientPrescription_SP]";

                if (ApiSources.ToLower() == "saleforce")
                    DB_SP_Name = "[SF].[Get_PatientPrescription_SP]";

                var _patientPrescriptionModel = DB.ExecuteSPAndReturnDataTable(DB_SP_Name);

                Er_Status = Convert.ToInt32(DB.param[2].Value);
                Msg = DB.param[3].Value.ToString();

                return _patientPrescriptionModel;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public DataTable GetPatient_RefillPrescriptionDT(string lang, int hospitalId, string resgistrationNo, ref int Er_Status, ref string Msg, string ApiSources = "MobileApp", int EpisodeId = 0, string EpisodeType = "OP")
        {
            try
            {
                DB.param = new SqlParameter[]
                {
                    new SqlParameter("@BranchId", hospitalId),
                    new SqlParameter("@RegistrationNo", resgistrationNo),
                    new SqlParameter("@status", SqlDbType.Int),
                    new SqlParameter("@msg", SqlDbType.NVarChar, 500),
                    new SqlParameter("@EpisodeType", EpisodeType),
                    new SqlParameter("@EpisodeId", EpisodeId)

                };
                DB.param[2].Direction = ParameterDirection.Output;
                DB.param[3].Direction = ParameterDirection.Output;



                string DB_SP_Name = "[DBO].[GetPatientPrescriptions_Refill_List]";

                //if (ApiSources.ToLower() == "saleforce")
                //    DB_SP_Name = "[SF].[Get_PatientPrescription_SP]";

                var _patientPrescriptionModel = DB.ExecuteSPAndReturnDataTable(DB_SP_Name);

                Er_Status = Convert.ToInt32(DB.param[2].Value);
                Msg = DB.param[3].Value.ToString();

                return _patientPrescriptionModel;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public DataTable GetPatient_RefillRequestDT(string lang, int hospitalId, string resgistrationNo, ref int Er_Status, ref string Msg, string ApiSources = "MobileApp", int EpisodeId = 0, string EpisodeType = "OP")
        {
            try
            {
                DB.param = new SqlParameter[]
                {
                    new SqlParameter("@BranchId", hospitalId),
                    new SqlParameter("@RegistrationNo", resgistrationNo)
                };

                string DB_SP_Name = "[DBO].[GetPatientPrescriptions_Refill_Request_List]";

                //if (ApiSources.ToLower() == "saleforce")
                //    DB_SP_Name = "[SF].[Get_PatientPrescription_SP]";

                var _patientPrescriptionModel = DB.ExecuteSPAndReturnDataTable(DB_SP_Name);

                Er_Status = 1;
                Msg = "";

                return _patientPrescriptionModel;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public DataTable Save_Patient_RefillRequest(int hospitalId, string resgistrationNo
             ,string RowIDs            
            ,ref int Er_Status
            ,ref string Msg
            ,string ApiSources = "MobileApp")
        {
            try
            {
                DB.param = new SqlParameter[]
                {
                    new SqlParameter("@BranchId", hospitalId),
                    new SqlParameter("@Registration_No", resgistrationNo),
                    new SqlParameter("@RowIDs", RowIDs),                   
                    new SqlParameter("@status", SqlDbType.Int),
                    new SqlParameter("@msg", SqlDbType.NVarChar, 500),
                };
                DB.param[3].Direction = ParameterDirection.Output;
                DB.param[4].Direction = ParameterDirection.Output;

                //DB.ExecuteNonQuerySP("dbo.SAVE_Agreement_Acceptance_SP");

                string DB_SP_Name = "[DBO].[Save_PatientPrescriptions_Refill_Request_SP]";

                

                //if (ApiSources.ToLower() == "saleforce")
                //    DB_SP_Name = "[SF].[Get_PatientPrescription_SP]";

                var _patientPrescriptionModel = DB.ExecuteSPAndReturnDataTable(DB_SP_Name);

                Er_Status = Convert.ToInt32(DB.param[3].Value);
                if (DB.param[4].Value != null)
                    Msg = DB.param[4].Value.ToString();

                //Er_Status = 1;
                //Msg = "";

                return _patientPrescriptionModel;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public DataTable GetPatientDataDT(string lang, int hospitalId, int resgistrationNo, ref int Er_Status, ref string Msg)
        {
            try
            {
                DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalId),
                    new SqlParameter("@RegistrationNo ", resgistrationNo),
                    new SqlParameter("@Er_Status", SqlDbType.Int),
                    new SqlParameter("@Msg", SqlDbType.NVarChar, 200)
                };
                DB.param[3].Direction = ParameterDirection.Output;
                DB.param[4].Direction = ParameterDirection.Output;

                var _patientPrescriptionModel = DB.ExecuteSPAndReturnDataTable("[DBO].[Get_PatientData_SP]");

                Er_Status = Convert.ToInt32(DB.param[3].Value);
                Msg = DB.param[4].Value.ToString();

                return _patientPrescriptionModel;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //public DataTable GetPatientDataDT_V2(string lang, int hospitalId, int resgistrationNo, ref int Er_Status, ref string Msg)
        //{
        //    try
        //    {
        //        DB.param = new SqlParameter[]
        //        {
        //            new SqlParameter("@Lang", lang),
        //            new SqlParameter("@BranchId", hospitalId),
        //            new SqlParameter("@RegistrationNo ", resgistrationNo),
        //            new SqlParameter("@Er_Status", SqlDbType.Int),
        //            new SqlParameter("@Msg", SqlDbType.NVarChar, 200)
        //        };
        //        DB.param[3].Direction = ParameterDirection.Output;
        //        DB.param[4].Direction = ParameterDirection.Output;

        //        var _patientPrescriptionModel = DB.ExecuteSPAndReturnDataTable("[DBO].[Get_PatientData_V2_SP]");

        //        Er_Status = Convert.ToInt32(DB.param[3].Value);
        //        Msg = DB.param[4].Value.ToString();

        //        return _patientPrescriptionModel;

        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        throw;
        //    }
        //}

        public DataTable GetPatientFamily_List(string Lang, int hospitalId, string pCellNo, string nationalId, int registrationNo, ref int erStatus, ref string msg)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@PcellNo", pCellNo),
                new SqlParameter("@NationalId", nationalId),
                new SqlParameter("@RegistrationNo", registrationNo),
                new SqlParameter("@Er_Status", SqlDbType.Int),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 200),
                
            };
            DB.param[5].Direction = ParameterDirection.Output;
            DB.param[6].Direction = ParameterDirection.Output;            

            var dt = DB.ExecuteSPAndReturnDataTable("DBO.[Get_PatientFamily_SP]");

            erStatus = Convert.ToInt32(DB.param[5].Value);
            msg = DB.param[6].Value.ToString();            
            return dt;
        }


        public DataTable GetPatientSearch_List(string Lang, int hospitalId, string pCellNo, string nationalId, int registrationNo, ref int erStatus, ref string msg , string ApiSources = "MobileApp")
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@PcellNo", pCellNo),
                new SqlParameter("@NationalId", nationalId),
                new SqlParameter("@RegistrationNo", registrationNo),
                new SqlParameter("@Er_Status", SqlDbType.Int),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 200),

            };
            DB.param[5].Direction = ParameterDirection.Output;
            DB.param[6].Direction = ParameterDirection.Output;


            string DB_SP_Name = "DBO.[Patient_Serach_SP]";

            if (ApiSources.ToLower() == "saleforce")
                DB_SP_Name = "SF.[Patient_Serach_SP]";


            var dt = DB.ExecuteSPAndReturnDataTable(DB_SP_Name);

            erStatus = Convert.ToInt32(DB.param[5].Value);
            msg = DB.param[6].Value.ToString();
            return dt;
        }


        public string VerifyOTP_MobileOnly(string patientPhone, string VerificationCode)
        {

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@CellNo", patientPhone),
                new SqlParameter("@VerificationCode", VerificationCode),
            };

            DataTable dt = DB.ExecuteSPAndReturnDataTable("dbo.Get_OTPVerified_Mobile_SP");

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0].ItemArray.GetValue(0).ToString();
            }
            else
            {
                return null;
            }
        }

        public DataTable GetPatientList_ByMobile(string Lang, string pCellNo, int hospitalId, ref int erStatus, ref string msg)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@PcellNo", pCellNo),                
                new SqlParameter("@Er_Status", SqlDbType.Int),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 200),

            };
            DB.param[3].Direction = ParameterDirection.Output;
            DB.param[4].Direction = ParameterDirection.Output;

            var dt = DB.ExecuteSPAndReturnDataTable("DBO.[Get_PatientList_ByMobile_SP]");

            erStatus = Convert.ToInt32(DB.param[3].Value);
            msg = DB.param[4].Value.ToString();
            return dt;
        }

        public DataTable GetPatientOrder_List(string Lang, int hospitalId, int registrationNo, int visitID,string EpisodeType = "OP")
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@VisitId", visitID),                
                new SqlParameter("@RegistrationNo", registrationNo),
                new SqlParameter("@EpisodeType", EpisodeType)
            };
            

            var dt = DB.ExecuteSPAndReturnDataTable("DBO.[Get_PatientOrders_SP]");
            return dt;
        }


        //public DataTable GetPatientServiceOrder_List(int BranchId, int AppointmentID, string BillType, int OperatorID, string Service_ids, string item_Ids, string department_ids, ref int errStatus, ref string errMessage)
        public DataTable GetPatientServiceOrder_List(string Lang, int hospitalId, int registrationNo, int visitID, string EpisodeType , ref int errStatus , ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@VisitId", visitID),
                new SqlParameter("@EpisodeType", EpisodeType),
                new SqlParameter("@MRN", registrationNo),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 1000)
            };
            DB.param[5].Direction = ParameterDirection.Output;
            DB.param[6].Direction = ParameterDirection.Output;

            var dataTable = DB.ExecuteSPAndReturnDataTable("dbo.Get_ServicesList_WithAmount_SP");
            errStatus = Convert.ToInt32(DB.param[5].Value);
            errMessage = DB.param[6].Value.ToString();

            return dataTable;
        }

        

        public void UpdatePatientData(int hospitalId, int registrationNo, DateTime DOB,int Gender, string pCellNo,int Marital_Status, int NationalityID, string Patient_Email,string Sources, ref int Status, ref string msg)
        {
            DB.param = new[]
            {
                new SqlParameter("@Branch_Id", hospitalId),
                new SqlParameter("@PPhone", pCellNo),
                new SqlParameter("@Patient_Id", registrationNo),
                new SqlParameter("@DOB", DOB),
                new SqlParameter("@Gender", Gender),
                new SqlParameter("@Marital", Marital_Status),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 200),
                new SqlParameter("@Email", Patient_Email),
                new SqlParameter("@Sources", Sources),
            };
            
            DB.param[6].Direction = ParameterDirection.Output;
            DB.param[7].Direction = ParameterDirection.Output;


            string DB_SP_Name = "DBO.[Update_PatientProfile_Mobile_SP]";

            if (Sources.ToLower() == "saleforce")
                DB_SP_Name = "SF.[Update_PatientProfile_Mobile_SP]";

            var userInfoModel = DB.ExecuteSPAndReturnDataTable(DB_SP_Name);
            
            Status = Convert.ToInt32(DB.param[6].Value);
            msg = DB.param[7].Value.ToString();

            return;
        }
        public List<PreDefineMedReport> GetPreDefineMedicalReports(string lang, int hospitalID, int RegistrationNo , int VisitId = 0)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalID),
                    new SqlParameter("@RegistrationNo", RegistrationNo),
                    new SqlParameter("@VisitId", VisitId)
                };

            var allReportList
                = DB.ExecuteSPAndReturnDataTable("DBO.[Get_MedicalReports_SP]").ToListObject< PreDefineMedReport>();

            // MAP
            //var MapReport = MapDefineMedicalReport(allReportList);


            return allReportList;
        }

        public DataTable GetPreDefineMedicalReports_Temp(string lang, int hospitalID, int RegistrationNo, int VisitId = 0)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalID),
                    new SqlParameter("@RegistrationNo", RegistrationNo),
                    new SqlParameter("@VisitId", VisitId),
                    new SqlParameter("@ForListing", "0")
                };

            var allReportList
                = DB.ExecuteSPAndReturnDataTable("DBO.[Get_MedicalReports_SP]");

            // MAP
            //var MapReport = MapDefineMedicalReport(allReportList);


            return allReportList;
        }


        public DataTable GetBookingPatientFamily_List(string Lang, int hospitalId, int registrationNo, int BookinghospitalId, ref int erStatus, ref string msg, string PPhone = null)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@RegistrationNo", registrationNo),
                new SqlParameter("@BookingBranchId", BookinghospitalId),
                new SqlParameter("@Er_Status", SqlDbType.Int),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 200),
                new SqlParameter("@BookingPhone", PPhone)
            };

            DB.param[4].Direction = ParameterDirection.Output;
            DB.param[5].Direction = ParameterDirection.Output;

            var dt = DB.ExecuteSPAndReturnDataTable("DBO.[Get_BookingFamilyPatients_SP]");

            erStatus = Convert.ToInt32(DB.param[4].Value);
            msg = DB.param[5].Value.ToString();
            return dt;
        }


        public DataTable GetPatientFamilyProfile_List(string Lang, int hospitalId, int registrationNo,string Source, ref int erStatus, ref string msg)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@RegistrationNo", registrationNo),
                new SqlParameter("@Source", Source),
                new SqlParameter("@Er_Status", SqlDbType.Int),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 1000),
            };
            DB.param[4].Direction = ParameterDirection.Output;
            DB.param[5].Direction = ParameterDirection.Output;


            var dt = DB.ExecuteSPAndReturnDataTable("DBO.[FamilyProfile_Switch_List_SP]");

            erStatus = Convert.ToInt32(DB.param[4].Value);
            msg = DB.param[5].Value.ToString();
            return dt;
        }
        public List<PatientFamilyList> GetPatientFamilyProfile_List_V3(string Lang, int hospitalId, int registrationNo, string Source, ref int erStatus, ref string msg)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@RegistrationNo", registrationNo),
                new SqlParameter("@Source", Source),
                new SqlParameter("@Er_Status", SqlDbType.Int),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 1000),
            };
            DB.param[4].Direction = ParameterDirection.Output;
            DB.param[5].Direction = ParameterDirection.Output;


            var dt = DB.ExecuteSPAndReturnDataTable("DBO.[FamilyProfile_Switch_List_SP]").ToListObject<PatientFamilyList>();

            erStatus = Convert.ToInt32(DB.param[4].Value);
            msg = DB.param[5].Value.ToString();
            return dt;
        }

        public List<PatientFamilyList> GetPatientFamilyProfile_List_V3_ByMobile(string Lang, string Phone, string Source, ref int erStatus, ref string msg)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@MobileNo", Phone),                
                new SqlParameter("@Source", Source),
                new SqlParameter("@Er_Status", SqlDbType.Int),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 1000),
            };
            DB.param[3].Direction = ParameterDirection.Output;
            DB.param[4].Direction = ParameterDirection.Output;

            var dt = DB.ExecuteSPAndReturnDataTable("DBO.[FamilyProfile_Switch_List_ByMobile_SP]").ToListObject<PatientFamilyList>();

            erStatus = Convert.ToInt32(DB.param[3].Value);
            msg = DB.param[4].Value.ToString();
            return dt;
        }


        public DataTable Get_Patient_List_By_DateRange(string Lang, int hospitalId, DateTime StartDate, DateTime EndDate, ref int erStatus, ref string msg,string ApiSources="MobileApp")
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@StartDate", StartDate),
                new SqlParameter("@EndDate", EndDate),
                new SqlParameter("@Er_Status", SqlDbType.Int),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 200),
            };
            DB.param[4].Direction = ParameterDirection.Output;
            DB.param[5].Direction = ParameterDirection.Output;


            string DB_SP_Name = "DBO.[Get_ALLPatientList_ByDateRange_SP]";

            if (ApiSources.ToLower() == "saleforce")
                DB_SP_Name = "SF.[Get_ALLPatientList_ByDateRange_SP]";

            var dt = DB.ExecuteSPAndReturnDataTable(DB_SP_Name);

            erStatus = Convert.ToInt32(DB.param[4].Value);
            msg = DB.param[5].Value.ToString();
            return dt;
        }

        public DataTable Get_Patient_Updated_List_By_DateRange(string Lang, int hospitalId, DateTime StartDate, DateTime EndDate,DateTime UpdatedDate, ref int erStatus, ref string msg, string ApiSources = "MobileApp")
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@StartDate", StartDate),
                new SqlParameter("@EndDate", EndDate),
                new SqlParameter("@Er_Status", SqlDbType.Int),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 200),
                new SqlParameter("@UpdatedDate", UpdatedDate)
                
            };
            DB.param[4].Direction = ParameterDirection.Output;
            DB.param[5].Direction = ParameterDirection.Output;


            string DB_SP_Name = "DBO.[Get_ALLPatientList_Updated_ByDateRange_SP]";

            if (ApiSources.ToLower() == "saleforce")
                DB_SP_Name = "SF.[Get_ALLPatientList_Updated_ByDateRange_SP]";

            try
			{
                var dt = DB.ExecuteSPAndReturnDataTable(DB_SP_Name);

                erStatus = Convert.ToInt32(DB.param[4].Value);
                msg = DB.param[5].Value.ToString();
                return dt;
            }
            catch (Exception e)
			{
                erStatus = 0;
                msg = "Crashing";
                return null;
            }
            
            
        }


        public DataTable GetPatientBills(int hospitaId, int registrationNo, ref int errStatus, ref string errMessage, string EpisodeType = "OP" , int EpisodeID = 0)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchId ", hospitaId),
                new SqlParameter("@RegistrationNo ", registrationNo),
                new SqlParameter("@status",  SqlDbType.Int),
                new SqlParameter("@msg",  SqlDbType.NVarChar, 200),
                new SqlParameter("@EpisodeID",  EpisodeID),
                new SqlParameter("@EpisodeType",  EpisodeType )
            };
            DB.param[2].Direction = ParameterDirection.Output;
            DB.param[3].Direction = ParameterDirection.Output;

            var dt = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_Bills_SP]");

            errStatus = Convert.ToInt32(DB.param[2].Value);
            errMessage = DB.param[3].Value.ToString();

            return dt;
        }

        public void Save_PatientPwd (string Lang , int hospitaId, int registrationNo, string Patient_NationalID , string Patient_PWd, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@lang", Lang),
                new SqlParameter("@BranchId", hospitaId),
                new SqlParameter("@RegistrationNo", registrationNo),
                new SqlParameter("@NationalId", Patient_NationalID),
                new SqlParameter("@Pwd", Patient_PWd),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200)
            };
            DB.param[5].Direction = ParameterDirection.Output;
            DB.param[6].Direction = ParameterDirection.Output;

            var flag = DB.ExecuteSP("dbo.[Save_Update_Patient_UserPwd_SP]");

            errStatus = Convert.ToInt32(DB.param[5].Value);
            errMessage = DB.param[6].Value.ToString();
        }

        public void Save_PatientPwd_V4(string Lang, int hospitaId, string registrationNo, string Patient_NationalID, string Patient_PWd, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@lang", Lang),
                new SqlParameter("@BranchId", hospitaId),
                new SqlParameter("@RegistrationNo", registrationNo),
                new SqlParameter("@NationalId", Patient_NationalID),
                new SqlParameter("@Pwd", Patient_PWd),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200)
            };
            DB.param[5].Direction = ParameterDirection.Output;
            DB.param[6].Direction = ParameterDirection.Output;

            var flag = DB.ExecuteSP("dbo.[Save_Update_Patient_UserPwd_SP]");

            errStatus = Convert.ToInt32(DB.param[5].Value);
            errMessage = DB.param[6].Value.ToString();
        }

        public void Save_PatientPwd_NewForUAE(string Lang, int hospitaId, string registrationNo, string Patient_NationalID, string Patient_PWd, ref int errStatus, ref string errMessage)
        {
            try
			{
                DB.param = new SqlParameter[]
            {
                new SqlParameter("@lang", Lang),
                new SqlParameter("@BranchId", hospitaId),
                new SqlParameter("@RegistrationNo", registrationNo),
                new SqlParameter("@NationalId", Patient_NationalID),
                new SqlParameter("@Pwd", Patient_PWd),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200)
            };
                DB.param[5].Direction = ParameterDirection.Output;
                DB.param[6].Direction = ParameterDirection.Output;

                var flag = DB.ExecuteSP("dbo.[Save_Update_Patient_UserPwd_V2_SP]");

                errStatus = Convert.ToInt32(DB.param[5].Value);
                errMessage = DB.param[6].Value.ToString();
            }
            catch(Exception ex)
			{

			}
            
        }
        


        public DataTable GetAllAppointmentList(string lang, int hospitalID, DateTime Todate, DateTime FromDate , int RegistrationID)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalID),
                    new SqlParameter("@FromDate", FromDate),
                    new SqlParameter("@ToDate", Todate),
                    new SqlParameter("@RegistrationNo", RegistrationID)                    
                };

            var allDt
                = DB.ExecuteSPAndReturnDataTable("DBO.[Get_FutureAppointments_SP]");

            return allDt;

        }

        public void AddPatient_Log (int BranchID, int MRR, string SSN,string FirstName, string LastName,string MobileNumber , string Sources)
        {
            try
            {
                DB.param = new SqlParameter[]
                {
                    new SqlParameter("@BranchID", BranchID),
                    new SqlParameter("@MRR", MRR),
                    new SqlParameter("@SSN", SSN),
                    new SqlParameter("@FirstName", FirstName),
                    new SqlParameter("@LastName", LastName),
                    new SqlParameter("@MobileNumber", MobileNumber),
                    new SqlParameter("@Sources", Sources)
                };
                var flag = DB.ExecuteSP("[APILOG].[INSERT_ADD_PATIENT_Log]");
            }
            catch(Exception ex)
            {

            }
                        
        }

        public void AddPatient_Log_newUAE(int BranchID, string MRR, string SSN, string FirstName, string LastName, string MobileNumber, string Sources)
        {
            try
            {
                DB.param = new SqlParameter[]
                {
                    new SqlParameter("@BranchID", BranchID),
                    new SqlParameter("@MRR", MRR),
                    new SqlParameter("@SSN", SSN),
                    new SqlParameter("@FirstName", FirstName),
                    new SqlParameter("@LastName", LastName),
                    new SqlParameter("@MobileNumber", MobileNumber),
                    new SqlParameter("@Sources", Sources)
                };
                var flag = DB.ExecuteSP("[APILOG].[INSERT_ADD_PATIENT_Log]");
            }
            catch (Exception ex)
            {

            }

        }

        public string GetBranchName(int BranchID)
        {
            var sql = "select BranchName  from BAS_Branch_TB where HIS_Id = " + BranchID;
            var BranchNAME = DB.ExecuteSQLScalar(sql);
            return BranchNAME;
        }


        public void Save_VideoCallDetails (string AppointmentID , int BranchID, string ChannelId,string Tokken, DateTime StartTime, DateTime EndTime, string PatientMRN, int DoctorID, int ClinicID, string URL ,string Sources,string APPID,DateTime AppointmentTimeFrom, DateTime AppointmentTimeTo, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@AppointmentID", AppointmentID),
                    new SqlParameter("@BranchID", BranchID),
                    new SqlParameter("@ChannelId", ChannelId),
                    new SqlParameter("@Tokken", Tokken),
                    new SqlParameter("@StartTime", StartTime),
                    new SqlParameter("@EndTime", EndTime),
                    new SqlParameter("@PatientMRN", PatientMRN),
                    new SqlParameter("@DoctorID", DoctorID),
                    new SqlParameter("@ClinicID", ClinicID),                    
                    new SqlParameter("@URL", URL),
                    new SqlParameter("@Sources", Sources),
                    new SqlParameter("@APPID", APPID),
                    new SqlParameter("@AppointmentTimeFrom", AppointmentTimeFrom),
                    new SqlParameter("@AppointmentTimeTo", AppointmentTimeTo),                    
                    new SqlParameter("@status", SqlDbType.Int),
                    new SqlParameter("@msg", SqlDbType.NVarChar, 200)
                };

            DB.param[14].Direction = ParameterDirection.Output;
            DB.param[15].Direction = ParameterDirection.Output;

            var flag = DB.ExecuteSP("[dbo].[SAVE_VideoCall_Details_SP]");
            
            errStatus = Convert.ToInt32(DB.param[14].Value);
            errMessage = DB.param[15].Value.ToString();




        }

        public DataTable GET_VideoCallDetails(string Lang, string AppointmentID, int BranchID, string PatientMRN, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", Lang),
                    new SqlParameter("@AppointmentID", AppointmentID),
                    new SqlParameter("@BranchID", BranchID),
                    new SqlParameter("@PatientMRN", PatientMRN),                    
                    new SqlParameter("@status", SqlDbType.Int),
                    new SqlParameter("@msg", SqlDbType.NVarChar, 1000)
                };

            DB.param[4].Direction = ParameterDirection.Output;
            DB.param[5].Direction = ParameterDirection.Output;

            var ReturnDataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_VideoCallDetails_SP]");

            errStatus = Convert.ToInt32(DB.param[4].Value);
            errMessage = DB.param[5].Value.ToString();

            return ReturnDataTable;

        }

        public DataTable GET_VideoCallDetails_V3(string Lang, string AppointmentID, int BranchID, string PatientMRN, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", Lang),
                    new SqlParameter("@AppointmentID", AppointmentID),
                    new SqlParameter("@BranchID", BranchID),
                    new SqlParameter("@PatientMRN", PatientMRN),
                    new SqlParameter("@status", SqlDbType.Int),
                    new SqlParameter("@msg", SqlDbType.NVarChar, 1000)
                };

            DB.param[4].Direction = ParameterDirection.Output;
            DB.param[5].Direction = ParameterDirection.Output;

            var ReturnDataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_VideoCallDetails_UAE_Zoom_SP]");

            errStatus = Convert.ToInt32(DB.param[4].Value);
            errMessage = DB.param[5].Value.ToString();

            return ReturnDataTable;

        }

        public void UPDATE_VideoCallPatientJoin(string AppointmentID, int BranchID, string JoinBy)
        {
            DB.param = new SqlParameter[]
                {   
                    new SqlParameter("@AppointmentID", AppointmentID),
                    new SqlParameter("@BranchID", BranchID),                    
                    new SqlParameter("@JoinBy", JoinBy)
                };            

            DB.ExecuteSP("[dbo].[Update_VideoCall_Join_SP]");
        }

        public DataTable GET_FoodAllergyList_ByPatient (string Lang, int PatientMRN, int BranchID, string Source,string EpisodeType = "OP", int EpisodeID = 0)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", Lang),
                    new SqlParameter("@RegistrationNo", PatientMRN),
                    new SqlParameter("@BranchID", BranchID),
                    new SqlParameter("@Source", Source),
                    new SqlParameter("@EpisodeType", EpisodeType),
                    new SqlParameter("@EpisodeId", EpisodeID)                    
                };
            
            var ReturnDataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_AllergyList_Food_Patient_SP]");
            return ReturnDataTable;
        }

        public List<AllergyList> GET_FoodAllergyList_ByPatient_V3(string Lang, int PatientMRN, int BranchID, string Source, string EpisodeType = "OP", int EpisodeID = 0)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", Lang),
                    new SqlParameter("@RegistrationNo", PatientMRN),
                    new SqlParameter("@BranchID", BranchID),
                    new SqlParameter("@Source", Source),
                    new SqlParameter("@EpisodeType", EpisodeType),
                    new SqlParameter("@EpisodeId", EpisodeID)
                };

            var ReturnDataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_AllergyList_Food_Patient_SP]").ToListObject<AllergyList>();
            return ReturnDataTable;
        }

        public void Save_FoodAllergyList_ByPatient(int PatientMRN, int BranchID,string FoodIDs, string Source, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
                {                    
                    new SqlParameter("@RegistrationNo", PatientMRN),
                    new SqlParameter("@BranchID", BranchID),                    
                    new SqlParameter("@FoodIds", FoodIDs),
                    new SqlParameter("@Source", Source),
                    new SqlParameter("@status", SqlDbType.Int),
                    new SqlParameter("@msg", SqlDbType.NVarChar, 1000)
                };

            DB.param[4].Direction = ParameterDirection.Output;
            DB.param[5].Direction = ParameterDirection.Output;

            DB.ExecuteNonQuerySP("[dbo].[Save_PatientFoodAllergy_SP]");

            errStatus = Convert.ToInt32(DB.param[4].Value);
            errMessage = DB.param[5].Value.ToString();

            return ;
        }


        public void Save_VideoCall_Postponed_byDoctor
            (int BranchID, int Appointmentid , int PatientMRN,int postponedmin 
            , int DoctorId,string DoctorName,string DepartmentName,string AppointmentDate
            ,string Source, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@AppointmentID", Appointmentid),
                    new SqlParameter("@BranchID", BranchID),
                    new SqlParameter("@DoctorId", DoctorId),
                    new SqlParameter("@DoctorName", DoctorName),
                    new SqlParameter("@DepartmentName", DepartmentName),
                    new SqlParameter("@PatientMRN", PatientMRN),
                    new SqlParameter("@AppointmentDate", AppointmentDate),
                    new SqlParameter("@postponed", postponedmin),
                    new SqlParameter("@Source", Source),
                    new SqlParameter("@status", SqlDbType.Int),
                    new SqlParameter("@msg", SqlDbType.NVarChar, 1000)
                    

                };

            DB.param[9].Direction = ParameterDirection.Output;
            DB.param[10].Direction = ParameterDirection.Output;

            DB.ExecuteNonQuerySP("[dbo].[SAVE_Appointment_Postponed_FCM_SP]");

            errStatus = Convert.ToInt32(DB.param[9].Value);
            errMessage = DB.param[10].Value.ToString();
        }


        public DataTable GetVidoCallDoctorStatusMsg(string lang, int hospitalID, int RegistrationNo,int AppointmentID, string ApiSources)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalID),
                    new SqlParameter("@RegistrationNo", RegistrationNo),
                    new SqlParameter("@AppointmentID", AppointmentID)
                };


            string DB_SP_Name = "DBO.Get_Doctor_VideoCall_Status_MSG_SP";

            var allDt
                = DB.ExecuteSPAndReturnDataTable(DB_SP_Name);

            return allDt;

        }




        public DataTable GET_Patient_VitalSign(string Lang, int PatientMRN, int BranchID, string Source, string EpisodeType = "OP", int EpisodeID = 0)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", Lang),
                    new SqlParameter("@RegistrationNo", PatientMRN),
                    new SqlParameter("@BranchID", BranchID),
                    new SqlParameter("@Source", Source),
                    new SqlParameter("@EpisodeType", EpisodeType),
                    new SqlParameter("@EpisodeId", EpisodeID),
                    new SqlParameter("@Er_Status", SqlDbType.Int),
                    new SqlParameter("@msg", SqlDbType.NVarChar, 1000)
                };
            DB.param[6].Direction = ParameterDirection.Output;
            DB.param[7].Direction = ParameterDirection.Output;

            var ReturnDataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_PatientVitals_SP]");
            return ReturnDataTable;
        }


        public DataTable GetPatientMissedAppointmentList(string lang, int hospitalID, int RegistrationNo)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalID),
                    new SqlParameter("@RegistrationNo", RegistrationNo)
                };

            var allDataDt
                = DB.ExecuteSPAndReturnDataTable("DBO.[Get_Notification_MissAppoitment_List_SP]");

            return allDataDt;

        }

        public DataTable GetPatientMissedAppointmentList_UAE(string lang, int hospitalID, int RegistrationNo , List<Get_Missed_Appointments_UAE> UAETable)
        {
            if (UAETable.Count > 0)
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Lang", typeof(string));
                dataTable.Columns.Add("Id", typeof(int));
                dataTable.Columns.Add("BranchID", typeof(int));
                dataTable.Columns.Add("apptDatetime", typeof(string));
                dataTable.Columns.Add("RegistrationNo", typeof(string));
                dataTable.Columns.Add("Doctor_ID", typeof(string));                
                
                foreach (var model in UAETable)
                {
                    dataTable.Rows.Add(lang, model.id, hospitalID
                        , model.appDateTime, model.registrationno, model.doctor_Id                        
                    );
                }

                var returnDataTable =  DB.ExecuteSP_With_DataTable_AndReturnDataTable("[dbo].[Get_MissAppoitment_List_UAE_SP]", "@UAEDoctors", dataTable);
                return returnDataTable;
            }
            return null;
        }


        
        public void  Save_AppoitmentLogs (int BranchID , string AppointmentID , string MRN,int OperatorId ,string AppoitmentStatus,
                                            string Sources , int Doctor_Id , int IsVideo = 0 , int BookingType = 0)
		{
            DB.param = new SqlParameter[]
                {                    
                    new SqlParameter("@BranchId", BranchID),
                    new SqlParameter("@AppointmentId", AppointmentID),
                    new SqlParameter("@RegistrationNo", MRN),
                    new SqlParameter("@OperatorId", OperatorId),
                    new SqlParameter("@AppoitmentStatus", AppoitmentStatus),
                    new SqlParameter("@Sources", Sources),
                    new SqlParameter("@Doctor_Id", Doctor_Id),
                    new SqlParameter("@IsVideo", IsVideo),
                    new SqlParameter("@BookingType", BookingType)

                };

            var allDataDt
                = DB.ExecuteSP ("[Logs].[Add_Appointment_Logs]");

            //

        }



        public DataTable GeT_InPatient_Visit(string Lang, string PatientMRN, int BranchID)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@RegistrationNo", PatientMRN),
                new SqlParameter("@BranchID", BranchID)                    
            };
            
            var ReturnDataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_InPatientVisit_SP]");
            return ReturnDataTable;
        }

        public bool Save_Patient_RegistrationData (
                int     BranchID,               string  PatientFirstName,       string  PatientMiddleName,
                string  PatientLastName,        string  PatientFamilyName,      string  PatientPhone,
                string  PatientEmail,           string  PatientNationalId,      string  PatientBirthday,
                int     PatientGender,          string  PatientAddress,         int     PatientNationalityId,
                int     PatientMaritalStatusId, string  IdExpiry,               int     IdType,
                string  CurrentCity,            int     CountryId,              string  Pwd,
                ref int errStatus,              ref string errMessage
            )
		{
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@BranchID", BranchID),
                    new SqlParameter("@PatientFirstName", PatientFirstName),
                    new SqlParameter("@PatientMiddleName", PatientMiddleName),
                    new SqlParameter("@PatientLastName", PatientLastName),
                    new SqlParameter("@PatientPhone", PatientPhone),
                    new SqlParameter("@PatientEmail",PatientEmail ),
                    new SqlParameter("@PatientNationalId", PatientNationalId),
                    new SqlParameter("@PatientBirthday", PatientBirthday),
                    new SqlParameter("@PatientGender",PatientGender ),
                    new SqlParameter("@PatientAddress", PatientAddress),
                    new SqlParameter("@PatientNationalityId", PatientNationalityId),
                    new SqlParameter("@PatientMaritalStatusId", PatientMaritalStatusId),
                    new SqlParameter("@IdExpiry", IdExpiry),
                    new SqlParameter("@IdType", IdType),
                    new SqlParameter("@CurrentCity", CurrentCity),
                    new SqlParameter("@CountryId", CountryId),
                    new SqlParameter("@status", SqlDbType.Int),
                    new SqlParameter("@msg", SqlDbType.NVarChar, 1000),
                    new SqlParameter("@PWD", Pwd)
                };
            DB.param[16].Direction = ParameterDirection.Output;
            DB.param[17].Direction = ParameterDirection.Output;

            var ReturnDataTable = DB.ExecuteSP("[dbo].[Save_Patient_Registraion_Data_MW_SP]");

            errStatus = Convert.ToInt32(DB.param[16].Value);
            errMessage = DB.param[17].Value.ToString();


            return ReturnDataTable;

        }


        public RegisterPatientUAE Add_PatientFile (string DataID)
		{
            DB.param = new SqlParameter[]
                {                    
                    new SqlParameter("@DataID", DataID)
                };
            
            var _patientModel = DB.ExecuteSPAndReturnDataTable("DBO.[Get_PatientRegistraionData_MW_SP]").ToModelObject<RegisterPatientUAE>();
            return _patientModel;
		}


    }
}
