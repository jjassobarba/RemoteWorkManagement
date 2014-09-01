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

        $scope.isNewPass = function () {
            $http.post('/Account/IsNewPass').then(function (result) {
                console.log(result);
            });
        };
        $scope.isNewPass();

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
                else
                {
                    alert("An Error has been occurred");
                }
                
            });
        };
    }]);


})();