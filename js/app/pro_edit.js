 $(document).ready(function() {
 
     
     $("#title-nav-product").addClass("active");
     $("#myFormEdit").validationEngine();

     $('#proActive').iCheck({
         checkboxClass: 'icheckbox_square-red',
         radioClass: 'iradio_square-red',
         increaseArea: '20%'
     });


     $.ajax({
         url: "/Product/GetProClassList",
         type: "GET",
         dataType: 'json',
         cache: false,
         success: function (items) {
             $.each(items.classList, function (i, val) {
                 /* console.log(" >> app_ser: " + val.app_ser);
                  console.log(" >> pro_class_no: " + val.pro_class_no);
                  console.log(" >> pro_class_name: " + val.pro_class_name);
                  console.log(" >> pro_class_desc: " + val.pro_class_desc);
                  console.log(" >> class_active: " + val.class_active);
                  */
                 //if (val.class_active == 'on') {
                     console.log(" ------------------------------- ");
                     console.log(" >> hiddenProClassId: " + $("#hiddenProClassId").val());
                     console.log(" >> app_ser: " + val.app_ser);
                     console.log(" ------------------------------- ");
                     if ($("#hiddenProClassId").val() == val.app_ser) {
                         $('#inputProductClassId').append($("<option></option>").attr("value", val.app_ser).attr("selected", "selected").text(val.pro_class_name));
                     }else{
                         $('#inputProductClassId').append($("<option></option>").attr("value", val.app_ser).text(val.pro_class_name));
                     }
                 //}
             })
         },
         error: function (xhr, ajaxOptions, thrownError) {
             console.log(xhr.status);
             console.log(thrownError);
         }
     });



     // Action on Click
     $("#btn-submit").click(function () {

         if ($("#myFormEdit").validationEngine('validate')) {
             $.isLoading(
                 {
                     text: "更新中....",
                     'class': "fa fa-refresh",
                     'tpl': '<span class="isloading-wrapper %wrapper%">%text%<i class="%class%"></i></span>',
                 });
             // Setup Loading plugin
             // $("#div-form-edit").removeClass("alert-success");

             // Re-enabling event
             setTimeout(function () {
                 $.isLoading("hide");
                 $("#myFormEdit").submit();
             }, 2000);
         }
     });
});	  



 function deleteFromSubmit() {
     $("#myDeleteForm").submit();
 }

/*		
function proDeletefn(ser) {

     console.log(" >> del app_ser: " + ser);
     if (ser && confirm('確定要刪除嗎??')) {
         window.location.href = "/Product/ProDelete?appSer=" + ser;
         // console.log("/Product/ProDelete?appSer=" + ser);
     }
 }
 */


		
		
		
		
		