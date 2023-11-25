using Egw.PubManagement.Application.Messaging.Languages;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Persistence.Entities;

using FluentValidation;

using Microsoft.EntityFrameworkCore;
namespace ApplicationTests.Messaging.Languages;

public class UpdateLanguageTestCase : MediatorTestCase
{
    public UpdateLanguageTestCase(DatabaseFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task TestUpdateMissingLanguage()
    {
        await ShouldThrow<NotFoundProblemDetailsException>(new UpdateLanguageInput { Code = "en" });
    }

    [Theory]
    [InlineData(true, "eng", null, null, null)]
    [InlineData(true, "eng", "ru", null, null)]
    [InlineData(false, "eng", "russss", null, null)]
    [InlineData(false, "eng", "", null, null)]
    [InlineData(true, "eng", null, "ru", null)]
    [InlineData(true, "eng", null, "ru-ru", null)]
    [InlineData(false, "eng", null, "russs", null)]
    [InlineData(false, "eng", null, "r", null)]
    [InlineData(false, "eng", null, null, "")]
    [InlineData(true, "eng", null, null, "title")]
    public async Task UpdateValidation(bool success, string code, string? egwCode, string? bcp47, string? title)
    {
        Db.Languages.Add(new Language("eng", "en", "en-us", true, "English", Now));
        await Db.SaveChangesAsync();
        var request = new UpdateLanguageInput { Code = code, EgwCode = egwCode, Bcp47Code = bcp47, Title = title };

        if (success)
        {
            await Execute(request);
        }
        else
        {
            await ShouldThrow<ValidationException>(request);
        }
    }


    [Fact]
    public async Task TestDuplicateEgwCode()
    {
        Db.Languages.Add(new Language("eng", "en", "en-us", true, "English", Now));
        Db.Languages.Add(new Language("ukr", "ua", "uk-uk", true, "Ukrainian", Now));
        await Db.SaveChangesAsync();
        await ShouldThrow<DbUpdateException>(new UpdateLanguageInput { Code = "ukr", EgwCode = "en" });
        await ShouldThrow<DbUpdateException>(new UpdateLanguageInput { Code = "ukr", EgwCode = "EN" });
    }

    [Fact]
    public async Task TestDuplicateBcpCode()
    {
        Db.Languages.Add(new Language("eng", "en", "en-us", true, "English", Now));
        Db.Languages.Add(new Language("ukr", "ua", "uk-uk", true, "Ukrainian", Now));
        await Db.SaveChangesAsync();
        await ShouldThrow<DbUpdateException>(new UpdateLanguageInput { Code = "ukr", Bcp47Code = "en-us" });
        await ShouldThrow<DbUpdateException>(new UpdateLanguageInput { Code = "ukr", Bcp47Code = "en-US" });
    }
}