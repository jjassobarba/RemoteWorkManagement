(function () {
    angular.module('RemoteManagement', []).controller('HomeCtrl', function ($scope) {
        $scope.name = "Jhon";

        $scope.activeMenu = function () {
            //Attach the active element in menu
            $('.nav li').removeClass('active');
            if (!$('#reportsLink').hasClass('active')) {
                $('#reportsLink').addClass('active');
            }
        };
    });
})();