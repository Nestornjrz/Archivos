(function () {
    'use strict';

    angular
        .module('archivosApp')
        .controller('menuCtrl', menuCtrl);

    menuCtrl.$inject = ['$rootScope'];

    function menuCtrl($rootScope) {
        var vm = this;
        vm.menu = {};
        vm.menu.introduccion = true;

        vm.usuariosFn = function () {
            ocultar();
            vm.menu.usuarios.class = "active";
            vm.menu.usuarios.mostrar = true;
        }
        vm.lugaresFn = function () {
            ocultar();
            vm.menu.lugares.class = "active";
            vm.menu.lugares.mostrar = true;
        }
      
        //////
        function ocultar() {
            vm.menu.introduccion = false;
            vm.menu.usuarios = {};
            vm.menu.lugares = {};
        }
    }
})();
