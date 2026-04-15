using Avalonia;
using Avalonia.Controls.Primitives;
using System.Windows.Input;

namespace DiplomProject.CustomControls;

public class TheoryCategoryCardControl : TemplatedControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<TheoryCategoryCardControl, string>(nameof(Title));

    public static readonly StyledProperty<string> DescriptionProperty =
        AvaloniaProperty.Register<TheoryCategoryCardControl, string>(nameof(Description));

    public static readonly StyledProperty<ICommand> CommandProperty =
        AvaloniaProperty.Register<TheoryCategoryCardControl, ICommand>(nameof(Command));

    public static readonly StyledProperty<object> CommandParameterProperty =
        AvaloniaProperty.Register<TheoryCategoryCardControl, object>(nameof(CommandParameter));

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

    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
}