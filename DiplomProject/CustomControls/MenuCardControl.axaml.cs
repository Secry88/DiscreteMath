using Avalonia;
using Avalonia.Controls.Primitives;
using System.Windows.Input;

namespace DiplomProject.CustomControls;

public class MenuCardControl : TemplatedControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<MenuCardControl, string>(nameof(Title));

    public static readonly StyledProperty<string> DescriptionProperty =
        AvaloniaProperty.Register<MenuCardControl, string>(nameof(Description));

    public static readonly StyledProperty<string> IconProperty =
        AvaloniaProperty.Register<MenuCardControl, string>(nameof(Icon));

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<MenuCardControl, ICommand?>(nameof(Command));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public string Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
}