using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Project.Api.DTOs.Requests;
using Xunit;

namespace Project.UnitTests.Registration;

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

    [Fact]
    public void RegisterRequest_Attributes_HaveExpectedErrorMessages()
    {
        var t = typeof(Project.Api.DTOs.Requests.RegisterRequest);
        var emailProp = t.GetProperty("Email")!;
        var emailAttrs = emailProp.GetCustomAttributes(true);
        Assert.Contains(emailAttrs, a => a is System.ComponentModel.DataAnnotations.RequiredAttribute req && req.ErrorMessage == "Required");
        Assert.Contains(emailAttrs, a => a is System.ComponentModel.DataAnnotations.EmailAddressAttribute em && em.ErrorMessage == "InvalidFormat");
        Assert.Contains(emailAttrs, a => a is System.ComponentModel.DataAnnotations.StringLengthAttribute sl && sl.ErrorMessage == "TooLong");

        var userProp = t.GetProperty("UserName")!;
        var userAttrs = userProp.GetCustomAttributes(true);
        Assert.Contains(userAttrs, a => a is System.ComponentModel.DataAnnotations.RequiredAttribute req2 && req2.ErrorMessage == "Required");
        Assert.Contains(userAttrs, a => a is System.ComponentModel.DataAnnotations.RegularExpressionAttribute re && re.ErrorMessage == "InvalidFormat");

        var passProp = t.GetProperty("Password")!;
        var passAttrs = passProp.GetCustomAttributes(true);
        Assert.Contains(passAttrs, a => a is System.ComponentModel.DataAnnotations.RequiredAttribute req3 && req3.ErrorMessage == "Required");
        Assert.Contains(passAttrs, a => a is System.ComponentModel.DataAnnotations.StringLengthAttribute sl2 && sl2.ErrorMessage == "InvalidLength");

        var roleProp = t.GetProperty("Role")!;
        var roleAttrs = roleProp.GetCustomAttributes(true);
        Assert.Contains(roleAttrs, a => a is System.ComponentModel.DataAnnotations.RequiredAttribute req4 && req4.ErrorMessage == "Required");
        Assert.Contains(roleAttrs, a => a is System.ComponentModel.DataAnnotations.RegularExpressionAttribute re2 && re2.ErrorMessage == "NotAllowed");
    }
}
