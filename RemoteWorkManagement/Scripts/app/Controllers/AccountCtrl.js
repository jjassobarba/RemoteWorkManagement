(function () {
    angular.module('RemoteManagement').controller('AccountCtrl', ['$scope', 'userService', '$http', function ($scope, userSerice, $http) {
        $scope.loginBox = true;
        $scope.forgotBox = false;
        $scope.errorForgot = false;


        $scope.hideMsgError = function() {
            $scope.errorForgot = false;
        };

        $scope.hideForgotBox = function () {
            $scope.loginBox = true;
            $scope.forgotBox = false;
        };

        $scope.hideLoginBox = function () {
            $scope.loginBox = false;
            $scope.forgotBox = true;
        };

        $scope.isNewPass = function () {
            $http.post('/Account/IsNewPass').then(function (result) {
                if (result.data.isTemporal) {
                    $('#modal-wizard').modal('toggle');
                }
            });
        };
        $scope.isNewPass();

        $scope.RecoverPassword = function () {
            $scope.errorForgot = false;
            $scope.emailRecover = $scope.email;
            var requestVU = $http({
                method: 'post',
                url: '/Account/ValidateUser/',
                params: {
                    mail: $scope.email
                }
            }).then(function (resultVU) {
                if (resultVU.data.result == "True") {
                    var request = $http({
                        method: 'post',
                        url: '/Account/RecoverPassword/',
                        params: {
                            mail: $scope.email
                        }
                    }).then(function (result) {
                        if (result.data.result) {
                            $scope.email = "";
                            alert("An email has been sent");
                        }

                        if (!result.data.result) {
                            alert("An Error has been occurred");
                        }
                    });
                }
                else {
                    $scope.errorForgot = true;
                }
            });




            
        };
    }]);


})();