(function () {
    'use strict';

    function MultipleStartNodesListViewController($scope, $timeout, $http, $compile, userService, userStartNodesResource) {        

        // load umbraco's default view. Adding ?nointercept prevents my httpinterceptor from causing circular redirects
        $http.get('views/propertyeditors/listview/listview.html?nointercept').success(function (template) {
            $compile($("#multiple-start-nodes_listview").html(template).contents())($scope);

            if ($scope.$$childHead.contentId === -1) {
                userService.getCurrentUser().then(function (user) {
                    if (user.userType === 'admin' || user.startMediaId !== -1) {
                        return;
                    }

                    userStartNodesResource.getById(user.id).then(function (response) {
                        if (response.data.media === "") {
                            return;
                        }
                        $scope.$$childHead.options.allowBulkCopy = false;
                        $scope.$$childHead.options.allowBulkDelete = false;
                        $scope.$$childHead.options.allowBulkMove = false;
                    });
                });
            }     
        });

    };

    // Register the controller
    angular.module("umbraco").controller('MultipleStartNodes.ListViewController', MultipleStartNodesListViewController);

})();