
using Microsoft.AspNetCore.Components.Forms;
using WebAutoApp.Client.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAutoApp.Client.PageModels
{
    public class FilterPageModel : OperationImageModel
    {
        private string _filter;
        protected string Filter
        {
            get => _filter;
            set
            {
                Disabled = false;
                _filter = value;
                if (_filter.Equals("nothing"))
                    Disabled = true;
                GrayScale = false;
                Invert = false;
                ValueRangeBlur = 3;

                if (_filter.Equals("grayscale"))
                    GrayScale = true;

                if (_filter.Equals("invert"))
                    Invert = true;
            }
        }

        protected List<string> FilterList { get; set; }
        protected bool Disabled { get; set; }
        protected bool Invert { get; set; } = false;
        protected bool GrayScale { get; set; } = false;
        protected double ValueRangeBlur { get; set; } = 3;
        public FilterPageModel() : base()
        {
            FilterList = new List<string>()
            {
                "Nothing",
                "GrayScale",
                "Invert",
                "Blur"
            };
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
                Filter = FilterList[0].ToLower();
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



        protected async Task OnDownload()
        {
            Error = string.Empty;
            if (IsCompression)
                Result = await Compression(Result);

            BAL_Result result = null;
            switch (Filter)
            {
                case "grayscale":
                    result = await ModuleService.SendImageForFilterGrayScale(Result.base64Data, IsCompression, Result.format);
                    break;
                case "invert":
                    result = await ModuleService.SendImageForFilterInvert(Result.base64Data, IsCompression, Result.format);
                    break;
                case "blur":
                    result = await ModuleService.SendImageForFilterBlur(Result.base64Data, ValueRangeBlur / 10, IsCompression, Result.format);
                    break;
            }

            if (result == null || !string.IsNullOrEmpty(result.error))
                Error = result == null ? "Error !" : result.error;
            else
                await OnDownloadFile(result);

            StateHasChanged();
        }

        protected int ConvertSharpBlurToCss(double ValueBlur)
        {
            if (ValueBlur < 3 || ValueBlur > 200)
            {
                return 0;
            }
            
            return (int)(ValueBlur / 10);
        }
    }
}
