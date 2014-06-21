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
        whitelist:'gif|png|jpg|jpeg'
        //blacklist:'exe|php'
        //onchange:''
        //
    });
    
    $("input[type='radio']").change(function () {

        if ($(this).val() == "flexOther") {
            $("#otherInputText").show();
        } else {
            $("#otherInputText").hide();
        }

    });
});