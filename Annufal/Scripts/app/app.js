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
			controller: ['$scope', '$http', '$rootScope', '$location', '$window', function ($scope, $http, $rootScope, $location, $window) {
			        self = this;

			        self.parseJwtToken = function (token) {
			            var base64Url = token.split('.')[1];
			            var base64 = base64Url.replace('-', '+').replace('_', '/');
			            return JSON.parse($window.atob(base64));
			        }

			        $scope.isLoggedIn = function () {
			            var token = $scope.getAuthToken();
			            if (token) {
			                var params = self.parseJwtToken(token);
			                return Math.round(new Date().getTime() / 1000) <= params.exp;
			            }
						else
							return false;
					};

					$scope.getAuthToken = function () {
					    return $window.localStorage['authToken'];
					}

					$scope.login = function (username, password) {
					    $http.post('/token', $.param({
					        grant_type: 'password',
					        userName: username,
					        password: password
					    })).success(function (data) {
					        $window.localStorage['authToken'] = data.access_token;
                            //if error, remove it
					        delete $scope.error;
					        delete $scope.errorMsg;
					    }).error(function (data) {
					        $scope.error = data.error;
					        $scope.errorMsg = data.error_description;
					    });
					};

					$scope.logout = function () {
					    $window.localStorage.removeItem('authToken');
						$location.path('/');
					};

					$scope.goToRegister = function () {
						$location.path('/register');
					}
			}]
		}
	});
})()