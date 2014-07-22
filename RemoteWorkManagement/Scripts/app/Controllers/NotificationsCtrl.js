(function () {
    angular.module('RemoteManagement').controller('NotificationsCtrl', ['$scope', 'userService', '$http', function ($scope, userService, $http) {
        //---------------------------Variables Declaration---------------------
        $scope.users = [];
        $scope.showInfo = false;

        //---------------------------------------------------------------------

        //---------------------------Public Functions--------------------------
        //GET
        $scope.getUsers = function () {
            userService.getAllUsers().then(function (response) {
                response.users.map(function (d) {
                    var user = {};
                    user.id = d.Id;
                    user.name = d.Name;
                    $scope.users.push(user);
                });
                $scope.users = $scope.users.sortBy(function (d) {
                    return d.name;
                });
            });
        };
        $scope.getUsers();

        $scope.getUser = function () {
            userService.getUser($scope.selectedUser).then(function (response) {
                $scope.fullName = response.userInfo.FirstName + " " + response.userInfo.LastName;
                $scope.email = response.userInfo.IdMembership.Email;
                $scope.position = response.userInfo.Position;
                $scope.flexTime = response.userInfo.FlexTime;
                $scope.remoteDays = response.userInfo.RemoteDays;
                $scope.emailNotifications = response.userInfo.ReceiveNotifications;
                $scope.getNotificationForUser();
                $scope.showInfo = true;
            });
        };


        $scope.getNotificationForUser = function () {
            $http.post('/Notifications/GetNotificationForUser',
                { userId: $scope.selectedUser }).then(function (response) {
                    console.log(response.data);
                });
        };
        //---------------------------------------------------------------------

        //POST
        $scope.insertNotification = function () {
            $http.post('/Notifications/InsertNotification',
            {
                userId: $scope.selectedUser,
                projectLeaderMail: $scope.projectLeaderEmail,
                teamMail: $scope.teamEmail,
                otherEmails: ""
            }).then(function (response) {
                if (response.data.success)
                    $scope.resetForm();
            });
        };
        //----------------------------------------------------------------------

        //------------------------------Public Functions------------------------
        $scope.resetForm = function () {

            $scope.insertNotifForm.$setPristine();
        };
    }]);
})();