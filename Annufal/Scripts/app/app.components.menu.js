(function () {

    //left menu list component
    app.directive('menu', function () {
        return {
            restrict: 'EA',
            templateUrl: 'Templates/menu.html',
            controller: ['$scope', function ($scope) {

            }]
        }
    });
})();