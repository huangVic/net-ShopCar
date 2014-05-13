/* global $, window */

$(function () {
    'use strict';

    // Initialize the jQuery File Upload widget:
    /*
    $('#fileupload').fileupload({
        // Uncomment the following to send cross-domain cookies:
        //xhrFields: {withCredentials: true},
        url: 'server/php/1',
        maxNumberOfFiles:3,
    });
    */

    // Enable iframe cross-domain access via redirect option:
    /*
    $('#fileupload').fileupload(
        'option',
        'redirect',
        window.location.href.replace(
            /\/[^\/]*$/,
            '/cors/result.html?%s'
        )
    );
    */

    console.log(" >> optionUrl: " + "@Url.Action('UploadFiles')");


    var formDatax = $('#fileupload').find(':input').serialize();
    console.log(" >> formData: " + formDatax);

    $('#fileupload').fileupload({
        url: "/Product/UploadFiles",
        dataType: 'json',
        cache: false,
        acceptFileTypes: /(\.|\/)(gif|jpe?g|png)$/i,
       /* disableImageResize: /Android(?!.*Chrome)|Opera/
        .test(window.navigator && navigator.userAgent),
        imageMaxWidth: 50,
        imageMaxHeight: 50,
        imageCrop: true, // Force cropped images
        */
        maxNumberOfFiles: 5,
        previewMaxWidth: 50,
        previewMaxHeight: 50
    });

    $('#fileupload').fileupload('option', {
        maxFileSize: 2000000,
        resizeMaxWidth: 1024,
        resizeMaxHeight: 768,
        previewMaxWidth: 50,
        previewMaxHeight: 50
    });

    console.log(" >> hostname: " + window.location.hostname)


    
    $.ajax({
        // Uncomment the following to send cross-domain cookies:
        //xhrFields: {withCredentials: true},
        url: "/Product/UploadFiles?appSer=" + $('#appSer').val() ,
        dataType: 'json',
        cache: false,
        context: $('#fileupload')[0],
    }).always(function () {
        $(this).removeClass('fileupload-processing');
    }).done(function (result) {
        $(this).fileupload('option', 'done')
            .call(this, $.Event('done'), { result: result });

    });
    


    
    $('#fileupload').bind('fileuploadsubmit', function (e, data) {
        // The example input, doesn't have to be part of the upload form:
        /*
        data.formData = { 
            proNo: $('#inputProductionNo').val(),
            proName: $('#inputProductionName').val(),
            proPrice: $('#inputProductPrice').val(),
            proSpecialPrice: $('#inputProductSpecialPrice').val(),
            proClassId: $('#inputProductClassId').val(),
            prodFeature: $('#inputProductFeature').val(),
            prodDesc: $('#inputProductDesc').val()
        },
        */
        data.formData = { appSer: $('#appSer').val() },
        console.log(" >> formData: " + data.formData.appSer)
        if (!data.formData.appSer) {
            data.context.find('button').prop('disabled', false);
            alert("此產品不存在")
            return false;
        }
    });
    



    /*
    if (window.location.hostname === 'blueimp.github.io') {

        console.log("=====================================>")
        // Demo settings:
        $('#fileupload').fileupload('option', {
            url: '//jquery-file-upload.appspot.com/',
            // Enable image resizing, except for Android and Opera,
            // which actually support image resizing, but fail to
            // send Blob objects via XHR requests:
            disableImageResize: /Android(?!.*Chrome)|Opera/
                .test(window.navigator.userAgent),
            maxFileSize: 2000000,
            acceptFileTypes: /(\.|\/)(gif|jpe?g|png)$/i,
            resizeMaxWidth: 1920,
            resizeMaxHeight: 1200
        });
        // Upload server status check for browsers with CORS support:
        if ($.support.cors) {
            $.ajax({
                url: '//jquery-file-upload.appspot.com/',
                type: 'HEAD'
            }).fail(function () {
                $('<div class="alert alert-danger"/>')
                    .text('Upload server currently unavailable - ' +
                            new Date())
                    .appendTo('#fileupload');
            });
        }
    } else {
        // Load existing files:

        console.log("------------------------------->")
        $('#fileupload').addClass('fileupload-processing');
        $.ajax({
            // Uncomment the following to send cross-domain cookies:
            //xhrFields: {withCredentials: true},
            url: $('#fileupload').fileupload('option', 'url'),
            dataType: 'json',
            context: $('#fileupload')[0]
        }).always(function () {
            $(this).removeClass('fileupload-processing');
        }).done(function (result) {
            $(this).fileupload('option', 'done')
                .call(this, $.Event('done'), {result: result});
        });
    }
    */

});


function deleteFile(url, obj) {
    //alert(id);
    console.log(" >> file_delete << ")
    console.log(" >> this << " + obj.tagName)
    var row = obj.parentNode.parentNode;
    $.getJSON(url, function (data) {
        $.each(data.file, function (key, val) {
            console.log(" >> data.file.sussess << " + val.sussess)
            console.log(" >> data.file.message << " + val.message)
            if (val.sussess === 'true') {
                $(row).remove();
            } else {
                alert('檔案刪除失敗!!')
            }
        });

        //console.log(" >> row << " + row.tagName)
    });
    return false;
};
