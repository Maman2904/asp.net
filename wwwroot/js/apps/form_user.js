var KTForm = function() {
    // Private functions
    var demos = function() {

    }
    let result = "";
    // option role
    $.get('/id/api/user_role', function(data){
        var item = data.data;
        item.forEach(value => {
            var selected_role = value.role_id == $('#hidden_role').val() ? "selected" : null;
            $('#role_id_0').append("<option value='"+value.role_id+"' "+selected_role+">" + value.role_name + "</option>");
        });
        $('#role_id_0').selectpicker({maxOptions: 1});
        $('#role_id_0').selectpicker('refresh');
    });

    // option procurement - group
    $.get('/id/api/procurement_group', function(data){
        var item = data.data;
        item.forEach(value => {
            var selected_area = value.initial_area == $('#hidden_initial_area').val() ? "selected" : null;
            $('#initial_area').append("<option value='"+value.initial_area+"' "+selected_area+">" + value.initial_area + " - " + value.person_number + "</option>");
        });
        $('#initial_area').selectpicker({maxOptions: 1});
        $('#initial_area').selectpicker('refresh');
    });

    $('input[type=radio]').change(
        function(){
            let value = this.value
            if (this.checked) {
                result += value + ";";
            } 
            // console.log("result",result);
            $('#hidden-company_area').val(result);
    });

    $('#kt_select2_company_area').on('change', function() {
        var fileName = $(this).val();
        console.log("file: ", fileName);
    });

   
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
$('#kt_select2_company_area').select2();
});
