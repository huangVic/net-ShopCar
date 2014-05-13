 $(document).ready(function() {

             $("#title-nav-product").addClass("active");
             $("#nav-list").addClass("active");

			// **************************************//
			// ********* DateTime Picker ************//
			// **************************************//
			/*
			$('#datetimepicker1').datepicker({
				format: "dd",
				language: "zh-TW",
				todayHighlight: true
			});
			
			$('#datetimepicker2').datepicker({
				format: "dd",
				language: "zh-TW",
				todayHighlight: true
			});
			*/
			
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
                    { "mData": "app_ser" },
			        { "mData": "prod_no"},
			        { "mData": "prod_name"},
			        { "mData": "prod_price" },
			        { "mData": "prod_special_price" },
			        { "mData": "prod_class" },
                    { "mData": "pro_active" },
                    { "mData": null }
			        //{ "mData": ""}
			       // { "mData": "selesMan" }
			    ],
				
				
				"aoColumnDefs": [
                    {
                        "sClass": "control-center",
                        "aTargets": [0,1,5]
                    },
                     {
                         "sClass": "control-right",
                         "aTargets": [3,4]
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
                        "aTargets": [6]
                    },
						{
						    "sClass": "control-center",
						    bSortable: false,
						    "mRender": function ( data, type, row ) {
							
						       /* $('.btn-proUpdate').click(function () {
										var btn = $(this)
										console.log(btn.attr( "data-text" ));
										window.location.href = "/Product/ProEdit?appSer=" + btn.attr( "data-text" ) ; 
						        });

						        $('.btn-proDelete').click(function () {
						            var btnx = $(this)
						            console.log(btnx.attr("data-text"));
						          
						                // window.location.href = "/Product/ProDelete?appSer=" + btnx.attr("data-text");row.app_ser
						                console.log("xx");
						        });
								*/
						        //var xx = '\'' + ;

						        var btnStr = '<button type="button" class="btn btn-sm btn-default btn-proUpdate" onclick="proUpdatefn(' + row.app_ser + ')"><i class="fa fa-edit"></i> 編輯</button>';
						        btnStr += '&nbsp;&nbsp;<button type="button" class="btn btn-sm btn-danger btn-proDelete" data-toggle="modal" data-target="#myModal" onclick="proDeletefn(\'' + row.prod_name + '\',\'' + row.prod_no + '\',' + row.app_ser + ')"><i class="glyphicon glyphicon-trash"></i> 刪除</button>';
						        return btnStr
							},
						   "aTargets": [7]
						}
				]
				
				/*
				"fnRowCallback": function (nRow, aData, iDisplayIndex) {
                    $(nRow).on("click", function (event) { 
                        if ($(this).hasClass('row_selected')) {
                            $(this).removeClass('row_selected');
                        }
                        else {
                            oTable.$('tr.row_selected').removeClass('row_selected');
                            $(this).addClass('row_selected');
							var xx = $(this).find('td').eq(0).text().trim();
							console.log(xx);
							viewCusDetail(xx); // 切換到瀏覽頁面,載入客戶基本資料
                        }
                    });
				}
				*/							
			});
			 

			 /*
			$("#myDtFilter").keyup( function () {
			     oTable.fnFilter( $("#myDtFilter").val());
			});
			*/ 
			
			 
			 /*
			oTable.$('td').hover( function() {
				var iCol = $('td', this.parentNode).index(this) % 2;
				$('td:nth-child('+(iCol+1)+')', oTable.$('tr')).addClass( 'highlighted' );
			}, function() {
				oTable.$('td.highlighted').removeClass('ex_highlight');
			} );
			*/
		});	  
		
		
		
		
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