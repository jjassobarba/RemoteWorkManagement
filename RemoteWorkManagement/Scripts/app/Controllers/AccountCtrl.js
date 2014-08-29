(function () {
    angular.module('RemoteManagement').controller('AccountCtrl', ['$scope', 'userService','$http', function ($scope, userSerice, $http) {
        $scope.loginBox = true;
        $scope.forgotBox = false;

        $scope.changeBox = function () {

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
                console.log(result);
            });            
        };
    }]);


})();