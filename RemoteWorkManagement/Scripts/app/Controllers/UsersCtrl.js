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
        $scope.selectedDays = [];
        $scope.selectedFlex = "";
        $scope.roles = [];
        $scope.users = [];
        //--------------------------------

        //---------------Public Functions-----------------------------
        //GET
        $scope.getRoles = function () {
            $http.get('/Home/GetAllRoles').then(function (result) {
                $scope.roles = [];
                result.data.roles.each(function (f) {
                    var rol = {};
                    rol.value = f;
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
            });
        };
        $scope.getUsers();
        
        //POST
        $scope.registerUser = function ($files) {
            userService.registerUser(
                $scope.email,
                $scope.firstName,
                $scope.lastName,
                $scope.position,
                $scope.projectLeader,
                $scope.selectedDays,
                $scope.selectedFlex).then(
                    $files.each(function (n) {
                        if (n.type == "image/png" || n.type == "image/jpg" || n.type == "image/gif" || n.type == "image/jpeg") {
                            $scope.upload = $upload.upload({
                                url: '/Home/UploadFile',
                                method: 'POST',
                                file: n
                            }).success(function (data, status, headers, config) {
                                $scope.getUsers();
                            }).error(function (data, status, headers, config) {

                            });
                        }
                    })
                );
        };

        $scope.getUser = function () {
            $http.post('/Home/GetUser', {
                params:
                    {
                        userId: $scope.selectedUser
                    }
            }).then(function (result) {

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
        //-------------------------------------------------------------

    }]);
})();