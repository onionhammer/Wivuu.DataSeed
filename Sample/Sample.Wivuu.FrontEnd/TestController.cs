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
                // Map UserForm <-> UserFormViewModel
                cfg.CreateMap<UserForm, UserFormViewModel>().ReverseMap();
            });

        IMapper Mapper { get; } =
            Map.CreateMapper();

        public async Task<List<UserFormViewModel>> FakeGetUserForms(DateTime since)
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
                // Update form
                var form = Mapper.Map(model, await business.UserForms.Find(model.Id));

                business.UserForms.UpdateForm(form);
                await business.Save();
            }
        }

        public async Task SetActive(Guid id, bool active)
        {
            using (var business = new BusinessContext())
            {
                await business.UserForms.SetActive(id, active);
                await business.Save();
            }
        }
    }

    [TestClass]
    public class TestController
    {
        [TestMethod]
        public async Task TestFakeController()
        {
            var rand      = new Random();
            var newEmail  = $"{rand.NextString(4, 8)}@test2.com";
            var newIncome = rand.Next(5000, 10000);

            var controller = new FakeController();
            var since      = new DateTime(1890, 1, 1);
            var results    = await controller.FakeGetUserForms(since);
            Assert.IsNotNull(results);
            Assert.AreNotEqual(0, results.Count);

            var firstResult = results[0];
            Assert.IsTrue(firstResult.DateOfBirth > since);

            var active = firstResult.IsActive;
            firstResult.IsActive = !active;

            // Update the user's email address
            firstResult.NetYearlyIncome = newIncome; // Try to save yearly income
            firstResult.Email = newEmail;
            await controller.UpdateUserForm(firstResult);

            // Re-retrieve results
            results = await controller.FakeGetUserForms(since);
            firstResult = results[0];

            Assert.AreEqual(active, firstResult.IsActive);
            Assert.AreEqual(newEmail, firstResult.Email);
            Assert.AreNotEqual(newIncome, firstResult.NetYearlyIncome);

            try
            {
                // Toggle active
                await controller.SetActive(firstResult.Id, false);
                results = await controller.FakeGetUserForms(since);
                Assert.AreEqual(0, results.Count);
            }
            finally
            {
                await controller.SetActive(firstResult.Id, true);
            }
        }
    }
}
