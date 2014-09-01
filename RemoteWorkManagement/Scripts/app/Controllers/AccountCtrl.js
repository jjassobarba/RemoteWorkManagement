(function () {
    angular.module('RemoteManagement').controller('AccountCtrl', ['$scope', 'userService', '$http', function ($scope, userSerice, $http) {
        $scope.loginBox = true;
        $scope.forgotBox = false;

        $scope.hideForgotBox = function () {
            $scope.loginBox = true;
            $scope.forgotBox = false;
        };

        $scope.hideLoginBox = function () {
            $scope.loginBox = false;
            $scope.forgotBox = true;
        };

        //Check if the user is new in the system
        $scope.isNewPass = function () {
            $http.post('/Account/IsNewPass').then(function (result) {
                if (result.data.isTemporal) {
                    $('#modal-wizard').modal('toggle');
                }
            });
        };
        $scope.isNewPass();

        $scope.checkStep = function(obj, $event) {
            console.log($event.target);
        };

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
                if (result.data.result) {
                    alert("An email has been sent");
                }
                else {
                    alert("An Error has been occurred");
                }

            });
        };
    }]);


})();