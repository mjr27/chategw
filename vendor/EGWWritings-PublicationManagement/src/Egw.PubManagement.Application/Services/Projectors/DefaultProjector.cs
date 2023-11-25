using Egw.PubManagement.Application.Models;
using Egw.PubManagement.Core;
using Egw.PubManagement.Persistence.Entities;

using Mapster;

using MapsterMapper;

using Microsoft.EntityFrameworkCore;
namespace Egw.PubManagement.Application.Services.Projectors;

/// <summary>
/// Default projector
/// </summary>
public class DefaultProjector :
    IEntityProjector<ArchivedPublication, ArchivedPublicationDto>,
    IEntityProjector<Cover, CoverDto>,
    IEntityProjector<CoverType, CoverTypeDto>,
    IEntityProjector<Language, LanguageDto>,
    IEntityProjector<ExportedFile, ExportedFileDto>,
    IEntityProjector<FolderType, FolderTypeDto>,
    IEntityProjector<Paragraph, ParagraphDto>,
    IEntityProjector<Publication, PublicationDto>,
    IEntityProjector<PublicationAuthor, PublicationAuthorDto>,
    IEntityProjector<PublicationChapter, PublicationChapterDto>,
    IEntityProjector<Mp3File, PublicationMp3FileDto>,
    IEntityProjector<PublicationSeries, PublicationSeriesDto>
{
    private readonly IMapper _mapper;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="mapper">Mapper</param>
    public DefaultProjector(IMapper mapper)
    {
        _mapper = mapper;
    }
    /// <inheritdoc />
    public Task<IQueryable<ArchivedPublicationDto>> Project(IQueryable<ArchivedPublication> queryable,
        CancellationToken cancellationToken)
        => Task.FromResult(_mapper.From(queryable).ProjectToType<ArchivedPublicationDto>());
    /// <inheritdoc />
    public Task<IQueryable<LanguageDto>> Project(IQueryable<Language> queryable,
        CancellationToken cancellationToken)
        => Task.FromResult(_mapper.From(queryable).ProjectToType<LanguageDto>());

    /// <inheritdoc />
    public Task<IQueryable<ExportedFileDto>> Project(IQueryable<ExportedFile> queryable,
        CancellationToken cancellationToken)
        => Task.FromResult(
            _mapper.From(queryable).ProjectToType<ExportedFileDto>());

    /// <inheritdoc />
    public Task<IQueryable<FolderTypeDto>> Project(IQueryable<FolderType> queryable,
        CancellationToken cancellationToken)
        => Task.FromResult(_mapper.From(queryable).ProjectToType<FolderTypeDto>());


    /// <inheritdoc />
    public Task<IQueryable<ParagraphDto>> Project(IQueryable<Paragraph> queryable, CancellationToken cancellationToken)
        => Task.FromResult(_mapper.From(queryable.Include(r => r.Metadata).Include(r => r.Publication)).ProjectToType<ParagraphDto>());

    /// <inheritdoc />
    public Task<IQueryable<PublicationDto>> Project(IQueryable<Publication> queryable,
        CancellationToken cancellationToken)
        => Task.FromResult(_mapper.From(queryable).ProjectToType<PublicationDto>());

    /// <inheritdoc />
    public Task<IQueryable<PublicationAuthorDto>> Project(IQueryable<PublicationAuthor> queryable,
        CancellationToken cancellationToken)
        => Task.FromResult(_mapper.From(queryable).ProjectToType<PublicationAuthorDto>());


    /// <inheritdoc />
    public Task<IQueryable<PublicationChapterDto>> Project(IQueryable<PublicationChapter> queryable,
        CancellationToken cancellationToken)
        => Task.FromResult(_mapper.From(queryable).ProjectToType<PublicationChapterDto>());

    /// <inheritdoc />
    public Task<IQueryable<PublicationMp3FileDto>> Project(IQueryable<Mp3File> queryable,
        CancellationToken cancellationToken)
        => Task.FromResult(_mapper.From(queryable).ProjectToType<PublicationMp3FileDto>());

    /// <inheritdoc />
    public Task<IQueryable<PublicationSeriesDto>> Project(IQueryable<PublicationSeries> queryable,
        CancellationToken cancellationToken)
        => Task.FromResult(_mapper.From(queryable).ProjectToType<PublicationSeriesDto>());

    /// <inheritdoc />
    public Task<IQueryable<CoverDto>> Project(IQueryable<Cover> queryable, CancellationToken cancellationToken)
        => Task.FromResult(_mapper.From(queryable).ProjectToType<CoverDto>());

    /// <inheritdoc />
    public Task<IQueryable<CoverTypeDto>> Project(IQueryable<CoverType> queryable, CancellationToken cancellationToken)
        => Task.FromResult(_mapper.From(queryable).ProjectToType<CoverTypeDto>());
}