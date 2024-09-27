using Microsoft.AspNetCore.Components;
using LibraryServiceImageTransform.Models;
using LibraryServiceImageTransform.Services;
using LibraryLogs; // Assuming you have a logging library

namespace WebApp.Components.PageModels
{
    public class HomePageModel : BasePageModel
    {
        protected List<BAL_Category> _categories { get; set; } = new List<BAL_Category>();

        protected Dictionary<string, List<BAL_Module>> _groupedShows = new Dictionary<string, List<BAL_Module>>();

        protected bool IsBusy { get; set; } = true;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                IsBusy = true;
                Logger?.Info("Chargement des catégories, types et modules au démarrage.");

                try
                {
                    await GetCategorysAsync();
                    string idType = await GetTypesAsync();
                    await GetModulesAsync(idType);
                    await GetExtensionsAsync();
                }
                catch (Exception ex)
                {
                    Logger?.Error($"Erreur lors du chargement initial : {ex.Message}");
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
            if (WebService != null && GUI_APP != null && !string.IsNullOrEmpty(idType))
            {
                try
                {
                    Logger?.Info($"Chargement des modules pour le type '{idType}'.");
                    List<BAL_Module> modules = await WebService.GetModulesByType(idType);
                    modules = modules.OrderByDescending(m => m.Visit).ToList();
                    GUI_APP.modules = modules;
                    _groupedShows["Modules"] = modules;
                    Logger?.Info($"Modules chargés : {modules.Count}");
                }
                catch (Exception ex)
                {
                    Logger?.Error($"Erreur lors du chargement des modules : {ex.Message}");
                }
                StateHasChanged();
            }
        }

        private async Task GetCategorysAsync()
        {
            if (WebService != null && GUI_APP != null)
            {
                try
                {
                    Logger?.Info("Chargement des catégories.");
                    GUI_APP.categories = await WebService.GetAllCategory();

                    if (GUI_APP.categories != null)
                    {
                        if(GUI_APP.categories.Count() > 3)
                        {
                            _categories = GUI_APP.categories.GetRange(0, 3);
                            Logger?.Info($"{_categories.Count} catégories affichées.");
                        }
                        else
                        {
                            _categories = GUI_APP.categories;
                        }
                    }
                    else
                    {
                        _categories = new List<BAL_Category>();
                        GUI_APP.categories = _categories;
                        Logger?.Error($"0 catégories affichées.");

                    }

                }
                catch (Exception ex)
                {
                    Logger?.Error($"Erreur lors du chargement des catégories : {ex.Message}");
                }

                StateHasChanged();
            }
        }

        private async Task<string> GetTypesAsync()
        {
            if (WebService != null && GUI_APP != null)
            {
                try
                {
                    Logger?.Info("Chargement des types.");
                    var types = await WebService.GetAllType();

                    if (types != null)
                    {
                        GUI_APP.types = types;
                        BAL_Type? type = GUI_APP.types.FirstOrDefault(t => t?.Name == "web");

                        Logger?.Info(type != null ? $"Type trouvé : {type.Name}" : "Aucun type 'web' trouvé.");
                        return type?.Id ?? string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Error($"Erreur lors du chargement des types : {ex.Message}");
                }
            }
            return string.Empty;
        }

        private async Task GetExtensionsAsync()
        {
            if (WebService != null && GUI_APP != null)
            {
                try
                {
                    Logger?.Info("Chargement des extensions.");
                    var extensions = await WebService.GetAllExtension();
                    if (extensions != null)
                        GUI_APP.extensions = extensions;
                }
                catch (Exception ex)
                {
                    Logger?.Error($"Erreur lors du chargement des extensions : {ex.Message}");
                }
            }
        }

        protected void OnChangeCategory(string id)
        {
            Logger?.Info($"Changement de catégorie vers : {id}");
            IsBusy = true;
            _groupedShows.Clear();
            StateHasChanged();

            List<BAL_Module> modules = id.Equals("-1")
                ? GUI_APP.modules.OrderByDescending(m => m.Visit).ToList()
                : GUI_APP.modules.Where(c => c.Category.Contains(id)).OrderByDescending(m => m.Visit).ToList();

            _groupedShows[id] = modules;
            Logger?.Info($"Nombre de modules trouvés pour la catégorie '{id}' : {modules.Count}");
            IsBusy = false;
            StateHasChanged();
        }

        protected void OnSearch(string search)
        {
            Logger?.Info($"Recherche initiée pour : {search}");
            if (GUI_APP != null)
            {
                _groupedShows.Clear();

                if (string.IsNullOrEmpty(search))
                {
                    _groupedShows.Add("Modules", GUI_APP.modules);
                }
                else
                {
                    List<BAL_Module> modulesSearch = GUI_APP.modules
                        .Where(m => m.Title.ToLower().Contains(search.ToLower()))
                        .OrderByDescending(m => m.Visit)
                        .ToList();
                    _groupedShows.Add("Modules", modulesSearch);
                    Logger?.Info($"{modulesSearch.Count} modules trouvés pour la recherche '{search}'.");
                }

                StateHasChanged();
            }
        }

        protected async Task OnNavigateCard(string path)
        {
            Logger?.Info($"Navigation vers le module avec chemin : {path}");
            BAL_Module? module = GUI_APP?.modules.FirstOrDefault(i => i.Path == path);

            if (module != null)
            {
                module.Visit++;
                try
                {
                    await WebService.UpdateModule(module);
                    Logger?.Info($"Visite mise à jour pour le module : {module.Title}");
                }
                catch (Exception ex)
                {
                    Logger?.Error($"Erreur lors de la mise à jour du module : {ex.Message}");
                }

                NavigationManager?.NavigateTo(path);
            }
            else
            {
                Logger?.Warning($"Aucun module trouvé pour le chemin : {path}");
            }
        }
    }
}
