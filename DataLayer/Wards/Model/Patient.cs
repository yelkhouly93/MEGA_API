using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer.Wards.Model
{
    public class Patient
    {
        public string PIN { get; set; }
        public string RegNo { get; set; }
        public string PatientName { get; set; }

        public string BedNo { get; set; }
        public string Age { get; set; }
        public string Sex { get; set; }

        public string BloodGroup { get; set; }
        public string DoctorID { get; set; }
        public string DoctorName { get; set; }
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string Drug { get; set; }
        public string Package { get; set; }

        public string OperatorID { get; set; }
        public string OperatorName { get; set; }

        public string IPID { get; set; }
        public string Diagnosis { get; set; }

        public string Prefix { get; set; }
        public string Dispatch { get; set; }
        public string sOrderNo { get; set; }
        public string OrderNo { get; set; }
        public string IssueAuthoritycode { get; set; }
        public string RegistrationNo { get; set; }
        public string BedID { get; set; }
        public string BedName { get; set; }
        public string StationID { get; set; }
        public string StationName { get; set; }
        public string DateTime { get; set; }
        public string AdmitDateTime { get; set; }
        public string DischargeDate { get; set; }
        public string Stationlno { get; set; }

        public string DrugDispatch { get; set; }
        public string DrugAck { get; set; }
        public string DrugTakeHome { get; set; }

        public string PrescriptionOrderStatus { get; set; }
        public string PrescriptionOrderType { get; set; }

        public string InvExecuted { get; set; }
        public string InvStat { get; set; }
        public string InvPriority { get; set; }
        public string InvBedStat { get; set; }
        public string InvPatientStat { get; set; }
        public string InvPhlem { get; set; }
        public string InvTestDone { get; set; }
        public string InvTestDate { get; set; }

        public string MedEquID { get; set; }
        public string MedStart { get; set; }
        public string MedEnd { get; set; }
    }

    public class Allergies
    {
        public string Description { get; set; }
    }
    public class Doctor
    {
        public string ID { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
    }

    public class ItemCode
    {

        public int Row { get; set; }
        public string ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string GenericID { get; set; }
        public string UnitID { get; set; }
        public string UnitName { get; set; }
        public string Quantity { get; set; }

        public string ItemID { get; set; }
        public string PresID { get; set; }
        public string GenericName { get; set; }
        public string Dose { get; set; }
        public string DoseName { get; set; }

        public string FrequencyID { get; set; }
        public string Frequency { get; set; }

        public string Duration { get; set; }
        public string DurationName { get; set; }
        public string DurationID { get; set; }

        public string Administer { get; set; }
        public string AdministerID { get; set; }

        public string RouteofAdminID { get; set; }

        public string Timing { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Remarks { get; set; }

        public string Discontinue { get; set; }
        public string DiscontinueDate { get; set; }
    }

    public class LaboratoryTest
    {
        public int Row { get; set; }
        public string ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }

        public string ProfileID { get; set; }
        public string Profile { get; set; }
        public string Section { get; set; }
        public string SampleID { get; set; }
        public string Sample { get; set; }
        public string Status { get; set; }
        public string CollectedBy { get; set; }
        public string TestDoneBy { get; set; }
        public string VerifiyBy { get; set; }
        public string DestID { get; set; }
        public string Remarks { get; set; }
    }

    public class ResultsView
    {
        public string iRow { get; set; }
        public string iiRow { get; set; }
        public string iiiRow { get; set; }

        public int Row { get; set; }

        public string OrderNo { get; set; }
        public string sOrderNo { get; set; }
        public string Doctor{ get; set; }
        public string Section{ get; set; }

        public string TestID { get; set; }
        public string Code{ get; set; }
        public string TestName{ get; set; }
        public string OrderDateTime{ get; set; }

        public string TestDoneBy { get; set; }
        public string VerifiyBy { get; set; }
        public string CollectedBy { get; set; }

        public string DateCompleted { get; set; }
        public string PType { get; set; }
        public string BillNo { get; set; }
        public string VisitDate { get; set; }
        public string Room { get; set; }

        public string Qty { get; set; }
        public string Unit { get; set; }
        public string OrderStat { get; set; }
        public string Operator { get; set; }

        public string Blank { get; set; }
    }
    public class ItemList
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
    public class ExtraFood
    {
        public int Row { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Units { get; set; }
        public int Quantity { get; set; }
    }


    public class OrderNo
    {
        public string Prefix { get; set; }
        public string OrderID { get; set; }
        public string Station { get; set; }
        public string Prescription { get; set; }
        
    }

    public class Response
    {
        public string Message { get; set; }
    }


    public class InvestigationDetail
    {
        public string Remarks { get; set; }
        public string DateTime { get; set; }
        public string ToBeDoneBY { get; set; }
        public string ExStatus { get; set; }
        public string Phlebotomy { get; set; }
        public string PatientStatus { get; set; }
        public string TransDoneFromStationID { get; set; }
        public string Priority { get; set; }
        public string ToBeDoneAt { get; set; }
    }

    public class PatientRefModel
    {
        public string PIN { get; set; }
        public string RegNo { get; set; }
        public string PatientName { get; set; }

        public string Prefix { get; set; }
        public string Dispatch { get; set; }
        public string sOrderNo { get; set; }
        public string OrderNo { get; set; }
        public string IssueAuthoritycode { get; set; }
        public string RegistrationNo { get; set; }
        public string DateTime { get; set; }

        public string DoctorID { get; set; }
        public string DoctorName { get; set; }

        public string RefDoctor { get; set; }
        public string RefDoctorName { get; set; }
        public string RefReason { get; set; }
        public string Remarks { get; set; }
        public string RefDate { get; set; }
        public string Operator { get; set; }

        public string IPID { get; set; }
        public string BedID { get; set; }
        public string BedNo { get; set; }
    }

    public class PatientFolder
    {
        public List<PatientFolderList> IPList = new List<PatientFolderList>();
        public List<PatientFolderList> OPList = new List<PatientFolderList>();
    }

    public class PatientFolderList
    {
        public string DateString { get; set; }
        public string IPID { get; set; }
        public string VisitID { get; set; }
    }

    public class BedList
    {
        public string BedName { get; set; }
        public string PatientName { get; set; }
        public string IPID { get; set; }
        public string RegNo { get; set; }
        public string PIN { get; set; }
    }
}

