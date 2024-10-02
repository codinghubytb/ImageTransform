using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace WebApp.Components.PageModels
{
    public class ResizePageModel : OperationImageModel
    {
        private int _widthMax;
        private int _heightMax;
        private int _width;
        private int _height;
        private bool _isDefault = true;
        private bool _isProportional = true; // Propriété pour gérer le mode proportionnel

        protected int Width
        {
            get => _width;
            set
            {
                if (_width != value) // Vérifiez si la valeur change
                {
                    _width = value;
                    WidthImageResult = (_width * 100) / _widthMax;

                    if (_width < 0)
                        _width = 0;
                    if (_width > _widthMax)
                    {
                        WidthImageResult = 100;
                    }

                    // Ajuster la hauteur si le mode proportionnel est activé
                    if (_isProportional)
                    {
                        Height = (_width * _heightMax) / _widthMax; // Maintenir le rapport d'aspect
                    }

                    IsDefault = (_width == _widthMax && _height == _heightMax);
                }
            }
        }

        protected int Height
        {
            get => _height;
            set
            {
                if (_height != value) // Vérifiez si la valeur change
                {
                    _height = value;
                    HeightImageResult = (_height * 100) / _heightMax;

                    if (_height < 0)
                        _height = 0;
                    if (_height > _heightMax)
                    {
                        HeightImageResult = 100;
                    }

                    // Ajuster la largeur si le mode proportionnel est activé
                    if (_isProportional)
                    {
                        Width = (_height * _widthMax) / _heightMax; // Maintenir le rapport d'aspect
                    }

                    IsDefault = (_width == _widthMax && _height == _heightMax);
                }
            }
        }

        protected bool IsDefault
        {
            get => _isDefault;
            set
            {
                if (_isDefault != value) // Vérifiez si la valeur change
                {
                    _isDefault = value;
                    if (_isDefault)
                    {
                        if (Width != _widthMax || Height != _heightMax)
                        {
                            Width = _widthMax;
                            Height = _heightMax;
                        }
                    }
                }
            }
        }

        public bool IsProportional
        {
            get => _isProportional;
            set
            {
                if (_isProportional != value) // Vérifiez si la valeur change
                {
                    _isProportional = value;

                    // Si la case est cochée, ajustez la hauteur en fonction de la largeur
                    if (_isProportional)
                    {
                        Height = (_width * _heightMax) / _widthMax; // Maintenir le rapport d'aspect
                    }
                }
            }
        }

        protected int WidthImageResult { get; set; } = 100;
        protected int HeightImageResult { get; set; } = 100;

        public ResizePageModel() : base()
        {
        }

        protected async Task HandleImageUpload(InputFileChangeEventArgs e)
        {
            IsBusy = true;
            Error = string.Empty;

            try
            {
                file = e.File;
                Result = await base.GetImageUpload(file);

                if (!string.IsNullOrEmpty(Result.error))
                {
                    Error = Result.error;
                    file = null;
                }
                else
                {
                    var dimensions = await JS.InvokeAsync<int[]>("getImageDimensions", Result.image);
                    _widthMax = dimensions[0];
                    _heightMax = dimensions[1];
                    Width = _widthMax;
                    Height = _heightMax;
                }
            }
            catch
            {
                Logger?.Error("Erreur lors du chargement du fichier");
            }
            finally
            {
                IsBusy = false;
                StateHasChanged();
            }
        }

        protected async Task OnDownload()
        {
            Error = string.Empty;
            if (IsCompression)
                Result = await Compression(Result);

            var result = await ModuleService.SendImageForResizing(Result.base64Data, Width, Height, IsCompression, Result.format);

            if (!string.IsNullOrEmpty(result.error))
                Error = result.error;
            else
                await OnDownloadFile(result);

            StateHasChanged();
        }
    }
}
