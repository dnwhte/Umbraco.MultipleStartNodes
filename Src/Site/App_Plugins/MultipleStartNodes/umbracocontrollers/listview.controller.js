(function () {
    'use strict';

    function MultipleStartNodesListViewController($scope, $timeout, userService, userStartNodesResource) {
        function init() {
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
        };

        $timeout(function () {
            init();
        });
    };

    // Register the controller
    angular.module("umbraco").controller('MultipleStartNodes.ListViewController', MultipleStartNodesListViewController);

})();