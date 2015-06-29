(function () {
    'use strict';

    angular
        .module('archivosApp')
        .controller('archivosMovimientoCtrl', archivosMovimientoCtrl);

    archivosMovimientoCtrl.$inject = ['$scope', '$timeout', '$rootScope', 'archivosResource', 'Upload'];

    function archivosMovimientoCtrl($scope, $timeout, $rootScope, archivosResource, Upload) {
        /* jshint validthis:true */
        $scope.$watch('files', function () {
            $scope.upload($scope.files);
        });
        $scope.log = '';
        $scope.upload = function (files) {
            $scope.listadoMensajes = [];
            if (files && files.length) {
                for (var i = 0; i < files.length; i++) {
                    var file = files[i];
                    Upload.upload({
                        url: archivosResource.archivosMovimientosUrl,
                        fields: {
                            'titulo': $scope.archivosMovimiento.titulo,
                            'descripcion': $scope.archivosMovimiento.descripcion
                        },
                        file: file
                    }).progress(function (evt) {
                        var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                        $scope.log = 'progress: ' + progressPercentage + '% ' +
                                    evt.config.file.name + '\n' + $scope.log;

                    }).success(function (data, status, headers, config) {
                        $scope.data = data;
                        $timeout(function () {
                            //$scope.log = 'file: ' + config.file.name + ', Response: ' + JSON.stringify(data) + '\n' + $scope.log;
                            $scope.listadoMensajes.push(data);
                        });
                    });
                }

            }
            $timeout(function () {                
                $rootScope.$broadcast('actualizarListadoArchivos', {});
            });
        };
    }
})();
