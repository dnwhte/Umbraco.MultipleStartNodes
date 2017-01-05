var multipleStartNodesUtilities = {
    forceUmbTreeToUseStartNodes: function (template) {
        return template.replace(/<umb-tree\s/g, '<umb-tree customtreeparams="usestartnodes=true" ');
    },
    hideIncccessibleBreadcrumbs: function (template) {
        return template.split('ng-if="!$last"').join('ng-if="!ancestor.metaData.Hidden && !$last"');
    },
    conditionallyHideDropzone: function (template) {
        var $template = $('<div ng-controller="MultipleStartNodes.DropZoneController as dzvm">' + template + '</div>');
        var $dropzone = $template.find('umb-file-dropzone');
        var ngIfInitial = $dropzone.attr('ng-if');
        $dropzone.attr('ng-if', 'dzvm.canUpload && (' + ngIfInitial + ')');

        return $template[0].outerHTML;
    },
    hideActionsForStartNodes: function (template) {
        return '<div ng-controller="MultipleStartNodes.ListViewController">' + template + '</div>';
    },
    conditionallyHideUploadOptions: function (template) {
        var $template = $('<div ng-controller="MultipleStartNodes.MediaPickerController as vm">' + template + '</div>');
        var $uploadBtn = $template.find('.upload-button').find('umb-button');
        //$uploadBtn.attr('disabled', '!vm.canEdit || (' + $uploadBtn.attr('disabled') + ')'); // ARG!!! JQUERY THINKS IT KNOWS EVERYTHING ABOUT THE DISABLED ATTRIBUTE
        $uploadBtn[0].outerHTML = $uploadBtn[0].outerHTML.replace(/ disabled="([^"]*)"/, ' disabled="!vm.canEdit || (' + $uploadBtn.attr('disabled') + ')"'); // hacky alternative?


        console.log($uploadBtn);
        console.log($template[0].outerHTML);

        return $template[0].outerHTML;
    }




    //// jquery way. For more complicated stuff
    //forceUmbTreeToUseStartNodes: function (template) {
    //    var $template = $(template);

    //    $template.find('umb-tree').attr('customtreeparams', 'usestartnodes=true');

    //    return $template[0].outerHTML;
    //}
};
