using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Trees;
using System.Web.Http;
using MultipleStartNodes.Utilities;
using MultipleStartNodes.Helpers;

namespace MultipleStartNodes.Events
{
    class UmbracoStartupEvent : ApplicationEventHandler
    {
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            //Add a web api handler. Here we can change the values from each web api call.
            //https://github.com/jbreuer/Hybrid-Framework-for-Umbraco-v7-Best-Practises/blob/dc1a41a68456df219b6381623c7b937c39a2098c/Umbraco.Extensions/Events/UmbracoEvents.cs#L66
            GlobalConfiguration.Configuration.MessageHandlers.Add(new WebApiHandler());
        }

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            TreeControllerBase.TreeNodesRendering += TreeControllerBase_TreeNodesRendering;
            TreeControllerBase.MenuRendering += TreeControllerBase_MenuRendering;
            MediaService.Saving += MediaServiceSaving;
        }                   

        private void TreeControllerBase_TreeNodesRendering(TreeControllerBase sender, TreeNodesRenderingEventArgs e)
        {
            if (e.Nodes.Count > 0
                && (e.Nodes.First().ParentId != null && e.Nodes.First().ParentId.ToString() == "-1")
                && (sender.TreeAlias == "content" || sender.TreeAlias == "media")
                && sender.Security.CurrentUser.UserType.Alias != "admin"
                && ((e.QueryStrings.Get("isDialog") == "false" || e.QueryStrings.Get("usestartnodes") == "true")
                    || (e.QueryStrings.Get("isDialog") == "true" && Settings.LimitPickersToStartNodes))
                )
            {
                if (sender.TreeAlias == "content" && sender.Security.CurrentUser.StartContentId == -1)
                {
                    BackOfficeUtils.RenderContentStartNodes(sender.Security.CurrentUser.Id, sender, e);
                }
                else if (sender.TreeAlias == "media" && sender.Security.CurrentUser.StartMediaId == -1)
                {
                    BackOfficeUtils.RenderMediaStartNodes(sender.Security.CurrentUser.Id, sender, e);
                }                
            }
        }

        private void TreeControllerBase_MenuRendering(TreeControllerBase sender, MenuRenderingEventArgs e)
        {
            // Remove all except the Reload option from the root context menu for non admin users
            if (e.NodeId == "-1"
                && (sender.TreeAlias == "content" || sender.TreeAlias == "media")
                && sender.Security.CurrentUser.UserType.Alias != "admin")
            {
                e.Menu.DefaultMenuAlias = "";
                e.Menu.Items.RemoveRange(0, e.Menu.Items.Count - 1);
            }
            // Remove delete, move, and copy options from user's start nodes
            else if (e.NodeId != "-1"
                && (sender.TreeAlias == "content" || sender.TreeAlias == "media")
                && sender.Security.CurrentUser.UserType.Alias != "admin")
            {
                BackOfficeUtils.RestrictContentStartNodeOptions(sender.Security.CurrentUser.Id, sender, e, sender.TreeAlias);
            }
        } 

        private void MediaServiceSaving(IMediaService sender, Umbraco.Core.Events.SaveEventArgs<Umbraco.Core.Models.IMedia> e)
        {
            if (ContextHelpers.EnsureUmbracoContext().Security.CurrentUser.UserType.Alias != "admin" && ContextHelpers.EnsureUmbracoContext().Security.CurrentUser.StartMediaId == -1)
            {
                BackOfficeUtils.ValidateMediaUploadAccess(ContextHelpers.EnsureUmbracoContext().Security.CurrentUser.Id, sender, e);
            }
        }        
    }
}
