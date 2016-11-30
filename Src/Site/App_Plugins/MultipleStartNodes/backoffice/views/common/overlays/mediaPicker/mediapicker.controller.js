(function () {
    'use strict';

    function MultipleStartNodesMediaPickerController($scope, $timeout, userService, userStartNodesResource) {

        // private variables
        var userId = null,
            startNodes = null;

        // setup view variables
        var vm = this;
        vm.canEdit = false;        

        function init() {
            // get current user
            userService.getCurrentUser().then(function (user) {
                if (user.userType === 'admin') {
                    vm.canEdit = true;
                    return;
                }

                userId = user.id;

                // get start nodes for current user
                userStartNodesResource.getById(userId).then(function (response) {                    
                    if (response.data.media === "")
                        return;

                    startNodes = response.data.media.split(',');

                    if (_.contains(startNodes, "-1")) {
                        vm.canEdit = true;
                        return;
                    }

                    watchForFolderChange();
                });
            });
        };

        function watchForFolderChange() {
            $scope.$watch(function (scope) { return scope.$$childHead.path },
                function (currentPath) {
                    if (!currentPath || currentPath.length === 0) {
                        vm.canEdit = false;
                        return;
                    }

                    determineFolderAccess(currentPath[currentPath.length -1].path);
                }
            );
        };

        function determineFolderAccess(currentPath) {
            var currentPathArray = currentPath.split(',');
            if (_.intersection(startNodes, currentPathArray).length > 0) {
                vm.canEdit = true;
            } else {
                vm.canEdit = false;
            }
        };

        $timeout(function () {            
            init();            
        });      
        return vm;
    };

    // Register the controller
    angular.module("umbraco").controller('MultipleStartNodes.MediaPickerController', MultipleStartNodesMediaPickerController);

})();