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
    using System.ComponentModel.DataAnnotations;

    public partial class news
    {
        public int news_id { get; set; }
        public Nullable<int> movie_id { get; set; }
        public string news_title { get; set; }
        public string news_content { get; set; }
        public string news_img { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> news_release { get; set; }
    
        public virtual movy movy { get; set; }
    }
}
