
using Microsoft.AspNetCore.Components.Forms;

namespace WebApp.Components.PageModels
{
    public class CompressionPageModel : OperationImageModel
    {
        private int _quality = 100;
        protected int Quality {
            get => _quality;
            set
            {
                _quality = value;
                if (_quality > 100)
                    _quality = 100;
                if(_quality < 0)
                    _quality = 0;
            }
        }
        public CompressionPageModel() : base()
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
            catch (Exception ex)
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

            var result = await ModuleService.SendImageForCompression(Result.base64Data, Quality, Result.format);

            if (!string.IsNullOrEmpty(result.error))
                Error = result.error;
            else
                await OnDownloadFile(result);

            StateHasChanged();

        }

    }
}
