var multipleStartNodesUtilities = {
    modCopyMoveView: function (template, requestUrl) {
        var cacheKey = requestUrl.replace(/\W/g, '');

        if (!this[cacheKey]) {
            this[cacheKey] = template.replace(/<umb-tree\s/g, '<umb-tree customtreeparams="usestartnodes=true" ');
        }

        return this[cacheKey];
    },
    modBreadcrumbsView: function (template, requestUrl) {
        var cacheKey = requestUrl.replace(/\W/g, '');

        if (!this[cacheKey]) {
            this[cacheKey] = template.split('ng-if="!$last"').join('ng-if="!ancestor.metaData.Hidden && !$last"');
        }

        return this[cacheKey];
    },
    modListLayoutView: function (template, requestUrl) {
        var cacheKey = requestUrl.replace(/\W/g, '');

        if (!this[cacheKey]) {
            var $template = $('<div ng-controller="MultipleStartNodes.DropZoneController as dzvm">' + template + '</div>');
            var $dropzone = $template.find('umb-file-dropzone');
            var ngIfInitial = $dropzone.attr('ng-if');
            $dropzone.attr('ng-if', 'dzvm.canUpload && (' + ngIfInitial + ')');

            this[cacheKey] = $template[0].outerHTML;
        }

        return this[cacheKey];        
    },
    modListView: function (template, requestUrl) {
        var cacheKey = requestUrl.replace(/\W/g, '');

        if (!this[cacheKey]) {
            this[cacheKey] = '<div ng-controller="MultipleStartNodes.ListViewController">' + template + '</div>';
        }

        return this[cacheKey];
    },
    modMediaPickerOverlayView: function (template, requestUrl) {
        var cacheKey = requestUrl.replace(/\W/g, '');

        if (!this[cacheKey]) {
            // conditionally hide folder creation button
            template = template.replace('ng-hide="showFolderInput"', 'ng-hide="!vm.canEdit || showFolderInput"');

            // get jquery template object
            var $template = $('<div ng-controller="MultipleStartNodes.MediaPickerController as vm">' + template + '</div>');

            // conditionally disable upload button 
            var $uploadBtn = $template.find('.upload-button').find('umb-button');
            $uploadBtn[0].setAttribute('disabled', '!vm.canEdit || (' + $uploadBtn.attr('disabled') + ')');

            // conditionally disable dropzone
            var $dropzone = $template.find('umb-file-dropzone');
            $dropzone.attr('hide-dropzone', function (i, val) {
                var chars = val.split('');
                chars.splice(2, 0, '!vm.canEdit || (');
                chars.splice(chars.length - 2, 0, ')');
                return chars.join('');
            });

            this[cacheKey] = $template[0].outerHTML;
        }

        return this[cacheKey];        
    },
    modMediaPickerDialogView: function (template, requestUrl) {
        var cacheKey = requestUrl.replace(/\W/g, '');

        if (!this[cacheKey]) {
            // conditionally hide folder creation button
            template = template.replace('ng-hide="showFolderInput"', 'ng-hide="!vm.canEdit || showFolderInput"');

            // get jquery template object
            var $template = $('<div ng-controller="MultipleStartNodes.MediaPickerController as vm">' + template + '</div>');

            // conditionally disable upload button 
            var $uploadBtn = $template.find('.upload-button').find('button');
            $uploadBtn.attr('ng-disabled', '!vm.canEdit || (' + $uploadBtn.attr('ng-disabled') + ')');

            // conditionally disable dropzone
            var $dropzone = $template.find('umb-file-dropzone');
            $dropzone.attr('hide-dropzone', function (i, val) {
                var chars = val.split('');
                chars.splice(2, 0, '!vm.canEdit || (');
                chars.splice(chars.length - 2, 0, ')');
                return chars.join('');
            });

            this[cacheKey] = $template[0].outerHTML;
        }

        return this[cacheKey];        
    }
};
