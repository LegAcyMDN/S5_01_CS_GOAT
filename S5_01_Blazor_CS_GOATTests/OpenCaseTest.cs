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
        
        // Navigate to wallet page with longer timeout
        await Page.GotoAsync($"{BaseUrl}/", new PageGotoOptions
        { 
            WaitUntil = WaitUntilState.DOMContentLoaded,
            Timeout = 60000 // 60 seconds for Blazor WASM to load
        });


        Console.WriteLine("page chargé");

        // Wait for Blazor to initialize - look for a specific element that appears when app is ready
        await Page.WaitForSelectorAsync("text=connexion", new PageWaitForSelectorOptions 
        { 
            Timeout = 60000 
        });

        Console.WriteLine("a detecté le bouton connexion");

        // Click on connexion button
        await Page.GetByText("connexion").First.ClickAsync();
        Console.WriteLine("a cliqué sur le bouton connexion");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Console.WriteLine("la page de login s'est affiché");
        
        // Fill connexion informations
        await Page.FillAsync("[id='identifier']", "caseOpener69");
        await Page.FillAsync("[id='password']", "Jordan123%");
        
        // Submit
        await Page.ClickAsync("button[type='submit']");
        Console.WriteLine("click sur le bouton submit");
        
        // Wait to get redirected to the home page
        await Page.WaitForURLAsync($"{BaseUrl}/", new PageWaitForURLOptions 
        { 
            Timeout = 30000 
        });
        Console.WriteLine("attendre d'être redirect à la page principale et de voir le username");
        
        
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
                Timeout = 60000
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
        
        // Navigate to wallet page with longer timeout
        await Page.GotoAsync($"{BaseUrl}/", new PageGotoOptions
        { 
            WaitUntil = WaitUntilState.DOMContentLoaded,
            Timeout = 60000 // 60 seconds for Blazor WASM to load
        });


        Console.WriteLine("page chargé");

        // Wait for Blazor to initialize - look for a specific element that appears when app is ready
        await Page.WaitForSelectorAsync("text=connexion", new PageWaitForSelectorOptions 
        { 
            Timeout = 60000 
        });

        Console.WriteLine("a detecté le bouton connexion");

        // Click on connexion button
        await Page.GetByText("connexion").First.ClickAsync();
        Console.WriteLine("a cliqué sur le bouton connexion");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Console.WriteLine("la page de login s'est affiché");
        
        // Fill connexion informations
        await Page.FillAsync("[id='identifier']", "caseOpener69");
        await Page.FillAsync("[id='password']", "Jordan123%");
        
        // Submit
        await Page.ClickAsync("button[type='submit']");
        Console.WriteLine("click sur le bouton submit");
        
        // Wait to get redirected to the home page
        await Page.WaitForURLAsync($"{BaseUrl}/", new PageWaitForURLOptions 
        { 
            Timeout = 30000 
        });
        Console.WriteLine("attendre d'être redirect à la page principale et de voir le username");
        
        
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
        
        // Navigate to wallet page with longer timeout
        await Page.GotoAsync($"{BaseUrl}/", new PageGotoOptions
        { 
            WaitUntil = WaitUntilState.DOMContentLoaded,
            Timeout = 60000 // 60 seconds for Blazor WASM to load
        });


        Console.WriteLine("page chargé");

        // Wait for Blazor to initialize - look for a specific element that appears when app is ready
        await Page.WaitForSelectorAsync("text=connexion", new PageWaitForSelectorOptions 
        { 
            Timeout = 60000 
        });

        Console.WriteLine("a detecté le bouton connexion");

        // Click on connexion button
        await Page.GetByText("connexion").First.ClickAsync();
        Console.WriteLine("a cliqué sur le bouton connexion");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Console.WriteLine("la page de login s'est affiché");
        
        // Fill connexion informations
        await Page.FillAsync("[id='identifier']", "caseOpener69");
        await Page.FillAsync("[id='password']", "Jordan123%");
        
        // Submit
        await Page.ClickAsync("button[type='submit']");
        Console.WriteLine("click sur le bouton submit");
        
        // Wait to get redirected to the home page
        await Page.WaitForURLAsync($"{BaseUrl}/", new PageWaitForURLOptions 
        { 
            Timeout = 30000 
        });
        Console.WriteLine("attendre d'être redirect à la page principale et de voir le username");
        
        
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