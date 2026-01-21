using System.Security.Claims;
using AspNet.Security.OpenId;
using Microsoft.AspNetCore.Authentication;
using S5_01_App_CS_GOAT.Models.DataManager;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using Shared.DTO.Helpers;

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
                        string? steamIdClaim = context.Identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                        if (string.IsNullOrEmpty(steamIdClaim))
                        {
                            return;
                        }

                        string steamId = steamIdClaim.Replace("https://steamcommunity.com/openid/id/", "");

                        Microsoft.AspNetCore.Http.HttpContext httpContext = context.HttpContext;
                        bool isLinkingMode = context.Properties.Items.ContainsKey("linkUserId");

                        if (isLinkingMode)
                        {
                            context.Identity.AddClaim(new Claim("steamid", steamId));
                            return;
                        }

                        SteamManager steamService = httpContext.RequestServices.GetRequiredService<SteamManager>();
                        IUserRepository userRepository = httpContext.RequestServices.GetRequiredService<IUserRepository>();

                        SteamUserData? steamUserData = await steamService.GetSteamUserDataAsync(steamId);

                        if (steamUserData == null)
                        {
                            return;
                        }

                        User? user = await userRepository.GetBySteamIdAsync(steamId);

                        if (user == null)
                        {
                            string randomSalt = SecurityService.GenerateToken(32);
                            string randomPassword = SecurityService.GenerateToken(64);
                            string hashedPassword = SecurityService.HashAndSalt(randomPassword, randomSalt);

                            user = new User
                            {
                                SteamId = steamId,
                                Login = $"steam_{steamId}",
                                DisplayName = steamUserData.Username,
                                Email = null,
                                Phone = null,
                                SaltPassword = randomSalt,
                                HashPassword = hashedPassword,
                                TwoFaIsPhone = false,
                                TwoFaIsEmail = false,
                                IsAdmin = false,
                                CreationDate = DateTime.UtcNow,
                                LastLogin = DateTime.UtcNow,
                                Wallet = 0.0,
                                DeletedOn = null,
                                Seed = SecurityService.GenerateSeed(16),
                                Nonce = 0
                            };

                            await userRepository.AddAsync(user);
                        }
                        else
                        {
                            if (user.Login.StartsWith("steam_"))
                            {
                                user.DisplayName = steamUserData.Username;
                            }
                            user.LastLogin = DateTime.UtcNow;
                            await userRepository.UpdateAsync(user);
                        }

                        context.Identity.AddClaim(new Claim("user_id", user.UserId.ToString()));
                        context.Identity.AddClaim(new Claim("steamid", steamId));
                        context.Identity.AddClaim(new Claim("username", user.DisplayName));
                    },

                    OnRemoteFailure = context =>
                    {
                        context.HandleResponse();
                        string URL = configuration["Urls:BlazorFrontend"] ?? "https://localhost:7030";
                        context.Response.Redirect($"{URL}?error=steam_auth_failed");
                        return Task.CompletedTask;
                    }
                };
            });

            return builder;
        }
    }
}