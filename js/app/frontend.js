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
        url: '/Frontend/GetProClassList',
        cache: false,
        params: { _po: po },
        headers: { 'Content-Type': 'application/json' } 
    })
    .success(function (data) {
        //console.log(data.classList);
        $scope.classList = data.classList;

        $scope.getProductList(data.classList[0].app_ser);
    });

    
    // --------- get product item  --------------//
    $scope.getProductItem = function (productAppSer) {

        $scope.productItem = [];
        $scope.productFiles = [];
        $scope.prodFeature = null;
        $scope.prodDesc = null;

        $scope.listFlag = false;
        $scope.detailFlag = true;

        var oo = Math.random()
        
        $http({
            method: 'GET',
            url: '/Frontend/getProductData',
            cache: false,
            params: { _po: oo, appSer: productAppSer },
            headers: { 'Content-Type': 'application/json' }
        })
        .success(function (data) {
            $scope.productItem = data.productItem;
            $scope.productFiles = data.files;
            $scope.userInfo = data.userInfo;
            $scope.prodFeature = data.productItem[0].prod_feature.replace(/\r\n/g, "<br />")
            $scope.prodDesc = data.productItem[0].prod_desc.replace(/\r\n/g, "<br />")
        });


    };


    // --------- add product item to shopCar --------------//
    $scope.addToCar = function() {

        console.log("------- add to Car ----- ");
        console.log("  >>> app_ser " + $scope.productItem[0].app_ser);
        console.log("  >>> prod_no " + $scope.productItem[0].prod_no);
        console.log("  >>> prod_name " + $scope.productItem[0].prod_name);

        console.log("------- user info ----- ");
        console.log("  >>> user_id " + $scope.userInfo.user_id);
        console.log("  >>> user_name " + $scope.userInfo.user_name);
        console.log("  >>> user_app_ser " + $scope.userInfo.app_ser);

        

        if ($scope.userInfo.user_id == "" || $scope.userInfo.app_ser ==""){
            alert("請先登入會員或註冊新會員!!");
        }else{
            $.post('/Frontend/addProductToCar', $scope.productItem[0]).success(function (data) {
                console.log("------- preOrderList in Car ----- ");
                $(".cart-detail").find("li").remove();
                var total = 0;
                $.each(data.preOrderList, function (i, val) {
                    var price = val.prod_price;
                    if (val.prod_special_price > 0) { price = val.prod_special_price };
                    total += price;
                    $(".cart-detail").append('<li>' + val.prod_name + ' (' + val.prod_no + ')' + '&nbsp;&nbsp;&nbsp;&nbsp; $' + new Intl.NumberFormat("en-US").format(price) + '</li>');
                });
                $('#cart-total').text('$ ' + new Intl.NumberFormat("en-US").format(total));
            });
        }


        /*
        $http({
            method: 'POST',
            url: '/Frontend/addProductToCar',
            cache: false,
            data: { app_ser: $scope.productItem[0].app_ser, prod_no: $scope.productItem[0].prod_no, prod_name: $scope.productItem[0].prod_name },
           // headers: { 'Content-Type': 'application/json' }
        })
       .success(function (data) {


           console.log("------- preOrderList in Car ----- ");
           console.log("  >>> app_ser " + data.preOrderList[0].app_ser);
           console.log("  >>> product_id " + data.preOrderList[0].product_id);
           console.log("  >>> prod_name " + data.preOrderList[0].product_name);

       });
       */
    }

    

    // ----- product list -------//
    $scope.gap = 5;
    $scope.nonfilertKey = ['app_ser', 'details'];
    $scope.filteredItems = [];
    $scope.groupedItems = [];
    $scope.itemsPerPage = 6;
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
                url: '/Frontend/ClassToActiveProductData',
                cache: false,
                params: { _po: p1, classAppSer: classAppSer },
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





/*****************************
* MemberAdd controller
*****************************/
frontendApp.controller('memberAddCtrl', ['$scope', '$filter', '$http', function ($scope, $filter, $http) {

    $("#form1").validationEngine();

    $scope.formSubmit = function () {
        /*
        console.log($scope.email);
        console.log($scope.ps);
        console.log($scope.ps_confirm);*/
        if ($("#form1").validationEngine('validate')) {

            console.log($scope.email);
            
            var po = Math.random()
            $http({
                method: 'GET',
                url: '/Frontend/IDExistMember',
                cache: false,
                params: { _po: po, user_id: $scope.email },
                headers: { 'Content-Type': 'application/json' }
            })
           .success(function (data) {
               if (data.message == 'OK') {
                   $("#form1").submit();
               } else {
                   alert(data.message);
               }

           }); 
        };
    };

}]);


