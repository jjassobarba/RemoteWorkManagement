(function () {
    angular.module('RemoteManagement').controller('TeamMemberCtrl', ['$scope', 'userService', '$upload', '$http', '$notification', function ($scope, userService, $upload, $http, $notification) {
        //---------------------------Variables Declaration---------------------
        $scope.users = [];
        $scope.showInfo = true;
        $scope.remoteDaysArray = [];
        $scope.idProjectLeader = "";
        $scope.idSensei = "";
        $scope.idNotification = "";
        $scope.$on('LOAD', function () { $scope.loading = true; });
        $scope.$on('UNLOAD', function() { $scope.loading = false; });
       
        //---------------------------------------------------------------------

        //---------------------------Public Functions--------------------------
        //GET
        $scope.getUser = function () {
            userService.getActualUser().then(function (response) {
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
                $scope.idProjectLeader = response.userInfo.IdProjectLeader;
                $scope.idSensei = response.userInfo.IdSensei;
            });
        };
        $scope.getUser();

        //Gets status checkIn
        $scope.getStatusCheckIn = function() {
            var request = $http({
                method: 'post',
                url: '/TeamMember/GetCheckInStatus'
            }).success(function (data, status, headers, config) {
                if (data.data.CheckOutDate == "1753") {
                    console.log(data);
                    $scope.disable('btnCheckIn');
                    $scope.enable('btnCheckOut');
                } else {
                    console.log(data);
                    $scope.disable('btnCheckOut');
                    $scope.enable('btnCheckIn');
                }
            }).error(function (data, status, headers, config) {
                console.log(data);
            });
        };
        $scope.getStatusCheckIn();


        //POST-------------------------------------------------------
        $scope.checkIn = function() {
            var request = $http({
                method: 'post',
                url: '/TeamMember/CheckIn'
            }).success(function(data, status, headers, config) {
                console.log(data);
                $scope.getStatusCheckIn();
                $notification.success('CheckIn done!', 'Now You can work remotely!');
            }).error(function(data, status, headers, config) {
                console.log(data);
                $scope.getStatusCheckIn();
                $notification.success('Error!', 'Something is wrong please try again!');
            });
        };


        $scope.checkOut = function() {
            var request = $http({
                method: 'post',
                url: '/TeamMember/CheckOut'
            }).success(function (data, status, headers, config) {
                console.log(data);
                $scope.getStatusCheckIn();
                $notification.success('CheckOut done!', ':)!');
            }).error(function (data, status, headers, config) {
                console.log(data);
                $scope.getStatusCheckIn();
                $notification.success('Error!', 'Something is wrong please try again!');
            });
        };


        //Upload Profile Picture
        $scope.updatePicture = function () {
            $scope.$emit('LOAD');
            var files = document.getElementById('uploadImageButton').files[0];
            $scope.upload = $upload.upload({
                url: '/TeamMember/UpdateProfilePicture',
                method: 'POST',
                file: files
            }).success(function (data, status, headers, config) {
                $scope.showAlert("notification-shape", "notice");
                $scope.getUser();
                $scope.$emit('UNLOAD');
            }).error(function (data, status, headers, config) {
                $scope.showAlert("notification-shape", "error");
                $scope.$emit('UNLOAD');
            });
        };


        //Removes disable attributes for a specific id
        $scope.enable = function (id) {
            console.log("habilitando");
            document.getElementById(id).removeAttribute('disabled');
        }
        //Sets disabled attribute for an specific id
        $scope.disable = function (id) {
            document.getElementById(id).setAttribute('disabled', 'disabled');
        }

        //------------------------------Public Functions------------------------
        //Show the alert
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
       
    }]);
})();