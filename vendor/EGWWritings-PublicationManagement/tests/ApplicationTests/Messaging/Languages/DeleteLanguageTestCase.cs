using Egw.PubManagement.Application.Messaging.Languages;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Persistence.Entities;

using FluentAssertions;

using WhiteEstate.DocFormat.Enums;
namespace ApplicationTests.Messaging.Languages;

public class DeleteLanguageTestCase : MediatorTestCase
{
    public DeleteLanguageTestCase(DatabaseFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task DeleteMissingLanguage()
    {
        await ShouldThrow<NotFoundProblemDetailsException>(new DeleteLanguageInput { Code = "en" });
    }

    [Fact]
    public async Task DeleteExistingLanguage()
    {
        Db.Languages.Add(new Language("en", "en", "en-en", false, "English", Now));
        await Db.SaveChangesAsync();
        Db.Languages.Count().Should().Be(1);
        await Execute(new DeleteLanguageInput { Code = "en" });
        Db.Languages.Count().Should().Be(0);
    }

    [Fact]
    public async Task DeleteLanguageWithFolder()
    {
        Db.Languages.Add(new Language("en", "en", "en-en", false, "English", Now));
        var folder = new Folder("English", null, 1, "egwwritings", Now);
        Db.Folders.Add(folder);
        await Db.SaveChangesAsync();
        Db.Languages.Single().RootFolderId = folder.Id;
        await Db.SaveChangesAsync();
        await ShouldThrow<ConflictProblemDetailsException>(new DeleteLanguageInput { Code = "en" });
    }

    [Fact]
    public async Task DeleteLanguageWithPublications()
    {
        Db.Languages.Add(new Language("en", "en", "en-en", false, "English", Now));
        Db.Publications.Add(new Publication(127, PublicationType.Book, "en", "AA", Now));
        await Db.SaveChangesAsync();
        await ShouldThrow<ConflictProblemDetailsException>(new DeleteLanguageInput { Code = "en" });
    }
}