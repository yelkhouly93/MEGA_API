using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer.Model
{
    //model calsses for Damam ApiCalls
    public class accountData
    {
        public string idNumber { get; set; }
        public string idType { get; set; }
    }

    public class accountData_dam
    {
        public string idValue { get; set; }
        public string idType { get; set; }
    }

    public class patientData
    {
        public int PATIENT_ID { get; set; }
        public string AGE { get; set; }
        public int AGE_YEAR { get; set; }
        public float? BMI { get; set; }
        public float? BSA { get; set; }
        public string DATE_OF_BIRTH { get; set; }
        public string GUARDIAN_ID { get; set; }
        public float? HIGHT { get; set; }
        public string MARITAL_CODE { get; set; }

        public string MARITAL_DESC { get; set; }
        public string MOBILE_NO { get; set; }

        public string MRN { get; set; }
        public int? NATIONALITY_CODE { get; set; }
        public string OCCUPATION_CODE { get; set; }
        public string OCCUPATION_DESC { get; set; }
        public string PATIENT_NAME { get; set; }
        public string PAT_NAME_1 { get; set; }
        public string PAT_NAME_2 { get; set; }

        public string PAT_NAME_3 { get; set; }
        public string PAT_NAME_FAMILY { get; set; }


        public string RH { get; set; }
        public string RH_DESC { get; set; }
        public char? SEX { get; set; }
        public string SEX_DESC { get; set; }

        public string WEIGHT { get; set; }
        public string EMAIL_ADDRESS { get; set; }
        public string PHONE { get; set; }


    }
    public class ClinicsByApi
    {
        public int code { get; set; }
        public string description { get; set; }
    }   

    public class StaffList
    {
        public string CONSULTANT { get; set; }
        public string STAFF_NAME { get; set; }
        public int JULIAN_DATE { get; set; }
        public string STAFF_SEX { get; set; }
        public string START_DATE { get; set; }
    }

    public class AvailableDaysModelApi
    {
        public string DATE { get; set; }
        public List<StaffList> STAFF_LIST { get; set; }
    }

    public class AvailableSlotsModelApi
    {
        public int appointmentId { get; set; }
        public DateTime startDate { get; set; }
        public string slotTitle { get; set; }
        public string workEntityDesc { get; set; }
        public object staffName { get; set; }
        public object hejDate { get; set; }
    }

    public class SaveAppointmentRequestBody
    {
        public string appointmentId { get; set; }
    }

    public class ErrorResponseObject
    {
        public string timestamp { get; set; }
        public string message { get; set; }
        public string details { get; set; }
    }

    public class PatientReservations
    {
        public int appointmentId { get; set; }
        public DateTime startDate { get; set; }
        public object slotTitle { get; set; }
        public string workEntityDesc { get; set; }
        public string staffName { get; set; }
        public string hejDate { get; set; }
    }

    public class DescriptionLine
    {
        public string line { get; set; }
    }

    public class PatientMedicationsFromApi
    {
        public string mainDescription { get; set; }
        public string orderType { get; set; }
        public List<DescriptionLine> descriptionLines { get; set; }
        public int id { get; set; }
    }

    public class PatientDiagnosisFromApi
    {
        public int DIAG_CODE { get; set; }
        public object DIAGNOSIS_OUTCOME_CODE { get; set; }
        public string DESC1 { get; set; }
        public object DESC2 { get; set; }
        public object DESC3 { get; set; }
        public DateTime DATE_RECORDED { get; set; }
    }


    public class NewUserRegsitrationFromApi
    {
        public DateTime dateOfBirth { get; set; }
        public string familyName { get; set; }
        public string firstName { get; set; }
        public string idNumber { get; set; }
        public string mobileNo { get; set; }
        public int nationalityCode { get; set; }
        public string secandName { get; set; }
        public string sex { get; set; }
    }

}
