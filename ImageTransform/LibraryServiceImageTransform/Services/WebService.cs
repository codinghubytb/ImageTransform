using LibraryServiceImageTransform.Models;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibraryServiceImageTransform.Services
{
    public class WebService : HttpService
    {
        /// <summary>
        /// Initializes a new instance of the WebService class with a default HttpClient and API URL.
        /// </summary>
        public WebService(HttpClient httpClient) : base(httpClient)
        {
        }

        /// <summary>
        /// Retrieves a list of all modules from the API.
        /// </summary>
        /// <returns>A list of BAL_Module objects if the request is successful; otherwise, null.</returns>
        public async Task<List<BAL_Module>> GetModulesByType(string idType)
        {
            try
            {
                var modules = await HttpResponse_GET<BAL_Module>($"api/modules-by-type?type={idType}");
                return modules;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves a list of all categories from the API.
        /// </summary>
        /// <returns>A list of BAL_Category objects if the request is successful; otherwise, null.</returns>
        public async Task<List<BAL_Category>> GetAllCategory()
        {
            try
            {
                return await HttpResponse_GET<BAL_Category>("api/categorys");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves a list of all extensions from the API.
        /// </summary>
        /// <returns>A list of BAL_Extension objects if the request is successful; otherwise, null.</returns>
        public async Task<List<BAL_Extension>> GetAllExtension()
        {
            try
            {
                return await HttpResponse_GET<BAL_Extension>("api/extensions");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves a list of all types from the API.
        /// </summary>
        /// <returns>A list of BAL_Type objects if the request is successful; otherwise, null.</returns>
        public async Task<List<BAL_Type>> GetAllType()
        {
            try
            {
                return await HttpResponse_GET<BAL_Type>("api/types");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<HttpResponseMessage> UpdateModule(BAL_Module module)
        {
            try
            {
                return await HttpResponse_PUT<BAL_Module>("api/module-update", module);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }

        public async Task<BAL_Result> SendImageForCropping(string base64Image, int left, int top, int width, int height, bool isCompression, string extension)
        {
            return await SendImageForOperation("imagetransform/cropper",
                            base64Image,
                            extension,
                            new Dictionary<string, string>
                            {
                                { "left", left.ToString() },
                                { "top", top.ToString() },
                                { "width", width.ToString() },
                                { "height", height.ToString() },
                                { "iscompression", isCompression.ToString() }
                            });
        }

        public async Task<BAL_Result> SendImageForResizing(string base64Image, int width, int height, bool isCompression, string extension)
        {
            return await SendImageForOperation("imagetransform/resize",
                            base64Image,
                            extension,
                            new Dictionary<string, string>
                            {
                                { "width", width.ToString() },
                                { "height", height.ToString() },
                                { "iscompression", isCompression.ToString() }
                            });
        }

        public async Task<BAL_Result> SendImageForConverting(string base64Image, string format, bool isCompression, string extension)
        {
            return await SendImageForOperation("imagetransform/convert",
                            base64Image,
                            extension,
                            new Dictionary<string, string>
                            {
                                { "format", format.ToString() },
                                { "iscompression", isCompression.ToString() }
                            });
        }

        public async Task<BAL_Result> SendImageForFilterBlur(string base64Image, double blurIntensity, bool isCompression, string extension)
        {
            return await SendImageForOperation("imagetransform/filter",
                            base64Image,
                            extension,
                            new Dictionary<string, string>
                            {
                                { "filterType", "blur" },
                                { "iscompression", isCompression.ToString() },
                                { "blurIntensity", blurIntensity.ToString().Replace(",", ".") }
                            });
        }
        public async Task<BAL_Result> SendImageForFilterInvert(string base64Image, bool isCompression, string extension)
        {
            return await SendImageForOperation("imagetransform/filter",
                            base64Image,
                            extension,
                            new Dictionary<string, string>
                            {
                                { "filterType", "invert" },
                                { "iscompression", isCompression.ToString() }
                            });
        }
        public async Task<BAL_Result> SendImageForFilterBrightness(string base64Image, double brightnessLevel, bool isCompression, string extension)
        {
            return await SendImageForOperation("imagetransform/filter",
                            base64Image,
                            extension,
                            new Dictionary<string, string>
                            {
                                { "filterType", "brightness" },
                                { "iscompression", isCompression.ToString() },
                                { "brightnessLevel", brightnessLevel.ToString().Replace(",", ".") }
                            });
        }
        public async Task<BAL_Result> SendImageForFilterGrayScale(string base64Image, bool isCompression, string extension)
        {
            return await SendImageForOperation("imagetransform/filter",
                            base64Image,
                            extension,
                            new Dictionary<string, string>
                            {
                                { "filterType", "grayscale" },
                                { "iscompression", isCompression.ToString() }
                            });
        }

        public async Task<BAL_Result> SendImageForRotate(string base64Image, int angle, bool isCompression, string extension)
        {
            return await SendImageForOperation("imagetransform/rotate",
                            base64Image,
                            extension,
                            new Dictionary<string, string>
                            {
                                { "angle", angle.ToString() },
                                { "iscompression", isCompression.ToString() }
                            });
        }

        public async Task<BAL_Result> SendImageForCompression(string base64Image, int quality, string extension)
        {
            return await SendImageForOperation("imagetransform/compression",
                            base64Image,
                            extension,
                            new Dictionary<string, string>
                            {
                                { "quality", quality.ToString() }
                            });
        }

        public async Task<BAL_Result> SendImageForWatermark(string base64Image, string base64watermark, string position,
            double widthWatermark, double opacity,  string extension, string extensionwatermark)
        {
            return await SendImageForOperation("imagetransform/watermark",
                            base64Image,
                            extension,
                            new Dictionary<string, string>
                            {
                                { "position", position.ToString() },
                                { "opacity", opacity.ToString().Replace(",", ".") },
                                { "widthWatermark", widthWatermark.ToString().Replace(",", ".") }
                            },
                            base64watermark,
                            extensionwatermark);
        }
    }

}
