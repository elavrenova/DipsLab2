﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace AuthServer
{
    public class Config
    {
        private const int Expiration = 600;

        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>();
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api", "API")
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients() =>
            // client credentials client
            new List<Client>
            {
                new Client
                {
                    ClientId = "clientId",
                    ClientName = "Test client",
                    AllowedGrantTypes = GrantTypes.Code.Concat(GrantTypes.ResourceOwnerPassword).ToList(),
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    RequireConsent = false,
                    RedirectUris = { "http://localhost:60992" },
                    EnableLocalLogin = true,
                    AccessTokenLifetime = Expiration,
                    IdentityTokenLifetime = Expiration,
                    AuthorizationCodeLifetime = Expiration,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "api"
                    },
                    ClientSecrets = new List<Secret>() { new Secret("Secret".Sha256()) }
                }
            };

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "User1",
                    Password = "pass1"
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "User2",
                    Password = "pass2"
                }
            };
        }
    }
}
