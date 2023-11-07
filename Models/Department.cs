using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp_net.Models
{
    public class Department
    {
        public  required int DepartmentId { get; set; }

        public required string DepartmentName { get; set; }
    }
}