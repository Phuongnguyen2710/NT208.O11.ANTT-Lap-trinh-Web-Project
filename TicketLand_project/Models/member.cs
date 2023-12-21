﻿//------------------------------------------------------------------------------
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

    public partial class member
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public member()
        {
            this.bookings = new HashSet<booking>();
            this.comments = new HashSet<comment>();
        }

        public int member_id { get; set; }
        [Required]
        public string member_name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên!")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu!")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Password must contain only letters and numbers.")]
        public string password { get; set; }
        public Nullable<bool> gender { get; set; }
        public Nullable<System.DateTime> date_of_birth { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email không hợp lệ!")]
        public string email { get; set; }
        public string city { get; set; }
        public string phone { get; set; }
        public Nullable<int> role_id { get; set; }
        public string member_avatar { get; set; }
        public Nullable<int> member_point { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<booking> bookings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<comment> comments { get; set; }
        public virtual ROLE ROLE { get; set; }
    }
}
