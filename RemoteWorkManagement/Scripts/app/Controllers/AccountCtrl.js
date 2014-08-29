﻿(function () {
    angular.module('RemoteManagement').controller('AccountCtrl', ['$scope', 'userService', '$http', function ($scope, userService, $http) {
        $scope.isNewPass = function () {
            $http.post('/Account/IsNewPass').then(function (result) {
                console.log(result);
            });
        };
        $scope.isNewPass();
    angular.module('RemoteManagement').controller('AccountCtrl', ['$scope', 'userService','$http', function ($scope, userSerice, $http) {
        
        $scope.RecoverPassword = function () {
            $scope.emailRecover = $scope.email;
            alert($scope.emailRecover);
            var request = $http({
                method: 'post',
                url: '/Account/RecoverPassword/',
                params: {
                    mail: $scope.email
                }
            }).then(function (result) {
                console.log(result);
            });            
        };
    }]);
})();