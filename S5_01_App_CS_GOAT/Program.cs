using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.DataManager;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddScoped<IDataRepository<Case, int, string>, CaseManager>();
builder.Services.AddScoped<IDataRepository<User, int, string>, UserManager>();
builder.Services.AddScoped<IDataRepository<InventoryItem, int, string>, InventoryItemManager>();
builder.Services.AddScoped<IDataRepository<PaymentMethod, int, string>, PaymentMethodManager>();
builder.Services.AddScoped<IDataRepository<Ban, int, string>, BanManager>();
builder.Services.AddScoped<IDataRepository<FairRandom, int, string>, FairRandomManager>();
builder.Services.AddScoped<IDataRepository<Favorite, int, string>, FavoriteManager>();
builder.Services.AddScoped<IDataRepository<GlobalNotification, int, string>, GlobalNotificationManager>();
builder.Services.AddScoped<IDataRepository<MoneyTransaction, int, string>, MoneyTransactionManager>();
builder.Services.AddScoped<IDataRepository<Wear, int, string>, WearManager>();
builder.Services.AddScoped<IWearRelatedRepository<Wear>, WearManager>();
builder.Services.AddScoped<INotificationRelatedRepository<int?>, UserNotificationManager>();
builder.Services.AddScoped<IDataRepository<UserNotification, int, string>, UserNotificationManager>();
builder.Services.AddScoped<IDataRepository<Limit, int, (int, int)>, LimitManager>();
builder.Services.AddScoped<IDataRepository<PromoCode, int, string>, PromoCodeManager>();
builder.Services.AddScoped<ISkinRelatedRepository<Skin>, SkinManager>();
builder.Services.AddScoped<IDataRepository<PriceHistory, int, string>, PriceHistoryManager>();
builder.Services.AddScoped<IDataRepository<Notification, int, string>, NotificationManager>();
builder.Services.AddScoped<INotificationRepository<Notification>, NotificationManager>();
builder.Services.AddScoped<IDataRepository<UpgradeResult, int, string>, UpgradeResultManager>();
builder.Services.AddScoped<IDataRepository<ItemTransaction, int, string>, ItemTransactionManager>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
