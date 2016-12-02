angular.module('umbraco.services').config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push(function ($q) {
        return {
            'request': function (request) {
                var requestUrl = request.url;
                 
                if (requestUrl.indexOf("views/components/editor/umb-breadcrumbs.html") === 0) {
                    request.url = "/App_Plugins/MultipleStartNodes/umbracoviews/umb-breadcrumbs.html";
                }
                else if (requestUrl.indexOf("views/common/overlays/mediapicker/mediapicker.html") === 0) {
                    request.url = "/App_Plugins/MultipleStartNodes/umbracoviews/mediapicker.html";
                }
                else if (requestUrl.indexOf("views/propertyeditors/listview/listview.html") === 0) {
                    request.url = "/App_Plugins/MultipleStartNodes/umbracoviews/listview.html";
                }
                else if (requestUrl.indexOf("views/propertyeditors/listview/layouts/list/list.html") === 0) {
                    request.url = "/App_Plugins/MultipleStartNodes/umbracoviews/list.html";
                }
                else if (requestUrl.indexOf("views/propertyeditors/listview/layouts/grid/grid.html") === 0) {
                    request.url = "/App_Plugins/MultipleStartNodes/umbracoviews/grid.html";
                }
                else if (requestUrl.indexOf("views/content/move.html") === 0) {
                    request.url = "/App_Plugins/MultipleStartNodes/umbracoviews/contentmove.html";
                }              
                else if (requestUrl.indexOf("views/content/copy.html") === 0) {
                    request.url = "/App_Plugins/MultipleStartNodes/umbracoviews/contentcopy.html";
                }
                else if (requestUrl.indexOf("views/media/move.html") === 0) {
                    request.url = "/App_Plugins/MultipleStartNodes/umbracoviews/mediamove.html";
                }    

                return request || $q.when(request);
            }
        };
    });
}]);