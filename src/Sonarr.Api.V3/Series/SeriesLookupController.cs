using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.MediaCover;
using NzbDrone.Core.MetadataSource;
using NzbDrone.Core.Organizer;
using NzbDrone.Core.SeriesStats;
using NzbDrone.Core.Tv;
using Sonarr.Http;

namespace Sonarr.Api.V3.Series
{
    [V3ApiController("series/lookup")]
    public class SeriesLookupController : Controller
    {
        private readonly ISearchForNewSeries _searchProxy;
        private readonly IBuildFileNames _fileNameBuilder;
        private readonly IMapCoversToLocal _coverMapper;
        private readonly ISeriesTranslationService _seriesTranslationService;

        public SeriesLookupController(ISearchForNewSeries searchProxy,
                                      IBuildFileNames fileNameBuilder,
                                      IMapCoversToLocal coverMapper,
                                      ISeriesTranslationService seriesTranslationService)
        {
            _searchProxy = searchProxy;
            _fileNameBuilder = fileNameBuilder;
            _coverMapper = coverMapper;
            _seriesTranslationService = seriesTranslationService;
        }

        [HttpGet]
        public IEnumerable<SeriesResource> Search([FromQuery] string term)
        {
            var tvDbResults = _searchProxy.SearchForNewSeries(term);
            return MapToResource(tvDbResults);
        }

        private IEnumerable<SeriesResource> MapToResource(IEnumerable<NzbDrone.Core.Tv.Series> series)
        {
            foreach (var currentSeries in series)
            {
                var resource = currentSeries.ToResource();
                resource.DisplayTitle = _seriesTranslationService.FetchDisplayTitle(currentSeries);

                _coverMapper.ConvertToLocalUrls(resource.Id, resource.Images);

                var poster = currentSeries.Images.FirstOrDefault(c => c.CoverType == MediaCoverTypes.Poster);

                if (poster != null)
                {
                    resource.RemotePoster = poster.RemoteUrl;
                }

                resource.Folder = _fileNameBuilder.GetSeriesFolder(GetSeriesForFolder(currentSeries, resource.DisplayTitle));
                resource.Statistics = new SeriesStatistics().ToResource(resource.Seasons);

                yield return resource;
            }
        }

        private NzbDrone.Core.Tv.Series GetSeriesForFolder(NzbDrone.Core.Tv.Series series, string displayTitle)
        {
            if (displayTitle.IsNullOrWhiteSpace() || displayTitle == series.Title)
            {
                return series;
            }

            return new NzbDrone.Core.Tv.Series
            {
                Title = displayTitle,
                Year = series.Year,
                ImdbId = series.ImdbId,
                TvdbId = series.TvdbId,
                TvMazeId = series.TvMazeId,
                TvRageId = series.TvRageId,
                TmdbId = series.TmdbId
            };
        }
    }
}
