using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.DataManager;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;



var builder = WebApplication.CreateBuilder(args);


// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins("https://localhost:7030") // Your Blazor app URL
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // If you're using authentication/cookies
    });
});


// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Generic repositories for standard entities (sorted alphabetically)
builder.Services.AddScoped<IDataRepository<Ban, int>, CrudRepository<Ban>>();
builder.Services.AddScoped<IReadableRepository<Case, int>, CrudRepository<Case>>();
builder.Services.AddScoped<IDataRepository<FairRandom, int>, CrudRepository<FairRandom>>();
builder.Services.AddScoped<IDataRepository<Favorite, (int,int)>, CrudRepository<Favorite, (int, int)>>();
builder.Services.AddScoped<IDataRepository<GlobalNotification, int>, CrudRepository<GlobalNotification>>();
builder.Services.AddScoped<IDataRepository<InventoryItem, int>, CrudRepository<InventoryItem>>();
builder.Services.AddScoped<IDataRepository<ItemTransaction, int>, CrudRepository<ItemTransaction>>();
builder.Services.AddScoped<IDataRepository<Limit, int>, CrudRepository<Limit>>();
builder.Services.AddScoped<IDataRepository<MoneyTransaction, int>, CrudRepository<MoneyTransaction>>();
builder.Services.AddScoped<IReadableRepository<NotificationType, int>, CrudRepository<NotificationType>>();
builder.Services.AddScoped<IReadableRepository<PaymentMethod, int>, CrudRepository<PaymentMethod>>();
builder.Services.AddScoped<IDataRepository<PromoCode, int>, CrudRepository<PromoCode>>();

// Specialized managers with custom interfaces or logic (existing managers)
builder.Services.AddScoped<IDataRepository<User, int>, UserManager>();
builder.Services.AddScoped<IDataRepository<Wear, int>, WearManager>();
builder.Services.AddScoped<IWearRelatedRepository<Wear>, WearManager>();
builder.Services.AddScoped<INotificationRelatedRepository<int?>, UserNotificationManager>();
builder.Services.AddScoped<IDataRepository<UserNotification, int>, UserNotificationManager>();
builder.Services.AddScoped<ISkinRelatedRepository<Skin>, SkinManager>();
builder.Services.AddScoped<IDataRepository<PriceHistory, int>, PriceHistoryManager>();
builder.Services.AddScoped<IDataRepository<UpgradeResult, int>, UpgradeResultManager>();

builder.Services.AddDbContext<CSGOATDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("RemoteConnectionString")));

string? secret = builder.Configuration.GetValue<string>("JWT_SECRET");
if (secret == null) throw new Exception("JWT_SECRET environment variable is not set in appssettings.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.RequireHttpsMetadata = false;
     options.SaveToken = true;
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = builder.Configuration.GetValue<string>("JWT_ISSUER"),
         ValidAudience = builder.Configuration.GetValue<string>("JWT_AUDIENCE"),
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
         ClockSkew = TimeSpan.Zero
     };
 });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazorApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
