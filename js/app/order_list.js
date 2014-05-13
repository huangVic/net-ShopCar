

$(document).ready(function () {

             $("#title-nav-order").addClass("active");
             $("#nav-orderlist").addClass("active");

			
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
                 sAjaxSource: "/Product/ProToJsonData",

                 aoColumns: [
                     { "mData": null },
                     { "mData": "app_ser" },
                     { "mData": "prod_no" },
                     { "mData": "prod_price" },
                     { "mData": "pro_active" }
                 ],


                 "aoColumnDefs": [
                       {
                           "sClass": "control-center",
                           bSortable: false,
                           "mRender": function (data, type, row) {
                               return '<i class="fa fa-plus-circle" onclick="openTr(this)"></i>'
                               //return '<img src="../images/sort_asc.png">'
                           },
                           "aTargets": [0]
                       }
                 ]
             });

             $('#myDataTable tbody td').on('click', 'img', function () {
                 console.log('-----------------------')
                 var nTr = $(this).parents('tr')[0];
                 if (oTable.fnIsOpen(nTr)) {
                     /* This row is already open - close it */
                     this.class = "fa fa-minus-circle";
                     oTable.fnClose(nTr);
                 }
                 else {
                     /* Open this row */
                     this.class = "fa fa-plus-circle";
                     oTable.fnOpen(nTr, fnFormatDetails(oTable, nTr), 'details');
                 }
             });
 });

function openTr(obj) {
    console.log('-----------------------')
   // console.log(obj)

    // var nTr = $(this).parents('tr')[0];
    var curRow = $(obj).closest('tr'),
        newRow = curRow.clone(true);
    console.log(newRow);
    curRow.after(newRow);
    console.log('added');
}

/* Formating function for row details */
function fnFormatDetails(oTable, nTr) {
    var aData = oTable.fnGetData(nTr);
    var sOut = '<table cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">';
    sOut += '<tr><td>Rendering engine:</td><td>' + aData[1] + ' ' + aData[4] + '</td></tr>';
    sOut += '<tr><td>Link to source:</td><td>Could provide a link here</td></tr>';
    sOut += '<tr><td>Extra info:</td><td>And any further details here (images etc)</td></tr>';
    sOut += '</table>';

    return sOut;
}
		
		
		
 function proUpdatefn(ser) {

     console.log(" >> up app_ser: " + ser);
     if (ser) {
          window.location.href = "/Product/ProEdit?appSer=" + ser;
         // console.log("/Product/ProEdit?appSer=" + ser);
     }else {
         alert('更新失敗!!');
     }
 }


 function proDeletefn(proName, proNo, ser) {
     //"' + row.prod_name + '","' + row.prod_no+ '",
     console.log(" >> del app_ser: " + ser);
     //if (ser && confirm('確定要刪除嗎??')) {
     if(ser) {
         //window.location.href = "/Product/ProDelete?appSer=" + ser;
         // console.log("/Product/ProDelete?appSer=" + ser);
        $('#label-ProName').text(proName);
         $('#label-ProNo').text(proNo);
         $('#inputAppSer').val(ser);
     } 
 }


 function deleteFromSubmit() {
      $("#myDeleteForm").submit();
 }