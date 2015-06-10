using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Annufal.Core.Profile
{
    public class ProfileValidationModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }

        //user validating
        [Required]
        public string Validator { get; set; }

        public string Comment { get; set; }
    }
}