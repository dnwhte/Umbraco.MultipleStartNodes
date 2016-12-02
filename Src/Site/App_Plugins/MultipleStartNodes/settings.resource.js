angular.module("umbraco.resources")
	.factory("settingsResource", function ($http, $cacheFactory) {    
	    return {
	        limitPickers: function () {
	            return $http.get("backoffice/MultipleStartNodes/SettingsApi/LimitPickersToStartNodes", { cache: true });
	        }
	    };
	});