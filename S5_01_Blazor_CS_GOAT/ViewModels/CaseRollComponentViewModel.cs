using Microsoft.JSInterop;
using S5_01_Blazor_CS_GOAT.ViewModels;
using Shared.DTO;

public class CaseRollComponentViewModel : ViewModelBase
{
    private readonly IJSRuntime _jsRuntime;
    
    // Event that fires when animation completes
    public event EventHandler<string>? AnimationCompleted;

    public CaseRollComponentViewModel(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public List<SkinDTO> ListSkins { get; set; } = new();
    public List<SkinDTO> ListSkinsInCaseRoll { get; set; } = new();

    public async Task OpenCaseAsyncWithId(string componentId)
    {
        if (ListSkins.Count == 0)
            return;

        ListSkinsInCaseRoll.Clear();
        ListSkinsInCaseRoll = ListSkins;

        Console.WriteLine($"Opening case with ID: {componentId}");

        // Wait for animation to complete
        var success = await _jsRuntime.InvokeAsync<bool>("caseRollLogic.rollForItem", componentId);
        
        if (success)
        {
            Console.WriteLine($"Animation completed for {componentId}");
            // Raise event
            AnimationCompleted?.Invoke(this, componentId);
        }
    }
}