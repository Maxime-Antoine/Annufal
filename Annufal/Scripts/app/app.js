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
			.otherwise({
				redirectTo: '/'
			});
	}]);

// ---------------------------------------- Config ---------------------------------------
//


// ---------------------------------------- Controllers -------------------------------------
//



// ---------------------------------------- Services ----------------------------------------



// ---------------------------------------- Directives --------------------------------------
//
	app.directive('loginForm', function(){
		return {
			restrict: 'EA',
			templateUrl: 'Templates/login.html',
			controller: ['$scope', '$http', '$rootScope', '$location', function ($scope, $http, $rootScope, $location) {
					$scope.isLoggedIn = function () {
						if ($rootScope.authToken)
							return true;
						else
							return false;
					};

					$scope.getAuthToken = function () {
						return $rootScope.authToken;
					}

					$scope.login = function (username, password) {
						$http.post('/token', $.param({
							grant_type: 'password',
							userName: username,
							password: password
						})).success(function (data) {
							$rootScope.authToken = data.access_token;
						})
					};

					$scope.logout = function () {
						delete $rootScope.authToken;
						$location.path('/');
					};

					$scope.goToRegister = function () {
						$location.path('/register');
					}
			}]
		}
	});
})()