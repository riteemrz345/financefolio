using FinanceFolio.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace FinanceFolio.Data
{

    public class AppDbcontext: DbContext
    {

        public AppDbcontext(DbContextOptions<AppDbcontext> opt): base(opt)
        {
            
        }
        public DbSet<ExpenseDto> Expenses{ get; set; }
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ExpenseDto>().ToCollection("Expenses");
        }
    }
}
