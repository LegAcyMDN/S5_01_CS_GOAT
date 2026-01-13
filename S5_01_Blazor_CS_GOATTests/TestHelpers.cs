using Microsoft.Playwright;

namespace S5_01_Blazor_CS_GOATTests;

/// <summary>
/// Helper methods for E2E tests
/// </summary>
public static class TestHelpers
{
    /// <summary>
    /// Performs login process and waits until redirected to home page
    /// </summary>
    /// <param name="page">Playwright page instance</param>
    /// <param name="baseUrl">Base URL of the application</param>
    /// <param name="username">Login identifier</param>
    /// <param name="password">User password</param>
    public static async Task LoginAsync(IPage page, string baseUrl, string username, string password)
    {
        await page.GotoAsync($"{baseUrl}/", new PageGotoOptions
        { 
            WaitUntil = WaitUntilState.DOMContentLoaded,
            Timeout = 60000 // 60 seconds for Blazor WASM to load
        });


        Console.WriteLine("page chargé");

        // Wait for Blazor to initialize - look for a specific element that appears when app is ready
        await page.WaitForSelectorAsync("text=connexion", new PageWaitForSelectorOptions 
        { 
            Timeout = 60000 
        });

        Console.WriteLine("a detecté le bouton connexion");

        // Click on connexion button
        await page.GetByText("connexion").First.ClickAsync();
        Console.WriteLine("a cliqué sur le bouton connexion");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Console.WriteLine("la page de login s'est affiché");
        
        // Fill connexion informations
        await page.FillAsync("[id='identifier']", username);
        await page.FillAsync("[id='password']", password);
        
        // Submit
        await page.ClickAsync("button[type='submit']");
        Console.WriteLine("click sur le bouton submit");
        
        
        // await page.WaitForTimeoutAsync(10000);
        //
        // Console.WriteLine(await page.ContentAsync());
        
        // Wait to get redirected to the home page
        await page.WaitForURLAsync($"{baseUrl}/", new PageWaitForURLOptions 
        { 
            Timeout = 60000 
        });
    }
    
    /// <summary>
    /// Navigates to a specific case page
    /// </summary>
    /// <param name="page">Playwright page instance</param>
    /// <param name="baseUrl">Base URL of the application</param>
    /// <param name="caseId">ID of the case to open</param>
    public static async Task NavigateToCaseAsync(IPage page, string baseUrl, int caseId)
    {
        Console.WriteLine($"[NAV] Navigating to case {caseId}");
        
        // Click the case with the specified ID or index
        await page.Locator($".case-content").Nth(caseId - 1).ClickAsync();
        
        // Wait for case page to load
        await page.WaitForURLAsync($"{baseUrl}/caseview/{caseId}", new PageWaitForURLOptions 
        { 
            Timeout = 60000
        });
        
        Console.WriteLine($"[NAV] Case page {caseId} loaded");
    }
    
    /// <summary>
    /// Enables esthetic mode for case opening
    /// </summary>
    /// <param name="page">Playwright page instance</param>
    public static async Task EnableEstheticModeAsync(IPage page)
    {
        Console.WriteLine("[ESTHETIC] Enabling esthetic mode");
        await page.Locator(".slider").First.ClickAsync(new LocatorClickOptions { Force = true });
        Console.WriteLine("[ESTHETIC] Esthetic mode enabled");
    }
    
    /// <summary>
    /// Selects the number of cases to open
    /// </summary>
    /// <param name="page">Playwright page instance</param>
    /// <param name="count">Number of cases (1, 2, 3, 4, 5, or 10)</param>
    public static async Task SelectCaseCountAsync(IPage page, int count)
    {
        Console.WriteLine($"[SELECT] Selecting {count} cases");
        await page.Locator($".letsgo-gambling > .gambling-x:has-text(\"{count}\")").ClickAsync();
        Console.WriteLine($"[SELECT] Selected {count} cases");
    }
    
    /// <summary>
    /// Clicks the buy button and waits for the result popup
    /// </summary>
    /// <param name="page">Playwright page instance</param>
    /// <param name="timeoutMs">Timeout in milliseconds (default: 60000)</param>
    public static async Task BuyCaseAndWaitForResultAsync(IPage page, int timeoutMs = 60000)
    {
        Console.WriteLine("[BUY] Clicking buy button");
        await page.GetByText("ACHETER").First.ClickAsync();
        
        Console.WriteLine("[BUY] Waiting for result popup");
        await page.GetByText("Tu as obtenu :")
            .WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = timeoutMs
            });
        
        Console.WriteLine("[BUY] Result popup displayed");
    }
}

/// <summary>
/// Test user credentials
/// </summary>
public static class TestUsers
{
    public static class CaseOpener
    {
        public const string Username = "caseOpener69";
        public const string DisplayName = "OOO67";
        public const string Password = "Jordan123%";
        public const string Email = "six.seven@gmail.com";
    }
}