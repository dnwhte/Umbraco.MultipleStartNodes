using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace MultipleStartNodes.Utilities
{
    public class Resources
    {
        static DatabaseSchemaHelper databaseSchemaHelper = null;

        public static IRuntimeCacheProvider AppCache
        {
            get { return ApplicationContext.Current.ApplicationCache.RuntimeCache; }
        }

        public static IEntityService EntityService
        {
            get { return ApplicationContext.Current.Services.EntityService; }
        }          

        public static ServiceContext Services
        {
            get { return ApplicationContext.Current.Services; }
        }

        public static UmbracoHelper Umbraco
        {
            get { return new UmbracoHelper(UmbracoContext.Current); }
        }

        public static string CacheKeyPrefix
        {
            get { return "UserStartNodes"; }
        }

        public static DatabaseContext DatabaseContext
        {
            get { return ApplicationContext.Current.DatabaseContext; }
        }

        public static Database Database
        {
            get { return ApplicationContext.Current.DatabaseContext.Database; }
        }

        public static DatabaseSchemaHelper DatabaseSchemaHelper
        {
            get
            {
                if (databaseSchemaHelper == null)
                {
                    databaseSchemaHelper = new DatabaseSchemaHelper(Resources.Database, ApplicationContext.Current.ProfilingLogger.Logger, Resources.DatabaseContext.SqlSyntax);
                }
                return databaseSchemaHelper;
            }
        }     
    }
}
