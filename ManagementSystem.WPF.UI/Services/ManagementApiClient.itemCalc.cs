using ManagementSystem.Common.OtherModels;
using System.IO.Compression;
using System.Text;
using System.Text.Json.Serialization;

namespace ManagementSystem.WPF.UI.Services
{
    public partial class ManagementApiClient
    {
        #region ItemCalc

        public async Task<ServiceActionResult<ItemCalc>> GetItemCalcsAsync(int pageIndex, PageFilterItemCalc pageFilter, CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult<ItemCalc> actionResult;
            try
            {
                string jsonBody = JsonSerializer.Serialize(pageFilter);

                using var request = new HttpRequestMessage(HttpMethod.Get, $"ItemCalc/page/{pageIndex}")
                {
                    Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                };

                // Send the request and deserialize the response
                var response = await _http.SendAsync(request);
                response.EnsureSuccessStatusCode();

                actionResult = await response.Content.ReadFromJsonAsync<ServiceActionResult<ItemCalc>>(cancellationToken);

            }
            catch (Exception ex)
            {
                actionResult = new ServiceActionResult<ItemCalc>()
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

        public async Task<ServiceActionResult<ItemCalc>> GetAllItemCalcsAsync(string itemCode, CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult<ItemCalc> actionResult;
            try
            {
                actionResult = await _http.GetFromJsonAsync<ServiceActionResult<ItemCalc>>($"ItemCalc/All/{itemCode}", _jsonOptions, cancellationToken) ?? new ServiceActionResult<ItemCalc>();
                
            }
            catch (Exception ex)
            {
                actionResult = new ServiceActionResult<ItemCalc>()
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