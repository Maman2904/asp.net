"use strict";
// Class definition

var KTDatatableAjaxTable = function () {
    var openItemPoDatatableInitialize = function () {
        var openItemPoColumns = [
            {
                data: 'name_vendor',
                orderable: true
            },
            {
                data: 'po_item_number',
                orderable: false
            },
            {
                data: 'doc_date',
                orderable: false
            },
            {
                data: 'po_number',
                orderable: false
            },
            {
                data: 'po_name',
                orderable: false
            },
            {
                data: 'po_quantity',
                orderable: false
            },
            {
                data: 'uom',
                orderable: false
            },
            {
                data: 'currency',
                orderable: false
            },
            {
                data: 'net_price',
                orderable: false
            }
        ];

        $('#kt_datatable_open_item_po').DataTable({
            "order": [[0, "asc"]],
            searchDelay: 500,
            processing: true,
            serverSide: true,
            columns: openItemPoColumns,
            fixedHeader: false,
            ajax: {
                url: '/' + lang + '/OpenItemPo/open_item_po_datatable',
                type: 'POST'
            },
            initComplete: function () {
                $('.dataTables_filter')
                        .html("");
                // Apply the search
                var i = 0;
                this.api().columns().every(function () {
                    var that = this;
                    var title = this.header().textContent;
                    var textinput = $('<input type="text" placeholder="Search ' + title + '" class="mt-2" />')
                        .appendTo($(that.header()))
                        .on('change', function () {
                            var val = $(this).val();

                            that
                                .search(val ? val : '', true, false)
                                .draw();
                        });
                    i++;
                });
            }
        });
    };

    return {
        // Public functions
        init: function () {
            // init datatable
            openItemPoDatatableInitialize();
        },
        openItemPoDatatableInitialize
    };
}();

jQuery(document).ready(function () {
    KTDatatableAjaxTable.openItemPoDatatableInitialize();
});