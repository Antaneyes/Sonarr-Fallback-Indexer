using System.Collections.Generic;

namespace NzbDrone.Core.MetadataSource.Tmdb
{
    public class TmdbTranslationsResponse
    {
        public int Id { get; set; }
        public List<TmdbTranslationResource> Translations { get; set; }
    }

    public class TmdbTranslationResource
    {
        public string Iso_3166_1 { get; set; }
        public string Iso_639_1 { get; set; }
        public string Name { get; set; }
        public TmdbTranslationDataResource Data { get; set; }
    }

    public class TmdbTranslationDataResource
    {
        public string Name { get; set; }
        public string Overview { get; set; }
        public string Homepage { get; set; }
    }

    public class TmdbLocalizedSeriesResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Original_Name { get; set; }
    }
}
