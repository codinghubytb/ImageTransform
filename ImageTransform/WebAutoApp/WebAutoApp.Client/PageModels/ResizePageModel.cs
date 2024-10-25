using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace WebAutoApp.Client.PageModels
{
    public class ResizePageModel : OperationImageModel
    {
        private int _width;
        private int _height;
        private bool _isDefault = true;
        private bool _isProportional = true; // Propriété pour gérer le mode proportionnel

        protected int WidthDefault { get; set; }

        protected int HeightDefault { get; set; }
        protected int Width
        {
            get => _width;
            set
            {
                if (value < 0)
                {
                    _width = WidthDefault;
                    _height = HeightDefault;
                }
                else
                {
                    _width = value;

                    if (_isProportional)
                    {
                        _height = (_width * HeightDefault) / WidthDefault; // Maintenir le rapport d'aspect
                    }

                    IsDefault = (_width == WidthDefault && _height == HeightDefault);
                }


            }
        }

        protected int Height
        {
            get => _height;
            set
            {
                if (value < 0)
                {
                    _width = WidthDefault;
                    _height = HeightDefault;
                }
                else
                {
                    _height = value;

                    if (_isProportional)
                    {
                        _width = (_height * WidthDefault) / HeightDefault; // Maintenir le rapport d'aspect
                    }

                    IsDefault = (_width == WidthDefault && _height == HeightDefault);
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
                        if (Width != WidthDefault || Height != HeightDefault)
                        {
                            Width = WidthDefault;
                            Height = HeightDefault;
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
                        Height = (Width * HeightDefault) / WidthDefault; // Maintenir le rapport d'aspect
                    }
                }
            }
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
                    WidthDefault = dimensions[0];
                    HeightDefault = dimensions[1];
                    Width = WidthDefault;
                    Height = HeightDefault;
                }
            }
            catch
            {
                //"Erreur lors du chargement du fichier");
            }
            finally
            {
                IsBusy = false;
                StateHasChanged();
            }
        }

        protected async Task OnDownload()
        {
            if (Width < 4001 || Height < 4001)
            {
                Error = string.Empty;
                if (IsCompression)
                    Result = await Compression(Result);

                var result = await ModuleService.SendImageForResizing(Result.base64Data, Width, Height, IsCompression, Result.format);

                if (!string.IsNullOrEmpty(result.error))
                    Error = result.error;
                else
                    await OnDownloadFile(result);

            }
            else
            {
                Error = "Value too high";
            }
            StateHasChanged();
        }

        protected void OnChangeWidth(int value)
        {
            Width = value;
            StateHasChanged();
        }
        protected void OnChangeHeight(int value)
        {
            Height = value;
            StateHasChanged();
        }
    }
}
