(function () {
    'use strict';

    angular
        .module('archivosApp')
        .controller('lugaresCtrl', lugaresCtrl);

    lugaresCtrl.$inject = ['$rootScope', 'archivosResource'];

    function lugaresCtrl($rootScope, archivosResource) {
        /* jshint validthis:true */
        var vm = this;        
        vm.lugare = {};
        vm.nuevoParaCargar = function () {
            vm.lugare = {};
        };
        vm.guardar = function () {
            archivosResource.lugares.save(vm.lugare)
         .$promise.then(
             function (mensaje) {
                 if (!mensaje.error) {
                     vm.lugare = mensaje.objetoDto;
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
        //Captura de evento de usuario
        $rootScope.$on('actualizarLugare', function (event, objValrecibido) {
            vm.lugare = objValrecibido;
        });
    }
})();
