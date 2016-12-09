# Umbraco.MultipleStartNodes
Adds the ability to have multiple content and media start nodes for Umbraco users. 

##How it works

**On the back-end** the package relies heavily on Umbraco's [Tree Events](https://our.umbraco.org/documentation/extending/section-trees/trees-v7) and ASP.NET's [Delegating Handlers](https://www.asp.net/web-api/overview/advanced/httpclient-message-handlers).

###Tree Events###

The *TreeControllerBase.TreeNodesRendering* event is used to replace the nodes that appear when the content/media tree is initially rendered. This is done by clearing the e.Nodes list, querying our custom table for the current user's start nodes, and adding them to e.Nodes.

The *TreeControllerBase.MenuRendering* event is used to remove admin functions (create, sort, republish) from the content/media root node as well as remove any context menu options in the config RemoveActionsForStartNodes from the user's start nodes.

###Delegating Handlers###

*WebApiHandler* captures a number UmbracoApi requests and modifies the result before returning it to the client.

    /umbraco/backoffice/umbracoapi/content/getbyid
    /umbraco/backoffice/umbracoapi/content/postsave

Lorem ipsum dolor sit amet, consectetur adipisicing elit. Reiciendis, praesentium.

    /umbraco/backoffice/umbracoapi/media/getbyid
    /umbraco/backoffice/umbracoapi/media/postsave

Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, placeat.

    /umbraco/backoffice/umbracoapi/entity/getancestors

Lorem ipsum dolor sit amet, consectetur adipisicing elit. Libero, pariatur.

    /umbraco/backoffice/umbracoapi/entity/searchall

Lorem ipsum dolor sit amet, consectetur adipisicing elit. Laudantium, eaque.

    /umbraco/backoffice/umbracoapi/entity/search

Lorem ipsum dolor sit amet, consectetur adipisicing elit. Dolore, cupiditate.

    /umbraco/backoffice/umbracoapi/media/getchildren

Lorem ipsum dolor sit amet, consectetur adipisicing elit. Provident, nam.

    /umbraco/backoffice/umbracoapi/media/getchildfolders

Lorem ipsum dolor sit amet, consectetur adipisicing elit. Tempora, incidunt.

    /umbraco/backoffice/umbracoapi/content/postmove
    /umbraco/backoffice/umbracoapi/content/postcopy
    /umbraco/backoffice/umbracoapi/media/postmove

Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ullam, necessitatibus?

---

**On the front-end** 

