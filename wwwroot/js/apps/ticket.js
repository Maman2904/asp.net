"use strict";
// Class definition

var KTDatatableAjaxTable = function() {
    // Private functions
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
				$('#kt_datatable_ticketing_barang_pending').KTDatatable().reload();
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

	var datatableInitialize = function() {
        var print = function(isPrint) {
            if(role_id == "4" || isPrint) {
                $('.dataTables_filter')
                    .html('<a href="'+base_url + lang + '/Ticket/PrintTicket/' +'" target="_blank"><button class="btn btn-primary" style="background-color:#8950FC"><i class="fa fa-print"></i> Print</button></a>');
            } else {
                $('.dataTables_filter')
                    .html("");
            }
        }
        // Begin filter per column
        $('#kt_datatable_draft thead tr').clone(true).appendTo( '#kt_datatable_draft thead' );
        $('#kt_datatable_draft thead tr:eq(1) th').each( function (i) {
            var title = $(this).text();
            if(title !== 'Actions' && title !== 'Aksi')
                $(this).html( '<input type="text" placeholder="Search '+title+'" />' );
            else
                $(this).text( "" );

            $( 'input', this ).on( 'keyup change', function () {
                if ( draftDatatable.column(i).search() !== this.value ) {
                    draftDatatable
                        .column(i)
                        .search( this.value )
                        .draw();
                }
            } );
        } );

        $('#kt_datatable_process thead tr').clone(true).appendTo( '#kt_datatable_process thead' );
        $('#kt_datatable_process thead tr:eq(1) th').each( function (i) {
            var title = $(this).text();
            if(title !== 'Actions' && title !== 'Aksi')
                $(this).html( '<input type="text" placeholder="Search '+title+'" />' );
            else
                $(this).text( "" );

            $( 'input', this ).on( 'keyup change', function () {
                if ( processDatatable.column(i).search() !== this.value ) {
                    processDatatable
                        .column(i)
                        .search( this.value )
                        .draw();
                }
            } );
        } );

        $('#kt_datatable_need_revision thead tr').clone(true).appendTo( '#kt_datatable_need_revision thead' );
        $('#kt_datatable_need_revision thead tr:eq(1) th').each( function (i) {
            var title = $(this).text();
            if(title !== 'Actions' && title !== 'Aksi')
                $(this).html( '<input type="text" placeholder="Search '+title+'" />' );
            else
                $(this).text( "" );

            $( 'input', this ).on( 'keyup change', function () {
                if ( revisionDatatable.column(i).search() !== this.value ) {
                    revisionDatatable
                        .column(i)
                        .search( this.value )
                        .draw();
                }
            } );
        } );

        $('#kt_datatable_rejected thead tr').clone(true).appendTo( '#kt_datatable_rejected thead' );
        $('#kt_datatable_rejected thead tr:eq(1) th').each( function (i) {
            var title = $(this).text();
            if(title !== 'Actions' && title !== 'Aksi')
                $(this).html( '<input type="text" placeholder="Search '+title+'" />' );
            else
                $(this).text( "" );

            $( 'input', this ).on( 'keyup change', function () {
                if ( rejectedDatatable.column(i).search() !== this.value ) {
                    rejectedDatatable
                        .column(i)
                        .search( this.value )
                        .draw();
                }
            } );
        } );
        
        $('#kt_datatable_pending thead tr').clone(true).appendTo( '#kt_datatable_pending thead' );
        $('#kt_datatable_pending thead tr:eq(1) th').each( function (i) {
            var title = $(this).text();
            if(title !== 'Actions' && title !== 'Aksi')
                $(this).html( '<input type="text" placeholder="Search '+title+'" />' );
            else
                $(this).text( "" );

            $( 'input', this ).on( 'keyup change', function () {
                if ( pendingDatatable.column(i).search() !== this.value ) {
                    pendingDatatable
                        .column(i)
                        .search( this.value )
                        .draw();
                }
            } );
        } );
        
        $('#kt_datatable_error thead tr').clone(true).appendTo( '#kt_datatable_error thead' );
        $('#kt_datatable_error thead tr:eq(1) th').each( function (i) {
            var title = $(this).text();
            if(title !== 'Actions' && title !== 'Aksi')
                $(this).html( '<input type="text" placeholder="Search '+title+'" />' );
            else
                $(this).text( "" );

            $( 'input', this ).on( 'keyup change', function () {
                if ( errorDatatable.column(i).search() !== this.value ) {
                    errorDatatable
                        .column(i)
                        .search( this.value )
                        .draw();
                }
            } );
        } );

        $('#kt_datatable_need_reversal thead tr').clone(true).appendTo( '#kt_datatable_need_reversal thead' );
        $('#kt_datatable_need_reversal thead tr:eq(1) th').each( function (i) {
            var title = $(this).text();
            if(title !== 'Actions' && title !== 'Aksi')
                $(this).html( '<input type="text" placeholder="Search '+title+'" />' );
            else
                $(this).text( "" );

            $( 'input', this ).on( 'keyup change', function () {
                if ( reversalDatatable.column(i).search() !== this.value ) {
                    reversalDatatable
                        .column(i)
                        .search( this.value )
                        .draw();
                }
            } );
        } );
        
        $('#kt_datatable_simulation thead tr').clone(true).appendTo( '#kt_datatable_simulation thead' );
        $('#kt_datatable_simulation thead tr:eq(1) th').each( function (i) {
            var title = $(this).text();
            if(title !== 'Actions' && title !== 'Aksi')
                $(this).html( '<input type="text" placeholder="Search '+title+'" />' );
            else
                $(this).text( "" );

            $( 'input', this ).on( 'keyup change', function () {
                if ( simulationDatatable.column(i).search() !== this.value ) {
                    simulationDatatable
                        .column(i)
                        .search( this.value )
                        .draw();
                }
            } );
        } );

        $('#kt_datatable_all thead tr').clone(true).appendTo( '#kt_datatable_all thead' );
        $('#kt_datatable_all thead tr:eq(1) th').each( function (i) {
            var title = $(this).text();
            if(title !== 'Actions' && title !== 'Aksi')
                $(this).html( '<input type="text" placeholder="Search '+title+'" />' );
            else
                $(this).text( "" );

            $( 'input', this ).on( 'keyup change', function () {
                if ( allDatatable.column(i).search() !== this.value ) {
                    allDatatable
                        .column(i)
                        .search( this.value )
                        .draw();
                }
            } );
        } );

        var firstColumn = [{
            targets: 0,
            data: 'start_date'
        },
        {
            targets: 1,
            data: 'received_date'
        },
        {
            targets: 2,
            data: 'ticket_number'
        }]

        var fifthColumn = [{
            targets: 4,
            data: 'vendor_number'
        },
        {
            targets: 5,
            data: 'name'
        },
        {
            targets: 6,
            data: 'invoice_number'
        },
        {
            targets: 7,
            data: 'amount',
            className: 'text-right'
        }]

        var invoice_number = [];
        if(role_id == "2" || role_id == "6") {
            invoice_number = [{
                targets: 3,
                data: 'invoice_number'
            }]
        }
        
        var for_accounting = [];
        var for_another_role = [];
        if(role_id == "6") {
            for_accounting = [{
                targets: invoice_number.length ? 5 : 4,
                data: 'miro_number'
            }, {
                targets: invoice_number.length ? 6 : 5,
                data: 'name'
            }, {
                targets: invoice_number.length ? 7 : 6,
                data: 'posting_date'
            }]
        } else if(role_id == "1" || role_id == "3" || role_id == "4" || role_id == "7") {
            for_another_role = [{
                targets: invoice_number.length ? 5 : 4,
                data: 'remmitance_number'
            }, {
                targets: invoice_number.length ? 6 : 5,
                data: 'remmitance_date'
            }]
        }

        var draftDatatable = $('#kt_datatable_draft').DataTable({
            scrollX: true,
            scrollCollapse: true,
            // ajax: {
			// 	url: base_url + '/' + lang + '/api/',
			// 	type: 'POST',
			// 	data: {
			// 		// parameters for custom backend script demo
			// 		columnsDef: [
			// 			'start_date', 'received_date',
			// 			'ticket_number', 'status', 'vendor_number',
			// 			'name', 'invoice_number', 'amount'],
			// 	},
			// },
            data: [{'start_date':'21 March 2021', 'received_date':'24 March 2021', 'ticket_number':'8210202107-0001', 'status': 1, 'vendor_number':'01-2012-0001', 'name':'PT Arifindo Teknologi', 'invoice_number':'INV-001-IV-2392', 'amount':'Rp 1,502,200,000'}],
            columnDefs: [
                ...firstColumn,
                {
                    targets: 3,
                    data: {status: 'status', position: 'position'},
                    render: function (data, row, type) {
                        var status = {
							1: "Draft",
							2: "Rejected",
							3: "Revise",
						};
                        return status[data.status];
                    }
                },
                ...fifthColumn,
                {
                    targets: -1,
                    orderable: false,
                    className: 'text-right',
                    data: 'ticket_number',
                    render: function(data, row, type) {
                        return '\
                            <a href="/'+ lang +'/Ticket/Detil/'+ data +'"><i class="flaticon2-writing"></i></a>\
                        ';
                    }
                }
            ],
            initComplete: print(false)
        });
        
        var processDatatable = $('#kt_datatable_process').DataTable({
            scrollX: true,
            scrollCollapse: true,
            data: [{'start_date':'21 March 2021', 'received_date':'24 March 2021', 'ticket_number':'8210202107-0001', 'status': 1, 'vendor_number':'01-2012-0001', 'name':'PT Arifindo Teknologi', 'invoice_number':'INV-001-IV-2392', 'amount':'Rp 1,502,200,000'}],
            columnDefs: [
                ...firstColumn,
                {
                    targets: 3,
                    data: {status: 'status', position: 'position'},
                    render: function (data, row, type) {
                        var status = {
							1: "Submitted",
							2: "Rejected",
							3: "Revise",
						};
                        return status[data.status];
                    }
                },
                ...fifthColumn,
                {
                    targets: -1,
                    orderable: false,
                    className: 'text-right',
                    data: 'ticket_number',
                    render: function(data, row, type) {
                        return '\
                            <a href="/'+ lang +'/Ticket/Detil/'+ data +'"><i class="flaticon2-paper"></i></a>\
                        ';
                    }
                }
            ],
            initComplete : print(false)
        });
        
        var revisionDatatable = $('#kt_datatable_need_revision').DataTable({
            scrollX: true,
            scrollCollapse: true,
            data: [{'start_date':'21 March 2021', 'received_date':'24 March 2021', 'ticket_number':'8210202107-0001', 'vendor_number':'01-2012-0001', 'name':'PT Arifindo Teknologi', 'invoice_number':'INV-001-IV-2392', 'amount':'Rp 1,502,200,000'}],
            columnDefs: [
                ...firstColumn,
                {
                    targets: 3,
                    data: 'vendor_number'
                },
                {
                    targets: 4,
                    data: 'name'
                },
                {
                    targets: 5,
                    data: 'invoice_number'
                },
                {
                    targets: 6,
                    data: 'amount',
                    className: 'text-right'
                },
                {
                    targets: -1,
                    orderable: false,
                    className: 'text-right',
                    data: 'ticket_number',
                    render: function(data, row, type) {
                        return '\
                            <a href="/'+ lang +'/Ticket/Detil/'+ data +'"><i class="flaticon2-paper"></i></a>\
                        ';
                    }
                }
            ],
            initComplete : print(false)
        });
        
        var rejectedDatatable = $('#kt_datatable_rejected').DataTable({
            scrollX: true,
            scrollCollapse: true,
            data: [{'start_date':'21 March 2021', 'received_date':'24 March 2021', 'ticket_number':'8210202107-0001', 'vendor_number':'01-2012-0001', 'name':'PT Arifindo Teknologi', 'invoice_number':'INV-001-IV-2392', 'amount':'Rp 1,502,200,000'}],
            columnDefs: [
                ...firstColumn,
                {
                    targets: 3,
                    data: 'vendor_number'
                },
                {
                    targets: 4,
                    data: 'name'
                },
                {
                    targets: 5,
                    data: 'invoice_number'
                },
                {
                    targets: 6,
                    data: 'amount',
                    className: 'text-right'
                },
                {
                    targets: -1,
                    orderable: false,
                    className: 'text-right',
                    data: 'ticket_number',
                    render: function(data, row, type) {
                        return '\
                            <a href="/'+ lang +'/Ticket/Detil/'+ data +'"><i class="flaticon2-paper"></i></a>\
                        ';
                    }
                }
            ],
            initComplete : print(false)
        });

        var pendingDatatable = $('#kt_datatable_pending').DataTable({
            scrollX: true,
            scrollCollapse: true,
            data: [{'start_date':'21 March 2021', 'received_date':'24 March 2021', 'ticket_number':'8210202107-0001', 'status': 1, 'position': 3, 'vendor_number':'01-2012-0001', 'name':'PT Arifindo Teknologi', 'invoice_number':'INV-001-IV-2392', 'amount':'Rp 1,502,200,000'}],
            columnDefs: [
                ...firstColumn,
                {
                    targets: 3,
                    data: {status: 'status', position: 'position'},
                    render: function (data, row, type) {
                        var status = {
							1: "Submitted",
							2: "Rejected",
							3: "Revise",
						};
                        var position = {
							3: "Procurement",
							5: "FC"
						};
                        return '\
                        '+status[data.status]+'<br/><strong>Position</strong>: '+position[data.position]+'\
                        ';
                    }
                },
                ...fifthColumn,
                {
                    targets: -1,
                    orderable: false,
                    className: 'text-right',
                    data: 'ticket_number',
                    render: function(data, row, type) {
                        return '\
                            <a href="/'+ lang +'/Ticket/Detil/'+ data +'"><i class="flaticon2-paper"></i></a>\
                        ';
                    }
                }
            ],
            initComplete : print(true)
        });
        
        var errorDatatable = $('#kt_datatable_error').DataTable({
            scrollX: true,
            scrollCollapse: true,
            data: [{'start_date':'21 March 2021', 'received_date':'24 March 2021', 'ticket_number':'8210202107-0001', 'status': 1, 'position': 3, 'vendor_number':'01-2012-0001', 'name':'PT Arifindo Teknologi', 'invoice_number':'INV-001-IV-2392', 'amount':'Rp 1,502,200,000'}],
            columnDefs: [
                ...firstColumn,
                {
                    targets: 3,
                    data: {status: 'status', position: 'position'},
                    render: function (data, row, type) {
                        var status = {
							1: "Submitted",
							2: "Rejected",
							3: "Revise",
						};
                        var position = {
							3: "Procurement",
							5: "FC"
						};
                        return '\
                        '+status[data.status]+'<br/><strong>Position</strong>: '+position[data.position]+'\
                        ';
                    }
                },
                ...fifthColumn,
                {
                    targets: -1,
                    orderable: false,
                    className: 'text-right',
                    data: 'ticket_number',
                    render: function(data, row, type) {
                        return '\
                            <a href="/'+ lang +'/Ticket/Detil/'+ data +'"><i class="flaticon2-paper"></i></a>\
                        ';
                    }
                }
            ],
            initComplete : print(false)
        });
        
        var reversalDatatable = $('#kt_datatable_need_reversal').DataTable({
            scrollX: true,
            scrollCollapse: true,
            data: [{'start_date':'21 March 2021', 'received_date':'24 March 2021', 'ticket_number':'8210202107-0001', 'status': 1, 'position': 3, 'vendor_number':'01-2012-0001', 'name':'PT Arifindo Teknologi', 'invoice_number':'INV-001-IV-2392', 'amount':'Rp 1,502,200,000'}],
            columnDefs: [
                ...firstColumn,
                {
                    targets: 3,
                    data: {status: 'status', position: 'position'},
                    render: function (data, row, type) {
                        var status = {
							1: "Submitted",
							2: "Rejected",
							3: "Revise",
						};
                        var position = {
							3: "Procurement",
							5: "FC"
						};
                        return '\
                        '+status[data.status]+'<br/><strong>Position</strong>: '+position[data.position]+'\
                        ';
                    }
                },
                ...fifthColumn,
                {
                    targets: -1,
                    orderable: false,
                    className: 'text-right',
                    data: 'ticket_number',
                    render: function(data, row, type) {
                        return '\
                            <a href="/'+ lang +'/Ticket/Detil/'+ data +'"><i class="flaticon2-paper"></i></a>\
                        ';
                    }
                }
            ],
            initComplete : print(false)
        });

        var simulationDatatable = $('#kt_datatable_simulation').DataTable({
            scrollX: true,
            scrollCollapse: true,
            data: [{'start_date':'21 March 2021', 'received_date':'24 March 2021', 'ticket_number':'8210202107-0001', 'status':1, 'miro_number': '12345678909876543','name':'PT Arifindo Teknologi', 'posting_date': '25 March 2021', 'remmitance_number': '1234567890', 'remmitance_date': '25 March 2021', 'invoice_number':'INV-001-IV-2392', 'amount':'Rp 1,502,200,000'}],
            columnDefs: [
                ...firstColumn,
                {
                    targets: -2,
                    data: {status: 'status', position: 'position'},
                    render: function (data, row, type) {
                        var status = {
							1: "Approved For Payment",
							2: "Rejected",
							3: "Revise",
						};
                        return status[data.status];
                    }
                },
                ...invoice_number,
                {
                    targets: invoice_number.length ? 4 : 3,
                    data: 'amount'
                },
                ...for_accounting,
                {
                    targets: -1,
                    className: 'text_right',
                    orderable: false,
                    data: 'ticket_number',
                    render: function(data, row, type, full, meta) {
                        return '\
                        <a href="/'+ lang +'/Ticket/Detil/'+ data +'"><i class="flaticon2-paper"></i></a>\
                        ';
                    }
                }
            ],
            initComplete : print(false)
        });

        var allDatatable = $('#kt_datatable_all').DataTable({
            scrollX: true,
            scrollCollapse: true,
            data: [{'start_date':'21 March 2021', 'received_date':'24 March 2021', 'ticket_number':'8210202107-0001', 'status':1, 'miro_number': '12345678909876543','name':'PT Arifindo Teknologi', 'posting_date': '25 March 2021', 'remmitance_number': '1234567890', 'remmitance_date': '25 March 2021', 'invoice_number':'INV-001-IV-2392', 'amount':'Rp 1,502,200,000'}],
            columnDefs: [
                ...firstColumn,
                {
                    targets: -2,
                    data: {status: 'status', position: 'position'},
                    render: function (data, row, type) {
                        var status = {
							1: "Approved For Payment",
							2: "Rejected",
							3: "Revise",
						};
                        return status[data.status];
                    }
                },
                ...invoice_number,
                {
                    targets: invoice_number.length ? 4 : 3,
                    data: 'amount'
                },
                ...for_accounting,
                ...for_another_role,
                {
                    targets: -1,
                    className: 'text-right',
                    orderable: false,
                    data: 'ticket_number',
                    render: function(data, row, type, full, meta) {
                        return '\
                        <a href="/'+ lang +'/Ticket/Detil/'+ data +'"><i class="flaticon2-paper"></i></a>\
                        ';
                    }
                }
            ],
            initComplete : print(false)
        });
    };


    return {
        // Public functions
        init: function() {
            // init datatable
            datatableInitialize();
		},
		deleteData
    };
}();

jQuery(document).ready(function() {
	KTDatatableAjaxTable.init();
});