using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Net.Http;
using LibraryServiceImageTransform.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LibraryServiceImageTransform.Services
{
    public class HttpService
    {
        #region Fields

        /// <summary>
        /// HttpClient instance for making HTTP requests.
        /// </summary>
        protected HttpClient HttpClient { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the HttpService class with the specified HttpClient and API URL.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance to be used for making HTTP requests.</param>
        /// <param name="apiUrl">The base URL for the API.</param>
        public HttpService(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        #region Methods

        /// <summary>
        /// Sends a GET request to the specified path and returns a list of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the returned list.</typeparam>
        /// <param name="path">The path to append to the API base URL for the request.</param>
        /// <returns>A list of objects of type T if the request is successful; otherwise, the default value for a List of type T.</returns>
        protected async Task<List<T>> HttpResponse_GET<T>(string path)
        {
            try
            {
                HttpResponseMessage response = await HttpClient.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<T>>(content);
                }

                return default(List<T>);
            }
            catch (Exception ex)
            {
                return default(List<T>);
            }
        }
        protected async Task<T?> HttpResponse_GETOne<T>(string path)
        {
            try
            {
                HttpResponseMessage response = await HttpClient.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(content);
                }

                return default(T);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
        protected async Task<HttpResponseMessage> HttpResponse_GET(string path)
        {
            try
            {
                HttpResponseMessage response = await HttpClient.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }

                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        protected async Task<BAL_Result> SendImageForOperation( string path, string base64Image, string extension, Dictionary<string, string> additionalParams, string base64Watermark = null, string watermarkExtension = null)
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




        /// <summary>
        /// Sends a PUT request with the specified data to the specified path and returns the HTTP response message.
        /// </summary>
        /// <typeparam name="T">The type of the data to be sent in the request body.</typeparam>
        /// <param name="path">The path to append to the API base URL for the request.</param>
        /// <param name="data">The data to be sent in the request body.</param>
        /// <returns>The HTTP response message indicating the result of the request.</returns>
        protected async Task<HttpResponseMessage> HttpResponse_PUT<T>(string path, T data)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(data);
                var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await HttpClient.PutAsync(path, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Sends a POST request with the specified data to the specified path and returns the HTTP response message.
        /// </summary>
        /// <typeparam name="T">The type of the data to be sent in the request body.</typeparam>
        /// <param name="path">The path to append to the API base URL for the request.</param>
        /// <param name="data">The data to be sent in the request body.</param>
        /// <returns>The HTTP response message indicating the result of the request.</returns>
        protected async Task<HttpResponseMessage> HttpResponse_POST<T>(string path, T data)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(data);
                var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await HttpClient.PostAsync(path, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Sends a DELETE request to the specified path and returns the HTTP response message.
        /// </summary>
        /// <param name="path">The path to append to the API base URL for the request.</param>
        /// <returns>The HTTP response message indicating the result of the request.</returns>
        protected async Task<HttpResponseMessage> HttpResponse_DELETE(string path)
        {
            try
            {
                HttpResponseMessage response = await HttpClient.DeleteAsync(path);
                return response;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        #endregion

    }
}
