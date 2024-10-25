using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace WebAutoApp.Client.PageModels
{
    public class CropperPageModel : OperationImageModel
    {
        private int _width;
        private int _height;
        private int _top;
        private int _left;

        protected int Width
        {
            get => _width;
            set
            {
                if (value < 0)
                    _width = 0;
                else if (value > 100)
                    _width = 100;
                else
                    _width = value;

                if(_left > 100 - Width)
                    _left = 100 - Width;

            }
        }
        protected int Height
        {
            get => _height;
            set
            {
                if (value < 0)
                    _height = 0;
                else if (value > 100)
                    _height = 100;
                else
                    _height = value;

                if (_top > 100 - Height)
                    _top = 100 - Height;
            }
        }
        protected int Left
        {
            get => _left;
            set
            {
                if (value < 0)
                    _left = 0;
                else if (value > 100 - Width)
                    _left = 100 - Width;
                else
                    _left = value;
            }
        }
        protected int Top
        {
            get => _top;
            set
            {
                if (value < 0)
                    _top = 0;
                else if (value > 100 - Height)
                    _top = 100 - Height;
                else
                    _top = value;
            }
        }
        protected int WidthDefault { get; set; }
        protected int HeightDefault { get; set; }


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
                    Width = 20;
                    Height = 20;
                    Left = 0;
                    Top = 0;
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
            Error = string.Empty;
            if (IsCompression)
                Result = await Compression(Result);

            if(Width == 0 || Height == 0)
            {
                Error = "Width or height cannot be zero";
                StateHasChanged();
                return;
            }
            var result = await ModuleService.SendImageForCropping(Result.base64Data, 
                WidthDefault * Left / 100, 
                HeightDefault * Top / 100,
                WidthDefault * Width / 100,
                HeightDefault * Height / 100,
                IsCompression, Result.format);

            if (!string.IsNullOrEmpty(result.error))
                Error = result.error;
            else
                await OnDownloadFile(result);

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

        protected void OnChangeLeft(int value)
        {
            Width = value;
            StateHasChanged();
        }
        protected void OnChangeTop(int value)
        {
            Height = value;
            StateHasChanged();
        }
    }
}
