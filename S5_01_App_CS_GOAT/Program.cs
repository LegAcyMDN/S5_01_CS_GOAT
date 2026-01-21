using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using S5_01_App_CS_GOAT.Configuration;
using S5_01_App_CS_GOAT.Models.DataManager;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

/// <summary>
/// CS:GOAT Application Startup Configuration
/// 
/// This program configures the ASP.NET Core application with:
/// - PostgreSQL database context with Entity Framework Core
/// - Multi-scheme authentication: JWT tokens (API authentication), Cookie-based Steam OAuth (web authentication)
/// - CORS policy for Blazor frontend communication
/// - Dependency injection containers for all service, repository, and manager layers
/// - Automatic mapper for DTO conversions
/// - Middleware pipeline for HTTP request processing
/// - Hosted background services for scheduled token/promo code cleanup
/// </summary>

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ============================================================================
// DATABASE CONFIGURATION
// ============================================================================
// Configures PostgreSQL database connection via Entity Framework Core
// RemoteConnectionString contains the connection details for the production database
builder.Services.AddDbContext<CSGOATDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("RemoteConnectionString"))
);

// ============================================================================
// CORS AND COOKIE POLICY
// ============================================================================
// Enables Cross-Origin Resource Sharing for the Blazor frontend and local development environments.
// The Blazor frontend URL is loaded from configuration to allow flexible deployment URLs.
// AllowCredentials() required for JWT tokens in cookies and authentication headers.
string BlazorUrl = builder.Configuration["Urls:BlazorFrontend"] ?? throw new Exception("Blazor frontend URL is not configured.");
builder.Services.AddCors(options => options.AddPolicy("AllowBlazorApp", policy => policy.WithOrigins(
                BlazorUrl,
                "https://localhost:7030",
                "http://localhost:7030",
                "https://127.0.0.1:7030",
                "http://127.0.0.1:7030"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));

// Cookie security policy: Lax SameSite prevents CSRF attacks while allowing cookie submission on same-site requests.
// Secure flag ensures cookies only transmitted over HTTPS.
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    options.Secure = CookieSecurePolicy.Always; // Use HTTPS only
});

// ============================================================================
// CONTROLLERS AND JSON SERIALIZATION
// ============================================================================
// Configures JSON serialization for API responses:
// - IgnoreCycles: Prevents infinite loops when serializing circular entity references (e.g., User -> Inventory -> User)
// - PropertyNameCaseInsensitive: Allows both PascalCase (C#) and camelCase (JavaScript) property names
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// ============================================================================
// API DOCUMENTATION AND MAPPING
// ============================================================================
// Swagger/OpenAPI: Generates interactive API documentation with [ProducesResponseType] attributes
// AutoMapper: Automatically maps DTOs to domain entities across all loaded assemblies
// HttpClientFactory: Typed HTTP client for external service calls (PayPal, Stripe, Steam API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient();

// ============================================================================
// REPOSITORY DEPENDENCY INJECTION
// ============================================================================
// Read-Only Repositories: For web-scraped static reference data (cases, skins, wear conditions)
// - These entities are populated from external sources and shouldn't be modified by the API
builder.Services.AddScoped<IReadableRepository<Case, int>, ReadRepository<Case>>();
builder.Services.AddScoped<IReadableRepository<CaseContent, (int, int)>, ReadRepository<CaseContent, (int, int)>>();
builder.Services.AddScoped<IReadableRepository<Skin, int>, ReadRepository<Skin>>();
builder.Services.AddScoped<IReadableRepository<Wear, int>, ReadRepository<Wear>>();

// Type Repositories: For enumeration-like entities (ban types, notification types, payment methods)
// - TypeRepository provides optimized queries for type entities that rarely change
builder.Services.AddScoped<ITypeRepository<BanType>, TypeRepository<BanType>>();
builder.Services.AddScoped<ITypeRepository<LimitType>, TypeRepository<LimitType>>();
builder.Services.AddScoped<ITypeRepository<NotificationType>, TypeRepository<NotificationType>>();
builder.Services.AddScoped<ITypeRepository<PaymentMethod>, TypeRepository<PaymentMethod>>();
builder.Services.AddScoped<ITypeRepository<WearType>, TypeRepository<WearType>>();

