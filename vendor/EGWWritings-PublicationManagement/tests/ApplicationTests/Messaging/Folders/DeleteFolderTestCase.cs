using Egw.PubManagement.Application.Messaging.Folders;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Persistence.Entities;

using FluentAssertions;

using WhiteEstate.DocFormat.Enums;

namespace ApplicationTests.Messaging.Folders;

public class DeleteFolderTestCase : MediatorTestCase
{
    public DeleteFolderTestCase(DatabaseFixture fixture) : base(fixture)
    {
        Db.Languages.Add(new Language("en", "en", "en-us", false, "English", Now) { RootFolderId = 1 });
        Db.Folders.Add(new Folder("English", null, 1, "root", Now) { Id = 1 });
        Db.Folders.Add(new Folder("Books", 1, 1, "egwwritings", Now) { Id = 2 });
        Db.Folders.Add(new Folder("Bibles", 1, 2, "bible", Now) { Id = 3 });
        Db.Publications.Add(new Publication(1, PublicationType.Bible, "en", "AA", Now));
        Db.PublicationPlacement.Add(new PublicationPlacement(1, 3, 1, Now));
        Db.SaveChanges();
    }

    [Theory]
    [InlineData(typeof(ConflictProblemDetailsException), 1)]
    [InlineData(null, 2)]
    [InlineData(typeof(ConflictProblemDetailsException), 3)]
    [InlineData(typeof(NotFoundProblemDetailsException), 5)]
    [InlineData(typeof(NotFoundProblemDetailsException), -1)]
    public async Task TestDeleteFolder(Type? exception, int folderId)
    {
        var cmd = new DeleteFolderInput { Id = folderId };
        if (exception is null)
        {
            await Execute(cmd);
            Db.Folders.Should().HaveCount(2);
        }
        else
        {
            await this.ShouldThrow(cmd, exception);
        }
    }
}