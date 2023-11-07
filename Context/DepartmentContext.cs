using System;

using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using asp_net.Models;


namespace asp_net.Context
{
	public class DepartmentContext : DbContext
    {
        public DepartmentContext(DbContextOptions<DepartmentContext> options) : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
    }
}

