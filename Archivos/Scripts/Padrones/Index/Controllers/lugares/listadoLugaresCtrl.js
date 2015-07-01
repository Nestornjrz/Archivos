(function () {
    'use strict';

    angular
        .module('archivosApp')
        .controller('listadoLugaresCtrl', listadoLugaresCtrl);

    listadoLugaresCtrl.$inject = ['$rootScope', '$modal', 'archivosResource'];

    function listadoLugaresCtrl($rootScope, $modal, archivosResource) {
        /* jshint validthis:true */
        var vm = this;
        vm.lugares = archivosResource.lugares.query();
        vm.actualizar = function (lugare) {
            $rootScope.$broadcast('actualizarLugare', lugare);
        }

        vm.eliminar = function (lugare) {
            var modalInstance = $modal.open({
                templateUrl: 'ModalEliminacionLugare.html',
                controller: function ($scope, $modalInstance) {
                    $scope.lugare = lugare;
                    $scope.objeto = {};
                    $scope.objeto.id = lugare.lugarID;
                    $scope.objeto.mensaje = "Se eliminara el lugar numero ";
                    $scope.ok = function () {
                        archivosResource.lugares.delete({ id: lugare.lugarID },
                              function (respuesta) {
                                  $scope.respuesta = respuesta;
                                  vm.lugares = archivosResource.lugares.query();
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
                vm.lugares = archivosResource.lugares.query();
            });
        }

        //Captura de eventos
        $rootScope.$on('actualizarListadoLugares', function (event, objValrecibido) {
            vm.lugares = archivosResource.lugares.query();
        });
    }
})();
