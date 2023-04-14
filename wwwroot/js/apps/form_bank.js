		var KTForm = (function () {
  // Private functions
  var demos = function () {};
  let result = "";
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

    // console.log("result",result)

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
