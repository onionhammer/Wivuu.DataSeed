using System;
using System.Data.Entity.Migrations;
using Wivuu.DataSeed;

namespace Sample.Wivuu.Domain.Migrations.Data
{
    internal class AddSampleForms : Seed<MyDbContext>
    {
        public override void Apply(MyDbContext context)
        {
            var rand = new Random(0x1);

            context.UserForms.AddOrUpdate(new Models.UserForm
            {
                Id                   = rand.NextGuid(),
                FirstName            = rand.NextString(4, 8),
                LastName             = rand.NextString(4, 8),
                DateOfBirth          = rand.NextDateTime(),
                Email                = $"{rand.NextString(4, 8)}@testing123.com",
                NetYearlyIncome      = rand.Next(50000, 100000),
                SocialSecurityNumber = "123-45-6789"
            });
        }
    }
}