var orderApp = angular.module('orderApp', [], function ($locationProvider) {
    $locationProvider.html5Mode(false);
});

orderApp.controller('NavCtrl', ['$scope', '$location', function ($scope, $location) {
    $scope.navClass = function (page) {
        var currentRoute = $location.absUrl().split("/")[4];
       /* console.log(currentRoute)
        console.log($location.absUrl())
        console.log($location.absUrl().split("/")[4])
       */
      
        return page === currentRoute ? 'active' : '';
    };
}]);

orderApp.controller('orderListCtrl', ['$scope', '$filter', '$http', function ($scope, $filter, $http) {

    $scope.sort = {
        sortingOrder: 'app_no',
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
        url: '/Order/OrderAllList',
        cache: false,
        params: { _po: po },
        headers: { 'Content-Type': 'application/json' }
    })
    .success(function (data) {
        $scope.items=data;     

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


        $scope.goUpdate = function (appSer) {
            console.log('>>>goUpdate ser : ' + appSer);
            var po = Math.random()
            window.location.href = "/Order/OrderEdit?app_ser=" + appSer +"&po="+ po
        }

    });


    /*
    $scope.items = [
        {
            "app_ser": "1", "app_no": "Q123456789", "create_date": "2012/12/24", "amount": "amount 1", "status": "status 1", "field5 ": "field5 1",
            "details": [
                {
                    "app_dser": "1",
                    "productID": "W2990421a",
                    "proName": "ASUS S550CB-0051A3337U 時尚黑 750G + 24G 混碟設計a",
                    "pro_class_id": "1"
                },
                {
                    "app_dser": "2",
                    "productID": "C299042ab",
                    "proName": "ASUS S550CB-0051A3337U 混碟設計ab",
                    "pro_class_id": "2"
                }
            ]
        }
    ];
    */
    /*
    $scope.items = [
        { "app_ser": "1", "app_no": "app_no 1", "create_date": "create_date 1", "amount": "amount 1", "status": "status 1", "field5 ": "field5 1" },
        { "app_ser": "2", "app_no": "app_no 2", "create_date": "create_date 1", "amount": "amount 2", "status": "status 2", "field5 ": "field5 2" },
        { "app_ser": "3", "app_no": "app_no 3", "create_date": "create_date 1", "amount": "amount 3", "status": "status 3", "field5 ": "field5 3" },
        { "app_ser": "4", "app_no": "app_no 4", "create_date": "create_date 1", "amount": "amount 4", "status": "status 4", "field5 ": "field5 4" },
        { "app_ser": "5", "app_no": "app_no 5", "create_date": "create_date 1", "amount": "amount 5", "status": "status 5", "field5 ": "field5 5" },
        { "app_ser": "6", "app_no": "app_no 6", "create_date": "create_date 1", "amount": "amount 6", "status": "status 6", "field5 ": "field5 6" },
        { "app_ser": "7", "app_no": "app_no 7", "create_date": "create_date 1", "amount": "amount 7", "status": "status 7", "field5 ": "field5 7" },
        { "app_ser": "8", "app_no": "app_no 8", "create_date": "create_date 1", "amount": "amount 8", "status": "status 8", "field5 ": "field5 8" },
        { "app_ser": "9", "app_no": "app_no 9", "create_date": "create_date 1", "amount": "amount 9", "status": "status 9", "field5 ": "field5 9" },
        { "app_ser": "10", "app_no": "app_no 10", "create_date": "create_date 1", "amount": "amount 10", "status": "status 10", "field5 ": "field5 10" },
        { "app_ser": "11", "app_no": "app_no 11", "create_date": "create_date 1", "amount": "amount 11", "status": "status 11", "field5 ": "field5 11" },
        { "app_ser": "12", "app_no": "app_no 12", "create_date": "create_date 1", "amount": "amount 12", "status": "status 12", "field5 ": "field5 12" },
        { "app_ser": "13", "app_no": "app_no 13", "create_date": "create_date 1", "amount": "amount 13", "status": "status 13", "field5 ": "field5 13" },
        { "app_ser": "14", "app_no": "app_no 14", "create_date": "create_date 1", "amount": "amount 14", "status": "status 14", "field5 ": "field5 14" },
        { "app_ser": "15", "app_no": "app_no 15", "create_date": "create_date 1", "amount": "amount 15", "status": "status 15", "field5 ": "field5 15" },
        { "app_ser": "16", "app_no": "app_no 16", "create_date": "create_date 1", "amount": "amount 16", "status": "status 16", "field5 ": "field5 16" },
        { "app_ser": "17", "app_no": "app_no 17", "create_date": "create_date 1", "amount": "amount 17", "status": "status 17", "field5 ": "field5 17" },
        { "app_ser": "18", "app_no": "app_no 18", "create_date": "create_date 1", "amount": "amount 18", "status": "status 18", "field5 ": "field5 18" },
        { "app_ser": "19", "app_no": "app_no 19", "create_date": "create_date 1", "amount": "amount 19", "status": "status 19", "field5 ": "field5 19" },
        { "app_ser": "20", "app_no": "app_no 5", "create_date": "create_date 1", "amount": "amount 5", "status": "status 5", "field5 ": "field5 5" },
        { "app_ser": "21", "app_no": "app_no 6", "create_date": "create_date 1", "amount": "amount 6", "status": "status 6", "field5 ": "field5 6" },
        { "app_ser": "22", "app_no": "app_no 7", "create_date": "create_date 1", "amount": "amount 7", "status": "status 7", "field5 ": "field5 7" },
        { "app_ser": "23", "app_no": "app_no 8", "create_date": "create_date 1", "amount": "amount 8", "status": "status 8", "field5 ": "field5 8" },
        { "app_ser": "24", "app_no": "app_no 9", "create_date": "create_date 1", "amount": "amount 9", "status": "status 9", "field5 ": "field5 9" },
        { "app_ser": "25", "app_no": "app_no 10", "create_date": "create_date 1", "amount": "amount 10", "status": "status 10", "field5 ": "field5 10" },
        { "app_ser": "26", "app_no": "app_no 11", "create_date": "create_date 1", "amount": "amount 11", "status": "status 11", "field5 ": "field5 11" },
        { "app_ser": "27", "app_no": "app_no 12", "create_date": "create_date 1", "amount": "amount 12", "status": "status 12", "field5 ": "field5 12" },
        { "app_ser": "28", "app_no": "app_no 13", "create_date": "create_date 1", "amount": "amount 13", "status": "status 13", "field5 ": "field5 13" },
        { "app_ser": "29", "app_no": "app_no 14", "create_date": "create_date 1", "amount": "amount 14", "status": "status 14", "field5 ": "field5 14" },
        { "app_ser": "30", "app_no": "app_no 15", "create_date": "create_date 1", "amount": "amount 15", "status": "status 15", "field5 ": "field5 15" },
        { "app_ser": "31", "app_no": "app_no 16", "create_date": "create_date 1", "amount": "amount 16", "status": "status 16", "field5 ": "field5 16" },
        { "app_ser": "32", "app_no": "app_no 17", "create_date": "create_date 1", "amount": "amount 17", "status": "status 17", "field5 ": "field5 17" },
        { "app_ser": "33", "app_no": "app_no 18", "create_date": "create_date 1", "amount": "amount 18", "status": "status 18", "field5 ": "field5 18" },
        { "app_ser": "34", "app_no": "app_no 19", "create_date": "create_date 1", "amount": "amount 19", "status": "status 19", "field5 ": "field5 19" },
        { "app_ser": "35", "app_no": "app_no 5", "create_date": "create_date 1", "amount": "amount 5", "status": "status 5", "field5 ": "field5 5" },
        { "app_ser": "36", "app_no": "app_no 6", "create_date": "create_date 1", "amount": "amount 6", "status": "status 6", "field5 ": "field5 6" },
        { "app_ser": "37", "app_no": "app_no 7", "create_date": "create_date 1", "amount": "amount 7", "status": "status 7", "field5 ": "field5 7" },
        { "app_ser": "38", "app_no": "app_no 8", "create_date": "create_date 1", "amount": "amount 8", "status": "status 8", "field5 ": "field5 8" },
        { "app_ser": "39", "app_no": "app_no 9", "create_date": "create_date 1", "amount": "amount 9", "status": "status 9", "field5 ": "field5 9" },
        { "app_ser": "40", "app_no": "app_no 10", "create_date": "create_date 1", "amount": "amount 10", "status": "status 10", "field5 ": "field5 10" },
        { "app_ser": "41", "app_no": "app_no 11", "create_date": "create_date 1", "amount": "amount 11", "status": "status 11", "field5 ": "field5 11" },
        { "app_ser": "42", "app_no": "app_no 12", "create_date": "create_date 1", "amount": "amount 12", "status": "status 12", "field5 ": "field5 12" },
        { "app_ser": "43", "app_no": "app_no 13", "create_date": "create_date 1", "amount": "amount 13", "status": "status 13", "field5 ": "field5 13" },
        { "app_ser": "44", "app_no": "app_no 14", "create_date": "create_date 1", "amount": "amount 14", "status": "status 14", "field5 ": "field5 14" },
        { "app_ser": "45", "app_no": "app_no 15", "create_date": "create_date 1", "amount": "amount 15", "status": "status 15", "field5 ": "field5 15" },
        { "app_ser": "46", "app_no": "app_no 16", "create_date": "create_date 1", "amount": "amount 16", "status": "status 16", "field5 ": "field5 16" },
        { "app_ser": "47", "app_no": "app_no 17", "create_date": "create_date 1", "amount": "amount 17", "status": "status 17", "field5 ": "field5 17" },
        { "app_ser": "48", "app_no": "app_no 18", "create_date": "create_date 1", "amount": "amount 18", "status": "status 18", "field5 ": "field5 18" },
        { "app_ser": "49", "app_no": "app_no 19", "create_date": "create_date 1", "amount": "amount 19", "status": "status 19", "field5 ": "field5 19" },
        { "app_ser": "50", "app_no": "app_no 5", "create_date": "create_date 1", "amount": "amount 5", "status": "status 5", "field5 ": "field5 5" },
        { "app_ser": "51", "app_no": "app_no 6", "create_date": "create_date 1", "amount": "amount 6", "status": "status 6", "field5 ": "field5 6" },
        { "app_ser": "52", "app_no": "app_no 7", "create_date": "create_date 1", "amount": "amount 7", "status": "status 7", "field5 ": "field5 7" },
        { "app_ser": "53", "app_no": "app_no 8", "create_date": "create_date 1", "amount": "amount 8", "status": "status 8", "field5 ": "field5 8" },
        { "app_ser": "54", "app_no": "app_no 9", "create_date": "create_date 1", "amount": "amount 9", "status": "status 9", "field5 ": "field5 9" },
        { "app_ser": "55", "app_no": "app_no 10", "create_date": "create_date 1", "amount": "amount 10", "status": "status 10", "field5 ": "field5 10" },
        { "app_ser": "56", "app_no": "app_no 11", "create_date": "create_date 1", "amount": "amount 11", "status": "status 11", "field5 ": "field5 11" },
        { "app_ser": "57", "app_no": "app_no 12", "create_date": "create_date 1", "amount": "amount 12", "status": "status 12", "field5 ": "field5 12" },
        { "app_ser": "58", "app_no": "app_no 13", "create_date": "create_date 1", "amount": "amount 13", "status": "status 13", "field5 ": "field5 13" },
        { "app_ser": "59", "app_no": "app_no 14", "create_date": "create_date 1", "amount": "amount 14", "status": "status 14", "field5 ": "field5 14" },
        { "app_ser": "60", "app_no": "app_no 15", "create_date": "create_date 1", "amount": "amount 15", "status": "status 15", "field5 ": "field5 15" },
        { "app_ser": "61", "app_no": "app_no 16", "create_date": "create_date 1", "amount": "amount 16", "status": "status 16", "field5 ": "field5 16" },
        { "app_ser": "62", "app_no": "app_no 17", "create_date": "create_date 1", "amount": "amount 17", "status": "status 17", "field5 ": "field5 17" },
        { "app_ser": "63", "app_no": "app_no 18", "create_date": "create_date 1", "amount": "amount 18", "status": "status 18", "field5 ": "field5 18" },
        { "app_ser": "64", "app_no": "app_no 19", "create_date": "create_date 1", "amount": "amount 19", "status": "status 19", "field5 ": "field5 19" },
        { "app_ser": "65", "app_no": "app_no 20", "create_date": "create_date 1", "amount": "amount 20", "status": "status 20", "field5 ": "field5 20" }
    ];
    */

   
}]);



