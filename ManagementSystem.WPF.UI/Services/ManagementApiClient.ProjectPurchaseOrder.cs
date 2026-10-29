using ManagementSystem.Common.OtherModels;
using System.Text.Json.Serialization;

namespace ManagementSystem.WPF.UI.Services
{
    public partial class ManagementApiClient
    {
        #region ProjectPurchaseOrder

        public async Task<ServiceActionResult> SearchProjectPurchaseOrdersAsync(string PSCode, CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult actionResult = null;
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, _http.BaseAddress.AbsoluteUri.TrimEnd('/') + "/ProjectPurchaseOrder/Search")
                {
                    Content = JsonContent.Create(new SearchPSOrderForOptions() { PSCode = PSCode })
                };
                
                HttpResponseMessage httpResponse = await _http.SendAsync(request, cancellationToken);
                if (httpResponse.IsSuccessStatusCode)
                {
                    actionResult = await httpResponse.Content.ReadFromJsonAsync<ServiceActionResult>();

                    if (actionResult.ActionIsSuccess)
                    {
                        actionResult.ActionResult = JsonSerializer.Deserialize<SearchOptionResultPSOrder[]>(
                            (actionResult.ActionResult ?? string.Empty).
                            ToString() ?? string.Empty, _jsonOptions) ?? null;
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

        public async Task<ServiceActionResult> GetProjectPurchaseOrdersAsync(CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.GetFromJsonAsync<ServiceActionResult>("ProjectPurchaseOrder", _jsonOptions, cancellationToken) ?? new ServiceActionResult();
                if (actionResult.ActionIsSuccess)
                {
                    actionResult.ActionResult = JsonSerializer.Deserialize<PSOrder[]>(
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

        public async Task<ServiceActionResult> GetProjectPurchaseOrderByIdAsync(int PSId, CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.GetFromJsonAsync<ServiceActionResult>($"ProjectPurchaseOrder/{PSId}", cancellationToken) ?? new ServiceActionResult();
                if (actionResult.ActionIsSuccess)
                {
                    actionResult.ActionResult = JsonSerializer.Deserialize<PSOrder>(
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

        public async Task<ServiceActionResult> CreateProjectPurchaseOrderAsync(PSOrder PSOrder, CancellationToken cancellationToken = default)
        {
            if (PSOrder is null) throw new ArgumentNullException(nameof(PSOrder));

            SetHttpClientLanguage();

            //// Remove navigation properties so the server receives only the expected data shape.
            //if (ProjectPurchaseOrder.ProjectPurchaseOrderDetails != null)
            //{
            //    foreach (var detail in ProjectPurchaseOrder.ProjectPurchaseOrderDetails)
            //    {
            //        // Clear back-reference to parent to avoid sending complex/circular graph
            //        // and to satisfy server-side validation that expects no nested ProjectPurchaseOrder object.
            //        detail.ProjectPurchaseOrder = null;
            //    }
            //}

            var resp = await _http.PostAsJsonAsync("ProjectPurchaseOrder", PSOrder, _jsonOptions, cancellationToken);
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

        public async Task<ServiceActionResult> UpdateProjectPurchaseOrderAsync(PSOrder PSOrder, CancellationToken cancellationToken = default)
        {
            if (PSOrder is null) throw new ArgumentNullException(nameof(PSOrder));

            SetHttpClientLanguage();

            var resp = await _http.PutAsJsonAsync($"ProjectPurchaseOrder/{PSOrder.PSId}", PSOrder, _jsonOptions, cancellationToken);

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

        public async Task<ServiceActionResult> DeleteProjectPurchaseOrderAsync(int PSId, CancellationToken cancellationToken = default)
        {
            if (PSId == 0) throw new ArgumentNullException(nameof(PSId));

            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.DeleteFromJsonAsync<ServiceActionResult>($"ProjectPurchaseOrder/{PSId}", cancellationToken) ?? new ServiceActionResult();
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