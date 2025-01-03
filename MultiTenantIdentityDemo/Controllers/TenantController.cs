using Microsoft.AspNetCore.Mvc;
using MultiTenantIdentityDemo.Manager;
using MultiTenantIdentityDemo.Models;

namespace MultiTenantIdentityDemo.Controllers;

public class TenantController : Controller
{
    private readonly TenantManager _tenantManager;

    public TenantController(TenantManager tenantManager)
    {
        _tenantManager = tenantManager;
    }

    [HttpPost]
    public async Task<IActionResult> AddTenant(string identifier, string name)
    {
        if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(name))
        {
            return BadRequest("Tenant identifier ve name zorunludur.");
        }

        try
        {
            var tenantInfo = new AppTenantInfo
            {
                Id = Guid.NewGuid().ToString(),
                Identifier = identifier,
                Name = name,
                ConnectionString = $"Server=(localdb)\\mssqllocaldb;Database={identifier}Db;Trusted_Connection=True;"
            };
             

            return Ok($"Tenant '{name}' ve veritabanı başarıyla oluşturuldu!");
        }
        catch (Exception ex)
        {
            // Hata durumunda geri bildirim ver
            return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
        }
    }
}
