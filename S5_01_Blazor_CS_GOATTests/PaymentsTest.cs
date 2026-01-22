// using Microsoft.Playwright;

// namespace S5_01_Blazor_CS_GOATTests;

// [TestClass]
// [TestCategory("Payment")]
// public class PaymentsTest : TestBase
// {


//     [TestMethod]
//     public async Task CanAddMoneyPayPal()
//     {
//         await TestHelpers.LoginAsync(Page, BaseUrl,"payeur123", "YoTuVasPayer69420%%");
        
//         var burgerMenuButton = Page.GetByText("LePayeur");
//         await Expect(burgerMenuButton).ToBeVisibleAsync();

//         // Open burger menu
//         await burgerMenuButton.ClickAsync();
        
//         await Page.WaitForTimeoutAsync(2000);
        
//         await Page.GetByText("Portefeuille :").ClickAsync();
        
//         await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
//         await Page.WaitForURLAsync($"{BaseUrl}/wallet", new PageWaitForURLOptions 
//         { 
//             Timeout = 30000 
//         });

//         await Page.Locator("button.amount-btn.paypal-btn").First.ClickAsync();
        
//         await Page.WaitForURLAsync("https://www.sandbox.paypal.com/**", new PageWaitForURLOptions 
//         {
//             Timeout = 60000
//         });

//         Console.WriteLine("opened PayPal checkoutLink");
        
        
//         await Page.GetByTestId("countrySelector").SelectOptionAsync(new SelectOptionValue
//         {
//             Value = "FR"
//         });
        
//         var contactSection = Page.GetByTestId("contactFieldsSection");
        
//         await contactSection.Locator("#email").FillAsync("jordan.lebret@etu.univ-smb.fr");
        
//         await Page.GetByTestId("phoneType").SelectOptionAsync(new SelectOptionValue
//         {
//             Value = "MOBILE"
//         }); 
        
//         await Page.GetByTestId("phone").FillAsync("0616458627");
        
                
//         await Page.ScreenshotAsync(new PageScreenshotOptions { Path = "/home/jmulins/Desktop/S5_01_CS_GOAT/S5_01_Blazor_CS_GOATTests/TestResults/screenshot1.png", FullPage = true});

//         await Page.Locator("[id='cardType']").SelectOptionAsync(new SelectOptionValue
//         {
//             Value = "VISA"
//         });

//         await Page.FillAsync("[id='cardNumber']", "4020024174579555");
        
//         await Page.FillAsync("[id='cardExpiry']", "0428");
        
//         await Page.FillAsync("[id='cardCvv']", "375");
        
//         await Page.FillAsync("[id='firstName']", "Jordan");
        
//         await Page.FillAsync("[id='lastName']", "Lebret");
        
                
//         await Page.ScreenshotAsync(new PageScreenshotOptions { Path = "/home/jmulins/Desktop/S5_01_CS_GOAT/S5_01_Blazor_CS_GOATTests/TestResults/screenshot2.png", FullPage = true});
        
//         await Page.FillAsync("[id='billingLine1']", "19 Clos des Trolles");
        
//         await Page.FillAsync("[id='billingPostalCode']", "74940");
        
//         await Page.FillAsync("[id='billingCity']", "Annecy");
        
                
//         await Page.ScreenshotAsync(new PageScreenshotOptions { Path = "/home/jmulins/Desktop/S5_01_CS_GOAT/S5_01_Blazor_CS_GOATTests/TestResults/screenshot3.png", FullPage = true});
        
        
        
        
//         Console.WriteLine("clicking on the toggle to not make an account");
        
//         // Toggle the create account button so we dont create an account
//         var checkBoxAccount = await Page.Locator("svg").AllAsync();


//         await checkBoxAccount[3].ScrollIntoViewIfNeededAsync();
//         await checkBoxAccount[3].ClickAsync();
        
//         // await checkBoxAccount.ScrollIntoViewIfNeededAsync();
//         await checkBoxAccount[3].ScreenshotAsync(new LocatorScreenshotOptions() { Path = "/home/jmulins/Desktop/S5_01_CS_GOAT/S5_01_Blazor_CS_GOATTests/TestResults/screenshot_checkbox_before.png", });
//         // await checkBoxAccount.Locator("svg").ClickAsync();
//         // await Page.GetByTestId("onboard-options-switch").CheckAsync(new LocatorCheckOptions{ Force = true});
//         // await Page.ScreenshotAsync(new PageScreenshotOptions { Path = "/home/jmulins/Desktop/S5_01_CS_GOAT/S5_01_Blazor_CS_GOATTests/TestResults/screenshot_checkbox.png", FullPage = true});

//         // var checkboxMore = Page.GetByTestId("optional-with-password-flow");
        
//         // await Page.ScreenshotAsync(new PageScreenshotOptions { Path = "/home/jmulins/Desktop/S5_01_CS_GOAT/S5_01_Blazor_CS_GOATTests/TestResults/screenshot4.png", FullPage = true});

//         Console.WriteLine("clicked on the toggle to not make an account");

//         // click to pay
//         await Page.GetByTestId("submit-button").ClickAsync();
//         Console.WriteLine("submit button clicked");
                
//         await Page.ScreenshotAsync(new PageScreenshotOptions { Path = "/home/jmulins/Desktop/S5_01_CS_GOAT/S5_01_Blazor_CS_GOATTests/TestResults/screenshot5.png", FullPage = true});
        
//         await Page.WaitForTimeoutAsync(10000);
        
//         await Page.ScreenshotAsync(new PageScreenshotOptions { Path = "/home/jmulins/Desktop/S5_01_CS_GOAT/S5_01_Blazor_CS_GOATTests/TestResults/screenshot6.png", FullPage = true});

//         // Console.WriteLine(await Page.ContentAsync());
//         // Paypal will put a prompt with a recommanded address, click on Continuer
//         await Page.GetByTestId("normalized-address-submit").ClickAsync();
//         Console.WriteLine("continued with the recommanded address");
        
//         await Page.WaitForTimeoutAsync(20000);
//         // PayPay will ask for a fictive number on your phone, just put 1234
//         await Page.FillAsync("[id='otp']", "1234");

//         await Page.Locator("#submit-button").ClickAsync();

//         Console.WriteLine("inputed the fictive number and confirmed");
        
//         await Page.WaitForURLAsync($"{BaseUrl}/payment/success?**", new PageWaitForURLOptions 
//         { 
//             Timeout = 60000 
//         });

//         Console.WriteLine("payment success");
        
//         await Page.WaitForURLAsync($"{BaseUrl}/wallet", new PageWaitForURLOptions 
//         { 
//             Timeout = 60000 
//         });

//         Console.WriteLine("returned to wallet");




//     }
// }
