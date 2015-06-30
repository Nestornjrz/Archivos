(function () {
    'use strict';

    angular
        .module('archivosApp')
        .controller('lugaresCtrl', lugaresCtrl);

    lugaresCtrl.$inject = ['$rootScope', 'archivosResource'];

    function lugaresCtrl($rootScope, archivosResource) {
        /* jshint validthis:true */
        var vm = this;        
        vm.lugar = {};
        vm.nuevoParaCargar = function () {
            vm.lugar = {};
        };
        vm.guardar = function () {

        }
    }
})();
