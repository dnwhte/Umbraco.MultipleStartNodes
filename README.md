# Umbraco.MultipleStartNodes
This package gives you the ability to set multiple content/media start nodes for users.

##How it works

**On the back-end** the package relies heavily on Umbraco's [Tree Events](https://our.umbraco.org/documentation/extending/section-trees/trees-v7) and ASP.NET's [Delegating Handlers](https://www.asp.net/web-api/overview/advanced/httpclient-message-handlers).

###Tree Events###

The *TreeControllerBase.TreeNodesRendering* event is used to replace the nodes that appear when the content/media tree is initially rendered. This is done by clearing the e.Nodes list, querying our custom table for the current user's start nodes, and adding them to e.Nodes.

The *TreeControllerBase.MenuRendering* event is used to remove admin functions (create, sort, republish) from the content/media root node as well as remove any context menu options in the config RemoveActionsForStartNodes from the user's start nodes.

###Delegating Handlers###

*WebApiHandler* captures a number UmbracoApi requests and modifies the result before returning it to the client.

####List of captured api requests and the actions taken####

    /umbraco/backoffice/umbracoapi/content/getbyid
    /umbraco/backoffice/umbracoapi/content/postsave
    /umbraco/backoffice/umbracoapi/media/getbyid
    /umbraco/backoffice/umbracoapi/media/postsave

Throw an http forbidden error if the user shouldn't have access to the node. Otherwise remove inaccessible ancestors from the node's path - this prevents the tree from reloading every time a node is requested.

    /umbraco/backoffice/umbracoapi/entity/getancestors

Add a *hidden=true* value to AdditionalData on inaccessible ancestor nodes. Nodes with hidden=true are not displayed in the edit view's breadcrumbs.

    /umbraco/backoffice/umbracoapi/entity/searchall
    /umbraco/backoffice/umbracoapi/entity/search

Remove inaccessible content/media nodes from results.

    /umbraco/backoffice/umbracoapi/media/getchildren
    /umbraco/backoffice/umbracoapi/media/getchildfolders

Replace default start nodes with user's custom start nodes. Used by media pickers and the media section's ListView.

    /umbraco/backoffice/umbracoapi/content/postmove
    /umbraco/backoffice/umbracoapi/content/postcopy
    /umbraco/backoffice/umbracoapi/media/postmove

Remove inaccessible ancestors from the node's path - this prevents the tree from reloading every time a node is requested.

---

**On the front-end** a number of angular http interceptors are used to swap out Umbraco html views for custom versions with custom controllers.

    views/components/editor/umb-breadcrumbs.html

Add an additional conditional that prevents inaccessible ancestors from displaying.

    views/common/overlays/mediapicker/mediapicker.html
    views/common/dialogs/mediaPicker.html

Disable the upload button, dropzone, and add folder button if the user is in a folder they do not have access to. Use angular to watch for a folder change and enable the features if the user has access. This should only apply when pickers are not being limited to the user's start nodes.

    views/propertyeditors/listview/listview.html
    views/propertyeditors/listview/layouts/list/list.html
    views/propertyeditors/listview/layouts/grid/grid.html

Remove the move and delete functions from the user's start nodes. Disable the upload dropzone if the user does not have access to the folder.

    views/content/move.html
    views/content/copy.html
    views/media/move.html

Add a value to the umb-tree customtreeparams attribute that lets the *TreeControllerBase.TreeNodesRendering* event know to render the user's custom start nodes instead of the default.
