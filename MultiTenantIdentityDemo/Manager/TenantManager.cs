using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.Stores;
using Microsoft.EntityFrameworkCore;
using MultiTenantIdentityDemo.Data;
using MultiTenantIdentityDemo.Models;
using System;
using System.Threading.Tasks;

namespace MultiTenantIdentityDemo.Manager
{
    public class TenantManager
    {
        private readonly IMultiTenantStore<AppTenantInfo> _tenantStore;

        public TenantManager(IMultiTenantStore<AppTenantInfo> tenantStore)
        {
            _tenantStore = tenantStore;
        }

        public async Task AddTenantAsync(string identifier, string name)
        {
            var tenant = new AppTenantInfo
            {
                Id = Guid.NewGuid().ToString(),
                Identifier = identifier,
                Name = name,
                ConnectionString = $"Server=(localdb)\\mssqllocaldb;Database={identifier}Db;Trusted_Connection=True;"
            };

            var added = await _tenantStore.TryAddAsync(tenant);
            if (!added)
            {
                throw new InvalidOperationException($"Tenant already exists: {identifier}");
            }

            // Tenant için veritabanı oluştur
            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseSqlServer(tenant.ConnectionString);

            using var context = new TenantDbContext(optionsBuilder.Options);
            await context.Database.EnsureCreatedAsync(); // Veritabanını oluştur
        }

    }
}
