using Microsoft.AspNetCore.Authentication;
using AspNet.Security.OpenId;
using System.Security.Claims;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Configuration
{
    public static class SteamAuthConfiguration
    {
        public static AuthenticationBuilder ConfigureSteamAuth(this AuthenticationBuilder builder, IConfiguration configuration)
        {
            builder.AddSteam(options =>
            {
                options.ApplicationKey = configuration["Steam:ApiKey"];
                options.CallbackPath = "/signin-steam";
                options.SaveTokens = true;
                
                options.Events = new OpenIdAuthenticationEvents
                {
                    OnAuthenticated = async context =>
                    {
                        // 1. Extraire le SteamID depuis les claims
                        var steamIdClaim = context.Identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        
                        if (string.IsNullOrEmpty(steamIdClaim))
                        {
                            Console.WriteLine("❌ No SteamID found in claims");
                            return;
                        }
                        
                        // Nettoyer le SteamID (retirer le préfixe OpenID)
                        var steamId = steamIdClaim.Replace("https://steamcommunity.com/openid/id/", "");
                        Console.WriteLine($"✅ Steam authentication successful for SteamID: {steamId}");
                        
                        // 2. Récupérer les services depuis le DI container
                        var httpContext = context.HttpContext;
                        var steamService = httpContext.RequestServices.GetRequiredService<SteamUserService>();
                        var userRepository = httpContext.RequestServices.GetRequiredService<IUserRepository>();
                        
                        // 3. Appeler l'API Steam pour obtenir les détails
                        var steamUserData = await steamService.GetSteamUserDataAsync(steamId);
                        
                        if (steamUserData == null)
                        {
                            Console.WriteLine("❌ Failed to fetch Steam user data from API");
                            return;
                        }
                        
                        Console.WriteLine($"📦 Steam data retrieved: {steamUserData.Username}");
                        
                        // 4. Créer ou mettre à jour l'utilisateur dans la DB
                        var user = await userRepository.GetBySteamIdAsync(steamId);
                        
                        if (user == null)
                        {
                            // Créer un nouvel utilisateur avec un mot de passe aléatoire
                            Console.WriteLine("➕ Creating new user from Steam account");
                            
                            // Générer un sel et un mot de passe aléatoire pour l'utilisateur OAuth
                            string randomSalt = SecurityService.GenerateToken(32); // 32 bytes = ~43 chars en base64
                            string randomPassword = SecurityService.GenerateToken(64); // Un mot de passe très long et aléatoire
                            string hashedPassword = SecurityService.HashAndSalt(randomPassword, randomSalt);
                            
                            user = new User
                            {
                                SteamId = steamId,
                                Login = $"steam_{steamId}",
                                DisplayName = steamUserData.Username,
                                Email = null, // Steam ne partage pas l'email
                                Phone = null, // Steam ne partage pas le téléphone
                                SaltPassword = randomSalt, // ✅ Sel généré aléatoirement
                                HashPassword = hashedPassword, // ✅ Hash généré aléatoirement
                                TwoFaIsPhone = false,
                                TwoFaIsEmail = false,
                                IsAdmin = false,
                                CreationDate = DateTime.UtcNow,
                                LastLogin = DateTime.UtcNow,
                                Wallet = 0.0,
                                DeletedOn = null,
                                Seed = SecurityService.GenerateSeed(16), // ✅ Seed pour provably fair
                                Nonce = 0
                            };
                            
                            await userRepository.AddAsync(user);
                            
                            Console.WriteLine($"✅ User created with ID: {user.UserId}");
                            Console.WriteLine($"🔐 Random credentials generated for OAuth user");
                        }
                        else
                        {
                            // Mettre à jour les infos (username peut changer sur Steam)
                            Console.WriteLine($"🔄 Updating existing user ID: {user.UserId}");
                            
                            user.DisplayName = steamUserData.Username;
                            user.LastLogin = DateTime.UtcNow;
                            
                            await userRepository.UpdateAsync(user);
                            
                            Console.WriteLine("✅ User updated");
                        }
                        
                        // 5. Ajouter le UserID aux claims pour l'utiliser plus tard
                        context.Identity.AddClaim(new Claim("user_id", user.UserId.ToString()));
                        context.Identity.AddClaim(new Claim("steamid", steamId));
                        context.Identity.AddClaim(new Claim("username", user.DisplayName));
                        
                        Console.WriteLine($"✅ Claims added for user {user.UserId}");
                    },
                    
                    OnRemoteFailure = context =>
                    {
                        Console.WriteLine($"❌ Steam authentication failed: {context.Failure?.Message}");
                        context.HandleResponse();
                        context.Response.Redirect($"https://localhost:7030?error=steam_auth_failed");
                        return Task.CompletedTask;
                    }
                };
            });
            
            return builder;
        }
    }
}