// CRUD Repositories: For standard entities with Create, Read, Update, Delete operations
// - CrudRepository handles generic queries and mutations with proper change tracking
// - Composite keys used as tuples (e.g., (int, int) for many-to-many relationship entities)
builder.Services.AddScoped<IDataRepository<Ban, int>, CrudRepository<Ban>>();
builder.Services.AddScoped<IDataRepository<Favorite, (int, int)>, CrudRepository<Favorite, (int, int)>>();
builder.Services.AddScoped<IDataRepository<GlobalNotification, int>, CrudRepository<GlobalNotification>>();
builder.Services.AddScoped<IDataRepository<InventoryItem, int>, CrudRepository<InventoryItem>>();
builder.Services.AddScoped<IDataRepository<ItemTransaction, int>, CrudRepository<ItemTransaction>>();
builder.Services.AddScoped<IDataRepository<Limit, (int, int)>, CrudRepository<Limit, (int, int)>>();
builder.Services.AddScoped<IDataRepository<MoneyTransaction, int>, CrudRepository<MoneyTransaction>>();
builder.Services.AddScoped<IDataRepository<Notification, int>, CrudRepository<Notification>>();
builder.Services.AddScoped<IDataRepository<NotificationSetting, (int, int)>, CrudRepository<NotificationSetting, (int, int)>>();
builder.Services.AddScoped<IDataRepository<RandomTransaction, int>, CrudRepository<RandomTransaction>>();
builder.Services.AddScoped<IDataRepository<Token, int>, CrudRepository<Token>>();
builder.Services.AddScoped<IDataRepository<Transaction, int>, CrudRepository<Transaction>>();
builder.Services.AddScoped<IDataRepository<UpgradeResult, (int, int)>, CrudRepository<UpgradeResult, (int, int)>>();
builder.Services.AddScoped<IDataRepository<UserNotification, int>, CrudRepository<UserNotification>>();

// Custom managers for complex entities
// - CaseOpenningManager: Orchestrates case opening with random selection and transaction handling
// - FairRandomManager: Implements provably fair randomization using server/user seed combination
// - PayPalManager: Handles PayPal Checkout API integration for order creation and capture
// - PriceHistoryManager: Maintains historical prices and interfaces with Flask ML service for predictions
// - PromoCodeManager: Validates, consumes, and manages promotion code lifecycle
// - UserManager: Comprehensive user account operations (auth, profile, GDPR export, verification)
// - SellingManager: Manages item sales with wallet crediting and transaction recording
// - SendingManager: Sends verification codes via email/SMS with rate limiting
// - SteamManager: Integrates Steam API for user profile data and OAuth authentication
// - StripeManager: Handles Stripe payment processing with webhook handlers for events
// - UpgradeManager: Calculates upgrade probabilities and manages item combination transactions
builder.Services.AddScoped<ICaseOpenningRepository, CaseOpenningManager>();
builder.Services.AddScoped<IFairRandomRepository, FairRandomManager>();
builder.Services.AddScoped<IPriceHistoryRepository, PriceHistoryManager>();
builder.Services.AddScoped<IPayPalRepository, PayPalManager>();
builder.Services.AddScoped<IPromoCodeRepository, PromoCodeManager>();
builder.Services.AddScoped<IUserRepository, UserManager>();
builder.Services.AddScoped<ISellingRepository, SellingManager>();
builder.Services.AddScoped<ISendingRepository, SendingManager>();
builder.Services.AddHttpClient<ISteamRepository, SteamManager>();
builder.Services.AddScoped<IStripeRepository, StripeManager>();
builder.Services.AddScoped<IUpgradeRepository, UpgradeManager>();

