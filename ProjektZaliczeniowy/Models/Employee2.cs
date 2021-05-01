using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjektZaliczeniowy.Models
{
    public class Employee2
    {
        public int BusinessEntityID { get; set; }

        [DisplayName("Employee's login")]
        [Required(ErrorMessage = "Employee's login is required")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "Login must have at least 3 characters")]
        //DO WSTAWIENIA (Walidacja danych wstawianych przez użytkownika, czesciowe zabezp. przeciw sql injection)
        [RegularExpression("[^#!$'^&*~]*", ErrorMessage = "The field cannot contain special characters")] 
        public string LoginID { get; set; }

        [DisplayName("Position")]
        [Required(ErrorMessage = "Position is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "The position must have at least 3 characters")]
        [RegularExpression("[^#!$^&*~]*", ErrorMessage = "The position cannot contain special characters")]
        public string JobTitle { get; set; }

        [DisplayName("Date of birth")]
        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; } //w bazie jest typu date

        [DisplayName("Sex")]
        [Required(ErrorMessage = "The gender of the employee is required")]
        [StringLength(1)]
        [RegularExpression("[MF]*", ErrorMessage = "Enter the appropriate letter: M - male or F - female")]
        public string Gender { get; set; } //moze string?

        [DisplayName("Date of employment")]
        [Required(ErrorMessage = "Employment date is required")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; } //w bazie jest typu date

        [DisplayName("Modification date")]
        [Required(ErrorMessage = "Modification date is required")]
        //[DataType(DataType.Date)]
        public DateTime ModifiedDate { get; set; }
    }
}
