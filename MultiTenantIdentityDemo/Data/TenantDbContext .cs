using Finbuckle.MultiTenant.EntityFrameworkCore.Stores.EFCoreStore;
using Microsoft.EntityFrameworkCore;
using MultiTenantIdentityDemo.Models;

namespace MultiTenantIdentityDemo.Data
{
    public class TenantDbContext : EFCoreStoreDbContext<AppTenantInfo>
    {
        public TenantDbContext(DbContextOptions<TenantDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } // Kullanıcıya özel tablolar
    }
}
