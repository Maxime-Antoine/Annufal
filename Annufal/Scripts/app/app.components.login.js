(function () {
	var app = angular.module('annufal');

	//login box component
	app.directive('loginForm', function () {
		return {
			restrict: 'EA',
			templateUrl: 'Templates/login.html',
			controller: ['$scope', '$location', '$http', '$timeout', 'authSvc', 'API_TOKEN_URL', 'API_URL',
				function ($scope, $location, $http, $timeout, authSvc, API_TOKEN_URL, API_URL) {
					$scope.input = {};

					$scope.login = function () {
						if (!$scope.input.username)
							$scope.showMsg('error', 'Login vide', 5000);
						else if (!$scope.input.password)
							$scope.showMsg('error', 'Password vide', 5000);
						else
							$http.post(API_TOKEN_URL, $.param({
								grant_type: 'password',
								userName: $scope.input.username,
								password: $scope.input.password
							})).success(function (data) {
								$location.path('/landing');
							}).error(function (data) {
								$scope.showMsg('error', data.error_description, 5000);
							});
					};

					$scope.logout = function () {
						authSvc.deleteAuthToken();
						$location.path('/');
					};

					$scope.goToRegister = function () {
						$location.path('/register');
					};

					$scope.isLoggedIn = function () {
						return authSvc.isLoggedIn();
					};

					$scope.getAuthToken = authSvc.getAuthToken;

					$scope.loggedInUsername = authSvc.loggedInUserName;

					$scope.isAdmin = authSvc.isAdmin;

					$scope.resetPwd = function () {
						if (!$scope.input.username) {
							$scope.showMsg('error', "Nom d'utilisateur vide", 5000);
						} else {
							$http.get(API_URL + 'account/resetPassword/' + $scope.input.username)
								 .success(function (data) {
									$scope.showMsg('info', "Un email a ete envoye", 5000);
								 })
								 .error(function (data) {
									$scope.showMsg('error', "Utilisateur inconnu", 5000);
								 });
						}
					};

					//need not be in $scope, but helps with 'this' scope issues
					$scope.showMsg = function (type, msg, delay) {
						$scope.msgType = type;
						$scope.msg = msg;
						$timeout(function () {
							scope = $scope;
							delete scope.msgType;
							delete scope.msg;
						}, delay);
					};
				}]
		}
	});
})();