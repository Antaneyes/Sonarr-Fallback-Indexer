using System.Collections.Generic;
using System.Linq;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.Messaging.Events;

namespace NzbDrone.Core.Tv
{
    public interface ISeriesTranslationRepository : IBasicRepository<SeriesTranslation>
    {
        SeriesTranslation FindBySeriesIdAndLanguage(int seriesId, string language);
        List<SeriesTranslation> FindBySeriesIdsAndLanguage(IEnumerable<int> seriesIds, string language);
    }

    public class SeriesTranslationRepository : BasicRepository<SeriesTranslation>, ISeriesTranslationRepository
    {
        public SeriesTranslationRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }

        public SeriesTranslation FindBySeriesIdAndLanguage(int seriesId, string language)
        {
            return Query(t => t.SeriesId == seriesId && t.Language == language).SingleOrDefault();
        }

        public List<SeriesTranslation> FindBySeriesIdsAndLanguage(IEnumerable<int> seriesIds, string language)
        {
            var ids = seriesIds.Distinct().ToList();

            if (!ids.Any())
            {
                return new List<SeriesTranslation>();
            }

            return Query(t => ids.Contains(t.SeriesId) && t.Language == language).ToList();
        }
    }
}
