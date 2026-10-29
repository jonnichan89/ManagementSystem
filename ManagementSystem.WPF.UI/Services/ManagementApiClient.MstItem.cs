using Azure.Core;
using ManagementSystem.Common.OtherModels;
using ManagementSystem.WPF.UI.Data.OtherModels;
using ManagementSystem.WPF.UI.Models;
using ManagementSystem.WPF.UI.Services.ServiceInterfaces;
using Microsoft.EntityFrameworkCore.Metadata;
using System.IO.Compression;
using System.Text;
using System.Text.Json.Serialization;

namespace ManagementSystem.WPF.UI.Services
{
    public partial class ManagementApiClient : IManagementApiClient
    {
        #region Item

        public async Task<ServiceActionResult<MstItem>> GetMstItemsAsync(int pageIndex, PageFilterMstItem pageFilter, CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult<MstItem> actionResult;
            try
            {
                string jsonBody = JsonSerializer.Serialize(pageFilter);

                using var request = new HttpRequestMessage(HttpMethod.Get, $"MstItem/page/{pageIndex}")
                {
                    Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                };

                // Send the request and deserialize the response
                var response = await _http.SendAsync(request);
                response.EnsureSuccessStatusCode();

                actionResult = await response.Content.ReadFromJsonAsync<ServiceActionResult<MstItem>>(cancellationToken);

                //actionResult = await _http.GetFromJsonAsync<ServiceActionResult<MstItem>>(
                //    $"MstItem/{pageIndex}", _jsonOptions, cancellationToken) ?? new ServiceActionResult<MstItem>();

                //int i = 0;
                if (actionResult.ActionIsSuccess)
                {
                    await DownloadMstItemImageFile(actionResult.ActionResult, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                actionResult = new ServiceActionResult<MstItem>()
                {
                    ActionIsSuccess = false,
                    ErrorMessage =
                        string.Format(
                            TranslationSource.Instance["HttpClientFailedToReadResponse"] ?? "{0}",
                            Environment.NewLine +
                            ExceptionHelper.GetErrorMessage(ex, true))
                };
            }
            return actionResult;
        }

        private async Task DownloadMstItemImageFile(MstItem[] items, CancellationToken cancellationToken)
        {
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    item.ImagePath = await DownloadImageFile(_folderImageMstItems, item.ImagePath, item.ImageFileSize, cancellationToken);
                }
            }
        }

        private async Task DownloadMstItemImageFile(MstItem item, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                item.ImagePath = await DownloadImageFile(_folderImageMstItems, item.ImagePath, item.ImageFileSize, cancellationToken);
            }
        }

