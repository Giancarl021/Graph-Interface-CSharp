using System;

namespace GraphInterface.Auth;
public class GraphInterfaceCredentials
{
    public string TenantId { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public GraphInterfaceCredentials(string tenantId, string clientId, string clientSecret)
    {
        TenantId = tenantId;
        ClientId = clientId;
        ClientSecret = clientSecret;

        if (string.IsNullOrWhiteSpace(TenantId))
            throw new ArgumentException("TenantId is required");
        if (string.IsNullOrWhiteSpace(ClientId))
            throw new ArgumentException("ClientId is required");
        if (string.IsNullOrWhiteSpace(ClientSecret))
            throw new ArgumentException("ClientSecret is required");
    }
    private GraphInterfaceCredentials()
    {
        TenantId = string.Empty;
        ClientId = string.Empty;
        ClientSecret = string.Empty;
    }
    public static GraphInterfaceCredentials Empty { get; } = new();
}
