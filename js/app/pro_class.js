 $(document).ready(function() {
 
     
             $("#title-nav-product").addClass("active");
             $("#nav-class").addClass("active");

			
             $('#editClassActive').iCheck({
                 checkboxClass: 'icheckbox_square-red',
                 radioClass: 'iradio_square-red',
                 increaseArea: '20%'
             });

     // **************************************//
     // ********* DateTable Init *************//
     // **************************************//
             var oTable = $('#myDataTable').dataTable({
                 "sDom": "<'row'<'col-lg-6'f><'col-lg-6'p>>rt<'row'<'col-lg-6'i>>",
                 "sPaginationType": "bootstrap", //bootstrap , two_button, full_numbers
                 "iDisplayLength": 10,
                 "bPaginate": true,
                 "bProcessing": false,
                 "bLengthChange": false,
                 "bFilter": true,
                 "bInfo": true, // 筆數
                 "bSort": true,
                 "bSortClasses": true,
                 "oLanguage": {
                     "sLengthMenu": "每頁顯示  _MENU_  筆",
                     "sInfoFiltered": "  ( 總計  _MAX_ 筆 )",
                     "sInfo": "目前顯示  _START_ 至  _END_ 共  _TOTAL_ 筆",
                     "sZeroRecords": "無符合資料",
                     "oPaginate": {
                         "sPrevious": "上頁",
                         "sNext": "下頁"
                     },
                     "sSearch": '搜尋'
                 },
                 sAjaxSource: "/Product/GetProClassList",
                 "sAjaxDataProp": "classList",
                 aoColumns: [
                     { "mData": "app_ser" },
                     { "mData": "pro_class_no" },
                     { "mData": "pro_class_name" },
                     { "mData": "class_active" },
                     { "mData": null }
                     //{ "mData": ""}
                    // { "mData": "selesMan" }
                 ],


                 "aoColumnDefs": [
                     {
                         "sClass": "control-center",
                         "aTargets": [0]
                     },
                      {
                          "sClass": "control-center",
                          "mRender": function (data, type, row) {
                              var str = "關閉"
                              if (data == 'on') {
                                  str = "啟用"
                              }
                              return str
                          },
                          "aTargets": [3]
                      },
                     {
                         "sClass": "control-center",
                         "mRender": function (data, type, row) {
                             var btnStr = '<button type="button" class="btn btn-sm btn-default btn-proUpdate" onclick="proUpdatefn(' + row.app_ser + ')"><i class="fa fa-edit"></i> 編輯</button>';
                             btnStr += '&nbsp;&nbsp;<button type="button" class="btn btn-sm btn-danger btn-proDelete"  onclick="proDeletefn(\'' + row.pro_class_name + '\',\'' + row.pro_class_no + '\',' + row.app_ser + ')"><i class="glyphicon glyphicon-trash"></i> 刪除</button>';
                             return btnStr
                         },
                         "aTargets": [4]
                     }
                 ]

             });

});	  
		
		
		
	


 function proUpdatefn(ser) {

     console.log(" >> up app_ser: " + ser);
     //console.log($('#editForm input').serialize());
     $.ajax({
         url: "/Product/GetProClassItem",
         //data: 'app_ser=' + ser,
         data: { app_ser: ser },
         type: "GET",
         cache:false,
         dataType: 'json',

         success: function (items) {
             $.each(items.classItem, function (i, val) {
                 console.log(" >> app_ser: " + val.app_ser);
                 console.log(" >> pro_class_no: " + val.pro_class_no);
                 console.log(" >> pro_class_name: " + val.pro_class_name);
                 console.log(" >> pro_class_desc: " + val.pro_class_desc);
                 console.log(" >> class_active: " + val.class_active);
                 $('#editAppSer').val(val.app_ser);
                 $('#editProClassNo').val(val.pro_class_no);
                 $('#editProClassName').val(val.pro_class_name);
                 $('#editProClassDesc').val(val.pro_class_desc);
                 if (val.class_active == 'on') {
                     $('#editClassActive').iCheck('check');
                 } else {
                     // $('#editClassActive').removeAttr("checked");
                     $('#editClassActive').iCheck('uncheck');
                 }
                
             });
             $('#myModal').modal('show');
         },

         error: function (xhr, ajaxOptions, thrownError) {
             console.log(xhr.status);
             console.log(thrownError);
         }
     });

 }


 function proDeletefn(className, classNo, ser) {
     //"' + row.prod_name + '","' + row.prod_no+ '",
     console.log(" >> del app_ser: " + ser);
     //if (ser && confirm('確定要刪除嗎??')) {
     if (ser) {
         //window.location.href = "/Product/ProDelete?appSer=" + ser;
         // console.log("/Product/ProDelete?appSer=" + ser);

         $.ajax({
             url: "/Product/ClassExistProduct",
             //data: 'app_ser=' + ser,
             data: { class_appser: ser },
             type: "GET",
             cache: false,
             dataType: 'json',

             success: function (data) {
                 if (data.message == 'OK') {
                     $('#del-label-className').text(className);
                     $('#del-label-classNo').text(classNo);
                     $('#del-input-AppSer').val(ser);

                     $('#myModalDelet').modal('show');
                 } else {
                     var html = ''
                     html += '<div class="alert alert-warning alert-danger" style="">'
                     html += '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>'
                     html += '<strong>禁止刪除!!&nbsp;</strong> 產品分類: ' + className + ' 已被使用, 將無法刪除....'
                     html += '</div>'
                     $('#errorMsg').html(html);
                 }

             },
             error: function (xhr, ajaxOptions, thrownError) {
                 console.log(xhr.status);
                 console.log(thrownError);
             }
         });
     }
 }


 function deleteFromSubmit() {
     $("#myDeleteForm").submit();
 }
		