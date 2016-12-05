(function () {
    'use strict';

    function MultipleStartNodesDropZoneController($scope, $timeout, userService, userStartNodesResource) {

        var vm = this;
        vm.canUpload = true;

        function init() {           
            if ($scope.$$childHead.vm.nodeId === -1) {
                userService.getCurrentUser().then(function (user) {
                    if (user.userType === 'admin' || user.startMediaId !== -1) {
                        return;
                    }

                    userStartNodesResource.getById(user.id).then(function (response) {
                        if (response.data.media === "") {
                            return;
                        }
                        vm.canUpload = false;
                    });
                });
            }
        };

        $timeout(function () {            
            init();            
        });      
        return vm;
    };

    // Register the controller
    angular.module("umbraco").controller('MultipleStartNodes.DropZoneController', MultipleStartNodesDropZoneController);

})();