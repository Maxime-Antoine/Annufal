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
			.when('/admin', {
				templateUrl: 'Templates/admin/home.html'
			})
			.when('/profile', {
				templateUrl: 'Templates/profile/show.html'
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
	app.controller('registerCtrl', ['$scope', 'userSvc', function ($scope, userSvc) {
		$scope.register = function (username, password, confirmPassword, email) {
			userSvc.create(username, password, confirmPassword, email)
				   .then(function (data) { alert('worked'); }, function (data) { alert('error'); });
		};
	}]);


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

	//authentication service
	app.service('authSvc', ['$window', function ($window) {
		var self = this;

		self.parseJwtToken = function (token) {
			var base64Url = token.split('.')[1];
			var base64 = base64Url.replace('-', '+').replace('_', '/');
			console.log(JSON.parse($window.atob(base64))); //DEBUG
			return JSON.parse($window.atob(base64));
		};

		self.getAuthToken = function () {
			return $window.localStorage['authToken'];
		};

		self.saveAuthToken = function (token) {
			$window.localStorage['authToken'] = token;
			//save other common params
			if (token) {
				var params = self.parseJwtToken(token);
				if (params.unique_name)
					$window.localStorage['user_name'] = params.unique_name;
				if (params.role) {
					params.role.forEach(function (r) {
						if (r.toUpperCase() === 'ADMIN')
							$window.localStorage['isAdmin'] = true;
					});
				}
			}
		};

		self.deleteAuthToken = function () {
			$window.localStorage.removeItem('authToken');
			$window.localStorage.removeItem('user_name');
			$window.localStorage.removeItem('isAdmin');
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
			return $window.localStorage['user_name'];
		}

		self.loggedInUserIsAdmin = function () {
			return $window.localStorage['isAdmin'];
		}
	}]);

	//users management service
	app.service('userSvc', ['$http', 'API_URL', function ($http, API_URL) {
		self = this;

		self.create = function (username, password, confirmPassword, email) {
			return $http.post(API_URL + '/account/create', {
				username: username,
				password: password,
				confirmPassword: confirmPassword,
				email: email
			});
		}
	}]);

// ---------------------------------------- Directives --------------------------------------

	//login box component
	app.directive('loginForm', function(){
		return {
			restrict: 'EA',
			templateUrl: 'Templates/login.html',
			controller: ['$scope', '$location', '$http', 'authSvc', 'API_TOKEN_URL',
				function ($scope, $location, $http, authSvc, API_TOKEN_URL) {
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

					$scope.isAdmin = function () {
						return authSvc.loggedInUserIsAdmin();
					};
			}]
		}
	});

	//left menu list component
	app.directive('menu', function () {
		return {
			restrict: 'EA',
			templateUrl: 'Templates/menu.html',
			controller: ['$scope', function ($scope) {

			}]
		}
	});

	//custom validation directive to check if 2 fields in a form are equals
	app.directive('compareTo', function () {
		return {
			require: "ngModel",
			scope: {
				otherModelValue: "=compareTo"
			},
			link: function (scope, element, attributes, ngModel) {
				ngModel.$validators.compareTo = function (modelValue) {
					return modelValue == scope.otherModelValue;
				};

				scope.$watch("otherModelValue", function () {
					ngModel.$validate();
				});
			}
		}
	});
})()