/*****************************
* MemberEdit controller
*****************************/
frontendApp.controller('memberEditCtrl', ['$scope', '$filter', '$http', function ($scope, $filter, $http) {

    $("#form1").validationEngine();
    $('#BirthDay').datepicker({
        format: "yyyy/mm/dd",
        language: "zh-TW",
        todayHighlight: true
    });


    /*
    $scope.$watch('checkVal', function () {
        console.log($scope.checkVal);
        if ($scope.checkVal) {
            if ($("#form1").validationEngine('validate')) {
            }
        }
    });
    */

    
    $scope.formSubmit = function () {

        if ($("#form1").validationEngine('validate')) {
            $("#form1").submit();
        };
    };
    

}]);




/*****************************
* News List controller
*****************************/
frontendApp.controller('newsListCtrl', ['$scope', '$filter', '$http', function ($scope, $filter, $http) {

    $scope.sort = {
        sortingOrder: 'createDate',
        reverse: false
    };
    // init

    $scope.gap = 5;
    $scope.nonfilertKey = ['app_ser', 'details'];
    $scope.filteredItems = [];
    $scope.groupedItems = [];
    $scope.itemsPerPage = 5;
    $scope.pagedItems = [];
    $scope.currentPage = 0;
    $scope.items = []

    var po = Math.random()
    $http({
        method: 'GET',
        url: '/Frontend/NewsToJsonData',
        cache: false,
        params: { _po: po },
        headers: { 'Content-Type': 'application/json' }
    })
    .success(function (data) {
        $scope.items = data.newsList;

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
                    /*
                    console.log('======================')
                    console.log('query >> ' + $scope.query)
                    console.log('attr >> ' + attr)
                    console.log('item >> ' + item[attr])
                    console.log('return >> ' + searchVaildate(attr))
                    */
                    if (searchVaildate(attr)) {
                        if (searchMatch(item[attr], $scope.query))
                            return true;
                    }
                }
                return false;
            });
            // take care of the sorting order
            if ($scope.sort.sortingOrder !== '') {
                $scope.filteredItems = $filter('orderBy')($scope.filteredItems, $scope.sort.sortingOrder, $scope.sort.reverse);
            }
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


        $scope.openDetail = function (appSer) {
            //console.log('>>> : ' + appSer);
            $scope.selectItems = [];

            for (var attr in $scope.items) {
                //console.log('attr >>> : ' + attr)
                //console.log('items >>> : ' + $scope.items[attr].app_ser)
                if ($scope.items[attr].app_ser === appSer) {

                    $scope.selectItems.push($scope.items[attr]);
                    /*
                    for (var i = 0; i < $scope.items[attr].details.length; i++) {
                        console.log('details >>> : ' + $scope.items[attr].details[i]);
                        $scope.detailsItems.push($scope.items[attr].details[i]);
                    }
                    */
                }
            }
            //console.log('selectItems >>> : ' + $scope.selectItems)
        };



    });


}]);



frontendApp.directive("customSort", function () {
    return {
        restrict: 'A',
        transclude: true,
        scope: {
            order: '=',
            sort: '='
        },
        template:
          ' <a ng-click="sort_by(order)" >' +
          '    <span ng-transclude></span>' +
          '    <i ng-class="selectedCls(order)"></i>' +
          '</a>',
        link: function (scope) {

            // change sorting order
            scope.sort_by = function (newSortingOrder) {
                var sort = scope.sort;

                if (sort.sortingOrder == newSortingOrder) {
                    sort.reverse = !sort.reverse;
                }

                sort.sortingOrder = newSortingOrder;
            };


            scope.selectedCls = function (column) {
                if (column == scope.sort.sortingOrder) {
                    return ('fa fa-sort-' + ((scope.sort.reverse) ? 'down' : 'up'));
                }
                else {
                    return 'fa fa-sort'
                }
            };
        }// end link
    }
});