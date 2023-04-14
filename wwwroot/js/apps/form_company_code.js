var KTForm = (function () {
  // Private functions
  var demos = function () {};
  let result = "";
  // option role
  $.get("/id/api/user_role", function (data) {
    var item = data.data;
    item.forEach((value) => {
      var selected_role =
        value.role_id == $("#hidden_role").val() ? "selected" : null;
      $("#role_id_0").append(
        "<option value='" +
          value.role_id +
          "' " +
          selected_role +
          ">" +
          value.role_name +
          "</option>"
      );
    });
    $("#role_id_0").selectpicker({ maxOptions: 1 });
    $("#role_id_0").selectpicker("refresh");
  });

  // option ountry id
  $.get("/id/api/country", function (data) {
    var item = data.data;
    item.forEach((value) => {
      var selected_country_id =
        value.country_id == $("#hidden_country_of_origin").val()
          ? "selected"
          : null;
      $("#country_of_origin").append(
        "<option value='" +
          value.country_id +
          "' " +
          selected_country_id +
          ">" +
          value.country_id +
          "</option>"
      );
    });
    $("#country_of_origin").selectpicker({ maxOptions: 1 });
    $("#country_of_origin").selectpicker("refresh");
  });

  $("input[type=radio]").change(function () {
    let value = this.value;
    if (this.checked) {
      result += value + ";";
    }
    // console.log("result",result);
    $("#hidden-company_area").val(result);
  });

  // Public functions
  return {
    init: function () {
      demos();
      // modalDemos();
    },
  };
})();

// Initialization
jQuery(document).ready(function () {
  KTForm.init();
});
