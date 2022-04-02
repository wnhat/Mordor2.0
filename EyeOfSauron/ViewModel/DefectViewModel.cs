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
    internal class DefectViewModel
    {
    }
    public sealed class Defect
    {
        public Defect(string name, string saction)
        {
            Name = name;
            Saction = saction;
        }

        public string Name { get; }

        public string Saction { get; }
    }
    public sealed class TreesViewModel : ViewModelBase
    {
        private object? _selectedItem;

        public ObservableCollection<Defect> Defects { get; }

        public object? SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public TreesViewModel()
        {
            Defects = new ObservableCollection<Defect>
            {
                new Defect("Defect1", "Saction1"),
                new Defect("Defect1", "Saction2"),
            };
        }
        private static string GenerateString(int length)
        {
            var random = new Random();

            return string.Join(string.Empty,
                Enumerable.Range(0, length)
                .Select(v => (char)random.Next('a', 'z' + 1)));
        }
    }
}
