(function () {
    angular.module('RemoteManagement').controller('UsersCtrl', ['$scope', 'userService', '$upload', '$http', function ($scope, userService, $upload, $http) {
        //---------------------Variables Declaration------------------------
        $scope.daysOfTheWeek = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday'];
        $scope.flexTimeSchedule = ['08:00 - 17:00', '08:30 - 17:30', '09:00 - 18:00', '09:30 - 18:30', 'Other'];
        $scope.firstName = "";
        $scope.lastName = "";
        $scope.email = "";
        $scope.position = "";
        $scope.projectLeader = "";
        $scope.sensei = "";
        $scope.selectedDays = [];
        $scope.selectedFlex = "";
        $scope.selectedEditFlex = "";
        $scope.roles = [];
        $scope.users = [];
        $scope.editSelections = [];
        $scope.editRol = {};
        $scope.usersFullList = [];
        $scope.senseis = [];
        $scope.projectLeaders = [];

        $scope.showInfo = "False";

        //--------------------------------

        //---------------Public Functions-----------------------------
        //GET
        $scope.getRoles = function () {
            $http.get('/Home/GetAllRoles').then(function (result) {
                $scope.roles = [];
                result.data.roles.each(function (f) {
                    var rol = {};
                    rol.value = f;
                    rol.name = f;
                    $scope.roles.push(rol);
                });
            });
        };
        $scope.getRoles();

        $scope.getUsers = function () {
            $http.get('/Home/GetAllUsers').then(function (result) {
                $scope.users = [];
                result.data.users.each(function (d) {
                    var user = {};
                    user.id = d.Id;
                    user.name = d.Name;
                    $scope.users.push(user);
                });
                $scope.users = $scope.users.sortBy(function (n) {
                    return n.name;
                });
            });
        };
        $scope.getUsers();

        $scope.getAllUsersInfo = function () {
            userService.getAllUsersInfo().then(function (result) {
                $scope.usersFullList = [];
                $scope.usersFullList = result.usersInfo;
            });
        };
        $scope.getAllUsersInfo();

        $scope.getSenseis = function () {
            $http.post('/Home/GetSenseis').then(function (result) {
                $scope.senseis = result.data.senseis;
            });
        };
        $scope.getSenseis();

        $scope.getTeamLeaders = function () {
            $http.post('/Home/GetProjectLeaders').then(function (result) {
                $scope.projectLeaders = result.data.teamLeaders;
            });
        };
        $scope.getTeamLeaders();


        //POST .-Update
        $scope.updateUser = function () {
            $http({
                method: 'post',
                url: '/Home/UpdateUser',
                params: {
                    idUserInfo: $scope.selectedUser,
                    username: $scope.editEmail,
                    firstName: $scope.editFirstName,
                    lastName: $scope.editLastName,
                    position: $scope.editPosition,
                    rol: $scope.editRol,
                    projectLeader: $scope.editProjectLeader,
                    sensei: $scope.editSensei,
                    remoteDays: $scope.editSelections.days,
                    flexTime: $scope.selectedEditFlex
                }
            }).then(function (result) {
                if (result.data.data) {
                    $scope.showAlert("update-notification", "notice");
                    $scope.resetForm();
                    $scope.getUsers();
                } else {
                    $scope.showAlert("update-notification", "error");
                    $scope.resetForm();
                    $scope.getUsers();
                    $scope.getAllUsersInfo();
                }
            });
        };


        //POST -Register ver2
        $scope.saveUpload = function (modelInstance) {
            var files = document.getElementById('uploadImageButton').files[0];
            $scope.upload = $upload.upload({
                url: '/Home/CreateUser',
                method: 'POST',
                data: {
                    username: $scope.email,
                    firstName: $scope.firstName,
                    lastName: $scope.lastName,
                    position: $scope.position,
                    rol: $scope.selectedRol,
                    projectLeader: $scope.projectLeader,
                    sensei: $scope.sensei,
                    remoteDays: $scope.selectedDays.days,
                    flexTime: $scope.selectedFlex
                },
                file: files
            }).success(function (data, status, headers, config) {
                $scope.showAlert("notification-shape", "notice");
                $scope.resetForm();
                $scope.getUsers();
                $scope.getAllUsersInfo();
            }).error(function (data, status, headers, config) {
                $scope.showAlert("notification-shape", "error");
                $scope.resetForm();
                $scope.getUsers();
                $scope.getAllUsersInfo();
            });
        };

        $scope.getUser = function () {
            $http.post('/Home/GetUser',
                { userId: $scope.selectedUser })
                .then(function (result) {
                    var userInfoData = result.data.userInfo;
                    if (userInfoData.RemoteDays != undefined) {
                        $scope.editSelections = [];
                        var remoteDaysArray = userInfoData.RemoteDays.split(",");
                        remoteDaysArray.remove(function (d) {
                            return d == "";
                        });
                        $scope.editSelections.days = remoteDaysArray;
                    }
                    $scope.editFirstName = userInfoData.FirstName;
                    $scope.editLastName = userInfoData.LastName;
                    $scope.editEmail = userInfoData.IdMembership.Email;
                    $scope.editRol = userInfoData.Rol.RolName;
                    $scope.editPosition = userInfoData.Position;
                    $scope.editProjectLeader = userInfoData.ProjectLeader;
                    $scope.selectedEditFlex = userInfoData.FlexTime;
                    $scope.editOtherFlexTime = userInfoData.OtherFlexTime;
                    $scope.editSensei = userInfoData.IdSensei;
                    $scope.editProjectLeader = userInfoData.IdProjectLeader;
                    $scope.showInfo = "True";
                });

        };
        //DELETE
        //------------------------------------------------------------

        //--------------------Methods---------------------------------
        //Adds a the selected day to the array
        $scope.addDay = function (selectedDay, index) {
            if (selectedDay) {
                $scope.selectedDays.push($scope.daysOfTheWeek[index]);
            } else {
                selectedDay = $scope.daysOfTheWeek[index];
                $scope.selectedDays.remove(function (d) {
                    return d == selectedDay;
                });
            }
        };

        //Upload the profile picture
        $scope.onFileSelect = function ($files) {
            $files.each(function (n) {
                if (n.type == "image/png" || n.type == "image/jpg" || n.type == "image/gif" || n.type == "image/jpeg") {
                    $scope.upload = $upload.upload({
                        url: '/Home/UploadFile',
                        method: 'POST',
                        file: n
                    }).success(function (data, status, headers, config) {

                    }).error(function (data, status, headers, config) {

                    });
                }
            });
        };

        //Show the alert
        $scope.showAlert = function (elementId, typeMessage1) {
            $scope.typeMessage = typeMessage1;
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
                $scope.messageLegend = ($scope.typeMessage == "error") ? '<p><span class="icon icon-exclamation-sign big"></span>There was an error</p>' : '<p><span class="icon icon-exclamation-sign big"></span>The changes has been saved</p>';
                var notification = new NotificationFx({
                    wrapper: svgshape,
                    message: $scope.messageLegend,
                    layout: 'other',
                    effect: 'cornerexpand',
                    type: $scope.typeMessage, // notice, warning or error
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

        //Reset form
        $scope.resetForm = function () {
            $scope.firstName = "";
            $scope.lastName = "";
            $scope.email = "";
            $scope.position = "";
            $scope.projectLeader = "";
            $scope.sensei = "";
            $scope.selectedDays = [];
            $scope.selectedFlex = "";
            $scope.selectedEditFlex = "";
            $scope.users = [];
            $scope.editSelections = {};
            $scope.editRol = {};
            $scope.selectDay = "";
            $scope.selectedRol = "";
            //$scope.modelInstance.img = "";
            $scope.addUserForm.$setPristine();
        };
        //-------------------------------------------------------------

    }]);
})();