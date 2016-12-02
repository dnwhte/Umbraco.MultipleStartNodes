(function () {
    'use strict';

    function MultipleStartNodesDropZoneController($scope, $timeout) {

        var vm = this;
        vm.canUpload = true;

        function init() {           
            if ($scope.$$childHead.vm.nodeId === -1) {
                vm.canUpload = false;
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