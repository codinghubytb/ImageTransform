using LibraryLogs;
using librarymongodb.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WebApp.Services;

namespace WebApp.Components.PageModels
{
    public class BasePageModel : ComponentBase
    {
        [Inject]
        public WebService? WebService { get; set; }

        [Inject]
        public ModuleService? ModuleService { get; set; }

        [Inject]
        public GUI_APP? GUI_APP { get; set; }

        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        [Inject]
        public Logger? Logger { get; set; } // Logger injection

        [Inject]
        public IJSRuntime JS { get; set; }

        
    }
}
