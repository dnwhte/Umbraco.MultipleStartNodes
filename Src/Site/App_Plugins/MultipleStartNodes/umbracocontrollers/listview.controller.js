(function () {
    'use strict';

    function MultipleStartNodesListViewController($scope, $timeout) {

        var vm = this;       

        function init() {           
            if ($scope.$$childHead.contentId === -1) {                
                $scope.$$childHead.options.allowBulkCopy = false;
                $scope.$$childHead.options.allowBulkDelete = false;
                $scope.$$childHead.options.allowBulkMove = false;                
            }            
        };

        $timeout(function () {            
            init();            
        });      
        return vm;
    };

    // Register the controller
    angular.module("umbraco").controller('MultipleStartNodes.ListViewController', MultipleStartNodesListViewController);

})();