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
				controller: 'registerCtrl',
				templateUrl: 'Templates/register.html'
			})
			.when('/passwordReset', {
				templateUrl: 'Templates/passwordReset.html'
			})
			.when('/landing', {
				templateUrl: 'Templates/landing.html',
				access: {
					requiresLogin: true
				}
			})
			.when('/profile', {
				templateUrl: 'Templates/profile/show.html',
				access: {
					requiresLogin: true
				}
			})
			.when('/profile/create', {
				controller: 'createProfileCtrl',
				templateUrl: 'Templates/profile/create.html',
				access: {
					requiresLogin: true
				}
			})
			.when('/admin', {
				templateUrl: 'Templates/admin/home.html',
				access: {
					requiresLogin: true,
					requiredPermissions: ['Admin'],
					permissionType: 'AtLeastOne'
				}
			})
			.otherwise({
				redirectTo: '/'
			});
	}]);

	app.run(['$rootScope', '$location', 'authSvc', function ($rootScope, $location, authSvc) {
		$rootScope.$on('$routeChangeStart', function(event, args){
			if(args.access !== undefined){ //if access conditions
				authorizationResult = authSvc.authorizeRoute(args.access);

				if (authorizationResult === 'LoginRequired')
					$location.path('/');
				else if (authorizationResult === 'NotAuthorized')
					$location.path('/').replace();
			}
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
				if (params.role)
					$window.localStorage['user_roles'] = params.role;
			}
		};

		self.deleteAuthToken = function () {
			$window.localStorage.removeItem('authToken');
			$window.localStorage.removeItem('user_name');
			$window.localStorage.removeItem('user_roles');
		};

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
		};

		self.isAdmin = function () {
			return self.isInRole('Admin');
		};

		self.isInRole = function (role) {
			var found = false;
			var roles = $window.localStorage['user_roles']
			
			if (roles) {
				roles.split(',').forEach(function (r) {
					if (r.toUpperCase() === role.toUpperCase()) {
						found = true;
					}
				});
			}

			return found;
		};

		//authorize an angular route access
		self.authorizeRoute = function(access){
			if (!access.requiresLogin)
				return 'Authorized';

			if (!self.isLoggedIn())
				return 'LoginRequired';

			//requiresLogin + loggedIn => now check roles requirement
			if (!access.requiredPermissions)
				return 'Authorized';

			switch(access.permissionType){
				case 'AtLeastOne':
					var result = 'NotAuthorized';
					//if any of the required role is in user profile => Authorized
					access.requiredPermissions.forEach(function(rp){
						if(self.isInRole(rp))
							result = 'Authorized';
					});
					return result;
				case 'All':
					var result = 'Authorized';
					//if any of the required role is not in user profile => NotAuthorized
					access.requiredPermissions.forEach(function(rp){
						if(!self.isInRole(rp))
							result = 'Not Authorized';
					});
					return result;
			}

			//unknown case... in doubt => not authorized
			return 'NotAuthorized';
		};
	}]);

	//users management service
	app.service('userSvc', ['$http', 'API_URL', function ($http, API_URL) {
		self = this;

		self.create = function (username, password, confirmPassword, email) {
			return $http.post(API_URL + 'account/create', {
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
			controller: ['$scope', '$location', '$http', '$timeout', 'authSvc', 'API_TOKEN_URL', 'API_URL',
				function ($scope, $location, $http, $timeout, authSvc, API_TOKEN_URL, API_URL) {
					$scope.input = {};

					$scope.login = function () {
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
					$scope.showMsg = function(type, msg, delay){
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