// ============================================================================
// BACKGROUND SERVICES
// ============================================================================
// Hosted Services: Long-running tasks that execute on scheduled intervals
// - Token cleanup: Removes expired verification tokens periodically
// - PromoCode cleanup: Deactivates expired promo codes and resets monthly limits
builder.Services.AddHostedService<TimedActionService<IDataRepository<Token, int>, Token, int>>();
builder.Services.AddHostedService<TimedActionService<IPromoCodeRepository, PromoCode, int>>();

// ============================================================================
// AUTHENTICATION CONFIGURATION
// ============================================================================
// Multi-scheme authentication setup:
// 1. JWT Bearer: For API authentication (client sends token in Authorization header)
// 2. Cookie: For web-based Steam OAuth authentication (persistent user sessions)
// 3. Steam OAuth: External OAuth provider via custom configuration extension
// 
// JWT Secret must be configured in appsettings.json or environment variables.
// Asymmetric signing recommended for production (RSA instead of HMAC).
string? secret = builder.Configuration.GetValue<string>("Jwt:Secret");
if (secret == null)
{
    throw new Exception("Jwt Secret environment variable is not set in appssettings.");
}

builder.Services.AddAuthentication(options =>
    {
        // Default scheme for API endpoints is JWT Bearer
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        // Sign-in scheme used when redirecting to login (Steam OAuth)
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    // Cookie authentication scheme for web-based Steam OAuth login persistence
    .AddCookie(options =>
    {
        // Redirect to Steam login endpoint when authentication is required but session expired
        options.LoginPath = "/api/steam/login";
        // Session validity: 7 days
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        // Sliding expiration: Resets expiry time with each request (extends session if actively used)
        options.SlidingExpiration = true;
    })
    // Steam OAuth provider configured via extension method in SteamAuthConfiguration.cs
    .ConfigureSteamAuth(builder.Configuration)
    // JWT Bearer authentication for API calls (mobile app, external clients)
    .AddJwtBearer(options =>
    {
        // Allow HTTP for development; set to true in production with HTTPS enforced
        options.RequireHttpsMetadata = false;
        // Save token to HttpContext so controllers can access raw JWT string if needed
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Verify the token was issued by this application (Issuer)
            ValidateIssuer = true,
            // Verify the token targets this application (Audience)
            ValidateAudience = true,
            // Reject expired tokens
            ValidateLifetime = true,
            // Verify token signature matches issuer's secret key
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            // Secret key for HMAC signature validation (must match key used to sign tokens in JwtService)
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            // No clock skew tolerance: strict expiry time validation (production security best practice)
            ClockSkew = TimeSpan.Zero
        };
    });

WebApplication app = builder.Build();

// ============================================================================
// MIDDLEWARE PIPELINE CONFIGURATION
// ============================================================================
// CRITICAL: Middleware order affects security and functionality. Current order:
// 1. Swagger (dev only) - API documentation UI
// 2. HTTPS Redirection - Force HTTPS for all requests
// 3. Cookie Policy - Enforce secure/SameSite cookie handling
// 4. CORS - Allow cross-origin requests from Blazor frontend
// 5. Authentication - Validate JWT/Cookie credentials
// 6. Authorization - Check user permissions ([Authorize] attributes)
// 7. Routing/Controllers - Map requests to controller actions

// Enable Swagger/OpenAPI in development environment only
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

// Redirect all HTTP requests to HTTPS (required for secure authentication)
app.UseHttpsRedirection();

// Enforce cookie policy (SameSite and Secure flags)
app.UseCookiePolicy();

// Enable CORS for configured Blazor frontend origins
app.UseCors("AllowBlazorApp");

// Validate JWT tokens and Steam OAuth cookies
app.UseAuthentication();

// Enforce authorization policies ([Authorize], [AllowAnonymous], role checks)
app.UseAuthorization();

// Route requests to appropriate controller actions
app.MapControllers();

// Start the application and listen for HTTP requests
app.Run();
