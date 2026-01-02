using Microsoft.Playwright;

namespace S5_01_Blazor_CS_GOATTests;
[TestClass]
public class UserTests : TestBase
{
    [TestMethod]
    public async Task UserCanConnect()
    {
        // Navigate to wallet page
        await Page.GotoAsync($"{BaseUrl}/");

        // Wait for page to load
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Click on connexion button
        await Page.GetByText("connexion").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Fill connexion informations
        //testuser123
        //test.user123@gmail.com
        //Test123#
        await Page.FillAsync("[id='identifier']", "testuser123");
        await Page.FillAsync("[id='password']", "Test123#");
        
        // Submit
        await Page.ClickAsync("button[type='submit']");
        
        // Wait to get redirected to the home page
        await Page.WaitForURLAsync($"{BaseUrl}");
        
        
        // See if the username is at the top right
        await Expect(Page.GetByText("testuser123")).ToBeVisibleAsync();

    }
    
    
    [TestMethod]
    public async Task UserCanConnectAndChangePassword()
    {
        // Navigate to wallet page
        await Page.GotoAsync($"{BaseUrl}/");

        // Wait for page to load
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Click on connexion button
        await Page.GetByText("connexion").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Fill connexion informations
        //testuser123
        //test.user123@gmail.com
        //Test123#
        await Page.FillAsync("[id='identifier']", "testuser123");
        await Page.FillAsync("[id='password']", "Test123#");
        
        // Submit
        await Page.ClickAsync("button[type='submit']");
        
        // Wait to get redirected to the home page
        await Page.WaitForURLAsync($"{BaseUrl}");
        
        
        // See if the username is at the top right
        var burgerMenuButton = Page.GetByText("testuser123");
        await Expect(burgerMenuButton).ToBeVisibleAsync();

        // Open burger menu
        await burgerMenuButton.ClickAsync();

        await Page.WaitForTimeoutAsync(2000); // Wait for burger menu animation
        
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