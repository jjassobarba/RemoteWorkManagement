$(document).ready(function () {
    //Create the tabs for the user page
    $("#tabs").tabs();

    //Create the image uploader zone for the user
    $("#uploadImageArea").ace_file_input({
        style: 'well',
        btn_choose: 'Drop image here or click to choose',
        btn_change: null,
        no_icon: 'icon-picture',
        droppable: true,
        thumbnail: 'small',
        preview_error: function (filename, error_code) {
            //name of the file that failed
            //error_code values
            //1 = 'FILE_LOAD_FAILED',
            //2 = 'IMAGE_LOAD_FAILED',
            //3 = 'THUMBNAIL_FAILED'
            //alert(error_code);
        }
    }).on('change', function () {
        //console.log($(this).data('ace_input_files'));
        //console.log($(this).data('ace_input_method'));
    });

    //Create the uploader for the button in the user page
    $('#uploadImageButton').ace_file_input({
        no_file: 'No File ...',
        btn_choose: 'Choose',
        btn_change: 'Change',
        droppable: false,
        onchange: null,
        thumbnail: true,
        whitelist: 'gif|png|jpg|jpeg'
        //blacklist:'exe|php'
        //onchange:''
        //
    });
    
    $.widget("ui.dialog", $.extend({}, $.ui.dialog.prototype, {
        _title: function (title) {
            var $title = this.options.title || '&nbsp;';
            if (("title_html" in this.options) && this.options.title_html == true)
                title.html($title);
            else title.text($title);
        }
    }));

    $("#addEmailButton").on('click', function (e) {
        e.preventDefault();

        $("#email-dialog").removeClass('hide').dialog({
            modal: true,
            title: "<div class='widget-header widget-header-small'><h4 class='small'><i class='icon-ok'></i>Email Notifications</h4></div>",
            title_html: true,
            width: 350,
            buttons: [
                {
                    text: "Cancel",
                    "class": "btn btn-xs",
                    click: function () {
                        $(this).dialog("close");
                    }
                },
                {
                    text: "OK",
                    "class": "btn btn-primary btn-xs",
                    click: function () {
                        $(this).dialog("close");
                    }
                }
            ]
        });
    });
    

});