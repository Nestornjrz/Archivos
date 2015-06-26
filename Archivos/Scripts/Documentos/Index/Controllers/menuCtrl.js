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

        vm.formularioFn = function () {
            ocultar();
            vm.menu.formulario.class = "active";
            vm.menu.formulario.mostrar = true;
        }
        vm.listadoFn = function () {
            ocultar();
            vm.menu.listado.class = "active";
            vm.menu.listado.mostrar = true;
        }
     
      
        //////
        function ocultar() {
            vm.menu.introduccion = false;
            vm.menu.formulario = {};
            vm.menu.listado = {};
        }
    }
})();
