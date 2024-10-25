using librarymongodb.Models;

using Microsoft.AspNetCore.Components.Forms;

namespace WebAutoApp.Client.PageModels
{
    public class ConvertPageModel : OperationImageModel
    {
        protected string ToExtension = "png";

        protected async Task HandleImageUpload(InputFileChangeEventArgs e)
        {
            IsBusy = true;
            Error = string.Empty;

            try
            {
                file = e.File;
                Result = await base.GetImageUpload(file, 1024*1024*1);

                if (!string.IsNullOrEmpty(Result.error))
                {
                    Error = Result.error;
                    file = null;
                }
            }
            catch (Exception ex)
            {
                //"Erreur lors du chargement du fichier");
            }
            finally
            {
                IsBusy = false;
                StateHasChanged();
            }
        }



        protected async Task OnDownloadToJpg()
        {
            Error = string.Empty;
            if (IsCompression)
                Result = await Compression(Result);

            var result = await ModuleService.SendImageForConverting(Result.base64Data, "jpg", false, Result.format);

            //result.image);

            if (!string.IsNullOrEmpty(result.error))
                Error = result.error;
            else
                await OnDownloadFile(result);

            StateHasChanged();

        }


        protected async Task OnDownloadFromJpg()
        {
            Error = string.Empty;
            if (IsCompression)
                Result = await Compression(Result);

            var result = await ModuleService.SendImageForConverting(Result.base64Data, ToExtension, false, Result.format);

            //result.image);

            if (!string.IsNullOrEmpty(result.error))
                Error = result.error;
            else
                await OnDownloadFile(result);

            StateHasChanged();

        }

    }
}
