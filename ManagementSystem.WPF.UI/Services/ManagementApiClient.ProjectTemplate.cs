using ManagementSystem.Common.OtherModels;
using ManagementSystem.Common.OtherModels;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;

namespace ManagementSystem.WPF.UI.Services
{
    public partial class ManagementApiClient
    {
        #region ProjectTemplate

        //public async Task<ServiceActionResult> SearchProjectTemplatesAsync(string PTCode, CancellationToken cancellationToken = default)
        //{
        //    SetHttpClientLanguage();

        //    ServiceActionResult actionResult = null;
        //    try
        //    {
        //        var request = new HttpRequestMessage(HttpMethod.Get, _http.BaseAddress.AbsoluteUri.TrimEnd('/') + "/ProjectTemplate/Search")
        //        {
        //            Content = JsonContent.Create(new SearchProjectTemplateForOptions() { PTName = PTCode })
        //        };

        //        HttpResponseMessage httpResponse = await _http.SendAsync(request, cancellationToken);
        //        if (httpResponse.IsSuccessStatusCode)
        //        {
        //            actionResult = await httpResponse.Content.ReadFromJsonAsync<ServiceActionResult>();

        //            if (actionResult.ActionIsSuccess)
        //            {
        //                actionResult.ActionResult = JsonSerializer.Deserialize<SearchOptionResultProjectTemplate[]>(
        //                    (actionResult.ActionResult ?? string.Empty).
        //                    ToString() ?? string.Empty, _jsonOptions) ?? null;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        actionResult = new ServiceActionResult()
        //        {
        //            ActionIsSuccess = false,
        //            ErrorMessage =
        //                string.Format(
        //                    TranslationSource.Instance["HttpClientFailedToReadResponse"] ?? "{0}",
        //                    Environment.NewLine +
        //                    ExceptionHelper.GetErrorMessage(ex, true))
        //        };
        //    }
        //    return actionResult;
        //}

