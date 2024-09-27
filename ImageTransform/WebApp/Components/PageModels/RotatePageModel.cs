using LibraryServiceImageTransform.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using LibraryServiceImageTransform.Models;

namespace WebApp.Components.PageModels
{
    public class RotatePageModel : OperationImageModel
    {
        private int _angle;
        protected int Angle { 
            get => _angle; 
            set
            {
                _angle = value;
                if (value > 360)
                    _angle = value - 360;
                if (value < -360)
                    _angle = value + 360;
            }
        }
        public RotatePageModel() : base()
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

            var result = await WebService.SendImageForRotate(Result.base64Data, Angle, IsCompression, Result.format);

            if(!string.IsNullOrEmpty(result.error))
                Error = result.error;
            else
                await OnDownloadFile(result);

            StateHasChanged();

        }

        
    }
}
