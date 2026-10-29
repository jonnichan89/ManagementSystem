using ManagementSystem.Common.OtherModels;
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
        #region ItemAdjustment

        public async Task<ServiceActionResult> GetItemAdjustmentsAsync(CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.GetFromJsonAsync<ServiceActionResult>("ItemAdjustment", _jsonOptions, cancellationToken) ?? new ServiceActionResult();
                if (actionResult.ActionIsSuccess)
                {
                    actionResult.ActionResult = JsonSerializer.Deserialize<ItemAdjustment[]>(
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

        public async Task<ServiceActionResult> GetItemAdjustmentByIdAsync(int IAId, CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.GetFromJsonAsync<ServiceActionResult>($"ItemAdjustment/{IAId}", cancellationToken) ?? new ServiceActionResult();
                if (actionResult.ActionIsSuccess)
                {
                    ItemAdjustment item = JsonSerializer.Deserialize<ItemAdjustment>(
                        (actionResult.ActionResult ?? string.Empty).
                        ToString() ?? string.Empty) ?? null;

                    foreach (ItemAdjustmentDetail itemDetail in item.ItemAdjustmentDetails)
                    {
                        itemDetail.ItemAdjustment = item;
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

        public async Task<ServiceActionResult> CreateItemAdjustmentAsync(ItemAdjustment itemAdjustment, CancellationToken cancellationToken = default)
        {
            if (itemAdjustment is null) throw new ArgumentNullException(nameof(itemAdjustment));

            SetHttpClientLanguage();

            var resp = await _http.PostAsJsonAsync("ItemAdjustment", itemAdjustment, _jsonOptions, cancellationToken);
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

        public async Task<ServiceActionResult> UpdateItemAdjustmentAsync(ItemAdjustment itemAdjustment, CancellationToken cancellationToken = default)
        {
            if (itemAdjustment is null) throw new ArgumentNullException(nameof(itemAdjustment));

            SetHttpClientLanguage();

            var resp = await _http.PutAsJsonAsync($"ItemAdjustment/{itemAdjustment.IAId}", itemAdjustment, _jsonOptions, cancellationToken);

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

        public async Task<ServiceActionResult> DeleteItemAdjustmentAsync(int IAId, CancellationToken cancellationToken = default)
        {
            if (IAId == 0) throw new ArgumentNullException(nameof(IAId));

            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.DeleteFromJsonAsync<ServiceActionResult>($"ItemAdjustment/{IAId}", cancellationToken) ?? new ServiceActionResult();
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