        public async Task<ServiceActionResult> GetAllMstItemImagesAsync(CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult actionResult = new ServiceActionResult();
            try
            {
                SetHttpClientLanguage();

                var resp = await _http.GetAsync("MstItem/AllImages", cancellationToken);

                if (!resp.IsSuccessStatusCode)
                {
                    actionResult.ActionIsSuccess = false;
                    actionResult.ErrorMessage = await resp.Content.ReadAsStringAsync(cancellationToken);
                    return actionResult;
                }

                using var respStream = await resp.Content.ReadAsStreamAsync(cancellationToken);
                using var archive = new ZipArchive(respStream, ZipArchiveMode.Read, leaveOpen: false);
                foreach (var entry in archive.Entries)
                {
                    // skip directory entries
                    if (string.IsNullOrEmpty(entry.Name))
                        continue;

                    var destPath = Path.Combine(_folderImageMstItems, entry.FullName);
                    var destDir = Path.GetDirectoryName(destPath);
                    if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                        Directory.CreateDirectory(destDir);

                    using var entryStream = entry.Open();
                    using var outFile = new FileStream(destPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    await entryStream.CopyToAsync(outFile, cancellationToken);
                }

                actionResult.ActionIsSuccess = true;
            }
            catch (Exception ex)
            {
                actionResult = new ServiceActionResult()
                {
                    ActionIsSuccess = false,
                    ErrorMessage =
                        string.Format(
                            TranslationSource.Instance["HttpClientFailedToReadResponse"] ?? "{0}",
                            Environment.NewLine +
                            ExceptionHelper.GetErrorMessage(ex, true))
                };
            }
            return actionResult;
        }

        public async Task<ServiceActionResult<MstItem>> GetAllMstItemsAsync(CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult<MstItem> actionResult;
            try
            {
                actionResult = await _http.GetFromJsonAsync<ServiceActionResult<MstItem>>("MstItem/All", _jsonOptions, cancellationToken) ?? new ServiceActionResult<MstItem>();
                if (actionResult.ActionIsSuccess)
                {
                    foreach (var item in actionResult.ActionResult)
                    {
                        if (!string.IsNullOrEmpty(item.ImagePath))
                        {
                            item.ImagePath = System.IO.Path.Combine(
                                _folderImageMstItems, 
                                System.IO.Path.GetFileName(item.ImagePath));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                actionResult = new ServiceActionResult<MstItem>()
                {
                    ActionIsSuccess = false,
                    ErrorMessage =
                        string.Format(
                            TranslationSource.Instance["HttpClientFailedToReadResponse"] ?? "{0}",
                            Environment.NewLine +
                            ExceptionHelper.GetErrorMessage(ex, true))
                };
            }
            return actionResult;
        }

        public async Task<ServiceActionResult> GetMstItemByIdAsync(int itemId, CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.GetFromJsonAsync<ServiceActionResult>($"MstItem/{itemId}", cancellationToken) ?? new ServiceActionResult();
                if (actionResult.ActionIsSuccess)
                {
                    MstItem item = JsonSerializer.Deserialize<MstItem>(
                        (actionResult.ActionResult ?? string.Empty).
                        ToString() ?? string.Empty) ?? null;

                    await DownloadMstItemImageFile(item, cancellationToken);
                    actionResult.ActionResult = item;
                }
            }
            catch (Exception ex)
            {
                actionResult = new ServiceActionResult()
                {
                    ActionIsSuccess = false,
                    ErrorMessage =
                        string.Format(
                            TranslationSource.Instance["HttpClientFailedToReadResponse"] ?? "{0}",
                            Environment.NewLine +
                            ExceptionHelper.GetErrorMessage(ex, true))
                };
            }
            return actionResult;
        }

        public async Task<ServiceActionResult> GetMstItemLastModifiedAsync(CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.GetFromJsonAsync<ServiceActionResult>($"MstItem/LastModified", cancellationToken) ?? new ServiceActionResult();
                if (actionResult.ActionIsSuccess)
                {
                    actionResult.ActionResult = actionResult.ActionResult.ToString();
                    //actionResult.ActionResult = JsonSerializer.Deserialize<MstItem>(
                    //    (actionResult.ActionResult ?? string.Empty).
                    //    ToString() ?? string.Empty) ?? null;
                }
            }
            catch (Exception ex)
            {
                actionResult = new ServiceActionResult()
                {
                    ActionIsSuccess = false,
                    ErrorMessage =
                        string.Format(
                            TranslationSource.Instance["HttpClientFailedToReadResponse"] ?? "{0}",
                            Environment.NewLine +
                            ExceptionHelper.GetErrorMessage(ex, true))
                };
            }
            return actionResult;
        }

        public async Task<ServiceActionResult> GetMstItemCountAsync(CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.GetFromJsonAsync<ServiceActionResult>($"MstItem/Count", cancellationToken) ?? new ServiceActionResult();
                if (actionResult.ActionIsSuccess)
                {
                    actionResult.ActionResult = int.Parse(actionResult.ActionResult.ToString());
                    //actionResult.ActionResult = JsonSerializer.Deserialize<MstItem>(
                    //    (actionResult.ActionResult ?? string.Empty).
                    //    ToString() ?? string.Empty) ?? null;
                }
            }
            catch (Exception ex)
            {
                actionResult = new ServiceActionResult()
                {
                    ActionIsSuccess = false,
                    ErrorMessage =
                        string.Format(
                            TranslationSource.Instance["HttpClientFailedToReadResponse"] ?? "{0}",
                            Environment.NewLine +
                            ExceptionHelper.GetErrorMessage(ex, true))
                };
            }
            return actionResult;
        }

        public async Task<ServiceActionResult> CreateMstItemAsync(MstItem item, CancellationToken cancellationToken = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            SetHttpClientLanguage();

            // Build multipart/form-data content for form binding on server ([FromForm] MstItem model, IFormFile image)
            using var content = new MultipartFormDataContent();

            //// Add scalar properties of MstItem as form fields. Skip ImagePath because server will populate it.
            //var props = typeof(MstItem).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //foreach (var p in props)
            //{
            //    if (p.Name == nameof(MstItem.ImagePath)) continue;

            //    if (p.GetMethod == null) continue;
            //    var val = p.GetValue(item);
            //    if (val == null) continue;

            //    string strVal;
            //    if (val is DateTime dt) strVal = dt.ToString("o", CultureInfo.InvariantCulture);
            //    else if (val is bool b) strVal = b ? "True" : "False";
            //    else if (val is IFormattable fmt) strVal = fmt.ToString(null, CultureInfo.InvariantCulture);
            //    else strVal = val.ToString() ?? string.Empty;

            //    // name must match the server parameter property name so model binder can bind.
            //    content.Add(new StringContent(strVal), p.Name);
            //}

            //if (imageStream != null)
            //{
            //    // Add file content. Use provided filename or fallback.
            //    StreamContent streamContent = new StreamContent(imageStream);

            //    switch (System.IO.Path.GetExtension(imageFileName))
            //    {
            //        case ".jpeg":
            //        case ".jpg":
            //            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            //            break;
            //        case ".png":
            //            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            //            break;

            //        default:
            //            break;
            //    }
            //    content.Add(streamContent, "image", imageFileName);
            //}

            if (!string.IsNullOrWhiteSpace(item.ImagePathToUpload))
            {
                string imageFileName = item.ItemCode + System.IO.Path.GetExtension(item.ImagePathToUpload);

                Stream imageStream = new FileStream(item.ImagePathToUpload, FileMode.Open);
                if (imageStream != null)
                {
                    // Add file content. Use provided filename or fallback.
                    StreamContent streamContent = new StreamContent(imageStream);

                    switch (System.IO.Path.GetExtension(imageFileName))
                    {
                        case ".jpeg":
                        case ".jpg":
                            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                            break;

                        case ".png":
                            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                            break;

                        default:
                            break;
                    }
                    content.Add(streamContent, "images", imageFileName);
                }
            }

            var json = JsonSerializer.Serialize(item, _jsonOptions);
            //var json = JsonSerializer.Serialize(PT);
            content.Add(new StringContent(json, Encoding.UTF8, "application/json"), "formValue");

            var respMultipart = await _http.PostAsync("MstItem", content, cancellationToken);

            if (respMultipart.IsSuccessStatusCode)
            {
                ServiceActionResult? actionResult = await respMultipart.Content.ReadFromJsonAsync<ServiceActionResult>(_jsonOptions, cancellationToken);
                if (actionResult == null)
                    throw new Exception(
                        string.Format(
                            TranslationSource.Instance["HttpClientFailedToReadResponse"] ?? "{0}",
                            Environment.NewLine +
                            await respMultipart.Content.ReadAsStringAsync()));

                return actionResult;
            }
            else
            {
                throw new Exception(await respMultipart.Content.ReadAsStringAsync());
            }
        }

        public async Task<ServiceActionResult> UpdateMstItemAsync(MstItem item, CancellationToken cancellationToken = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            SetHttpClientLanguage();

            // Build multipart/form-data content for form binding on server ([FromForm] MstItem model, IFormFile image)
            using var content = new MultipartFormDataContent();

            // Add scalar properties of MstItem as form fields. Skip ImagePath because server will populate it.
            //var props = typeof(MstItem).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //foreach (var p in props)
            //{
            //    if (p.Name == nameof(MstItem.ImagePath)) continue;

            //    if (p.GetMethod == null) continue;
            //    var val = p.GetValue(item);
            //    if (val == null) continue;

            //    string strVal;
            //    if (val is DateTime dt) strVal = dt.ToString("o", CultureInfo.InvariantCulture);
            //    else if (val is bool b) strVal = b ? "True" : "False";
            //    else if (val is IFormattable fmt) strVal = fmt.ToString(null, CultureInfo.InvariantCulture);
            //    else strVal = val.ToString() ?? string.Empty;

            //    // name must match the server parameter property name so model binder can bind.
            //    content.Add(new StringContent(strVal), p.Name);
            //}

            //if (imageStream != null)
            //{
            //    // Add file content. Use provided filename or fallback.
            //    StreamContent streamContent = new StreamContent(imageStream);

            //    switch (System.IO.Path.GetExtension(imageFileName))
            //    {
            //        case ".jpeg":
            //        case ".jpg":
            //            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            //            break;
            //        case ".png":
            //            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            //            break;

            //        default:
            //            break;
            //    }
            //    content.Add(streamContent, "image", imageFileName);
            //}

            if (!string.IsNullOrWhiteSpace(item.ImagePathToUpload))
            {
                string imageFileName = item.ItemCode + System.IO.Path.GetExtension(item.ImagePathToUpload);

                Stream imageStream = new FileStream(item.ImagePathToUpload, FileMode.Open);
                if (imageStream != null)
                {
                    // Add file content. Use provided filename or fallback.
                    StreamContent streamContent = new StreamContent(imageStream);

                    switch (System.IO.Path.GetExtension(imageFileName))
                    {
                        case ".jpeg":
                        case ".jpg":
                            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                            break;

                        case ".png":
                            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                            break;

                        default:
                            break;
                    }
                    content.Add(streamContent, "images", imageFileName);
                }
            }

            var json = JsonSerializer.Serialize(item, _jsonOptions);
            //var json = JsonSerializer.Serialize(PT);
            content.Add(new StringContent(json, Encoding.UTF8, "application/json"), "formValue");

            var respMultipart = await _http.PutAsync($"MstItem/{item.ItemId}", content, cancellationToken);

            if (respMultipart.IsSuccessStatusCode)
            {
                ServiceActionResult? actionResult = await respMultipart.Content.ReadFromJsonAsync<ServiceActionResult>(_jsonOptions, cancellationToken);
                if (actionResult == null)
                    throw new Exception(
                        string.Format(
                            TranslationSource.Instance["HttpClientFailedToReadResponse"] ?? "{0}",
                            Environment.NewLine +
                            await respMultipart.Content.ReadAsStringAsync()));

                return actionResult;
            }
            else
            {
                throw new Exception(await respMultipart.Content.ReadAsStringAsync());
            }
        }

        public async Task<ServiceActionResult> DeleteMstItemAsync(int itemId, CancellationToken cancellationToken = default)
        {
            if (itemId == 0) throw new ArgumentNullException(nameof(itemId));

            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.DeleteFromJsonAsync<ServiceActionResult>($"MstItem/{itemId}", cancellationToken) ?? new ServiceActionResult();
                if (actionResult.ActionIsSuccess)
                {
                    actionResult.ActionResult = bool.Parse(actionResult.ActionResult.ToString());
                }
            }
            catch (Exception ex)
            {
                actionResult = new ServiceActionResult()
                {
                    ActionIsSuccess = false,
                    ErrorMessage =
                        string.Format(
                            TranslationSource.Instance["HttpClientFailedToReadResponse"] ?? "{0}",
                            Environment.NewLine +
                            ExceptionHelper.GetErrorMessage(ex, true))
                };
            }
            return actionResult;
        }

        #endregion
    }
}