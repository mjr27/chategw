using Egw.PubManagement.Application.Messaging.Folders;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Persistence.Entities;

using FluentAssertions;

using FluentValidation;
namespace ApplicationTests.Messaging.Folders;

public class CreateFolderTestCase : MediatorTestCase
{
    [Theory]
    [InlineData(typeof(ValidationException), -1, "egwwritings")]
    [InlineData(typeof(ValidationException), 0, "egwwritings")]
    [InlineData(null, 1001, "egwwritings",2)]
    [InlineData(null, 1002, "egwwritings",1)]
    [InlineData(typeof(NotFoundProblemDetailsException), 1003, "egwwritings")]
    async Task CreateFolder(Type? exception, int parentId, string type, int order = 1)
    {
        var cmd = new CreateFolderInput { ParentId = parentId, Title = "New Folder", Type = type };
        if (exception is null)
        {
            await Execute(cmd);
            Db.Folders.Should().HaveCount(3);
            Folder folder = Db.Folders.Single(r => r.Title == "New Folder");
            folder.Order.Should().Be(order);
        }
        else
        {
            await this.ShouldThrow(cmd, exception);
        }
    }

    public CreateFolderTestCase(DatabaseFixture fixture) : base(fixture)
    {
        var l = new Language("en", "en", "en-us", false, "English", Now);
        Db.Languages.Add(l);
        var f1 = new Folder("English", null, 1, "root", Now) { Id = 1001 };
        Db.Folders.Add(f1);
        Db.SaveChanges();
        var f2 = new Folder("EGW Writings", f1.Id, 1, "egwwritings", Now) { Id = 1002 };
        l.RootFolderId = f1.Id;
        Db.Folders.Add(f2);
        Db.SaveChanges();
    }
}