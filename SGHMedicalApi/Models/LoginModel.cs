using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SGHMedicalApi.Models
{
    public class LoginModel
    {
    }

    public class CustomPrincipalSerializedModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string UserRoleDesc { get; set; }
        public int UserRole { get; set; }
        public string IpAddress { get; set; }
    }

    public class WebApiLoginModel
    {
        public bool IsComplete { get; set; }
        public string message { get; set; }
        public string __sghis { get; set; }
        public List<WebApiLoginModel_EmployeeDetails> EmployeeDetails { get; set; }

    }

    public class WebApiLoginModel_EmployeeDetails
    {
        public string ID { get; set; }
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }

    public class _logWebApi
    {
        [Required(ErrorMessage = "Employee Id is required")]
        public string EmployeeId { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Module Key is required")]
        public string ModuleKey { get; set; }
        [Required(ErrorMessage = "Application ID is required")]
        public string AppID { get; set; }

    }
}