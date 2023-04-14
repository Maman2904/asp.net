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
    var draftDatatableInitialize = function () {
        var draftColumns = [
            ...basicColumn,
            ...lastColumn,
            {
                title: 'Actions',
                data: 'ticket_number',
                searchable: false,
                orderable: false
            }
        ];

        var draftDatatable = $('#kt_datatable_draft').DataTable({
            "order": [[1, "asc"]],
            searchDelay: 500,
            processing: true,
            serverSide: true,
            columns: draftColumns,
            fixedHeader: false,
            ajax: {
                url: '/' + lang + '/Ticket/draft_ticket_datatable',
                type: 'POST'
            },
            columnDefs: [

                {
                    targets: 0,
                    orderable: false,
                    data: 'ticket_number',
                    render: function (data, row, type) {
                        return '\
                            <a href="/'+ lang + '/ChangeTicket/Detil/' + data + '">' + data + '</a>\
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
                            <a href="/'+ lang + '/ChangeTicket/Detil/' + data + '"><i class="flaticon2-paper"></i></a>\
                        ';
                    }
                }
            ],
            initComplete: function () {
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
            draftDatatableInitialize();
        },
        draftDatatableInitialize
    };
}();

jQuery(document).ready(function () {
    KTDatatableAjaxTable.draftDatatableInitialize();
});