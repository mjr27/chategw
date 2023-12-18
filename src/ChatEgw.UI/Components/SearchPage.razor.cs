using System.ComponentModel.DataAnnotations;
using ChatEgw.UI.Application;
using ChatEgw.UI.Application.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace ChatEgw.UI.Components;

public partial class SearchPage
{
    [Inject] public required ISearchService SearchService { get; set; }
    [Inject] public required ILogger<SearchPage> Logger { get; set; }
    [Inject] public required IDialogService DialogService { get; set; }

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

    private string _error = "";


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
    private string _prompt = "";

    private void HandleValidSubmit()
    {
        _state = LoadingState.Loading;
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        Task.Run(DoSearch, _cancellationTokenSource.Token);
    }

    private async Task DoSearch()
    {
        _expanded = false;
        _error = "";
        _prompt = "";
        try
        {
            CancellationToken ct = _cancellationTokenSource.Token;
            HashSet<string> folders = TreeService.Selected;
            AnsweringResponse answer = await SearchService.Search(_aiType, Model.Query,
                new SearchFilterRequest
                {
                    Folders = folders.ToArray(),
                    IsEgw = TreeService.EgwWritingsOnly ? true : null
                }, ct);
            Answers = answer.Answers.ToList();
            _state = LoadingState.Completed;
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception e)
        {
            _error = e.Message;
            Logger.LogError(e, "Error while invoking");
            await InvokeAsync(StateHasChanged);
        }
    }

    private void HandleInvalidSubmit()
    {
        _state = LoadingState.New;
    }

    private void HandleShowPrompt(string prompt)
    {
        var parameters = new DialogParameters<PromptDialog> { { x => x.Prompt, prompt } };
        var options = new DialogOptions
        {
            FullWidth = true
        };
        DialogService.Show<PromptDialog>("", parameters, options);
    }
}