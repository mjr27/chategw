using ChatEgw.UI.Application;
using ChatEgw.UI.Application.Impl;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ChatEgwApplicationExtensions
{
    public static IServiceCollection AddApplicationPart(this IServiceCollection services)
    {
        return services
            .AddSingleton<IQueryEmbeddingService, PythonInteropServiceImpl>()
            .AddSingleton<IQuestionAnsweringService, PythonInteropServiceImpl>()
            .AddSingleton<IQueryPreprocessService, PythonInteropServiceImpl>()
            .AddSingleton<IInstructGenerationService, OpenAiInstructGenerationServiceImpl>()
            .AddSingleton<IRawSearchEngine, RawSearchEngineImpl>()
            .AddSingleton<ISearchService, SearchServiceImpl>();
    }
}