using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using S5_01_Blazor_CS_GOAT;
using S5_01_Blazor_CS_GOAT.Models;
using S5_01_Blazor_CS_GOAT.Service;
using S5_01_Blazor_CS_GOAT.ViewModels;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IService<Case>>(sp => new WebService<Case>("case"));
builder.Services.AddScoped<IService<Skin>>(sp => new WebService<Skin>("skin"));
builder.Services.AddScoped<IService<User>>(sp => new WebService<User>("user"));
builder.Services.AddScoped<IService<InventoryItemDetail>>(sp => new WebService<InventoryItemDetail>("inventoryitem"));
builder.Services.AddScoped<IThreeDModelService<ThreeDModel>>(sp => new ThreeDModelWebService<ThreeDModel>("wear/get3dmodel"));
builder.Services.AddScoped<IService<MoneyTransaction>>(sp => new WebService<MoneyTransaction>("moneytransaction"));
builder.Services.AddScoped<IService<Limit>>(sp => new WebService<Limit>("limit"));

builder.Services.AddScoped<CacheService>(sp => 
{
    var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
    return new CacheService(sp.GetRequiredService<IJSRuntime>(), httpClient);
});

builder.Services.AddScoped<AuthService>();

// Enregistrement des ViewModels pour le pattern MVVM
builder.Services.AddScoped<HomeViewModel>();
builder.Services.AddScoped<InventoryViewModel>();
builder.Services.AddScoped<ProfileViewModel>();
builder.Services.AddScoped<ThreeDViewViewModel>();
builder.Services.AddScoped<CaseViewViewModel>();
builder.Services.AddScoped<LoginViewModel>();
builder.Services.AddScoped<WalletViewModel>();
builder.Services.AddScoped<RegisterViewModel>();
builder.Services.AddScoped<HistoryViewModel>();
builder.Services.AddScoped<UpgradeViewModel>();
builder.Services.AddScoped<NavMenuViewModel>();
builder.Services.AddScoped<ConnectMenuViewModel>();
builder.Services.AddScoped<LiveFeedViewModel>();
builder.Services.AddScoped<AuthOverlayViewModel>();

// Enregistrement des ViewModels pour les composants
builder.Services.AddTransient<CaseComponentViewModel>();
builder.Services.AddTransient<CaseRollComponentViewModel>();
builder.Services.AddTransient<WeaponDisplayComponentViewModel>();

// Configuration du HttpClient avec la bonne BaseAddress de l'API
#if DEBUG
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7009/api/") });
#else
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://apicsgoat-h7bhhpd4e7bnc9bh.eastus-01.azurewebsites.net/api/") });
#endif

var host = builder.Build();

var authService = host.Services.GetRequiredService<AuthService>();
await authService.InitializeAsync();

await host.RunAsync();