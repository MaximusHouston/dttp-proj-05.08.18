/**
 * System configuration for Angular samples
 * Adjust as necessary for your application needs.
 */
(function (global) {
    System.config({
        paths: {
            // paths serve as alias
            'npm:': 'node_modules/'
        },
        // map tells the System loader where to look for things
        map: {
            // our app is within the app folder
            app: 'app',

            //Added 09/21/2017
            'tslib': 'npm:tslib',
            'pako':'npm:pako',

            // angular bundles
            '@angular/core': 'npm:@angular/core/bundles/core.umd.js',
            '@angular/common': 'npm:@angular/common/bundles/common.umd.js',
            '@angular/compiler': 'npm:@angular/compiler/bundles/compiler.umd.js',
            '@angular/platform-browser': 'npm:@angular/platform-browser/bundles/platform-browser.umd.js',
            '@angular/platform-browser-dynamic': 'npm:@angular/platform-browser-dynamic/bundles/platform-browser-dynamic.umd.js',

            //added for Angular 4
            '@angular/animations': 'node_modules/@angular/animations/bundles/animations.umd.min.js',
            '@angular/animations/browser': 'node_modules/@angular/animations/bundles/animations-browser.umd.js',
            '@angular/platform-browser/animations': 'node_modules/@angular/platform-browser/bundles/platform-browser-animations.umd.js',

            '@angular/http': 'npm:@angular/http/bundles/http.umd.js',

            //added for Angular 5
            '@angular/common/http': 'npm:@angular/common/bundles/common-http.umd.js',

            '@angular/router': 'npm:@angular/router/bundles/router.umd.js',
            '@angular/forms': 'npm:@angular/forms/bundles/forms.umd.js',

            // other libraries
            'rxjs': 'npm:rxjs',
            'angular2-in-memory-web-api': 'npm:angular2-in-memory-web-api',
            'ng-block-ui': 'npm:ng-block-ui/bundles/umd',

            'jszip': 'npm:jszip',

            //kendo UI
            '@progress/kendo-angular-buttons': 'npm:@progress/kendo-angular-buttons',
            '@progress/kendo-angular-grid': 'npm:@progress/kendo-angular-grid',
            '@progress/kendo-data-query': 'npm:@progress/kendo-data-query',

            '@progress/kendo-angular-dropdowns': 'npm:@progress/kendo-angular-dropdowns',
            '@progress/kendo-angular-popup': 'npm:@progress/kendo-angular-popup',
            '@progress/kendo-popup-common': 'npm:@progress/kendo-popup-common',
            '@progress/kendo-angular-inputs': 'npm:@progress/kendo-angular-inputs',
            '@progress/kendo-angular-layout': 'npm:@progress/kendo-angular-layout',
            '@progress/kendo-angular-dialog': 'npm:@progress/kendo-angular-dialog',

            //Added on 09/12/2017
            '@progress/kendo-angular-dateinputs': 'npm:@progress/kendo-angular-dateinputs',
            '@progress/kendo-angular-excel-export': 'npm:@progress/kendo-angular-excel-export',
            '@progress/kendo-angular-pdf-export': 'npm:@progress/kendo-angular-pdf-export',
            '@progress/kendo-drawing': 'npm:@progress/kendo-drawing',
            '@progress/kendo-file-saver': 'npm:@progress/kendo-file-saver',
            '@progress/kendo-angular-resize-sensor': 'npm:@progress/kendo-angular-resize-sensor',

            '@progress/kendo-angular-upload': 'npm:@progress/kendo-angular-upload',


            '@progress/kendo-angular-intl': 'npm:@progress/kendo-angular-intl',

            //Added on 09/12/2017
            '@progress/kendo-angular-l10n': 'npm:@progress/kendo-angular-l10n',
            '@progress/kendo-ooxml': 'npm:@progress/kendo-ooxml',
            '@progress/kendo-date-math': 'npm:@progress/kendo-date-math',

            '@telerik/kendo-dropdowns-common': 'npm:@telerik/kendo-dropdowns-common',
            '@telerik/kendo-inputs-common': 'npm:@telerik/kendo-inputs-common',
            '@telerik/kendo-draggable': 'npm:@telerik/kendo-draggable',

            '@telerik/kendo-intl': 'npm:@telerik/kendo-intl',            
            'ngx-progressbar': 'node_modules/ngx-progressbar/bundles/ngx-progressbar.umd.js'            

            //Kendo UI for Angular scopes
            //'@progress': 'npm:@progress',
            //'@telerik': 'npm:@telerik'

        },
        // packages tells the System loader how to load when no filename and/or no extension
        packages: {
            app: {
                main: './main.js',
                defaultExtension: 'js'
            },
            rxjs: {
                defaultExtension: 'js'
            },
            'angular2-in-memory-web-api': {
                main: './index.js',
                defaultExtension: 'js'
            },
            'ng-block-ui': {
                main: 'index.js',
                defaultExtension: 'js'
            },

            //Added on 09/21/2017
            'npm:pako': {
                main: '.dist/pako_deflate.js',
                defaultExtension: 'js'
            },

            'npm:tslib': {
                main: 'tslib.js',
                defaultExtension: 'js'
            },

            //Added on 09/12/2017
            'npm:jszip': {
                main: './dist/jszip.js',
                defaultExtension: 'js'
            },

            //kendo UI
            'npm:@progress/kendo-angular-buttons': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            },
            'npm:@progress/kendo-angular-grid': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            },
            'npm:@telerik/kendo-intl': {
                defaultExtension: 'js',
                main: "dist/npm/main.js"
            },
            'npm:@progress/kendo-data-query': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            },
            'npm:@progress/kendo-angular-dropdowns': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            },
            'npm:@progress/kendo-angular-popup': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            },
            'npm:@progress/kendo-popup-common': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            },
            'npm:@telerik/kendo-dropdowns-common': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            },


            'npm:@telerik/kendo-draggable': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            },

            'npm:@progress/kendo-angular-inputs': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            },

            'npm:@telerik/kendo-inputs-common': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            },

            'npm:@progress/kendo-angular-layout': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            },

            'npm:@progress/kendo-angular-dialog': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            },

            'npm:@progress/kendo-angular-intl': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            },

            //Added on 09/12/2017
            'npm:@progress/kendo-angular-l10n': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            },
            'npm:@progress/kendo-angular-dateinputs': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            },
            'npm:@progress/kendo-angular-excel-export': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            },
            'npm:@progress/kendo-angular-pdf-export': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            },
            'npm:@progress/kendo-drawing': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            },
            'npm:@progress/kendo-file-saver': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            },
            'npm:@progress/kendo-angular-resize-sensor': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            },

            'npm:@progress/kendo-angular-upload': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            },

            'npm:@progress/kendo-ooxml': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            }, 'npm:@progress/kendo-date-math': {
                main: './dist/npm/main.js',
                defaultExtension: 'js'
            }

        }
    });
})(this);
