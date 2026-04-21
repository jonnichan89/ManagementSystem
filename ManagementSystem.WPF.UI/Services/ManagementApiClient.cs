using ManagementSystem.WPF.UI.Services.ServiceInterfaces;
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

        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions;

        public ManagementApiClient(HttpClient http, JsonSerializerOptions jsonOptions)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _jsonOptions = jsonOptions;
        }

        private void SetHttpClientLanguage()
        {
            _http.DefaultRequestHeaders.AcceptLanguage.Clear();
            _http.DefaultRequestHeaders.AcceptLanguage.Add(
                new StringWithQualityHeaderValue(CultureInfo.CurrentUICulture.ToString())
            );
        }

        public async Task<string> DownloadItemImageFile(string imageFilePath, CancellationToken cancellationToken = default)
        {
            string fileName = System.IO.Path.GetFileName(imageFilePath);

            string outputPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "items");
            System.IO.Directory.CreateDirectory(outputPath);

            string outputFullFileName = System.IO.Path.Combine(outputPath, fileName);

            if (!System.IO.File.Exists(outputFullFileName))
            {
                var resp = await _http.GetAsync(Uri.EscapeDataString(imageFilePath), cancellationToken);

                using var downloadStream = await _http.GetStreamAsync(imageFilePath.TrimStart('/'));
                using var fileStream = new FileStream(outputFullFileName, FileMode.Create, FileAccess.Write);

                await downloadStream.CopyToAsync(fileStream); // Streams data to the file
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

        public async Task<ServiceActionResult> GetMstUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.GetFromJsonAsync<ServiceActionResult>($"MstUser/{Uri.EscapeDataString(userId)}", cancellationToken) ?? new ServiceActionResult();
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

            var resp = await _http.PutAsJsonAsync($"MstUser/{Uri.EscapeDataString(user.UserId)}", user, _jsonOptions, cancellationToken);

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

        public async Task<ServiceActionResult> DeleteMstUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            if (userId is null) throw new ArgumentNullException(nameof(userId));

            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.DeleteFromJsonAsync<ServiceActionResult>($"MstUser/{Uri.EscapeDataString(userId)}", cancellationToken) ?? new ServiceActionResult();
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

        #region Item

        public async Task<ServiceActionResult<MstItem>> GetMstItemsAsync(CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult<MstItem> actionResult;
            try
            {
                actionResult = await _http.GetFromJsonAsync<ServiceActionResult<MstItem>>("MstItem", _jsonOptions, cancellationToken) ?? new ServiceActionResult<MstItem>();
                //if (actionResult.ActionIsSuccess)
                //{
                //    actionResult.ActionResult = JsonSerializer.Deserialize<MstItem[]>(
                //        (actionResult.ActionResult ?? string.Empty).
                //        ToString() ?? string.Empty) ?? null;
                //}

                //string[] files = System.IO.Directory.GetFiles(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "items1"));
                //System.IO.Directory.CreateDirectory(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "items"));

                int i = 0;
                foreach (var item in actionResult.ActionResult) // (MstItem[]?)actionResult.ActionResult ?? Array.Empty<MstItem>())
                {
                    //System.IO.File.Copy(files[i], System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, item.ImagePath.TrimStart('/')), true);
                    //i++;
                    //if (i >= files.Length)
                    //{
                    //    i = 0;
                    //}

                    if (!string.IsNullOrEmpty(item.ImagePath))
                    {
                        item.ImagePath = await DownloadItemImageFile(item.ImagePath, cancellationToken);
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
                    actionResult.ActionResult = JsonSerializer.Deserialize<MstItem>(
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

        public async Task<ServiceActionResult> CreateMstItemAsync(MstItem item, Stream? imageStream = null, string imageFileName = "", CancellationToken cancellationToken = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            SetHttpClientLanguage();

            // Build multipart/form-data content for form binding on server ([FromForm] MstItem model, IFormFile image)
            using var content = new MultipartFormDataContent();

            // Add scalar properties of MstItem as form fields. Skip ImagePath because server will populate it.
            var props = typeof(MstItem).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in props)
            {
                if (p.Name == nameof(MstItem.ImagePath)) continue;

                if (p.GetMethod == null) continue;
                var val = p.GetValue(item);
                if (val == null) continue;

                string strVal;
                if (val is DateTime dt) strVal = dt.ToString("o", CultureInfo.InvariantCulture);
                else if (val is bool b) strVal = b ? "True" : "False";
                else if (val is IFormattable fmt) strVal = fmt.ToString(null, CultureInfo.InvariantCulture);
                else strVal = val.ToString() ?? string.Empty;

                // name must match the server parameter property name so model binder can bind.
                content.Add(new StringContent(strVal), p.Name);
            }

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
                content.Add(streamContent, "image", imageFileName);
            }

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

        public async Task<ServiceActionResult> UpdateMstItemAsync(MstItem item, Stream? imageStream = null, string imageFileName = "", CancellationToken cancellationToken = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            SetHttpClientLanguage();

            // Build multipart/form-data content for form binding on server ([FromForm] MstItem model, IFormFile image)
            using var content = new MultipartFormDataContent();

            // Add scalar properties of MstItem as form fields. Skip ImagePath because server will populate it.
            var props = typeof(MstItem).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in props)
            {
                if (p.Name == nameof(MstItem.ImagePath)) continue;

                if (p.GetMethod == null) continue;
                var val = p.GetValue(item);
                if (val == null) continue;

                string strVal;
                if (val is DateTime dt) strVal = dt.ToString("o", CultureInfo.InvariantCulture);
                else if (val is bool b) strVal = b ? "True" : "False";
                else if (val is IFormattable fmt) strVal = fmt.ToString(null, CultureInfo.InvariantCulture);
                else strVal = val.ToString() ?? string.Empty;

                // name must match the server parameter property name so model binder can bind.
                content.Add(new StringContent(strVal), p.Name);
            }

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
                content.Add(streamContent, "image", imageFileName);
            }

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