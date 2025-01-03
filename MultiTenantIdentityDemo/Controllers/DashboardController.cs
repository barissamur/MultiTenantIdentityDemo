using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Mvc;

namespace MultiTenantIdentityDemo.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            var tenantInfo = HttpContext.GetMultiTenantContext<TenantInfo>()?.TenantInfo;

            if (tenantInfo == null)
            {
                return NotFound("Kiracı bulunamadı.");
            }

            ViewBag.TenantName = tenantInfo.Name;
            return View();
        }
    }
}
