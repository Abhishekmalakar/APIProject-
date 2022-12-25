using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace _astoriaTrainingAPI.Models
{
    public partial class AllowanceMaster
    {
        public AllowanceMaster()
        {
            EmployeeAllowanceDetail = new HashSet<EmployeeAllowanceDetail>();
        }

        public int AllowanceId { get; set; }
        public string AllowanceName { get; set; }
        public string AllowanceDiscription { get; set; }

        public virtual ICollection<EmployeeAllowanceDetail> EmployeeAllowanceDetail { get; set; }

    }
}
