angular.module("umbraco.resources")
	.factory("userStartNodesResource", function ($http) {
	    return {
	        getById: function (id) {
	            return $http.get("backoffice/MultipleStartNodes/UserStartNodesApi/GetById?userId=" + id);
	        },
	        save: function (user) {
	            return $http.post("backoffice/MultipleStartNodes/UserStartNodesApi/Save", angular.toJson(user));
	        }
	    };
	});