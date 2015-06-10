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

    app.controller('createProfileCtrl', ['$scope', 'profileSvc', function ($scope, profileSvc) {
        $scope.input = {};

        $scope.create = function () {
            if ($scope.input)
                profileSvc.create($scope.input)
						  .success(function (data) {
						      alert('worked');
						  })
						  .error(function (data) {
						      alert('error');
						  });
        }
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

    app.service('profileSvc', ['$http', 'API_URL', function ($http, API_URL) {
        self = this;

        self.create = function (profile) {
            return $http.post(API_URL + 'profile', profile);
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