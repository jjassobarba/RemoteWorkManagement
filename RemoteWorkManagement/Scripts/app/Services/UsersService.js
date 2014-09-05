(function () {
    angular.module('RemoteManagement').service('userService', ['$http', '$q', function userService($http, $q) {

        //Return Public API
        return ({
            loginUser: loginUser,
            registerUser: registerUser,
            getUser: getUser,
            getUserByUserName : getUserByUserName,
            getAllUsers: getAllUsers,
            getAllUsersInfo: getAllUsersInfo
        });

        //------Public Methods------

        function loginUser(username, password) {
            var request = $http({
                method: 'post',
                url: '/Home/Authorize',
                params: {
                    username: username,
                    password: password
                }
            });
            return (request.then(handleSuccess, handleError));
        }

        function registerUser(username, firstName, lastName, position, rol, projectLeader, sensei, remoteDays, flexTime) {
            var request = $http({
                method: 'post',
                url: '/Home/CreateUser',
                params: {
                    username: username,
                    firstName: firstName,
                    lastName: lastName,
                    position: position,
                    rol: rol,
                    projectLeader: projectLeader,
                    sensei: sensei,
                    remoteDays: remoteDays,
                    flexTime: flexTime
                }
            });
            return (request.then(handleSuccess, handleError));
        }

        function getUser(idUser) {
            var request = $http({
                method: 'post',
                url: '/Home/GetUser',
                params: {
                    userId: idUser
                }
            });
            return (request.then(handleSuccess, handleError));
        }

        function getUserByUserName(userName) {
            console.log(userName);
            var request = $http({
                method: 'post',
                url : '/Home/GetUserByUserName',
                params: {
                    username : userName
                }
            });
            console.log("termino");
            return (request.then(handleSuccess, handleError));
        }

        function getAllUsers() {
            var request = $http({
                method: 'get',
                url: '/Home/GetAllUsers'
            });
            return (request.then(handleSuccess, handleError));
        }

        function getAllUsersInfo() {
            var request = $http({
                method: 'get',
                url: '/Home/GetAllUsersInfo'
            });
            return (request.then(handleSuccess, handleError));
        }
        //--------------------------


        //------------Private Methods-------------
        // I transform the error response, unwrapping the application dta from
        // the API response payload.

        function handleError(response) {
            // The API response from the server should be returned in a
            // normalized format. However, if the request was not handled by the
            // server (or what not handles properly - ex. server error), then we
            // may have to normalize it on our end, as best we can.
            if (!angular.isObject(response.data) || !response.data.message) {
                return ($q.reject("An unknown error has ocurred"));
            }
            return ($q.reject(response.data.message));
        }

        // I transform the successful response, unwrapping the application data
        // from the API response payload.

        function handleSuccess(response) {
            return (response.data);
        }

        //----------------------------------------
    }]);
})();