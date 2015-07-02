(function () {
    'use strict';
    angular
        .module('archivosApp')
        .controller('listadoArchivosCtrl', listadoArchivosCtrl);

    listadoArchivosCtrl.$inject = ['$rootScope', 'archivosResource'];

    function listadoArchivosCtrl($rootScope, archivosResource) {
        var vm = this;
        vm.lugares = archivosResource.lugares.query();
        //para ver diferentes listados
        vm.verListadoDetalladoSn = false;
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
            vm.grupoArchiMovs = {};
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
            vm.grupoArchiMovs = {};
        }
        vm.buscar = function () {
            var titulo = "";
            var descripcion = "";
            var lugarID

            titulo = (vm.filtro.titulo == null) ? "" : vm.filtro.titulo;
            descripcion = (vm.filtro.descripcion == null) ? "" : vm.filtro.descripcion;
            lugarID = (vm.filtro.lugar == null) ? 0 : vm.filtro.lugar.lugarID;

            if ((vm.filtro.titulo == null || vm.filtro.titulo == "")
                && (vm.filtro.descripcion == null || vm.filtro.descripcion == "")
                && (vm.filtro.lugar == null || vm.filtro.lugar == "")
                ) {
                alert("Deve se leccionar algun criterio");
                return;
            }

            archivosResource.archivosMovimientos.query(
               {
                   "titulo": titulo,
                   "descripcion": descripcion,
                   "lugarID": lugarID
               },
               function (respuesta) {
                   vm.archivosMovimientos = respuesta;
                   crearGrupo(vm.archivosMovimientos);
               });
        }

        function traerArchivosMovimientos() {
            archivosResource.archivosMovimientos.query(
                {
                    titulo: "",
                    descripcion: "",
                    "lugarID": 0
                },
                function (respuesta) {
                    vm.archivosMovimientos = respuesta;
                    crearGrupo(vm.archivosMovimientos);
                });
        }

        function crearGrupo(archivosMovimientos) {
            var archiMovByCabID = _.groupBy(archivosMovimientos, function (valueArchMov, index, list) {
                return valueArchMov.archivosMovimientoCabID;
            });
            vm.grupoArchiMovs = _.collect(archiMovByCabID, function (archivosMovValue, archiMovIndex, archivMovList) {
                var cabecera = {};
                var archivos = [];
                _.each(archivosMovValue, function (archivoValue, archivoIndex) {
                    cabecera = {
                        'archivosMovimientoCabID': archivoValue.archivosMovimientoCabID,
                        'titulo': archivoValue.titulo
                    };
                    archivos.push(archivoValue);
                });
                return { 'cabecera': cabecera, 'archivos': archivos }
            });
        }

        //Captura de eventos
        $rootScope.$on('actualizarListadoArchivos', function (event, objValorRecibido) {
            traerArchivosMovimientos();
        });
    }
})();
