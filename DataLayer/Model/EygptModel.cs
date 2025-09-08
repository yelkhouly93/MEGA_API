using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Model
{
	public class EYG_Patient_Info_Search
	{
		public int branch_Id { get; set; }
		public string branch_EN { get; set; }
		public string branch_AR { get; set; }
		public string registrationNo { get; set; }
		public string firstName { get; set; }
		public string middleName { get; set; }
		public string lastName { get; set; }
		public string familyName { get; set; }
		public string patientFullName { get; set; }
		public string nationalID { get; set; }
		public string pCellno { get; set; }
		public string PPhone { get; set; }
		public string dob { get; set; }
		public string age { get; set; }		
		public string title { get; set; }
		public string gender { get; set; }
		public string maritalStatus { get; set; }
		public string nationalityId { get; set; }
		public string nationality { get; set; }
	}


	public class EYG_Patient_Info_DATA
	{
		public int hospital_id { get; set; }
		public string branch_EN { get; set; }
		public string branch_AR { get; set; }
		public string registration_no { get; set; }
		public string first_name { get; set; }
		public string middle_name { get; set; }
		public string last_name { get; set; }
		public string family_name { get; set; }
		public string name { get; set; }
		public string id { get; set; }
		public string phone { get; set; }
		public string birthday { get; set; }
		public string age { get; set; }
		public string title_id { get; set; }
		public string gender { get; set; }
		public string bloodGroup { get; set; }
		public string height { get; set; }
		public string weight { get; set; }

		//public string address { get; set; }
		//public string marital_status_id { get; set; }
		//public string name_ar { get; set; }
		public string Address { get; set; }
		public string nationalityId { get; set; }
		public string nationality { get; set; }
		public string familymembersCount { get; set; }
		public string name_ar { get; set; }
		public string marital_status_id { get; set; }

		public string marital_status { get; set; }
		public string email { get; set; }
		public string IsCash { get; set; }


	}

	public class EYG_Patient_Vital
	{
		public string visitId { get; set; }
		public string visitType { get; set; }
		public string visitDateTime { get; set; }
		public string systolic { get; set; }
		public string diastolic { get; set; }
		public string temperature { get; set; }
		public string respiration { get; set; }
		public string pulse { get; set; }
		public string o2sat { get; set; }
		public string height { get; set; }
		public string weight { get; set; }
	}

	public class EYG_Patient_AppointmentList
	{
		public int id { get; set; }
		public string appointmentNo { get; set; }
		public string registrationNo { get; set; }
		public string clinicName { get; set; }
		public string ClinicID { get; set; }
		public string appDate { get; set; }
		public string appTime { get; set; }
		public string patientVisited { get; set; }
		public string patientName { get; set; }
		public string doctorName { get; set; }
		public string confirmationStatus { get; set; }
		public string confirmationStatus_AR { get; set; }
		public string doctorName_AR { get; set; }
		public string clinicName_AR { get; set; }
		public string isUpComming { get; set; }

		public string doctorId { get; set; }

		
	}

	public class EYG_Patient_LabList
	{	
		public string registration_no { get; set; }
		public string report_type { get; set; }
		public DateTime report_date { get; set; }
		public string opip { get; set; }
		public string test_name { get; set; }
		public string report_filename { get; set; }
		public string ftp_path { get; set; }
		public string report_id { get; set; }
		public string test_id { get; set; }
		public string OrderId { get; set; }		
	}


	public class EYG_TestResult_Details
	{

		public string testCode { get; set; }
		public string testName { get; set; }		
		public string section { get; set; }
		public string sample_name { get; set; }
		public string collected_date { get; set; }
		public List<ResultParameters> parameters { get; set; }


		public EYG_TestResult_Details()
		{
			this.testCode = string.Empty;
			testName = string.Empty;
			section = string.Empty;
			sample_name = string.Empty;
			collected_date = string.Empty;
			parameters = new List<ResultParameters>();
		}

	}
	public class ResultParameters
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


	public class EYG_Doctors_Days
	{
		public int exception { get; set; }
		public int id { get; set; }
		public int departmentID { get; set; }
		public string doctor_Id { get; set; }
		public DateTime scheduled_Day { get; set; }
		public string fromTime { get; set; }
		public string toTime { get; set; }
		public int slotTypeId { get; set; }
		public string slotTypeName { get; set; }
		public int dayBooked { get; set; }
		public int noSchedules { get; set; }


		public string availableSlots { get; set; }
		public string bookedSlots { get; set; }
		public string totalSlotCount { get; set; }
		public string split_Shift { get; set; }
		public string schedule_1 { get; set; }
		public string schedule_2 { get; set; }
		public string sno { get; set; }
		public string pageNo { get; set; }

	}

	public class EYG_Doctors_Slots
	{
		public string id { get; set; }
		public string time_to { get; set; }
		public string time_from { get; set; }
		public int slot_type_id { get; set; }
		public string slot_type_name { get; set; }
	}



	public class EYG_PatientVisit
	{
		public string id { get; set; }
		public string appointmentNo { get; set; }
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


	public class Medical_Perscription_EYGPT
	{

		public string visit_Id { get; set; }
		public string doctor_Name { get; set; }
		public string prescription_Date { get; set; }
		public string drug_Name { get; set; }
		public List<orders> orders { get; set; }
	}
	public class EygptRegistration
	{

		public string phone { get; set; }
		public string registration_no { get; set; }
		public string national_id { get; set; }
		public string name { get; set; }		
	}
	public class Eyg_sms_Resp
	{
		public int status { get; set; }
		public string response { get; set; }
	}


}
