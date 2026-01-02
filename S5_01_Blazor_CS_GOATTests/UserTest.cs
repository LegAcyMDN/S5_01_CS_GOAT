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
}