(function () {
    angular.module('RemoteManagement').controller('AccountCtrl', ['$scope', 'userService', '$http', function ($scope, userSerice, $http) {
        $scope.loginBox = true;
        $scope.forgotBox = false;
        $scope.errorForgot = false;
        $scope.isDisabledNext = true;
        $scope.msgAlert = "";
        $scope.classMsgAlert = "";
        $scope.stepIndex = 1;
        $scope.passMatchMsg = "";

        $scope.hideMsgError = function() {
            $scope.msgAlert = "";
            $scope.classMsgAlert = "";
        };

        $scope.hideForgotBox = function () {
            $scope.loginBox = true;
            $scope.forgotBox = false;
            $scope.msgAlert = "";
            $scope.email = "";
            $scope.classMsgAlert = "";
        };

        $scope.hideLoginBox = function () {
            $scope.loginBox = false;
            $scope.forgotBox = true;
        };

        $scope.isNewPass = function () {
            $http.post('/Account/IsNewPass').then(function (result) {
                if (result.data.isTemporal) {
                    $('#modal-wizard').modal('toggle');
                }
            });
        };
        $scope.isNewPass();

        $scope.RecoverPassword = function () {
            $scope.errorForgot = false;
            $scope.emailRecover = $scope.email;
            var requestVU = $http({
                method: 'post',
                url: '/Account/ValidateUser/',
                params: {
                    mail: $scope.email
                }
            }).then(function (resultVU) {
                if (resultVU.data.result == "True") {
                    var request = $http({
                        method: 'post',
                        url: '/Account/RecoverPassword/',
                        params: {
                            mail: $scope.email
                        }
                    }).then(function (result) {
                        if (result.data.result) {
                            $scope.email = "";
                            $scope.classMsgAlert = "alert-success";
                            $scope.msgAlert = "An email has been sent";
                        }

                        if (!result.data.result) {
                            $scope.classMsgAlert = "alert-error";
                            $scope.msgAlert = "An Error has been occurred";
                        }
                    });
                }
                else {
                    $scope.classMsgAlert = "alert-error";
                    $scope.msgAlert = " Your email is not registered";
                }
            });
            
        };        

        $scope.nextStep = function () {   
            console.log("forward " + $scope.stepIndex);
            switch($scope.stepIndex) {
                case 1:
                    console.log("Type Old Password");
                    var request = $http({
                        method: 'post',
                        url: '/Account/ValidateOldPassword',
                        params: {
                            password: $scope.oldPassword
                        }
                    }).then(function (result) {
                        if (result.data.result) {
                            console.log("OldPassValid");
                        } else {
                            console.log("OldPassNotValid");
                            $("#prevButton").click();
                        }
                    });
                    break;
                case 2:
                    console.log("Type New Password");
                    break;
                case 3:
                    console.log("Repeat Password");
                    break;
                case 4:
                    console.log("Make Changes");
                    break;                
            }
            $scope.stepIndex = $scope.stepIndex + 1;
        };

        $scope.prevStep = function () {            
            $scope.stepIndex = $scope.stepIndex - 1;
            console.log("back " + $scope.stepIndex);
            switch ($scope.stepIndex) {
                case 1:
                    console.log("Type Old Password");      
                    break;
                case 2:
                    console.log("Type New Password");
                    break;
                case 3:
                    console.log("Repeat Password");
                    break;
                case 4:
                    console.log("Make Changes");
                    break;
            }
        };

        $scope.validateNewPwd = function () {
            
        };
                
        $scope.validateMatchPwd = function () {
            if($scope.newPassword != $scope.repeatPassword)
            {
                $scope.passMatchMsg = "Passwords don't match.";
            }
            
            if ($scope.repeatPassword == "") {
                $scope.passMatchMsg = "";
            }

            if ($scope.newPassword == $scope.repeatPassword) {
                $scope.passMatchMsg = "";
            }           
        };       

    }]);
})();