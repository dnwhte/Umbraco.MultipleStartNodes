module.exports = function(grunt) {
    require('load-grunt-tasks')(grunt);
    var path = require('path');

    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        pkgMeta: grunt.file.readJSON('config/meta.json'),
        dest: grunt.option('target') || 'dist',
        basePath: path.join('<%= dest %>', 'App_Plugins', '<%= pkgMeta.directory %>'),

        clean: {
            build: '<%= grunt.config("basePath").substring(0, 4) == "dist" ? "dist/**/*" : "null" %>',
            tmp: ['tmp']
        },

        copy: {
            dll: {
                cwd: 'Src/MultipleStartNodes/bin/Release/',
                src: ['MultipleStartNodes.dll'],
                dest: '<%= dest %>/bin/',
                expand: true
            },
            debug: {
                cwd: 'Src/MultipleStartNodes/bin/Debug/',
                src: ['MultipleStartNodes.dll', '*.pdb'],
                dest: '<%= dest %>/bin/',
                expand: true
            },
            plugin: {
                cwd: 'Src/Site/App_Plugins/MultipleStartNodes/',
                src: '**/*',
                dest: '<%= basePath %>',
                expand: true
            },
            config: {
                cwd: 'Src/Site/Config/',
                src: 'MultipleStartNodes.config',
                dest: '<%= dest %>/config/',
                expand: true
            },
        },

        msbuild: {
            options: {
                stdout: true,
                verbosity: 'quiet',
                maxCpuCount: 4,
                version: 4.0,
                buildParameters: {
                    WarningLevel: 2,
                    NoWarn: 1607
                }
            },
            dist: {
                src: ['Src/MultipleStartNodes/MultipleStartNodes.csproj'],
                options: {
                    projectConfiguration: 'Release',
                    targets: ['Clean', 'Rebuild'],
                }
            },
            dev: {
                src: ['Src/MultipleStartNodes/MultipleStartNodes.csproj'],
                options: {
                    projectConfiguration: 'Debug',
                    targets: ['Clean', 'Rebuild'],
                }
            }
        },

        umbracoPackage: {
            dist: {
                src: '<%= dest %>',
                dest: 'pkg',
                options: {
                    name: "<%= pkgMeta.name %>",
                    version: '<%= pkgMeta.version %>',
                    url: '<%= pkgMeta.url %>',
                    license: '<%= pkgMeta.license %>',
                    licenseUrl: '<%= pkgMeta.licenseUrl %>',
                    author: '<%= pkgMeta.author %>',
                    authorUrl: '<%= pkgMeta.authorUrl %>',
                    readme: '<%= grunt.file.read("config/readme.txt") %>',
                    manifest: 'config/package.template.xml'
                }
            }
        }
    });

    grunt.registerTask('default', ['clean', 'msbuild:dist', 'copy:dll', 'copy:plugin', 'copy:config']);
    grunt.registerTask('package', ['clean:tmp', 'default', 'umbracoPackage', 'clean:tmp']);
};
