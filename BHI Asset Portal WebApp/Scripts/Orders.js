


function showSetSuccess(item) {
    $(item).parent().css("border-color", "green");
    $(item).parent().css("border-style", "solid");
    $(item).closest('.CreativeSet').find('.SetSuccessMessage').show();
}
function hideSetSuccess(item) {

    $(item).parent().css("border-style", "none");
    $(item).closest('.CreativeSet').find('.SetSuccessMessage').hide();

}

function sendEmailToAm(OrderID, item, AgentName, AgentEmail) {
    spinnerReplace(item);
    $.post('/Orders/SendEmailToAM', { OrderID: OrderID, AgentName: AgentName, AgentEmail: AgentEmail })
        .done(function (ret) {
            $(item).text("Submit to Account Manager Last Submitted: " + ret.response )
            
            spinnerReplace(item);
        });
    createNewOrder();
}

function createNewOrder() {

    $.post('/Orders/UpsertOrder', {
        order: '{ "orderName":"BDX-CPM_FBX_RDC_CAPSTONE_AZ_2020 - JAN - JUN  -  A130912",      "workOrderNumber":"A130912",      "customer":"Capstone Homes",      "contractStartDate":"1/1/2020",      "contractEndDate":"6/ 30 / 2020"}',
        secret: 'n[(pfCbWy=iD9wO'
    })
        .done(function (ret) {
           

            
        });


}

function spinnerReplace(button) {
    $(button).toggle();
    $(button).next().toggle();
}
function calculateValidLineItems()
{
    var count = 0;
    $('.LineItem-CreativeSets').each(function () {
        $lineItem = $(this);
        if ($(this).find('form.CreativeSetForm').toArray().some(function (item) {
            return $(item).validate().checkForm();
        })) {
            count++;
            $lineItem.parent('.panel').find('.valid-check').show();
            $lineItem.parent('.panel').find('.valid-x').hide();
        }
        else {
            $lineItem.parent('.panel').find('.valid-check').hide();
            $lineItem.parent('.panel').find('.valid-x').show();
        }
    });
    $('#valid-items').text(count);
    console.log(count);
    if (count > 0) {
        $('#SendToAMBttn').prop('disabled', false);
    }
    else {
        $('#SendToAMBttn').prop('disabled', true);
    }

}
function fillDropDowns(type, list) {
    $('.AdTypeText[value="' + type + '"]').closest('form').each(function () {
        console.log(type);
        var id = $(this).find('.CreativeSetID').val();
        var $dropdown = $(this).find('.setsDropDown');
        $.each(list, function () {
            $dropdown.append($("<option />").val(this.CreativeSetID).text(this.SetName));
        });
        $dropdown.find('option[value="' + id + '"]').remove();
        
        if (($dropdown).find('option').length == 0) {
            
            $dropdown.parent().parent().hide();
        }
        else {
            $dropdown.parent().parent().show();

        }

        
    
    });
}

function fillCopyDropDowns() {
    $('.setsDropDown > option').each(function () {
        $(this).remove();
    });
    var ID = $('#ID').val();
    $.get('/CreativeSets/GetCopyLists?OrderID=' + ID).done(function (retValue) {
        console.log('filling drop downs');
        console.log(retValue);
        fillDropDowns('BannerAds', retValue.BannerAds);
        fillDropDowns('NativeAds', retValue.NativeAds);
        fillDropDowns('SingleAds', retValue.SingleAds);
        fillDropDowns('PremiumAds', retValue.PremiumAds)
        fillDropDowns("PremiumAds", retValue.BannerAds);
        fillDropDowns("PremiumAds", retValue.NativeAds);
        fillDropDowns('BannerAds', retValue.PremiumAds);
        fillDropDowns("NativeAds", retValue.PremiumAds);

        
    });

}

function copyAssets(button) {
    spinnerReplace(button);

    var $dropDown = $(button).closest('form').find('.setsDropDown');
    var copyFromID = $dropDown.children("option:selected").first().val();
    var $copyToForm = $(button).closest('form');
    var copyToID = $copyToForm.attr('creativeSetID');
    var $copyFromForm = $('form[creativeSetID="' + copyFromID + '"]');
    var copyToColumns = []
    var copyFromColumns = [];
    $copyFromForm.find('.ImageUpload').each(function () {
        var currentSize = $(this).attr('creativeSize');
        if ($copyToForm.find('.ImageUpload[creativeSize="' + currentSize + '"]').length > 0) {
            copyFromColumns.push($(this).attr('columnName'));
            copyToColumns.push($copyToForm.find('.ImageUpload[creativeSize="' + currentSize + '"]').attr('columnName'));
        }
    });

    $.post('/CreativeSets/CopySet', { OriginalSetID: copyFromID, TargetSetID: copyToID, OriginalColumns: copyFromColumns, TargetColumns: copyToColumns }).done(function (data) {
        console.log(data.items);
        data.items.forEach(function (item) {
            console.log(item.ColumnName);

            var dropzone = $copyToForm.find('.ImageUpload[columnName="' + item.ColumnName + '"]')[0].dropzone;
          //  dropzone.files[0].previewElement.remove(); 

            var mockFile = { name: "Filename", size: 1 };
            // Call the default addedfile event handler
            dropzone.emit("addedfile", mockFile);

            // And optionally show the thumbnail of the file:
            //change this to be dynamic'
            dropzone.createThumbnailFromUrl(mockFile, item.URL, null, true);
            dropzone.files.push(mockFile);
            dropzone.emit("complete", mockFile);
            console.log($copyToForm.find('.ImageUpload[columnName="' + item.ColumnName + '"]').siblings('.hidden-image-url').children());
            $($copyToForm).find('.ImageUpload[columnName="' + item.ColumnName + '"]').siblings('.hidden-image-url').children().val('filled');

        });
        if ($copyToForm.validate().checkForm()) {
            console.log('valid');
            showSetSuccess($copyToForm);
            calculateValidLineItems();
        }
        spinnerReplace(button);

        
    });

}



