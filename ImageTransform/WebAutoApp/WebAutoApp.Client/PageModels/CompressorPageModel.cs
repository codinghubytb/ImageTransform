
using Microsoft.AspNetCore.Components.Forms;

namespace WebAutoApp.Client.PageModels
{
    public class CompressorPageModel : OperationImageModel
    {
        private int _quality = 100;
        protected int Quality {
            get => _quality;
            set
            {
                if (value > 100)
                    _quality = 100;
                else if(value < 1)
                    _quality = 1;
                else
                    _quality = value;
            }
        }
        public CompressorPageModel() : base()
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
                //Erreur lors du chargement du fichier");
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
