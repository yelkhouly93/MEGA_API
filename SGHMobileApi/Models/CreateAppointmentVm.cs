using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace SGHMobileApi.Models
{
    public class CreateAppointmentVm
    {
        [Required(ErrorMessage = "DoctorID is required")]
        public int DoctorId { get; set; }
        [Required(ErrorMessage = "ScheduleDayID is required")]
        public int ScheduleDayId { get; set; }
        public int Age { get; set; }
        public string SexType { get; set; }
        [Required(ErrorMessage = "FromDateTime is required")]
        public DateTime FromDateTime { get; set; }
        [Required(ErrorMessage = "ToDateTime is required")]
        public DateTime ToDateTime { get; set; }

        [Required(ErrorMessage = "ToDateTime is required")]
        public string PatientName { get; set; }
        public string PhoneNo { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public string Remarks { get; set; }
    }

    public class UpdateAppointmentVm
    {
        [Required(ErrorMessage = "AppointmentID is required")]
        public int AppointmentId { get; set; }
        [Required(ErrorMessage = "DoctorID is required")]
        public int DoctorId { get; set; }
        [Required(ErrorMessage = "ScheduleDayID is required")]
        public int ScheduleDayId { get; set; }
        [Required(ErrorMessage = "FromDateTime is required")]
        public DateTime FromDateTime { get; set; }
        [Required(ErrorMessage = "ToDateTime is required")]
        public DateTime ToDateTime { get; set; }
        public string Remarks { get; set; }
    }
}