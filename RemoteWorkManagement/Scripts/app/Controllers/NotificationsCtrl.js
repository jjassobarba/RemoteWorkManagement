(function () {
    angular.module('RemoteManagement').controller('NotificationsCtrl', ['$scope', 'userService', '$http', function ($scope, userService, $http) {
        //---------------------------Variables Declaration---------------------
        $scope.users = [];
        $scope.showInfo = false;
        $scope.remoteDaysArray = [];

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
                $scope.picture = response.userInfo.Picture;
                if ($scope.remoteDays != undefined) {
                    $scope.editSelections = [];
                    var remoteDaysArray = $scope.remoteDays.split(",");
                    remoteDaysArray.remove(function (d) {
                        return d == "";
                    });
                    $scope.remoteDaysArray = remoteDaysArray;
                }
                $scope.emailNotifications = response.userInfo.ReceiveNotifications;
                $scope.getNotificationForUser();
                $scope.showInfo = true;
            });
        };


        $scope.getNotificationForUser = function () {
            $http.post('/Notifications/GetNotificationForUser',
                { userId: $scope.selectedUser }).then(function (response) {
                    $scope.projectLeaderEmail = "";
                    $scope.teamEmail = "";
                    $scope.selectedValue = false;
                    $scope.selectedTeamValue = false;
                    if (response.data.notifications.length > 0) {
                        var userNotification = response.data.notifications[0];
                        $scope.projectLeaderEmail = userNotification.ProjectLeader;
                        $scope.teamEmail = userNotification.TeamLeader;
                        $scope.selectedValue = true;
                        $scope.selectedTeamValue = true;
                    }
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
                    $scope.showAlert("notification-shape");
                    $scope.resetForm();
            });
        };
        //----------------------------------------------------------------------

        //------------------------------Public Functions------------------------
        $scope.showAlert = function (elementId) {
            var svgshape = document.getElementById(elementId),
                s = Snap(svgshape.querySelector('svg')),
                path = s.select('path'),
                pathConfig = {
                    from: path.attr('d'),
                    to: svgshape.getAttribute('data-path-to')
                };

            window.setTimeout(function () {

                path.animate({ 'path': pathConfig.to }, 300, mina.easeinout);

                // create the notification
                var notification = new NotificationFx({
                    wrapper: svgshape,
                    message: '<p><span class="icon icon-exclamation-sign big"></span>The changes has been saved</p>',
                    layout: 'other',
                    effect: 'cornerexpand',
                    type: 'notice', // notice, warning or error
                    onClose: function () {
                        setTimeout(function () {
                            path.animate({ 'path': pathConfig.from }, 300, mina.easeinout);
                        }, 200);
                    }
                });

                // show the notification
                notification.show();

            }, 500);
        };

        $scope.resetForm = function () {
            $scope.insertNotifForm.$setPristine();
        };
    }]);
})();