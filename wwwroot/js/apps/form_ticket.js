function formatToNumber(n) {
    // format number 1,000,000.00 to 1000000,00 for count
    var replace = n.replace(/\./g, "");
    return replace.replace(/,/g, ".");
}
function formatToString(n) {
    // format number 1000000.00 to 1000000,00
    return String(n).replace(/\./g, ",")
}

function formatNumber(n) {
    // format number 1000000 to 1.234.567
    return n.replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ".")
}

function formatCurrency(input, blur) {
    // get input value
    var input_val = input.val();
    // don't validate empty input
    if (input_val === "") return;
    // original length
    var original_len = input_val.length;
    // initial caret position 
    var caret_pos = input.prop("selectionStart");

    // check for decimal
    if (input_val.indexOf(",") >= 0) {
        // get position of first decimal
        // this prevents multiple decimals from
        // being entered
        var decimal_pos = input_val.indexOf(",");

        // split number by decimal point
        var left_side = input_val.substring(0, decimal_pos);
        var right_side = input_val.substring(decimal_pos);

        // add commas to left side of number
        left_side = formatNumber(left_side);

        // validate right side
        right_side = formatNumber(right_side);

        // On blur make sure 2 numbers after decimal
        if (blur === "blur") {
            right_side += "00";
        }

        // Limit decimal to only 2 digits
        right_side = right_side.substring(0, 2);

        // join number by .
        input_val = left_side + "," + right_side;
    } else {
        // no decimal entered
        // add commas to number
        // remove all non-digits
        input_val = formatNumber(input_val);
        input_val = input_val;
        // final formatting
        if (blur === "blur") {
            input_val += ",00";
        }
    }
    // send updated string to input
    input.val(input_val);
    // put caret back in the right position
    var updated_len = input_val.length;
    caret_pos = updated_len - original_len + caret_pos;
    input[0].setSelectionRange(caret_pos, caret_pos);
}
// format invoice_number => 010.005-21.01719480
function formatInvoiceNumber(value) {
    var value_replace = value.replace(/[A-Za-z\W\s_]+/g, '');
    if(value_replace.length > 16){      
        var val = `${value_replace.substr(0, 3)}.${value_replace.substr(3, 3)}-${value_replace.substr(6, 2)}.${value_replace.substr(8, 8)}`
        return val;
    } else {
        let split = 2;
        const dots = [];

        for (let i = 0, len = value_replace.length; i < len; i += split) {
            split = i >= 0 && i <= 4 ? 3 : i >= 5 && i <= 7 ? 2 : 8;
            dots.push(value_replace.substr(i, split));
        }
        const temp_dots = dots.join('.');
        const temp = dots[0] + "." + dots[1] + "-" + dots[2] + "." + dots[3];
        return temp_dots.length >= 19 ? temp : temp_dots;
    }
}

var changeFormatCurrency = function(target) {
    target.on({
        keyup: function() {
            formatCurrency($(this));
        },
        blur: function() {
            var val = this.value.trim();
            val = val.substring(1);
            formatCurrency($(this), "blur");
        }
    });
}
var count_ppn = function(dpp, ppn_number) {
    var total_ppn = dpp * ppn_number / 100;
    return total_ppn;
}
var count_pph = function(dpp, pph_number) {
    var total_pph = -(dpp * pph_number / 100);
    return total_pph;
}
var count_dpp_ppn = function(total_dpp, total_ppn) {
    var total_dpp_ppn = total_dpp + total_ppn;
    return total_dpp_ppn;
}
var change_total = function(dpp, ppn, pph, dpp_ppn) {  
    dpp = formatToString(dpp);
    $(".total_dpp").val(dpp);
    
    ppn = formatToString(ppn);
    $(".total_ppn").val(ppn);
    
    pph = formatToString(pph);
    $(".total_pph").val(pph);
    
    dpp_ppn = formatToString(dpp_ppn);
    $(".total_dpp_ppn, #invoice_amount").val(dpp_ppn);
}

var change_total_dpp_original = function(dpp) {  
    dpp = formatToString(dpp);
    $(".total_dpp_original").val(dpp);
}

var deleteGl = function(obj, ppn_number, pph_number) {
    let total = obj.closest(".gl_row").find('.gl_total_row').text();
    let total_dpp = $('.total_dpp_original').val();
    let total_all = $('.gl_total').val();
    let dpp = total_dpp - total;
    let all = total_all - total;
    change_total_dpp_original(dpp);
    $('.gl_total').text(all).val(all);
    let parent = obj.closest(".gl_row");
    parent.remove();
    checkLimItem('.gl_row', '#gl_add');
}

var deleteMaterial = function(obj, ppn_number, pph_number) {
    let total = obj.closest(".material_row").find('.material_total_row').text();
    let total_dpp = $('.total_dpp_original').text();
    let total_all = $('.material_total').val();
    let dpp = total_dpp - total;
    let all = total_all - total;
    change_total_dpp_original(dpp);
    $('.material_total').text(all).val(all);
    let parent = obj.closest(".material_row");
    parent.remove();
    checkLimItem('.material_row', '#material_add');
}


