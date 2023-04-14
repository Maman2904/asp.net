"use strict";
// Class definition
var DeleteCompanyCode = async function(id) {
	console.log("masukkk")
	Swal.fire({
		title: 'Apakah anda yakin menghapus data ini?',
		showCancelButton: true,
		confirmButtonText: `Ya`,
		cancelButtonText: `Tidak`,
	  }).then(async (result) => {
		/* Read more about isConfirmed, isDenied below */
		if (result.isConfirmed) {
			let doDelete = await fetch("/"+lang+"/TableManagement/DeleteCompanyCode/"+id, {
				method: 'DELETE'
			});
			console.log("delete response: ")
      alert("Sukses")
			if ( doDelete.ok ) {
      location.reload();
				myTable.ajax.reload();
				Swal.fire('Deleted!', 'Data berhasil dihapus', 'success');
				$('#kt_datatable_company_code_all').KTDatatable().reload();
			} else {
				Swal.fire({
					text: "Terjadi kesalahan, silahkan coba lagi atau hubungi administrator.",
					icon: "error",
					buttonsStyling: false,
					confirmButtonText: "Ok",
					customClass: {
						confirmButton: "btn font-weight-bold btn-light-primary"
					}
				});
			}
		  
		}
	  })	
}

var KTDatatableAjaxTable = (function () {
  // if ( success_message != '' )
  // 	Swal.fire('Sukses', success_message, 'success');
  var datatable;
  var loadActiveDataTable = function () {
    if (datatable) {
      datatable.reload();
    }
  };

  // demo initializer
  var datatableInitialize = function () {
    var datatableColumn = [
      {
        field: "company_id",
        title: "Company Code",
        // width: "80px",
        sortable: true,
      },
      {
        field: "name",
        title: "Name",
        // width: "80px",
        sortable: true,
      },
      {
        field: "actions",
        title: "Actions",
        width: "30%",
        sortable: false,
        overflow: "visible",
        template: function (row) {
          // if (row.role == "Admin") {
          //   return (
          //     '\
          // 	<a href="/' +
          //     lang +
          //     "/TableManagement/Bank/Detil/" +
          //     row.bank_id +
          //     '"><i class="flaticon2-paper"></i></a>\
          // 	'
          //   );
          // }
            return '\
            <a href=/'+lang+'/TableManagement/CompanyCode/Edit/'+row.company_id+' class="btn btn-sm btn-clean btn-icon mr-2" title="Lihat Detil">\
							<span class="svg-icon svg-icon-md">\
								<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" fill="currentColor" class="bi bi-pencil" viewBox="0 0 20 20">\
									<path d="M12.146.146a.5.5 0 0 1 .708 0l3 3a.5.5 0 0 1 0 .708l-10 10a.5.5 0 0 1-.168.11l-5 2a.5.5 0 0 1-.65-.65l2-5a.5.5 0 0 1 .11-.168l10-10zM11.207 2.5 13.5 4.793 14.793 3.5 12.5 1.207 11.207 2.5zm1.586 3L10.5 3.207 4 9.707V10h.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.5h.293l6.5-6.5zm-9.761 5.175-.106.106-1.528 3.821 3.821-1.528.106-.106A.5.5 0 0 1 5 12.5V12h-.5a.5.5 0 0 1-.5-.5V11h-.5a.5.5 0 0 1-.468-.325z"/>\
								</svg>\
							</span>\
            </a>\
            <button'+ ' class="btn btn-sm btn-clean btn-icon mr-2 deleteCompany" title="Delete" onclick=DeleteCompanyCode(' + row.company_id + ') data-cc_id='+row.company_id+'>\
							<span class="svg-icon svg-icon-md">\
								<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" fill="currentColor" class="bi bi-trash" viewBox="0 0 20 20">\
									<path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0V6z"/>\
									<path fill-rule="evenodd" d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118zM2.5 3V2h11v1h-11z"/>\
								</svg>\
              </span>\
            </button>\
                    ';
        },
      },
    ];
    datatable = $("#kt_datatable_company_code_all").KTDatatable({
      data: {
        type: "remote",
        source: {
          read: {
            method: "GET",
            url: "/" + lang + "/TableManagement/all_company_code_datatable",
            map: function (raw) {
              var dataSet = raw;
              if (typeof raw.data !== "undefined") {
                dataSet = raw.data;
              }
              // console.log(dataSet, "dataset")
              return dataSet;
            },
            error: function (xhr, error, thrown) {
              console.log("Error occurred!");
              console.log(xhr, error, thrown);
            },
          },
        },
        pageSize: 10,
        serverPaging: true,
        serverFiltering: true,
        serverSorting: true,
      },

      // layout definition
      layout: {
        scroll: false,
        footer: false,
      },

      // column sorting
      sortable: true,

      pagination: true,

      search: {
        input: $("#kt_datatable_search_query"),
        key: "generalSearch",
      },
      // columns definition
      columns: datatableColumn,
    });

    var params = datatable.getDataSourceParam();

    $(".select2").select2({
      width: "100%",
    });

    $("#kt_search_datatable").on("click", function () {
      datatable.load();
    });
  };

  return {
    // Public functions
    init: function () {
      // init datatable
      datatableInitialize();
    },
    loadActiveDataTable,
    // deleteData
  };
})();

jQuery(document).ready(function () {
  KTDatatableAjaxTable.init();
});
