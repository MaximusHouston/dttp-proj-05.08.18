//Setup sort functionality
$(function () {
    Initialise();
    //close modals if any are present, or will be present in the future:
    $('#content').on('click', '.modal .close-btn, .modal .cancel-btn', function (e) {
        $(this).parents('.modal').fadeOut(200);
    });

    //scroll to non-ajax validation errors
    scrollToValidationErrors();
});

function Initialise(element)
{
    $("#content form").each(function ()
    {
        form = $(this);
        if (form.find("input[name=SortColumn]").length > 0 && form.find("input[name=IsDesc]").length > 0)
        {
            SortSetup(form);
        }
        PostBackSetup(form);
    });

    if ($('.clear-search-btn').length)
    {
        $('.search-bar input').on('keyup', function () {
            if ($(this).val().length > 0) $(this).next().show();
            else $(this).next().hide();
        });

        $('.clear-search-btn').on('click', function () {
            $(this).prev().val('');
            $(this).hide();
        });
    }
   
}
// do an ajax post refreshing content
function Post(form, element, resultToElement, asyncCallback)
{
    var postrequest = "post";
    var action = form.attr('action');

    //set #content as element to be replaced by default
    resultToElement = resultToElement || "content";

    AllScAttributes(form, element, function (form, name, value)
    {
        if (name == "sc-ajaxget")
        {
            postrequest = "ajaxget";
            action = value;
        }

        if (name == "sc-ajaxpost")
        {
            postrequest = "ajaxpost";
            action = value;
        }

        if (name == "sc-post")
        {
            postrequest = "post";
            action = value;
        }

        if (value[0] == "$")
        {
            value = eval(value);
        }

        name = name.replace("sc-", "");

        form.append("<input id='" + name + "' name='" + name + "' type='hidden' value='" + value + "' />");
        
    });

    if (postrequest == "ajaxpost")
    {
        $.post(action, $(form).serialize(), function (data)
        {
            PostAction(resultToElement, data, asyncCallback);
        });
        
    }

    if (postrequest == "post")
    {
        $(form).attr("action",action)
        $(form).submit();
    }

    if (postrequest == "ajaxget")
    {
        $.get(action, function (data)
        {
            $("#" + resultToElement).html(data);
            Initialise();
        });
    }
    
}

function PostAction(resultToElement, data, asyncCallback)
{
    if (data.indexOf("REDIRECT_TO_REFERRER:") == 0)
    {
        window.location = document.referrer;
        return;
    }
    if (data.indexOf("RELOAD:") == 0) {
        window.location.reload();
        return;
    }
    if (data.indexOf("REDIRECT_TO:") == 0)
    {
        data = data.replace("REDIRECT_TO:", "");
        window.location = data;
        return;
    }
    $("#" + resultToElement).html(data);

    //look for page messages that were added after post, then scroll to them
    scrollToValidationErrors();

    if (typeof asyncCallback === 'function') asyncCallback(data);
    Initialise();
}

function scrollToValidationErrors()
{
    if ($('form[data-scrollbackonpost="true"]').length == 0)
    {
        if ($('[class^="pagemessage-"]').length)
        {
            //scroll up to top of page if page messages exist
            $(window).scrollTop(($('[class^="pagemessage-"]').eq(0).offset().top) - 20);
        }
        else if ($('.input-validation-error').length)
        {
            //look for errors in form, then scroll to where they are
            $(window).scrollTop(($('.input-validation-error').eq(0).prev().offset().top) - 20);
        }
    }
}

//method to scroll to last recorded page position, called by page scripts
function scrollToLastPagePosition(localstoragekey)
{
    if (Modernizr.localstorage)
    {
        var pos = localStorage.getItem(localstoragekey);
        if (typeof pos !== "undefined" && pos !== null)
        {
            console.log(pos);
            var pos = parseFloat(pos);

            if (pos > $(window).scrollTop())
            {
                $(window).scrollTop(pos);
            }

            localStorage.setItem(localstoragekey, null);
        }
    }
}

//Do an ajax post after the user has confirmed it's ok
function PostAfterConfirm(aTag, formId, otherScAttributes, targetElementId, asyncCallback)
{
    return function ()
    {
        var currentScAttributes = ['sc-ajaxpost'];
        if(otherScAttributes && otherScAttributes.length > 0)
        {
            for(var k = 0; k < otherScAttributes.length; k++)
            {
                currentScAttributes.push(otherScAttributes[k]);
            }
        }
        
        var currentATag = aTag.cloneNode();
        currentATag.removeAttribute('data-confirm');

        //swap data- tags for sc- tags
        for (var i = 0; i < currentScAttributes.length; i++)
        {
            currentATag.setAttribute(currentScAttributes[i], currentATag.getAttribute('data-' + currentScAttributes[i]));
            currentATag.removeAttribute('data-' + currentScAttributes[i]);
        }

        //do standard post
        Post($(formId), currentATag, targetElementId, asyncCallback);
    }
}

// add form post event on 'a' elements,adding any input hidden types needed
function PostBackSetup(form)
{
    form.find("a").each(function ()
    {
        if (HasScAttributes(this))
        {
            $(this).click(function ()
            {
                Post(form,this)
                return false;
            });
        }
    });
}

