angular.module("umbraco.resources")
	.factory("userStartNodesResource", function ($http, $cacheFactory) {
	    var $httpDefaultCache = $cacheFactory.get('$http');

	    return {
	        getById: function (id, bustCache) {
	            var url = "backoffice/MultipleStartNodes/UserStartNodesApi/GetById?userId=" + id;

	            if (bustCache) {
	                $httpDefaultCache.remove(url);
	            }	            

	            return $http.get(url, { cache: true });
	        },
	        save: function (user) {
	            return $http.post("backoffice/MultipleStartNodes/UserStartNodesApi/Save", angular.toJson(user));
	        }
	    };
	});