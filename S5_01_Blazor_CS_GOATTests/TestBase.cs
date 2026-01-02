namespace S5_01_Blazor_CS_GOATTests;
using Microsoft.Extensions.Configuration;

public class TestBase : PageTest
{
    protected string BaseUrl { get; private set; } = string.Empty;
    
    [TestInitialize]
    public async Task TestSetup()
    {
        // Load configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{GetEnvironment()}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        BaseUrl = configuration["TestSettings:BaseUrl"] 
                  ?? throw new Exception("BaseUrl not configured");

        Console.WriteLine($"🔍 Testing against: {BaseUrl}");
    }

    private static string GetEnvironment()
    {
        // Check if running in Azure DevOps/GitHub Actions
        var environment = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT");
        return environment ?? "Local";
    }
}