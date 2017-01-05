angular.module('umbraco.services').config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push(function ($q) {
        return {
            'response': function (response) {                
                var requestUrl = response.config.url;

                if (requestUrl.indexOf("views/components/editor/umb-breadcrumbs.html") === 0) {
                    response.data = multipleStartNodesUtilities.hideIncccessibleBreadcrumbs(response.data);
                }
                else if (requestUrl.indexOf("views/common/overlays/mediapicker/mediapicker.html") === 0) {
                    response.data = multipleStartNodesUtilities.conditionallyHideUploadOptions(response.data);
                }
                else if (requestUrl.indexOf("views/propertyeditors/listview/listview.html") === 0) {
                    response.data = multipleStartNodesUtilities.hideActionsForStartNodes(response.data);
                }
                else if (requestUrl.indexOf("views/propertyeditors/listview/layouts/grid/grid.html") === 0
                    || requestUrl.indexOf("views/propertyeditors/listview/layouts/list/list.html") === 0) {
                    response.data = multipleStartNodesUtilities.conditionallyHideDropzone(response.data);
                }
                else if (requestUrl.indexOf("views/content/copy.html") === 0
                    || requestUrl.indexOf("views/content/move.html") === 0
                    || requestUrl.indexOf("views/media/move.html") === 0
                    || requestUrl.indexOf("views/common/overlays/copy/copy.html") === 0
                    || requestUrl.indexOf("views/common/overlays/move/move.html") === 0) {
                    response.data = multipleStartNodesUtilities.forceUmbTreeToUseStartNodes(response.data);
                }


                return response;
            },

            'request': function (request) {
                var requestUrl = request.url;
                 
                //if (requestUrl.indexOf("views/common/overlays/mediapicker/mediapicker.html") === 0) {
                //    request.url = "/App_Plugins/MultipleStartNodes/umbracoviews/mediapicker.html";
                //}                
                //else if (requestUrl.indexOf("views/common/dialogs/mediaPicker.html") === 0) {
                //    request.url = "/App_Plugins/MultipleStartNodes/umbracoviews/mediapickerdialog.html";
                //}  

                return request || $q.when(request);
            }
        };
    });
}]);