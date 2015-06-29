(function () {
    'use strict';
    angular
        .module('archivosApp')
        .controller('listadoArchivosCtrl', listadoArchivosCtrl);

    listadoArchivosCtrl.$inject = ['$rootScope', 'archivosResource'];

    function listadoArchivosCtrl($rootScope, archivosResource) {
        var vm = this;
        //Menu
        vm.menu = {};

        vm.filtro = {};

        traerArchivosMovimientos();

        vm.listadoFn = function () {
            ocultar();
            vm.menu.listado.class = "active";
            vm.menu.listado.mostrar = true;
            traerArchivosMovimientos();
        }
        vm.filtroFormFn = function () {
            ocultar();
            vm.menu.filtroForm.class = "active";
            vm.menu.filtroForm.mostrar = true;
            vm.archivosMovimientos = {};
        }
        //////
        vm.listadoFn();
        function ocultar() {
            vm.menu.listado = {};
            vm.menu.filtroForm = {};
        }

        vm.nuevaBusqueda = function () {
            vm.filtro = {};
        }
        vm.buscar = function () {
            var titulo = "";
            var descripcion = "";

            titulo = (vm.filtro.titulo == null) ? "" : vm.filtro.titulo;
            descripcion = (vm.filtro.descripcion == null) ? "" : vm.filtro.descripcion;

            archivosResource.archivosMovimientos.query(
               {
                   "titulo": titulo,
                   "descripcion": descripcion
               },
               function (respuesta) {
                   vm.archivosMovimientos = respuesta;
               });
        }

        function traerArchivosMovimientos() {
            archivosResource.archivosMovimientos.query(
                {
                    titulo: "",
                    descripcion: ""
                },
                function (respuesta) {
                    vm.archivosMovimientos = respuesta;
                });
        }

        //Captura de eventos
        $rootScope.$on('actualizarListadoArchivos', function (event, objValorRecibido) {
            traerArchivosMovimientos();
        });
    }
})();
