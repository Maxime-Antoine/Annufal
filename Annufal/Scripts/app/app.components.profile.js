(function () {
    var app = angular.module('annufal');


    //-------------------------------------------- Services --------------------------------------------

    app.service('profileSvc', ['$http', 'API_URL', function ($http, API_URL) {
        self = this;

        self.create = function (profile) {
            return $http.post(API_URL + 'profile', profile);
        }

        self.get = function (id) { //userId as GUID or profileId as INT
            return $http.get(API_URL + 'profile/' + id);
        }

        self.edit = function (profile) {
            return $http.put(API_URL + 'profile', profile);
        }
    }]);



    //-------------------------------------------- Directives ------------------------------------------

    app.directive('createProfile', function () {
        return {
            restrict: 'EA',
            templateUrl: 'Templates/profile/create.html',
            controller: ['$scope', '$route', 'profileSvc', function ($scope, $route, profileSvc) {
                $scope.input = {};

                $scope.create = function () {
                    if ($scope.input)
                        profileSvc.create($scope.input)
                                  .success(function (data) {
                                      $route.reload();
                                  })
                                  .error(function (data) {
                                      alert('error');
                                  });
                }
            }]
        };
    });

    app.directive('showProfile', function () {
        return {
            restrict: 'EA',
            templateUrl: 'Templates/profile/show.html',
            controller: ['$scope', 'profileSvc', 'authSvc', function ($scope, profileSvc, authSvc) {
                $scope.profile = {};

                //load current user profile
                var userId = authSvc.loggedInUserId();
                if (userId)
                    profileSvc.get(userId)
                              .success(function (data) {
                                  console.log(data);
                                  $scope.profile = data;
                              })
                              .error(function(data){
                                  alert('Error');
                              });
            }]
        };
    });
})();