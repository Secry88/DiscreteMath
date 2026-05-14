using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using DiscreteMath.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DiscreteMath.Views;

public partial class TeacherRegionIdentificationView : UserControl
{
    public TeacherRegionIdentificationView() => InitializeComponent();

    private async void OnSelectImageClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not TeacherRegionIdentificationViewModel vm) return;

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            AllowMultiple = false,
            Title = "Выберите изображение диаграммы",
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Изображения")
                {
                    Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif" }
                }
            }
        });

        if (files.Count == 0) return;

        try
        {
            await using var stream = await files[0].OpenReadAsync();
            using var memStream = new MemoryStream();
            await stream.CopyToAsync(memStream);
            var base64 = Convert.ToBase64String(memStream.ToArray());
            vm.SetImageData(base64);
        }
        catch { /* ignore read errors */ }
    }

    private void OnClearImageClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is TeacherRegionIdentificationViewModel vm)
            vm.ClearImage();
    }
}
