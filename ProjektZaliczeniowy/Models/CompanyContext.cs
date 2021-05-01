using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektZaliczeniowy.Models
{
    public class CompanyContext : DbContext //db proxy - uchwyt
    {
        public CompanyContext(DbContextOptions<CompanyContext> options): base(options)
        {

        }

        public DbSet<Employee2> Employee2s { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee2>().HasKey(c => c.BusinessEntityID); //klucz
            modelBuilder.Entity<Employee2>().ToTable("Employee2");
        }
    }
}
