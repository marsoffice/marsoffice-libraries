using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace MarsOffice.Microfunction
{
    class AadPrincipal
    {
        public string Auth_typ { get; set; }
        public IEnumerable<AadPrincipalClaim> Claims { get; set; }
    }

    class AadPrincipalClaim
    {
        public string Typ { get; set; }
        public string Val { get; set; }
    }

    public static class MarsOfficePrincipal
    {
        public static ClaimsPrincipal Parse(HttpRequest req)
        {
            try
            {
                var headerString = req.Headers["x-ms-client-principal"].ToString();
                var decoded = Convert.FromBase64String(headerString);
                var json = $"{Encoding.ASCII.GetString(decoded)}";
                var principal = JsonSerializer.Deserialize<AadPrincipal>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var claims = new List<Claim>() {
                    new Claim("name", principal.Claims.Single(x => x.Typ == "name").Val),
                    new Claim("id", principal.Claims.Single(x => x.Typ == "http://schemas.microsoft.com/identity/claims/objectidentifier").Val),
                };
                claims.AddRange(
                    principal.Claims.Where(x => x.Typ == "roles").Select(x => new Claim(x.Typ, x.Val)).ToList()
                );
                claims.AddRange(
                    principal.Claims.Where(x => x.Typ == "groups").Select(x => new Claim(x.Typ, x.Val)).ToList()
                );
                var emailValue = "";
                var foundEmailClaim = principal.Claims.Where(x => x.Typ == ClaimTypes.Email).FirstOrDefault();
                if (foundEmailClaim != null)
                {
                    emailValue = foundEmailClaim.Val;
                }
                else
                {
                    foundEmailClaim = principal.Claims.Where(x => x.Typ == ClaimTypes.Name).FirstOrDefault();
                    if (foundEmailClaim != null)
                    {
                        var split = foundEmailClaim.Val.Split("#");
                        if (split.Length > 1)
                        {
                            emailValue = split.Last();
                        }
                        else
                        {
                            emailValue = split[0];
                        }
                    }
                }
                claims.Add(new Claim("email", emailValue));
                return new ClaimsPrincipal(new ClaimsIdentity(claims, "EasyAuth", "name", "roles"));
            }
            catch (Exception)
            {
                return new ClaimsPrincipal(new ClaimsIdentity());
            }
        }
    }
}
