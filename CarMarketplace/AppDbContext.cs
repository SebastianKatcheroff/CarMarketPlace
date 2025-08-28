using CarMarketplace.DAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarMarketplace
{
    public class AppDbContext : DbContext
    {
        public DbSet<Buyers> Buyer { get; set; }
        public DbSet<Quotes> Quote { get; set; }
        public DbSet<Cars> Car { get; set; }
        public DbSet<ZipCode> ZipCode { get; set; }
        public DbSet<CarModel> CarModel { get; set; }
        public DbSet<CarSubModel> CarSubModel { get; set; }
        public DbSet<CarMake> CarMake { get; set; }
        public DbSet<QuoteStatus> QuoteStatus { get; set; }
        public DbSet<Lookup> Lookup { get; set; }
        public DbSet<Customers> Customers { get; set; }        
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=CarMarketPlaceDB;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True;");
        }
    }
}
