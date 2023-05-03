using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGHMobileApi.Models
{
    public class CancelledAppointmentVm
    {
        public int Id { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime UpdateDatetime { get; set; }

        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string DoctorCode { get; set; }
        public string BranchCode { get; set; }

        public string OpType { get; set; }

    }
}