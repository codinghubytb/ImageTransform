using Microsoft.AspNetCore.Components;
using librarymongodb.Models;
using System.Reflection;

namespace WebAutoApp.Client.PageModels
{
    public class HomePageModel : BasePageModel
    {
        protected List<BAL_Category> _categories { get; set; } = new List<BAL_Category>();

        protected List<BAL_Module> _modules = new List<BAL_Module>();
        protected Dictionary<string, List<BAL_Module>> _groupedShows = new Dictionary<string, List<BAL_Module>>();

        protected bool IsBusy { get; set; } = true;
        protected string TextSearch { get; set; } = string.Empty;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                IsBusy = true;

                try
                {
                    await GetCategorysAsync();
                    string idType = await GetTypesAsync();
                    await GetModulesAsync(idType);
                }
                catch (Exception ex)
                {
                    //$"Erreur lors du chargement initial : {ex.Message}");
                }
                finally
                {
                    IsBusy = false;
                    StateHasChanged();
                }
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task GetModulesAsync(string idType)
        {
            if (WebService != null && !string.IsNullOrEmpty(idType))
            {
                try
                {
                    _modules = await WebService.GetModulesByType(idType);
                    _groupedShows["Modules"] = _modules.OrderByDescending(m => m.Visit).ToList(); ;
                }
                catch (Exception ex)
                {
                    //$"Erreur lors du chargement des modules : {ex.Message}");
                }
                StateHasChanged();
            }
        }

        private async Task GetCategorysAsync()
        {
            if (WebService != null)
            {
                try
                {
                    var categories = await WebService.GetAllCategory();
                    if (categories != null)
                    {
                        if (categories.Count() > 3)
                        {
                            _categories = categories.GetRange(0, 3);
                        }
                        else
                        {
                            _categories = categories;
                        }
                    }
                    else
                    {
                        _categories = new List<BAL_Category>();
                        //$"0 catégories affichées.");

                    }

                }
                catch (Exception ex)
                {
                    //$"Erreur lors du chargement des catégories : {ex.Message}");
                }

                StateHasChanged();
            }
        }

        private async Task<string> GetTypesAsync()
        {
            if (WebService != null)
            {
                try
                {
                    var types = await WebService.GetAllType();
                    if (types != null)
                    {
                        BAL_Type? type = types.FirstOrDefault(t => t?.Name == "web");

                        return type?.Id ?? string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    //$"Erreur lors du chargement des types : {ex.Message}");
                }
            }
            return string.Empty;
        }

        protected void OnChangeCategory(string id)
        {
            IsBusy = true;
            _groupedShows.Clear();
            StateHasChanged();

            List<BAL_Module> modules = id.Equals("-1")
                ? _modules.OrderByDescending(m => m.Visit).ToList()
                : _modules.Where(c => c.Category.Contains(id)).OrderByDescending(m => m.Visit).ToList();

            _groupedShows[id] = modules;
            IsBusy = false;
            StateHasChanged();
        }

        protected void OnSearch(ChangeEventArgs e)
        {
            _groupedShows.Clear();

            if (string.IsNullOrEmpty(e.Value.ToString()))
            {
                _groupedShows.Add("Modules", _modules);
            }
            else
            {
                List<BAL_Module> modulesSearch = _modules
                    .Where(m => m.Title.ToLower().Contains(e.Value.ToString().ToLower()))
                    .OrderByDescending(m => m.Visit)
                    .ToList();
                _groupedShows.Add("Modules", modulesSearch);
            }

            StateHasChanged();
        }

        protected async Task OnNavigateCard(string path)
        {
            BAL_Module? module = _modules.FirstOrDefault(i => i.Path == path);

            if (module != null)
            {
                module.Visit++;
                try
                {
                    await WebService.UpdateModule(module);
                }
                catch (Exception ex)
                {
                    //$"Erreur lors de la mise à jour du module : {ex.Message}");
                }

                NavigationManager?.NavigateTo(path);
            }
            else
            {
                //$"Aucun module trouvé pour le chemin : {path}");
            }
        }
    }
}
