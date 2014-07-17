﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using DansTesComs.Core;

namespace DansTesComs.WebSite.Models
{
    [MetadataType(typeof(CommentaireExterneMetaData))]
    public partial class CommentaireExterne
    {
  
    }


    public class CommentaireExterneMetaData
    {
        [MaxLength(50, ErrorMessage = "Titre trop long!")]
        [Required(ErrorMessage = "Titre obligatoire.")]
        public string Titre { get; set; }

    }

}