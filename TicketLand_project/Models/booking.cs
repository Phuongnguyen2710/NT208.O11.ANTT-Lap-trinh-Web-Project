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
    
    public partial class booking
    {
        public int booking_id { get; set; }
        public Nullable<int> member_id { get; set; }
        public Nullable<int> schedule_id { get; set; }
        public Nullable<System.TimeSpan> booking_date { get; set; }
        public Nullable<int> total_price { get; set; }
        public Nullable<int> booking_status { get; set; }
    
        public virtual member member { get; set; }
        public virtual schedule schedule { get; set; }
    }
}
