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

    //Close the alerts if the user clicks in the 'x'
    $('.alert .close').on('click', function (e) {
        $(this).parent().hide();
    });

    //Header in widgets
    $.widget("ui.dialog", $.extend({}, $.ui.dialog.prototype, {
        _title: function (title) {
            var $title = this.options.title || '&nbsp;';
            if (("title_html" in this.options) && this.options.title_html == true)
                title.html($title);
            else title.text($title);
        }
    }));

    //Modal to add emails to notify
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

    //handling tabs and loading/displaying relevant messages and forms
    //not needed if using the alternative view, as described in docs
    var prevTab = 'inbox';
    $('#inbox-tabs a[data-toggle="tab"]').on('show.bs.tab', function (e) {
        var currentTab = $(e.target).data('target');
        if (currentTab == 'write') {
            Inbox.show_form();
        } else {
            if (prevTab == 'write')
                Inbox.show_list();

            //load and display the relevant messages 
        }
        prevTab = currentTab;
    });



    //basic initializations
    $('.message-list .message-item input[type=checkbox]').removeAttr('checked');
    $('.message-list').delegate('.message-item input[type=checkbox]', 'click', function () {
        $(this).closest('.message-item').toggleClass('selected');
        if (this.checked) Inbox.display_bar(1);//display action toolbar when a message is selected
        else {
            Inbox.display_bar($('.message-list input[type=checkbox]:checked').length);
            //determine number of selected messages and display/hide action toolbar accordingly
        }
    });


    //check/uncheck all messages
    $('#id-toggle-all').removeAttr('checked').on('click', function () {
        if (this.checked) {
            Inbox.select_all();
        } else Inbox.select_none();
    });

    //select all
    $('#id-select-message-all').on('click', function (e) {
        e.preventDefault();
        Inbox.select_all();
    });

    //select none
    $('#id-select-message-none').on('click', function (e) {
        e.preventDefault();
        Inbox.select_none();
    });

    //select read
    $('#id-select-message-read').on('click', function (e) {
        e.preventDefault();
        Inbox.select_read();
    });

    //select unread
    $('#id-select-message-unread').on('click', function (e) {
        e.preventDefault();
        Inbox.select_unread();
    });

    /////////

    //display first message in a new area
    $('.message-list .message-item:eq(0) .text').on('click', function () {
        //show the loading icon
        $('.message-container').append('<div class="message-loading-overlay"><i class="icon-spin icon-spinner orange2 bigger-160"></i></div>');

        $('.message-inline-open').removeClass('message-inline-open').find('.message-content').remove();

        var message_list = $(this).closest('.message-list');

        //some waiting
        setTimeout(function () {

            //hide everything that is after .message-list (which is either .message-content or .message-form)
            message_list.next().addClass('hide');
            $('.message-container').find('.message-loading-overlay').remove();

            //close and remove the inline opened message if any!

            //hide all navbars
            $('.message-navbar').addClass('hide');
            //now show the navbar for single message item
            $('#id-message-item-navbar').removeClass('hide');

            //hide all footers
            $('.message-footer').addClass('hide');
            //now show the alternative footer
            $('.message-footer-style2').removeClass('hide');


            //move .message-content next to .message-list and hide .message-list
            message_list.addClass('hide').after($('.message-content')).next().removeClass('hide');

            //add scrollbars to .message-body
            $('.message-content .message-body').slimScroll({
                height: 200,
                railVisible: true
            });

        }, 500 + parseInt(Math.random() * 500));
    });


    //back to message list
    $('.btn-back-message-list').on('click', function (e) {
        e.preventDefault();
        Inbox.show_list();
        $('#inbox-tabs a[data-target="inbox"]').tab('show');
    });

    var Inbox = {
        //displays a toolbar according to the number of selected messages
        display_bar: function (count) {
            if (count == 0) {
                $('#id-toggle-all').removeAttr('checked');
                $('#id-message-list-navbar .message-toolbar').addClass('hide');
                $('#id-message-list-navbar .message-infobar').removeClass('hide');
            } else {
                $('#id-message-list-navbar .message-infobar').addClass('hide');
                $('#id-message-list-navbar .message-toolbar').removeClass('hide');
            }
        },
        select_all: function () {
            var count = 0;
            $('.message-item input[type=checkbox]').each(function () {
                this.checked = true;
                $(this).closest('.message-item').addClass('selected');
                count++;
            });

            $('#id-toggle-all').get(0).checked = true;

            Inbox.display_bar(count);
        },
        select_none: function () {
            $('.message-item input[type=checkbox]').removeAttr('checked').closest('.message-item').removeClass('selected');
            $('#id-toggle-all').get(0).checked = false;

            Inbox.display_bar(0);
        },
        select_read: function () {
            $('.message-unread input[type=checkbox]').removeAttr('checked').closest('.message-item').removeClass('selected');

            var count = 0;
            $('.message-item:not(.message-unread) input[type=checkbox]').each(function () {
                this.checked = true;
                $(this).closest('.message-item').addClass('selected');
                count++;
            });
            Inbox.display_bar(count);
        },
        select_unread: function () {
            $('.message-item:not(.message-unread) input[type=checkbox]').removeAttr('checked').closest('.message-item').removeClass('selected');

            var count = 0;
            $('.message-unread input[type=checkbox]').each(function () {
                this.checked = true;
                $(this).closest('.message-item').addClass('selected');
                count++;
            });

            Inbox.display_bar(count);
        }
    };

    //show message list (back from writing mail or reading a message)
    Inbox.show_list = function () {
        $('.message-navbar').addClass('hide');
        $('#id-message-list-navbar').removeClass('hide');

        $('.message-footer').addClass('hide');
        $('.message-footer:not(.message-footer-style2)').removeClass('hide');

        $('.message-list').removeClass('hide').next().addClass('hide');
        //hide the message item / new message window and go back to list
    };

    //show write mail form
    Inbox.show_form = function () {
        if ($('.message-form').is(':visible')) return;
        if (!form_initialized) {
            initialize_form();
        }

        var message = $('.message-list');
        $('.message-container').append('<div class="message-loading-overlay"><i class="icon-spin icon-spinner orange2 bigger-160"></i></div>');

        setTimeout(function () {
            message.next().addClass('hide');

            $('.message-container').find('.message-loading-overlay').remove();

            $('.message-list').addClass('hide');
            $('.message-footer').addClass('hide');
            $('.message-form').removeClass('hide').insertAfter('.message-list');

            $('.message-navbar').addClass('hide');
            $('#id-message-new-navbar').removeClass('hide');


            //reset form??
            $('.message-form .wysiwyg-editor').empty();

            $('.message-form .ace-file-input').closest('.file-input-container:not(:first-child)').remove();
            $('.message-form input[type=file]').ace_file_input('reset_input');

            $('.message-form').get(0).reset();

        }, 300 + parseInt(Math.random() * 300));
    };

    $('#modal-wizard .modal-header').ace_wizard();
    $('#modal-wizard .wizard-actions .btn[data-dismiss=modal]').removeAttr('disabled');


    //  time picker
    $('#timepickerCheckOut').timepicker({
        minuteStep: 1,
        showSeconds: true,
        showMeridian: false
    }).next().on(ace.click_event, function () {
        $(this).prev().focus();
    });

    $('#timepickerCheckIn').timepicker({
        minuteStep: 1,
        showSeconds: true,
        showMeridian: false
    }).next().on(ace.click_event, function () {
        $(this).prev().focus();
    });
    //  ./time picker

    var form_initialized = false;
    function initialize_form() {
        if (form_initialized) return;
        form_initialized = true;
    }; //initialize_form
});