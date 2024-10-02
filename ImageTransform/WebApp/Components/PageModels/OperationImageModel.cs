
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using WebApp.Models;

namespace WebApp.Components.PageModels
{
    public class OperationImageModel : BasePageModel
    {
        protected IBrowserFile file;
        protected bool IsBusy { get; set; }
        protected string Error { get; set; }
        protected BAL_Result Result { get; set; }
        protected bool IsCompression { get; set; }

        public OperationImageModel()
        {
            Result = new BAL_Result(); 
            IsBusy = false;
            Error = string.Empty;
            IsCompression = false;
            file = null;
        }

        public async Task<BAL_Result> GetImageUpload(IBrowserFile file, long maxFileSize = 1024*1024*5)
        {
            BAL_Result result = new BAL_Result();
            try
            {
                if (file.Size > maxFileSize)
                {
                    throw new InvalidOperationException($"File size exceeds allowed limit. ({maxFileSize / (1024 * 1024)} mo)");
                }

                // Utilisez OpenReadStream avec RemoteJSDataStream pour lire les fichiers volumineux
                using (var stream = file.OpenReadStream(maxFileSize)) // Limite de taille
                {
                    var buffer = new byte[file.Size];
                    var bytesRead = 0;
                    var chunkSize = 4096;

                    while (bytesRead < file.Size)
                    {
                        var read = await stream.ReadAsync(buffer, bytesRead, Math.Min(chunkSize, (int)(file.Size - bytesRead)));
                        if (read == 0)
                            break;

                        bytesRead += read;
                    }

                    result.base64Data = Convert.ToBase64String(buffer, 0, bytesRead);
                    result.image = $"data:{file.ContentType};base64,{result.base64Data}";
                    result.format = file.ContentType.Split('/')[1];
                }
                return result;
            }
            catch (Exception ex)
            {
                return new BAL_Result()
                {
                    error = ex.Message
                };
            }
        }


        public async Task OnDownloadFile(BAL_Result result)
        {
            if (result != null && !string.IsNullOrEmpty(result.base64Data))
            {
                await JS.InvokeVoidAsync("downloadImage", result.image, $"{Guid.NewGuid()}.{result.format.ToLower()}");
            }
        }

        public async Task<BAL_Result> Compression(BAL_Result result)
        {
            return await ModuleService.SendImageForCompression(result.base64Data, 90, result.format);
        }

        public virtual void DeleteImage()
        {
            Result = new BAL_Result();
            IsBusy = false;
            Error = string.Empty;
            IsCompression = false;
            file = null;
            StateHasChanged();
        }

    }
}
