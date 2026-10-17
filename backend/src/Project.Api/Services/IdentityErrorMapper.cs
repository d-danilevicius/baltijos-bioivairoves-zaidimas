using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Project.Api.Services;

public static class IdentityErrorMapper
{
    public static RegistrationResult Map(IEnumerable<IdentityError> errors, ILogger logger)
    {
        var dict = new Dictionary<string, List<string>>();
        bool isConflict = false;

        foreach (var e in errors)
        {
            var code = e.Code ?? string.Empty;
            switch (code)
            {
                case "PasswordTooShort":
                    Add(dict, "password", "InvalidLength");
                    break;
                case "InvalidUserName":
                    Add(dict, "userName", "InvalidFormat");
                    break;
                case "InvalidEmail":
                    Add(dict, "email", "InvalidFormat");
                    break;
                case "DuplicateEmail":
                    Add(dict, "email", "AlreadyRegistered");
                    isConflict = true;
                    break;
                case "DuplicateUserName":
                    Add(dict, "userName", "AlreadyInUse");
                    isConflict = true;
                    break;
                default:
                    // Unknown identity code - log and fail loudly to return 500
                    logger.LogWarning("Unexpected Identity error code {Code}", code);
                    throw new InvalidOperationException("Unexpected identity error code");
            }
        }

        var resultDict = dict.ToDictionary(k => k.Key, v => v.Value.ToArray());
        return isConflict ? RegistrationResult.Conflict(resultDict) : RegistrationResult.Invalid(resultDict);
    }

    private static void Add(Dictionary<string, List<string>> dict, string key, string code)
    {
        if (!dict.TryGetValue(key, out var list))
        {
            list = new List<string>();
            dict[key] = list;
        }
        if (!list.Contains(code)) list.Add(code);
    }
}
