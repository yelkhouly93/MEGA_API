using DataLayer.Common;
using DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;


namespace DataLayer.Data
{
#pragma warning disable 1591
    public class PhysicianDB
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");
        public List<Physician> GetAllPhsicians(string lang, int hospitalID, int clinicID, int pageno = -1, int pagesize = 10)
        {

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId", hospitalID),
                new SqlParameter("@DeptId", clinicID),
                new SqlParameter("@PageNo", pageno),
                new SqlParameter("@PageSize", pagesize)
            };

            var _allPhysiciansModel = new List<PhysicianModel>();
            List<Physician> _allPhysicians = new List<Physician>();


            _allPhysiciansModel = DB.ExecuteSPAndReturnDataTable("DBO.[Get_Doctors_SP]").ToListObject<PhysicianModel>();

            _allPhysicians = MapPhysicianModelToPhysician(_allPhysiciansModel);

            return _allPhysicians;

        }

        private List<Physician> MapPhysicianModelToPhysician(List<PhysicianModel> _allPhysiciansModel)
        {
            List<Physician> _allPhysicians = new List<Physician>();
            foreach (var physicainModel in _allPhysiciansModel)
            {
                Physician _physicain = new Physician();
                _physicain.about_physician = physicainModel.DoctorProfile;
                _physicain.address = physicainModel.DoctorAddress;
                _physicain.awards = null;
                _physicain.birthday = physicainModel.DOB;
                _physicain.certificates = physicainModel.DOCTORCERTIFICATES;
                //_physicain.clinic_services = physicainModel.CLINICSERVICES;
                _physicain.clinic_services = new List<Clinic_Services>();
                _physicain.clinic_services.Add(new Clinic_Services()
                {
                    desc = null,
                    name = null
                });
                _physicain.clinics = new List<Clinic>();
                _physicain.clinics.Add(new Clinic()
                {
                    name = physicainModel.CLINICS
                });
                _physicain.degree = physicainModel.DEGREE;
                //_physicain.equipments = physicainModel.EQUIPMENTS;
                _physicain.equipments = new List<Equipment>();
                _physicain.equipments.Add(new Equipment()
                {
                    desc = null,
                    name = null
                });
                _physicain.experience = physicainModel.EXPERIENCE;
                _physicain.experience_level = physicainModel.EXPERIENCELEVEL;
                _physicain.family_name = physicainModel.LASTNAME;
                _physicain.first_name = physicainModel.FIRSTNAME;
                _physicain.first_name = physicainModel.FIRSTNAME_AR;
                _physicain.full_name = physicainModel.FullName;
                _physicain.graduated_from = physicainModel.GRADUATEFROM;
                _physicain.graduation_year = physicainModel.GRADUATEYEAR;
                _physicain.id = physicainModel.ID;
                _physicain.image_url = physicainModel.IMAGEURL;
                _physicain.last_name = physicainModel.LASTNAME;
                _physicain.last_name_ar = physicainModel.FIRSTNAME_AR;
                _physicain.middle_name = physicainModel.MIDDLENAME;
                _physicain.mobile1 = physicainModel.MOBILE1;
                _physicain.mobile2 = physicainModel.MOBILE2;
                //_physicain.performed_operations = physicainModel.PERFORMEDOPERATIONS;
                _physicain.performed_operations = new List<Performed_Operations>();
                _physicain.performed_operations.Add(new Performed_Operations()
                {
                    desc = null,
                    name = null
                });
                _physicain.phone_number = physicainModel.PHONENUMBER;
                _physicain.specialities = new List<Speciality>();
                _physicain.specialities.Add(new Speciality()
                {
                    desc = physicainModel.SPECIALTY,
                    name = physicainModel.SPECIALTY
                });

                _allPhysicians.Add(_physicain);
            }

            return _allPhysicians;
        }



        public List<AvailableSlots> GetAvailableSlotsByPhysician(string lang, int hospitalID, int clinicID, int physicianID, DateTime selectedDate,string ApiSources="MobileApp", int isVideo = 0)
        {

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@Branch_Id", hospitalID),
                new SqlParameter("@DepartmentId", clinicID),
                new SqlParameter("@DoctorId", physicianID),
                new SqlParameter("@TDate", selectedDate)                
            };

            var _allAvailableSlotsModel = new List<AvailableSlotsModel>();
            List<AvailableSlots> _allAvailableSlots = new List<AvailableSlots>();

            string DB_SP_Name = "DBO.[Get_DoctorSchedules_SP]";

            if (ApiSources.ToLower() == "saleforce")
            {
                DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@Branch_Id", hospitalID),
                    new SqlParameter("@DepartmentId", clinicID),
                    new SqlParameter("@DoctorId", physicianID),
                    new SqlParameter("@TDate", selectedDate)
                };
                DB_SP_Name = "SF.[Get_DoctorSchedules_SP]";
            }
            else
            {
                DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@Branch_Id", hospitalID),
                    new SqlParameter("@DepartmentId", clinicID),
                    new SqlParameter("@DoctorId", physicianID),
                    new SqlParameter("@TDate", selectedDate),
                    new SqlParameter("@IsVideoAppointment", isVideo)
                    
                };
            }
                

            _allAvailableSlotsModel = DB.ExecuteSPAndReturnDataTable(DB_SP_Name).ToListObjectNew<AvailableSlotsModel>();

            _allAvailableSlots = MapAvailableSlotsModelToAvailableSlot(_allAvailableSlotsModel);

            return _allAvailableSlots;

        }

        private List<AvailableSlots> MapAvailableSlotsModelToAvailableSlot(List<AvailableSlotsModel> _allAvailableSlotsModel)
        {
            List<AvailableSlots> _allAvailableSlots = new List<AvailableSlots>();
            foreach (var availableSlotsModel in _allAvailableSlotsModel)
            {
                AvailableSlots _availableSlot = new AvailableSlots();
                _availableSlot.Id = availableSlotsModel.Id;
                _availableSlot.time_to = availableSlotsModel.ToTime.ToString("HH:mm:ss");
                _availableSlot.time_from = availableSlotsModel.FromTime.ToString("HH:mm:ss");
                _availableSlot.slot_type_id = availableSlotsModel.SlotTypeId;
                _availableSlot.slot_type_name = availableSlotsModel.SlotTypeName;
                _availableSlot.Doctor_ClinicTime = availableSlotsModel.DoctorClinicTime;

                _allAvailableSlots.Add(_availableSlot);

            }

            return _allAvailableSlots;

        }

        //public List<AvailableDays> GetAvailableDaysByPhysician(string lang, int hospitalID, int clinicID, int physicianID, DateTime selectedDate, int pageno = -1, int pagesize = 10)
        //{

        //    try
        //    {
        //        DB.param = new SqlParameter[]
        //            {
        //        new SqlParameter("@Lang", lang),
        //        new SqlParameter("@Branch_Id", hospitalID),
        //        new SqlParameter("@DepartmentId", clinicID),
        //        new SqlParameter("@DoctorId", physicianID),
        //        new SqlParameter("@TDate", selectedDate),
        //        new SqlParameter("@DatesOnly", 1),
        //        new SqlParameter("@PageNo", pageno),
        //        new SqlParameter("@PageSize", pagesize)
        //            };

        //        var _allAvailableSlotsModel = new List<AvailableSlotsModel>();
        //        List<AvailableDays> _allAvailableSlots = new List<AvailableDays>();

        //        _allAvailableSlotsModel = DB.ExecuteSPAndReturnDataTable("DBO.[Get_DoctorSchedules_SP]").ToListObjectNew<AvailableSlotsModel>();


        //        _allAvailableSlots = MapAvailableSlotsModelToAvailableDays(_allAvailableSlotsModel);

        //        return _allAvailableSlots;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }

        //}


        public DataTable GetAvailableOnlyDayOfDoctors_TEMPTEST(string lang, int hospitalID, int clinicID, int physicianID, string selectedDate, string ApiSources = "MobileApp", int pageno = -1, int pagesize = 10)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@Branch_Id", hospitalID),
                new SqlParameter("@DepartmentId", clinicID),
                new SqlParameter("@DoctorId", physicianID),
                new SqlParameter("@TDate", selectedDate),
                new SqlParameter("@DatesOnly", 1),
                new SqlParameter("@ThisDateOnly", 1),
                new SqlParameter("@PageNo", pageno),
                new SqlParameter("@PageSize", pagesize)
            };

            string DB_SP_Name = "DBO.[Get_DoctorSchedules_SPv3]";

            if (ApiSources.ToLower() == "saleforce")
                DB_SP_Name = "SF.[Get_DoctorSchedules_SPv3]";

            var allAvailableSlots = DB.ExecuteSPAndReturnDataTable(DB_SP_Name);
            return allAvailableSlots;

        }


        public DataTable GetAvailableOnlyDayOfDoctors(string lang, int hospitalID, int clinicID, int physicianID, DateTime selectedDate,string ApiSources="MobileApp", int pageno = -1, int pagesize = 10)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@Branch_Id", hospitalID),
                new SqlParameter("@DepartmentId", clinicID),
                new SqlParameter("@DoctorId", physicianID),
                new SqlParameter("@TDate", selectedDate),
                new SqlParameter("@DatesOnly", 1),
                new SqlParameter("@ThisDateOnly", 1),
                new SqlParameter("@PageNo", pageno),
                new SqlParameter("@PageSize", pagesize)
            };

            string DB_SP_Name = "DBO.[Get_DoctorSchedules_SPv3]";

            if (ApiSources.ToLower() == "saleforce")
                DB_SP_Name = "SF.[Get_DoctorSchedules_SPv3]";

            var allAvailableSlots = DB.ExecuteSPAndReturnDataTable(DB_SP_Name);
            return allAvailableSlots;
            
        }

        public DataTable GetAvailableDaysOfDoctors(string lang, int hospitalID, int clinicID, int physicianID, DateTime selectedDate,string ApiSources = "MobileApp", int IsVideo = 0 )
        {
            

            string DB_SP_Name = "DBO.[Get_DoctorSchedules_SPv2]";
            if (ApiSources.ToLower() == "saleforce")
            {
                DB_SP_Name = "SF.[Get_DoctorSchedules_SPv2]";
                DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@Branch_Id", hospitalID),
                    new SqlParameter("@DepartmentId", clinicID),
                    new SqlParameter("@DoctorId", physicianID),
                    new SqlParameter("@TDate", selectedDate),
                    new SqlParameter("@DatesOnly", 1)
                };
            }
            else
            {
                DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@Branch_Id", hospitalID),
                new SqlParameter("@DepartmentId", clinicID),
                new SqlParameter("@DoctorId", physicianID),
                new SqlParameter("@TDate", selectedDate),
                new SqlParameter("@DatesOnly", 1),
                new SqlParameter("@IsVideoAppointment", IsVideo)
            };
            }
                

            var allAvailableSlots = DB.ExecuteSPAndReturnDataTable(DB_SP_Name);
            return allAvailableSlots;
            
        }

        private List<AvailableDays> MapAvailableSlotsModelToAvailableDays(List<AvailableSlotsModel> _allAvailableSlotsModel)
        {            
            List<AvailableDays> _allAvailableSlots = new List<AvailableDays>();
            foreach (var availableSlotsModel in _allAvailableSlotsModel)
            {
                AvailableDays _availableSlot = new AvailableDays();
                //_availableSlot.Id = i++;
                _availableSlot.Id = availableSlotsModel.Id;
                _availableSlot.doctor_id = availableSlotsModel.DoctorId;
                _availableSlot.schedule_day = availableSlotsModel.ScheduledDay.Date;//.ToShortDateString();

                _allAvailableSlots.Add(_availableSlot);

            }

            return _allAvailableSlots;

        }

        public List<AvailableDays2> GetAvailableDaysByPhysician2(string lang, int hospitalID, int clinicID, int physicianID, DateTime selectedDate, int pageno = -1, int pagesize = 10)
        {

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@Branch_Id", hospitalID),
                new SqlParameter("@DepartmentId", clinicID),
                new SqlParameter("@DoctorId", physicianID),
                new SqlParameter("@TDate", selectedDate),
                new SqlParameter("@DatesOnly", 1),
                new SqlParameter("@PageNo", pageno),
                new SqlParameter("@PageSize", pagesize)
            };

            var _allAvailableSlotsModel = new List<AvailableSlotsModel>();
            List<AvailableDays2> _allAvailableSlots = new List<AvailableDays2>();

            _allAvailableSlotsModel = DB.ExecuteSPAndReturnDataTable("DBO.[Get_DoctorSchedules_SP]").ToListObject<AvailableSlotsModel>();

            _allAvailableSlots = MapAvailableSlotsModelToAvailableDays2(_allAvailableSlotsModel);

            return _allAvailableSlots;

        }

        private List<AvailableDays2> MapAvailableSlotsModelToAvailableDays2(List<AvailableSlotsModel> _allAvailableSlotsModel)
        {
            List<AvailableDays2> _allAvailableSlots = new List<AvailableDays2>();
            foreach (var availableSlotsModel in _allAvailableSlotsModel)
            {
                AvailableDays2 _availableSlot = new AvailableDays2();
                _availableSlot.Id = availableSlotsModel.Id;
                _availableSlot.doctor_id = availableSlotsModel.DoctorId;
                _availableSlot.schedule_day = availableSlotsModel.ScheduledDay.Date;//.ToShortDateString();
                _availableSlot.time_from = availableSlotsModel.FromTime.ToShortTimeString();
                _availableSlot.time_to = availableSlotsModel.ToTime.ToShortTimeString();

                _allAvailableSlots.Add(_availableSlot);

            }

            return _allAvailableSlots;

        }



        public ConsultationAmount GetConsultationAmount(string lang, int hospitaId, int patientId, int doctorId, int doctorScheduleId, string billType, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchId", hospitaId),
                new SqlParameter("@RegistrationNo", patientId),
                new SqlParameter("@DoctorId", doctorId),
                new SqlParameter("@BillType", billType),
                new SqlParameter("@DoctorScheduleId", doctorScheduleId),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 500)
            };

            DB.param[5].Direction = ParameterDirection.Output;
            DB.param[6].Direction = ParameterDirection.Output;

            var _consultationAmountModel = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_ConsultationCashAmount_SP]").ToModelObject<ConsultationAmountModel>();

            errStatus = Convert.ToInt32(DB.param[5].Value);
            errMessage = DB.param[6].Value.ToString();

            ConsultationAmount _consultationAmountObj = new ConsultationAmount();
            if (errStatus > 0 && _consultationAmountModel != null)
            {
                _consultationAmountObj.cash_amount = _consultationAmountModel.CashAmount;
                _consultationAmountObj.deductible_amount = _consultationAmountModel.DeductibleAmount;
            }

            return _consultationAmountObj;
        }



        public DataTable GetConsultationAmountdDataTable(string lang, int hospitaId, int patientId, int doctorId, int doctorScheduleId, string billType, ref int errStatus, ref string errMessage)

        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchId", hospitaId),
                new SqlParameter("@RegistrationNo", patientId),
                new SqlParameter("@DoctorId", doctorId),
                new SqlParameter("@BillType", billType),
                new SqlParameter("@DoctorScheduleId", doctorScheduleId),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 500)
            };

            DB.param[5].Direction = ParameterDirection.Output;
            DB.param[6].Direction = ParameterDirection.Output;

            var consultationAmountModel = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_ConsultationCashAmount_SP]");

            errStatus = Convert.ToInt32(DB.param[5].Value);
            errMessage = DB.param[6].Value.ToString();

            return consultationAmountModel;
        }


        public DataTable GetAllPhsiciansDataTable(string lang, int hospitalId, int clinicId, int pageno = -1, int pagesize = 10,bool MyDoctors =false , int RegistrationNo = 0)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@DeptId", clinicId),
                new SqlParameter("@PageNo", pageno),
                new SqlParameter("@PageSize", pagesize),
                new SqlParameter("@MyDoctors", MyDoctors),
                new SqlParameter("@RegistrationNo", RegistrationNo)
            };

            var allPhysiciansModel = DB.ExecuteSPAndReturnDataTable("DBO.[Get_Doctors_SPV2]");
            return allPhysiciansModel;

        }

        public DataTable GetPhsiciansAdvanceSearchDT(string lang, string hospitalId, string clinicId, string SpecialityName, string SubSpecialty, string AssistArea,string SpokenLanguage,string GeneralSearch, int pageno = -1, int pagesize = 10 , string ApiSources = "MobileApp" , int Isvideo = 0)
        {
            

            

            string DB_SP_Name = "DBO.[Get_DoctorsAdvanceSrch_SP]";

            if (ApiSources.ToLower() == "saleforce")
            {
                DB.param = new SqlParameter[]
               {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalId),
                    new SqlParameter("@DeptId", clinicId),
                    new SqlParameter("@DeptUniName", SpecialityName),
                    new SqlParameter("@SubSpecialty", SubSpecialty),
                    new SqlParameter("@AssistArea", AssistArea),
                    new SqlParameter("@SpokenLanguage", SpokenLanguage),
                    new SqlParameter("@CommonSearchParam", GeneralSearch),
                    new SqlParameter("@PageNo", pageno),
                    new SqlParameter("@PageSize", pagesize)
               };
                DB_SP_Name = "SF.[Get_DoctorsAdvanceSrch_SP]";
            }
            else
            {
                DB.param = new SqlParameter[]
               {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalId),
                    new SqlParameter("@DeptId", clinicId),
                    new SqlParameter("@DeptUniName", SpecialityName),
                    new SqlParameter("@SubSpecialty", SubSpecialty),
                    new SqlParameter("@AssistArea", AssistArea),
                    new SqlParameter("@SpokenLanguage", SpokenLanguage),
                    new SqlParameter("@CommonSearchParam", GeneralSearch),
                    new SqlParameter("@PageNo", pageno),
                    new SqlParameter("@PageSize", pagesize),
                    new SqlParameter("@VideoAppointmentAvailability", Isvideo)
               };
            }
                

            var allPhysiciansModel = DB.ExecuteSPAndReturnDataTable(DB_SP_Name);
            return allPhysiciansModel;

        }

        public DataTable GetPhsiciansBySpecialityDataTable(string lang, string hospitalId, string clinicId, string SpecialityName, int pageno = -1, int pagesize = 10,string ApiSources = "MobileApp")
        {
            DB.param = new SqlParameter[]
           {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@DeptId", clinicId),
                new SqlParameter("@DeptUniName", SpecialityName),
                new SqlParameter("@PageNo", pageno),
                new SqlParameter("@PageSize", pagesize)
           };

            string DB_SP_Name = "DBO.[Get_DoctorsBySpecialty_SP]";

            if (ApiSources.ToLower() == "saleforce")
                DB_SP_Name = "SF.[Get_DoctorsBySpecialty_SP]";

            var allPhysiciansModel = DB.ExecuteSPAndReturnDataTable(DB_SP_Name);
            return allPhysiciansModel;

        }

        public DoctorsDataList GetAllDoctorsProfile(string lang, string hospitalId, string clinicId,string SpecialityName,string DoctorID, int pageno = -1, int pagesize = 10 , string ApiSources = "MobileApp")
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId", hospitalId),
                new SqlParameter("@DeptId", clinicId),
                new SqlParameter("@DeptUniName", SpecialityName),
                new SqlParameter("@DoctorId", DoctorID),
                new SqlParameter("@PageNo", pageno),
                new SqlParameter("@PageSize", pagesize)
            };


            string DB_SP_Name = "DBO.[Get_DoctorsProfile_SP]";
            if (ApiSources.ToLower() == "saleforce")
                DB_SP_Name = "SF.Get_DoctorsProfile_SP";

            var allPhysiciansModel = DB.ExecuteSPAndReturnDataSet(DB_SP_Name);
            if (allPhysiciansModel != null)
            {
                var RetrunDoctorData = MapDoctorDataSetToDoctorsData(allPhysiciansModel);
                return RetrunDoctorData;
            }
            else
                return null;
            
        }


        private DoctorsDataList MapDoctorDataSetToDoctorsData(DataSet DoctorDataset)
        {
            try
            {
                DoctorsDataList _DoctorDataListReturn = new DoctorsDataList();

                List<DoctorsData> DoctorDataList = new List<DoctorsData>();

                List<Reservation> _reservationList = new List<Reservation>();
                PatientData _pateitnData = new PatientData();

                if (DoctorDataset != null && DoctorDataset.Tables[0] != null && DoctorDataset.Tables[0].Rows.Count > 0)
                {
                    var _DocModel = DoctorDataset.Tables[0].ToListObject<Doctors>();
                    foreach (var row in _DocModel)
                    {

                        DoctorsData _DoctorData = new DoctorsData();
                        Doctors TempDoctors = new Doctors();
                        TempDoctors.Branch_Id = row.Branch_Id;
                        TempDoctors.DoctorFullName = row.DoctorFullName;
                        TempDoctors.ID = row.ID;
                        TempDoctors.IMAGEURL = row.IMAGEURL;
                        TempDoctors.Position = row.Position;
                        TempDoctors.SPECIALTY = row.SPECIALTY;
                        TempDoctors.TopProceduresPerformed = row.TopProceduresPerformed;
                        TempDoctors.YearsOfExp = row.YearsOfExp;
                        TempDoctors.Gender = row.Gender;
                        TempDoctors.Nationality = row.Nationality;
                        TempDoctors.SUBSPECIALTY = row.SUBSPECIALTY;
                        TempDoctors.DocCode = row.DocCode;
                        TempDoctors.ClinicCode = row.ClinicCode;


                        _DoctorData.DoctorProfile = TempDoctors;

                        if (DoctorDataset.Tables[1] != null && DoctorDataset.Tables[1].Rows.Count > 0)
                        {
                            List<Doctor_Language> Temp_Language = new List<Doctor_Language>();
                            List<Doctor_AssistingArea> Temp_AssistingArea = new List<Doctor_AssistingArea>();
                            List<Doctor_Education> Temp_Education = new List<Doctor_Education>();
                            List<Doctor_Membership> Temp_Membership = new List<Doctor_Membership>();
                            List<Doctor_Experience> Temp_Experience = new List<Doctor_Experience>();
                            List<Doctor_License> Temp_License = new List<Doctor_License>();
                            List<Doctor_Publication> Temp_Publication = new List<Doctor_Publication>();
                            List<Doctor_Privilege> Temp_Privilege = new List<Doctor_Privilege>();
                            List<Doctor_Accomplishment> Temp_Accomplishment = new List<Doctor_Accomplishment>();


                            DataRow[] DrsubDataTableRow = DoctorDataset.Tables[1].Select("Branch_Id=" + TempDoctors.Branch_Id + " and DocHISId = " + TempDoctors.ID);
                            if (DrsubDataTableRow != null)
                            {
                                foreach (DataRow Subrow in DrsubDataTableRow)
                                {
                                    if (Subrow["ProfileFeature"].ToString() == "Languages")
                                    {
                                        var mapObjFormSection = new Doctor_Language
                                        {
                                            ProfileFeature = Subrow["ProfileFeature"].ToString(),
                                            Sno = (int)Subrow["ID"],
                                            Branch_Id = (int)Subrow["Branch_Id"],
                                            DocHISId = (int)Subrow["DocHISId"],
                                            Value_EN = Subrow["Value_EN"].ToString(),
                                            VALUE_AR = Subrow["VALUE_AR"].ToString()
                                        };
                                        Temp_Language.Add(mapObjFormSection);
                                    }
                                    else if (Subrow["ProfileFeature"].ToString() == "AssistingArea")
                                    {
                                        var mapObjFormSection = new Doctor_AssistingArea
                                        {
                                            ProfileFeature = Subrow["ProfileFeature"].ToString(),
                                            Sno = (int)Subrow["ID"],
                                            Branch_Id = (int)Subrow["Branch_Id"],
                                            DocHISId = (int)Subrow["DocHISId"],
                                            Value_EN = Subrow["Value_EN"].ToString(),
                                            VALUE_AR = Subrow["VALUE_AR"].ToString()
                                        };
                                        Temp_AssistingArea.Add(mapObjFormSection);
                                    }
                                    else if (Subrow["ProfileFeature"].ToString() == "Education")
                                    {
                                        var mapObjFormSection = new Doctor_Education
                                        {
                                            ProfileFeature = Subrow["ProfileFeature"].ToString(),
                                            Sno = (int)Subrow["ID"],
                                            Branch_Id = (int)Subrow["Branch_Id"],
                                            DocHISId = (int)Subrow["DocHISId"],
                                            Value_EN = Subrow["Value_EN"].ToString(),
                                            VALUE_AR = Subrow["VALUE_AR"].ToString()
                                        };
                                        Temp_Education.Add(mapObjFormSection);
                                    }
                                    else if (Subrow["ProfileFeature"].ToString() == "Membership")
                                    {
                                        var mapObjFormSection = new Doctor_Membership
                                        {
                                            ProfileFeature = Subrow["ProfileFeature"].ToString(),
                                            Sno = (int)Subrow["ID"],
                                            Branch_Id = (int)Subrow["Branch_Id"],
                                            DocHISId = (int)Subrow["DocHISId"],
                                            Value_EN = Subrow["Value_EN"].ToString(),
                                            VALUE_AR = Subrow["VALUE_AR"].ToString()
                                        };
                                        Temp_Membership.Add(mapObjFormSection);
                                    }
                                    else if (Subrow["ProfileFeature"].ToString() == "Experience")
                                    {
                                        var mapObjFormSection = new Doctor_Experience
                                        {
                                            ProfileFeature = Subrow["ProfileFeature"].ToString(),
                                            Sno = (int)Subrow["ID"],
                                            Branch_Id = (int)Subrow["Branch_Id"],
                                            DocHISId = (int)Subrow["DocHISId"],
                                            Value_EN = Subrow["Value_EN"].ToString(),
                                            VALUE_AR = Subrow["VALUE_AR"].ToString()
                                        };
                                        Temp_Experience.Add(mapObjFormSection);
                                    }
                                    else if (Subrow["ProfileFeature"].ToString() == "License")
                                    {
                                        var mapObjFormSection = new Doctor_License
                                        {
                                            ProfileFeature = Subrow["ProfileFeature"].ToString(),
                                            Sno = (int)Subrow["ID"],
                                            Branch_Id = (int)Subrow["Branch_Id"],
                                            DocHISId = (int)Subrow["DocHISId"],
                                            Value_EN = Subrow["Value_EN"].ToString(),
                                            VALUE_AR = Subrow["VALUE_AR"].ToString()
                                        };
                                        Temp_License.Add(mapObjFormSection);
                                    }
                                    else if (Subrow["ProfileFeature"].ToString() == "Publication")
                                    {
                                        var mapObjFormSection = new Doctor_Publication
                                        {
                                            ProfileFeature = Subrow["ProfileFeature"].ToString(),
                                            Sno = (int)Subrow["ID"],
                                            Branch_Id = (int)Subrow["Branch_Id"],
                                            DocHISId = (int)Subrow["DocHISId"],
                                            Value_EN = Subrow["Value_EN"].ToString(),
                                            VALUE_AR = Subrow["VALUE_AR"].ToString()
                                        };
                                        Temp_Publication.Add(mapObjFormSection);
                                    }
                                    else if (Subrow["ProfileFeature"].ToString() == "Privilege")
                                    {
                                        var mapObjFormSection = new Doctor_Privilege
                                        {
                                            ProfileFeature = Subrow["ProfileFeature"].ToString(),
                                            Sno = (int)Subrow["ID"],
                                            Branch_Id = (int)Subrow["Branch_Id"],
                                            DocHISId = (int)Subrow["DocHISId"],
                                            Value_EN = Subrow["Value_EN"].ToString(),
                                            VALUE_AR = Subrow["VALUE_AR"].ToString()
                                        };
                                        Temp_Privilege.Add(mapObjFormSection);
                                    }
                                    else if (Subrow["ProfileFeature"].ToString() == "Accomplishment")
                                    {
                                        var mapObjFormSection = new Doctor_Accomplishment
                                        {
                                            ProfileFeature = Subrow["ProfileFeature"].ToString(),
                                            Sno = (int)Subrow["ID"],
                                            Branch_Id = (int)Subrow["Branch_Id"],
                                            DocHISId = (int)Subrow["DocHISId"],
                                            Value_EN = Subrow["Value_EN"].ToString(),
                                            VALUE_AR = Subrow["VALUE_AR"].ToString()
                                        };
                                        Temp_Accomplishment.Add(mapObjFormSection);
                                    }

                                }
                            }
                            _DoctorData.Accomplishment = Temp_Accomplishment;
                            _DoctorData.AssistingArea = Temp_AssistingArea;
                            _DoctorData.Education = Temp_Education;
                            _DoctorData.Experience = Temp_Experience;
                            _DoctorData.Language = Temp_Language;
                            _DoctorData.License = Temp_License;
                            _DoctorData.Membership= Temp_Membership;
                            _DoctorData.Privilege = Temp_Privilege;
                            _DoctorData.Publication = Temp_Publication;
                        }
                        
                        
                        DoctorDataList.Add(_DoctorData);
                    }
                    _DoctorDataListReturn.DoctorDataList = DoctorDataList;
                }

                return _DoctorDataListReturn;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }


    }
#pragma warning restore 1591
}