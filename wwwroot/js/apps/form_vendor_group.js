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

  // option scheme_group id
  $.get("/id/api/m_scheme_group", function (data) {
    var item = data.data;
    item.forEach((value) => {
      var selected_scheme_group_id =
        value.scheme_group_id == $("#hidden_scheme_group_id").val()
          ? "selected"
          : null;
      $("#scheme_group_id").append(
        "<option value='" +
          value.scheme_group_id +
          "' " +
          selected_scheme_group_id +
          ">" +
          value.scheme_group_id +
          "</option>"
      );
    });
    $("#scheme_group_id").selectpicker({ maxOptions: 1 });
    $("#scheme_group_id").selectpicker("refresh");
  });
  $.get("/id/api/m_gl", function (data) {
    var item = data.data;
    item.forEach((value) => {
      var selected_gl_id =
        value.gl_id == $("#hidden_gl_id").val()
          ? "selected"
          : null;
      $("#gl_id").append(
        "<option value='" +
          value.gl_id +
          "' " +
          selected_gl_id +
          ">" +
          value.gl_id +
          "</option>"
      );
    });
    $("#gl_id").selectpicker({ maxOptions: 1 });
    $("#gl_id").selectpicker("refresh");
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
