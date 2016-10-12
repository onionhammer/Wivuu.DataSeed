using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.Wivuu.Business;
using Sample.Wivuu.Domain.Models;
using Sample.Wivuu.FrontEnd.ViewModels;
using Wivuu.DataSeed;

namespace Sample.Wivuu.FrontEnd
{
    public class FakeController
    {
        static MapperConfiguration Map { get; } =
            new MapperConfiguration(cfg =>
            {
                // Initialize mapping
                cfg.CreateMap<UserForm, UserFormViewModel>();
                cfg.CreateMap<UserFormViewModel, UserForm>();
            });

        public async Task<List<UserFormViewModel>> FakeGetUserForms(
            DateTime since)
        {
            using (var business = new BusinessContext())
            {
                return await business.UserForms
                    .GetFormsByBirthDate(since)
                    .Take(10)
                    .ProjectToListAsync<UserFormViewModel>(Map);
            }
        }

        public async Task UpdateUserForm(UserFormViewModel model)
        {
            using (var business = new BusinessContext())
            {
                var form = await business.UserForms.Find(model.Id);

                // Update form
                var mapper = Map.CreateMapper();
                mapper.Map(model, form);

                await business.UserForms.UpdateForm(form);
            }
        }
    }

    [TestClass]
    public class TestController
    {
        [TestMethod]
        public async Task TestFakeController()
        {
            var rand     = new Random();
            var newEmail = $"{rand.NextString(4, 8)}@test2.com";
            var newIncome = rand.Next(5000, 10000);

            var controller = new FakeController();
            var since      = new DateTime(1890, 1, 1);
            var results    = await controller.FakeGetUserForms(since);
            Assert.IsNotNull(results);
            Assert.AreNotEqual(0, results.Count);

            var firstResult = results[0];
            Assert.IsTrue(firstResult.DateOfBirth > since);

            // Update the user's email address
            firstResult.NetYearlyIncome = newIncome; // Try to save yearly income
            firstResult.Email = newEmail;
            await controller.UpdateUserForm(firstResult);

            // Re-retrieve results
            results = await controller.FakeGetUserForms(since);
            Assert.IsNotNull(results);
            Assert.AreNotEqual(0, results.Count);
            firstResult = results[0];

            Assert.AreEqual(newEmail, firstResult.Email);
            Assert.AreNotEqual(newIncome, firstResult.NetYearlyIncome);
        }
    }
}
