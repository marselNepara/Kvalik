using System;
using Avalonia.Controls;

namespace MatyeApp.Controllers;

public class NavigationController
{
    private ContentControl? _contentArea;

    public UserControl? CurrentView { get; private set; }

    public event Action<UserControl>? NavigationChanged;

    public void SetContentArea(ContentControl contentArea)
    {
        _contentArea = contentArea;
    }

    public void Navigate(UserControl view)
    {
        CurrentView = view;
        if (_contentArea != null)
            _contentArea.Content = view;
        NavigationChanged?.Invoke(view);
    }
}
