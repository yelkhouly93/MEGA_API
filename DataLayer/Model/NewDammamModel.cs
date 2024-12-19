using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer.Model
{
    public class patientData_Dam
    {
        public string address { get; set; }
        public string birthday { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string family_name { get; set; }
        public string name { get; set; }
        public string name_ar { get; set; }
        public string gender { get; set; }
        public string hospital_id { get; set; }
        public string marital_status_id { get; set; }
        public string phone { get; set; }
        public string registration_no { get; set; }
        public string title_id { get; set; }
        public string nationalityId { get; set; }
        public string nationality { get; set; }
        public string age { get; set; }
        public string weight { get; set; }
        public string height { get; set; }
        public string bloodGroup { get; set; }
        public string familyMembersCount { get; set; }
        public string patientId { get; set; }
        public string national_id { get; set; }



    }

    public class Medical_Perscription_Dam
    {

        public string prescriptionNo { get; set; }
        public string fileNumber { get; set; }
        public string requester { get; set; }
        public string opip { get; set; }
        public List<orders>  orders { get; set; }
    }
    public class orders
	{
        public string orderNo { get; set; }
        public string quantity { get; set; }
        public string requestDate { get; set; }
        public string drugCode { get; set; }
        public string drugName { get; set; }
        public string drugLocation { get; set; }
        public string dose { get; set; }
        public string dispensingUnit { get; set; }
        public string scheduleOfAdmin { get; set; }
        public string frequency { get; set; }
        public string frequencyDoses { get; set; }
        public string frequencyUom { get; set; }
        public string duration { get; set; }
        public string durationUom { get; set; }


        // For Lab and Rediology
        public string procedureNumber { get; set; }
        public string procedureName { get; set; }
        public string procedureLoc { get; set; }
        public int orderLine { get; set; }

        public string ftpPath { get; set; }



    }

    public class PostResponse
    {
        public  string errorCode { get; set; }
        public string errorMessage { get; set; }

    }

    public class data
    {
        public string patientId { get; set; }
        public string mrn { get; set; }
        
    }
    public class PostResponse_AddPatient
    {
        public string errorCode { get; set; }
        public string code { get; set; }        
        public string errorMessage { get; set; }
        public data data { get; set; }
        

    }



    public class PatientVisit_Dam
	{
        public string id { get; set; }
        public string appointmentNo { get; set; }
        public string registrationNo { get; set; }
        public string clinicName { get; set; }
        public string appDate { get; set; }
        public string appTime { get; set; }
        public string patientVisited { get; set; }
        public string patientName { get; set; }
        public string appointmentType { get; set; }
        public string doctorId { get; set; }
        public string doctorName { get; set; }
        public string videoCallUrl { get; set; }
        public string paid { get; set; }

    }
    

    public class PatientInsurance_Dam
    {
        public string contractNo { get; set; }
        public string contractDesc { get; set; }
        public string purchaserCode { get; set; }
        public string purchaserDesc { get; set; }
        public string policyNo { get; set; }
        public string policyDesc { get; set; }
        public string accountNo { get; set; }
        public string insuranceId { get; set; }
        public string idExpiryDate { get; set; }

    }


    public class AddPatientResponse
	{
        string request_id { get; set; }
        int code { get; }

    }

    public class InsuranceApprovals_Dam
    {        
        public DateTime visitDateTime { get; set; }
        public string visitSpecialty { get; set; }
        public string visitDoctor { get; set; }
        public int visitId { get; set; }
        public int approvalRequestId { get; set; }
        public DateTime createdAt { get; set; }
        public string registrationNo { get; set; }
        public string insuranceComp { get; set; }
        public string doctorId { get; set; }
        public DateTime approvalDateTime { get; set; }
        public string finalApprovalStatus { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }

    }
    public class MedicalReport_Damm
    {
        public int EPISODE_NO { get; set; }
        public string PATIENT_NAME { get; set; }
        public string FILE_ID { get; set; }
        public string DOBG { get; set; }
        public string SEX { get; set; }
        public string VISIT_START_DATE { get; set; }
        public string NAT { get; set; }
        public string DIAG_OPD { get; set; }
        public string HISTORY_MED { get; set; }
        public string PHY_EXAM { get; set; }
        public string INVESTIGATION_MED { get; set; }
        public string RECOMMENDATION_MED { get; set; }
        public string DOC_TRANS { get; set; }
        public string DOC_SPEC { get; set; }
    }

}
