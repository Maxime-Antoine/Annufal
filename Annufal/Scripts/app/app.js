(function(){
	var app = angular.module('annufal', ['ngRoute']);
		
// ---------------------------------------- Routes ------------------------------------------
//
	app.config(['$routeProvider', function($routeProvider){
		$routeProvider
			.when('/', {
				templateUrl: 'Templates/register.html'
			})
			.when('/register', {
				templateUrl: 'Templates/register.html'
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