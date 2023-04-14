function formatNumber(n) {
    // format number 1000000 to 1.234.567
    return n.replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ".")
}
function formatCurrency(input, blur) {
    // get input value
    var input_val = input.text();
    console.log(input)
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
    // input.setSelectionRange(caret_pos, caret_pos);
    console.log(input[0].value);
    return input[0].value;
}
var numberWithCommas = function(x) {
    var a = x.toString().replace(/\./g, ",")
    return a.toString().replace(/\B(?<!\.\d*)(?=(\d{3})+(?!\d))/g, ".");
}

var KTForm = function() {
    // Private functions
    var demos = function() {
        // basic
    }

    // estimated payment
    var date = new Date(created_at)
    date.setDate(date.getDate() + parseInt(top_duration));
    var month = '' + (date.getMonth() + 1),
        day = '' + date.getDate(),
        year = date.getFullYear();
    
    if (month.length < 2) 
        month = '0' + month;
    if (day.length < 2) 
        day = '0' + day;

    var estimated_date = day + "/" + month + "/" + year;
    $('.estimated_payment').text(estimated_date)

    $("#po_table .po_row").each(function() {
        $(this).find(".gr_amount").text(numberWithCommas($(this).find(".gr_amount").text()));
        $(this).find(".qty_billed").text(numberWithCommas($(this).find(".qty_billed").text()));
        $(this).find(".gr_qty").text(numberWithCommas($(this).find(".gr_qty").text()));
        $(this).find(".amount_invoice").text(numberWithCommas($(this).find(".amount_invoice").text()));
        $(this).find(".amount_total").text(numberWithCommas($(this).find(".amount_total").text()));

    })
    $("#po_table").find(".total_price").text(numberWithCommas($("#po_table").find(".total_price").text()));
    $("#po_table").find(".dpp").text(numberWithCommas($("#po_table").find(".dpp").text()));
    $("#po_table").find(".ppn").text(numberWithCommas($("#po_table").find(".ppn").text()));
    $("#po_table").find(".pph").text(numberWithCommas($("#po_table").find(".pph").text()));
    $("#po_table").find(".dpp_ppn").text(numberWithCommas($("#po_table").find(".dpp_ppn").text()));

    $("#wht_base").find(".wht_base").text(numberWithCommas($("#wht_base").find(".wht_base").text()));
    $("#tax_amt_fc").find(".tax_amt_fc").text(numberWithCommas($("#tax_amt_fc").find(".tax_amt_fc").text()));
    // Public functions
    return {
        init: function() {
            demos();
        }
    };
    }();
    
// Initialization
jQuery(document).ready(function() {
KTForm.init();
});
