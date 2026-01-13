using Microsoft.Playwright;

namespace S5_01_Blazor_CS_GOATTests;

[TestClass]
[TestCategory("Case")]
public class OpenCaseTest : TestBase
{
    [TestMethod]
    public async Task CanFastOpenCase()
    {
        await TestHelpers.LoginAsync(Page, BaseUrl, "caseOpener69", "Jordan123%");
    
        // Click the first case in the list
        await Page.Locator(".case-content").First.ClickAsync();
    
        // Wait for case page to load
        await Page.WaitForURLAsync($"{BaseUrl}/caseview/**", new PageWaitForURLOptions 
        {
            Timeout = 60000
        });
    
        Console.WriteLine("📦 On case page, clicking ACHETER button...");
        await Page.GetByText("ACHETER").First.ClickAsync();
    
        // Target the button specifically using GetByRole or a more specific selector
        var confirmButton = Page.GetByRole(AriaRole.Button, new() { Name = "Confirmer" });
    
        await Expect(confirmButton).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions 
        { 
            Timeout = 5000 
        });
    
        Console.WriteLine("✅ Popup visible, clicking Confirmer button...");
        await confirmButton.ClickAsync();
    
        Console.WriteLine("🔘 Clicked Confirmer, waiting for case opening...");
    
        // Wait for the result
        await Page.GetByText("Tu as obtenu :")
            .WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 120000 // 2 minutes for case opening animation
            });
    
        Console.WriteLine("✅ Case opened successfully!");
    }
    
    
        [TestMethod]
    public async Task CanEstheticOpenCase()
    {
        /*
         * identifiant : caseOpener69
         * nom d'affichage : OOO67
         * mdp : Jordan123%
         * email : six.seven@gmail.com
         */
        
        await TestHelpers.LoginAsync(Page, BaseUrl, "caseOpener69", "Jordan123%");
        
        
        // Click the first case in the list
        await Page.Locator(".case-content").First.ClickAsync();
        
        // Wait for case page to load
        await Page.WaitForURLAsync($"{BaseUrl}/caseview/**", new PageWaitForURLOptions 
        { 
            Timeout = 60000
        });
        
        
        
        // Click the slider to put the case in esthetic mode
        await Page.Locator(".slider").First.ClickAsync(new LocatorClickOptions {Force =  true});
        
        // buy case
        await Page.GetByText("ACHETER").First.ClickAsync();
        
        var confirmButton = Page.GetByRole(AriaRole.Button, new() { Name = "Confirmer" });
    
        await Expect(confirmButton).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions 
        { 
            Timeout = 5000 
        });
        
        await confirmButton.ClickAsync();
        
        // await Page.Locator(".case-content").First.ClickAsync();
        
        await Page.GetByText("Tu as obtenu :")
            .WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 90000
            });
        
    }
    
    
            [TestMethod]
    public async Task CanEstheticOpenTenCases()
    {
        /*
         * identifiant : caseOpener69
         * nom d'affichage : OOO67
         * mdp : Jordan123%
         * email : six.seven@gmail.com
         */
        
        await TestHelpers.LoginAsync(Page, BaseUrl, "caseOpener69", "Jordan123%");
        
        
        // Click the first case in the list
        await Page.Locator(".case-content").First.ClickAsync();
        
        // Wait for case page to load
        await Page.WaitForURLAsync($"{BaseUrl}/caseview/**", new PageWaitForURLOptions 
        { 
            Timeout = 60000
        });
        
        
        
        // Click the slider to put the case in esthetic mode
        await Page.Locator(".slider").First.ClickAsync(new LocatorClickOptions {Force =  true});
        
        
        // Select the ten cases button
        await Page.Locator(".letsgo-gambling > .gambling-x:has-text(\"10\")").ClickAsync();
        
        await Page.GetByText("ACHETER").First.ClickAsync();
        
        var confirmButton = Page.GetByRole(AriaRole.Button, new() { Name = "Confirmer" });
    
        await Expect(confirmButton).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions 
        { 
            Timeout = 5000 
        });
        
        await confirmButton.ClickAsync();
        
        // await Page.Locator(".case-content").First.ClickAsync();
        
        await Page.GetByText("Tu as obtenu :")
            .WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 60000 * 3
            });
    
    
        await Expect(Page.Locator(".popup-skins > li"))
            .ToHaveCountAsync(10);
    
    }
    
}