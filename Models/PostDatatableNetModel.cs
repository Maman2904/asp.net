using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tufol.Models
{
    public class searchDatatable
    {
        public string value { get; set; }
        public string regex { get; set; }
    }
    public class orderDatatable
    {
        public string column { get; set; }
        public string dir { get; set; }
    }
    public class searchColumnDatatable
    {
        public string value { get; set; }
        public string regex { get; set; }
    }
    public class columnDatatable
    {
        public string data { get; set; }
        public string name { get; set; }
        public string searchable { get; set; }
        public string orderable { get; set; }
        public searchColumnDatatable search { get; set; }
    }
    public class PostDatatableNetModel
    {
        public int draw { get; set; }
        public List<columnDatatable> columns { get; set; }
        public List<orderDatatable> order { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public searchDatatable search { get; set; }
    }
}
