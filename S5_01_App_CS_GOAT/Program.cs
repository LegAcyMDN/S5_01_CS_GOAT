using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.DataManager;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IDataRepository<Case, int, string>, CaseManager>();
builder.Services.AddScoped<IDataRepository<Item, int, string>, ItemManager>();
builder.Services.AddScoped<IDataRepository<User, int, string>, UserManager>();
builder.Services.AddScoped<IDataRepository<InventoryItem, int, string>, InventoryItemManager>();
builder.Services.AddScoped<IDataRepository<PaymentMethod, int, string>, PaymentMethodManager>();


builder.Services.AddDbContext<CSGOATDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("RemoteConnectionString")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
