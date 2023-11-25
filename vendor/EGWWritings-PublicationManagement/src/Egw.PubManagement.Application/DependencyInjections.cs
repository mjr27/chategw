using Egw.PubManagement.Application.GraphQl;
using Egw.PubManagement.Application.GraphQl.Mutations;
using Egw.PubManagement.Application.GraphQl.Scalars;
using Egw.PubManagement.Application.GraphQl.Types;
using Egw.PubManagement.Application.Models;
using Egw.PubManagement.Application.Models.Internal;
using Egw.PubManagement.Application.Services.Filters;
using Egw.PubManagement.Application.Services.Projectors;
using Egw.PubManagement.Core;
using Egw.PubManagement.Core.GraphQl;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence.Enums;
using Egw.PubManagement.Persistence.Services;

using FluentValidation;

using HotChocolate.Data.Filters;
using HotChocolate.Execution.Configuration;

using Mapster;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using WhiteEstate.DocFormat;

namespace Egw.PubManagement.Application;

/// <summary>
/// Application dependency injections
/// </summary>
public static class DependencyInjections
{
    /// <summary>
    /// Adds publications part to graphql builder
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IRequestExecutorBuilder AddPublicationsPart(this IRequestExecutorBuilder builder)
    {
        return builder
                .BindRuntimeType<ParaId, ParaIdType>()
                .BindRuntimeType<Mp3FileName, Mp3FileNameType>()
                .AddConvention<IFilterConvention, ParaIdFilteringConvention>()
                .RegisterDbContext<PublicationDbContext>()
                .AddTypeExtension<WhiteEstatePublicationQueries>()
                .AddTypeExtension<WhiteEstatePublicationMutations>()
                .AddTypeExtension<ArchiveMutations>()
                .AddTypeExtension<CoversMutations>()
                .AddTypeExtension<ExportMutations>()
                .AddTypeExtension<FolderMutations>()
                .AddTypeExtension<LanguageMutations>()
                .AddTypeExtension<PublicationMutations>()
                .AddTypeExtension<ParagraphMutations>()
                .AddTypeExtension<Mp3FileMutations>()
                .AddType<CoverGraphQlType>()
                .AddType<ExportedFileType>()
                .AddType<FolderGraphQlType>()
                .AddType<LanguageType>()
                .AddType<ParagraphType>()
                .AddType<PublicationChapterType>()
                .AddType<PublicationMp3FileType>()
                .AddType<PublicationType>()
            ;
    }


    /// <summary>
    /// Inject application dependencies
    /// </summary>
    /// <param name="services"></param>
    /// <param name="dbConfigurationAction">DB Configuration action</param>
    /// <returns></returns>
    public static IServiceCollection AddPublicationsPart(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> dbConfigurationAction)
    {
        services.AddValidatorsFromAssemblyContaining<LanguageDto>();
        services.AddMediatR(o => o.RegisterServicesFromAssemblyContaining<IMetadataBuilder>());

        services.AddScoped<DefaultProjector>()
            .AddDefaultProjector<Language, LanguageDto, DefaultProjector>()
            .AddDefaultProjector<ExportedFile, ExportedFileDto, DefaultProjector>()
            .AddProjector<Folder, FolderDto, FolderProjector>()
            .AddDefaultProjector<FolderType, FolderTypeDto, DefaultProjector>()
            .AddDefaultProjector<Paragraph, ParagraphDto, DefaultProjector>()
            .AddDefaultProjector<Publication, PublicationDto, DefaultProjector>()
            .AddDefaultProjector<PublicationAuthor, PublicationAuthorDto, DefaultProjector>()
            .AddDefaultProjector<PublicationChapter, PublicationChapterDto, DefaultProjector>()
            .AddDefaultProjector<Mp3File, PublicationMp3FileDto, DefaultProjector>()
            .AddDefaultProjector<PublicationSeries, PublicationSeriesDto, DefaultProjector>()
            .AddDefaultProjector<CoverType, CoverTypeDto, DefaultProjector>()
            .AddDefaultProjector<Cover, CoverDto, DefaultProjector>()
            .AddDefaultProjector<ArchivedPublication, ArchivedPublicationDto, DefaultProjector>();
        services.AddScoped<DefaultFilter>()
            .AddDefaultFilter<Language, DefaultFilter>()
            .AddDefaultFilter<ExportedFile, DefaultFilter>()
            .AddDefaultFilter<FolderType, DefaultFilter>()
            .AddDefaultFilter<Folder, DefaultFilter>()
            .AddDefaultFilter<Paragraph, DefaultFilter>()
            .AddDefaultFilter<Publication, DefaultFilter>()
            .AddDefaultFilter<PublicationAuthor, DefaultFilter>()
            .AddDefaultFilter<PublicationChapter, DefaultFilter>()
            .AddDefaultFilter<Mp3File, DefaultFilter>()
            .AddDefaultFilter<PublicationSeries, DefaultFilter>()
            .AddDefaultFilter<CoverType, DefaultFilter>()
            .AddDefaultFilter<Cover, DefaultFilter>()
            .AddDefaultFilter<ArchivedPublication, DefaultFilter>();

        services.AddDbContextFactory<PublicationDbContext>(dbConfigurationAction);
        return services;
    }

