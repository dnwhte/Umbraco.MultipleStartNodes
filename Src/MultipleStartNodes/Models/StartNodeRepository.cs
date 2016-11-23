using System;
using Umbraco.Core.Cache;
using System.Net;
using System.Web.Http;
using MultipleStartNodes.Utilities;

namespace MultipleStartNodes.Models
{
    public class StartNodeRepository
    {
        public static StartNodeCollection GetCachedStartNodesByUserId(int userId)
        {
            StartNodeCollection startNodes = Resources.AppCache.GetCacheItem<StartNodeCollection>(Resources.CacheKeyPrefix + userId.ToString());

            if (startNodes == null)
            {
                startNodes = GetAndCacheStartNodes(userId);
            }

            return startNodes;
        }

        public static UserStartNodes GetByUserId(int userId)
        {
            // validates that the userId corresponds to an actual existing Umbraco user
            if (Resources.Services.UserService.GetUserById(userId) == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return Resources.Database.SingleOrDefault<UserStartNodes>(userId);
        }

        public static void Create(UserStartNodes userStartNodes)
        {
            Resources.Database.Insert(userStartNodes);
        }

        public static void Update(UserStartNodes userStartNodes)
        {
            Resources.Database.Update(userStartNodes);
        }

        public static void Delete(UserStartNodes userStartNodes)
        {
            Resources.Database.Delete(userStartNodes);
        }

        public static void Save(UserStartNodes userStartNodes)
        {         
            if (Resources.Database.Exists<UserStartNodes>(userStartNodes.UserId))
            {
                Update(userStartNodes); 
            }
            else
            {
                Create(userStartNodes);
            }

            CacheUserStartNodes(userStartNodes.UserId, userStartNodes);
        }

        private static StartNodeCollection CacheUserStartNodes(int userId, UserStartNodes userStartNodes)
        {
            StartNodeCollection startNodes = new StartNodeCollection();

            if (userStartNodes != null)
            {
                startNodes.Content = (!string.IsNullOrWhiteSpace(userStartNodes.Content)) ? Array.ConvertAll(userStartNodes.Content.Split(','), int.Parse) : null;
                startNodes.Media = (!string.IsNullOrWhiteSpace(userStartNodes.Media)) ? Array.ConvertAll(userStartNodes.Media.Split(','), int.Parse) : null;
            }

            Resources.AppCache.InsertCacheItem<StartNodeCollection>(Resources.CacheKeyPrefix + userId.ToString(), () => startNodes);

            return startNodes;
        }

        private static StartNodeCollection GetAndCacheStartNodes(int userId)
        {
            UserStartNodes userStartNodes = GetByUserId(userId);

            return CacheUserStartNodes(userId, userStartNodes);
        }
    }
}
