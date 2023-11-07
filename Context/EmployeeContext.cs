using System;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using asp_net.Models;

using Microsoft.EntityFrameworkCore;
namespace asp_net.Context
{
	public class EmployeeContext : DbContext
    {
        public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
    }
}

