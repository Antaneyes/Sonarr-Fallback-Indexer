using System;
using System.Linq;
using NLog;
using NzbDrone.Common.Extensions;
using NzbDrone.Common.Http;
using NzbDrone.Core.Configuration;

namespace NzbDrone.Core.MetadataSource.Tmdb
{
    public interface ITmdbSeriesTranslationProxy
    {
        string GetSeriesTitle(int tmdbId, string language);
    }

    public class TmdbSeriesTranslationProxy : ITmdbSeriesTranslationProxy
    {
        private const string TmdbUrl = "https://api.themoviedb.org/3";

        private readonly IHttpClient _httpClient;
        private readonly IConfigService _configService;
        private readonly Logger _logger;

        public TmdbSeriesTranslationProxy(IHttpClient httpClient, IConfigService configService, Logger logger)
        {
            _httpClient = httpClient;
            _configService = configService;
            _logger = logger;
        }

        public string GetSeriesTitle(int tmdbId, string language)
        {
            if (tmdbId <= 0 || _configService.TmdbApiKey.IsNullOrWhiteSpace())
            {
                return null;
            }

            var localizedTitle = GetLocalizedSeriesTitle(tmdbId, language);

            if (localizedTitle.IsNotNullOrWhiteSpace())
            {
                return localizedTitle;
            }

            var requestBuilder = new HttpRequestBuilder(TmdbUrl)
                .Resource($"/tv/{tmdbId}/translations");

            requestBuilder.SuppressHttpError = true;

            ApplyAuthentication(requestBuilder);

            try
            {
                var response = _httpClient.Get<TmdbTranslationsResponse>(requestBuilder.Build());
                var translations = response.Resource?.Translations;

                if (translations == null)
                {
                    return null;
                }

                var languageParts = language.Split('-');
                var languageCode = languageParts[0];
                var countryCode = languageParts.Length > 1 ? languageParts[1] : null;

                var translation = translations.FirstOrDefault(t =>
                                      t.Iso_639_1.Equals(languageCode, StringComparison.InvariantCultureIgnoreCase) &&
                                      countryCode != null &&
                                      t.Iso_3166_1.Equals(countryCode, StringComparison.InvariantCultureIgnoreCase)) ??
                                  translations.FirstOrDefault(t =>
                                      t.Iso_639_1.Equals(languageCode, StringComparison.InvariantCultureIgnoreCase));

                return translation?.Data?.Name.IsNotNullOrWhiteSpace() == true ? translation.Data.Name : null;
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Unable to fetch TMDb translation for series {0}", tmdbId);
                return null;
            }
        }

        private string GetLocalizedSeriesTitle(int tmdbId, string language)
        {
            var requestBuilder = new HttpRequestBuilder(TmdbUrl)
                .Resource($"/tv/{tmdbId}")
                .AddQueryParam("language", language);

            requestBuilder.SuppressHttpError = true;

            ApplyAuthentication(requestBuilder);

            try
            {
                var response = _httpClient.Get<TmdbLocalizedSeriesResource>(requestBuilder.Build());
                var title = response.Resource?.Name;

                return title.IsNotNullOrWhiteSpace() ? title : null;
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Unable to fetch localized TMDb series details for series {0}", tmdbId);
                return null;
            }
        }

        private void ApplyAuthentication(HttpRequestBuilder requestBuilder)
        {
            var apiKey = _configService.TmdbApiKey.Trim();

            if (apiKey.StartsWith("eyJ") && apiKey.Contains('.'))
            {
                requestBuilder.SetHeader("Authorization", $"Bearer {apiKey}");
            }
            else
            {
                requestBuilder.AddQueryParam("api_key", apiKey);
            }
        }
    }
}