var checkLimItem = function (classGlMat, idButton) {
    var po_len = $('.po_row').length;
    var com_len = $(classGlMat).length;
    if (com_len >= po_len) {
        $(idButton).prop('disabled', true);
    } else {
        $(idButton).prop('disabled', false);
    }
}

var numberWithCommas = function(x) {
    var a = x.toString().replace(/\./g, ",")
    return a.toString().replace(/\B(?<!\.\d*)(?=(\d{3})+(?!\d))/g, ".");
}

$('.btn-save').on('click', function(){
    let dpp = $("#total_dpp_value").val();
    if(!dpp || dpp == 0){
        $('#total_dpp').text("DPP is required and cannot be 0");
        $('html, body').animate({
            scrollTop: $("#total_dpp_scroll").offset().top
        }, 500); 

        return false;
    }
})

$('.btn-send').on('click', function(){
    let dpp = $("#total_dpp_value").val();
    if(!dpp || dpp == 0){
        $('#total_dpp').text("DPP is required and cannot be 0");
        $('html, body').animate({
            scrollTop: $("#total_dpp_scroll").offset().top
        }, 500); 

        return false;
    }
})

$('.btn-approve').on('click', function(){
    // $('#note').prop("required", false);
    $('.valid-verification-note').text("");
    $('#verification-note').text("");
    if(confirm('Are you sure you wish to approve?'))
        $('form').submit();
    else
        return false
})
$('.btn-reject').on('click', function(){
    if(!($('#note').val())) {
        $('#note').prop("required", true);
        $('.valid-verification-note').text("*");
        $('#verification-note').text(note);
    } else {
        if(confirm('Are you sure you wish to reject?'))
            $('form').submit();
        else
            return false
    }
})
$('.btn-revise').on('click', function(){
    if(!($('#note').val())) {
        $('#note').prop("required", true);
        $('.valid-verification-note').text("*");
        $('#verification-note').text(note);
    } else {
        if(confirm('Are you sure you wish to revise?'))
            $('form').submit();
        else
            return false
    }
})
var KTForm = function() {
    // Private functions
    var demos = function() {
        // basic
        $('.kt_select2_country').select2({
            placeholder: placeholder_country,
            width: "100%"
        });
        $('.kt_select2_tax_status').select2({
            placeholder: placeholder_tax_status,
            width: "100%"
        });
        $('.kt_select2_po_type_group').select2({
            placeholder: placeholder_po_type_group,
            width: "100%"
        });
        $('.kt_select2_po_type').select2({
            placeholder: placeholder_po_type,
            width: "100%"
        });
        $('.kt_select2_po_number').select2({
            placeholder: placeholder_po_number,
            width: "100%"
        });
        $('.kt_select2_gr_number').select2({
            placeholder: placeholder_gr_number,
            width: "100%"
        });
        $('.kt_select2_location').select2({
            placeholder: placeholder_location,
            width: "100%"
        });
        $('.kt_select2_company').select2({
            placeholder: placeholder_company,
            width: "100%"
        });
        $('.kt_select2_gl_debt_or_credit').select2({
            width: "100%"
        });
        $('.kt_select2_material_debt_or_credit').select2({
            width: "100%"
        });
        $('.kt_select2_gl_number').select2({
            placeholder: placeholder_gl_number,
            width: "100%"
        });
        $('.kt_select2_cost_center').select2({
            placeholder: placeholder_cost_center,
            width: "100%"
        });
        $('.kt_select2_material_id').select2({
            placeholder: placeholder_material_id,
            minimumInputLength: 3,
            width: "100%",
            ajax: {
                url: base_url + lang + '/api/material_item',
                data: function (params) {
                    return {
                        q: params.term,
                        page: params.page
                    };
                },
                processResults: function (data, params) {
                    params.page = params.page || 1;
                    return {
                        results: data.items,
                        page: params.page,
                        pagination: {
                            more: (params.page * 10) < data.total_count
                        }
                    };
                }
            }
        });
    }

    checkLimItem('.gl_row', '#gl_add');
    checkLimItem('.material_row', '#material_add');

    // for ppn
    var ppn_number = 0;
    $.get(base_url + lang + '/api/ppn/?country_id='+country_vendor+'&area_code='+area, function(data){
        var item = data.data;
        ppn_number = item.tax;
    });
    // for ppn
    var pph_number = 0;
    $.get(base_url + lang + '/api/getApiRowTaxType/?tax_type_id='+tax_type_id, function(data){
        var item = data.data;
        pph_number = item.rate;
    });

    $(".tax_amt_fc").on("keyup", function() {
        changeFormatCurrency($("#tax_amt_fc").find(".tax_amt_fc"));
    })

    $("#tax_invoice_number").on("keyup", function() {
        var inv_num = formatInvoiceNumber($("#tax_invoice_number").val());
        $("#tax_invoice_number").val(inv_num);
    })

    if($(".total_dpp_original").val() == 0) 
        $(".btn-send").prop("disabled", true).css("cursor", "default")
    else
        $(".btn-send").prop("disabled", false).css("cursor", "pointer");

    if($('#ticket_number').val()) {
        var total_po = 0;
        var total_gl = 0;
        var total_material = 0;
        var incil = false;
        $("#po_table .po_row").each(function() {
            if($(this).find(".qty_billed").val()) {
                formatCurrency($(this).find(".amount"), "blur");
                formatCurrency($(this).find(".po_total_row"), "blur");

                var format_qty_billed = formatToNumber($(this).find(".qty_billed").val());
                var format_gr_qty = formatToNumber($(this).find(".gr_qty").val());
                var format_total_row = formatToNumber($(this).find(".po_total_row").val());
    
                var qty_billed = parseFloat(format_qty_billed);
                var gr_qty = parseFloat(format_gr_qty);
                var total_row = parseFloat(format_total_row);
    
                $(this).find(".qty_billed").val(formatToString($(this).find(".qty_billed").val()));
                $(this).find(".gr_qty").val(formatToString($(this).find(".gr_qty").val()));
                
                if(!isNaN(total_row))
                    total_po += total_row;
                
                if(qty_billed > gr_qty) 
                    incil = true
                formatCurrency($(this).find(".qty_billed"), "blur");
                formatCurrency($(this).find(".gr_qty"), "blur");
                
                formatCurrency($(this).find(".amount_invoice"), "blur");
            } else {
                formatCurrency($(this).find(".disabled_po_total_row"), "blur")
                formatCurrency($(this).find(".disabled_amount"), "blur");
                formatCurrency($(this).find(".disabled_qty_billed"), "blur");
                formatCurrency($(this).find(".disabled_gr_qty"), "blur");
                formatCurrency($(this).find(".disabled_amount_invoice"), "blur");
            }
        })

        if(incil || $(".total_dpp_original").val() == 0) 
            $(".btn-send").prop("disabled", true).css("cursor", "default")
        else
            $(".btn-send").prop("disabled", false).css("cursor", "pointer");

        if(role_id == 6) {
            formatCurrency($("#tax_amt_fc").find(".tax_amt_fc"), "blur");
            if(table_gl.length > 0) {
                $("#gl_table .gl_row").each(function() {
                    var format_gl_amount = formatToNumber($(this).find(".gl_amount").val());
        
                    var gl_amount = parseFloat(format_gl_amount);
                    
                    if(isNaN(gl_amount))
                        gl_amount =  0;
                    
                    if(!isNaN(gl_amount))
                        total_gl += gl_amount;
        
                    gl_amount = formatToString(gl_amount);
                    $(this).find(".gl_total_row").val(gl_amount);
                    formatCurrency($(this).find(".gl_total_row"), "blur")
                    formatCurrency($(this).find(".gl_amount"), "blur");
                })
                total_gl = formatToString(total_gl);
                $(".gl_total").val(total_gl);
                formatCurrency($("#gl_table").find(".gl_total"), "blur");
            }
            
            if(table_material.length > 0) {
                $("#material_table .material_row").each(function() {
                    var format_material_qty = formatToNumber($(this).find(".material_qty").val());
                    var format_material_amount = formatToNumber($(this).find(".material_amount").val());
        
                    var material_qty = parseFloat(format_material_qty);
                    var material_amount = parseFloat(format_material_amount);

                    var material_total_row = material_qty * material_amount;
                    if(isNaN(material_total_row))
                        material_total_row =  0;

                    if(!isNaN(material_total_row))
                        total_material += material_total_row;
                    
                    material_total_row = formatToString(material_total_row);
                    $(this).find(".material_total_row").val(material_total_row);
                    formatCurrency($(this).find(".material_total_row"), "blur")

                    formatCurrency($(this).find(".material_amount"), "blur");
                })            
                total_material = formatToString(total_material);
                $(".material_total").val(total_material);
                formatCurrency($("#material_table").find(".material_total"), "blur");
            }

            $("#po_table_accounting .po_row").each(function() {
                $(this).find(".gr_amount").text(numberWithCommas($(this).find(".gr_amount").text()));
                $(this).find(".qty_billed").text(numberWithCommas($(this).find(".qty_billed").text()));
                $(this).find(".gr_qty").text(numberWithCommas($(this).find(".gr_qty").text()));
                $(this).find(".amount_invoice").text(numberWithCommas($(this).find(".amount_invoice").text()));
                $(this).find(".amount_total").text(numberWithCommas($(this).find(".amount_total").text()));
        
            })
            $("#amount_accounting").find(".amount").text(numberWithCommas($("#amount_accounting").find(".amount").text()));
            $("#tax_amount_accounting").find(".tax_amount").text(numberWithCommas($("#tax_amount_accounting").find(".tax_amount").text()));
            $("#po_table_accounting").find(".total_price").text(numberWithCommas($("#po_table_accounting").find(".total_price").text()));
            $("#po_table_accounting").find(".total_dpp").text(numberWithCommas($("#po_table_accounting").find(".total_dpp").text()));
            $("#po_table_accounting").find(".total_ppn").text(numberWithCommas($("#po_table_accounting").find(".total_ppn").text()));
            $("#table_accounting").find(".total_diff_in").text(numberWithCommas($("#table_accounting").find(".total_diff_in").text()));
            $("#table_accounting").find(".total_dpp_ppn").text(numberWithCommas($("#table_accounting").find(".total_dpp_ppn").text()));
        }

        total_po = formatToString(total_po);
        $(".po_total").val(total_po);
        formatCurrency($("#po_table").find(".po_total"), "blur");

        formatCurrency($("#table_dpp_ppn_pph").find(".total_dpp"), "blur");
        formatCurrency($("#table_dpp_ppn_pph").find(".total_ppn"), "blur");
        formatCurrency($("#wht_base, #table_dpp_ppn_pph").find(".total_pph"), "blur");
        formatCurrency($(".invoice_amount, #table_dpp_ppn_pph").find(".total_dpp_ppn, #invoice_amount"), "blur");
    }

    $('#table_dpp_ppn_pph').on('keyup', function() {
        var format_dpp = formatToNumber($(this).find(".total_dpp").val());
        var dpp = parseFloat(format_dpp);
        if(isNaN(dpp))
            dpp = 0
        ppn = count_ppn(dpp, ppn_number);
        pph = count_pph(dpp, pph_number);
        dpp_ppn = count_dpp_ppn(dpp, ppn);

        if(isNaN(ppn))
            ppn =  0;
        if(isNaN(pph))
            pph =  0;
        if(isNaN(dpp_ppn))
            dpp_ppn =  0;

        change_total(dpp, ppn, pph, dpp_ppn)
        changeFormatCurrency($(this).find(".total_dpp"));
        formatCurrency($(this).find(".total_ppn"), "blur")
        formatCurrency($(this).find(".total_pph"), "blur")
        formatCurrency($("#table_dpp_ppn_pph, .invoice_amount").find(".total_dpp_ppn, #invoice_amount"), "blur");
    })
    
    $('#po_table').on('keyup', function(){
        var total = 0;
        var incil = false;
        $("#po_table .po_row").each(function() {
            if($(this).find('.is_disabled').val() == 0 || $(this).find('.is_disabled').val() == undefined) {
                formatCurrency($(this).find(".amount"), "blur");
                var format_qty_billed = formatToNumber($(this).find(".qty_billed").val());
                var format_gr_qty = formatToNumber($(this).find(".gr_qty").val());
                var format_inv_amount = formatToNumber($(this).find(".amount_invoice").val());
    
                var qty_billed = parseFloat(format_qty_billed);
                var gr_qty = parseFloat(format_gr_qty);
                var inv_amount = parseFloat(format_inv_amount);
    
                if(qty_billed > gr_qty) 
                    incil = true;
    
                var po_total_row = qty_billed * inv_amount;
                if(isNaN(po_total_row))
                    po_total_row =  0;
                else
                    total += po_total_row;
                
                po_total_row = formatToString(po_total_row);
                $(this).find(".po_total_row").val(po_total_row);
                formatCurrency($(this).find(".po_total_row"), "blur")
    
                changeFormatCurrency($(this).find(".qty_billed"));
                changeFormatCurrency($(this).find(".gr_qty"));
                changeFormatCurrency($(this).find(".amount_invoice"));
            } else {
                formatCurrency($(this).find(".disabled_po_total_row"), "blur")
                formatCurrency($(this).find(".disabled_amount"), "blur");
                formatCurrency($(this).find(".disabled_qty_billed"), "blur");
                formatCurrency($(this).find(".disabled_gr_qty"), "blur");
                formatCurrency($(this).find(".disabled_amount_invoice"), "blur");
            }
        })
        if(incil) 
            $(".btn-send").prop("disabled", true).css("cursor", "default")
        else
            $(".btn-send").prop("disabled", false).css("cursor", "pointer");
        
        dpp = role_id == 6 ? total + parseFloat($(".gl_total").val()) + parseFloat($(".material_total").val()) : total;
        
        total = formatToString(total);
        $(".po_total").val(total);
        formatCurrency($(this).find(".po_total"), "blur");
        
        change_total_dpp_original(dpp)
    })
    
    $('#gl_table').on('keyup', function(){
        var total = 0;
        $("#gl_table .gl_row").each(function() {
            var format_gl_amount = formatToNumber($(this).find(".gl_amount").val());

            var gl_amount = parseFloat(format_gl_amount);
            
            if(isNaN(gl_amount))
                gl_amount =  0;
            
            if(!isNaN(gl_amount))
                total += gl_amount;

            gl_amount = formatToString(gl_amount);
            $(this).find(".gl_total_row").val(gl_amount);
            formatCurrency($(this).find(".gl_total_row"), "blur")

            changeFormatCurrency($(this).find(".gl_amount"));
        })
        dpp = role_id == 6 ? parseFloat($(".po_total").val()) + total + parseFloat($(".material_total").val()) : total;

        total = formatToString(total);
        $(".gl_total").val(total);
        formatCurrency($(this).find(".gl_total"), "blur");
        
        change_total_dpp_original(dpp)
    })
    
    $('#material_table').on('keyup', function(){
        var total = 0;
        $("#material_table .material_row").each(function() {
            var format_material_qty = formatToNumber($(this).find(".material_qty").val());
            var format_material_amount = formatToNumber($(this).find(".material_amount").val());

            var material_qty = parseFloat(format_material_qty);
            var material_amount = parseFloat(format_material_amount);
            
            var material_total_row = material_qty * material_amount;
            if(isNaN(material_total_row))
            material_total_row =  0;
            
            if(!isNaN(material_total_row))
                total += material_total_row;
            
            material_total_row = formatToString(material_total_row);
            $(this).find(".material_total_row").val(material_total_row);
            formatCurrency($(this).find(".material_total_row"), "blur")

            changeFormatCurrency($(this).find(".material_amount"));
        })
        dpp = role_id == 6 ? parseFloat($(".po_total").val()) + parseFloat($(".gl_total").val()) + total : total;

        total = formatToString(total);
        $(".material_total").val(total);
        formatCurrency($(this).find(".material_total"), "blur");
        
        change_total_dpp_original(dpp)
    })
    
    $('#payment_data').on('change', function(){
        $('#type_of_payment').val($('#payment_data').val() == 1 ? "1" : "2")
        $('#local_bank_name').val($('#payment_data').val() == 1 ? localidr_bank_name : localforex_bank_name)
        $('#local_swift_code').val($('#payment_data').val() == 1 ? localidr_swift_code : localforex_swift_code)
        $('#local_account_holder').val($('#payment_data').val() == 1 ? localidr_account_holder : localforex_account_holder)
        $('#local_account_bank').val($('#payment_data').val() == 1 ? localidr_bank_account : localforex_bank_account)
        $('#local_account_number').val("")
    })

    $('#local_account_number').on('keyup', function(){
        if($('#payment_data').val() == 1) {
            if($('#local_account_number').val() != $('#local_account_bank').val())
                $('#valid_local_account_bank').text(valid_bank_account);
            else
                $('#valid_local_account_bank').text("");
        } else {
            if($('#local_account_number').val() != $('#local_account_bank').val())
                $('#valid_local_account_bank').text(valid_bank_account);
            else
                $('#valid_local_account_bank').text("");
        }
    })

    $('#foreign_bank_account').on('change', function(){
        if($('#foreign_bank_account').val() != $('#foreign_account_bank').val()) 
            $('#valid_foreign_account_bank').text(valid_bank_account);
        else
            $('#valid_foreign_account_bank').text("");
    })

    // AWB / MAIL
    if(awb_mail != "") {
        $('#awb_check').prop('checked', true);
        if($('#awb_check').is(':checked')) {
            $('#awb_mail').show()
            $('#awb_mail').val(awb_mail)
        } else {
            $('#awb_mail').hide()
            $('#awb_mail').val("")
        }
        $('#awb_check').on('change', function(){
            if($('#awb_check').is(':checked')) {
                $('#awb_mail').show()
                $('#awb_mail').val(awb_mail)
            } else {
                $('#awb_mail').hide()
                $('#awb_mail').val("")
            }
        })
    } else {
        if($('#awb_check').is(':checked')) {
            $('#awb_mail').show()
            $('#awb_mail').prop("disabled", false)
        } else {
            $('#awb_mail').hide()
            $('#awb_mail').val("")
        }
        $('#awb_check').on('change', function(){
            if($('#awb_check').is(':checked')) {
                $('#awb_mail').show()
                $('#awb_mail').prop("disabled", false)
            } else {
                $('#awb_mail').hide()
                $('#awb_mail').val("")
            }
        })
    }

    // Reimburse Back to Back
    if(top_duration == 5 || top_duration == top_interval) {
        $('#reimburse_top').prop('checked', true);
        $('#reimburse_top').on('change', function(){
            if($('#reimburse_top').is(':checked')) {
                $('#top_duration').val(5)
            } else {
                $('#top_duration').val(top_interval)
            }
        })
    } else {
        $('#reimburse_top').on('change', function(){
            if($('#reimburse_top').is(':checked')) {
                $('#top_duration').val(5)
            } else {
                $('#top_duration').val(top_interval)
            }
        })
    }

    // estimated_date
    $.get(base_url + lang + '/api/top', function(data){
        var item = data.data;
        if(created_at != null)
            var date = new Date(created_at)
        else 
            var date = new Date()

        item.forEach(value => {
            if(value.top_id == $('#top_id').val()) {
                date.setDate(date.getDate() + value.interval);
                var month = '' + (date.getMonth() + 1),
                    day = '' + date.getDate(),
                    year = date.getFullYear();
            
                if (month.length < 2) 
                    month = '0' + month;
                if (day.length < 2) 
                    day = '0' + day;

                var estimated_date = year + "-" + month + "-" + day;
                $('#top_duration').val(value.interval)
                $('#estimated_payment').val(estimated_date)
            }
        });
    });

    // option country
    $.get(base_url + lang + '/api/country', function(data){
        var item = data.data;
        item.forEach(value => {
            var selected = value.country_id == country_id ? "selected" : null;
            $("#country_id").append("<option value='"+value.country_id+"' "+selected+">" + value.name + "</option>");
        });
    });

    // option company
    $.get(base_url + lang + '/api/CompanyByVendorNumber/?vendor_number=' + vendor_number, function(data){
        var item = data.data;
        item.forEach(value => {
            var selected = value.company_id == company_id ? "selected" : null;
            $("#company_id").append("<option value='"+value.company_id+"' "+selected+">" + value.name + "</option>");
        });
    });

    // option tax status
    $.get(base_url + lang + '/api/tax_status', function(data){
        var item = data.data;
        item.forEach(value => {
            var selected = value.tax_status_id == tax_status_id ? "selected" : null;
            $("#tax_status_id").append("<option value='"+value.tax_status_id+"' "+selected+">" + value.name + "</option>");
        });
    });

    if(tax_status_id != null) {
        if(tax_status_id == 1) {
            $('.tax_invoice').show();
            $('.tax_invoice').prop("required", true).prop("disabled", false);
        }
        else {
            $('.tax_invoice').hide();
            $('.tax_invoice').prop("required", false).prop("disabled", true);
        }
    }
    $('#tax_status_id').on('change', function(){
        if($('#tax_status_id').val() == 1) {
            $('.tax_invoice').show();
            $('.tax_invoice').prop("required", true).prop("disabled", false);
        }
        else {
            $('.tax_invoice').hide();
            $('.tax_invoice').prop("required", false).prop("disabled", true);
        }
    })

    var onchange_table_po = function(po_type_group_id, gr) {
        $.get(base_url + lang + '/api/table_po/?po_type_group_id='+po_type_group_id+'&company_id='+$('#company_id').val()+'&po_number='+$('#po_number').val() + '&vendor_number=' + vendor_number + gr, function(data){
            $('#po_table tbody tr').remove();
            var item = data.data;
            var tr = '';
            item.forEach(value => {
                if(value.is_active == 0 || value.is_used == 1) {
                    var is_active_value = 1;
                    var is_active = "readonly";
                    var class_po_total = "disabled_po_total_row";
                    var class_gr_qty = "disabled_gr_qty";
                    var class_qty_billed = "disabled_qty_billed";
                    var class_amount_invoice = "disabled_amount_invoice";
                    var class_amount = "disabled_amount";
                    var css_background = "style='background-color: #C4C4C4;'";
                    var qty_billed = 0;
                    var amount_invoice = 0;
                    var po_total_row = 0;
                } else {
                    var is_active_value = 0;
                    var is_active = "";
                    var class_po_total = "po_total_row";
                    var class_gr_qty = "gr_qty";
                    var class_qty_billed = "qty_billed";
                    var class_amount_invoice = "amount_invoice";
                    var class_amount = "amount";
                    var css_background = "";
                    var qty_billed = "";
                    var amount_invoice = "";
                    var po_total_row = "";
                }
                var gl_description = value.gl_description ? value.gl_description : "-"
                tr += '<tr class="po_row" '+css_background+'>\
                        <input type="hidden" class="is_disabled" name="is_disabled[]" value="'+is_active_value+'">\
                        <input type="hidden" class="po_item_number" name="po_item_number[]" value="'+value.po_item_number+'">\
                        <input type="hidden" class="po_name" name="po_name[]" value="'+value.po_name+'">\
                        <input type="hidden" class="vendor_origin" name="vendor_origin[]" value="'+value.vendor_origin+'">\
                        <input type="hidden" class="document_no" name="document_no[]" value="'+value.document_no+'">\
                        <input type="hidden" class="item_1" name="item_1[]" value="'+value.item_1+'">\
                        <input type="hidden" class="gl_description" name="gl_description[]" value="'+value.gl_description+'">\
                        <td>'+value.po_name+'</td>\
                        <td>'+gl_description+'</td>\
                        \
                        <td style="background-color: #C4C4C4;"><input type="text" class="form-control '+class_gr_qty+'" name="gr_qty[]" value="'+numberWithCommas(value.qty)+'" readonly></td>\
                        \
                        <td><input type="text" class="form-control '+class_qty_billed+'" name="qty_billed[]" size="3" value="'+qty_billed+'" required '+is_active+'></td>\
                        \
                        <input type="hidden" class="uom" name="uom[]" value="'+value.uom+'">\
                        <td>'+value.uom+'</td>\
                        \
                        <input type="hidden" class="currency" name="currency[]" value="'+value.currency+'">\
                        <td>'+value.currency+'</td>\
                        \
                        <td style="background-color: #C4C4C4;"><input type="text" class="form-control '+class_amount+'" name="gr_amount[]" value="'+numberWithCommas(value.amount)+'" readonly></td>\
                        \
                        <td><input type="text" class="form-control '+class_amount_invoice+'" name="amount_invoice[]" value="'+amount_invoice+'" size="3" required '+is_active+'></td>\
                        \
                        <td><input type="text" class="form-control '+class_po_total+'" name="amount_total[]" value="'+po_total_row+'" readonly></td>\
                        </tr>\
                        ';
                        $('#invoice_currency').val(value.currency);
                    })
                    $('#po_table tbody').append(tr);
                });
    }

    var get_gr_number = function(change, po_number, vendor_number, company_id) {
        $.get(base_url + lang + '/api/gr_number/?po_number='+po_number+'&po_type_group_id='+$("#po_type_group").val()+'&po_type='+$('#po_type_id').val()+'&vendor_number='+vendor_number+'&company_id='+company_id, function(data){
            $("#gr_number").find('option').remove();
            $("#gr_number").append("<option value=''>Label</option>");
            if(!change && gr_number != "")
                $("#gr_number").append("<option value='"+gr_number+"' selected>" + gr_number + "</option>");
            
            var item = data.data;
            item.forEach(value => {
                var selected = value.lpb_number == gr_number ? "selected" : null;
                $("#gr_number").append("<option value='"+value.lpb_number+"' "+selected+">" + value.lpb_number + "</option>");
            });
        });
    }

    var get_po_number = function(change, company_id, po_type_id) {
        $.get(base_url + lang + '/api/po_number/?po_type_group_id='+$("#po_type_group").val()+'&po_type='+po_type_id+'&vendor_number='+vendor_number+'&company_id='+company_id, function(data){
            $("#po_number").find('option').remove();
            $("#po_number").append("<option value=''>Label</option>");
            if(!change && po_number != "")
                $("#po_number").append("<option value='"+po_number+"' selected>" + po_number + "</option>");
            var item = data.data;
            item.forEach(value => {
                var selected = value.po_number == po_number ? "selected" : null;
                $("#po_number").append("<option value='"+value.po_number+"' "+selected+">" + value.po_number + "</option>");
            });
        });
    }

    var get_company_area = function(company_id) {
        $.get(base_url + lang + '/api/company_area/?company_id='+company_id, function(data){
            $("#area_code").find('option').remove();
            $("#area_code").append("<option value=''>Label</option>");
            var item = data.data;
            item.forEach(value => {
                var selected = value.code == area_code ? "selected" : null;
                $("#area_code").append("<option value='"+value.code+"' "+selected+">" + value.name + "</option>");
            });
        });
    }

    var get_po_type_id  = function(po_type_group_id) {
        $.get(base_url + lang + '/api/po_type/?po_type_group_id='+po_type_group_id, function(data){
            $("#po_type_id").find('option').remove();
            $("#po_type_id").append("<option value=''>Label</option>");
            var item = data.data;
            item.forEach(value => {
                var selected = value.po_type_id == po_type_id ? "selected" : null;
                $("#po_type_id").append("<option value='"+value.po_type_id+"' "+selected+">" + value.name + "</option>");
            });
        });
    }
    
    // option po type group
    $.get(base_url + lang + '/api/po_type_group', function(data){
        var item = data.data;
        item.forEach(value => {
            var selected = value.po_type_group_id == po_type_group_id ? "selected" : null;
            $("#po_type_group").append("<option value='"+value.po_type_group_id+"' "+selected+">" + value.name + "</option>");
        });
    });

    // option po type
    $('#po_type_group').on('change', function() {
        get_po_type_id($('#po_type_group').val());
    })
    
    if(po_type_group_id != "")
        get_po_type_id(po_type_group_id);

    // option po number
    if(po_type_id != "") {
        get_po_number(false, company_id, po_type_id)
    }

    // gr_number
    if(gr_number == "")
        $('.gr_number').hide();

    if(po_number != ""){
        if(po_type_id != 1) {
            get_gr_number(false, po_number, vendor_number, company_id)
            $("#po_number").on('change', function(){
                get_gr_number(true, $("#po_number").val(), vendor_number, $("#company_id").val())
                $("#po_table").find(".po_total").val(0);
            })
        }
        $("#po_number, #gr_number").on('change', function(){
            $("#po_table").find(".po_total").val(0);
            var gr = (gr_number ? '&gr_number='+$('#gr_number').val() : "");
            onchange_table_po($("#po_type_group").val(), gr);
        })
    }

    $('#po_type_id').on('change', function(){
        $('#po_table tbody tr').remove();
        $("#po_table").find(".po_total").val(0);
        var name = null;
        var gr_number = true;
        if($('#po_type_id').val() != 1) {
            $('.gr_number').show();
            gr_number = true;
            // file_gr_number required
            $("#file_lpb_gr").prop("required", true)
            $("#required_file_lpb_gr").text("*")
            $("#po_number").on('change', function(){
                get_gr_number(true, $('#po_number').val(), vendor_number, $("#company_id").val())
            })
        } else {
            // file_gr_number required
            $("#file_lpb_gr").prop("required", false)
            $("#required_file_lpb_gr").text("")
            $('.gr_number').hide();
            gr_number = false;
        }
        $("#gr_number").find('option').remove();
        $("#po_number").find('option').remove();
        // option po number
        get_po_number(true, $('#company_id').val(), $('#po_type_id').val())

        $("#po_number, #gr_number").on('change', function(){
            $("#po_table").find(".po_total").val(0);
            var gr = (gr_number ? '&gr_number='+$('#gr_number').val() : "");
            onchange_table_po($("#po_type_group").val(), gr);
        })
    })
    
    // option location
    if(company_id != "") {
        get_company_area(company_id)
        if(area_code != "") {
            get_po_number(false, company_id, $('#po_type_id').val())
        }
    }
    
    $('#company_id').on('change', function() {
        get_company_area($('#company_id').val())
        get_po_number(true, $('#company_id').val(), $('#po_type_id').val())
    })
    
    //get area code for email
    $('#area_code').on('change', function() {
        const selected_area = $('#area_code').val();
        $("#area_code2").val(selected_area);
        $("#chat").show()
        get_po_number(true, $('#company_id').val(), $('#po_type_id').val())
    })

    // option gl_item
    $.get(base_url + lang + '/api/gl_item', function(data){
        var item = data.data;
        item.forEach(value => {
            // var selected = value.gl_id == gl_item ? "selected" : null;
            $(".kt_select2_gl_number").append("<option value='"+value.gl_id+"'>" + value.gl_id + " - " + value.description + "</option>");
        });
    });
    
    // option cc
    $.get(base_url + lang + '/api/cc_item', function(data){
        var item = data.data;
        item.forEach(value => {
            // var selected = value.cc_id == cc_item ? "selected" : null;
            $(".kt_select2_cost_center").append("<option value='"+value.cc_id+"'>" + value.cc_id + " - " + value.description + "</option>");
        });
    });

    $('#material_table').on('change', 'tbody tr', function(){
        var index = $(this).index();
        var id = $('#material_table tbody tr').eq(index).find('.kt_select2_material_id').val();
        var plnt = $('#material_table tbody tr').eq(index).find('.material_plnt');
        $.get(base_url + lang + '/api/material_item_by_id/?material_id='+id, function(data){
            var item = data.data;
            var plnt_val = item.material_plnt ? item.material_plnt : "-";
            plnt.val(plnt_val);
        });
    })

    $('#gl_add').on('click', function() {
        $("#gl_table").each(function () {
            $('.gl_row:first').find('.kt_select2_gl_number').select2('destroy');
            $('.gl_row:first').find('.kt_select2_gl_debt_or_credit').select2('destroy');
            $('.gl_row:first').find('.kt_select2_cost_center').select2('destroy');
            var clone = $('.gl_row:first').clone();
            var deleteRow = '<td>\
                                <button type="button" class="btn btn-danger" style="cursor: pointer;" onclick="deleteGl($(this), '+ppn_number+', '+pph_number+')">\
                                    <i class="fa fa-trash"></i>\
                                </button>\
                            </td>';
            clone.append(deleteRow);
            $('#gl_table tbody').append(clone);
            $('.gl_row:last').find('.gl_amount').val("");
            $('.gl_row:last').find('.gl_total_row').text("0");
            $('.kt_select2_gl_number').select2();
            $('.kt_select2_gl_debt_or_credit').select2();
            $('.kt_select2_cost_center').select2();
        });
        checkLimItem('.gl_row', '#gl_add');
    })
    
    $('#material_add').on('click', function() {
        $("#material_table").each(function () {
            $('.material_row:first').find('.kt_select2_material_id').select2('destroy');
            $('.material_row:first').find('.kt_select2_material_debt_or_credit').select2('destroy');
            var clone = $('.material_row:first').clone();
            var deleteRow = '<td>\
                                <button type="button" class="btn btn-danger" style="cursor: pointer;" onclick="deleteMaterial($(this), '+ppn_number+', '+pph_number+')">\
                                    <i class="fa fa-trash"></i>\
                                </button>\
                            </td>';
            clone.append(deleteRow);
            $('#material_table tbody').append(clone);
            $('.kt_select2_material_id').select2({
                placeholder: placeholder_material_id,
                minimumInputLength: 3,
                width: "100%",
                ajax: {
                    url: base_url + lang + '/api/material_item',
                    data: function (params) {
                        return {
                            q: params.term,
                            page: params.page
                        };
                    },
                    processResults: function (data, params) {
                        params.page = params.page || 1;
                        return {
                            results: data.items,
                            page: params.page,
                            pagination: {
                                more: (params.page * 10) < data.total_count
                            }
                        };
                    }
                }
            });
            $('.material_row:last').find('.material_amount').val("");
            $('.material_row:last').find('.material_qty').val("");
            $('.material_row:last').find('.material_plnt').val("");
            $('.material_row:last').find('.material_total_row').text("0");
            $('.kt_select2_material_debt_or_credit').select2();
        });
        checkLimItem('.material_row', '#material_add');
    })
    
    // Public functions
    return {
        init: function() {
            demos();
            // modalDemos();
        }
    };
    }();
    
// Initialization
jQuery(document).ready(function() {
KTForm.init();
});
