(function () {
    var scioApp = angular.module('myApp', []);
    scioApp.controller('HomeCtrl', ['$scope', function ($scope) {
        $scope.name = "Jhon";
    }]);
})();