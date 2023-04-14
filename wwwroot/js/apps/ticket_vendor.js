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
                            <a href="/'+ lang + '/Ticket/Detil/' + data + '">' + data + '</a>\
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
    var processDatatableInitialize = function () {
        var processColumns = [
            ...basicColumn,
            ...lastColumn,
            {
                title: 'Actions',
                data: 'ticket_number',
                searchable: false,
                orderable: false
            }
        ];

        var processDatatable = $('#kt_datatable_process').DataTable({
            "order": [[1, "asc"]],
            searchDelay: 500,
            processing: true,
            serverSide: true,
            columns: processColumns,
            fixedHeader: false,
            ajax: {
                url: '/' + lang + '/Ticket/process_ticket_datatable',
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

    var revisionDatatableInitialize = function () {
        var revisionColumns = [
            ...basicColumn,
            ...lastColumn,
            {
                title: 'Actions',
                data: 'ticket_number',
                searchable: false,
                orderable: false
            }
        ];

        var revisionDatatable = $('#kt_datatable_need_revision').DataTable({
            "order": [[1, "asc"]],
            searchDelay: 500,
            processing: true,
            serverSide: true,
            columns: revisionColumns,
            fixedHeader: false,
            ajax: {
                url: '/' + lang + '/Ticket/need_revision_ticket_datatable',
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

    var rejectedDatatableInitialize = function () {
        var rejectedColumns = [
            ...basicColumn,
            ...lastColumn,
            {
                title: 'Actions',
                data: 'ticket_number',
                searchable: false,
                orderable: false
            }
        ];

        var rejectedDatatable = $('#kt_datatable_rejected').DataTable({
            "order": [[1, "asc"]],
            searchDelay: 500,
            processing: true,
            serverSide: true,
            columns: rejectedColumns,
            fixedHeader: false,
            ajax: {
                url: '/' + lang + '/Ticket/rejected_ticket_datatable',
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
        draftDatatableInitialize,
        processDatatableInitialize,
        revisionDatatableInitialize,
        rejectedDatatableInitialize,
        allDatatableInitialize
    };
}();

jQuery(document).ready(function () {
    var loadedAllDatatable = false,
        loadedProcessDatatable = false,
        loadedRevisionDatatable = false,
        loadedRejectedDatatable = false;
    $('a[href="#process"]').click(function (e) {
        e.preventDefault();
        if (!loadedProcessDatatable) {
            loadedProcessDatatable = true;
            KTDatatableAjaxTable.processDatatableInitialize();
        }
    });
    $('a[href="#need_revision"]').click(function (e) {
        e.preventDefault();
        if (!loadedRevisionDatatable) {
            loadedRevisionDatatable = true;
            KTDatatableAjaxTable.revisionDatatableInitialize();
        }
    });
    $('a[href="#rejected"]').click(function (e) {
        e.preventDefault();
        if (!loadedRejectedDatatable) {
            loadedRejectedDatatable = true;
            KTDatatableAjaxTable.rejectedDatatableInitialize();
        }
    });
    $('a[href="#all"]').click(function (e) {
        e.preventDefault();
        if (!loadedAllDatatable) {
            loadedAllDatatable = true;
            KTDatatableAjaxTable.allDatatableInitialize();
        }
    });
    KTDatatableAjaxTable.draftDatatableInitialize();
});