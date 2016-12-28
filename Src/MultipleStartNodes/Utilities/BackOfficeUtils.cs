using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Trees;
using Umbraco.Core.Models.EntityBase;
using Umbraco.Core.Models;
using System;
using MultipleStartNodes.Models;

namespace MultipleStartNodes.Utilities
{
    public class BackOfficeUtils
    {      
        public static void RenderContentStartNodes(int userId, TreeControllerBase sender, TreeNodesRenderingEventArgs e)
        {
            StartNodeCollection startNodes = StartNodeRepository.GetCachedStartNodesByUserId(userId);
               
            if (startNodes.Content == null)
            {                
                return;
            }        

            if (startNodes.Content.Any())
            {
                // Remove default start nodes
                e.Nodes.Clear();

                IEnumerable<IUmbracoEntity> startNodesEntities = sender.Services.EntityService.GetAll(Umbraco.Core.Models.UmbracoObjectTypes.Document, startNodes.Content);

                // Feels like a lot of duct tape. A lot taken from:
                // https://github.com/umbraco/Umbraco-CMS/blob/5397f2c53acbdeb0805e1fe39fda938f571d295a/src/Umbraco.Web/Trees/ContentTreeController.cs#L75
                foreach (IUmbracoEntity startNodeEntity in startNodesEntities)
                {
                    UmbracoEntity entity = (UmbracoEntity)startNodeEntity;

                    // Not as safe as the approach in core
                    // https://github.com/umbraco/Umbraco-CMS/blob/5397f2c53acbdeb0805e1fe39fda938f571d295a/src/Umbraco.Core/Models/UmbracoEntityExtensions.cs#L34
                    bool isContainer = (entity.AdditionalData.ContainsKey("IsContainer") && entity.AdditionalData["IsContainer"].ToString() == "True");

                    TreeNode node = sender.CreateTreeNode(
                        startNodeEntity.Id.ToInvariantString(),
                        "-1",
                        e.QueryStrings,
                        startNodeEntity.Name,
                        entity.ContentTypeIcon,
                        entity.HasChildren && (isContainer == false)
                    );

                    AddQueryStringsToAdditionalData(node, e.QueryStrings);

                    if (e.QueryStrings.Get("isDialog") == "true")
                    {
                        node.RoutePath = "#";
                    }

                    // TODO: How should we order nodes?

                    e.Nodes.Add(node);
                }
            }
        }

        public static void RenderMediaStartNodes(int userId, TreeControllerBase sender, TreeNodesRenderingEventArgs e)
        {
            StartNodeCollection startNodes = StartNodeRepository.GetCachedStartNodesByUserId(userId);
            
            if (startNodes.Media == null)
            {                
                return;
            }

            if (startNodes.Media.Any())
            {
                // Remove default start nodes
                e.Nodes.Clear();
                IEnumerable<IUmbracoEntity> startNodesEntities = sender.Services.EntityService.GetAll(Umbraco.Core.Models.UmbracoObjectTypes.Media, startNodes.Media);

                foreach (IUmbracoEntity startNodeEntity in startNodesEntities)
                {
                    UmbracoEntity entity = (UmbracoEntity)startNodeEntity;

                    bool isContainer = (entity.AdditionalData.ContainsKey("IsContainer") && entity.AdditionalData["IsContainer"].ToString() == "True");

                    TreeNode node = sender.CreateTreeNode(
                        startNodeEntity.Id.ToInvariantString(),
                        "-1",
                        e.QueryStrings,
                        startNodeEntity.Name,
                        entity.ContentTypeIcon,
                        entity.HasChildren && (isContainer == false)
                    );

                    node.AdditionalData.Add("contentType", entity.ContentTypeAlias);

                    if (isContainer)
                    {
                        node.SetContainerStyle();
                        node.AdditionalData.Add("isContainer", true);
                    }

                    // TODO: How should we order nodes?

                    e.Nodes.Add(node);
                }
            }
        }

        public static void ValidateMediaUploadAccess(int userId, IMediaService sender, Umbraco.Core.Events.SaveEventArgs<Umbraco.Core.Models.IMedia> e)
        {
            IMedia firstItem = e.SavedEntities.FirstOrDefault();
            if (firstItem.HasIdentity)
                return;

            int[] startNodes = StartNodeRepository.GetCachedStartNodesByUserId(userId).Media;

            if (startNodes == null)
                return;

            if (!PathContainsAStartNode(firstItem.Path, startNodes))
            {
                e.CanCancel = true;
                e.CancelOperation(new Umbraco.Core.Events.EventMessage("Permission Denied", "You do not have permission to upload files to this folder."));
                foreach (var m in e.SavedEntities)
                {
                    sender.Delete(m);
                }
            }         
        }

        private static bool PathContainsAStartNode(string path, int[] startNodes)
        {
            int[] pathArray = Array.ConvertAll(path.Split(','), int.Parse);
            int firstIntersectionValue = pathArray.Intersect(startNodes).FirstOrDefault();

            return firstIntersectionValue != 0;
        }

        // Taken from Umbraco Source
        // https://github.com/umbraco/Umbraco-CMS/blob/5397f2c53acbdeb0805e1fe39fda938f571d295a/src/Umbraco.Web/Trees/TreeControllerBase.cs#L276
        protected static void AddQueryStringsToAdditionalData(TreeNode node, FormDataCollection queryStrings)
        {
            foreach (var q in queryStrings.Where(x => node.AdditionalData.ContainsKey(x.Key) == false))
            {
                node.AdditionalData.Add(q.Key, q.Value);
            }
        }

        internal static void RestrictContentStartNodeOptions(int userId, TreeControllerBase sender, MenuRenderingEventArgs e, string section)
        {
            int[] startNodes = null;

            if (section == "content")
            {
                startNodes = StartNodeRepository.GetCachedStartNodesByUserId(userId).Content;
            }
            else if (section == "media")
            {
                startNodes = StartNodeRepository.GetCachedStartNodesByUserId(userId).Media;
            }            

            if (startNodes == null || !startNodes.Contains(int.Parse(e.NodeId)))
                return;
            
            string[] preventedActionAliases = Settings.RemoveActionsForStartNodes;
            e.Menu.Items.RemoveAll(i => preventedActionAliases.Contains(i.Alias));
        }
    }
}