    private static IServiceCollection AddProjector<TInput, TOutput, TClass>(this IServiceCollection services)
        where TClass : class, IEntityProjector<TInput, TOutput> where TOutput : IProjectedDto<TInput>
    {
        return services.AddScoped<IEntityProjector<TInput, TOutput>, TClass>();
    }

    private static IServiceCollection AddDefaultProjector<TInput, TOutput, TClass>(this IServiceCollection services)
        where TClass : IEntityProjector<TInput, TOutput> where TOutput : IProjectedDto<TInput>
    {
        return services.AddScoped<IEntityProjector<TInput, TOutput>>(
            o => o.GetRequiredService<TClass>()
        );
    }

    private static IServiceCollection AddDefaultFilter<TInput, TClass>(this IServiceCollection services)
        where TClass : IEntityPrefilter<TInput>
    {
        return services.AddScoped<IEntityPrefilter<TInput>>(
            o => o.GetRequiredService<TClass>()
        );
    }

    /// <summary>
    /// Add publication-related mapping
    /// </summary>
    /// <param name="config"></param>
    /// <param name="configuration">Media root</param>
    /// <returns></returns>
    public static TypeAdapterConfig AddPublicationMapping(this TypeAdapterConfig config, IConfiguration configuration)
    {
        config.NewConfig<Language, LanguageDto>();
        config.NewConfig<ExportedFile, ExportedFileDto>();
        config.NewConfig<Folder, FolderDto>()
            .Map(r => r.ChildFolderCount, src => src.Children.Count)
            .Map(r => r.ChildPublicationCount, src => src.Placements.Count);
        config.NewConfig<FolderType, FolderTypeDto>()
            .Map(r => r.Id, src => src.FolderTypeId);
        config.NewConfig<Mp3File, PublicationMp3FileDto>();
        config.NewConfig<Paragraph, ParagraphDto>()
            .Map(
                r => r.Pagination,
                src => src.Metadata!.Pagination
            )
            .Map(r => r.Date, r => r.Metadata!.Date)
            .Map(r => r.EndDate, r => r.Metadata!.EndDate)
            .Map(r => r.Pagination, r => r.Metadata!.Pagination)
            .Map(r => r.BibleMetadata, r => r.Metadata!.BibleMetadata)
            .Map(r => r.LtMsMetadata, r => r.Metadata!.LtMsMetadata)
            .Map(
                r => r.RefCodeShort,
                src => src.Metadata != null
                    ? IRefCodeGenerator.Instance.Short(src.Publication.Type, src.Publication.Code, src.Metadata)
                    : null
            )
            .Map(
                r => r.RefCodeLong,
                src => src.Metadata != null
                    ? IRefCodeGenerator.Instance.Long(src.Publication.Type, src.Publication.Title, src.Metadata)
                    : null
            );
        config.NewConfig<Publication, PublicationDto>()
            .Map(r => r.Permission,
                src => src.Placement == null ? PublicationPermissionEnum.Hidden : src.Placement.Permission)
            .Map(r => r.FolderId, src => src.Placement == null ? (int?)null : src.Placement.FolderId)
            .Map(r => r.Order, src => src.Placement == null ? 0 : src.Placement.Order);
        config.NewConfig<PublicationAuthor, PublicationAuthorDto>();
        config.NewConfig<PublicationChapter, PublicationChapterDto>();
        config.NewConfig<PublicationSeries, PublicationSeriesDto>();
        config.NewConfig<CoverType, CoverTypeDto>()
            .Map(r => r.CoverCount, src => src.Covers.Count);
        return config;
    }
}