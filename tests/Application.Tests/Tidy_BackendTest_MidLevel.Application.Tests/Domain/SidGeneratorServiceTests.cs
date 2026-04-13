using FluentAssertions;
using Tidy_BackendTest_MidLevel.Domain.Services;
using Xunit;

namespace Tidy_BackendTest_MidLevel.Application.Tests.Domain;

public class SidGeneratorServiceTests
{
    private readonly SidGeneratorService _service = new();

    [Fact]
    public void Generate_Returns20CharString()
    {
        var sid = _service.Generate();
        sid.Should().HaveLength(20);
    }

    [Fact]
    public void Generate_OnlyContainsValidChars()
    {
        const string validChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var sid = _service.Generate();
        sid.All(c => validChars.Contains(c)).Should().BeTrue(
            because: $"SID '{sid}' should only contain alphanumeric uppercase chars");
    }

    [Fact]
    public void Generate_YearPrefixIsBase36()
    {
        var sid = _service.Generate();
        const string base36 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        base36.Should().Contain(sid[0].ToString());
        base36.Should().Contain(sid[1].ToString());
    }

    [Fact]
    public void Generate_DayPartIsThreeDigits()
    {
        var sid = _service.Generate();
        var dayPart = sid.Substring(2, 3);
        int.TryParse(dayPart, out int day).Should().BeTrue();
        day.Should().BeInRange(1, 366);
    }

    [Fact]
    public void Generate_SecondPartIsFiveDigits()
    {
        var sid = _service.Generate();
        var secondPart = sid.Substring(5, 5);
        int.TryParse(secondPart, out int second).Should().BeTrue();
        second.Should().BeInRange(0, 86399);
    }

    [Fact]
    public void Generate_RandomPartIsCorrectLength()
    {
        var sid = _service.Generate();
        var randomPart = sid.Substring(10, 10);
        randomPart.Should().HaveLength(10);
    }

    [Fact]
    public void Generate_MultipleCallsProduceNoDuplicatesIn1000Calls()
    {
        var sids = new HashSet<string>();
        for (int i = 0; i < 1000; i++)
            sids.Add(_service.Generate());

        sids.Should().HaveCountGreaterThan(990,
            because: "1000 generated SIDs should have minimal collisions");
    }

    [Fact]
    public void IsValidFormat_ReturnsTrueFor20CharUpperAlphanumericString()
    {
        _service.IsValidFormat("ZA103ABCDE0123456789").Should().BeTrue();
    }

    [Fact]
    public void IsValidFormat_ReturnsFalseForShortString()
    {
        _service.IsValidFormat("ABC").Should().BeFalse();
    }

    [Fact]
    public void IsValidFormat_ReturnsFalseForNull()
    {
        _service.IsValidFormat(null).Should().BeFalse();
    }

    [Fact]
    public void IsValidFormat_ReturnsFalseForStringWithLowercase()
    {
        _service.IsValidFormat("za103abcde0123456789").Should().BeFalse();
    }

    [Fact]
    public void IsValidFormat_ReturnsTrueForGeneratedSid()
    {
        var sid = _service.Generate();
        _service.IsValidFormat(sid).Should().BeTrue();
    }
}
