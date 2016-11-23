using MultipleStartNodes.Models;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace MultipleStartNodes.Controllers
{
    [PluginController("MultipleStartNodes")]
    public class UserStartNodesApiController : UmbracoAuthorizedJsonController
    {
        public UserStartNodes GetById(int userId)
        {
            StartNodeCollection startNodes = StartNodeRepository.GetCachedStartNodesByUserId(userId);
            UserStartNodes userStartNodes = new UserStartNodes();
            userStartNodes.UserId = userId;
            userStartNodes.Content = (startNodes.Content != null) ? string.Join(",", startNodes.Content) : "";
            userStartNodes.Media = (startNodes.Media != null) ? string.Join(",", startNodes.Media) : "";

            return userStartNodes;
        }

        public UserStartNodes Save(UserStartNodes userStartNodes)
        {
            StartNodeRepository.Save(userStartNodes);

            return userStartNodes;
        }
    }
}