        public async Task<ServiceActionResult> GetAllProjectTemplateImagesAsync(CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult actionResult = new ServiceActionResult();
            try
            {
                SetHttpClientLanguage();

                var resp = await _http.GetAsync("ProjectTemplate/AllImages", cancellationToken);

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

                    var destPath = Path.Combine(_folderImageProjectTemplates, entry.FullName);
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

        public async Task<ServiceActionResult<ProjectTemplate>> GetAllProjectTemplatesAsync(CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult<ProjectTemplate> actionResult;
            try
            {
                actionResult = await _http.GetFromJsonAsync<ServiceActionResult<ProjectTemplate>>("ProjectTemplate/All", _jsonOptions, cancellationToken) ?? new ServiceActionResult<ProjectTemplate>();
                if (actionResult.ActionIsSuccess)
                {
                    foreach (var item in actionResult.ActionResult)
                    {
                        if (!string.IsNullOrEmpty(item.ImagePath))
                        {
                            item.ImagePath = System.IO.Path.Combine(
                                _folderImageProjectTemplates,
                                System.IO.Path.GetFileName(item.ImagePath));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                actionResult = new ServiceActionResult<ProjectTemplate>()
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

        public async Task<ServiceActionResult<ProjectTemplate>> GetProjectTemplatesAsync(CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult<ProjectTemplate> actionResult;
            //ServiceActionResult actionResult;
            try
            {
                //string jsonBody = JsonSerializer.Serialize(pageFilter);

                //using var request = new HttpRequestMessage(HttpMethod.Get, $"ProjectTemplate/page/{pageIndex}")
                //{
                //    Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                //};

                //// Send the request and deserialize the response
                //var response = await _http.SendAsync(request);
                //response.EnsureSuccessStatusCode();

                //actionResult = await response.Content.ReadFromJsonAsync<ServiceActionResult<ProjectTemplate>>(cancellationToken);

                //foreach (var item in actionResult.ActionResult)
                //{
                //    if (!string.IsNullOrEmpty(item.ImagePath))
                //    {
                //        item.ImagePath = await DownloadImageFile(_folderImageProjectTemplates, item.ImagePath, item.ImageFileSize, cancellationToken);
                //    }
                //}
                actionResult = await _http.GetFromJsonAsync<ServiceActionResult<ProjectTemplate>>("ProjectTemplate/All", _jsonOptions, cancellationToken) ?? new ServiceActionResult<ProjectTemplate>();
                if (actionResult.ActionIsSuccess)
                {
                    //actionResult.ActionResult = JsonSerializer.Deserialize<ProjectTemplate[]>(
                    //    (actionResult.ActionResult ?? string.Empty).
                    //    ToString() ?? string.Empty, _jsonOptions) ?? null;
                    await DownloadProjectTemplateImageFile(actionResult.ActionResult, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                actionResult = new ServiceActionResult<ProjectTemplate>()
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

        private async Task DownloadProjectTemplateImageFile(ProjectTemplate[] PTs, CancellationToken cancellationToken)
        {
            foreach (var item in PTs)
            {
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    item.ImagePath = await DownloadImageFile(_folderImageProjectTemplates, item.ImagePath, item.ImageFileSize, cancellationToken);
                }

                await DownloadProjectTemplateImageFile(item, cancellationToken);
            }
        }

        private async Task DownloadProjectTemplateImageFile(ProjectTemplate PT, CancellationToken cancellationToken)
        {
            foreach (var item1 in PT.ProjectTemplates)
            {
                if (!string.IsNullOrEmpty(item1.ImagePath))
                {
                    item1.ImagePath = await DownloadImageFile(_folderImageProjectTemplates, item1.ImagePath, item1.ImageFileSize, cancellationToken);
                }
            }
        }

        public async Task<ServiceActionResult> GetProjectTemplateByIdAsync(int PTId, CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.GetFromJsonAsync<ServiceActionResult>($"ProjectTemplate/{PTId}", cancellationToken) ?? new ServiceActionResult();
                if (actionResult.ActionIsSuccess)
                {
                    actionResult.ActionResult = JsonSerializer.Deserialize<ProjectTemplate>(
                        (actionResult.ActionResult ?? string.Empty).
                        ToString() ?? string.Empty) ?? null;

                    await DownloadProjectTemplateImageFile((ProjectTemplate)actionResult.ActionResult, cancellationToken);

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

        public async Task<ServiceActionResult> GetProjectTemplateLastModifiedAsync(CancellationToken cancellationToken = default)
        {
            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.GetFromJsonAsync<ServiceActionResult>($"ProjectTemplate/LastModified", cancellationToken) ?? new ServiceActionResult();
                if (actionResult.ActionIsSuccess)
                {
                    actionResult.ActionResult = actionResult.ActionResult.ToString();
                    //actionResult.ActionResult = JsonSerializer.Deserialize<ProjectTemplate>(
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

        public async Task<ServiceActionResult> CreateProjectTemplateAsync(ProjectTemplate PT, CancellationToken cancellationToken = default)
        {
            if (PT is null) throw new ArgumentNullException(nameof(PT));

            SetHttpClientLanguage();

            //foreach (ProjectTemplate subPT in PT.ProjectTemplates)
            //{
            //    subPT.ParentProjectTemplate = null;
            //    subPT.ProjectTemplates = null;

            //    foreach (ProjectTemplateDetail subPTD in subPT.ProjectTemplateDetails)
            //    {
            //        subPTD.ProjectTemplate = null;
            //    }
            //}
            // Build multipart/form-data content for form binding on server ([FromForm] ProjectTemplate model, IFormFile[] image)
            using var content = new MultipartFormDataContent();

            foreach (ProjectTemplate subPT in PT.ProjectTemplates)
            {
                if (string.IsNullOrWhiteSpace(subPT.ImagePathToUpload)) continue;
                string imageFileName = subPT.PTName + System.IO.Path.GetExtension(subPT.ImagePathToUpload);

                Stream imageStream = new FileStream(subPT.ImagePathToUpload, FileMode.Open);
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

            var json = JsonSerializer.Serialize(PT, _jsonOptions);
            //var json = JsonSerializer.Serialize(PT);
            content.Add(new StringContent(json, Encoding.UTF8, "application/json"), "formValue");

            var respMultipart = await _http.PostAsync("ProjectTemplate", content, cancellationToken);

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

        public async Task<ServiceActionResult> UpdateProjectTemplateAsync(ProjectTemplate PT, CancellationToken cancellationToken = default)
        {
            if (PT is null) throw new ArgumentNullException(nameof(PT));

            SetHttpClientLanguage();
            using var content = new MultipartFormDataContent();

            foreach (ProjectTemplate subPT in PT.ProjectTemplates)
            {
                if (string.IsNullOrWhiteSpace(subPT.ImagePathToUpload)) continue;
                string imageFileName = subPT.PTName + System.IO.Path.GetExtension(subPT.ImagePathToUpload);

                Stream imageStream = new FileStream(subPT.ImagePathToUpload, FileMode.Open);
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

            var json = JsonSerializer.Serialize(PT, _jsonOptions);
            //var json = JsonSerializer.Serialize(PT);
            content.Add(new StringContent(json, Encoding.UTF8, "application/json"), "formValue");

            var respMultipart = await _http.PutAsync($@"ProjectTemplate/{PT.PTId}", content, cancellationToken);
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

        public async Task<ServiceActionResult> DeleteProjectTemplateAsync(int PTId, CancellationToken cancellationToken = default)
        {
            if (PTId == 0) throw new ArgumentNullException(nameof(PTId));

            SetHttpClientLanguage();

            ServiceActionResult actionResult;
            try
            {
                actionResult = await _http.DeleteFromJsonAsync<ServiceActionResult>($"ProjectTemplate/{PTId}", cancellationToken) ?? new ServiceActionResult();
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