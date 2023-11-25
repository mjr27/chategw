using Egw.PubManagement.Application.Messaging.Folders;
using Egw.PubManagement.Persistence.Entities;

using FluentAssertions;

using FluentValidation;
namespace ApplicationTests.Messaging.Folders;

public class RenameFolderTestCase : MediatorTestCase
{
    public RenameFolderTestCase(DatabaseFixture fixture) : base(fixture)
    {
        Db.Languages.Add(new Language("en", "en", "en-us", false, "English", Now) { RootFolderId = 1 });
        Db.Folders.Add(new Folder("English", null, 1, "egwwritings", Now) { Id = 1 });
        Db.SaveChanges();
    }

    [Theory]
    [InlineData(true, "New name")]
    [InlineData(false, "")]
    [InlineData(false, null)]
    public async Task TestRenameFolder(bool success, string newName)
    {
        var cmd = new RenameFolderInput { Id = 1, Title = newName };
        if (success)
        {
            await Execute(cmd);
            Db.Folders.Single().Title.Should().Be(newName);
        }
        else
        {
            await ShouldThrow<ValidationException>(cmd);
        }
    }
}