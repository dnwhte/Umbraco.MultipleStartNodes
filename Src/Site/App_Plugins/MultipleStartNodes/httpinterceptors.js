angular.module('umbraco.services').config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push(function ($q) {
        return {
            'response': function (response) {                
                var requestUrl = response.config.url.split(/[?#]/)[0];

                if (requestUrl === "views/components/editor/umb-breadcrumbs.html") {
                    response.data = multipleStartNodesUtilities.modBreadcrumbsView(response.data, requestUrl);
                }
                else if (requestUrl === "views/common/overlays/mediapicker/mediapicker.html") {
                    response.data = multipleStartNodesUtilities.modMediaPickerOverlayView(response.data, requestUrl);
                }
                else if (requestUrl === "views/common/dialogs/mediaPicker.html") {
                    response.data = multipleStartNodesUtilities.modMediaPickerDialogView(response.data, requestUrl);
                }
                else if (requestUrl === "views/propertyeditors/listview/listview.html") {
                    response.data = multipleStartNodesUtilities.modListView(response.data, requestUrl);
                }
                else if (requestUrl === "views/propertyeditors/listview/layouts/grid/grid.html"
                    || requestUrl === "views/propertyeditors/listview/layouts/list/list.html") {
                    response.data = multipleStartNodesUtilities.modListLayoutView(response.data, requestUrl);
                }
                else if (requestUrl === "views/content/copy.html"
                    || requestUrl === "views/content/move.html"
                    || requestUrl === "views/media/move.html"
                    || requestUrl === "views/common/overlays/copy/copy.html"
                    || requestUrl === "views/common/overlays/move/move.html") {
                    response.data = multipleStartNodesUtilities.modCopyMoveView(response.data, requestUrl);
                }


                return response;
            }
        };
    });
}]);