using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer.Model
{
    public class Client
    {
        public string ClientSecretKey { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string Role { get; set; }

        public int UserID { get; set; }
    }
    public class Hospitals
    {
        public int id { get; set; }
        public string value { get; set; }
        public string TeleMedContentPath { get; set; }
        public string PaymentGWTerminalId { get; set; }
        public string PaymentGWPass { get; set; }
        public string PaymentGWMID { get; set; }
        public string PaymentGWReqURL { get; set; }
        public string PaymentGWTerminalIdWeb { get; set; }
        public string PaymentGWPassWeb { get; set; }
        public string PaymentGWReqURLWeb { get; set; }
        public string CountryDialingCode { get; set; }
    }

    public class Clinics
    {
        public int id { get; set; }
        public string value { get; set; }
        public int AvaialbleDocCount { get; set; } // Ahsan changes 27/05/2021
    }

    public class Nationalities
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Titles
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class MaritalStatusCodes
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class GenericResponse
    {   
        public int status { get; set; }
        public string msg { get; set; }
        public object response { get; set; }
        public string error_type { get; set; }

    }

    public class GenericResponse_Report
    {
        public int status { get; set; }
        public string msg { get; set; }
        public object response { get; set; }
        public string error_type { get; set; }

    }

    public class RegisterPatientResponse
    {
        public int activation_num { get; set; }
        public int status { get; set; }
        public string msg { get; set; }
        public object response { get; set; }
        public string smsResponse { get; set; }
        public string success_type { get; set; }
        public string error_type { get; set; }
        //public int registration_no { get; set; }

    }


    public class PhysicianModel
    {
        public int ID { get; set; }
        public string DoctorProfile { get; set; }
        public string DoctorAddress { get; set; }
        public DateTime DOB { get; set; }
        public string DOCTORCERTIFICATES { get; set; }
        public string CLINICSERVICES { get; set; }
        public string CLINICS { get; set; }
        public string DEGREE { get; set; }
        public string EQUIPMENTS { get; set; }
        public string EXPERIENCE { get; set; }
        public string EXPERIENCELEVEL { get; set; }
        public string FIRSTNAME { get; set; }
        public string FIRSTNAME_AR { get; set; }
        public string FullName { get; set; }
        public string GRADUATEFROM { get; set; }
        public int GRADUATEYEAR { get; set; }
        public string IMAGEURL { get; set; }
        public string LASTNAME { get; set; }
        public string LASTNAME_AR { get; set; }
        public string MIDDLENAME { get; set; }
        public string MOBILE1 { get; set; }
        public string MOBILE2 { get; set; }
        public string NOTES { get; set; }
        public string PERFORMEDOPERATIONS { get; set; }
        public string PHONENUMBER { get; set; }
        public string SPECIALTY { get; set; }

    }

    public class Physician
    {
        public string about_physician { get; set; }
        public string address { get; set; }
        public string awards { get; set; }
        public DateTime birthday { get; set; }
        public string certificates { get; set; }
        public List<Clinic_Services> clinic_services { get; set; }
        public List<Clinic> clinics { get; set; }
        public string degree { get; set; }
        public List<Equipment> equipments { get; set; }
        public string experience { get; set; }
        public string experience_level { get; set; }
        public string family_name { get; set; }
        public string first_name { get; set; }
        public string first_name_ar { get; set; }
        public string full_name { get; set; }
        public string graduated_from { get; set; }
        public int graduation_year { get; set; }
        public int id { get; set; }
        public string image_url { get; set; }
        public string last_name { get; set; }
        public string last_name_ar { get; set; }
        public string middle_name { get; set; }
        public string mobile1 { get; set; }
        public string mobile2 { get; set; }
        public string notes { get; set; }
        public List<Performed_Operations> performed_operations { get; set; }
        public string phone_number { get; set; }
        public List<Speciality> specialities { get; set; }
    }


    public class Doctors
    {
        public int Branch_Id { get; set; }
        public int ID { get; set; }
        public string DoctorFullName { get; set; }
        public string Position { get; set; }
        public string SPECIALTY { get; set; }
        public string IMAGEURL { get; set; }
        public string TopProceduresPerformed { get; set; }
        public string YearsOfExp { get; set; }
        public string Gender { get; set; }
        
        public string Nationality { get; set; }
        public string SUBSPECIALTY { get; set; }

        public string DocCode { get; set; }

        
    }
    public class DoctorsDataList
    {
        public List<DoctorsData> DoctorDataList { get; set; }
    }
    public class DoctorsData
    {
        public Doctors  DoctorProfile { get; set; }
        public List<Doctor_Language> Language { get; set; }

        public List<Doctor_AssistingArea> AssistingArea { get; set; }
        public List<Doctor_Education> Education { get; set; }
        public List<Doctor_Membership> Membership { get; set; }

        public List<Doctor_Experience> Experience { get; set; }

        public List<Doctor_License> License { get; set; }

        public List<Doctor_Publication> Publication { get; set; }

        public List<Doctor_Privilege> Privilege { get; set; }

        public List<Doctor_Accomplishment> Accomplishment { get; set; }
    }
    public class Doctor_Accomplishment
    {
        public int Sno { get; set; }
        public int Branch_Id { get; set; }
        public int DocHISId { get; set; }

        public string ProfileFeature { get; set; }
        public string Value_EN { get; set; }
        public string VALUE_AR { get; set; }
    }
    public class Doctor_Privilege
    {
        public int Sno { get; set; }
        public int Branch_Id { get; set; }
        public int DocHISId { get; set; }

        public string ProfileFeature { get; set; }
        public string Value_EN { get; set; }
        public string VALUE_AR { get; set; }
    }
    public class Doctor_Publication
    {
        public int Sno { get; set; }
        public int Branch_Id { get; set; }
        public int DocHISId { get; set; }

        public string ProfileFeature { get; set; }
        public string Value_EN { get; set; }
        public string VALUE_AR { get; set; }
    }
    public class Doctor_License
    {
        public int Sno { get; set; }
        public int Branch_Id { get; set; }
        public int DocHISId { get; set; }

        public string ProfileFeature { get; set; }
        public string Value_EN { get; set; }
        public string VALUE_AR { get; set; }
    }
    public class Doctor_Experience
    {
        public int Sno { get; set; }
        public int Branch_Id { get; set; }
        public int DocHISId { get; set; }

        public string ProfileFeature { get; set; }
        public string Value_EN { get; set; }
        public string VALUE_AR { get; set; }
    }
    public class Doctor_Membership
    {
        public int Sno { get; set; }
        public int Branch_Id { get; set; }
        public int DocHISId { get; set; }

        public string ProfileFeature { get; set; }
        public string Value_EN { get; set; }
        public string VALUE_AR { get; set; }
    }
    public class Doctor_Education
    {
        public int Sno { get; set; }
        public int Branch_Id { get; set; }
        public int DocHISId { get; set; }

        public string ProfileFeature { get; set; }
        public string Value_EN { get; set; }
        public string VALUE_AR { get; set; }
    }
    public class Doctor_AssistingArea
    {
        public int Sno { get; set; }
        public int Branch_Id { get; set; }
        public int DocHISId { get; set; }

        public string ProfileFeature { get; set; }
        public string Value_EN { get; set; }
        public string VALUE_AR { get; set; }
    }
    public class Doctor_Language
    {
        public int Sno { get; set; }
        public int Branch_Id { get; set; }
        public int DocHISId { get; set; }
        
        public string ProfileFeature { get; set; }
        public string Value_EN { get; set; }
        public string VALUE_AR { get; set; }
    }
    public class Clinic_Services
    {
        public string desc { get; set; }
        public string name { get; set; }
    }

    public class Clinic
    {
        public string name { get; set; }
    }

    public class Equipment
    {
        public string desc { get; set; }
        public string name { get; set; }
    }

    public class Performed_Operations
    {
        public string desc { get; set; }
        public string name { get; set; }
    }

    public class Speciality
    {
        public string desc { get; set; }
        public string name { get; set; }
    }

    public class AvailableSlotsModel
    {
        public int Id { get; set; }
        public int DepartmentID { get; set; }
        public int DoctorId { get; set; }
        public DateTime ScheduledDay { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public int SlotTypeId { get; set; }
        public string SlotTypeName { get; set; }
        public string DoctorClinicTime { get; set; }
    }

    public class AvailableSlots
    {
        public int Id { get; set; }
        public string time_to { get; set; }
        public string time_from { get; set; }
        public int slot_type_id { get; set; }
        public string slot_type_name { get; set; }

        public string Doctor_ClinicTime { get; set; }

    }

    public class AvailableDays
    {
        public int Id { get; set; }
        public int doctor_id { get; set; }
        public DateTime schedule_day { get; set; }
    }

    public class AvailableDays2
    {
        public int Id { get; set; }
        public int doctor_id { get; set; }
        public DateTime schedule_day { get; set; }
        public string time_to { get; set; }
        public string time_from { get; set; }
    }

    public class UserInfoModel
    {
        public int Branch_Id { get; set; }
        public int PatientId { get; set; }
        public int TitleId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FamilyName { get; set; }
        public string PCellno { get; set; }
        public string PEmail { get; set; }
        public string PAddress { get; set; }
        public DateTime DOB { get; set; }
        public int GenderId { get; set; }
        public int MaritalStatusId { get; set; }
        public string Name { get; set; }
        public string NationalId { get; set; }

    }
    
    public class UserLoginInfoList
    {
        public string Branch_EN { get; set; }
        public string Branch_AR { get; set; }
        public string PatientId { get; set; }
        public string PatientName_EN { get; set; }
        public string PatientName_AR { get; set; }
        public string PatientCellNo { get; set; }
        public string PEMail { get; set; }
        public string Registrationno { get; set; }
        public string BranchId { get; set; }
        public string PatientCellNo2 { get; set; }

    }


    public class UserInfo2Model
    {
        public int Branch_Id { get; set; }
        public int PatientId { get; set; }
        public int TitleId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FamilyName { get; set; }
        public string PCellno { get; set; }
        public string PEmail { get; set; }
        public string PAddress { get; set; }
        public DateTime DOB { get; set; }
        public int GenderId { get; set; }
        public int MaritalStatusId { get; set; }
        public string Name { get; set; }
        public string NationalId { get; set; }
        public int RegistrationNo { get; set; }
        public string name_ar { get; set; }
    }


    public class UserInfo2Model_New
    {
        public string Branch_Id { get; set; }
        public string PatientId { get; set; }
        public string TitleId { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FamilyName { get; set; }
        public string PatientFullName { get; set; }
        public string PCellno { get; set; }        

        public string PEmail { get; set; }
        public string PAddress { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; }

        public string Gender_ar { get; set; }
        public string MaritalStatusId { get; set; }
        public string Name { get; set; }
        public string NationalId { get; set; }

        public string NationalityId { get; set; }
        
        public string Nationality { get; set; }
        public string RegistrationNo { get; set; }
        public string name_ar { get; set; }


        public string Age { get; set; }
        public string pWeight { get; set; }
        public string pHeight { get; set; }
        public string BloodGroup { get; set; }

        public int FamilyMembersCount { get; set; }

        public string image_url { get; set; }

        public bool IsCash { get; set; }

        public string Branch_Name { get; set; }
        public string Branch_AR { get; set; }
    }

    public class UserInfo
    {
        public string address { get; set; }
        public DateTime birthday { get; set; }
        public string email { get; set; }
        public string family_name { get; set; }
        public string first_name { get; set; }
        public int gender { get; set; }
        public int hospital_id { get; set; }
        public int id { get; set; }
        public string last_name { get; set; }
        public int marital_status_id { get; set; }
        public string middle_name { get; set; }
        public string name { get; set; }
        public string national_id { get; set; }
        public string phone { get; set; }
        public string registration_no { get; set; }
        public int title_id { get; set; }
        public string name_ar { get; set; }
    }


    public class UserInfo_New
    {
        public string address { get; set; }
        public DateTime birthday { get; set; }
        public string email { get; set; }
        public string family_name { get; set; }
        public string first_name { get; set; }
        public string gender { get; set; }
        public string gender_ar { get; set; }
        public string hospital_id { get; set; }
        public string id { get; set; }
        public string last_name { get; set; }
        public string marital_status_id { get; set; }
        public string middle_name { get; set; }
        public string name { get; set; }
        public string national_id { get; set; }
        public string phone { get; set; }
        public string registration_no { get; set; }
        public string title_id { get; set; }
        public string name_ar { get; set; }

        public string NationalityId { get; set; }
        public string Nationality { get; set; }


        public string Age { get; set; }
        public string Weight { get; set; }
        public string Height { get; set; }
        public string BloodGroup { get; set; }

        public int FamilyMembersCount { get; set; }

        public string image_url { get; set; }

        public bool IsCash { get; set; }

        public string Branch_Name { get; set; }
        public string Branch_Id { get; set; }
        public string Branch_Name_ar { get; set; }
    }


    public class LoginInfoList
    {
        public List<UserInfo> UserInfoObj { get; set; }
    }
    
    public class PatientModel
    {
        public string PAddress { get; set; }
        public int Branch_Id { get; set; }
        public int PatientId { get; set; }
        public int TitleId { get; set; }
        public DateTime DOB { get; set; }
        public string PEmail { get; set; }
        public string FamilyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int MaritalStatusId { get; set; }
        public string MiddleName { get; set; }
        public string Name { get; set; }
        public string NationalId { get; set; }
        public string PCellno { get; set; }
        public string RegistrationNo { get; set; }
        public int GenderId { get; set; }

    }

   


    public class PatientData
    {
        public Patient patient { get; set; }
        public List<Reservation> reservations { get; set; }
    }

    public class Patient
    {
        public string address { get; set; }
        public string birthday { get; set; }
        public string email { get; set; }
        public string family_name { get; set; }
        public string first_name { get; set; }
        public int gender { get; set; }
        public int hospital_id { get; set; }
        public int id { get; set; }
        public string last_name { get; set; }
        public int marital_status_id { get; set; }
        public string middle_name { get; set; }
        public string name { get; set; }
        public string national_id { get; set; }
        public string phone { get; set; }
        public string registration_no { get; set; }
        public int title_id { get; set; }

        

    }

    public class ReservationModel
    {
        public string ClinicName { get; set; }
        public DateTime AppDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Id { get; set; }
        public int AppointmentNo { get; set; }
        public int RegistrationNo { get; set; }
        public int PatientVisited { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public TimeSpan AppTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string VideoCallUrl { get; set; }
        public int DoctorId { get; set; }
        public int Paid { get; set; }
        public int AppointmentType { get; set; }
        public int Branch_Id { get; set; }
        public int Duration { get; set; }
        public string TimeZone { get; set; }
        public int NationionalId { get; set; }
        public string host2 { get; set; }
        public int autoJoin2 { get; set; }
        public string resourceId2 { get; set; }
        public string displayName2 { get; set; }
        public string token2 { get; set; }
        public int IsActive { get; set; }

        public int UpComingAppointment { get; set; }   // Ahsan changes 27/05/2021
    }

    public class Reservation
    {
        public string clinic_name { get; set; }
        public string date { get; set; }
        public int id { get; set; }
        public int patient_attend { get; set; }
        public string patient_name { get; set; }
        public string physician_name { get; set; }
        public string time_from { get; set; }
        public string video_call_url { get; set; }
        public int doctor_id { get; set; }
        public int paid { get; set; }
        public int appointment_type { get; set; }
        public int appointment_no { get; set; }
        public int branch_id { get; set; }
        public int duration { get; set; }
        public string time_zone { get; set; }
        public int registration_no { get; set; }
        public int nationional_id { get; set; }
        public string end_date { get; set; }
        public string end_time { get; set; }
        public string host2 { get; set; }
        public int autoJoin2 { get; set; }
        public string resourceId2 { get; set; }
        public string displayName2 { get; set; }
        public string token2 { get; set; }
        public int active { get; set; }

        public int UpComingAppointment { get; set; }   // Ahsan changes 27/05/2021
    }

    public class RegisterPatient
    {
        public string Lang { get; set; }
        public int HospitaId { get; set; }
        public int PatientId { get; set; }
        public string PateintUserName { get; set; }
        public string PatientPassword { get; set; }
        public int PatientTitleId { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientMiddleName { get; set; }
        public string PatientLastName { get; set; }
        public string PatientFamilyName { get; set; }
        public string PatientPhone { get; set; }
        public string PatientEmail { get; set; }
        public string PatientNationalId { get; set; }
        public DateTime PatientBirthday { get; set; }
        public int PatientGender { get; set; }
        public string PatientAddress { get; set; }
        public int PatientNationalityId { get; set; }
        public int PatientMaritalStatusId { get; set; }
        public string SkipDuplicateEmail { get; set; }
        public string SkipDuplicatePhone { get; set; }
        public string ActivationNum { get; set; }
        public string SkipSendActivationNum { get; set; }

    }

    public class RegisterPatient2
    {
        public string Lang { get; set; }
        public int HospitaId { get; set; }
        public int PatientId { get; set; }
        public int PatientTitleId { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientMiddleName { get; set; }
        public string PatientLastName { get; set; }
        public string PatientFamilyName { get; set; }
        public string PatientPhone { get; set; }
        public string PatientEmail { get; set; }
        public string PatientNationalId { get; set; }
        public DateTime PatientBirthday { get; set; }
        public int PatientGender { get; set; }
        public string PatientAddress { get; set; }
        public int PatientNationalityId { get; set; }
        public int PatientMaritalStatusId { get; set; }
        public bool skipDuplicateCheck { get; set; }
    }

    public class RegistePatientResponseModel
    {
        public int ID { get; set; }
        public int RegistrationNo { get; set; }
        public string PCellNo { get; set; }
        public string PEmail { get; set; }
        public string NationalId { get; set; }
        public string DisplayMsg { get; set; }
        public string name { get; set; }
        public string name_ar { get; set; }
    }

    public class RegistePatientResponseFailure
    {
        public string is_you { get; set; }
        public int registration_no { get; set; }
        public string name { get; set; }
        public string name_ar { get; set; }
    }

    public class RegistePatientResponseSuccess
    {
        public string phone { get; set; }
        public int registration_no { get; set; }
        public string national_id { get; set; }
        public string name { get; set; }
        public string name_ar { get; set; }
    }

    public class ResendVerificationModel
    {
        public int Id { get; set; }
        public int RegistrationNo { get; set; }
        public string PCellNo { get; set; }
        public int ActivationNo { get; set; }
        public string AppPassword { get; set; }
    }


    public class PatientDiagnosisModel
    {
        
        public int VisitId { get; set; }
        public string VisitType { get; set; }
        public string DiagnosisDesc { get; set; }
        public DateTime DiagnosisDateTime { get; set; }
    }

    public class PatientDiagnosis
    {
        public int visit_id { get; set; }
        public string visit_type { get; set; }
        public string diagnosis_desc { get; set; }
        public DateTime diagnosis_datetime { get; set; }
    }

    public class PatientPrescription
    {
        public int visit_id { get; set; }
        public string doctor_name { get; set; }
        public DateTime prescription_date { get; set; }
        public string drug_name { get; set; }
        public string route { get; set; }
        public string duration { get; set; }

    }

    public class PateintTestsModel
    {
        public int RegistrationNo { get; set; }
        public string ReportType { get; set; }
        public DateTime ReportDate { get; set; }
        public string OPIP { get; set; }
        public string TestName { get; set; }
        public string ReportFileName { get; set; }
        public string FTPPath { get; set; }
        public int GROUP_ReportId { get; set; }
        public int GROUP_TestId { get; set; }

    }

    public class PateintTests
    {
        public int registration_no { get; set; }
        public string report_type { get; set; }
        public DateTime report_date { get; set; }
        public string opip { get; set; }
        public string test_name { get; set; }
        public string report_filename { get; set; }
        public string ftp_path { get; set; }
        public int report_id { get; set; }
        public int test_id { get; set; }

    }

    public class ResultHeaderModel
    {
        public int OrderId { get; set; }
        public int RegistrationNo { get; set; }
        public DateTime OrderDateTime { get; set; }
        public DateTime TestDoneDateTime { get; set; }
        public DateTime CollectedDateTime { get; set; }
        public string BillNo { get; set; }
        public string OrderNo { get; set; }
        public string DoctorName { get; set; }
        public string ReportName { get; set; }
        public string DeptName { get; set; }
    }

    public class ResultTestModel
    {
        public int OrderId { get; set; }
        public string Section { get; set; }
        public string TestName { get; set; }
        public string ParameterName { get; set; }
        public string Rating { get; set; }
        public string Limits { get; set; }
        public string Comments { get; set; }
    }

    public class ResultFooterModel
    {
        public int OrderId { get; set; }
        public string OrganismName { get; set; }
        public string SourceName { get; set; }
        public string AntibioticName { get; set; }
        public int Result { get; set; }

    }


    public class PatientResultData
    {
        public List<ResultHeader> order { get; set; }
    }

    public class ResultHeader
    {
        public int order_id { get; set; }
        public int registration_no { get; set; }
        public DateTime order_datetime { get; set; }
        public DateTime testdone_datetime { get; set; }
        public DateTime collected_datetime { get; set; }
        public string bill_no { get; set; }
        public string order_no { get; set; }
        public string docto_name { get; set; }
        public string report_name { get; set; }
        public string dept_name { get; set; }

        List<ResultTest> testresult { get; set; }
        List<ResultFooter> cultureresult { get; set; }
    }

    public class ResultTest
    {
        public int order_id { get; set; }
        public string section { get; set; }
        public string test_name { get; set; }
        public string parameter_name { get; set; }
        public string rating { get; set; }
        public string limits { get; set; }
        public string comments { get; set; }
    }

    public class ResultFooter
    {
        public int order_id { get; set; }
        public string organism_name { get; set; }
        public string source_name { get; set; }
        public string antibiotic_name { get; set; }
        public int result { get; set; }

    }

    public class ConsultationAmountModel
    {
        public decimal CashAmount { get; set; }
        public decimal DeductibleAmount { get; set; }	
    }

    public class ConsultationAmount
    {
        public decimal cash_amount { get; set; }
        public decimal deductible_amount { get; set; }
    }


    public class PatientBillModel
    {
        public string BillNo { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VAT { get; set; }
        public string ItemCode { get; set; }
        public int RegistrationNo { get; set; }
        public string ItemName { get; set; }
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public int Paid { get; set; }
    }

    public class PatientBill
    {
        public string bill_no { get; set; }
        public decimal total_amount { get; set; }
        public decimal vat { get; set; }
        public string item_code { get; set; }
        public int registration_no { get; set; }
        public string item_name { get; set; }
        public decimal qty { get; set; }
        public decimal price { get; set; }
        public decimal paid { get; set; }

    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class TestParameter
    {
        public int parameterId { get; set; }
        public int sequenceNo { get; set; }
        public string parameterName { get; set; }
        public int resultType { get; set; }
        public string resultTypeDesc { get; set; }
        public string unit { get; set; }
        public string result { get; set; }
        public string limitRange { get; set; }
        public List<object> cultureSensitivity { get; set; }
        public int id { get; set; }
        public DateTime insertedAt { get; set; }
        public string ResultValueCategory { get; set; }

        public string rating { get; set; }
        

    }

    public class Entity
    {
        public int labNo { get; set; }
        public int testId { get; set; }
        public string testCode { get; set; }
        public string testName { get; set; }
        public string profileName { get; set; }
        public string sampleName { get; set; }
        public string section { get; set; }
        public string acknowledgeDt { get; set; }
        public string acknowledgeBy { get; set; }
        public string collectedBy { get; set; }
        public string collectedDt { get; set; }
        public string testDoneBy { get; set; }
        public string testDoneDt { get; set; }
        public string verifiedBy { get; set; }
        public string verifiedDt { get; set; }
        public List<TestParameter> testParameters { get; set; }
        public int id { get; set; }
        public DateTime insertedAt { get; set; }
    }

    public class TestResult
    {
        public int responseCode { get; set; }
        public List<string> responseMessage { get; set; }
        public Entity entity { get; set; }
    }

    public class TestResultPDF
    {
        public string ReportUrl { get; set; }        
    }

    public class InvoicePDF
    {
        public string InvoiceUrl { get; set; }
    }

    public class TestResultMain
    {
        public string testCode { get; set; }
        public string testName { get; set; }
        public string section { get; set; }
        public string sample_name { get; set; }        
        public string collected_date { get; set; }
        public List<TestResultParameter> parameters { get; set; }
    }

    public class TestResultParameter
    {
        public string parameter_name { get; set; }
        public string result { get; set; }
        public string unit { get; set; }
        public string range { get; set; }

        public string ResultValueCategory { get; set; }


        public string rating { get; set; }
        
        public string severityID { get; set; }

        public int Weightage { get; set; }


    }


    public class UserProfileURL
    {
        public string Image_URL { get; set; }
    }


    public class PreDefineMedReport
    {
        public string VisitType { get; set; }
        public string ReportType { get; set; }
        public int RegistrationNo { get; set; }
        public int VisitID { get; set; }
        public DateTime VisitiDateTime { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int NoOfDays { get; set; }
        public string Reason_AR { get; set; }
        public string URL { get; set; }

        public string DoctorName { get; set; }
        
        public string DepartmentName { get; set; }

        public string ReportID_pram { get; set; }

    }


    //Ahsan testing form E-Envoiceing 
    public class SaveBillReturn
    {
        public string ReturnCode { get; set; }
        public string ReturnMsgs { get; set; }
        public string BillNo { get; set; }
        public string BillAmount { get; set; }

        public string GenerateEInvoice { get; set; }
        

    }
    public class EInvoiceParam
    {        
        public int operatorId { get; set; }     
        public int claimTypeId { get; set; }
        
        public int billTypeId { get; set; }
        
        public int invoiceTypeId { get; set; }
        
        public int billReferenceId { get; set; }
        
        public string billReferenceNo { get; set; }
        
        public string companyName { get; set; }
        
        public string vatRegistrationNo { get; set; }
        
        public string transactionDateTime { get; set; }
        
        public decimal totalNet { get; set; }
        
        public decimal totalVAT { get; set; }
    }
    public class EInvoiceReturn
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public EInvoiceDetailVm Details { get; set; }
    }

    public class EInvoiceDetailVm
    {
        public long Id { get; set; }
        public int InvoiceTitleId { get; set; }
        public string InvoiceTitle { get; set; }
        public int ClaimTypeId { get; set; }
        public string ClaimType { get; set; }
        public int BillTypeId { get; set; }
        public string BillType { get; set; }
        public int InvoiceTypeId { get; set; }
        public string InvoiceType { get; set; }
        public int BillReferenceId { get; set; }
        public string BillReferenceNo { get; set; }
        public string CompanyName { get; set; }
        public string VATRegistrationNo { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public float TotalNet { get; set; }
        public float TotalVAT { get; set; }
        public string QRImage { get; set; }
        public string BaseUrl { get; set; }
        public string InvoiceHashKey { get; set; }
        public int OperatorId { get; set; }
    }

    public class MedicalPrescriptions
    {
        public string PdfURL { get; set; }
    }


    //****************   Login Controller **************************/

    public class login_check_modal
    {
        public string Branch_EN { get; set; }
        public string Branch_AR { get; set; }
        public string PatientId { get; set; }
        public string PatientName_EN { get; set; }
        public string PatientName_AR { get; set; }
        public string PatientCellNo { get; set; }
        public string PEMail { get; set; }
        public string Registrationno { get; set; }
        public string BranchId { get; set; }
        public string PatientCellNo2 { get; set; }
        
        public string PatientFullName { get; set; }
        public string DOB { get; set; }
        public string image_url { get; set; }
        

    }




}
