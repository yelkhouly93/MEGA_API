using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Model
{
	public class EYG_Patient_Info
	{
		public int hospital_id { get; set; }
		public string branch_EN { get; set; }
		public string branch_AR { get; set; }
		public string registration_no { get; set; }
		public string first_name { get; set; }
		public string middle_name { get; set; }
		public string last_name { get; set; }
		public string family_name { get; set; }
		public string id { get; set; }
		public string name { get; set; }
		public string phone { get; set; }
		public string birthday { get; set; }
		public string age { get; set; }
		public string title_id { get; set; }
		public string gender { get; set; }
		public string bloodGroup { get; set; }
		public string height { get; set; }
		public string weight { get; set; }

		public string maritalStatus { get; set; }
		public string nationalityId { get; set; }
		public string nationality { get; set; }

		public string address { get; set; }

		public string marital_status_id { get; set; }
		public string name_ar { get; set; }
		public string familymembersCount { get; set; }
		//public string marital_status_id { get; set; }
		//public string marital_status_id { get; set; }

		






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
}
