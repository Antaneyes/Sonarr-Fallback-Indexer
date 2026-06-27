using System;
using NzbDrone.Core.Datastore;

namespace NzbDrone.Core.Tv
{
    public class SeriesTranslation : ModelBase
    {
        public int SeriesId { get; set; }
        public int TmdbId { get; set; }
        public string Language { get; set; }
        public string Title { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
