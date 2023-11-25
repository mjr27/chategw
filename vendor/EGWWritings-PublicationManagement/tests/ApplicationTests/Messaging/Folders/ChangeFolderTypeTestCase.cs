using Egw.PubManagement.Application.Messaging.Folders;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Persistence.Entities;

using FluentAssertions;

using WhiteEstate.DocFormat.Enums;
namespace ApplicationTests.Messaging.Folders;

public class ChangeFolderTypeTestCase : MediatorTestCase
{
    public ChangeFolderTypeTestCase(DatabaseFixture fixture) : base(fixture)
    {
        Db.Languages.Add(new Language("en", "en", "en-us", false, "English", Now) { RootFolderId = 1 });
        Db.Folders.Add(new Folder("English", null, 1, "egwwritings", Now) { Id = 1 });
        Db.Publications.Add(new Publication(1, PublicationType.Book, "en", "AA", Now));
        Db.PublicationPlacement.Add(new PublicationPlacement(1, 1, 1, Now));
        Db.SaveChanges();
    }

    [Fact]
    public async Task TestChangeFolderType()
    {
        await Execute(new ChangeFolderTypeInput { Id = 1, Type = "biography" });
        Db.Folders.Single().TypeId.Should().Be("biography");
    }

    [Fact]
    public async Task MissingFolder()
    {
        await ShouldThrow<NotFoundProblemDetailsException>(new ChangeFolderTypeInput { Id = 100, Type = "biography" });
    }

    [Fact]
    public async Task MissingType()
    {
        await ShouldThrow<NotFoundProblemDetailsException>(new ChangeFolderTypeInput { Id = 1, Type = "" });
    }

    [Fact]
    public async Task TestWrongFolderType()
    {
        await ShouldThrow<ValidationProblemDetailsException>(new ChangeFolderTypeInput { Id = 1, Type = "bible" });
    }

}