using System.Collections.Generic;
using Project.Api.DTOs.Requests;

using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Project.UnitTests.Registration
{
    public class RegisterRequestValidationTests
    {
        private IList<ValidationResult> Validate(object model)
        {
            var ctx = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, ctx, results, true);
            return results;
        }

        [Fact]
        public void RegisterRequest_EmailRequiredAndLengthAndFormat_InvalidWhenEmptyOrTooLong()
        {
            var reqEmpty = new RegisterRequest { Email = "", UserName = "u1", Password = "Password123", Role = "Student" };
            var reqTooLong = new RegisterRequest { Email = new string('a', 255) + "@x.com", UserName = "u2", Password = "Password123", Role = "Student" };

            var resEmpty = Validate(reqEmpty);
            var resTooLong = Validate(reqTooLong);

            Assert.NotEmpty(resEmpty);
            Assert.NotEmpty(resTooLong);
        }

        [Fact]
        public void RegisterRequest_UsernameLimits_InvalidWhenTooShortOrTooLongOrBadCharacters()
        {
            var reqShort = new RegisterRequest { Email = "a@b.com", UserName = "ab", Password = "Password123", Role = "Student" };
            var reqLong = new RegisterRequest { Email = "a@b.com", UserName = new string('x', 31), Password = "Password123", Role = "Student" };
            var reqBad = new RegisterRequest { Email = "a@b.com", UserName = "bad$name", Password = "Password123", Role = "Student" };

            Assert.NotEmpty(Validate(reqShort));
            Assert.NotEmpty(Validate(reqLong));
            Assert.NotEmpty(Validate(reqBad));
        }

        [Fact]
        public void RegisterRequest_PasswordLimits_InvalidWhenTooShortOrTooLong()
        {
            var reqShort = new RegisterRequest { Email = "a@b.com", UserName = "validuser", Password = "short", Role = "Student" };
            var reqLong = new RegisterRequest { Email = "a@b.com", UserName = "validuser2", Password = new string('p', 129), Role = "Student" };

            Assert.NotEmpty(Validate(reqShort));
            Assert.NotEmpty(Validate(reqLong));
        }

        [Fact]
        public void RegisterRequest_RoleRequiredAndAllowed_InvalidWhenMissingOrAdministrator()
        {
            var reqMissing = new RegisterRequest { Email = "a@b.com", UserName = "user1", Password = "Password123", Role = "" };
            var reqAdmin = new RegisterRequest { Email = "a@b.com", UserName = "user2", Password = "Password123", Role = "Administrator" };

            Assert.NotEmpty(Validate(reqMissing));
            Assert.NotEmpty(Validate(reqAdmin));
        }
    }
}