function fillDown(item) {
    var currentText = $(item).val();
    $form = $(item).closest('.form-horizontal');
    $form.find('input[type="url"]').each(function () {
        if ($(this).val() == '') {
            $(this).val(currentText);
            $(this).blur();
        }
    });

}


$(document).on('blur', 'input[type=text], input[type=url], textarea', function () {
     var columnName = $(this).attr('name');
   

    if ($(this).valid()) {
        var creativeSetID = $(this).closest('.form-horizontal').find('input.CreativeSetID').val();
        if (creativeSetID != null) {

            var thisInput = this;
            $(this).css('border-color', "green");
            console.log('checking valid');
            if ($(this).closest('form').validate().checkForm()) {
                console.log('valid');
                showSetSuccess($(this).closest('form'));
                calculateValidLineItems();
            }
            $.post('/CreativeSets/SaveColumn', { CreativeSetID: creativeSetID, ColumnName: columnName, NewValue: $(this).val() })
                .done(function () {
                    $('<span class="success" style="color:green; margin-left:-55px;">Saved!</span>').insertAfter(thisInput).animate({ opacity: 0 }, 5000, function () {
                        $(this).remove();
                    });
                    if (columnName === 'SetName') {
                        fillCopyDropDowns();

                    }
                });

        }
        else {
            var orderNumber = $(this).closest('.panel').find("#ID").val();
            $(this).css('border-color', "green");
            $.post('/Orders/SaveOrderData', { OrderID: orderNumber, ColumnName: columnName, NewValue: $(this).val() })
                .done(function () {
                    $('<p class="success" style="color:green">Saved!</p>').insertAfter(thisInput).animate({ opacity: 0 }, 5000, function () {
                        $(this).remove();
                    });
                    
                });
        }
    }
    else {
        $(this).css('border-color', "red");
        //remove the success marker if it's not valid any more, but only validate if it once was valid. Do not always validate the whole form
        if ($(this).closest('form').find('.SetSuccessMessage').is(":visible")) {
            $(this).closest('form').validate();
            if (!$(this).closest('form').validate().checkForm()) {
                console.log('not valid)');
                hideSetSuccess($(this).closest('form'));
                calculateValidLineItems();
            }
        }
    }

});

