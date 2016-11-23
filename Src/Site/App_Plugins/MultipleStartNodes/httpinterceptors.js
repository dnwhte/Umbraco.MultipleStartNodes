angular.module('umbraco.services').config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push(function ($q) {
        return {
            'request': function (request) {
                var requestUrl = request.url;

                if (requestUrl.indexOf("views/content/move.html") === 0) {
                    request.url = "/App_Plugins/MultipleStartNodes/backoffice/views/content/move.html";
                }

                if (requestUrl.indexOf("views/content/copy.html") === 0) {
                    request.url = "/App_Plugins/MultipleStartNodes/backoffice/views/content/copy.html";
                }

                if (requestUrl.indexOf("views/media/move.html") === 0) {
                    request.url = "/App_Plugins/MultipleStartNodes/backoffice/views/media/move.html";
                }

                return request || $q.when(request);
            }
        };
    });
}]);