"use strict";
// Class definition

var KTDatatableAjaxTable = function () {
    // Private functions
    var deleteData = function (id) {
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
                if (doDelete.ok) {
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
    var basicColumn = [
        {
            data: 'ticket_number',
            orderable: false
        },
        {
            data: 'created_at',
            orderable: true
        },
        {
            data: 'received_date'
        },
        {
            data: 'vendor_number',
            orderable: false
        },
        {
            data: 'vendor_name',
            orderable: false
        },
        {
            data: 'invoice_number',
            orderable: false
        },
        {
            data: 'invoice_amount'
        }
    ]

    var miroColumn = [
        {
            data: 'miro_number',
            orderable: false
        },
        {
            data: 'posting_date',
            orderable: true
        }
    ];

    var remittanceColumn = [
        {
            data: 'remmitance_number',
            orderable: false
        },
        {
            data: 'remmitance_date',
        }
    ];

    var lastColumn = [
        {
            data: 'status',
            orderable: false
        }
    ];
    var allDatatableInitialize = function () {
        var allColumns = [...basicColumn];
        if (role_id == "6") {
            allColumns = [...allColumns, ...miroColumn];
        }
        allColumns = [...allColumns, ...remittanceColumn];
        allColumns = [
            ...allColumns,
            ...lastColumn,
            {
                title: 'Actions',
                data: 'ticket_number',
                searchable: false,
                orderable: false
            }
        ];
        var allDatatable = $('#kt_datatable_all').DataTable({
            "order": [[1, "asc"]],
            searchDelay: 500,
            processing: true,
            serverSide: true,
            columns: allColumns,
            fixedHeader: false,
            ajax: {
                url: '/' + lang + '/Ticket/all_ticket_datatable',
                type: 'POST'
            },
            columnDefs: [
                {
                    targets: 0,
                    orderable: false,
                    data: 'ticket_number',
                    render: function (data, row, type) {
                        return '\
                            <a href="/'+ lang + '/Ticket/Detil/' + data + '">' + data + '</a>\
                        ';
                    }
                },
                {
                    targets: -1,
                    className: 'text-right',
                    orderable: false,
                    data: 'ticket_number',
                    render: function (data, row, type, full, meta) {
                        return '\
                        <a href="/'+ lang + '/Ticket/Detil/' + data + '"><i class="flaticon2-paper"></i></a>\
                        ';
                    }
                }
            ],
            initComplete: function () {
                // Apply the search
                var i = 0;
                var input_date = [1, 2];
                input_date.push(8);
                if (role_id == "6") {
                    input_date.push(10);
                } 
                this.api().columns().every(function () {
                    var that = this;
                    var title = this.header().textContent;
                    if (input_date.includes(i)) {
                        var dateinput = $('<input type="date" class="mt-2" />')
                            .appendTo($(that.header()))
                            .on('change', function () {
                                var val = $(this).val();

                                that
                                    .search(val ? val : '', true, false)
                                    .draw();
                            });
                    } else {
                        if (title != 'Aksi' && title != 'Actions' && title != 'Opsi' && title != 'Options') {
                            var textinput = $('<input type="text" placeholder="Search ' + title + '" class="mt-2" />')
                                .appendTo($(that.header()))
                                .on('change', function () {
                                    var val = $(this).val();

                                    that
                                        .search(val ? val : '', true, false)
                                        .draw();
                                });
                        }

                    }
                    i++;
                });
            }
        });
    };
    var pendingDatatableInitialize = function () {
        var pendingColumns = [
            ...basicColumn,
            ...lastColumn,
            {
                title: 'Actions',
                data: 'ticket_number',
                searchable: false,
                orderable: false
            }
        ];

        var pendingDatatable = $('#kt_datatable_pending').DataTable({
            "order": [[1, "asc"]],
            searchDelay: 500,
            processing: true,
            serverSide: true,
            columns: pendingColumns,
            fixedHeader: false,
            ajax: {
                url: '/' + lang + '/Ticket/pending_ticket_datatable',
                type: 'POST'
            },
            columnDefs: [

                {
                    targets: 0,
                    orderable: false,
                    data: 'ticket_number',
                    render: function (data, row, type) {
                        return '\
                            <a href="/'+ lang + '/Ticket/Detil/' + data + '">'+data+'</a>\
                        ';
                    }
                },
                {
                    targets: -1,
                    orderable: false,
                    className: 'text-right',
                    data: 'ticket_number',
                    render: function (data, row, type) {
                        return '\
                            <a href="/'+ lang + '/Ticket/Detil/' + data + '"><i class="flaticon2-paper"></i></a>\
                        ';
                    }
                }
            ],
            initComplete: function () {
                if (role_id == "4") {
                    $('.dataTables_filter')
                        .html('<a href="'+base_url + lang + '/Ticket/PrintTicket/' +'" target="_blank"><button class="btn btn-primary" style="background-color:#8950FC"><i class="fa fa-print"></i> Print</button></a>');
                } else {
                    $('.dataTables_filter')
                        .html("");
                }
                // Apply the search
                var i = 0;
                var input_date = [1, 2];
                this.api().columns().every(function () {
                    var that = this;
                    var title = this.header().textContent;
                    if (input_date.includes(i)) {
                        var dateinput = $('<input type="date" class="mt-2" />')
                            .appendTo($(that.header()))
                            .on('change', function () {
                                var val = $(this).val();

                                that
                                    .search(val ? val : '', true, false)
                                    .draw();
                            });
                    } else {
                        if (title != 'Aksi' && title != 'Actions' && title != 'Opsi' && title != 'Options') {
                            var textinput = $('<input type="text" placeholder="Search ' + title + '" class="mt-2" />')
                                .appendTo($(that.header()))
                                .on('change', function () {
                                    var val = $(this).val();

                                    that
                                        .search(val ? val : '', true, false)
                                        .draw();
                                });
                        }
                        
                    }
                    i++;
                });
            }
        });
    };

    return {
        // Public functions
        init: function () {
            // init datatable
            pendingDatatableInitialize();
        },
        pendingDatatableInitialize,
        allDatatableInitialize,
        deleteData
    };
}();

jQuery(document).ready(function () {
    var loadedAllDatatable = false;
    $('a[href="#all"]').click(function (e) {
        e.preventDefault();
        if (!loadedAllDatatable) {
            loadedAllDatatable = true;
            KTDatatableAjaxTable.allDatatableInitialize();
        }
    });
    if(role_id == "8")
        KTDatatableAjaxTable.allDatatableInitialize();
    else
        KTDatatableAjaxTable.pendingDatatableInitialize();
});