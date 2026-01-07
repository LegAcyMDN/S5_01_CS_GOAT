using Microsoft.Playwright;

namespace S5_01_Blazor_CS_GOATTests;
[TestClass]
public class UserTests : TestBase
{
    
    [TestMethod]
    public async Task DebugServerConnection()
    {
        Console.WriteLine($"🧪 Attempting to navigate to: {BaseUrl}");
    
        try
        {
            var response = await Page.GotoAsync(BaseUrl, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 10000
            });
        
            Console.WriteLine($"✅ Navigation successful! Status: {response.Status}");
            Console.WriteLine($"📄 URL: {Page.Url}");
            Console.WriteLine($"📄 Title: {await Page.TitleAsync()}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Navigation failed: {ex.Message}");
            throw;
        }
    }
    
    
    [TestMethod]
    public async Task UserCanConnect()
    {
        // Navigate to wallet page with longer timeout
await Page.GotoAsync($"{BaseUrl}/", new PageGotoOptions  // LA PIPELINE TEST SANS LE / DONC CA MARCHE PAS
        { 
            WaitUntil = WaitUntilState.DOMContentLoaded,
            Timeout = 60000 // 60 seconds for Blazor WASM to load
        });

        // Wait for Blazor to initialize - look for a specific element that appears when app is ready
        await Page.WaitForSelectorAsync("text=connexion", new PageWaitForSelectorOptions 
        { 
            Timeout = 60000 
        });

        // Click on connexion button
        await Page.GetByText("connexion").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Fill connexion informations
        await Page.FillAsync("[id='identifier']", "testuser123");
        await Page.FillAsync("[id='password']", "Test123#");
        
        // Submit
        await Page.ClickAsync("button[type='submit']");
        
        // Wait to get redirected to the home page
        await Page.WaitForURLAsync($"{BaseUrl}/", new PageWaitForURLOptions 
        { 
            Timeout = 30000 
        });
        
        // See if the username is at the top right
        await Expect(Page.GetByText("testuser123")).ToBeVisibleAsync();
    }
    
    
    [TestMethod]
    public async Task UserCanConnectAndChangePassword()
    {
        // Navigate to wallet page with longer timeout
        await Page.GotoAsync($"{BaseUrl}/", new PageGotoOptions 
        { 
            WaitUntil = WaitUntilState.DOMContentLoaded,
            Timeout = 60000 
        });

        // Wait for Blazor to initialize
        await Page.WaitForSelectorAsync("text=connexion", new PageWaitForSelectorOptions 
        { 
            Timeout = 60000 
        });

        // Click on connexion button
        await Page.GetByText("connexion").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Fill connexion informations
        await Page.FillAsync("[id='identifier']", "testuser123");
        await Page.FillAsync("[id='password']", "Test123#");
        
        // Submit
        await Page.ClickAsync("button[type='submit']");
        
        // Wait to get redirected to the home page
        await Page.WaitForURLAsync($"{BaseUrl}/", new PageWaitForURLOptions 
        { 
            Timeout = 30000 
        });
        
        // See if the username is at the top right
        var burgerMenuButton = Page.GetByText("testuser123");
        await Expect(burgerMenuButton).ToBeVisibleAsync();

        // Open burger menu
        await burgerMenuButton.ClickAsync();

        await Page.WaitForTimeoutAsync(2000);
        
        // Click profile button
        await Page.GetByText("Profile").ClickAsync();
        
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Fill current password and new password
        await Page.FillAsync("[id='oldPassword']", "Test123#");
        await Page.FillAsync("[id='newPassword']", "Test123##");
        await Page.FillAsync("[id='confirmPassword']", "Test123##");
        
        // Submit the password change
        await Page.GetByText("Changer le mot de passe").ClickAsync();
        
        // See if the password was changed successfully
        await Expect(Page.GetByText("Mot de passe modifié avec succès !")).ToBeVisibleAsync();
        
        // Change password back to the old password
        await Page.FillAsync("[id='oldPassword']", "Test123##");
        await Page.FillAsync("[id='newPassword']", "Test123#");
        await Page.FillAsync("[id='confirmPassword']", "Test123#");
        
        // Submit the password change
        await Page.GetByText("Changer le mot de passe").ClickAsync();
        
        // See if the password was changed successfully
        await Expect(Page.GetByText("Mot de passe modifié avec succès !")).ToBeVisibleAsync();
    }
}