using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Model
{
	public class TokenModal
	{
		public string access_token { get; set; }
		public string token_type { get; set; }

		public int expires_in { get; set; }

	}
	public class UAE_SMS_TokenModal
	{
		public string token { get; set; }
		public string sessionId { get; set; }
	}
	public class UAE_LoginCheck
	{
		public string MRN { get; set; }
		public string PatientName { get; set; }
		public string Mobile { get; set; }
		public string IdNumber { get; set; }
	}

	//Patient Search:
	public class UAE_Patient_Info
	{
		public UAE_Patient_Info ()
		{
			this.CurrentCity = "";
			this.IdType = "";
			this.IdExpiry = "";
		}
		public string address { get; set; }
		public string birthday { get; set; }
		public string email { get; set; }
		public string family_name { get; set; }
		public string first_name { get; set; }
		public string gender { get; set; }
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
		public string nationalityId { get; set; }
		public string nationality { get; set; }
		public string age { get; set; }
		public string weight { get; set; }
		public string height { get; set; }
		public string bloodgroup { get; set; }
		public string familymembersCount { get; set; }
		public string hospital_id { get; set; }

		public int countryId { get; set; }
		public string CurrentCity { get; set; }
		public string IdType { get; set; }
		public string IdExpiry { get; set; }
	}


	public class PatientInsurance_UAE
	{
		public string registrationno { get; set; }
		public string category { get; set; }
		public string company { get; set; }
		public string grade { get; set; }
		public string policyNo { get; set; }
		public string insuranceId { get; set; }
		public string idExpiryDate { get; set; }
	}
	public class UAE_Doctor_Slots
	{
		public string AppoitmentDate { get; set; }
		public string StartTime { get; set; }
		public string Endtime { get; set; }
		public string DoctorID { get; set; }
		public string DoctorName { get; set; }		public string DepartmentID { get; set; }		
	}

	public class UAE_Patient_Family_List
	{
		public string Facility { get; set; }
		public string MRN { get; set; }
		public string PatientName { get; set; }
		public string DOB { get; set; }
		public string Mobile { get; set; }
		public string IDNumber { get; set; }
	}

	public class UAE_Patient_Appoitment_List
	{
		public int id { get; set; }
		public int appointmentNo { get; set; }
		public string registrationNo { get; set; }
		public string clinicName { get; set; }
		public DateTime appDate { get; set; }
		public string appTime { get; set; }
		public string patientVisited { get; set; }
		public string patientName { get; set; }
		public string doctorName { get; set; }
		public string confirmationStatus { get; set; }
		public string isUpComming { get; set; }

		public string doctorId { get; set; }

		public string clinicId { get; set; }

	}

	public class UAE_Doctor_Days
	{
		public int Id { get; set; }
		public int DepartmentID { get; set; }
		public string Doctor_ID { get; set; }
		public DateTime Scheduled_day { get; set; }
		public string Fromtime { get; set; }
		public string ToTime { get; set; }
		public int SlotTypeId { get; set; }
		public string SlotTypeName { get; set; }
		public int DayBooked { get; set; }
		public int noSchedules { get; set; }
		public string AvailableSlots { get; set; }

		public string totalSlotCount { get; set; }
		public string split_shift { get; set; }
		public string schedule_1 { get; set; }
		public string schedule_2 { get; set; }
	}

	// For Book Appoitment Save Appoitment
	public class UAE_BookAppointment
	{
		public UAE_BookAppointment()
		{
			this.PatientName = "";
			this.CreatedBy = "M";
			this.TransType = "Book";
			this.IsStandby = "false";
			this.CancelReason = "p";
			this.Remarks = "M";

		}
		public int BranchID { set; get; }
		public string Facility { set; get; }
		public DateTime AppDate { set; get; }
		public string StartTime { set; get; }
		public string EndTime { set; get; }
		public string DocId { set; get; }
		public int PhysicanId { set; get; }
		public string PatientId { set; get; }
		public string PatientName { set; get; }
		public string CreatedBy { set; get; }
		public string TransType { set; get; }
		public string IsStandby { set; get; }
		public string AppId { set; get; }
		public string CancelReason { set; get; }

		public string MobileNumber { set; get; }
		public string Remarks { set; get; }
	}

	public class AppointmentPostResponse
	{
		public string Error { get; set; }
		public string Message { get; set; }
		public string AppId { get; set; }
		public string AppStatus { get; set; }
	}


	public class RegisterPatientUAE
	{
		public string Lang { get; set; }
		public int HospitaId { get; set; }
		public string PatientId { get; set; }
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
		public DateTime IdExpiry { get; set; }
		public int IdType { get; set; }
		public string CurrentCity { get; set; }
		


	}

	public class RegistrationPostResponse
	{
		public int Error { get; set; }
		public string Message { get; set; }
		public string Mrn { get; set; }		

	}

	public class UAE_My_Docotr
	{
		public string id { get; set; }

		public string nearestAvailableSlot { get; set; }

	}

	public class PatientVisit_UAE
	{
		public string id { get; set; }
		public int appointmentNo { get; set; }
		public string registrationNo { get; set; }
		public string clinicName { get; set; }
		public string appDate { get; set; }
		public string appTime { get; set; }
		public int patientVisited { get; set; }
		public string patientName { get; set; }
		public string doctorName { get; set; }
		public string appointmentType { get; set; }
		public string videoCallUrl { get; set; }
		public string doctorId { get; set; }
		public int paid { get; set; }
		public string episodeId { get; set; }
		public string episodeStatus { get; set; }
		public string episodeType { get; set; }
		
		public string branchId { get; set; }

	}


	public class LabTest_List_Modal
	{
		public string branchid { get; set; }
		public string registration_no { get; set; }
		public string report_type { get; set; }
		public string report_date { get; set; }
		public string opip { get; set; }
		public string test_name { get; set; }
		public string report_filename { get; set; }
		public string ftp_path { get; set; }
		public string report_id { get; set; }
		public string test_id { get; set; }

	}
	public class BranchInfo
	{
		public string BranchID { get; set; }
		public string BranchCode { get; set; }
		public string BranchName { get; set; }
		public string latitude { get; set; }
		public string longitude { get; set; }
	}

	public class TestResultParameter_UAE
	{
		public string parameter_name { get; set; }
		public string result { get; set; }
		public string unit { get; set; }
		public string range { get; set; }
		public string resultValueCategory { get; set; }
		public string rating { get; set; }
		public string severityID { get; set; }
		public string weightage { get; set; }
	}
	public class TestResult_details_UAE
	{
		public string testCode { get; set; }
		public string testName { get; set; }
		public string section { get; set; }
		public string sample_name { get; set; }
		public string collected_date { get; set; }
		
		public List<TestResultParameter_UAE> parameters { get; set; }

	}

	public class LabRad_PDF_UAE
	{
		public string PatientId { get; set; }
		public string OrderNumber { get; set; }
		public string Base64 { get; set; }
	}
	public class LabRad_PDF_UAE_Response
	{
		public string Error { get; set; }
		public object Reports  { get; set; }		
	}


}
