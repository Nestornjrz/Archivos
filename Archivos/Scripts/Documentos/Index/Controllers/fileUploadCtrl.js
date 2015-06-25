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
            $scope.dynamic = 0;
            if (files && files.length) {
                for (var i = 0; i < files.length; i++) {
                    var file = files[i];
                    Upload.upload({
                        url: archivosResource.documentosUrl,
                        fields: { 'username': $scope.username },
                        file: file
                    }).progress(function (evt) {
                        var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                        console.log('progress: ' + progressPercentage + '% ' +
                                    evt.config.file.name + '\n' + $scope.log);
                        $scope.logProgres = 'progress: ' + progressPercentage + '% ' +
                                    evt.config.file.name + '\n' + $scope.log;

                        var type;
                        var value = progressPercentage;

                        if (value < 25) {
                            type = 'success';
                        } else if (value < 50) {
                            type = 'info';
                        } else if (value < 75) {
                            type = 'warning';
                        } else {
                            type = 'danger';
                        }

                        $scope.showWarning = (type === 'danger' || type === 'warning');

                        $scope.dynamic = value;
                        $scope.type = type;

                    }).success(function (data, status, headers, config) {
                        $scope.data = data;
                        $timeout(function () {
                            $scope.log = 'file: ' + config.file.name + ', Response: ' + JSON.stringify(data) + '\n' + $scope.log;
                            $scope.mensajeDelServidor = data.MensajeDelProceso;
                        });
                    });
                }
            }
        };


    }
})();