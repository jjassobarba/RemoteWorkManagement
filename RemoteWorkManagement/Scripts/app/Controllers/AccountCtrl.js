(function () {
    angular.module('RemoteManagement').controller('AccountCtrl', ['$scope', 'userService', '$http', function ($scope, userSerice, $http) {
        $scope.loginBox = true;
        $scope.forgotBox = false;
        $scope.errorForgot = false;
        $scope.isDisabledNext = true;
        $scope.msgAlert = "";
        $scope.passMatchMsg = "";
        $scope.classMsgAlert = "";
        $scope.msgUpdatePass = "Click finish to update your password";
        $scope.msgValidNewPwd = "";
        $scope.validOldPwd = "";
        $scope.stepIndex = 1;        
        $scope.word = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z]{6,}$/;
        document.getElementById('fakePrevButton').setAttribute('disabled', 'disabled');
        document.getElementById('fakeNextButton').setAttribute('disabled', 'disabled');        
        

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

        $scope.validateOldPwd = function () {            
            if ($scope.oldPassword != "") {
                var request = $http({
                    method: 'post',
                    url: '/Account/ValidateOldPassword',
                    params: {
                        password: $scope.oldPassword
                    }
                }).then(function (result) {
                    if (result.data.result) {
                        $scope.mycolor = "green";
                        $scope.validOldPwd = "Password is Valid!";
                        document.getElementById('fakeNextButton').removeAttribute('disabled');
                    } else {
                        $scope.mycolor = "red";
                        $scope.validOldPwd = "Password is not Valid!";
                        document.getElementById('fakeNextButton').setAttribute('disabled','disabled');
                    }
                });
            }
            else {
                $scope.validOldPwd = "";
                document.getElementById('fakeNextButton').setAttribute('disabled','disabled');
            }            
        };

        $scope.validateNewPwd = function () {
            if ($scope.newPassword != "") {                
                if (($scope.newPassword.match($scope.word))) {
                    $scope.mycolor = "green";
                    $scope.msgValidNewPwd = "Password is valid!";
                } else {
                    $scope.mycolor = "red";
                    $scope.msgValidNewPwd = "Pasword is not Valid!";
                }
                document.getElementById('fakeNextButton').removeAttribute('disabled');
            }else
            {
                document.getElementById('fakeNextButton').setAttribute('disabled', 'disabled');
            }
        };
                
        $scope.validateMatchPwd = function () {
            if($scope.newPassword != $scope.repeatPassword)
            {
                $scope.mycolor = "red";
                $scope.passMatchMsg = "Your passwords don't match!";
                document.getElementById('fakeNextButton').setAttribute('disabled', 'disabled');
            }
            
            if ($scope.repeatPassword == "") {
                $scope.passMatchMsg = "";
                document.getElementById('fakeNextButton').setAttribute('disabled', 'disabled');
            }

            if ($scope.newPassword == $scope.repeatPassword) {
                $scope.mycolor = "green";
                $scope.passMatchMsg = "Passwords Match!";
                document.getElementById('fakeNextButton').removeAttribute('disabled');
            }           
        };

        $scope.nextStep = function () {
            switch ($scope.stepIndex) {
                case 1:
                    $("#nextButton").click();
                    document.getElementById('fakeNextButton').setAttribute('disabled', 'disabled');
                    document.getElementById('fakePrevButton').removeAttribute('disabled');                    
                    $scope.stepIndex = $scope.stepIndex + 1;
                    break;
                case 2:
                    $("#nextButton").click();
                    document.getElementById('fakeNextButton').setAttribute('disabled', 'disabled');
                    document.getElementById('fakePrevButton').removeAttribute('disabled');
                    $scope.stepIndex = $scope.stepIndex + 1;
                    break;
                case 3:
                    $("#nextButton").click();
                    document.getElementById('fakeNextButton').innerHTML = "Finish";
                    document.getElementById('fakePrevButton').removeAttribute('disabled');
                    $scope.stepIndex = $scope.stepIndex + 1;
                    break;
                case 4:
                    document.getElementById('fakePrevButton').removeAttribute('disabled');
                     var request = $http({
                         method: 'post',
                         url: '/Account/ChangePassword',
                         params: {
                             newPassword : $scope.repeatPassword,
                             oldPassword :$scope.oldPassword
                         }
                     }).then(function (result) {
                         if (result.data.success) {
                             $scope.msgUpdatePass = "Congratulations, your password has been updated.  :)";
                         }
                         else
                         {
                             $scope.msgUpdatePass = "Sorry, try again  :(";
                         }
                     })
                     document.getElementById('fakeNextButton').setAttribute('disabled', 'disabled');
                     document.getElementById('fakePrevButton').setAttribute('disabled', 'disabled');
                     document.getElementById('fakeCancelButton').innerHTML = "<i class='icon-remove'></i> Close ";
                    break;
            }
            
        };

        $scope.prevStep = function () {            
            switch ($scope.stepIndex) {
                case 1:
                    $("#prevButton").click();
                    document.getElementById('fakePrevButton').setAttribute('disabled', 'disabled');
                    break;
                case 2:
                    $("#prevButton").click();
                    document.getElementById('fakePrevButton').setAttribute('disabled', 'disabled');
                    document.getElementById('fakeNextButton').removeAttribute('disabled');
                    $scope.stepIndex = $scope.stepIndex - 1;
                    break;
                case 3:
                    $("#prevButton").click();
                    document.getElementById('fakePrevButton').removeAttribute('disabled');
                    document.getElementById('fakeNextButton').removeAttribute('disabled');
                    $scope.stepIndex = $scope.stepIndex - 1;
                    break;
                case 4:
                    $("#prevButton").click();
                    document.getElementById('fakePrevButton').removeAttribute('disabled');
                    document.getElementById('fakeNextButton').removeAttribute('disabled');
                    document.getElementById('fakeNextButton').innerHTML = "Next  <i class='icon-arrow-right icon-on-right'></i>";
                    $scope.stepIndex = $scope.stepIndex - 1;
                    break;
            }
        };

        $scope.cancelStep = function () {
            $("#cancelButton").click();
        };
    }]);
})();