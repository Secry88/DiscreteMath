using CommunityToolkit.Mvvm.ComponentModel;
using DiplomProject.Models;

namespace DiplomProject.ViewModels
{
    public abstract class ViewModelBase : ObservableObject
    {
        public static KarmanovContext db = new KarmanovContext();
    }
}
