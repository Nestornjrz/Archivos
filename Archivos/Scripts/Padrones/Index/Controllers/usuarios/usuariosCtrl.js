(function () {
    'use strict';

    angular
        .module('archivosApp')
        .controller('usuariosCtrl', usuariosCtrl);

    usuariosCtrl.$inject = ['$rootScope', 'archivosResource'];

    function usuariosCtrl($rootScope, archivosResource) {
        /* jshint validthis:true */
        var vm = this;
        vm.usuario = {};
        vm.nuevoParaCargar = function () {
            vm.usuario = {};
        };
        vm.guardar = function () {
            archivosResource.usuarios.save(vm.usuario)
          .$promise.then(
              function (mensaje) {
                  if (!mensaje.error) {
                      vm.usuario = mensaje.objetoDto;
                      vm.mensajeDelServidor = mensaje.mensajeDelProceso;
                      $rootScope.$broadcast('actualizarListadoUsuarios',{});
                  } else {
                      vm.mensajeDelServidor = mensaje.mensajeDelProceso;
                  }
              },
               function (mensaje) {
                   vm.mensajeDelServidor = mensaje.data.mensajeDelProceso;
               }
           );
        }
        //Evento
        $rootScope.$on('actualizarUsuario', function (event, objValorRecibido) {
            vm.usuario = objValorRecibido;
        })
    }
})();
