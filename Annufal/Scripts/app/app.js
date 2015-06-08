(function(){
	var app = angular.module('annufal', ['ngRoute']);
		
// ---------------------------------------- Routes ---------------------------------------
//
	app.config(['$routeProvider', function($routeProvider){
		$routeProvider
			.when('/', {
				templateUrl: 'Templates/home.html'
			})
			.when('/register', {
				templateUrl: 'Templates/register.html'
			})
			.when('/landing', {
				templateUrl: 'Templates/landing.html'
			})
			.otherwise({
				redirectTo: '/'
			});
	}]);

// ---------------------------------------- Config ---------------------------------------
//
	app.constant('API_URL', 'api/');
	app.constant('API_TOKEN_URL', 'api/token');

	app.config(['$httpProvider', function ($httpProvider) {
		$httpProvider.interceptors.push('authInterceptor');
	}]);
// ---------------------------------------- Controllers -------------------------------------
//



// ---------------------------------------- Services ----------------------------------------

	//intercepts requests to API
	//if login request -> saved returned token
	//if other request -> attached saved token (if any)
	app.factory('authInterceptor', ['API_URL', 'API_TOKEN_URL', 'authSvc', function (API_URL, API_TOKEN_URL, authSvc) {
		return {
			request: function (config) {
				//if req to API and token is saved -> attach it
				var token = authSvc.getAuthToken();
				if (config.url.indexOf(API_URL) !== -1 && token) {
					config.headers.Authorization = 'Bearer ' + token;
				}

				return config;
			},
			response: function (res) {
				//if req to api/token and token is returned -> save it
				if (res.config.url.indexOf(API_TOKEN_URL) !== -1 && res.data.access_token) {
					authSvc.saveAuthToken(res.data.access_token);
				}

				return res;
			}
		}
	}]);

	//JWT authentication token handling service
	app.service('authSvc', ['$window', function ($window) {
		var self = this;

		self.parseJwtToken = function (token) {
			var base64Url = token.split('.')[1];
			var base64 = base64Url.replace('-', '+').replace('_', '/');
			return JSON.parse($window.atob(base64));
		};

		self.getAuthToken = function () {
			return $window.localStorage['authToken'];
		};

		self.saveAuthToken = function (token) {
			$window.localStorage['authToken'] = token;
		};

		self.deleteAuthToken = function () {
			$window.localStorage.removeItem('authToken');
		}

		self.isLoggedIn = function () {
			var token = self.getAuthToken();
			if (token) {
				var params = self.parseJwtToken(token);
				return Math.round(new Date().getTime() / 1000) <= params.exp; //Date().getTime() returns ms
			}
			else
				return false;
		};

		self.loggedInUserName = function () {
			var token = self.getAuthToken();
			if (token) {
				var params = self.parseJwtToken(token);
				return params.unique_name;
			}
		}
	}]);

// ---------------------------------------- Directives --------------------------------------
//
	app.directive('loginForm', function(){
		return {
			restrict: 'EA',
			templateUrl: 'Templates/login.html',
			controller: ['$scope', '$http', '$location', 'authSvc', 'API_TOKEN_URL', 'API_URL',
				function ($scope, $http, $location, authSvc, API_TOKEN_URL, API_URL) {
					$scope.login = function (username, password) {
						$http.post(API_TOKEN_URL, $.param({
							grant_type: 'password',
							userName: username,
							password: password
						})).success(function (data) {
							$location.path('/landing');
							//if error, remove it
							delete $scope.error;
							delete $scope.errorMsg;
						}).error(function (data) {
							$scope.error = data.error;
							$scope.errorMsg = data.error_description;
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

					$scope.getAuthToken = function () {
						return authSvc.getAuthToken();
					};

					$scope.loggedInUsername = function () {
						return authSvc.loggedInUserName();
					};
			}]
		}
	});
})()