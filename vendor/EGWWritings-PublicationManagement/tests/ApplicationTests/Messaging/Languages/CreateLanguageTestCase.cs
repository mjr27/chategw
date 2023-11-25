using Egw.PubManagement.Application.Messaging.Languages;

using FluentAssertions;

using FluentValidation;

using WhiteEstate.DocFormat.Enums;
namespace ApplicationTests.Messaging.Languages;

public class CreateLanguageTestCase : MediatorTestCase
{
    public CreateLanguageTestCase(DatabaseFixture fixture) : base(fixture)
    {
    }

    [Theory]
    [InlineData(true, "eng", "en", "en-us", "English")]
    [InlineData(false, "eng", "en", "en-us", "")]
    [InlineData(false, "eng", "en", "", "English")]
    [InlineData(false, "eng", "", "en-us", "English")]
    [InlineData(false, "", "en", "en-us", "English")]
    [InlineData(false, "eng", "en", "e", "English")] // invalid regexp
    public async Task CreateLanguage(bool success, string code, string egwCode, string bcp47, string title)
    {
        var request = new CreateLanguageInput
        {
            Code = code,
            EgwCode = egwCode,
            Bcp47Code = bcp47,
            Title = title,
            Direction = WemlTextDirection.LeftToRight
        };
        if (success)
        {
            await Execute(request);
        }
        else
        {
            Func<Task> act = () => Execute(request);
            await act.Should().ThrowAsync<ValidationException>();
        }

        Db.Languages.Count().Should().Be(success ? 1 : 0);
        // Language language = await Db.Languages.SingleAsync();
        // Assert.Equal(request.Title, language.Title);
        // Assert.Equal(request.Code, language.Code);
    }
}