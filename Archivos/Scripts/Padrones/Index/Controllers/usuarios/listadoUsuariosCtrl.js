(function () {
    'use strict';

    angular
        .module('archivosApp')
        .controller('listadoUsuariosCtrl', listadoUsuariosCtrl);

    listadoUsuariosCtrl.$inject = ['$rootScope', '$modal', 'archivosResource'];

    function listadoUsuariosCtrl($rootScope, $modal, archivosResource) {
        /* jshint validthis:true */
        var vm = this;
        vm.usuarios = archivosResource.usuarios.query();
        vm.actualizar = function (usuario) {
            $rootScope.$broadcast('actualizarUsuario', usuario);
        }
        vm.eliminar = function (usuario) {
            var modalInstance = $modal.open({
                templateUrl: 'ModalEliminacionUsuario.html',
                controller: function ($scope, $modalInstance) {
                    $scope.usuario = usuario;
                    $scope.objeto = {};
                    $scope.objeto.id = usuario.usuarioID;
                    $scope.objeto.mensaje = "Se eliminara el usuario numero ";
                    $scope.ok = function () {
                        archivosResource.usuarios.delete({ id: usuario.usuarioID },
                              function (respuesta) {
                                  $scope.respuesta = respuesta;
                                  vm.usuarios = archivosResource.usuarios.query();
                              });

                        //$rootScope.$broadcast('actualizarTodos', {});
                    };
                    $scope.cancel = function () {
                        $modalInstance.dismiss('cancel');
                    };
                },
                size: 'sm'
            });
            modalInstance.result.then(function (selectedItem) {

            }, function () {
                //$log.info('Modal dismissed at: ' + new Date());
                vm.usuarios = archivosResource.usuarios.query();
            });
        }
        //Captura de evento
        $rootScope.$on('actualizarListadoUsuarios', function (event, objValorRecibido) {
            vm.usuarios = archivosResource.usuarios.query();
        });
    }
})();
