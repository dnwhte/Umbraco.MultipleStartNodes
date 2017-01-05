var multipleStartNodesUtilities = {
    forceUmbTreeToUseStartNodes: function (template) {
        template = template.replace(/<umb-tree\s/g, '<umb-tree customtreeparams="usestartnodes=true" ');

        return template;
    }

    //// jquery way. For more complicated stuff
    //forceUmbTreeToUseStartNodes: function (template) {
    //    var $template = $(template);

    //    $template.find('umb-tree').attr('customtreeparams', 'usestartnodes=true');

    //    return $template[0].outerHTML;
    //}
};
