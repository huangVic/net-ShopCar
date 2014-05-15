var frontendApp = angular.module('frontendApp', ['ngSanitize'], function ($locationProvider) {
    $locationProvider.html5Mode(false);
});

frontendApp.$inject = ['$scope', '$filter', '$http'];

/*****************************
* Product Page controller
*****************************/
frontendApp.controller('productListCtrl', ['$scope', '$filter', '$http', function ($scope, $filter, $http) {
    var po = Math.random()
    console.log('po: ' + po);

    $scope.listFlag = false;
    $scope.detailFlag = false;

    // ------- class list ----------//
    $http({
        method: 'GET',
        url: '/Product/GetProClassList',
        cache: false,
        params: { _po: po },
        headers: { 'Content-Type': 'application/json' } 
    })
    .success(function (data) {
        //console.log(data.classList);
        $scope.classList = data.classList;

    });

    
    // --------- get product item  --------------//
    $scope.getProductItem = function (productAppSer) {
        console.log('---->' + productAppSer)

        $scope.productItem = [];
        $scope.productFiles = [];
        $scope.prodFeature = null;
        $scope.prodDesc = null;

        $scope.listFlag = false;
        $scope.detailFlag = true;

        var oo = Math.random()
        
        $http({
            method: 'GET',
            url: '/Product/getProductData',
            cache: false,
            params: { _po: po, appSer: productAppSer },
            headers: { 'Content-Type': 'application/json' }
        })
        .success(function (data) {
            $scope.productItem = data.productItem;
            $scope.productFiles = data.files;
            $scope.prodFeature = data.productItem[0].prod_feature.replace(/\r\n/g, "<br />")
            $scope.prodDesc = data.productItem[0].prod_desc.replace(/\r\n/g, "<br />")
        });


    };

    

    // ----- product list -------//
    $scope.gap = 5;
    $scope.nonfilertKey = ['app_ser', 'details'];
    $scope.filteredItems = [];
    $scope.groupedItems = [];
    $scope.itemsPerPage = 9;
    $scope.pagedItems = [];
    $scope.currentPage = 0;
    $scope.items = []

    $scope.getProductList = function (classAppSer) {
        console.log('classAppSer: ' + classAppSer);
        var p1 = Math.random();
        console.log('p1: ' + p1);

        $scope.listFlag = true;
        $scope.detailFlag = false;

        if (classAppSer) {
            $http({
                method: 'GET',
                url: '/Product/ClassToActiveProductData',
                cache: false,
                params: { _po: po, classAppSer: classAppSer },
                headers: { 'Content-Type': 'application/json' }
            })
           .success(function (data) {
               //console.log(data.productList);
               $scope.items = data.productList;

               var searchVaildate = function (attr) {
                   if (!attr) {
                       return false;
                   }
                   for (var val in $scope.nonfilertKey) {
                       //console.log('val >> ' + $scope.nonfilertKey[val])
                       if ($scope.nonfilertKey[val].toLowerCase() === attr.toLowerCase()) {
                           return false
                       }
                   };
                   return true
               }

               var searchMatch = function (haystack, needle) {
                   //console.log('needle >> ' + needle)
                   //console.log('haystack >> ' + haystack)
                   if (!needle) {
                       return true;
                   }
                   //console.log('#### ' + haystack.toLowerCase().indexOf(needle.toLowerCase()))
                   return haystack.toLowerCase().indexOf(needle.toLowerCase()) !== -1;
               };

               // init the filtered items
               $scope.search = function () {
                   $scope.selectItems = [];
                   $scope.filteredItems = $filter('filter')($scope.items, function (item) {
                       for (var attr in item) {
                               if (searchMatch(item[attr], $scope.query))
                                   return true;
                       }
                       return false;
                   });
                   // take care of the sorting order
                   /*if ($scope.sort.sortingOrder !== '') {
                       $scope.filteredItems = $filter('orderBy')($scope.filteredItems, $scope.sort.sortingOrder, $scope.sort.reverse);
                   }*/
                   $scope.currentPage = 0;
                   // now group by pages
                   $scope.groupToPages();
               };


               // calculate page in place
               $scope.groupToPages = function () {
                   $scope.pagedItems = [];

                   for (var i = 0; i < $scope.filteredItems.length; i++) {
                       if (i % $scope.itemsPerPage === 0) {
                           $scope.pagedItems[Math.floor(i / $scope.itemsPerPage)] = [$scope.filteredItems[i]];
                       } else {
                           $scope.pagedItems[Math.floor(i / $scope.itemsPerPage)].push($scope.filteredItems[i]);
                       }
                   }
               };

               $scope.range = function (size, start, end) {
                   var ret = [];
                   //console.log(size, start, end);

                   if (size < end) {
                       end = size;
                       start = size - $scope.gap;
                   }
                   for (var i = start; i < end; i++) {
                       if (i >= 0) {
                           ret.push(i);
                       }
                   }
                   //console.log(ret);
                   return ret;
               };

               $scope.prevPage = function () {
                   $scope.selectItems = [];
                   // $scope.detailsItems = [];
                   if ($scope.currentPage > 0) {
                       $scope.currentPage--;
                   }
               };

               $scope.nextPage = function () {
                   $scope.selectItems = [];
                   // $scope.detailsItems = [];
                   if ($scope.currentPage < $scope.pagedItems.length - 1) {
                       $scope.currentPage++;
                   }
               };

               $scope.setPage = function () {
                   $scope.selectItems = [];
                   // $scope.detailsItems = [];
                   $scope.currentPage = this.n;
               };

               // functions have been describe process the data for display
               $scope.search();

           });
        }
    };

    $scope.getProductList(1);

}]);


/*****************************
* Product Item Page controller
*****************************/
frontendApp.controller('productItemCtrl', ['$scope', '$filter', '$http', function ($scope, $filter, $http) {
    var po = Math.random()
    console.log('item po: ' + po);

    // ------- class list ----------//
    $http({
        method: 'GET',
        url: '/Product/GetProClassList',
        cache: false,
        params: { _po: po },
        headers: { 'Content-Type': 'application/json' } 
    })
    .success(function (data) {
        //console.log(data.classList);
        $scope.classList = data.classList;

    });
}]);