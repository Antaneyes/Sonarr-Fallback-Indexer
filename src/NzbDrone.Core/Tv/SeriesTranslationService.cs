using System;
using System.Collections.Generic;
using System.Linq;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.MetadataSource.Tmdb;

namespace NzbDrone.Core.Tv
{
    public interface ISeriesTranslationService
    {
        string Language { get; }
        string GetDisplayTitle(Series series);
        Dictionary<int, string> GetDisplayTitles(IEnumerable<Series> series);
        string FetchDisplayTitle(Series series);
        string RefreshTranslation(Series series);
    }

    public class SeriesTranslationService : ISeriesTranslationService
    {
        public const string DefaultLanguage = "es-ES";

        private readonly ISeriesTranslationRepository _translationRepository;
        private readonly ITmdbSeriesTranslationProxy _tmdbSeriesTranslationProxy;

        public SeriesTranslationService(ISeriesTranslationRepository translationRepository,
                                        ITmdbSeriesTranslationProxy tmdbSeriesTranslationProxy)
        {
            _translationRepository = translationRepository;
            _tmdbSeriesTranslationProxy = tmdbSeriesTranslationProxy;
        }

        public string Language => DefaultLanguage;

        public string GetDisplayTitle(Series series)
        {
            if (series == null || series.Id <= 0)
            {
                return series?.Title;
            }

            var translation = _translationRepository.FindBySeriesIdAndLanguage(series.Id, Language);

            return translation?.Title.IsNotNullOrWhiteSpace() == true ? translation.Title : series.Title;
        }

        public Dictionary<int, string> GetDisplayTitles(IEnumerable<Series> series)
        {
            var seriesList = series.Where(s => s.Id > 0).ToList();
            var translations = _translationRepository.FindBySeriesIdsAndLanguage(seriesList.Select(s => s.Id), Language)
                .Where(t => t.Title.IsNotNullOrWhiteSpace())
                .ToDictionary(t => t.SeriesId, t => t.Title);

            return seriesList.ToDictionary(s => s.Id, s => translations.GetValueOrDefault(s.Id, s.Title));
        }

        public string FetchDisplayTitle(Series series)
        {
            if (series == null)
            {
                return null;
            }

            var translatedTitle = _tmdbSeriesTranslationProxy.GetSeriesTitle(series.TmdbId, Language);

            return translatedTitle.IsNotNullOrWhiteSpace() ? translatedTitle : series.Title;
        }

        public string RefreshTranslation(Series series)
        {
            var translatedTitle = _tmdbSeriesTranslationProxy.GetSeriesTitle(series.TmdbId, Language);

            if (series == null || series.Id <= 0 || translatedTitle.IsNullOrWhiteSpace())
            {
                return translatedTitle;
            }

            var translation = _translationRepository.FindBySeriesIdAndLanguage(series.Id, Language);

            if (translation == null)
            {
                translation = new SeriesTranslation
                {
                    SeriesId = series.Id,
                    TmdbId = series.TmdbId,
                    Language = Language,
                    Title = translatedTitle,
                    LastUpdated = DateTime.UtcNow
                };

                _translationRepository.Insert(translation);
            }
            else
            {
                translation.TmdbId = series.TmdbId;
                translation.Title = translatedTitle;
                translation.LastUpdated = DateTime.UtcNow;
                _translationRepository.Update(translation);
            }

            return translatedTitle;
        }
    }
}
