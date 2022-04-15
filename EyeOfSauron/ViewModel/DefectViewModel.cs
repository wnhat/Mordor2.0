using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace EyeOfSauron.ViewModel
{

    public sealed class DefectListViewModel : ViewModelBase
    {
        private object? _selectedItem;

        public DefectList defectList { get; }

        public object? SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public DefectListViewModel()
        {
            defectList = new DefectList();
        }
    }
    public sealed class AetDetailDefect
    {
        public AetDetailDefect(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

    }
    public sealed class DefectList : ViewModelBase
    {
        public ObservableCollection<AetDetailDefect> list { get; }
        public DefectList()
        {
            list = new ObservableCollection<AetDetailDefect>
            {
                new AetDetailDefect("InnerDefect1"),
                new AetDetailDefect("InnerDefect2"),
            };
        }
    }
}
