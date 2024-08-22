using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiMafperu.Models
{
    public class Graph
    {
        private static IConfidentialClientApplication _confidentialClientApplication;
        public Graph() 
        {
            
        }

        public IConfidentialClientApplication GenerarConfidentialGraph(string clientId, string clientSecret, string authority)
        {
            _confidentialClientApplication = ConfidentialClientApplicationBuilder
              .Create(clientId)
              .WithClientSecret(clientSecret)
              .WithAuthority(new Uri(authority))
              //.WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
              .Build();
            return _confidentialClientApplication;
        }

    }
    public class TokenProvider : IAccessTokenProvider
    {
        private readonly ClientSecretCredential _credential;
        private readonly TokenRequestContext _tokenRequestContext;

        public TokenProvider(string tenantId, string clientId, string clientSecret)
        {
            _credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            _tokenRequestContext = new TokenRequestContext(new[] { "https://graph.microsoft.com/.default" });
        }

        public async Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object> additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
        {
            // Obtén un token de acceso válido
            var token = await _credential.GetTokenAsync(_tokenRequestContext, cancellationToken);
            return token.Token;
        }

        public AllowedHostsValidator AllowedHostsValidator { get; } = new AllowedHostsValidator();
    }
}