function HasScAttributes(element)
{
    return AllScAttributes(null,element,null);
}

// perform a function on every attribute starting with 'sc-'
function AllScAttributes(form, element, callback)
{
    var found = false;
    $.each(element.attributes, function ()
    {
        if (this.specified && this.name.indexOf('sc-') == 0)
        {
            if (callback != null) callback(form, this.name, this.value);
            found = true;
        }
    });
    return found;
}

function SortSetup(form)
{

    var currentSortColumn = form.find("input[name=SortColumn]");
    var currentIsDesc = form.find("input[name=IsDesc]");

    // what is the current sort
    if (currentSortColumn.val() != "")
    {
        form.find("#" + currentSortColumn.val()).attr("aria-sort", currentIsDesc.val() == "true" ? "descending" : "asending");
    }

    // set up click action for all sort headers
    var sortHeaders = form.find("th[aria-sort]");

    sortHeaders.click(function (e)
    {
        currentSortColumn.val(this.id);
        currentIsDesc.val($(this).attr("aria-sort") != "descending");
        form.submit();
    });

}

function CountrySelected(countryElementId, stateElementId)
{

    if ($(this.event.srcElement).parents("#shipToAddress").length == 1) {// in Project Location Tab
        $.get("/Shared/AjaxDropDownRegions" +
        "?stateElementId=" + stateElementId +
        "&countrycode=" + $("#shipToAddress #" + countryElementId.replace(".", "_")).val(), function (d) {
            $("#" + stateElementId.replace(".", "_")).replaceWith(d);

            if ($("#" + stateElementId.replace(".", "_")).selector == "#ShipToAddress_StateId") {

                $('#shipToAddress #ShipToAddress_StateId').replaceWith(d);

                //rebind ShipToAddress State DDL on change event
                $('#shipToAddress #ShipToAddress_StateId').on('change', function (e) {
                    var value = $('#shipToAddress #ShipToAddress_StateId').val();
                    $('#projectDetails #ShipToAddress_StateId').val(value);

                });

                $('#projectDetails #ShipToAddress_StateId').on('change', function (e) {
                    var value = $('#projectDetails #ShipToAddress_StateId').val();
                    $('#shipToAddress #ShipToAddress_StateId').val(value);
                });

            }


        });
    } else { // in Project Details Tab and other Tabs
        $.get("/Shared/AjaxDropDownRegions" +
        "?stateElementId=" + stateElementId +
        "&countrycode=" + $("#" + countryElementId.replace(".", "_")).val(), function (d) {
            $("#" + stateElementId.replace(".", "_")).replaceWith(d);

            if ($("#" + stateElementId.replace(".", "_")).selector == "#ShipToAddress_StateId") {

                $('#shipToAddress #ShipToAddress_StateId').replaceWith(d);

                //rebind ShipToAddress State DDL on change event
                $('#shipToAddress #ShipToAddress_StateId').on('change', function (e) {
                    var value = $('#shipToAddress #ShipToAddress_StateId').val();
                    $('#projectDetails #ShipToAddress_StateId').val(value);
                });

                $('#projectDetails #ShipToAddress_StateId').on('change', function (e) {
                    var value = $('#projectDetails #ShipToAddress_StateId').val();
                    $('#shipToAddress #ShipToAddress_StateId').val(value);
                });

            }


        });

    }

    
}

function DataScPostAfterConfirm($elAction, $elForm) {
    $elForm.attr("action", $elAction.attr("data-sc-post"));
    $elForm.submit();
}

