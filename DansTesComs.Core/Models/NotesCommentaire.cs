//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DansTesComs.Core.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class NotesCommentaire
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CommentaireId { get; set; }
        public int Value { get; set; }
    
        public virtual Commentaire Commentaire { get; set; }
        public virtual User User { get; set; }
    }
}