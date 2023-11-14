using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

public class CustomAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return Task.FromResult(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
    }

    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        if (string.IsNullOrEmpty(policyName))
        {
            return GetDefaultPolicyAsync();
        }

        if (policyName == "AdminPolicy")
        {
            // Create and configure an authorization policy
            var policy = new AuthorizationPolicyBuilder()
                .RequireRole("Admin") // Example: Require the "Admin" role
                .Build();

            return Task.FromResult(policy);
        }
        else if (policyName == "CustomPolicy")
        {
            // Create and configure another custom authorization policy
            var policy = new AuthorizationPolicyBuilder()
                // Add additional requirements or options
                .Build();

            return Task.FromResult(policy);
        }

        // If the policy name is not recognized, return the default policy
        return GetDefaultPolicyAsync();
    }

    public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
    {
        return Task.FromResult<AuthorizationPolicy>(null);
    }
}
