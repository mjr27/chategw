using System.ComponentModel.DataAnnotations;
using ChatEgw.UI.Application;
using ChatEgw.UI.Application.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ChatEgw.UI.Pages;

public partial class Index
{
    [Inject] public required ISearchService SearchService { get; set; }
    [Inject] public required IInstructGenerationService InstructGenerationService { get; set; }
    [Inject] public required ILogger<Index> Logger { get; set; }

    private enum LoadingState
    {
        New,
        Loading,
        Completed
    }

    private LoadingState _state = LoadingState.New;

    private class AiAnswerModel
    {
        [Required] public string Query { get; set; } = "";
    }

    private EditContext _editContext = null!;


    private void HandleAdornmentClick()
    {
        if (_editContext.Validate())
        {
            HandleValidSubmit();
        }
    }

    protected override void OnInitialized()
    {
        _editContext = new EditContext(Model);
    }

    private CancellationTokenSource _cancellationTokenSource = new();

    private void HandleValidSubmit()
    {
        _state = LoadingState.Loading;
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        Task.Run(DoSearch, _cancellationTokenSource.Token);
    }

    private async Task DoSearch()
    {
        _aiResponseCompleted = false;
        _showAiResponse = false;
        AiResponseWords = null;
        CancellationToken ct = _cancellationTokenSource.Token;
        HashSet<string> folders = TreeService.GetSelected();
        AnsweringResponse answer = await SearchService.Search(_aiType, Model.Query,
            new SearchFilterRequest
            {
                Folders = folders.ToArray()
            }, ct);
        Answers = answer.Answers.ToList();
        _state = LoadingState.Completed;
        await InvokeAsync(StateHasChanged);
        if (answer.IsAiResponse && answer.Answers.Any())
        {
            _showAiResponse = true;
            await InvokeAsync(StateHasChanged);
            AiResponseWords = new List<string>();
            try
            {
                await foreach (string word in InstructGenerationService.AutoComplete(answer.UpdatedQuery, Answers, ct))
                {
                    AiResponseWords.Add(word);
                    await InvokeAsync(StateHasChanged);
                }

                _aiResponseCompleted = true;
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception e)
            {
                AiResponseWords.Add(e.Message);
                _aiResponseCompleted = true;
                Logger.LogError(e, "Error while invoking OpenAI API");
                await InvokeAsync(StateHasChanged);
            }
        }
        else
        {
            _aiResponseCompleted = true;
            await InvokeAsync(StateHasChanged);
        }
    }

    private void HandleInvalidSubmit()
    {
        _state = LoadingState.New;
    }
}