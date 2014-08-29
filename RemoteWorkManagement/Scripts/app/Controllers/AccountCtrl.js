(function () {
    angular.module('RemoteManagement').controller('AccountCtrl', ['$scope', 'userService', '$http', function ($scope, userService, $http) {
        $scope.isNewPass = function () {
            $http.post('/Account/IsNewPass').then(function (result) {
                console.log(result);
            });
        };
        $scope.isNewPass();
    }]);
})();