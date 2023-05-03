using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer.Wards.Model
{
    public class DrugReturnModel
    {
        public string Prefix { get; set; }
        public string Status { get; set; }
        public string sOrderNo { get; set; }
        public string OrderNo { get; set; }
        public string IPID { get; set; }
        public string DrugOrderID { get; set; }
        public string IssueAuthoritycode { get; set; }
        public string PIN { get; set; }
        public string RegistrationNo { get; set; }
        public string PatientName { get; set; }
        public string DoctorID { get; set; }
        public string BedID { get; set; }
        public string BedName { get; set; }
        public string stationslno { get; set; }
        public string StationID { get; set; }
        public string DateTime { get; set; }
        public string OperatorID { get; set; }
        public string OperatorName { get; set; }
    }
    public class DrugList
    {
        public List<DrugModel> liSelected = new List<DrugModel>();
        public List<DrugModel> liDrugList = new List<DrugModel>();
    }
    public class DrugModel
    {
        public int Row { get; set; }
        public string OrderID { get; set; }
        public string ServiceID { get; set; }
        public string BatchID { get; set; }
        public string DrugOrderID{ get; set; }

        public string SNO { get; set; }
        public string DrugName{ get; set; }
        public string UnitID { get; set; }
        public string UnitName { get; set; }
        public string InputQuantity { get; set; }
        public string Quantity { get; set; }
        public string Remarks { get; set; }
        public string Batchno { get; set; }
        
    }

    public class OrderNoList
    {
        public string Prefix { get; set; }
        public string OrderID { get; set; }
        public string StationSLNo { get; set; }
        public string PrescriptionID { get; set; }
    }
}
