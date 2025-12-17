using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using S5_01_Blazor_CS_GOAT;
using S5_01_Blazor_CS_GOAT.Models;
using S5_01_Blazor_CS_GOAT.Service;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IService<Case>>(sp => new WebService<Case>("case"));
builder.Services.AddScoped<IService<Skin>>(sp => new WebService<Skin>("skin"));
builder.Services.AddScoped<IService<User>>(sp => new WebService<User>("user"));
builder.Services.AddScoped<IService<InventoryItemDetail>>(sp => new WebService<InventoryItemDetail>("inventoryitem"));
builder.Services.AddScoped<IThreeDModelService<ThreeDModel>>(sp => new ThreeDModelWebService<ThreeDModel>("wear/get3dmodel"));
builder.Services.AddScoped<IService<MoneyTransaction>>(sp => new WebService<MoneyTransaction>("moneytransaction"));






builder.Services.AddScoped<CacheService>(sp => 
{
    var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
    return new CacheService(sp.GetRequiredService<IJSRuntime>(), httpClient);
});

builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var host = builder.Build();

var authService = host.Services.GetRequiredService<AuthService>();
await authService.InitializeAsync();

await host.RunAsync();