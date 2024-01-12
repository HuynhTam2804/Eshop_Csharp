using Eshop.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace Eshop.Data
{
    public class EshopContext : DbContext
    {
        public EshopContext(DbContextOptions<EshopContext>
        options) : base(options)
        { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
