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
        private class ActionResultValue<T>
        {
            [JsonPropertyName("$values")]
            List<T> Values { get; set; }
        }

        private string _folderImageMstItems;
        private string _folderImageProjectTemplates;

        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly SecureStorageService _storage;

        public ManagementApiClient(HttpClient http, JsonSerializerOptions jsonOptions, SecureStorageService storage)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _jsonOptions = jsonOptions;
            _storage = storage;

            _folderImageMstItems = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MstItems");
            System.IO.Directory.CreateDirectory(_folderImageMstItems);

            _folderImageProjectTemplates = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProjectTemplates");
            System.IO.Directory.CreateDirectory(_folderImageProjectTemplates);
        }

        private void SetHttpClientLanguage()
        {
            _http.DefaultRequestHeaders.AcceptLanguage.Clear();
            _http.DefaultRequestHeaders.AcceptLanguage.Add(
                new StringWithQualityHeaderValue(CultureInfo.CurrentUICulture.ToString())
            );
        }

        private void SetHttpClientAuthorizationHeader()
        {
            string runtimeToken = _storage.ReadToken();
            if (string.IsNullOrEmpty(runtimeToken)) throw new UnauthorizedAccessException();

            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", runtimeToken);
            runtimeToken = null;
        }

        public async Task<string> DownloadImageFile(string localImageFolderPath, string urlImageFilePath, int imageFileSize, CancellationToken cancellationToken = default)
        {
            string fileName = System.IO.Path.GetFileName(urlImageFilePath);

            string outputFullFileName = System.IO.Path.Combine(localImageFolderPath, fileName);
            bool downloadImage = false;
            if (System.IO.File.Exists(outputFullFileName))
            {
                if (imageFileSize == 0)
                    downloadImage = false;
                else
                {
                    FileInfo fi = new FileInfo(outputFullFileName);
                    if ((int)fi.Length != imageFileSize)
                    {
                        downloadImage = true;
                    }
                }
            }
            else
            {
                downloadImage = true;
            }

            if (downloadImage)
            {
                try
                {
                    using var downloadStream = await _http.GetStreamAsync(urlImageFilePath.TrimStart('/'));
                    using var fileStream = new FileStream(outputFullFileName, FileMode.Create, FileAccess.Write);

                    await downloadStream.CopyToAsync(fileStream); // Streams data to the file
                }
                catch (Exception)
                {
                }
            }

            return outputFullFileName;
        }

        #region MstUser

        public async Task<ServiceActionResult> GetMstUsersAsync(CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.GetFromJsonAsync<ServiceActionResult>("MstUser", _jsonOptions, cancellationToken) ?? new ServiceActionResult();
                if (actionResult.ActionIsSuccess)
                {
                    actionResult.ActionResult = JsonSerializer.Deserialize<MstUser[]>(
                        (actionResult.ActionResult ?? string.Empty).
                        ToString() ?? string.Empty) ?? null;
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

        public async Task<ServiceActionResult> GetMstUserByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.GetFromJsonAsync<ServiceActionResult>($"MstUser/{Uri.EscapeDataString(userName)}", cancellationToken) ?? new ServiceActionResult();
                if (actionResult.ActionIsSuccess)
                {
                    actionResult.ActionResult = JsonSerializer.Deserialize<MstUser>(
                        (actionResult.ActionResult ?? string.Empty).
                        ToString() ?? string.Empty) ?? null;
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

        public async Task<ServiceActionResult> CreateMstUserAsync(MstUser user, CancellationToken cancellationToken = default)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));

            SetHttpClientLanguage();

            var resp = await _http.PostAsJsonAsync("MstUser", user, _jsonOptions, cancellationToken);

            if (resp.IsSuccessStatusCode)
            {
                ServiceActionResult? actionResult = await resp.Content.ReadFromJsonAsync<ServiceActionResult>(_jsonOptions, cancellationToken);
                if (actionResult == null)
                    throw new Exception(
                        string.Format(
                            TranslationSource.Instance["HttpClientFailedToReadResponse"] ?? "{0}",
                            Environment.NewLine +
                            await resp.Content.ReadAsStringAsync()));

                return actionResult;
            }
            else
            {
                throw new Exception(await resp.Content.ReadAsStringAsync());
            }
        }

        public async Task<ServiceActionResult> UpdateMstUserAsync(MstUser user, CancellationToken cancellationToken = default)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));

            SetHttpClientLanguage();

            var resp = await _http.PutAsJsonAsync($"MstUser/{Uri.EscapeDataString(user.UserName)}", user, _jsonOptions, cancellationToken);

            if (resp.IsSuccessStatusCode)
            {
                ServiceActionResult? actionResult = await resp.Content.ReadFromJsonAsync<ServiceActionResult>(_jsonOptions, cancellationToken);
                if (actionResult == null)
                    throw new Exception(
                        string.Format(
                            TranslationSource.Instance["HttpClientFailedToReadResponse"] ?? "{0}",
                            Environment.NewLine +
                            await resp.Content.ReadAsStringAsync()));

                return actionResult;
            }
            else
            {
                throw new Exception(await resp.Content.ReadAsStringAsync());
            }
        }

        public async Task<ServiceActionResult> DeleteMstUserAsync(string userName, CancellationToken cancellationToken = default)
        {
            if (userName is null) throw new ArgumentNullException(nameof(userName));

            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.DeleteFromJsonAsync<ServiceActionResult>($"MstUser/{Uri.EscapeDataString(userName)}", cancellationToken) ?? new ServiceActionResult();
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

        private class LoginDataResult
        {
            public string Token { get; set; }
            public string UserName { get; set; }
            public string AccessList { get; set; }
        }
        public async Task<ServiceActionResult> LoginUserAsync(string userName, string password, CancellationToken cancellationToken = default)
        {
            if (userName is null) throw new ArgumentNullException(nameof(userName));
            if (password is null) throw new ArgumentNullException(nameof(password));

            SetHttpClientLanguage();

            ServiceActionResult actionResult = null;
            try
            {
                HttpResponseMessage response = await _http.PostAsJsonAsync($"MstUser/Login", new { UserName = userName, UserPassword = password }, cancellationToken);

                // 2. Safely extract and deserialize the ServiceActionResult wrapper
                if (response.IsSuccessStatusCode)
                {
                    actionResult = await response.Content.ReadFromJsonAsync<ServiceActionResult>();

                    if (actionResult.ActionIsSuccess)
                    {
                        LoginDataResult data = JsonSerializer.Deserialize<LoginDataResult>(
                            (actionResult.ActionResult ?? string.Empty).
                            ToString() ?? string.Empty) ?? null;

                        App.UserName = data.UserName;
                        App.AccessList = data.AccessList;
                        App.AccessToken = data.Token;
                    }
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

        #region Supplier

        public async Task<ServiceActionResult> GetMstSuppliersAsync(CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.GetFromJsonAsync<ServiceActionResult>("MstSupplier", _jsonOptions, cancellationToken) ?? new ServiceActionResult();
                if (actionResult.ActionIsSuccess)
                {
                    actionResult.ActionResult = JsonSerializer.Deserialize<MstSupplier[]>(
                        (actionResult.ActionResult ?? string.Empty).
                        ToString() ?? string.Empty) ?? null;
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

        public async Task<ServiceActionResult> GetMstSupplierByIdAsync(int supplierId, CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.GetFromJsonAsync<ServiceActionResult>($"MstSupplier/{supplierId}", cancellationToken) ?? new ServiceActionResult();
                if (actionResult.ActionIsSuccess)
                {
                    actionResult.ActionResult = JsonSerializer.Deserialize<MstSupplier>(
                        (actionResult.ActionResult ?? string.Empty).
                        ToString() ?? string.Empty) ?? null;
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

        public async Task<ServiceActionResult> GetMstSupplierLastModifiedAsync(CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.GetFromJsonAsync<ServiceActionResult>($"MstSupplier/LastModified", cancellationToken) ?? new ServiceActionResult();
                if (actionResult.ActionIsSuccess)
                {
                    actionResult.ActionResult = actionResult.ActionResult.ToString();
                    //actionResult.ActionResult = JsonSerializer.Deserialize<MstSupplier>(
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

        public async Task<ServiceActionResult> CreateMstSupplierAsync(MstSupplier supplier, CancellationToken cancellationToken = default)
        {
            if (supplier is null) throw new ArgumentNullException(nameof(supplier));

            SetHttpClientLanguage();

            var resp = await _http.PostAsJsonAsync("MstSupplier", supplier, _jsonOptions, cancellationToken);

            if (resp.IsSuccessStatusCode)
            {
                ServiceActionResult? actionResult = await resp.Content.ReadFromJsonAsync<ServiceActionResult>(_jsonOptions, cancellationToken);
                if (actionResult == null)
                    throw new Exception(
                        string.Format(
                            TranslationSource.Instance["HttpClientFailedToReadResponse"] ?? "{0}",
                            Environment.NewLine +
                            await resp.Content.ReadAsStringAsync()));

                return actionResult;
            }
            else
            {
                throw new Exception(await resp.Content.ReadAsStringAsync());
            }
        }

        public async Task<ServiceActionResult> UpdateMstSupplierAsync(MstSupplier supplier, CancellationToken cancellationToken = default)
        {
            if (supplier is null) throw new ArgumentNullException(nameof(supplier));

            SetHttpClientLanguage();

            var resp = await _http.PutAsJsonAsync($"MstSupplier/{supplier.SupplierId}", supplier, _jsonOptions, cancellationToken);

            if (resp.IsSuccessStatusCode)
            {
                ServiceActionResult? actionResult = await resp.Content.ReadFromJsonAsync<ServiceActionResult>(_jsonOptions, cancellationToken);
                if (actionResult == null)
                    throw new Exception(
                        string.Format(
                            TranslationSource.Instance["HttpClientFailedToReadResponse"] ?? "{0}",
                            Environment.NewLine +
                            await resp.Content.ReadAsStringAsync()));

                return actionResult;
            }
            else
            {
                throw new Exception(await resp.Content.ReadAsStringAsync());
            }
        }

        public async Task<ServiceActionResult> DeleteMstSupplierAsync(int supplierId, CancellationToken cancellationToken = default)
        {
            if (supplierId == 0) throw new ArgumentNullException(nameof(supplierId));

            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.DeleteFromJsonAsync<ServiceActionResult>($"MstSupplier/{supplierId}", cancellationToken) ?? new ServiceActionResult();
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

        #region ItemUsage

        public async Task<ServiceActionResult> GetItemUsagesAsync(CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.GetFromJsonAsync<ServiceActionResult>("ItemUsage", _jsonOptions, cancellationToken) ?? new ServiceActionResult();
                if (actionResult.ActionIsSuccess)
                {
                    actionResult.ActionResult = JsonSerializer.Deserialize<ItemUsage[]>(
                        (actionResult.ActionResult ?? string.Empty).
                        ToString() ?? string.Empty, _jsonOptions) ?? null;
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

        public async Task<ServiceActionResult> GetItemUsageByIdAsync(int itemUsageId, CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.GetFromJsonAsync<ServiceActionResult>($"ItemUsage/{itemUsageId}", cancellationToken) ?? new ServiceActionResult();
                if (actionResult.ActionIsSuccess)
                {
                    ItemUsage item = JsonSerializer.Deserialize<ItemUsage>(
                        (actionResult.ActionResult ?? string.Empty).
                        ToString() ?? string.Empty) ?? null;

                    foreach (ItemUsageDetail itemDetail in item.ItemUsageDetails)
                    {
                        itemDetail.ItemUsage = item;
                    }

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

        public async Task<ServiceActionResult> CreateItemUsageAsync(ItemUsage itemUsage, CancellationToken cancellationToken = default)
        {
            if (itemUsage is null) throw new ArgumentNullException(nameof(itemUsage));

            SetHttpClientLanguage();

            //// Remove navigation properties so the server receives only the expected data shape.
            //if (itemUsage.ItemUsageDetails != null)
            //{
            //    foreach (var detail in itemUsage.ItemUsageDetails)
            //    {
            //        // Clear back-reference to parent to avoid sending complex/circular graph
            //        // and to satisfy server-side validation that expects no nested ItemUsage object.
            //        detail.ItemUsage = null;
            //    }
            //}

            var resp = await _http.PostAsJsonAsync("ItemUsage", itemUsage, _jsonOptions, cancellationToken);
            string a = await resp.Content.ReadAsStringAsync();
            if (resp.IsSuccessStatusCode)
            {
                ServiceActionResult? actionResult = await resp.Content.ReadFromJsonAsync<ServiceActionResult>(_jsonOptions, cancellationToken);
                if (actionResult == null)
                    throw new Exception(
                        string.Format(
                            TranslationSource.Instance["HttpClientFailedToReadResponse"] ?? "{0}",
                            Environment.NewLine +
                            await resp.Content.ReadAsStringAsync()));

                return actionResult;
            }
            else
            {
                throw new Exception(await resp.Content.ReadAsStringAsync());
            }
        }

        public async Task<ServiceActionResult> UpdateItemUsageAsync(ItemUsage itemUsage, CancellationToken cancellationToken = default)
        {
            if (itemUsage is null) throw new ArgumentNullException(nameof(itemUsage));

            SetHttpClientLanguage();

            var resp = await _http.PutAsJsonAsync($"ItemUsage/{itemUsage.ItemUsageId}", itemUsage, _jsonOptions, cancellationToken);

            if (resp.IsSuccessStatusCode)
            {
                ServiceActionResult? actionResult = await resp.Content.ReadFromJsonAsync<ServiceActionResult>(_jsonOptions, cancellationToken);
                if (actionResult == null)
                    throw new Exception(
                        string.Format(
                            TranslationSource.Instance["HttpClientFailedToReadResponse"] ?? "{0}",
                            Environment.NewLine +
                            await resp.Content.ReadAsStringAsync()));

                return actionResult;
            }
            else
            {
                throw new Exception(await resp.Content.ReadAsStringAsync());
            }
        }

        public async Task<ServiceActionResult> DeleteItemUsageAsync(int itemUsageId, CancellationToken cancellationToken = default)
        {
            if (itemUsageId == 0) throw new ArgumentNullException(nameof(itemUsageId));

            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.DeleteFromJsonAsync<ServiceActionResult>($"ItemUsage/{itemUsageId}", cancellationToken) ?? new ServiceActionResult();
                if (actionResult.ActionIsSuccess)
                {
                    actionResult.ActionResult = actionResult.ActionResult.ToString();
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