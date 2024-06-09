using System;

namespace ArloBrau.Models
{
    public class PlayerManager
    {
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public string? TechnicalFormation { get; set; }
        public string PlayerHistory { get; set; } = string.Empty;
        public int Height { get; set; }
        public string SkinColor { get; set; } = string.Empty;
        public bool VitaCertified { get; set; }
        public bool CcslCertified { get; set; }

        public int Age
        {
            get
            {
                int age = DateTime.Now.Year - BirthDate.Year;
                if (BirthDate > DateTime.Now.AddYears(-age)) age--;
                return age;
            }
        }
    }
}
