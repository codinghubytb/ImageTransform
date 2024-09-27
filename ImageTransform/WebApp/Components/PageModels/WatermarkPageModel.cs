using LibraryComponent.Enums;
using LibraryServiceImageTransform.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using static System.Net.Mime.MediaTypeNames;

namespace WebApp.Components.PageModels
{
    public class WatermarkPageModel : OperationImageModel
    {
        protected Position PositionWatermark { get; set; }

        protected IBrowserFile fileWatermark { get; set; }
        protected BAL_Result ResultWatermark { get; set; }

        public WatermarkPageModel() : base()
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
                Position PositionWatermark = (Position)Enum.GetValues(typeof(Position)).GetValue(3);
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

        protected async Task HandleWatermarkUpload(InputFileChangeEventArgs e)
        {
            IsBusy = true;
            Error = string.Empty;

            try
            {
                fileWatermark = e.File;
                ResultWatermark = await base.GetImageUpload(fileWatermark);

                if (!string.IsNullOrEmpty(Result.error))
                {
                    Error = Result.error;
                    fileWatermark = null;
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

            BAL_Result result = await WebService.SendImageForWatermark(Result.base64Data, ResultWatermark.base64Data,
                PositionWatermark.ToString().ToLower(), 0.2, 1, Result.format, ResultWatermark.format);

            if (!string.IsNullOrEmpty(result.error))
                Error = result.error;
            else
                await OnDownloadFile(result);

            StateHasChanged();

        }

        public override void DeleteImage()
        {
            fileWatermark = null;
            ResultWatermark = null;
            base.DeleteImage();
            StateHasChanged();
        }

    }
}
