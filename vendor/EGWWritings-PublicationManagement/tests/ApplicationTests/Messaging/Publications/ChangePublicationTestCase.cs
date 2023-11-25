using Egw.PubManagement.Application.Messaging.Publications;
using Egw.PubManagement.Persistence.Entities;

using FluentAssertions;

using WhiteEstate.DocFormat.Enums;
namespace ApplicationTests.Messaging.Publications;

public class ChangePublicationTestCase : MediatorTestCase
{
    public ChangePublicationTestCase(DatabaseFixture fixture) : base(fixture)
    {
        Db.Languages.Add(new Language("en", "en", "en-us", false, "English", Now) { RootFolderId = 1 });
        Db.Folders.Add(new Folder("English", null, 1, "egwwritings", Now) { Id = 1 });
        Db.Folders.Add(new Folder("English books part 1", 1, 1, "egwwritings", Now) { Id = 2 });
        Db.Folders.Add(new Folder("English books part 2", 1, 1, "egwwritings", Now) { Id = 3 });

        Db.Publications.Add(new Publication(1, PublicationType.Book, "en", "AA", Now));
        Db.PublicationPlacement.Add(new PublicationPlacement(1, 2, 1, Now));

        Db.Publications.Add(new Publication(2, PublicationType.Book, "en", "AB", Now));
        Db.PublicationPlacement.Add(new PublicationPlacement(2, 2, 2, Now));

        Db.Publications.Add(new Publication(3, PublicationType.Book, "en", "AC", Now));
        Db.PublicationPlacement.Add(new PublicationPlacement(3, 2, 3, Now));

        Db.SaveChanges();
    }

    [Theory]
    [InlineData(5, 5)]
    [InlineData(null, 4)]
    public async Task TestChangePublicationTocDepth(int? newDepth, int expectedDepth)
    {
        await Execute(new SetPublicationTocDepthInput { PublicationId = 1, TocDepth = newDepth });

        PublicationPlacement placement = Db.PublicationPlacement.Single(r => r.PublicationId == 1);
        placement.TocDepth.Should().Be(expectedDepth);
    }

    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(1, 2, 2)]
    [InlineData(1, 3, 3)]
    public async Task TestChangePublicationOrder(int publicatioId, int newOrder, int expectedOrder)
    {
        await Execute(new SetPublicationOrderInput { PublicationId = publicatioId, Order = newOrder });

        PublicationPlacement placement = Db.PublicationPlacement.Single(r => r.PublicationId == publicatioId);
        placement.Order.Should().Be(expectedOrder);
    }

    [Theory]
    [InlineData(1, 2, null, 3)]
    [InlineData(1, 2, 99, 3)]
    [InlineData(1, 2, 3, 3)]
    [InlineData(1, 3, null, 1)]
    [InlineData(1, 3, 99, 1)]
    public async Task TestMovePublication(int publicatioId, int folderId, int? order, int expectedOrder)
    {
        await Execute(new MovePublicationInput { PublicationId = publicatioId, FolderId = folderId, Order = order });

        PublicationPlacement placement = Db.PublicationPlacement.Single(r => r.PublicationId == publicatioId);
        placement.Order.Should().Be(expectedOrder);
    }
}