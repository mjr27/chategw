using Egw.PubManagement.Application.Messaging.Folders;
using Egw.PubManagement.Persistence.Entities;

using FluentAssertions;

namespace ApplicationTests.Messaging.Folders;

public class MoveFolderTestCase : MediatorTestCase
{
    public MoveFolderTestCase(DatabaseFixture fixture) : base(fixture)
    {
        Db.Languages.Add(new Language("en", "en", "en-us", false, "English", Now) { RootFolderId = 1 });
        Db.Folders.Add(new Folder("English", null, 1, "egwwritings", Now) { Id = 1 });
        Db.Folders.Add(new Folder("F1", 1, 1, "egwwritings", Now) { Id = 2 });
        Db.Folders.Add(new Folder("F11", 2, 1, "egwwritings", Now) { Id = 21 });
        Db.Folders.Add(new Folder("F12", 2, 2, "egwwritings", Now) { Id = 22 });
        Db.Folders.Add(new Folder("F12", 2, 3, "egwwritings", Now) { Id = 23 });
        Db.Folders.Add(new Folder("F2", 1, 2, "egwwritings", Now) { Id = 3 });
        Db.Folders.Add(new Folder("F11", 3, 1, "egwwritings", Now) { Id = 31 });
        Db.Folders.Add(new Folder("F12", 3, 2, "egwwritings", Now) { Id = 32 });
        Db.SaveChanges();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(5)]
    [InlineData(-100)]
    public async Task TesMoveFolder(int? newPosition)
    {
        await Execute(new MoveFolderInput { Id = 31, NewParent = 32, NewPosition = newPosition });
        Folder folder = Db.Folders.Single(r => r.Id == 31);
        Folder parentFolder = Db.Folders.Single(r => r.Id == 32);
        folder.Order.Should().Be(1);
        folder.ParentId.Should().Be(32);
        parentFolder.Order.Should().Be(1);
    }

    [Theory]
    [InlineData(null, 3)]
    [InlineData(-10, 1)]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 3)]
    [InlineData(100, 3)]
    public async Task TesMoveFolderToExisting(int? newPosition, int expectedPosition)
    {
        await Execute(new MoveFolderInput { Id = 21, NewParent = 3, NewPosition = newPosition });
        Folder folder = Db.Folders.Single(r => r.Id == 21);
        folder.ParentId.Should().Be(3);
        folder.Order.Should().Be(expectedPosition);
        Db.Folders.Single(r => r.Id == 22).Order.Should().Be(1);
    }
    
    [Theory]
    [InlineData(null, 1)]
    [InlineData(-10, 1)]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(2, 2)] //1 TODO: This should be 2
    [InlineData(3, 3)] //2 TODO: This should be 3
    [InlineData(4, 3)]
    [InlineData(100, 3)]
    public async Task TesMoveFolderToSame(int? newPosition, int expectedPosition)
    {
        await Execute(new MoveFolderInput { Id = 21, NewParent = 2, NewPosition = newPosition });
        Folder folder = Db.Folders.Single(r => r.Id == 21);
        Folder folder2 = Db.Folders.Single(r => r.Id == 22);
        folder.ParentId.Should().Be(2);
        folder.Order.Should().Be(expectedPosition);
        folder2.Order.Should().Be(expectedPosition == 1 ? 2 : 1);
    }
}