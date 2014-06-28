(function () {
    angular.module('RemoteManagement').controller('UsersCtrl', function ($scope, userService) {
        //---------------------Variables Declaration------------------------
        $scope.firstName = "";
        $scope.lastName = "";
        $scope.email = "";
        $scope.position = "";
        $scope.projectLeader = "";
        //--------------------------------

        //---------------Public Functions-----------------------------
        //GET

        //POST
        $scope.registerUser = function () {
            userService.registerUser($scope.email, $scope.password).then(console.log("Ok"));
        };

        //DELETE
        //-----------------

    });
})();