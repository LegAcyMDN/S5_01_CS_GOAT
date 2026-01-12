using Microsoft.Playwright;

namespace S5_01_Blazor_CS_GOATTests;

[TestClass]
public class OpenCaseTest : TestBase
{
    [TestMethod]
    public async Task CanFastOpenCase()
    {
        /*
         * identifiant : caseOpener69
         * nom d'affichage : OOO67
         * mdp : Jordan123%
         * email : six.seven@gmail.com
         */
        
        await TestHelpers.LoginAsync(Page, BaseUrl, "caseOpener69", "Jordan123%");
        
        
        // See if the username is at the top right
        await Expect(Page.GetByText("OOO67")).ToBeVisibleAsync();
        
        
        // Click the first case in the list
        await Page.Locator(".case-content").First.ClickAsync();
        
        // Wait for case page to load
        await Page.WaitForURLAsync($"{BaseUrl}/caseview/1", new PageWaitForURLOptions 
        {
            Timeout = 60000
        });
        
        
        await Page.GetByText("ACHETER").First.ClickAsync();
        
        
        // await Page.WaitForTimeoutAsync(60000);
        
        
        await Page.GetByText("Tu as obtenu :")
            .WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 120000
            });
        
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
        await Page.WaitForURLAsync($"{BaseUrl}/caseview/1", new PageWaitForURLOptions 
        { 
            Timeout = 60000
        });
        
        
        
        // Click the slider to put the case in esthetic mode
        await Page.Locator(".slider").First.ClickAsync(new LocatorClickOptions {Force =  true});
        
        // buy case
        await Page.GetByText("ACHETER").First.ClickAsync();
        
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
        
        
        // See if the username is at the top right
        await Expect(Page.GetByText("OOO67")).ToBeVisibleAsync();
        
        
        // Click the first case in the list
        await Page.Locator(".case-content").First.ClickAsync();
        
        // Wait for case page to load
        await Page.WaitForURLAsync($"{BaseUrl}/caseview/1", new PageWaitForURLOptions 
        { 
            Timeout = 60000
        });
        
        
        
        // Click the slider to put the case in esthetic mode
        await Page.Locator(".slider").First.ClickAsync(new LocatorClickOptions {Force =  true});
        
        
        // Select the ten cases button
        await Page.Locator(".letsgo-gambling > .gambling-x:has-text(\"10\")").ClickAsync();
        
        await Page.GetByText("ACHETER").First.ClickAsync();
        
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