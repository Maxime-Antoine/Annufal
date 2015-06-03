(function(){
	var app = angular.module('annufal', ['ngRoute']);
		
// ---------------------------------------- Routes ------------------------------------------
//
	app.config(['$routeProvider', function($routeProvider){
		$routeProvider
			.when('/', {
				templateUrl: 'templates/register.html'
			})
			.when('/register', {
				templateUrl: 'templates/register.html'
			})
			.otherwise({
				redirectTo: '/'
			});
	}]);

// ---------------------------------------- Controllers -------------------------------------
//


// ---------------------------------------- Directives --------------------------------------
//


})()