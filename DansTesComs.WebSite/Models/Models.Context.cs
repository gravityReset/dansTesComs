﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DansTesComs.WebSite.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DansTesComsEntities : DbContext
    {
        public DansTesComsEntities()
            : base("name=DansTesComsEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CommentaireExterne> CommentaireExternes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<CommentairesExterneContent> CommentairesExterneContents { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<Commentaire> Commentaires { get; set; }
    }
}