function makeDrapAndDrop() {

    var dragged;

    $('.ImageUpload').on('drop dragdrop', function (event) {
        console.log(event);
        console.log(dragged);
        var url = $(dragged).attr('src');

        var column = $(event.currentTarget).attr('columnname');
        var creativeSetID = $(event.currentTarget).attr('creativesetid');
        var width = $(event.currentTarget).attr('iWidth');
        var height = $(event.currentTarget).attr('iHeight');
        var fileName = $(dragged).attr('Name');
        console.log(url);
        console.log(column)
        if (url != undefined && column != undefined) {
            var dropzone = $(event.currentTarget)[0].dropzone;
            var mockFile = { name: "Filename", size: 1 };
            // Call the default addedfile event handler
            dropzone.emit("addedfile", mockFile);

            // And optionally show the thumbnail of the file:
            //change this to be dynamic'
            dropzone.createThumbnailFromUrl(mockFile, url, null, true);
            dropzone.files.push(mockFile);
            dropzone.emit("complete", mockFile);

            $.post('/CreativeSets/SaveImageWithURL', { CreativeSetID: creativeSetID, URL: url, FileName: fileName, ColumnName: column, Width: width, Height: height })
                .done(function (data) {
                    // Call the default addedfile event handler
                    console.log(dropzone.files);

                    dropzone.files[0].previewElement.remove();
                    dropzone.emit("addedfile", mockFile);

                    console.log(data);
                    // And optionally show the thumbnail of the file:
                    //change this to be dynamic'
                    dropzone.createThumbnailFromUrl(mockFile, data.file, null, true);
                    dropzone.files.push(mockFile);
                    dropzone.emit("complete", mockFile);
                    console.log($(event.currentTarget).siblings('.hidden-image-url'));
                    $(event.currentTarget).siblings('.hidden-image-url').children(0).val(data.file);

                });
        }
    });
    $('.ImageUpload').on('dragenter', function (event) {
        console.log(event);
        event.preventDefault();
    });
    $('.ImageUpload').on('dragleave', function () {
    });
    $('.ImageUpload').on('dragover', function (event) {
        event.preventDefault();
    });

    $(document).on('dragstart', '.draggableImage', function (event) {
        dragged = event.target;
        console.log(event);
    });
}
$(function () {

    makeDrapAndDrop();

   // $('#SendToAMBttn').text('Submit to Account Manager Last Submitted ' + )

    $.validator.setDefaults({


    });
    $('form.CreativeSetForm').each(function () {

        if ($(this).validate().checkForm()) {
            showSetSuccess(this);

        }

    });
    calculateValidLineItems();
    fillCopyDropDowns();
    
    $(document).on('click','.actions-button', function () {
        $('.actions').toggle();
        $('.actions-row').toggle();

    });
    $('#SendToAMBttn').click(function () {
        var $panel = $(this).closest('.panel');
        console.log($panel);
        var orderID = $panel.find('#ID').first().val();
        var name = $('#AgentName').val();
        var email = $('#AgentEmail').val();
        console.log(orderID);
        console.log(name);
        console.log(email);
        sendEmailToAm(orderID, this, name, email);
    })

    $(document).on('click', '.copySets', function () {
        copyAssets(this);
    });
   
    $('.the-count').each(function () {
        var count = $(this).siblings('.max-length-input').val().length;
        $(this).find('.current').text(count);
    });

    $(document).on('keyup', '.max-length-input', function () {
        var characterCount = $(this).val().length,
            current = $(this).siblings('.the-count').find('.current');
        current.text(characterCount);
    });

    $('input[type=text]:filled, input[type=url]:filled, textarea:filled').each(function () {
        if ($(this).valid()) {
            $(this).css('border-color', "green");
        }
        else {
            $(this).css('border-color', "red");
           
            
        }
    });

    $(document).on('change', 'input[type=checkbox]', function () {
        var creativeSetID = $(this).closest('form').find('#CreativeSet_CreativeSetID').val();
        var columnName = 'UsingScript';
        var thisInput = this;

        $(this).closest('form').find('.CreativeSetImageDetails').toggle();
        $(this).closest('form').find('.CreativeSetJSDetails').toggle();

        if ($(this).closest('form').validate().checkForm()) {
            showSetSuccess($(this).closest('form'));
        }
        else {
            hideSetSuccess($(this).closest('form'));
        }
        calculateValidLineItems();
        $.post('/CreativeSets/SaveColumn', { CreativeSetID: creativeSetID, ColumnName: columnName, NewValue: $(this).is(':checked') })
            .done(function () {
               
            });
        
    });
    $(document).on('blur', 'input[name="LeaderboardLandingURL"]', function () {
        fillDown(this);
    })
    

    $(document).on('click', ".AddNewBanner", function () {
        var lastItem = $(this).closest('.LineItem-CreativeSets').children('.CreativeSet').last();
        spinnerReplace(this);
        var thisButton = $(this);

        $.get('/CreativeSets/Add', { LineItemId: $(this).data('lineitemid'), GivenType: $(this).data('giventype') }

        ).done(function (partialView) {
            try {
                lastItem.after(partialView);
                spinnerReplace(thisButton);
                fillCopyDropDowns();
                makeDrapAndDrop();
            }
            catch (exception) {
            }
        })
               });

    $(document).on("click", '.SwitchLineItems', function (e) {
        console.log('inside here');
        willShow = $(this).find('.glyphicon-chevron-down').first().is(':visible');
        $sets = $(this).parents('.panel-heading').first().next('.LineItem-CreativeSets');
        $(this).parents('.panel-heading').find('.taxonomy').toggle();
        if (willShow) {
            $sets.css("position", "unset");
            $sets.css("left", "0px");

        }
        else {
            $sets.css("position", "absolute");
            $sets.css("left", "-9000px");

        }

        $(this).find('.glyphicon-chevron-down').toggle();
        $(this).find('.glyphicon-chevron-up').toggle();

        
        
    });

    $(document).on("click", '.DeleteCreative', function (e) {
        e.preventDefault();
        var creativeSet = $(this).closest('.CreativeSet');
        console.log(creativeSet);
        var choice = confirm($(this).attr('data-confirm'));
        var thisButton = $(this);
        spinnerReplace(this);
        if (choice) {
            $.get('/CreativeSets/Delete', { CreativeID: $(this).data('creativeid') }

            ).done(function () {
                
                
                creativeSet.remove();
                fillCopyDropDowns();

            })
        }
    });

    
    
});