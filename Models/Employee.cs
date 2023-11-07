using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp_net.Models
{
    public class Employee
    {
        public required int EmployeeId { get; set; }
        public required string EmployeeName { get; set; }
        public required string Department { get; set; }

        public required string DateOfJoining { get; set; }

        public  required string PhotoFileName { get; set; }
    }
}

