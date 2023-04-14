"use strict";
// Class definition

var KTDatatableAjaxAccounting = function () {
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
    var errorDatatableInitialize = function () {
        var errorColumns = [...basicColumn, ...miroColumn, ...remittanceColumn, { data: 'rpa_status_description', orderable: false }];
        errorColumns = [
            ...errorColumns,
            ...lastColumn,
            {
                title: 'Actions',
                data: 'ticket_number',
                searchable: false,
                orderable: false
            }
        ];
        var errorDatatable = $('#kt_datatable_error').DataTable({
            "order": [[1, "asc"]],
            searchDelay: 500,
            processing: true,
            serverSide: true,
            columns: errorColumns,
            fixedHeader: false,
            ajax: {
                url: '/' + lang + '/Ticket/error_ticket_datatable',
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
                var input_date = [1, 2, 8, 10];
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
    var reversalDatatableInitialize = function () {
        var reversalColumns = [...basicColumn, ...miroColumn, ...remittanceColumn];
        reversalColumns = [
            ...reversalColumns,
            ...lastColumn,
            {
                title: 'Actions',
                data: 'ticket_number',
                searchable: false,
                orderable: false
            }
        ];
        var reversalDatatable = $('#kt_datatable_need_reversal').DataTable({
            "order": [[1, "asc"]],
            searchDelay: 500,
            processing: true,
            serverSide: true,
            columns: reversalColumns,
            fixedHeader: false,
            ajax: {
                url: '/' + lang + '/Ticket/need_reversal_datatable',
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
                var input_date = [1, 2, 8, 10];
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
    var simulationDatatableInitialize = function () {
        var simulationColumns = [...basicColumn];
        simulationColumns = [
            ...simulationColumns,
            ...lastColumn,
            {
                title: 'Actions',
                data: 'ticket_number',
                searchable: false,
                orderable: false
            }
        ];

        var simulationDatatable = $('#kt_datatable_simulation').DataTable({
            "order": [[1, "asc"]],
            searchDelay: 500,
            processing: true,
            serverSide: true,
            columns: simulationColumns,
            fixedHeader: false,
            ajax: {
                url: '/' + lang + '/Ticket/simulation_ticket_datatable',
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
        },
        errorDatatableInitialize,
        reversalDatatableInitialize,
        simulationDatatableInitialize,
    };
}();

jQuery(document).ready(function () {
    var loadedErrorDatatable = false,
        loadedReversalDatatable = false,
        loadedSimulationDatatable = false;
    $('a[href="#error"]').click(function (e) {
        e.preventDefault();
        if (!loadedErrorDatatable) {
            loadedErrorDatatable = true;
            KTDatatableAjaxAccounting.errorDatatableInitialize();
        }
    });
    $('a[href="#need_reversal"]').click(function (e) {
        e.preventDefault();
        if (!loadedReversalDatatable) {
            loadedReversalDatatable = true;
            KTDatatableAjaxAccounting.reversalDatatableInitialize();
        }
    });
    $('a[href="#simulation"]').click(function (e) {
        e.preventDefault();
        if (!loadedSimulationDatatable) {
            loadedSimulationDatatable = true;
            KTDatatableAjaxAccounting.simulationDatatableInitialize();
        }
    });
});