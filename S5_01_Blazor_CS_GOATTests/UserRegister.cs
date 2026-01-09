using Microsoft.Playwright;

namespace S5_01_Blazor_CS_GOATTests;

[TestClass]
public class UserRegister : TestBase
{
    
    // Ca marche mais le delete account n'est pas encore implémenté donc c'est mis en commentaire
    // [TestMethod]
    // public async Task UserCanRegisterAndDeleteAccount()
    // {
    //     await Page.GotoAsync($"{BaseUrl}/", new PageGotoOptions
    //     {
    //         WaitUntil = WaitUntilState.DOMContentLoaded
    //     });
    //     
    //     
    //     await Page.WaitForSelectorAsync("text=connexion", new PageWaitForSelectorOptions());
    //     
    //     // Click on connexion button
    //     await Page.GetByText("connexion").First.ClickAsync();
    //     Console.WriteLine("a cliqué sur le bouton connexion");
    //     await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    //     
    //     
    //     
    //     
    //     await Page.GetByText("Créer un compte").First.ClickAsync();
    //     
    //     
    //     // Fill connexion informations
    //     await Page.FillAsync("[id='login']", "registeruser3274329237");
    //     await Page.FillAsync("[id='displayName']", "registeredUserDisplayName");
    //     await Page.FillAsync("[id='password']", "Test123#");
    //     await Page.FillAsync("[id='confirmPassword']", "Test123#");
    //     await Page.FillAsync("[id='email']", "user.toregister@gmail.com");
    //     
    //     
    //     await Page.ClickAsync("button[type='submit']");
    //     Console.WriteLine("click sur le bouton submit");
    //     
    //     
    //     // Wait to get redirected to the home page
    //     await Page.WaitForURLAsync($"{BaseUrl}/", new PageWaitForURLOptions 
    //     { 
    //         Timeout = 30000 
    //     });
    //     Console.WriteLine("attendre d'être redirect à la page principale et de voir le username");
    //     
    //     await Expect(Page.GetByText("registeredUserDisplayName")).ToBeVisibleAsync();
    //     
    // }
}