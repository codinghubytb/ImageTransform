using librarymongodb.Services;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebAutoApp.Client.Models;

namespace WebAutoApp.Client.Services
{
    public class ModuleService : HttpService
    {
        string Database { get; set; }
        public ModuleService(string database, HttpClient httpClient) : base(httpClient)
        {
            Database = database;
        }

        protected async Task<BAL_Result> SendImageForOperation(string path, string base64Image, 
            string extension, Dictionary<string, string> additionalParams, string base64Watermark = null, 
            string watermarkExtension = null)
        {
            try
            {
                // Convert Base64 to a byte array
                byte[] imageBytes = Convert.FromBase64String(base64Image);

                using (var content = new MultipartFormDataContent())
                {
                    // Add the image as binary file content
                    var imageContent = new ByteArrayContent(imageBytes);
                    imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue($"image/{extension}");
                    content.Add(imageContent, "image", $"image.{extension}");

                    // If watermark is provided, add it to the request
                    if (base64Watermark != null && watermarkExtension != null)
                    {
                        // Convert Base64 to a byte array
                        byte[] watermarkBytes = Convert.FromBase64String(base64Watermark);
                        var watermarkContent = new ByteArrayContent(watermarkBytes);
                        watermarkContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue($"image/{watermarkExtension}");
                        content.Add(watermarkContent, "watermark", $"watermark.{watermarkExtension}");
                    }

                    // Add additional parameters
                    foreach (var param in additionalParams)
                    {
                        content.Add(new StringContent(param.Value.ToString()), param.Key);
                    }

                    // Send the POST request to the respective operation endpoint
                    var response = await HttpClient.PostAsync(path, content);

                    // Process the response
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<BAL_Result>(jsonResponse);
                    }
                    else
                    {
                        return new BAL_Result()
                        {
                            error = "Error : Loading Image Transform"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new BAL_Result()
                {
                    error = "Error : Loading Image Transform"
                };
            }
        }


        public async Task<BAL_Result> SendImageForCropping(string base64Image, int left, int top, int width, int height, bool isCompression, string extension)
        {
            return await SendImageForOperation($"{Database}/cropper",
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
            return await SendImageForOperation($"{Database}/resize",
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
            return await SendImageForOperation($"{Database}/convert",
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
            return await SendImageForOperation($"{Database}/filter",
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
            return await SendImageForOperation($"{Database}/filter",
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
            return await SendImageForOperation($"{Database}/filter",
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
            return await SendImageForOperation($"{Database}/filter",
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
            return await SendImageForOperation($"{Database}/rotate",
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
            return await SendImageForOperation($"{Database}/compression",
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
            return await SendImageForOperation($"{Database}/watermark",
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
