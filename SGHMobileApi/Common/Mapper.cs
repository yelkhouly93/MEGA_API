using DataLayer.Model;
using SGHMobileApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGHMobileApi.Common
{
    public static class Mapper
    {
        public static CancelledAppointmentVm toCancelledAppointmentVm(this CancelledAppointment value) {

            return new CancelledAppointmentVm() {
                Id = value.Id,
                StartTime = value.StartTime,
                EndTime = value.EndTime,
                UpdateDatetime = value.UpdateDatetime,
                PatientName = value.PatientName,
                DoctorName = value.DoctorName,
                DoctorCode = value.DoctorCode,
                OpType = value.OpType
            };
        }

        public static List<CancelledAppointmentVm> toCancelledAppointmentVmList(this List<CancelledAppointment> values)
        {
            var appointments = new List<CancelledAppointmentVm>();

            foreach(var appointment in values) {
                appointments.Add(appointment.toCancelledAppointmentVm());
            };

            return appointments;
        }
    }
}