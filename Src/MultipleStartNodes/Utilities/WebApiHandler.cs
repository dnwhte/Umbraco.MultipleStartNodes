using AutoMapper;
using MultipleStartNodes.Helpers;
using MultipleStartNodes.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Membership;
using Umbraco.Web.Models.ContentEditing;
using Umbraco.Web.WebApi;

namespace MultipleStartNodes.Utilities
{
    public class WebApiHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            switch (request.RequestUri.AbsolutePath.ToLower())
            {                
                case "/umbraco/backoffice/umbracoapi/content/getbyid":
                case "/umbraco/backoffice/umbracoapi/content/postsave":
                    return RemoveInacessibleContentNodesFromPath(request, cancellationToken); // prevents constant tree reloading               
                case "/umbraco/backoffice/umbracoapi/media/getbyid":
                case "/umbraco/backoffice/umbracoapi/media/postsave":
                    return RemoveInacessibleMediaNodesFromPath(request, cancellationToken); // prevents constant tree reloading               
                case "/umbraco/backoffice/umbracoapi/entity/getancestors":
                    return RemoveInaccessibleAncestorsFromBreadcrumbs(request, cancellationToken); // used at the bottom of the edit view and in the media picker
                case "/umbraco/backoffice/umbracoapi/entity/searchall":
                    return RemoveInaccessibleNodesFromSearchResults(request, cancellationToken); // primary umbraco back-office search
                case "/umbraco/backoffice/umbracoapi/entity/search":
                    return RemoveInaccessibleNodesFromContentSearchResults(request, cancellationToken); // for the copy/move menus only ... I think
                case "/umbraco/backoffice/umbracoapi/media/getchildren":
                    return HandleListViewStartNodes(request, cancellationToken); // for the media section and picker list view
                case "/umbraco/backoffice/umbracoapi/media/getchildfolders":
                    return HandleRootChildFolders(request, cancellationToken); // more the media section grid view only...I think
                case "/umbraco/backoffice/umbracoapi/content/postmove":
                case "/umbraco/backoffice/umbracoapi/content/postcopy":
                case "/umbraco/backoffice/umbracoapi/media/postmove":
                    return RemoveInacessibleNodesFromPathPostMoveAndCopy(request, cancellationToken); // prevents constant tree reloading
                default:
                    return base.SendAsync(request, cancellationToken);
            }
        }        

        private Task<HttpResponseMessage> RemoveInacessibleContentNodesFromPath(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IUser user = ContextHelpers.EnsureUmbracoContext().Security.CurrentUser;
            int[] startNodes = StartNodeRepository.GetCachedStartNodesByUserId(user.Id).Content;

            if (user.UserType.Alias == "admin" || startNodes == null)
                return base.SendAsync(request, cancellationToken);

            return base.SendAsync(request, cancellationToken)
                .ContinueWith(task =>
                {
                    HttpResponseMessage response = task.Result;
                    try
                    {                        
                        HttpContent data = response.Content;
                        ContentItemDisplay content = ((ObjectContent)(data)).Value as ContentItemDisplay;

                        if (!PathContainsAStartNode(content.Path, startNodes))
                        {                            
                            response.StatusCode = HttpStatusCode.Forbidden; // prevent users from editing a node they shouldn't
                        }

                        content.Path = RemoveStartNodeAncestors(content.Path, startNodes);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error<WebApiHandler>("Could not update path.", ex);
                    }
                    return response;
                }
            );
        }

        private Task<HttpResponseMessage> RemoveInacessibleMediaNodesFromPath(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IUser user = ContextHelpers.EnsureUmbracoContext().Security.CurrentUser;
            int[] startNodes = StartNodeRepository.GetCachedStartNodesByUserId(user.Id).Media;

            if (user.UserType.Alias == "admin" || startNodes == null)
                return base.SendAsync(request, cancellationToken);

            return base.SendAsync(request, cancellationToken)
                .ContinueWith(task =>
                {
                    HttpResponseMessage response = task.Result;
                    try
                    {
                        HttpContent data = response.Content;
                        MediaItemDisplay media = ((ObjectContent)(data)).Value as MediaItemDisplay;

                        if (!PathContainsAStartNode(media.Path, startNodes))
                        {
                            response.StatusCode = HttpStatusCode.Forbidden; // prevent users from editing a node they shouldn't
                        }

                        media.Path = RemoveStartNodeAncestors(media.Path, startNodes);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error<WebApiHandler>("Could not update path.", ex);
                    }
                    return response;
                }
            );
        }

        private Task<HttpResponseMessage> RemoveInaccessibleAncestorsFromBreadcrumbs(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IUser user = ContextHelpers.EnsureUmbracoContext().Security.CurrentUser;
            int[] startNodes;

            if (request.RequestUri.Query.Contains("type=document"))
            {
                startNodes = StartNodeRepository.GetCachedStartNodesByUserId(user.Id).Content;
            }
            else if (request.RequestUri.Query.Contains("type=media"))
            {
                startNodes = StartNodeRepository.GetCachedStartNodesByUserId(user.Id).Media;
            }
            else
            {
                return base.SendAsync(request, cancellationToken);
            }            

            if (user.UserType.Alias == "admin" || startNodes == null)
                return base.SendAsync(request, cancellationToken);

            return base.SendAsync(request, cancellationToken)
                .ContinueWith(task =>
                {
                    HttpResponseMessage response = task.Result;
                    try
                    {
                        HttpContent data = response.Content;
                        ObjectContent dataContent = ((ObjectContent)(data));
                        IEnumerable<EntityBasic> entities = dataContent.Value as IEnumerable<EntityBasic>;
                        List<EntityBasic> entitiesList = entities.ToList();

                        if (Settings.LimitPickersToStartNodes)
                        {
                            entitiesList = entitiesList.Where(e => PathContainsAStartNode(e.Path, startNodes)).ToList();
                        }
                        else
                        {
                            foreach (EntityBasic e in entitiesList)
                            {
                                e.AdditionalData.Add("Hidden", !PathContainsAStartNode(e.Path, startNodes));
                            }
                        }                        

                        dataContent.Value = entitiesList;
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error<WebApiHandler>("Could not remove ancestors from path.", ex);
                    }
                    return response;
                }
            );
        }

        private Task<HttpResponseMessage> RemoveInaccessibleNodesFromSearchResults(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IUser user = ContextHelpers.EnsureUmbracoContext().Security.CurrentUser;
            StartNodeCollection startNodes = StartNodeRepository.GetCachedStartNodesByUserId(user.Id);

            if (user.UserType.Alias == "admin")
                return base.SendAsync(request, cancellationToken);

            return base.SendAsync(request, cancellationToken)
                .ContinueWith(task =>
                {
                    HttpResponseMessage response = task.Result;
                    try
                    {
                        HttpContent data = response.Content;
                        ObjectContent dataContent = ((ObjectContent)(data));
                        IEnumerable<EntityTypeSearchResult> entities = dataContent.Value as IEnumerable<EntityTypeSearchResult>;

                        EntityTypeSearchResult contentResults = entities.FirstOrDefault(e => e.EntityType == "Document");
                        EntityTypeSearchResult mediaResults = entities.FirstOrDefault(e => e.EntityType == "Media");

                        if (startNodes.Content != null && contentResults.Results.Any())
                        {
                            contentResults.Results = contentResults.Results.Where(e => PathContainsAStartNode(e.Path, startNodes.Content));
                        }

                        if (startNodes.Media != null && mediaResults.Results.Any())
                        {
                            mediaResults.Results = mediaResults.Results.Where(e => PathContainsAStartNode(e.Path, startNodes.Media));
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error<WebApiHandler>("Could not remove inaccessible nodes from search results.", ex);
                    }
                    return response;
                }
            );
        }

        private Task<HttpResponseMessage> RemoveInaccessibleNodesFromContentSearchResults(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.RequestUri.Query.ToLower().Contains("type=document"))
            {
                return base.SendAsync(request, cancellationToken);
            }

            IUser user = ContextHelpers.EnsureUmbracoContext().Security.CurrentUser;
            int[] startNodes = StartNodeRepository.GetCachedStartNodesByUserId(user.Id).Content;

            if (user.UserType.Alias == "admin" || startNodes == null)
                return base.SendAsync(request, cancellationToken);

            return base.SendAsync(request, cancellationToken)
                .ContinueWith(task =>
                {
                    HttpResponseMessage response = task.Result;
                    try
                    {
                        HttpContent data = response.Content;
                        ObjectContent dataContent = ((ObjectContent)(data));
                        IEnumerable<EntityBasic> entities = dataContent.Value as IEnumerable<EntityBasic>;

                        entities = entities.Where(e => PathContainsAStartNode(e.Path, startNodes));

                        dataContent.Value = entities;
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error<WebApiHandler>("Could not remove inaccessible nodes from search results.", ex);
                    }
                    return response;
                }
            );
        }

        private Task<HttpResponseMessage> HandleListViewStartNodes(HttpRequestMessage request, CancellationToken cancellationToken)
        {            
            // do at root in the media section or in the picker when it should be limited
            if (request.RequestUri.Query.Contains("id=-1") && (request.RequestUri.Query.Contains("pageNumber=1") || Settings.LimitPickersToStartNodes))
            {
                IUser user = ContextHelpers.EnsureUmbracoContext().Security.CurrentUser;
                ApplicationContext appContext = ContextHelpers.EnsureApplicationContext();
                int[] startNodes = StartNodeRepository.GetCachedStartNodesByUserId(user.Id, appContext, appContext.DatabaseContext).Media;

                if (user.UserType.Alias == "admin" || startNodes == null)
                    return base.SendAsync(request, cancellationToken);

                return base.SendAsync(request, cancellationToken)
                    .ContinueWith(task =>
                    {
                        HttpResponseMessage response = task.Result;
                        try
                        {
                            HttpContent data = response.Content;
                            ObjectContent dataContent = ((ObjectContent)(data));

                            int itemCount = startNodes.Length;
                            IMedia[] startIMedia = appContext.Services.MediaService.GetByIds(startNodes).ToArray();

                            var pagedResult = new PagedResult<ContentItemBasic<ContentPropertyBasic, IMedia>>(itemCount, 1, itemCount);
                            pagedResult.Items = startIMedia
                                .Select(Mapper.Map<IMedia, ContentItemBasic<ContentPropertyBasic, IMedia>>);

                            dataContent.Value = pagedResult;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error<WebApiHandler>("Could not replace start nodes.", ex);
                        }
                        return response;
                    }
                );
            }
            return base.SendAsync(request, cancellationToken);
        }

        private Task<HttpResponseMessage> HandleRootChildFolders(HttpRequestMessage request, CancellationToken cancellationToken)
        {            
            if (request.RequestUri.Query.Contains("id=-1"))
            {
                IUser user = ContextHelpers.EnsureUmbracoContext().Security.CurrentUser;
                ApplicationContext appContext = ContextHelpers.EnsureApplicationContext();
                int[] startNodes = StartNodeRepository.GetCachedStartNodesByUserId(user.Id, appContext, appContext.DatabaseContext).Media;

                if (user.UserType.Alias == "admin" || startNodes == null)
                    return base.SendAsync(request, cancellationToken);

                return base.SendAsync(request, cancellationToken)
                    .ContinueWith(task =>
                    {
                        HttpResponseMessage response = task.Result;
                        try
                        {
                            HttpContent data = response.Content;
                            ObjectContent dataContent = ((ObjectContent)(data));                            

                            IEnumerable<int> folderTypes = appContext.Services.ContentTypeService.GetAllMediaTypes().ToArray().Where(x => x.Alias.EndsWith("Folder")).Select(x => x.Id);
                            IMedia[] children = appContext.Services.MediaService.GetByIds(startNodes).ToArray();
                            dataContent.Value = children.Where(x => folderTypes.Contains(x.ContentTypeId)).Select(Mapper.Map<IMedia, ContentItemBasic<ContentPropertyBasic, IMedia>>);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error<WebApiHandler>("Could not replace start nodes.", ex);
                        }
                        return response;
                    }
                );
            }
            return base.SendAsync(request, cancellationToken);
        }

        private Task<HttpResponseMessage> RemoveInacessibleNodesFromPathPostMoveAndCopy(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IUser user = ContextHelpers.EnsureUmbracoContext().Security.CurrentUser;
            int[] startNodes;

            if (request.RequestUri.AbsolutePath.ToLower().Contains("/content/"))
            {
                startNodes = StartNodeRepository.GetCachedStartNodesByUserId(user.Id).Content;
            }
            else if (request.RequestUri.AbsolutePath.ToLower().Contains("/media/"))
            {
                startNodes = StartNodeRepository.GetCachedStartNodesByUserId(user.Id).Media;
            }
            else
            {
                return base.SendAsync(request, cancellationToken);
            }

            if (user.UserType.Alias == "admin" || startNodes == null)
                return base.SendAsync(request, cancellationToken);

            //// prevent moving/copying into inaccessible locations
            // do some hackery to read the post data more than once - http://stackoverflow.com/questions/12007689/cannot-read-body-data-from-web-api-post
            MediaTypeHeaderValue contentType = request.Content.Headers.ContentType;
            MoveOrCopy postModel = request.Content.ReadAsAsync<MoveOrCopy>().Result;            
            string contentInString = JsonConvert.SerializeObject(postModel);
            request.Content = new StringContent(contentInString);
            request.Content.Headers.ContentType = contentType;

            if (IndexOfInt(startNodes, postModel.ParentId) == -1)
            {
                // take error notification from https://github.com/umbraco/Umbraco-CMS/blob/a2a4ad39476f4a18c8fe2c04d42f6fa635551b63/src/Umbraco.Web/Editors/MediaController.cs#L656
                ApplicationContext appContext = ContextHelpers.EnsureApplicationContext();
                SimpleNotificationModel notificationModel = new SimpleNotificationModel();
                notificationModel.AddErrorNotification(appContext.Services.TextService.Localize("moveOrCopy/notValid", CultureInfo.CurrentCulture), "");
                throw new HttpResponseException(request.CreateValidationErrorResponse(notificationModel));
            }
            else
            {
                // perform default request
                return base.SendAsync(request, cancellationToken)
                    .ContinueWith(task =>
                    {
                        HttpResponseMessage response = task.Result;
                        try
                        {
                            string path = response.Content.ReadAsStringAsync().Result;
                            path = RemoveStartNodeAncestors(path, startNodes);
                            response.Content = new StringContent(path, Encoding.UTF8, "application/json");
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error<WebApiHandler>("Could not update path.", ex);
                        }
                        return response;
                    }
                );
            }
        }

        private string RemoveStartNodeAncestors(string path, int[] startNodes, int removeStartIndex = 1)
        {
            int[] pathArray = Array.ConvertAll(path.Split(','), int.Parse);

            int firstIntersectionValue = pathArray.Intersect(startNodes).FirstOrDefault();

            if (firstIntersectionValue != 0)
            {
                int index = IndexOfInt(pathArray, firstIntersectionValue);
                List<int> pathIntList = pathArray.ToList();
                pathIntList.RemoveRange(removeStartIndex, index - 1);
                path = string.Join(",", pathIntList);
            }

            return path;
        }

        private bool PathContainsAStartNode(string path, int[] startNodes){
            int[] pathArray = Array.ConvertAll(path.Split(','), int.Parse);
            int firstIntersectionValue = pathArray.Intersect(startNodes).FirstOrDefault();

            return firstIntersectionValue != 0;
        }

        private int IndexOfInt(int[] arr, int value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == value)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
