using System;
using System.ComponentModel.DataAnnotations;

namespace Annufal.Core.Profile
{
    public class CreateProfileBindingModel
    {
        [Required]
        [MinLength(2), MaxLength(25)]
        public string Prenom { get; set; }

        [Required]
        [MinLength(2), MaxLength(25)]
        public string Nom { get; set; }

        [Required]
        [MinLength(2), MaxLength(50)]
        public string Surnom { get; set; }

        [Required]
        public DateTime DateBapteme { get; set; }

        [Required]
        [MinLength(2), MaxLength(25)]
        public string VilleBapteme { get; set; }

        [Required]
        [MinLength(2), MaxLength(25)]
        public string FiliereBapteme { get; set; }

        [Required]
        [MinLength(2), MaxLength(50)]
        public string Parrain { get; set; }

        [Required]
        [MinLength(2), MaxLength(50)]
        public string Marraine { get; set; }

        [Required]
        [MinLength(2), MaxLength(50)]
        public string GmBapteme { get; set; }

        [MinLength(2), MaxLength(50)]
        public string GcBapteme { get; set; }

        [Required]
        [MinLength(2), MaxLength(25)]
        public string VilleActuelle { get; set; }

        public bool IsEtudiant { get; set; }

        public bool IsGM { get; set; }

        public bool IsGC { get; set; }

        [Phone]
        [MaxLength(20)]
        public string Tel { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [MinLength(2), MaxLength(100)]
        public string Facebook { get; set; }

        [MinLength(2), MaxLength(100)]
        public string LinkedIn { get; set; }

        [MinLength(2), MaxLength(100)]
        public string Twitter { get; set; }
    }
}