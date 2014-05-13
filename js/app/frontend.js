var frontendApp = angular.module('frontendApp', [], function ($locationProvider) {
    $locationProvider.html5Mode(false);
});

frontendApp.$inject = ['$scope', '$filter', '$http'];

/*****************************
* Product Page controller
*****************************/
frontendApp.controller('productListCtrl', ['$scope', '$filter', '$http', function ($scope, $filter, $http) {
    var po = Math.random()
    console.log('po: ' + po);
    $http({
        method: 'GET',
        url: '/Product/GetProClassList',
        cache: false,
        params: { _po: po },
        headers: { 'Content-Type': 'application/json' } 
    })
       .success(function (data) {
           console.log(data.classList);
           $scope.classList = data.classList;

       });
}]);



frontendApp.directive("customSort", function () {

});
