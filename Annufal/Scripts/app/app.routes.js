(function () {
    var app = angular.module('annufal');

    app.config(['$routeProvider', function ($routeProvider) {
        $routeProvider
            .when('/', {
                templateUrl: 'Templates/home.html'
            })
            .when('/register', {
                controller: 'registerCtrl',
                templateUrl: 'Templates/register.html'
            })
            .when('/display-message/:msg', {
                templateUrl: 'Templates/display-message.html',
                controller: 'displayMessageCtrl'
            })
            .when('/profile', {
                templateUrl: 'Templates/profile.html',
                controller: 'profileCtrl',
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
})();