function DiscountRequestActionInitialise(viewOnly)
{
    $(function ()
    {
        if (viewOnly === "False")
        {
            $('.datepicker').datepicker({
                showOn: "button",
                buttonImage: "/Images/datepicker.png",
                buttonImageOnly: true,
                dateFormat: DATE_FORMAT
            });

            numericStepperHelpers.enableNumericSteppers({ trailingChars: '%' });

            CheckBoxTextBoxCheck('attachLineByLineRow', 'attachLineByLine');

            CheckBoxTextBoxCheck('competitorPriceAvailableRow', 'competitorPriceAvailable');

            CheckBoxTextBoxCheck('copyOfCompQuoteRow', 'copyOfCompQuote');

            $("#StartUpCosts").on('keyup', function ()
            {
                 var total = (parseFloat($("#TotalSell").val()) + validatedStartUpCost() + parseFloat($("#TotalFreight").val()));
                 $("#TotalSaleValue").html("$" + total.toFixed(2));
                 DiscountRequestCalculateTotalRevisedPrice();  
            });

            $('.numeric-stepper button').on('click', DiscountRequestCalculateTotalRevisedPrice);
            $('.numeric-stepper input').on('keyup', DiscountRequestCalculateTotalRevisedPrice);

            $('.submit_dar_btn').on('click', function () {
                confirmModal.showConfirmMsg('Submit discount request', 'Once the discount request is submitted if any editing of the quote takes place the discount request will be made invalid. Submit this discount request?', function () { DataScPostAfterConfirm($('.submit_dar_btn'), $('#DiscountRequestForm')) });
            });
        }
        else
        {
            $('#DAC').find("input[type!=hidden], select").not('.js-alwaysactive').attr('disabled', 'true');
            $('#DAC').find('.cb-switch-label').addClass('disabled');

            $('#COM').find("input[type!=hidden], select").not('.js-alwaysactive').attr('disabled', 'true');
            $('#COM').find('.cb-switch-label').addClass('disabled');

            $('.delete_dar_btn').on('click', function () {
                confirmModal.showConfirmMsg('Delete discount request', 'Are you sure you want to delete this Discount Request?', function () { DataScPostAfterConfirm($('.delete_dar_btn'), $('#DiscountRequestForm')) }); 
            });

            $('.delete_commission_btn').on('click', function () {
                confirmModal.showConfirmMsg('Delete commission request', 'Are you sure you want to delete this Commission Request?', function () { DataScPostAfterConfirm($('.delete_commission_btn'), $('#CommissionRequestForm')) });
            });

            if($('.reject_dar_btn').length > 0)
            {
                $('.reject_dar_btn').on('click', function () {
                    confirmModal.showConfirmMsg('Reject discount request', 'Are you sure you want to reject this Discount Request?', function () { DataScPostAfterConfirm($('.reject_dar_btn'), $('#DiscountRequestForm')) });
                });
            }

            if ($('.reject_commission_btn').length > 0) {
                $('.reject_commission_btn').on('click', function () {
                    confirmModal.showConfirmMsg('Reject commission request', 'Are you sure you want to reject this Commission Request?', function () { DataScPostAfterConfirm($('.reject_commission_btn'), $('#CommissionRequestForm')) });
                });
            }

            if($('.approve_dar_btn').length > 0)
            {
                $('.approve_dar_btn').on('click', function () {
                    confirmModal.showConfirmMsg('Approve discount request', 'Are you sure you want to approve this Discount Request?', function () { DataScPostAfterConfirm($('.approve_dar_btn'), $('#DiscountRequestForm')) });
                });
            }

            if($('.approve_commission_btn').length > 0)
            {
                $('.approve_commission_btn').on('click', function () {
                    confirmModal.showConfirmMsg('Approve Commission Request', 'Are you sure you want to approve this Commisison Request?', function () {
                        DataScPostAfterConfirm($('.approve_commission_btn'), $('#CommissionRequestForm'))
                    });
                });
            }
        }
     
    });
}

function validatedStartUpCost()
{
    var sanitised = ($('#StartUpCosts').val().length > 0) ? $('#StartUpCosts').val().replace(",", "") : "0";
    return parseFloat(sanitised);
}

function DiscountRequestCalculateTotalRevisedPrice()
{
    var totalnet = parseFloat($("#TotalNet").val());
    var totalstartupcosts = validatedStartUpCost();
    var totalfrieght = parseFloat($("#TotalFreight").val());
    var totalList = parseFloat($('#TotalList').val());

    var discountRequest = parseFloat($("#DiscountRequestStepper input").val());

    var commissionRequest = parseFloat($("#CommissionRequestStepper input").val());

    $("#RequestedDiscount").val(discountRequest);

    $("#RequestedCommission").val(commissionRequest);

    var discountAmount = totalnet * (discountRequest / 100);

    $("#RequestedDiscountAmount").html("$" + discountAmount.toFixed(2));

    $('#RequestedNetMaterialValue').html("$" + (totalnet - discountAmount).toFixed(2));

    var revisedTotalNet = totalnet - discountAmount;

    var revisedTotalSell = revisedTotalNet + revisedTotalNet * (commissionRequest / 100);

    var revisedNetMultiplier = (revisedTotalNet / totalList).toFixed(3); // Change to 3 decimal

    if (isNaN(revisedNetMultiplier)) revisedNetMultiplier = 0;

    $('#TotalNetMultiplier').html(revisedNetMultiplier);

    $("#RequestedCommissionAmount").html("$" + (revisedTotalSell - revisedTotalNet).toFixed(2));

    $("#RevisedTotalSale").html("$" + (revisedTotalSell + totalstartupcosts + totalfrieght).toFixed(2));
}

function CheckBoxTextBoxCheck(rowid, containerid) {
    var $checkbox = $('#' + rowid + ' input[type="checkbox"]');

    var $containerEl = $('#' + containerid);

    var $inputEl = $containerEl.find('input, button');

    $checkbox.on('change', function () {
        var checked = $checkbox.is(':checked');
        (checked) ? $containerEl.removeClass('disabled') : $containerEl.addClass('disabled')
        $inputEl.attr('disabled', !checked);
    });

    var checked = $checkbox.is(':checked');
    (checked) ? $containerEl.removeClass('disabled') : $containerEl.addClass('disabled')
    $inputEl.attr('disabled', !checked);
}

window.scService = {
    Post: Post,
    PostAction: PostAction,
    PostAfterConfirm: PostAfterConfirm,
    PostBackSetup: PostBackSetup,
    HasScAttributes: HasScAttributes,
    AllScAttributes: AllScAttributes,
    DataScPostAfterConfirm: DataScPostAfterConfirm
}
