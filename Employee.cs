using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _astoriaTrainingAPI.Models
{
    public class Employee
    {
        public long EmployeeKey { get; set; }

        public string EmployeeID { get; set; }

        public string EmployeeName { get; set; }

        public string Company { get; set; }

        public string Designation { get; set; }

        public DateTime JoiningDate { get; set; }

        public string Gender { get; set; }


    }
}
