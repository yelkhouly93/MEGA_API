using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer.Wards.Model
{
    public class ExtraFoodModel
    {
        public string Prefix { get; set; }
        public string Dispatch { get; set; }
        public string sOrderNo { get; set; }
        public string OrderNo { get; set; }
        public string IPID { get; set; }        
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
        
    }
}
