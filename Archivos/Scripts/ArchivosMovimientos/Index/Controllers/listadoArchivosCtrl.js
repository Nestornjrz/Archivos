(function () {
    'use strict';
    angular
        .module('archivosApp')
        .controller('listadoArchivosCtrl', listadoArchivosCtrl);

    listadoArchivosCtrl.$inject = ['$rootScope', 'archivosResource'];

    function listadoArchivosCtrl($rootScope, archivosResource) {
        var vm = this;
        vm.archivosMovimientos = archivosResource.archivosMovimientos.query();
    }
})();
