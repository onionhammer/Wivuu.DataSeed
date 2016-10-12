using System;
using System.ComponentModel.DataAnnotations;

namespace Sample.Wivuu.FrontEnd.ViewModels
{
    public class UserFormViewModel
    {
        public Guid Id { get; internal set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime? DateOfBirth { get; set; }

        public string SocialSecurityNumber { get; set; }

        [Required]
        public string Email { get; set; }

        public decimal? NetYearlyIncome { get; set; }

        public bool IsActive { get; set; }
    }
}
