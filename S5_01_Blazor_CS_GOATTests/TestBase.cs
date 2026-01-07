using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace S5_01_Blazor_CS_GOATTests;

public class TestBase : PageTest
{
    protected string BaseUrl { get; private set; } = string.Empty;
    
    public override BrowserNewContextOptions ContextOptions()
    {
        var options = base.ContextOptions();
        
        // For local testing, ignore HTTPS certificate errors
        var environment = GetEnvironment();
        if (environment == "Local")
        {
            options.IgnoreHTTPSErrors = true;
        }
        
        return options;
    }
    
    [TestInitialize]
    public async Task TestSetup()
    {
        // Load configuration
        var environment = GetEnvironment();
        Console.WriteLine($"🌍 Environment: {environment}");
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        BaseUrl = configuration["TestSettings:BaseUrl"] 
                  ?? throw new Exception("BaseUrl not configured");

        Console.WriteLine($"🔍 Testing against: {BaseUrl}");
        
        // Wait for deployment to be ready
        if (environment == "Azure")
        {
            Console.WriteLine("⏳ Waiting 30s for Azure deployment to stabilize...");
            await Task.Delay(30000);
        }
    }

    private static string GetEnvironment()
    {
        // Check GitHub Actions environment variable
        var testEnv = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT");
        
        if (!string.IsNullOrEmpty(testEnv))
        {
            Console.WriteLine($"Using TEST_ENVIRONMENT: {testEnv}");
            return testEnv;
        }
        
        // Check if running in CI
        var isCI = Environment.GetEnvironmentVariable("CI");
        if (isCI == "true")
        {
            Console.WriteLine("Detected CI environment");
            return "Azure";
        }
        
        Console.WriteLine("Using Local environment");
        return "Local";
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        // Save video only on failure
        if (TestContext.CurrentTestOutcome == UnitTestOutcome.Failed)
        {
            try
            {
                var videoPath = await Page.Video.PathAsync();
                var testName = TestContext.TestName;
                var destinationPath = $"test-results/videos/{testName}-{DateTime.Now:yyyyMMdd-HHmmss}.webm";
                
                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
                
                // Copy video
                if (File.Exists(videoPath))
                {
                    File.Copy(videoPath, destinationPath, overwrite: true);
                    Console.WriteLine($"📹 Video saved: {destinationPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Could not save video: {ex.Message}");
            }
        }
    }
}