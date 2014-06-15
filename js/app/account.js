var accountApp = angular.module('accountApp', [], function ($locationProvider) {
    $locationProvider.html5Mode(false);
});

accountApp.config(['$httpProvider', function ($httpProvider) {
    // enable http caching
    $httpProvider.defaults.cache = false;
}])

accountApp.controller('NavCtrl', ['$scope','$location', function ($scope, $location) {  
    $scope.navClass = function (page) {
        var currentRoute = $location.absUrl().split("/")[4];
       /* console.log(currentRoute)
        console.log($location.absUrl())
        console.log($location.absUrl().split("/")[4])
       */
      
        return page === currentRoute ? 'active' : '';
    };
}]);


// 後臺帳號 controller
accountApp.controller('empListCtrl', ['$scope', '$http', function ($scope, $http) {

    var po = Math.random()
    console.log('po: ' + po);
    $http({
        method: 'GET',
        url: '/Account/GetAllEmpList',
        cache: false,
        params: { _po: po },
        //data: $.param($scope.formData),  // pass in data as strings
        headers: { 'Content-Type': 'application/json' }  // set the headers so angular passing info as form data (not request payload)
       })
       .success(function (data) {
           console.log(data.empList);

           $scope.accountist = data.empList;
           /*
            if (!data.success) {
                // if not successful, bind errors to error variables
                $scope.errorName = data.errors.name;
                $scope.errorSuperhero = data.errors.superheroAlias;
            } else {
                // if successful, bind success message to message
                $scope.message = data.message;
            }
            */
        });

    $scope.empRoles = ['管理者','一般員工'];
    $("#myFormEdit").validationEngine();


    $scope.empUpdate = function (appSer) {
        console.log('appSer: ' + appSer);
        var p1 = Math.random();
        console.log('p1: ' + p1);
        $http({
            method: 'GET',
            url: '/Account/GetEmpInfo',
            cache: false,
            params: { app_ser: appSer, _p1: p1 },
            headers: { 'Content-Type': 'application/json' }  
        })
        .success(function (account) {
            console.log(account);
            console.log(account.emp_name);
          //  $('#editempAppSer').val(account.app_ser);
            $scope.appSerUp = account.app_ser;
            $scope.empIdUp = account.emp_id;
            $scope.empNameUp = account.emp_name;
            $scope.empEmailUp = account.emp_email;
            $scope.empTelUp = account.emp_tel;
            $scope.createDateUp = account.creation_date;
            $scope.loginPass = account.login_password
           // $scope.empRole = account.emp_title;
            if (account.emp_title == '0') {
                $scope.empRole = '管理者'
            } else {
                $scope.empRole = '一般員工'
            }
            //console.log('status >>' + account.app_status);
            if (account.app_status == '100') {
                $scope.statusActive = true;
            } else {
                $scope.statusActive = false;
            }
            $('#myModalUpdate').modal('show');
         });

    };

    $scope.empDelete = function (empName,appSer) {
        console.log('empName: ' + empName);
        console.log('appSer: ' + appSer);

        $scope.empNameInfo = empName;
        $scope.appSerInfo = appSer;
        $('#myModalDelete').modal('show');
    };


    $scope.empUpdateSubmit = function () {
      
        if ($("#myFormEdit").validationEngine('validate')) {
            $("#myFormEdit").submit();
        };
    };

    $scope.empUpdateHide = function () {
       /* $scope.appSerUp = null;
        $scope.empNameUp = null;
        $scope.empEmailUp = null;
        $scope.empTelUp = null;
        $scope.createDateUp = null;
        $scope.loginPass = null;
        $scope.empRole = null;
        $scope.statusActive = null;
        */
        $('#myModalUpdate').validationEngine('hideAll');
        $('#myModalUpdate').modal('hide');
    }
}]);
		
		
// 後臺管理者帳號新增 controller
accountApp.controller('empAddCtrl', ['$scope', '$http', function ($scope, $http) {


    $scope.empRoles = ['管理者', '一般員工'];

    $("#myFormAdd").validationEngine();

    $scope.empCreateSubmit = function () {
        
        if ($("#myFormAdd").validationEngine('validate')) {
            $("#myFormAdd").submit();
        } 
    };


    $scope.toEmplist = function () {
        window.location.href = "/Account/EmpList"
    };

}]);


// 會員資料 controller
accountApp.controller('memberListCtrl', ['$scope', '$http', function ($scope, $http) {

    $('#editemBirthDay').datepicker({
        format: "yyyy/mm/dd",
        language: "zh-TW",
        todayHighlight: true
    });

    var po = Math.random()
    console.log('po: ' + po);
    $http({
        method: 'GET',
        url: '/Member/MemberToJsonData',
        cache: false,
        params: { _po: po },
        //data: $.param($scope.formData),  // pass in data as strings
        headers: { 'Content-Type': 'application/json' }  // set the headers so angular passing info as form data (not request payload)
    })
       .success(function (data) {
           console.log(data.memberItem);
           $scope.accountist = data.memberItem;
       });

    //$scope.empRoles = ['管理者', '一般員工'];
    $("#myFormEdit").validationEngine();
    

    $scope.empUpdate = function (appSer) {
        var p1 = Math.random();
        $http({
            method: 'GET',
            url: '/Member/MemberEdit',
            cache: false,
            params: { appSer: appSer, _p1: p1 },
            headers: { 'Content-Type': 'application/json' }
        })
        .success(function (account) {
            console.log(account);

            $scope.appSerUp = account.appSer;
            $scope.userIdUp = account.user_id;
            $scope.userNameUp = account.user_name;
            $scope.userPasswordUp = account.user_password;
            $scope.birthDayUp = account.birthDay;
            $scope.maleUp = account.male;
            $scope.mobileUp = account.mobile;
            $scope.telUp = account.tel;
            $scope.extnoUp = account.extno;
            $scope.addressUp = account.address;
            $scope.emailUp = account.email;
         
            $('#myModalUpdate').modal('show');
        });

        
    };

    $scope.empDelete = function (userName, userId, appSer) {

        $scope.userNameInfo = userName;
        $scope.userIdInfo = userId;
        $scope.appSerInfo = appSer;
        $('#myModalDelete').modal('show');
    };


    $scope.empUpdateSubmit = function () {

        if ($("#myFormEdit").validationEngine('validate')) {
            $("#myFormEdit").submit();
        };
    };

    $scope.empUpdateHide = function () {
        /* $scope.appSerUp = null;
         $scope.empNameUp = null;
         $scope.empEmailUp = null;
         $scope.empTelUp = null;
         $scope.createDateUp = null;
         $scope.loginPass = null;
         $scope.empRole = null;
         $scope.statusActive = null;
         */
        $('#myModalUpdate').validationEngine('hideAll');
        $('#myModalUpdate').modal('hide');
    }
}]);