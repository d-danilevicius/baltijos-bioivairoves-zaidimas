using System.Security.Cryptography;

namespace Project.Api.Services;

public class ClassCodeGenerator : IClassCodeGenerator
{
    private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int CodeLength = 6;

    public string Generate() => RandomNumberGenerator.GetString(Characters, CodeLength);
}
