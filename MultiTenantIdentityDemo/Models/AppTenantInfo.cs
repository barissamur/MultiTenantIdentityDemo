using Finbuckle.MultiTenant.Abstractions;

namespace MultiTenantIdentityDemo.Models;

public class AppTenantInfo : ITenantInfo
{
    public string Id { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public string Items { get; set; } = string.Empty;
}
