// var index_company = no;
var index_company = 0;
var company_id = '#company_id_'+index_company;
var purchase_organization_id = '#purchase_organization_id_'+index_company;
var deleteCompany = function(obj) {
    if ( confirm(delete_company) ) {
        let parent = obj.closest(".company");
        parent.remove();
        index_company--;
        var length = 0
        $.get(base_url + lang + '/api/getApiCompany/?vendor_type_id='+$('#hidden_vendor_type_id').val(), function(data){
            data.data.forEach(item => {
                length++;
            })
            if(length == index_company)
                $("#btn_add_company").css("pointer-events", "none")
            else
                $("#btn_add_company").css("pointer-events", "auto")
        })
    }
}
var validation_note = function(is_required, remove_class, add_class, valid_verification_note, verification_note) {
    $('#note').prop("required", is_required).removeClass(remove_class).addClass(add_class);
    $('.valid-verification-note').text(valid_verification_note);
    $('#verification-note').text(verification_note);
}
var condition_submit_form = function() {
    let topId = $("#top_id").val();
    if(topId && confirm('Are you sure you wish to approve?'))
        $('form').submit();
    else
        return false
}
var valid_top_id = function() {
    let topId = $("#top_id").val();
    if(!topId){
        $('#top_id_error').text("Term of payment is required");
        $('html, body').animate({
            scrollTop: $("#top_id_scroll").offset().top
        }, 500); 

        return false;
    }
}
$('#btn-approve').on('click', function(){
    valid_top_id()
    validation_note(false, "input-mandatory", "input-additional", "", "")
    condition_submit_form()
})
$('#btn-reject').on('click', function(){
    valid_top_id()
    if(!($('#note').val())) {
        validation_note(true, "input-additional", "input-mandatory", "*", note)
    } else {
        condition_submit_form()
    }
})
$('#btn-revise').on('click', function(){
    valid_top_id()
    if(!($('#note').val())) {
        validation_note(true, "input-additional", "input-mandatory", "*", note)
    } else {
        condition_submit_form()
    }
})
var KTForm = function() {
    // Private functions
    var demos = function() {
        // basic
        $('.kt_select2_vendor_type_id').select2({
            placeholder: placeholder_vendor_type,
            width: "100%",
            containerCss: {backgroundColor: "#F8CBAD", color: "white"}
        });
        $('.kt_select_sg_category_id').select2({
            width: "100%",
            containerCss: {backgroundColor: "#F8CBAD", color: "white"}
        });
        $('.kt_select2_top').select2({
            placeholder: placeholder_top,
            width: "100%",
            containerCss: {backgroundColor: "#F8CBAD", color: "white"}
        });
        $('.kt_select2_country').select2({
            placeholder: placeholder_country,
            width: "100%",
            containerCss: {backgroundColor: "#F8CBAD", color: "white"}
        });
        $('.kt_select2_country_origin').select2({
            placeholder: placeholder_country_origin,
            width: "100%",
            containerCss: {backgroundColor: "#F8CBAD", color: "white"}
        });
        $('.kt_select2_currency').select2({
            placeholder: placeholder_currency,
            width: "100%",
            containerCss: {backgroundColor: "#F8CBAD", color: "white"}
        });
        $('.kt_select2_tax_type').select2({
            placeholder: placeholder_tax_type,
            width: "100%",
            containerCss: {backgroundColor: "#F8CBAD", color: "white"}
        });
        $('.kt_select2_tax_number_type').select2({
            placeholder: placeholder_tax_num_type,
            width: "100%",
            containerCss: {backgroundColor: "#F8CBAD", color: "white"}
        });
        $('.kt_select2_bank').select2({
            placeholder: placeholder_bank,
            containerCss: {backgroundColor: "#F8CBAD", color: "white"},
            width: "100%"
        });
        $('#kt_select2_email').select2({
            placeholder: placeholder_email,
            width: "100%",
            containerCss: {backgroundColor: "#F8CBAD", color: "white"},
            tags: true,
            maximumSelectionLength: 3,
            tokenSeparators: [';']
        });
        $('#kt_select2_telephone').select2({
            placeholder: placeholder_telephone,
            width: "100%",
            containerCss: {backgroundColor: "#F8CBAD", color: "white"},
            tags: true,
            maximumSelectionLength: 3,
            tokenSeparators: [';']
        });
        $('.kt_select2_pic').select2({
            containerCss: {backgroundColor: "#F8CBAD", color: "white"},
            width: "100%",
            placeholder: placeholder_pic
        });
        $('.kt_select2_partner').select2({
            containerCss: {backgroundColor: "#F8CBAD", color: "white"},
            width: "100%",
            placeholder: placeholder_partner
        });
    }

    var get_sg_category = function(vendor_type_id) {
        $("#sg_category_id").find('option').remove();
        $.get(base_url + lang + '/api/GetApiSgCategory/?vendor_type_id='+vendor_type_id, function(data){
            var item = data.data;
            item.forEach(value => {
                var selected = value.sg_category_id == $('#hidden_sg_category_id').val() ? "selected" : null;
                $("#sg_category_id").append("<option value='"+value.sg_category_id+"' "+selected+">" + value.name + "</option>");
            });
        });
    }

    var get_purchase_organization_group = function(id_purchase_organization, company_id, vendor_type_id, purchase_organization_group_exist) {
        $(id_purchase_organization).find('option').remove();
        $.get(base_url + lang + '/api/purchase_organization_group?company='+company_id+'&vendor_type='+vendor_type_id, function(data){
            var item = data.data;
            var value_exist = purchase_organization_group_exist == "" || purchase_organization_group_exist == null ? item[0].purchase_organization_id : purchase_organization_group_exist;
            item.forEach(value => {
                var selected = value.purchase_organization_id == value_exist ? "selected" : null;
                $(id_purchase_organization).append("<option value='"+value.purchase_organization_id+"' "+selected+">" + value.name +"</option>");
            });
            $(id_purchase_organization).selectpicker({maxOptions: 1}).attr("multiple", false).selectpicker('refresh');
        });
    }

    var get_vendor_type_by_id = function(vendor_type_id) {
        $.get(base_url + lang + '/api/vendor_type_by_id?vendor_type_id='+vendor_type_id, function(data){
            var item = data.data;
            $('#account_group').text(item.sap_code);
            $('#cash-management').text(item.sap_code + " (" + item.name + ")");
        });
    }

    var get_tax_type = function() {
        $.get(base_url + lang + '/api/getApiTaxType/?account_group_id='+$('#account_group_id').val(), function(data){
            $("#tax_type_id").find('option').remove();
            $("#tax_type_id").append("<option value=''>Label</option>");
            var item = data.data;
            if(item.length > 0) {
                item.forEach(value => {
                    var selected = value.tax_type_id == tax_type_id ? "selected" : null;
                    $("#tax_type_id").append("<option value='"+value.tax_type_id+"' "+selected+">" + value.name + "</option>");
                    if(selected != null) {
                        var wht_type = value.wht_code ? value.wht_code : "-";
                        var wht_code = value.rates ? value.rates : "-";
                        $('#wht_type').text(wht_type);
                        $('#wht_code').text(wht_code);
                    }else{
                        $('#wht_type').text("-");
                        $('#wht_code').text("-");
                    }
                });
            } else {
                $('#wht_type').text("-");
                $('#wht_code').text("-");
            }
        });
    }

    var get_scheme_group = function(vendor_type_id) {
        $.get(base_url + lang + '/api/scheme_group/?vendor_type_id='+vendor_type_id, function(data){
            var item = data.data;
            $('#scheme_group').text(item.scheme_group_id + " (" + item.name + ")");
        });
    }

    var get_recon_acc = function(vendor_type_id) {
        $.get(base_url + lang + '/api/gl/?vendor_type_id='+vendor_type_id, function(data){
            $('#recon-acc').find('option').remove();
            var item = data.data;
            $('#recon-acc').text(item.gl_id + " (" + item.name + ")");
        });
    }

    var get_tax_number_type = function() {
        $.get(base_url + lang + '/api/GetApiTaxNumberType/?account_group_id='+$('#account_group_id').val(), function(data){
            $("#tax_number_type_id").find('option').remove();
            $("#tax_number_type_id").append("<option value=''>Label</option>");
            var item = data.data;
            item.forEach(value => {
                var selected = value.tax_number_type_id == tax_number_type_id ? "selected" : null;
                $("#tax_number_type_id").append("<option value='"+value.tax_number_type_id+"' "+selected+">" + value.name + "</option>");
            });
        });
    }

    var get_row_bank = function(id_bank_key, id_swift_code, bank_id) {
        $.get(base_url + lang + '/api/GetApiRowBank/?bank_id='+bank_id, function(data){
            var item = data.data;
            var bank_key = item.bank_id ? item.bank_id : "-";
            var swift_code = item.swift_code ? item.swift_code : "-";
            id_bank_key.text(bank_key);
            id_swift_code.text(swift_code);
        });
    }

    var btn_localforex = function(is_required, is_disabled, btn_text, btn_val) {
        $('.localforex_bank_input').prop("required", is_required).prop("disabled", is_disabled);
        $('#btn-localforex').text(btn_text);
        $("#btn-localforex").val(btn_val);
    }

    var change_acc_group_and_tax = function(country_id, vendor_type_id) {
        if (country_id == "ID")
            $('#account_group_id').val('a')
        else if ((vendor_type_id == 14 && country_id == "MY") || vendor_type_id == 4 || vendor_type_id == 5 || vendor_type_id == 10 || vendor_type_id == 12 )
            $('#account_group_id').val('b')
        else
            $('#account_group_id').val('c')

        // option tax type
        get_tax_type()
        
        // option tax num type
        get_tax_number_type()
    }

    var validation_npwp = function(is_required, remove_class, add_class, valid_npwp) {
        $('#npwp').prop("required", is_required).removeClass(remove_class).addClass(add_class);
        $('.valid-npwp').text(valid_npwp);
        $('#file_npwp').prop("required", is_required);
    }

    
    var validation_ktp = function(is_required, remove_class, add_class, valid_ktp, valid_id_ktp) {
        $('#id_card_number').prop("required", is_required).removeClass(remove_class).addClass(add_class);
        $('.valid-ktp').text(valid_ktp);
        $('#valid-id-card').text(valid_id_ktp);
    }
    
    var validation_sppkp = function(is_required, remove_class, add_class, valid_sppkp, valid_id_sppkp) {
        $('#sppkp_number').prop("required", is_required).removeClass(remove_class).addClass(add_class);
        $('.valid-sppkp').text(valid_sppkp);
        $('#valid-sppkp').text(valid_id_sppkp);
    }

    var condition_validation_ktp = function(title, country_id) {
        if((title == "Mr." && country_id == "ID") || (title == "Mr. and Mrs." && country_id == "ID") || (title == "Ms." && country_id == "ID")) {
            validation_ktp(true, "input-additional", "input-mandatory", "*", ktp)
        } else {
            validation_ktp(false, "input-mandatory", "input-additional", "", "")           
        }
    }

    var bank_key_and_swift_code = function(value_bank_key, value_swift_code) {
        var bank_key = value_bank_key ? value_bank_key : "-";
        var swift_code = value_swift_code ? value_swift_code : "-";
        $("#localidr_bank_key").text(bank_key);
        $("#localidr_swift_code").text(swift_code);
    }

    var input_bank_indonesia = function() {
        $('#file_sppkp').show();
        $('#localidr_bank').show();
        $('#foreign_bank').hide();
        $('.localidr_bank_input').prop("required", true).prop("disabled", false);
        $('.foreign_bank_input').prop("required", false).prop("disabled", true);
        if(localforex_bank_id.length != 0) {
            $('#localforex_bank').show();
            btn_localforex(true, false, btn_hide_localforex, 0)
        } else {
            $('#localforex_bank').hide();
            btn_localforex(false, true, btn_add_localforex, 1)
        }
    }

    var input_bank_not_indonesia = function() {
        $('#file_sppkp').hide();
        $('#localidr_bank').hide();
        $('#localforex_bank').hide();
        $('#foreign_bank').show();

        $('.localidr_bank_input').prop("required", false).prop("disabled", true);
        $('.localforex_bank_input').prop("required", false).prop("disabled", true);
        $('.foreign_bank_input').prop("required", true).prop("disabled", false);
    }

    // option currency
    $.get(base_url + lang + '/api/currency', function(data){
        var item = data.data;
        item.forEach(value => {
            var selected_currency = value.currency_id == currency_id ? "selected" : null;
            var selected_localforex_currency = value.currency_id == localforex_currency_id ? "selected" : null;
            var selected_foreign_currency = value.currency_id == foreign_currency_id ? "selected" : null;
            $("#currency_id").append("<option value='"+value.currency_id+"' "+selected_currency+">" + value.currency_id + "</option>");
            $("#localforex_currency_id").append("<option value='"+value.currency_id+"' "+selected_localforex_currency+">" + value.currency_id + "</option>");
            $("#foreign_currency_id").append("<option value='"+value.currency_id+"' "+selected_foreign_currency+">" + value.currency_id + "</option>");
        });
    });

    // option title
    $.get(base_url + lang + '/api/title', function(data){
        var item = data.data;
        item.forEach(value => {
            var selected = value.title_id == title_id ? "selected" : null;
            $("#title").append("<option value='"+value.title_id+"' "+selected+">" + value.title_id + "</option>");
        });
        $("#title").selectpicker('refresh');
    });
    
    // option country
    $.get(base_url + lang + '/api/country', function(data){
        var item = data.data;
        item.forEach(value => {
            var selected_country = value.country_id == country_id ? "selected" : null;
            var selected_foreign_country = value.country_id == foreign_bank_country_id ? "selected" : null;
            $("#country_id").append("<option value='"+value.country_id+"' "+selected_country+">" + value.name + "</option>");
            $("#foreign_bank_country_id").append("<option value='"+value.country_id+"' "+selected_foreign_country+">" + value.name + "</option>");
        });
    });

    // option bank
    $.get(base_url + lang + '/api/bank', function(data){
        var item = data.data;
        item.forEach(value => {
            var selected_idr = value.bank_id == localidr_bank_id ? "selected" : null;
            var selected_forex = value.bank_id == localforex_bank_id ? "selected" : null;
            $("#localidr_bank_id").append("<option value='"+value.bank_id+"' "+selected_idr+">" + value.name + "</option>");
            $("#localforex_bank_id").append("<option value='"+value.bank_id+"' "+selected_forex+">" + value.name + "</option>");
            if(selected_idr == "selected") {
                bank_key_and_swift_code(value.bank_id, value.swift_code)
            }
            if(selected_forex == "selected") {
                bank_key_and_swift_code(value.bank_id, value.swift_code)
            }
        });
    });

    // option vendor_type
    $.get(base_url + lang + '/api/vendor_type', function(data){
        var item = data.data;
        item.forEach(value => {
            var selected = value.vendor_type_id == $('#hidden_vendor_type_id').val() ? "selected" : null;
            $("#vendor_type_id").append("<option value='"+value.vendor_type_id+"' "+selected+">" + value.name + "</option>");
        });
    });

    // option vendor_type_by_id
    get_vendor_type_by_id($('#hidden_vendor_type_id').val())
    
    // option tax type
    get_tax_type();
    
    // option tax num type
    get_tax_number_type()
    
    // option sg category
    get_sg_category($('#hidden_vendor_type_id').val())

    // scheme group
    get_scheme_group($('#hidden_vendor_type_id').val())

    // recon account
    get_recon_acc($('#hidden_vendor_type_id').val())

    // option top
    $.get(base_url + lang + '/api/top', function(data){
        var item = data.data;
        $('#top_id').prop("required", true);
        item.forEach(value => {
            var selected = value.top_id == top_id ? "selected" : null;
            $('#top_id').append("<option value='"+value.top_id+"' "+selected+">" + value.name + " ("+value.top_id+")</option>");
        });
    });

    // option pic
    $.get(base_url + lang + '/api/pic', function(data){
        var item = data.data;
        $('#pic_id').prop("required", true);
        item.forEach(value => {
            var selected = value.initial_area == pic_id ? "selected" : null;
            $('#pic_id').append("<option value='"+value.initial_area+"' "+selected+">" + value.initial_area + "</option>");
        });
    });
    
    // option province
    $.get(base_url + lang + '/api/province', function(data){
        var item = data.data;
        item.forEach(value => {
            var selected = value.province_id == province_id ? "selected" : null;
            $('#partner').append("<option value='"+value.province_id+"' "+selected+">" + value.province_id + " "+value.name+"</option>");
        });
    });

    if(data_company != null) {
        var index = 0;
        var length = 0;
        data_company.forEach(company_foreach => {
            var company = '#company_id_'+index;
            var purchase_organization = '#purchase_organization_id_'+index;
            // option company
            $.get(base_url + lang + '/api/getApiCompany/?vendor_type_id='+$('#hidden_vendor_type_id').val(), function(data){
                var item = data.data;
                item.forEach(value => {
                    var selected = value.company_id == company_foreach.company_id ? "selected" : null;
                    $(company).append("<option value='"+value.company_id+"' "+selected+">" + value.name + "</option>");
                    length++;
                });
                if(length == data_company.length)
                    $("#btn_add_company").css("pointer-events", "none")
                else
                    $("#btn_add_company").css("pointer-events", "auto")
                $(company).selectpicker({maxOptions: 1}).attr("multiple", false).selectpicker('refresh');
                // option purchase organization
                get_purchase_organization_group(purchase_organization, $(company).val(), $('#hidden_vendor_type_id').val(), company_foreach.purchase_organization_id);
            });
            // option purchase organization when company onchange
            $(company).on('change', function() {
                get_purchase_organization_group(purchase_organization, $(company).val(), $('#hidden_vendor_type_id').val(), null);
            })

            index_company++;
            index++;
        });
    } else {
        // option company
        var length = 0;
        $.get(base_url + lang + '/api/getApiCompany/?vendor_type_id='+$('#hidden_vendor_type_id').val(), function(data){
            var item = data.data;
            var id = item[0].company_id;
            item.forEach(value => {
                var selected = value.company_id == id ? "selected" : null;
                $(company_id).append("<option value='"+value.company_id+"' "+selected+">" + value.name + "</option>");
                length++;
            });
            if(length == index_company + 1)
                $("#btn_add_company").css("pointer-events", "none")
            else
                $("#btn_add_company").css("pointer-events", "auto")
            $(company_id).selectpicker({maxOptions: 1}).attr("multiple", false).selectpicker('refresh');
            // option purchase organization
            get_purchase_organization_group(purchase_organization_id, id, $('#hidden_vendor_type_id').val(), null);
        });
        // option purchase organization when company onchange
        $(company_id).on('change', function() {
            get_purchase_organization_group(purchase_organization_id, $(company_id).val(), $('#hidden_vendor_type_id').val(), null);
        })
    }

    // vendor type change
    $('#vendor_type_id').on('change', function (){ 
        var vendor_type_id = $('#vendor_type_id').val();
        $('#hidden_vendor_type_id').val($('#vendor_type_id').val());
        // option sg category
        get_sg_category(vendor_type_id)
        
        if(data_company != null) {
            var length = 0;
            for (var index = 0; index < index_company; index++) {
                (function (index) {                        
                    var company = '#company_id_'+index;
                    var purchase_organization = '#purchase_organization_id_'+index;
                    $.get(base_url + lang + '/api/getApiCompany/?vendor_type_id='+vendor_type_id, function(data){
                        $(company).find('option').remove();
                        var item = data.data;
                        var id = item[0].company_id;
                        item.forEach(value => {
                            var selected = value.company_id == id ? "selected" : null;
                            $(company).append("<option value='"+value.company_id+"' "+selected+">" + value.name + "</option>");
                            length++;
                        });
                        var length_all = length / index_company
                        if(length_all == index_company || length_all < index_company)
                            $("#btn_add_company").css("pointer-events", "none")
                        else
                            $("#btn_add_company").css("pointer-events", "auto")
                        $(company).selectpicker({maxOptions: 1}).attr("multiple", false).selectpicker('refresh');
                        // option purchase organization
                        get_purchase_organization_group(purchase_organization, id, $('#hidden_vendor_type_id').val(), null);
                    })

                    // option purchase organization when company onchange
                    $(company).on('change', function() {
                        get_purchase_organization_group(purchase_organization, $(company).val(), $('#hidden_vendor_type_id').val(), null);
                    })
                })(index);
            }
        }
        // option company
        $.get(base_url + lang + '/api/getApiCompany/?vendor_type_id='+vendor_type_id, function(data){
            $(company_id).find('option').remove();
            var item = data.data;
            var id = item[0].company_id;
            item.forEach(value => {
                var selected = value.company_id == id ? "selected" : null;
                $(company_id).append("<option value='"+value.company_id+"' "+selected+">" + value.name + "</option>");
            });
            $(company_id).selectpicker({maxOptions: 1}).attr("multiple", false).selectpicker('refresh');
            // option purchaze organization
            get_purchase_organization_group(purchase_organization_id, id, $('#hidden_vendor_type_id').val(), null);
        });

        // option vendor_type_by_id
        get_vendor_type_by_id(vendor_type_id);

        // scheme group
        get_scheme_group($('#vendor_type_id').val())

        change_acc_group_and_tax(country_id, vendor_type_id)
        // recon account
        get_recon_acc($('#vendor_type_id').val())
    });

    if($('#kt_select2_email').val()) {
        list_email.forEach(item => {
            var selected = item == "" ? null : "selected";
            $('#kt_select2_email').append("<option value='"+item+"' "+selected+">" + item + "</option>");
        });
    }

    // input hidden email
    $('#kt_select2_email').on('change', function(){
        $('#hidden-email').val($('#kt_select2_email').select2('val').join(';'));
        var length = $('#hidden-email').val().length;
        if(length > 132)
            $('#valid-email').text(valid_email);
        else
            $('#valid-email').text("");
    })

    if($('#kt_select2_telephone').val()) {
        list_telephone.forEach(item => {
            var selected = item == "" ? null : "selected";
            $('#kt_select2_telephone').append("<option value='"+item+"' "+selected+">" + item + "</option>");
        });
    }

    // input hidden telephone
    $('#kt_select2_telephone').on('change', function(){
        $('#hidden-telephone').val($('#kt_select2_telephone').select2('val').join(';'));
        var length = $('#hidden-telephone').val().length;
        if(length > 60)
            $('#valid-telephone').text(valid_telephone);
        else
            $('#valid-telephone').text("");
    })

    $('#tax_type_id').on('change', function (){
        // trigger wht_type and wht_code
        $.get(base_url + lang + '/api/GetApiRowTaxType/?tax_type_id='+$('#tax_type_id').val(), function(data){
            var item = data.data;
            var wht_type = item.wht_code ? item.wht_code : "-";
            var wht_code = item.rates ? item.rates : "-";
            $('#wht_type').text(wht_type);
            $('#wht_code').text(wht_code);
        });
    });

    $('#localidr_bank_id').on('change', function (){ 
        var localidr_bank_id = $('#localidr_bank_id').val();
        // trigger bank_key and swift_code
        get_row_bank($('#localidr_bank_key'), $('#localidr_swift_code'), localidr_bank_id)
    });

    $('#localforex_bank_id').on('change', function (){ 
        var localforex_bank_id = $('#localforex_bank_id').val();
        // trigger bank_key and swift_code
        get_row_bank($('#localforex_bank_key'), $('#localforex_swift_code'), localforex_bank_id)
    });

    // Required NPWP & SPPKP
    $('#tax_number_type_id').on('change', function (){ 
        var tax_number_type_id = $('#tax_number_type_id').val();
        var account_group = $('#account_group_id').val();
        if(tax_number_type_id == 1 || tax_number_type_id == 2) {
            validation_npwp(true, "input-additional", "input-mandatory", "*")
        } else{
            validation_npwp(false, "input-mandatory", "input-additional", "")
        }
        
        if(tax_number_type_id == 2 || account_group == "b") {
            validation_sppkp(true, "input-additional", "input-mandatory", "*", sppkp)
        } else {
            validation_sppkp(false, "input-mandatory", "input-additional", "", "")
        }
    });
    
    // Required ID Card
    $('#title').on('change', function (){ 
        var title = $('#title').val();
        var country_id = $('#country_id').val();
        condition_validation_ktp(title, country_id);
    });
    
    // Required Bank Name, Bank Account, Account Holder (Local and Foreign)
    if(country_id == "ID") {
        input_bank_indonesia()
    } else {
        input_bank_not_indonesia()
    }

    if(roleId != "5"){
        $("#foreign_bank_key").hide();
        $('#foreign_bank_key').prop("required", false).prop("disabled", true);
    }

    $('#country_id').on('change', function (){ 
        var title = $('#title').val();
        var country_id = $('#country_id').val();
        var vendor_type_id = $('#vendor_type_id').val();
        if(country_id == "ID") {
            input_bank_indonesia()
        } else {
            input_bank_not_indonesia()
        }
        condition_validation_ktp(title, country_id)        
        
        change_acc_group_and_tax(country_id, vendor_type_id)
    });

    $('#btn-localforex').on('click', function (){ 
        $('#localforex_bank').toggle();
        if ($("#btn-localforex").val() == 1) {
            btn_localforex(true, false, btn_hide_localforex, 0)
        } else {
            btn_localforex(false, true, btn_add_localforex, 1)
        }
    });
    
    $('#btn_add_company').click(function(){
        if(index_company == 0) index_company = 1
        var id_purchase = '#purchase_organization_id_'+index_company;
        var id_company = '#company_id_'+index_company;
        var length = 0;
        $.get(base_url + lang + '/api/getApiCompany/?vendor_type_id='+$('#hidden_vendor_type_id').val(), function(data){
            var item = data.data;
            var id = item[0].company_id;
            $(id_company).find('option').remove();
            item.forEach(value => {
                var selected = value.company_id == id ? "selected" : null;
                $(id_company).append("<option value='"+value.company_id+"' "+selected+">" + value.name + "</option>");
                length++;
            });
            if(length == index_company)
                $("#btn_add_company").css("pointer-events", "none")
            else
                $("#btn_add_company").css("pointer-events", "auto")
            $(id_company).selectpicker({maxOptions: 1}).attr("multiple", false).selectpicker('refresh');
            // option purchase organization
            get_purchase_organization_group(id_purchase, id, $('#hidden_vendor_type_id').val(), null);
            
            // option purchaze organization when company onchange
            $(id_company).on('change', function() {
                get_purchase_organization_group(id_purchase, $(id_company).val(), $('#hidden_vendor_type_id').val(), null);
            })
        });
        
        var formCompany = '';
        if(controller == "VendorManagement" || controller == "Revision" || controller == "ChangeVendor") {
            formCompany += '\
            <div class="form-group company row">\
                <div class="col-sm-6 col-xs-12">\
                    <label>'+company+' ('+secondary+' '+index_company+')</label><br>\
                    <select class="form-control selectpicker" data-style="input-additional" asp-for="company_id" name="company_id[]" id="company_id_'+index_company+'">\
                    </select>\
                    <span class="text-danger" asp-validation-for="company_id"></span>\
                </div>\
                <div class="col-sm-5 col-xs-12">\
                    <label>'+purchase_organization+'</label><br>\
                    <select class="form-control selectpicker" data-style="input-additional" name="purchase_organization_id[]" id="purchase_organization_id_'+index_company+'">\
                    </select>\
                </div>\
                <div class="col-sm-1 col-xs-12">\
                    <button type="button" class="btn btn-secondary mt-8" style="cursor: pointer;" onclick="deleteCompany($(this))">\
                        <i class="fa fa-trash"></i>\
                    </button>\
                </div>\
            </div>\
            ';
        } else {
            formCompany += '\
            <div class="company">\
                <label>'+company+' ('+secondary+' '+index_company+')</label><br>\
                <select class="form-control select2 kt_select2_company col-10" data-style="input-additional" asp-for="company_id" name="company_id[]" id="company_id_'+index_company+'">\
                </select>\
                <button class="btn btn-secondary col-1 ml-14" style="cursor: pointer;" onclick="deleteCompany($(this))">\
                <i class="fa fa-trash"></i>\
                </button>\
            </div>\
            ';
        }
        $("#company_container").append(formCompany);
        index_company++;
        no++;
    })

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