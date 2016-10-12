using System;

namespace Sample.Wivuu.Domain.Models
{
    public class UserForm
    {
        public Guid Id { get; internal set; } = Guid.NewGuid();

        public string FirstName { get; internal set; }

        public string LastName { get; internal set; }

        public string SocialSecurityNumber { get; internal set; }

        public DateTime DateOfBirth { get; internal set; }

        public string Email { get; internal set; }

        public decimal NetYearlyIncome { get; internal set; }

        public bool IsActive { get; internal set; } = true;
    }
}