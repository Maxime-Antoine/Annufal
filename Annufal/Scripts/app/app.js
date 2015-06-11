(function () {
	var app = angular.module('annufal', ['ngRoute']);

	// ---------------------------------------- Config ---------------------------------------
	//
	app.constant('API_URL', 'api/');

	// ---------------------------------------- Controllers -------------------------------------
	//
	app.controller('appCtrl', ['$scope', 'authSvc', function ($scope, authSvc) {
		$scope.isLoggedIn = authSvc.isLoggedIn;
	}]);

	app.controller('registerCtrl', ['$scope', '$location', 'userSvc', function ($scope, $location, userSvc) {
		$scope.register = function (username, password, confirmPassword, email) {
			userSvc.create(username, password, confirmPassword, email)
				   .success(function (data) {
					   $location.path('/display-message/Un lien de confirmation a ete envoye par email');
				   })
				   .error(function (data) {
					   $location.path('/display-message/Une erreur s\'est produite');
				   });
		};
	}]);

	app.controller('displayMessageCtrl', ['$scope', '$routeParams', function ($scope, $routeParams) {
		$scope.msg = $routeParams.msg;
	}]);

	app.controller('profileCtrl', ['$scope', '$location', 'profileSvc', 'authSvc', function ($scope, $location, profileSvc, authSvc) {
		var userId = authSvc.loggedInUserId();

		if (userId)
			profileSvc.get(userId)
					  .success(function (data) {
						  $scope.userHasProfile = true;
					  })
					  .error(function (data) {
						  $scope.userHasProfile = false;
					  });
		else
			$location.path('/show-message/An error occured');
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