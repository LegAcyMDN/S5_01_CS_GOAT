using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using Radzen;
using S5_01_Blazor_CS_GOAT;
using S5_01_Blazor_CS_GOAT.Models;
using S5_01_Blazor_CS_GOAT.Service;
using S5_01_Blazor_CS_GOAT.ViewModels;
using Shared.DTO;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register services with configuration
builder.Services.AddScoped<IService<Case>>(sp => 
    new WebService<Case>(sp.GetRequiredService<IConfiguration>(), "case"));
builder.Services.AddScoped<IService<SkinDTO>>(sp => 
    new WebService<SkinDTO>(sp.GetRequiredService<IConfiguration>(), "skin"));
builder.Services.AddScoped<IService<User>>(sp => 
    new WebService<User>(sp.GetRequiredService<IConfiguration>(), "user"));
builder.Services.AddScoped<IService<InventoryItemDetail>>(sp => 
    new WebService<InventoryItemDetail>(sp.GetRequiredService<IConfiguration>(), "inventoryitem"));
builder.Services.AddScoped<IThreeDModelService<ThreeDModel>>(sp => 
    new ThreeDModelWebService<ThreeDModel>("wear/get3dmodel"));
builder.Services.AddScoped<IService<MoneyTransaction>>(sp => 
    new WebService<MoneyTransaction>(sp.GetRequiredService<IConfiguration>(), "moneytransaction"));
builder.Services.AddScoped<IService<Limit>>(sp => 
    new WebService<Limit>(sp.GetRequiredService<IConfiguration>(), "limit"));
builder.Services.AddScoped<IService<FairRandomDTO>>(sp => 
    new WebService<FairRandomDTO>(sp.GetRequiredService<IConfiguration>(), "fairrandom"));
builder.Services.AddScoped<IService<PriceHistoryDTO>>(sp => 
    new WebService<PriceHistoryDTO>(sp.GetRequiredService<IConfiguration>(), "pricehistory"));
builder.Services.AddScoped<IService<RandomTransactionDetailDTO>>(sp => 
    new WebService<RandomTransactionDetailDTO>(sp.GetRequiredService<IConfiguration>(), "randomtransaction"));
builder.Services.AddScoped<IService<RandomTransactionLiveFeedDTO>>(sp => 
    new WebService<RandomTransactionLiveFeedDTO>(sp.GetRequiredService<IConfiguration>(), "randomtransaction"));

builder.Services.AddScoped<CacheService>(sp => 
{
    var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
    return new CacheService(sp.GetRequiredService<IJSRuntime>(), httpClient);
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<FavoriteService>();
builder.Services.AddScoped<StripeService>();

// ViewModels
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
builder.Services.AddScoped<AdminViewModel>();
builder.Services.AddScoped<PromoCodeViewModel>();
builder.Services.AddScoped<FairRandomViewModel>();
builder.Services.AddScoped<LimitsViewModel>();

builder.Services.AddTransient<CaseComponentViewModel>();
builder.Services.AddTransient<CaseRollComponentViewModel>();
builder.Services.AddTransient<WeaponDisplayComponentViewModel>();

// HttpClient for other services (AuthService, StripeService, etc.)
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] 
    ?? throw new InvalidOperationException("API Base URL not configured");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

builder.Services.AddRadzenComponents();

var host = builder.Build();

var authService = host.Services.GetRequiredService<AuthService>();
await authService.InitializeAsync();

await host.RunAsync();