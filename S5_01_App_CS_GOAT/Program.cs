using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using S5_01_App_CS_GOAT.Models.DataManager;
using S5_01_App_CS_GOAT.Services;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CSGOATDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("RemoteConnectionString"))
);

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins("https://localhost:7030")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
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

// Readonly repositories for web-scraped entities
builder.Services.AddScoped<IReadableRepository<Case, int>, CrudRepository<Case>>();
builder.Services.AddScoped<IReadableRepository<CaseContent, (int, int)>, CrudRepository<CaseContent, (int,int)>>();
builder.Services.AddScoped<IReadableRepository<PriceHistory, int>, CrudRepository<PriceHistory>>();
builder.Services.AddScoped<IReadableRepository<Skin, int>, CrudRepository<Skin>>();
builder.Services.AddScoped<IReadableRepository<Wear, int>, CrudRepository<Wear>>();

// Type Repositories for type entities
builder.Services.AddScoped<ITypeRepository<BanType>, TypeRepository<BanType>>();
builder.Services.AddScoped<ITypeRepository<LimitType>, TypeRepository<LimitType>>();
builder.Services.AddScoped<ITypeRepository<NotificationType>, TypeRepository<NotificationType>>();
builder.Services.AddScoped<ITypeRepository<PaymentMethod>, TypeRepository<PaymentMethod>>();
builder.Services.AddScoped<ITypeRepository<WearType>, TypeRepository<WearType>>();

// Generic repositories for standard entities
builder.Services.AddScoped<IDataRepository<Ban, int>, CrudRepository<Ban>>();
builder.Services.AddScoped<IDataRepository<FairRandom, int>, CrudRepository<FairRandom>>();
builder.Services.AddScoped<IDataRepository<Favorite, (int,int)>, CrudRepository<Favorite, (int, int)>>();
builder.Services.AddScoped<IDataRepository<GlobalNotification, int>, CrudRepository<GlobalNotification>>();
builder.Services.AddScoped<IDataRepository<InventoryItem, int>, CrudRepository<InventoryItem>>();
builder.Services.AddScoped<IDataRepository<ItemTransaction, int>, CrudRepository<ItemTransaction>>();
builder.Services.AddScoped<IDataRepository<Limit, (int,int)>, CrudRepository<Limit, (int,int)>>();
builder.Services.AddScoped<IDataRepository<MoneyTransaction, int>, CrudRepository<MoneyTransaction>>();
builder.Services.AddScoped<IDataRepository<Notification, int>, CrudRepository<Notification>>();
builder.Services.AddScoped<IDataRepository<NotificationSetting, (int, int)>, CrudRepository<NotificationSetting, (int, int)>>();
builder.Services.AddScoped<IDataRepository<PromoCode, int>, CrudRepository<PromoCode>>();
builder.Services.AddScoped<IDataRepository<RandomTransaction, int>, CrudRepository<RandomTransaction>>();
builder.Services.AddScoped<IDataRepository<Token, int>, CrudRepository<Token>>();
builder.Services.AddScoped<IDataRepository<Transaction, int>, CrudRepository<Transaction>>();
builder.Services.AddScoped<IDataRepository<UpgradeResult, (int,int)>, CrudRepository<UpgradeResult, (int,int)>>();
builder.Services.AddScoped<IDataRepository<UserNotification, int>, CrudRepository<UserNotification>>();

// Custom managers for complex entities
builder.Services.AddScoped<IUserRepository, UserManager>();

// Timed services
builder.Services.AddHostedService<TimedActionService<IDataRepository<Token, int>, Token, int>>();
builder.Services.AddHostedService<TimedActionService<IDataRepository<PromoCode, int>, PromoCode, int>>();

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
