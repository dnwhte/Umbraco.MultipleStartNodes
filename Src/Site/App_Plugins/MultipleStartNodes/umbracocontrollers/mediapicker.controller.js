(function () {
    'use strict';

    function MultipleStartNodesMediaPickerController($scope, $timeout, userService, userStartNodesResource) {
        
        var startNodes = null;

        // setup view variables
        var vm = this;
        vm.canEdit = false;        

        function init() {
            // get current user
            userService.getCurrentUser().then(function (user) {
                // if admin let them edit anything
                // if they're using a traditional umbraco start node, let me edit whatever they have access to
                if (user.userType === 'admin' || user.startMediaId !== -1) {
                    vm.canEdit = true;
                    return;
                }                

                // get start nodes for current user
                userStartNodesResource.getById(user.id).then(function (response) {
                    // no start nodes? Return with umbraco default permissions
                    if (response.data.media === "") {
                        vm.canEdit = true;
                        return;
                    }

                    startNodes = response.data.media.split(',');                    

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