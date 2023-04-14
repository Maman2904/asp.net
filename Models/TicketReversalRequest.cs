using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tufol.Models

{
	public class TicketReversalRequest
	{
        public string? status_rpa { get; set; }
        public string? rpa_status_description { get; set; }
        public string? status { get; set; }
    }

}