"use strict";
// Class definition

var KTDatatableAjaxTable = function() {
	// if ( success_message != '' ) 
	// 	Swal.fire('Sukses', success_message, 'success');
    var activeDatatable, DataChangeRequestDatatable, pendingRegistrationDatatable, pendingDataChangeDatatable;
    // Private functions
    var loadActiveDatatable = function () {
        if ( activeDatatable ) {
            activeDatatable.reload();
        }
    }
    var loadDataChangeRequestDatatable = function () {
        if (DataChangeRequestDatatable){
            DataChangeRequestDatatable.reload();
        }
    }
	var deleteData = function(id) {
		Swal.fire({
		  title: 'Apakah anda yakin menghapus data ini?',
		  showCancelButton: true,
		  confirmButtonText: `Ya`,
		  cancelButtonText: `Tidak`,
		}).then(async (result) => {
		  /* Read more about isConfirmed, isDenied below */
		  if (result.isConfirmed) {
		    let doDelete = await fetch("", {
			  method: 'DELETE'
			});
		    if ( doDelete.ok ) {
				Swal.fire('Deleted!', 'Data berhasil dihapus', 'success');
				$('#kt_datatable_registration').KTDatatable().reload();
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

	// demo initializer
    var datatableInitialize = function() {
    	var pendingDatatableColumn = [
    		{
				field: 'vendor_type_name',
				title: 'Vendor Type',
                width: "300px",
                sortable: true
			},
    		{
                field: 'name',
				title: 'Name',
                width: "300px",
                sortable: true
            },
            {
                field: 'pic_id',
                title: 'Group PIC',
                width: "100px",
                sortable: true
            }, {
                field: 'actions',
				title: 'Actions',
                sortable: false,
                overflow: 'visible',
                template: function(row) {                	
					return '\
                        <a href="/' + lang + '/VendorManagement/PendingVendor/'+row.vendor_id+'" class="btn btn-sm btn-clean btn-icon mr-2" title="Lihat Detil">\
                            <span class="svg-icon svg-icon-md">\
                                <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="24px" height="24px" viewBox="0 0 24 24" version="1.1">\
                                    <g stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">\
										<rect x="0" y="0" width="24" height="24"/>\
										<path d="M6,9 L6,15 C6,16.6568542 7.34314575,18 9,18 L15,18 L15,18.8181818 C15,20.2324881 14.2324881,21 12.8181818,21 L5.18181818,21 C3.76751186,21 3,20.2324881 3,18.8181818 L3,11.1818182 C3,9.76751186 3.76751186,9 5.18181818,9 L6,9 Z" fill="#000000" fill-rule="nonzero"/>\
										<path d="M10.1818182,4 L17.8181818,4 C19.2324881,4 20,4.76751186 20,6.18181818 L20,13.8181818 C20,15.2324881 19.2324881,16 17.8181818,16 L10.1818182,16 C8.76751186,16 8,15.2324881 8,13.8181818 L8,6.18181818 C8,4.76751186 8.76751186,4 10.1818182,4 Z" fill="#000000" opacity="0.3"/>\
									</g>\
                                </svg>\
                            </span>\
                        </a>\
                    ';
                },
            }];
        var changeDatatableColumn = [
            {
                field: 'vendor_number',
                title: 'Vendor Number',
                // width: "120px",
                sortable: true
            },
            {
                field: 'vendor_type_name',
                title: 'Vendor Type',
                // width: "300px",
                sortable: true
            },
            {
                field: 'name',
                title: 'Name',
                // width: "200px",
                sortable: true
            },
            {
                field: 'is_extension',
                title: 'Extension',
                // width: "50px",
                template: function (row) {
                    return row.is_extension ? "&#10003;" : null;
                },
                sortable: true
            },
            {
                field: 'pic_id',
                title: 'Group PIC',
                // width: "100px",
                sortable: true
            }, {
                field: 'actions',
                title: 'Actions',
                sortable: false,
                overflow: 'visible',
                template: function (row) {
                    return '\
                        <a href="/' + lang + '/VendorManagement/PendingVendor/'+ row.vendor_id + '" class="btn btn-sm btn-clean btn-icon mr-2" title="Lihat Detil">\
                            <span class="svg-icon svg-icon-md">\
                                <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="24px" height="24px" viewBox="0 0 24 24" version="1.1">\
                                    <g stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">\
										<rect x="0" y="0" width="24" height="24"/>\
										<path d="M6,9 L6,15 C6,16.6568542 7.34314575,18 9,18 L15,18 L15,18.8181818 C15,20.2324881 14.2324881,21 12.8181818,21 L5.18181818,21 C3.76751186,21 3,20.2324881 3,18.8181818 L3,11.1818182 C3,9.76751186 3.76751186,9 5.18181818,9 L6,9 Z" fill="#000000" fill-rule="nonzero"/>\
										<path d="M10.1818182,4 L17.8181818,4 C19.2324881,4 20,4.76751186 20,6.18181818 L20,13.8181818 C20,15.2324881 19.2324881,16 17.8181818,16 L10.1818182,16 C8.76751186,16 8,15.2324881 8,13.8181818 L8,6.18181818 C8,4.76751186 8.76751186,4 10.1818182,4 Z" fill="#000000" opacity="0.3"/>\
									</g>\
                                </svg>\
                            </span>\
                        </a>\
                    ';
                },
            }];

		var datatable = $('#kt_datatable_vendor_pending').KTDatatable({
			data: {
				type: 'remote',
                source: {
                    read: {
                    	method: 'GET',
                        url: '/' + lang + '/VendorManagement/pending_vendor_datatable?type=' + escape("New Registration") + '&',
                        map: function(raw) {
                            var dataSet = raw;
                            if (typeof raw.data !== 'undefined') {
                                dataSet = raw.data;
                                console.log("datapending : ", dataSet)
                            }
							return dataSet;
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
                input: $('#kt_datatable_search_query'),
                key: 'generalSearch'
            },
            // columns definition
            columns: pendingDatatableColumn,
		});

		pendingDataChangeDatatable = $('#kt_datatable_vendor_request').KTDatatable({
			data: {
				type: 'remote',
                source: {
                    read: {
                        method: 'GET',
                        url: '/' + lang + '/VendorManagement/pending_vendor_datatable?type=' + escape("Vendor Data Change") + '&',
                        map: function(raw) {
                            var dataSet = raw;
                            if (typeof raw.data !== 'undefined') {
                                dataSet = raw.data;
                                console.log("datachange : ", dataSet)
                            }
							return dataSet;
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
                input: $('#kt_datatable_search_query1'),
                key: 'generalSearch'
            },
			// columns definition
            columns: changeDatatableColumn,
		});
        var activeDatatableColumn = [
            {
                field: 'vendor_number',
                title: 'Vendor Number',
                width: "200px",
                sortable: false
            },
            {
                field: 'vendor_type_name',
                title: 'Vendor Type',
                width: "300px",
                sortable: false
            },
            {
                field: 'name',
                title: 'Name',
                width: "300px",
                sortable: false
            }, {
                field: 'actions',
                title: 'Actions',
                sortable: false,
                overflow: 'visible',
                template: function (row) {
                    return '\
                        <a href="/' + lang + '/VendorManagement/ActiveVendor/'+row.vendor_number+'" class="btn btn-sm btn-clean btn-icon mr-2" title="Lihat Detil">\
                            <span class="svg-icon svg-icon-md">\
                                <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="24px" height="24px" viewBox="0 0 24 24" version="1.1">\
                                    <g stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">\
										<rect x="0" y="0" width="24" height="24"/>\
										<path d="M6,9 L6,15 C6,16.6568542 7.34314575,18 9,18 L15,18 L15,18.8181818 C15,20.2324881 14.2324881,21 12.8181818,21 L5.18181818,21 C3.76751186,21 3,20.2324881 3,18.8181818 L3,11.1818182 C3,9.76751186 3.76751186,9 5.18181818,9 L6,9 Z" fill="#000000" fill-rule="nonzero"/>\
										<path d="M10.1818182,4 L17.8181818,4 C19.2324881,4 20,4.76751186 20,6.18181818 L20,13.8181818 C20,15.2324881 19.2324881,16 17.8181818,16 L10.1818182,16 C8.76751186,16 8,15.2324881 8,13.8181818 L8,6.18181818 C8,4.76751186 8.76751186,4 10.1818182,4 Z" fill="#000000" opacity="0.3"/>\
									</g>\
                                </svg>\
                            </span>\
                        </a>\
                    ';
                },
            }];
		activeDatatable = $('#kt_datatable_vendor_active').KTDatatable({
			data: {
				type: 'remote',
                source: {
                    read: {
                    	method: 'GET',
                        url: '/' + lang +'/VendorManagement/active_vendor_datatable',
                        map: function(raw) {
                            var dataSet = raw;
                            if (typeof raw.data !== 'undefined') {
                                dataSet = raw.data;
                                console.log("active:", dataSet)
                            }
							return dataSet;
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
                input: $('#kt_datatable_search_query2'),
                key: 'generalSearch'
            },
			// columns definition
            columns: activeDatatableColumn,
		});


        //data changes

        var DataChangeRequestDatatableColumn = [
            {
                field: 'vendor_number',
                title: 'Vendor Number',
                // width: "120px",
                sortable: false
            },
            {
                field: 'vendor_type_name',
                title: 'Vendor Type',
                // width: "300px",
                sortable: false
            },
            {
                field: 'name',
                title: 'Name',
                // width: "200px",
                sortable: false
            },
            {
                field: 'city',
                title: 'City',
                // width: "100px",
                sortable: false
            },
            {
                field: 'country_name',
                title: 'Country',
                // width: "100px",
                sortable: false
            }, {
                field: 'actions',
                title: 'Actions',
                sortable: false,
                overflow: 'visible',
                template: function (row) {
                    return '\
                        <a href="/' + lang + '/VendorManagement/DataChangeRequest/'+row.vendor_number+'" class="btn btn-sm btn-clean btn-icon mr-2" title="Lihat Detil">\
                            <span class="svg-icon svg-icon-md">\
                                <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="24px" height="24px" viewBox="0 0 24 24" version="1.1">\
                                    <g stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">\
										<rect x="0" y="0" width="24" height="24"/>\
										<path d="M6,9 L6,15 C6,16.6568542 7.34314575,18 9,18 L15,18 L15,18.8181818 C15,20.2324881 14.2324881,21 12.8181818,21 L5.18181818,21 C3.76751186,21 3,20.2324881 3,18.8181818 L3,11.1818182 C3,9.76751186 3.76751186,9 5.18181818,9 L6,9 Z" fill="#000000" fill-rule="nonzero"/>\
										<path d="M10.1818182,4 L17.8181818,4 C19.2324881,4 20,4.76751186 20,6.18181818 L20,13.8181818 C20,15.2324881 19.2324881,16 17.8181818,16 L10.1818182,16 C8.76751186,16 8,15.2324881 8,13.8181818 L8,6.18181818 C8,4.76751186 8.76751186,4 10.1818182,4 Z" fill="#000000" opacity="0.3"/>\
									</g>\
                                </svg>\
                            </span>\
                        </a>\
                    ';
                },
            }];
		DataChangeRequestDatatable = $('#kt_datatable_vendor_request').KTDatatable({
			data: {
				type: 'remote',
                source: {
                    read: {
                    	method: 'GET',
                        url: '/' + lang +'/VendorManagement/data_change_request_datatable',
                        map: function(raw) {
                            var dataSet = raw;
                            if (typeof raw.data !== 'undefined') {
                                dataSet = raw.data;
                            }
                            console.log("data : ", dataSet)
							return dataSet;
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
                input: $('#kt_datatable_search_query'),
                key: 'generalSearch'
            },
			// columns definition
            columns: DataChangeRequestDatatableColumn,
		});

        var params = activeDatatable.getDataSourceParam();
		
		$('.select2').select2({
            width: "100%"
        });
		
        $('#kt_search_datatable').on('click', function() {
            console.log("masuk")
            activeDatatable.load();
            ChangeRequestDatatable.load();
        });
    };

    return {
        // Public functions
        init: function() {
            // init datatable
            datatableInitialize();
        },
        loadActiveDatatable,
        loadDataChangeRequestDatatable,
		deleteData,
    };
}();

jQuery(document).ready(function() {
	KTDatatableAjaxTable.init();
});