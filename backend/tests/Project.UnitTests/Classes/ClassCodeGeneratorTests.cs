using Project.Api.Services;

namespace Project.UnitTests.Classes;

public class ClassCodeGeneratorTests
{
    [Fact]
    public void Generate_Always_ReturnsSixCharacters()
    {
        var generator = new ClassCodeGenerator();

        var code = generator.Generate();

        Assert.Equal(6, code.Length);
    }

    [Fact]
    public void Generate_ManyTimes_UsesOnlyUppercaseLettersAndDigits()
    {
        var generator = new ClassCodeGenerator();

        for (var i = 0; i < 200; i++)
        {
            var code = generator.Generate();

            Assert.Matches("^[A-Z0-9]{6}$", code);
        }
    }

    [Fact]
    public void Generate_CalledTwice_ReturnsDifferentCodes()
    {
        var generator = new ClassCodeGenerator();

        var first = generator.Generate();
        var second = generator.Generate();

        Assert.NotEqual(first, second);
    }
}
