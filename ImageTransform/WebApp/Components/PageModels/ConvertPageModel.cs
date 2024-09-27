using LibraryServiceImageTransform.Models;
using LibraryServiceImageTransform.Services;
using Microsoft.AspNetCore.Components.Forms;

namespace WebApp.Components.PageModels
{
    public class ConvertPageModel : OperationImageModel
    {
        protected string FromExtension { get; set; }
        protected string ToExtension { get; set; }
        protected List<BAL_Extension> ToExtensions { get; set; }

        public ConvertPageModel() : base()
        {
        }

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
                else
                {
                    FromExtension = Result.format;
                    ToExtensions = GUI_APP.extensions.ToList(); 
                    ToExtensions.RemoveAll(m => m.Name == FromExtension);
                    ToExtension = ToExtensions.FirstOrDefault().Name;
                    if(Result.format.Equals("jpeg") && file.Size > 1024 * 1024 * 0.5)
                    {
                        Result = await WebService.SendImageForCompression(Result.base64Data, 10, FromExtension);
                    }
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

            var result = await WebService.SendImageForConverting(Result.base64Data, ToExtension, false, Result.format);

            Logger.Info(result.image);

            if (!string.IsNullOrEmpty(result.error))
                Error = result.error;
            else
                await OnDownloadFile(result);

            StateHasChanged();

        }

    }
}
