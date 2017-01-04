using System;
using Umbraco.Core.Cache;
using System.Net;
using System.Web.Http;
using MultipleStartNodes.Utilities;
using Umbraco.Core;
using MultipleStartNodes.Helpers;

namespace MultipleStartNodes.Models
{
    public class StartNodeRepository
    {
        public static StartNodeCollection GetCachedStartNodesByUserId(int userId)
        {
            ApplicationContext appContext = ContextHelpers.EnsureApplicationContext();
            return GetCachedStartNodesByUserId(userId, appContext, appContext.DatabaseContext);
        }

        public static StartNodeCollection GetCachedStartNodesByUserId(int userId, ApplicationContext applicationContext, DatabaseContext databaseContext)
        {
            StartNodeCollection startNodes = applicationContext.ApplicationCache.RuntimeCache.GetCacheItem<StartNodeCollection>(Resources.CacheKeyPrefix + userId.ToString());

            if (startNodes == null)
            {
                startNodes = GetAndCacheStartNodes(userId, applicationContext, databaseContext);
            }

            return startNodes;
        }

        public static UserStartNodes GetByUserId(int userId, ApplicationContext applicationContext, DatabaseContext databaseContext)
        {
            // validates that the userId corresponds to an actual existing Umbraco user
            if ( applicationContext.Services.UserService.GetUserById(userId) == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return databaseContext.Database.SingleOrDefault<UserStartNodes>(userId);
        }

        public static void Create(UserStartNodes userStartNodes, DatabaseContext databaseContext)
        {
            databaseContext.Database.Insert(userStartNodes);
        }

        public static void Update(UserStartNodes userStartNodes, DatabaseContext databaseContext)
        {
            databaseContext.Database.Update(userStartNodes);
        }

        public static void Delete(UserStartNodes userStartNodes, DatabaseContext databaseContext)
        {
            databaseContext.Database.Delete(userStartNodes);
        }

        public static void Save(UserStartNodes userStartNodes, ApplicationContext applicationContext, DatabaseContext databaseContext)
        {         
            if (databaseContext.Database.Exists<UserStartNodes>(userStartNodes.UserId))
            {
                Update(userStartNodes, databaseContext); 
            }
            else
            {
                Create(userStartNodes, databaseContext);
            }

            CacheUserStartNodes(userStartNodes.UserId, userStartNodes, applicationContext, databaseContext);
        }

        private static StartNodeCollection CacheUserStartNodes(int userId, UserStartNodes userStartNodes, ApplicationContext applicationContext, DatabaseContext databaseContext)
        {
            StartNodeCollection startNodes = new StartNodeCollection();

            if (userStartNodes != null)
            {
                startNodes.Content = (!string.IsNullOrWhiteSpace(userStartNodes.Content)) ? Array.ConvertAll(userStartNodes.Content.Split(','), int.Parse) : null;
                startNodes.Media = (!string.IsNullOrWhiteSpace(userStartNodes.Media)) ? Array.ConvertAll(userStartNodes.Media.Split(','), int.Parse) : null;
            }

            applicationContext.ApplicationCache.RuntimeCache.InsertCacheItem<StartNodeCollection>(Resources.CacheKeyPrefix + userId.ToString(), () => startNodes);

            return startNodes;
        }

        private static StartNodeCollection GetAndCacheStartNodes(int userId, ApplicationContext applicationContext, DatabaseContext databaseContext)
        {
            UserStartNodes userStartNodes = GetByUserId(userId, applicationContext, databaseContext);

            return CacheUserStartNodes(userId, userStartNodes, applicationContext, databaseContext);
        }
    }
}
