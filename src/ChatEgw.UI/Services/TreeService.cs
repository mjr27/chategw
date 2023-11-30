using ChatEgw.UI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChatEgw.UI.Services;

public class TreeModel(string id, string title)
{
    public string Id { get; } = id;
    public TreeModel? Parent { get; set; }
    public string Title { get; } = title;
    public HashSet<TreeModel> Children { get; set; } = new();
    public bool IsExpanded { get; set; }
    public bool? IsChecked { get; set; } = false;
    public bool HasChildren => Children.Any();


    public void AddChild(TreeModel child)
    {
        child.Parent = this;
        Children.Add(child);
    }

    public void SwitchCheck()
    {
        IsChecked = IsChecked switch
        {
            null => false,
            false => true,
            true => false
        };
        SetCheck(IsChecked ?? false);
        UpdateParentChecks();
    }

    private void UpdateParentChecks()
    {
        if (Parent == null)
        {
            return;
        }

        if (Parent.Children.All(r => r.IsChecked == true))
        {
            Parent.IsChecked = true;
        }
        else if (Parent.Children.All(r => r.IsChecked == false))
        {
            Parent.IsChecked = false;
        }
        else
        {
            Parent.IsChecked = null;
        }

        Parent.UpdateParentChecks();
    }

    private void SetCheck(bool isChecked)
    {
        IsChecked = isChecked;
        foreach (TreeModel child in Children)
        {
            child.SetCheck(isChecked);
        }
    }
}

public class TreeService(IDbContextFactory<SearchDbContext> dbContextFactory)
{
    public HashSet<TreeModel> Tree { get; set; } = new();

    public async Task Initialize()
    {
        await using SearchDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
        List<SearchNode> folders = await dbContext.Nodes
            .Where(r => r.IsFolder)
            .OrderBy(r => r.GlobalOrder)
            .ToListAsync();
        var outputFolders = new HashSet<TreeModel>();
        var allFolders = new Dictionary<string, TreeModel>();
        foreach (SearchNode folder in folders)
        {
            var treeItem = new TreeModel(folder.Id, folder.Title);
            allFolders[folder.Id] = treeItem;
            if (folder.ParentId == null)
            {
                outputFolders.Add(treeItem);
            }
            else
            {
                allFolders[folder.ParentId].AddChild(treeItem);
            }
        }

        Tree = outputFolders;
    }

    public HashSet<string> GetSelected()
    {
        var selected = new HashSet<string>();
        foreach (TreeModel item in Tree)
        {
            UpdateSelected(item, selected);
        }

        return selected;
    }

    /// <summary> Is EGW Writings Only </summary>
    public bool EgwWritingsOnly { get; set; } = true;

    private static void UpdateSelected(TreeModel model, ISet<string> selected)
    {
        
        switch (model.IsChecked)
        {
            case true:
                selected.Add(model.Id);
                break;
            case false:
                break;
            case null:
                foreach (TreeModel child in model.Children)
                {
                    UpdateSelected(child, selected);
                }

                break;
        }
    }
}