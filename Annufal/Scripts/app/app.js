(function(){
	var app = angular.module('annufal', ['ngRoute']);
		
// ---------------------------------------- Config ---------------------------------------
//
	app.constant('API_URL', 'api/');

// ---------------------------------------- Controllers -------------------------------------
//
	app.controller('registerCtrl', ['$scope', 'userSvc', function ($scope, userSvc) {
		$scope.register = function (username, password, confirmPassword, email) {
			userSvc.create(username, password, confirmPassword, email)
				   .then(function (data) { alert('worked'); }, function (data) { alert('error'); });
		};
	}]);

// ---------------------------------------- Services ----------------------------------------

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