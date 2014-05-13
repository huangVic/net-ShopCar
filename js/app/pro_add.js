 $(document).ready(function() {
 
     
             $("#title-nav-product").addClass("active");
             $("#nav-add").addClass("active");

             $('#proActive').iCheck({
                 checkboxClass: 'icheckbox_square-red',
                 radioClass: 'iradio_square-red',
                 increaseArea: '20%'
             });

             $("#myfomeAdd").validationEngine();
     // Action on Click
             $("#btn-submit").click(function () {
                 if ($("#myfomeAdd").validationEngine('validate')) {
                     $("#myfomeAdd").submit();
                 }
             });


             $.ajax({
                 url: "/Product/GetProClassList",
                 type: "GET",
                 cache: false,
                 dataType: 'json',
                 success: function (items) {
                     $.each(items.classList, function (i, val) {
                         console.log(" >> app_ser: " + val.app_ser);
                         console.log(" >> pro_class_no: " + val.pro_class_no);
                         console.log(" >> pro_class_name: " + val.pro_class_name);
                         console.log(" >> pro_class_desc: " + val.pro_class_desc);
                         console.log(" >> class_active: " + val.class_active);
                         
                         if (val.class_active == 'on') {
                             $('#inputProductClassId')
                                 .append($("<option></option>")
                                 .attr("value", val.app_ser)
                                 .text(val.pro_class_name));
                         }
                     })
                 },
                 error: function (xhr, ajaxOptions, thrownError) {
                             console.log(xhr.status);
                             console.log(thrownError);
                         }
                 });
});	  
		
		
 function toProList() {
         window.location.href = "/Product/ProList"
 }
		
		
		
		
		