orderApp.$inject = ['$scope', '$filter', '$http'];


orderApp.directive("customSort", function () {
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



orderApp.controller('orderEditCtrl', ['$scope', '$http', function ($scope, $http) {

    $('#inputCDate').datepicker({
        format: "yyyy/mm/dd",
        language: "zh-TW",
        todayHighlight: true
    });

    $("#myFormUpdate").validationEngine();
    

    $scope.statusList = [
        { app_status: '0', name: '取消' },
        { app_status: '10', name: '貨物退回' },
        { app_status: '100', name: '處裡中' },
        { app_status: '200', name: '送貨中' },
        { app_status: '990', name: '結案' }
    ]
    //$scope.OrderAppSer = '@(ViewBag.app_ser)';
    $scope.$watch('OrderAppSer', function () {
        console.log($scope.OrderAppSer);
        
        var po = Math.random()
       
        $http({
            method: 'GET',
            url: '/Order/GetOrderItem',
            cache: false,
            params: { _po: po, app_ser: $scope.OrderAppSer },
            headers: { 'Content-Type': 'application/json' }  
        })
        .success(function (data) {

            //console.log('data.app_no: ' + data[0].app_no);
            $scope.OrderAppNo = data[0].app_no;
            $scope.OrderCreateDate = data[0].create_date;
            $scope.OrderAmount = data[0].amount;
            $scope.OrderAppStatus = data[0].app_status;
            $scope.OrderPurchaser = data[0].purchaser;
            $scope.OrderPurchaserPhone = data[0].purchaser_phone;
            $scope.OrderPurchaserAddr = data[0].purchaser_addr;
            $scope.OrderDetails = data[0].details;
        });
    });

    
    $scope.orderUpdateSubmit = function () {
        //console.log($("#myFormUpdate").validationEngine('validate'))
        if ($("#myFormUpdate").validationEngine('validate')) {
                $("#myFormUpdate").submit();
        };
    };

    // 新增明細
    $scope.createDetail = function () {
        var po = Math.random()
        $http({
            method: 'GET',
            url: '/Product/ProToActiveProductData',
            cache: false,
            params: { _po: po },
            headers: { 'Content-Type': 'application/json' }
        })
        .success(function (data) {

            //console.log('data.app_no: ' + data[0].app_no);
            $scope.ProductList = data.productList;

            $('#myModalCreate').modal('show');
        });  
    };

    $scope.orderCreateSubmit = function () {
        $("#myFormCreate").validationEngine();
        if ($("#myFormCreate").validationEngine('validate')) {
            $("#myFormCreate").submit();
        }
    };

    $scope.orderCreateHide = function () {
        $('#myModalCreate').modal('hide');
    };



    // 更新明細
    $scope.updateDetail = function (app_dser, pro_app_ser) {
        var po = Math.random()
        $http({
            method: 'GET',
            url: '/Product/ProToActiveProductData',
            cache: false,
            params: { _po: po },
            headers: { 'Content-Type': 'application/json' }
        })
        .success(function (data) {

            //console.log('app_dser: ' + app_dser);
            $scope.UpProductItem = parseInt(pro_app_ser);
            $scope.DetailAppDser = parseInt(app_dser);
            $scope.upProductList = data.productList;
            
            $('#myModalUpdate').modal('show');
        });
    };

    $scope.orderDetailUpdateSubmit = function () {
        $("#myFormDetailUpdate").submit();
    };

    $scope.orderDetailUpdateHide = function () {
        $('#myModalUpdate').modal('hide');
    };

    // 刪除明細
    $scope.deleteDetail = function (app_dser, proId, proName ) {
       
            //console.log('app_dser: ' + app_dser);
        $scope.DeleteProductItem = parseInt(app_dser);
        $scope.DeleteProductID = proId;
        $scope.DeleteProductName = proName;

        $('#myModalDelete').modal('show');
    };

    $scope.orderDeleteSubmit = function () {
        $("#myFormDetailDelete").submit();
    };

    $scope.orderDeleteHide = function () {
        $('#myModalDelete').modal('hide');
    };
}]);



orderApp.controller('orderAddCtrl', ['$scope', '$http', function ($scope, $http) {

    $scope.statusList = [
       { app_status: '0', name: '取消' },
       { app_status: '10', name: '貨物退回' },
       { app_status: '100', name: '處裡中' },
       { app_status: '200', name: '送貨中' },
       { app_status: '990', name: '結案' }
    ]

    $scope.OrderDetails = [
        { pro_app_ser: '1', productID: 'W2990421-A8271234-12', proName: 'ASUS S550CB-0051A3337U 時尚黑 750G + 24G 混碟設計', prod_price: '15300', pro_class: '硬碟' }
    ];


    $scope.addItem = function (item) {
        $scope.items.push(item);
        $scope.newItem = null;
    };

    $scope.remove = function (item) {
        var index = $scope.OrderDetails.indexOf(item)
        $scope.OrderDetails.splice(index, 1);
    };
}]);