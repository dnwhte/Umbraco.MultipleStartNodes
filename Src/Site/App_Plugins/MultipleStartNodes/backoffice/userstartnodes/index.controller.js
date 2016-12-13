(function () {
    'use strict';

    function UserStartNodesIndexController($scope, $routeParams, userStartNodesResource, notificationsService, navigationService) {
        var vm = this;
        vm.title = "User Start Nodes";
        vm.loaded = true;
        //vm.user = null;        
        //vm.properties = [];

        //function init() {
        //    userStartNodesResource.getById($routeParams.id, true)
        //        .then(function (response) {
        //            vm.user = response.data;
        //            setupProperties(vm.user);
        //            vm.loaded = true;
        //            syncTree();
        //        },
        //        function (error) {
        //            vm.loaded = true;
        //            // Appears that Umbraco already handles http error notifications?
        //            //notificationsService.error("Error", error.data.Message);
        //        });
        //};        

        //function setupProperties() {
        //    vm.properties.push(
        //        {
        //            name: "content",
        //            model: {
        //                label: 'Content',                        
        //                description: '',
        //                view: 'contentpicker',
        //                config: {
        //                    multiPicker: "1",
        //                    entityType: "Document",
        //                    startNode: {
        //                        type: "content",
        //                        id: -1
        //                    },
        //                    filter: "",
        //                    minNumber: 0,
        //                    maxNumber: 0
        //                },
        //                value: vm.user.content
        //            }
        //        },
        //        {
        //            name: "media",
        //            model: {
        //                label: 'Media',                        
        //                description: '',
        //                view: 'contentpicker',
        //                config: {
        //                    multiPicker: "1",
        //                    entityType: "Media",
        //                    startNode: {
        //                        type: "media",
        //                        id: -1
        //                    },
        //                    filter: "Folder",
        //                    minNumber: 0,
        //                    maxNumber: 0
        //                },
        //                value: vm.user.media
        //            }
        //        }
        //    );            
        //};

        //function syncTree() {
        //    navigationService.syncTree({ tree: 'userStartNodes', path: ["-1", $routeParams.id], forceReload: false });
        //}

        //vm.save = function(){
        //    _.each(vm.properties, function (p) {
        //        vm.user[p.name] = p.model.value;
        //    });
            
        //    userStartNodesResource.save(vm.user)
        //        .then(function (response) {
        //            vm.user = response.data;                    
        //            $scope.userStartNodesForm.$setPristine();               

        //            notificationsService.success("Success", "User's start nodes have been saved.");
        //        },
        //        function (error) {
        //            // Appears that Umbraco already handles http error notifications?
        //            //notificationsService.error("Error", error.data.Message);
        //        });
        //};

        //init();
        return vm;
    };

    // Register the controller
    angular.module("umbraco").controller('UserStartNodes.IndexController', UserStartNodesIndexController);

})();