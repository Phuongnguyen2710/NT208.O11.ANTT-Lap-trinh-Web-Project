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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class QUANLYXEMPHIMEntities : DbContext
    {
        public QUANLYXEMPHIMEntities()
            : base("name=QUANLYXEMPHIMEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<booking> bookings { get; set; }
        public virtual DbSet<booking_detail> booking_detail { get; set; }
        public virtual DbSet<comment> comments { get; set; }
        public virtual DbSet<member> members { get; set; }
        public virtual DbSet<movy> movies { get; set; }
        public virtual DbSet<news> news { get; set; }
        public virtual DbSet<ROLE> ROLEs { get; set; }
        public virtual DbSet<room> rooms { get; set; }
        public virtual DbSet<schedule> schedules { get; set; }
        public virtual DbSet<seat> seats { get; set; }
    }
}
