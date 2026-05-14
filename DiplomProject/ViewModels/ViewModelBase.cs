using CommunityToolkit.Mvvm.ComponentModel;
using DiscreteMath.Models;

namespace DiscreteMath.ViewModels
{
    public abstract class ViewModelBase : ObservableObject
    {
        public static KarmanovContext db = new KarmanovContext();
    }
}
