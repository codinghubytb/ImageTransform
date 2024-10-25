using LibraryComponent;
using librarymongodb.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WebAutoApp.Client;
using WebAutoApp.Client.Services;

namespace WebAutoApp.Client.PageModels
{
    public class BasePageModel : ComponentBase
    {
        [Inject]
        public WebService WebService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        [Inject]
        public ModuleService ModuleService { get; set; }

        public bool IsCopy { get; set; }

        public async void OnClipboard(string text)
        {
            IsCopy = true;
            StateHasChanged();
            await JS.InvokeVoidAsync("CopyToClipboard", text);
            await Task.Delay(3000);
            IsCopy = false;
            StateHasChanged();
        }
    }
}
