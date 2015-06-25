(function () {
    'use strict';

    angular
        .module('archivosApp')
        .controller('fileUploadCtrl', cargosCtrl);

    cargosCtrl.$inject = ['$scope', '$timeout', 'archivosResource', 'Upload'];

    function cargosCtrl($scope, $timeout, archivosResource, Upload) {

        $scope.$watch('files', function () {
            $scope.upload($scope.files);
        });

        $scope.log = '';
        $scope.logProgres = '';

        $scope.upload = function (files) {
            $scope.listadoMensajes = [];
            if (files && files.length) {
                for (var i = 0; i < files.length; i++) {
                    var file = files[i];
                    Upload.upload({
                        url: archivosResource.documentosUrl,
                        fields: { 'username': $scope.username },
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
        };


    }
})();