(function () {
    angular.module('RemoteManagement').controller('TeamMemberCtrl', ['$scope', 'userService', '$upload', '$http', '$notification', '$q', '$timeout', function ($scope, userService, $upload, $http, $notification, $q, $timeout) {
        //---------------------------Variables Declaration---------------------
        $scope.users = [];
        $scope.showInfo = true;
        $scope.vallue = 40;
        $scope.remoteDaysArray = [];
        $scope.idProjectLeader = "";
        $scope.idSensei = "";
        $scope.idNotification = "";
        $scope.isAllowedDay = false;
        $scope.checkIncomment = "";
        $scope.remainingUsers = [];
        $scope.readyUsers = [];
        $scope.unAuthorizedUsersCheckedIn = [];
        $scope.usersAssignedTo = [];
        $scope.countCheckInOutUsers = 0;
        $scope.countNotLoggedUsers = 0;
        $scope.countLoggedUsers = 0;
        $scope.roundProgressData = 0;
        $scope.roundProgressData2 = 0;
        $scope.roundProgressUnauthorizedUser = [];
        $scope.color = "red";
        $scope.$on('LOAD', function () { $scope.loading = true; });
        $scope.$on('UNLOAD', function () { $scope.loading = false; });
        //$scope.isCheckInDisable = false;
        //$scope.isCheckOutDisable = false;
        //$scope.isCheckInManualDisable = true;
        //$scope.isCheckOutManualDisable = true;
        //$scope.isCheckboxInDisable = true;

        //---------------------------------------------------------------------

        // Here I synchronize the value of label and percentage in order to have a chart
        $scope.$watch('roundProgressData', function (newValue) {
            newValue.percentage = newValue.label / 100;
        }, true);

        $scope.$watch('roundProgressUnauthorizedUser', function (newValue) {
            newValue.percentage = newValue.label / 100;
        }, true);

        $scope.$watch('roundProgressData2', function (newValue) {
            newValue.percentage = newValue.label / 100;
        }, true);


        //this block of code is just for refresh the charts. 
        $timeout(function () {
            $scope.automatic();
        }, 10000);

        $scope.automatic = function () {
            $scope.chartLoader();
            $timeout(function () {
                $scope.auto();
            }, 10000);
        };

        $scope.auto = function () {
            $scope.automatic();
        };
        // end block..


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
        //$scope.getStatusCheckIn = function () {
        //    $http({
        //        method: 'post',
        //        url: '/TeamMember/GetCheckInStatus'
        //    }).success(function (data) {
        //        if (data.data.isEnablecheckOut) {
        //            $scope.isCheckboxInDisable = true;
        //            $scope.isCheckInDisable = true;
        //            $scope.isCheckboxOutDisable = false;
        //            $scope.isCheckOutDisable = false;
        //        } else {
        //            $scope.isCheckOutDisable = true;
        //            $scope.isCheckboxOutDisable = true;
        //            $scope.isCheckboxInDisable = false;
        //            $scope.isCheckInDisable = false;
        //        }
        //    }).error(function () {

        //    });
        //};
        //$scope.getStatusCheckIn();

        //Verifies if the user is allowed to work remotely today
        $scope.getStatusDay = function () {
            userService.isAllowedDay().then(function (response) {
                $scope.isAllowedDay = response.success;
            });
        };
        $scope.getStatusDay();


        $scope.chartLoader = function () {
            userService.getRemainingUsers().then(function (response) {
                $scope.remainingUsers = response.data;
                userService.getNotAllowedCheckInUsers().then(function (data2) {
                    $scope.unAuthorizedUsersCheckedIn = data2.data;

                    $scope.roundProgressUnauthorizedUser = {
                        label: $scope.unAuthorizedUsersCheckedIn.length,
                        percentage: $scope.unAuthorizedUsersCheckedIn.length,
                        marks: '%'
                    };
                    userService.getReadyUsers().then(function (data) {
                        $scope.readyUsers = data.data;
                        $scope.countCheckInOutUsers = (100 / ($scope.readyUsers.length + $scope.remainingUsers.length));
                        $scope.countNotLoggedUsers = ($scope.remainingUsers.length * $scope.countCheckInOutUsers).toFixed(1);
                        $scope.countLoggedUsers = ($scope.readyUsers.length * $scope.countCheckInOutUsers).toFixed(1);
                        $scope.roundProgressData = {
                            label: $scope.countNotLoggedUsers,
                            percentage: $scope.countNotLoggedUsers,
                            marks: '%'
                        };
                        $scope.roundProgressData2 = {
                            label: $scope.countLoggedUsers,
                            percentage: $scope.countLoggedUsers,
                            marks: '%'
                        };
                        $('.tip').tooltip();
                    });

                });
            });
        };
        $scope.chartLoader();

        //$scope.changeInStatus = function () {
        //    if ($scope.isManualCheckin)
        //        $scope.isCheckInDisable = true;
        //    else
        //        $scope.isCheckInDisable = false;
        //};
        //$scope.changeOutStatus = function () {
        //    if ($scope.isManualCheckout == true)
        //        $scope.isCheckOutDisable = true;
        //    else
        //        $scope.isCheckOutDisable = false;
        //};

        $scope.getAllUsersbyProyectLeader = function () {
            userService.getAllUsersbyProyectLeader().then(function (response) {
                $scope.usersAssignedTo = response.data;
            });
        };

        $scope.getAllUsersbyProyectLeader();

        //POST-------------------------------------------------------

        $scope.checkIn = function () {
            $scope.$emit('LOAD');
            if ($scope.isAllowedDay) {
                $http({
                    method: 'post',
                    url: '/TeamMember/CheckIn',
                    params: {
                        comment: "",
                        type: "Automatic"
                    }
                }).success(function (data) {
                    //$scope.getStatusCheckIn();
                    if (data.data.success) {
                        if (data.data.isMailSent) {
                            $notification.success('CheckIn done!', 'You can start being productive!');
                        } else {
                            $notification.warning('CheckIn done!', 'Your email has not been sent.');
                        }
                    } else {
                        $notification.error('Error!', 'You cant CheckIn without CheckOut! or two times per day');
                    }
                    $scope.$emit('UNLOAD');
                }).error(function () {
                    $scope.getStatusCheckIn();
                    $notification.error('Whoa! Something seems to be wrong.', 'please try again!');
                    $scope.$emit('UNLOAD');
                });
            } else {
                if ($scope.checkIncomment != "") {
                    $http({
                        method: 'post',
                        url: '/TeamMember/CheckIn',
                        params: {
                            comment: $scope.checkIncomment,
                            type: "Exception"
                        }
                    }).success(function (data) {
                        $scope.getStatusCheckIn();
                        if (data.data.success) {
                            if (data.data.isMailSent) {
                                $notification.success('CheckIn done!', 'You can start being productive!');
                            } else {
                                $notification.warning('CheckIn done!', 'Your email has not been sent.');
                            }
                        } else {
                            $notification.error('Error!', 'You cant CheckIn without CheckOut! or two times per day');
                        }
                        $scope.$emit('UNLOAD');
                    }).error(function () {
                        $scope.getStatusCheckIn();
                        $notification.error('Whoa! Something seems to be wrong.', 'please try again!');
                        $scope.$emit('UNLOAD');
                    });
                } else {
                    $scope.$emit('UNLOAD');
                    $("#warning-dialog").removeClass('hide').dialog({
                        modal: true,
                        title: "<div class='widget-header widget-header-small'><h4 class='small'><i class='icon-warning-sign'></i>Warning</h4></div>",
                        title_html: true,
                        width: 350,
                        buttons: [
                            {
                                text: "Cancel",
                                "class": "btn btn-xs",
                                click: function () {
                                    $(this).dialog("close");
                                }
                            },
                            {
                                text: "OK",
                                "class": "btn btn-primary btn-xs",
                                click: function () {
                                    $(this).dialog("close");
                                }
                            }
                        ]
                    });
                }
            }
        };

        $scope.checkInM = function () {
            $scope.$emit('LOAD');
            if ($scope.isAllowedDay) {
                $http({
                    method: 'post',
                    url: '/TeamMember/CheckInM',
                    params: {
                        comment: "",
                        type: "Manual",
                        time: $scope.checkInTime
                    }
                }).success(function (data) {
                    //$scope.getStatusCheckIn();
                    if (data.data.success) {
                        if (data.data.isMailSent) {
                            $notification.success('CheckIn done!', 'You can start being productive!');
                        } else {
                            $notification.warning('CheckIn done!', 'Your email has not been sent.');
                        }
                    } else {
                        $notification.error('Error!', 'You cant CheckIn without CheckOut! or two times per day');
                    }
                    $scope.$emit('UNLOAD');
                }).error(function () {
                    $scope.getStatusCheckIn();
                    $notification.error('Whoa! Something seems wrong.', 'please try again!');
                    $scope.$emit('UNLOAD');
                });
            } else {
                if ($scope.checkIncomment != "") {
                    $http({
                        method: 'post',
                        url: '/TeamMember/CheckInM',
                        params: {
                            comment: $scope.checkIncomment,
                            type: "Exception",
                            time: $scope.checkInTime
                        }
                    }).success(function (data) {
                        $scope.getStatusCheckIn();
                        if (data.data.success) {
                            if (data.data.isMailSent) {
                                $notification.success('CheckIn done!', 'Now You can work remotely!');
                            } else {
                                $notification.warning('CheckIn done!', 'Your email has not been sent.');
                            }
                        } else {
                            $notification.error('Error!', 'You cant CheckIn without CheckOut! or two times per day');
                        }
                        $scope.$emit('UNLOAD');
                    }).error(function () {
                        $scope.getStatusCheckIn();
                        $notification.error('Whoa! Something seems wrong.', 'please try again!');
                        $scope.$emit('UNLOAD');
                    });
                } else {
                    $scope.$emit('UNLOAD');
                    $("#warning-dialog").removeClass('hide').dialog({
                        modal: true,
                        title: "<div class='widget-header widget-header-small'><h4 class='small'><i class='icon-warning-sign'></i>Warning</h4></div>",
                        title_html: true,
                        width: 350,
                        buttons: [
                            {
                                text: "Cancel",
                                "class": "btn btn-xs",
                                click: function () {
                                    $(this).dialog("close");
                                }
                            },
                            {
                                text: "OK",
                                "class": "btn btn-primary btn-xs",
                                click: function () {
                                    $(this).dialog("close");
                                }
                            }
                        ]
                    });
                }
            }
        };

        $scope.checkOut = function () {
            $scope.$emit('LOAD');
            $http({
                method: 'post',
                url: '/TeamMember/CheckOut'
            }).success(function (data) {
                if (data.data.success) {
                    if (data.data.isMailSent) {
                        $notification.success('Check-out done!', 'Good job today!');
                    } else {
                        $notification.warning('Check-out done!', 'Your email has not been sent.');
                    }
                } else {
                    $notification.error('Error!', 'You can not Check-out without Check-in');
                }
                $scope.$emit('UNLOAD');
            }).error(function () {
                $scope.getStatusCheckIn();
                $notification.error('Whoa! Something seems to be wrong.', 'please try again!');
                $scope.$emit('UNLOAD');
            });
        };

        $scope.checkOutM = function () {
            $scope.$emit('LOAD');
            $http({
                method: 'post',
                url: '/TeamMember/CheckOutM',
                params: {
                    time: $scope.checkOutTime
                }
            }).success(function (data) {
                //$scope.getStatusCheckIn();
                if (data.data.success) {
                    if (data.data.isMailSent) {
                        $notification.success('CheckOut done!', 'Now you can be offline!');
                    } else {
                        $notification.warning('CheckOut done!', 'Your email has not been sent.');
                    }
                } else {
                    $notification.error('Error!', 'You cant CheckOut without CheckIn');
                }
                $scope.$emit('UNLOAD');
            }).error(function () {
                $scope.getStatusCheckIn();
                $notification.error('Whoa! Something seems wrong.', 'please try again!');
                $scope.$emit('UNLOAD');
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
            }).success(function () {
                $notification.success('Profile picture has been updated.', '');
                $scope.getUser();
                $scope.$emit('UNLOAD');
            }).error(function () {
                $notification.error('Whoa! Something seems wrong.', '');
                $scope.$emit('UNLOAD');
            });
        };
    }]);
})();