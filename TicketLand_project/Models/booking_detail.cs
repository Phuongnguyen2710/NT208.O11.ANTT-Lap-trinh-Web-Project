//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TicketLand_project.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class booking_detail
    {
        public int bkdetail_id { get; set; }
        public Nullable<int> booking_id { get; set; }
        public Nullable<int> seat_id { get; set; }
    
        public virtual booking booking { get; set; }
        public virtual seat seat { get; set; }
    }
}
