using FluentMigrator;
using NzbDrone.Core.Datastore.Migration.Framework;

namespace NzbDrone.Core.Datastore.Migration
{
    [Migration(1002)]
    public class add_series_translations : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Create.Table("SeriesTranslations")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("SeriesId").AsInt32().NotNullable()
                .WithColumn("TmdbId").AsInt32().NotNullable()
                .WithColumn("Language").AsString().NotNullable()
                .WithColumn("Title").AsString().Nullable()
                .WithColumn("LastUpdated").AsDateTimeOffset().NotNullable();

            Create.Index().OnTable("SeriesTranslations").OnColumn("SeriesId");
            Create.Index().OnTable("SeriesTranslations").OnColumn("TmdbId");
            Create.Index().OnTable("SeriesTranslations").OnColumn("Language");
        }
    }
}
