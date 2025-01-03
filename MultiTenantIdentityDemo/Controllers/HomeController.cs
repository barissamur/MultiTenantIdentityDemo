using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Mvc;
using MultiTenantIdentityDemo.Models;
using System.Diagnostics;

namespace MultiTenantIdentityDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var tenantContext = HttpContext.Items["TenantContext"] as MultiTenantContext<AppTenantInfo>;
            if (tenantContext != null)
            {
                var tenantInfo = tenantContext.TenantInfo;
                ViewBag.TenantName = tenantInfo.Name; // Tenant bilgilerini View'a aktarabilirsiniz
                Console.WriteLine($"Current Tenant: {tenantInfo.Name}");
            }
            else
            {
                Console.WriteLine("No Tenant Found.");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
