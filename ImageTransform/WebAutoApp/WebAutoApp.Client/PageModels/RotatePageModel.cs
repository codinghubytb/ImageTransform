
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;


namespace WebAutoApp.Client.PageModels
{
    public class RotatePageModel : OperationImageModel
    {
        private int _angle;
        protected int Angle { 
            get => _angle; 
            set
            {
                if (value > 360)
                    _angle = value % 360;
                else if (value < -360)
                    _angle = value % 360;
                else
                    _angle = value;
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

            var result = await ModuleService.SendImageForRotate(Result.base64Data, Angle, IsCompression, Result.format);

            if(!string.IsNullOrEmpty(result.error))
                Error = result.error;
            else
                await OnDownloadFile(result);

            StateHasChanged();

        }

        
    }
}
