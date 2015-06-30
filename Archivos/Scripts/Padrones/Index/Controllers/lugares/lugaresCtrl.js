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
            archivosResource.lugares.save(vm.lugar)
         .$promise.then(
             function (mensaje) {
                 if (!mensaje.error) {
                     vm.lugar = mensaje.objetoDto;
                     vm.mensajeDelServidor = mensaje.mensajeDelProceso;
                     $rootScope.$broadcast('actualizarListadoLugares', {});
                 } else {
                     vm.mensajeDelServidor = mensaje.mensajeDelProceso;
                 }
             },
              function (mensaje) {
                  vm.mensajeDelServidor = mensaje.data.mensajeDelProceso;
              }
          );
        }
    }
})();
