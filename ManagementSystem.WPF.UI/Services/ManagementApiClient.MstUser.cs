using ManagementSystem.WPF.UI.Data;
using System.Text.Json.Serialization;

namespace ManagementSystem.WPF.UI.Services
{
    public partial class ManagementApiClient
    {
        #region Login

        public async Task<ServiceActionResult> LoginAsync(LoginUser user, CancellationToken cancellationToken = default)
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

        #endregion
    }
}