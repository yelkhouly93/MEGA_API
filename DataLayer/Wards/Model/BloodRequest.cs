using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer.Wards.Model
{
    public class BloodRequest
    {
        public string Prefix { get; set; }
        public string Status { get; set; }
        public string sOrderNo { get; set; }
        public string OrderNo { get; set; }
        public string IPID { get; set; }
        public string BloodOrderID { get; set; }
        public string IssueAuthoritycode { get; set; }
        public string PIN { get; set; }
        public string RegistrationNo { get; set; }
        public string PatientName { get; set; }
        public string BedID { get; set; }
        public string BedName { get; set; }
        public string StationID { get; set; }
        public string StationName { get; set; }
        public string DateTime { get; set; }
        public string OperatorID { get; set; }
        public string OperatorName { get; set; }
        public string Docid { get; set; }

        public string Diagnosis { get; set; }

        public string TypeofRequest { get; set; }
        public string TypeofTransfusion { get; set; }
        public string Donor { get; set; }
        public string WBC { get; set; }
        public string RBC { get; set; }
        public string PCV { get; set; }
        public string HB { get; set; }
        public string Platelet { get; set; }
        public string Others { get; set; }
        public string PT { get; set; }
        public string PTTK { get; set; }
        public string EarlierDefect { get; set; }

        public List<BloodDetail> SelectedBlood { get; set; }
        public List<BloodDetail> BloodList { get; set; }
    }

    public class BloodDetail
    {
        public string ComponentID { get; set; }
        public string Quantity { get; set; }
        public string Remarks { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string DemandQuantity { get; set; }
        public string PrevQuantity { get; set; }
        public string RequiredDate { get; set; }
    }
}
