﻿namespace Annufal.ViewModels
{
    public class RegistrationViewModel
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string ConfirmationPassword { get; set; }

        public string Prenom { get; set; }

        public string Nom { get; set; }

        public string Surnom { get; set; }

        public string DateBapteme { get; set; }

        public string VilleBapteme { get; set; }

        public string FiliereBapteme { get; set; }

        public string Parrain { get; set; }

        public string Marraine { get; set; }

        public string GmBapteme { get; set; }

        public string GcBapteme { get; set; }

        public string VilleActuelle { get; set; }

        public bool IsEtudiant { get; set; }

        public bool IsGM { get; set; }

        public bool IsGC { get; set; }

        public string Tel { get; set; }

        public string Email { get; set; }

        public string LinkedIn { get; set; }

        public string Twitter { get; set; }
    }
}