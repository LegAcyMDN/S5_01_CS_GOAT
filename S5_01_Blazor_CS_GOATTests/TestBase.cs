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
    
        Console.WriteLine("🔧 Configuring browser context options...");
    
        // For local testing, ignore HTTPS certificate errors
        var environment = GetEnvironment();
        if (environment == "Local")
        {
            Console.WriteLine("🔓 Setting IgnoreHTTPSErrors = true for local testing");
            options.IgnoreHTTPSErrors = true;
        }
    
        Console.WriteLine($"📋 Context options: IgnoreHTTPSErrors={options.IgnoreHTTPSErrors}");
    
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
        
        // ... existing setup code ...
    
        // Monitor all API requests
        Page.Request += (_, request) =>
        {
            if (request.Url.Contains("/api/"))
            {
                Console.WriteLine($"[API REQUEST] {request.Method} {request.Url}");
                if (request.Method == "POST" && request.PostDataBuffer != null)
                {
                    try
                    {
                        var body = System.Text.Encoding.UTF8.GetString(request.PostDataBuffer);
                        // Console.WriteLine($"[REQUEST BODY] {body}");
                    }
                    catch { }
                }
            }
        };
    
        Page.Response += async (_, response) =>
        {
            if (response.Url.Contains("/api/"))
            {
                Console.WriteLine($"[API RESPONSE] {response.Status} {response.Url}");
                if (response.Status != 200)
                {
                    try
                    {
                        var body = await response.TextAsync();
                        Console.WriteLine($"[RESPONSE BODY] {body}");
                    }
                    catch { }
                }
            }
        };
    
        Page.RequestFailed += (_, request) =>
        {
            Console.WriteLine($"[REQUEST FAILED] {request.Url} - {request.Failure}");
        };
        
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

    // [TestCleanup]
    // public async Task TestCleanup()
    // {
    //     // Save video only on failure
    //     if (TestContext.CurrentTestOutcome == UnitTestOutcome.Failed)
    //     {
    //         try
    //         {
    //             var videoPath = await Page.Video.PathAsync();
    //             var testName = TestContext.TestName;
    //             var destinationPath = $"test-results/videos/{testName}-{DateTime.Now:yyyyMMdd-HHmmss}.webm";
    //             
    //             // Ensure directory exists
    //             Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
    //             
    //             // Copy video
    //             if (File.Exists(videoPath))
    //             {
    //                 File.Copy(videoPath, destinationPath, overwrite: true);
    //                 Console.WriteLine($"📹 Video saved: {destinationPath}");
    //             }
    //         }
    //         catch (Exception ex)
    //         {
    //             Console.WriteLine($"⚠️ Could not save video: {ex.Message}");
    //         }
    //     }
    // }
    //     [TestCleanup]
    // public async Task Cleanup()
    // {
    //     // Check if test failed
    //     if (TestContext.CurrentTestOutcome == UnitTestOutcome.Failed)
    //     {
    //         Console.WriteLine("════════════════════════════════════════════════════════");
    //         Console.WriteLine("TEST FAILED - Dumping page content:");
    //         Console.WriteLine("════════════════════════════════════════════════════════");
    //         
    //         try
    //         {
    //             // Get current URL
    //             Console.WriteLine($"Current URL: {Page.Url}");
    //             Console.WriteLine("────────────────────────────────────────────────────────");
    //             
    //             // Get page HTML content
    //             var content = await Page.ContentAsync();
    //             Console.WriteLine("HTML Content:");
    //             Console.WriteLine(content);
    //             Console.WriteLine("────────────────────────────────────────────────────────");
    //             
    //             // Get console logs if available
    //             Console.WriteLine("Console Messages:");
    //             // Note: Console messages need to be captured during the test
    //             
    //             // Take screenshot (saved to test results)
    //             var screenshotPath = Path.Combine(
    //                 TestContext.TestRunDirectory ?? ".", 
    //                 $"{TestContext.TestName}_failed_{DateTime.Now:yyyyMMdd_HHmmss}.png"
    //             );
    //             
    //             await Page.ScreenshotAsync(new PageScreenshotOptions
    //             {
    //                 Path = screenshotPath,
    //                 FullPage = true
    //             });
    //             
    //             Console.WriteLine($"Screenshot saved: {screenshotPath}");
    //             TestContext.AddResultFile(screenshotPath);
    //         }
    //         catch (Exception ex)
    //         {
    //             Console.WriteLine($"Error capturing page state: {ex.Message}");
    //         }
    //         
    //         Console.WriteLine("════════════════════════════════════════════════════════");
    //     }
    //
    //     // Cleanup resources
    //     if (Page != null)
    //         await Page.CloseAsync();
    //     
    //     if (Browser != null)
    //         await Browser.CloseAsync();
    //     
    //     Playwright?.Dispose();
    // }
}