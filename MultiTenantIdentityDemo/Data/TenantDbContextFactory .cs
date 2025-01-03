using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MultiTenantIdentityDemo.Data;

namespace MultiTenantIdentityDemo
{
    public class TenantDbContextFactory : IDesignTimeDbContextFactory<TenantDbContext>
    {
        public TenantDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            // Tasarım zamanında kullanılacak bir bağlantı dizesi
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DefaultTenantDb;Trusted_Connection=True;");
            return new TenantDbContext(optionsBuilder.Options);
        }
    }
}
