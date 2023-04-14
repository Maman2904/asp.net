using System.Collections.Generic;

namespace tufol.Helpers
{
    public class BadRequesBody
    {
        public string msg {get; set;}
        public string param {get; set;}
        public string location {get; set;}
        
    }

    public class BadRequestMessage
    {
        public List<BadRequesBody> items { get; set; }
    }

}