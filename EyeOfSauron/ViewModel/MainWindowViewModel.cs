using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Windows.Data;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using EyeOfSauron.MyUserControl;

namespace EyeOfSauron.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(ISnackbarMessageQueue snackbarMessageQueue, UserInfoViewModel userInfoViewModel)
        {
            DemoItems = new ObservableCollection<DemoItem>(new[]
            {
                new DemoItem("Color Tool",
                typeof(ColorTool) )
            });
            UserInfo = userInfoViewModel;
            SelectedIndex = 0;
            
            _demoItemsView = CollectionViewSource.GetDefaultView(DemoItems);
            _demoItemsView.Filter = DemoItemsFilter;


            DemoItems.Add(new("ProductSelectWindow", typeof(MyUserControl.ProductSelectWindow)));
            SelectedItem = DemoItems[1];

            HomeCommand = new CommandImplementation(
                _ =>
                {
                    SearchKeyword = string.Empty;
                    SelectedIndex = 0;
                });

            MovePrevCommand = new CommandImplementation(
                _ =>
                {
                    if (!string.IsNullOrWhiteSpace(SearchKeyword))
                        SearchKeyword = string.Empty;

                    SelectedIndex--;
                },
                _ => SelectedIndex > 0);

            MoveNextCommand = new CommandImplementation(
               _ =>
               {
                   if (!string.IsNullOrWhiteSpace(SearchKeyword))
                       SearchKeyword = string.Empty;

                   SelectedIndex++;
               },
               _ => SelectedIndex < DemoItems.Count - 1);
        }

        private readonly ICollectionView _demoItemsView;
        private UserInfoViewModel userInfo;
        private DemoItem? _selectedItem;
        private int _selectedIndex;
        private string? _searchKeyword;
        private bool _controlsEnabled = true;

        public string? SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                if (SetProperty(ref _searchKeyword, value))
                {
                    _demoItemsView.Refresh();
                }
            }
        }

        public ObservableCollection<DemoItem> DemoItems { get; }

        public UserInfoViewModel UserInfo
        {
            get => userInfo;
            set => SetProperty(ref userInfo, value);
        }

        public DemoItem? SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }

        public bool ControlsEnabled
        {
            get => _controlsEnabled;
            set => SetProperty(ref _controlsEnabled, value);
        }

        public CommandImplementation HomeCommand { get; }
        public CommandImplementation MovePrevCommand { get; }
        public CommandImplementation MoveNextCommand { get; }

        private static IEnumerable<DemoItem> GenerateDemoItems(ISnackbarMessageQueue snackbarMessageQueue)
        {
            if (snackbarMessageQueue is null)
                throw new ArgumentNullException(nameof(snackbarMessageQueue));

            yield return new DemoItem(
                "Color Tool",
                typeof(ColorTool),
                new[]
                {
                    DocumentationLink.WikiLink("Brush-Names", "Brushes"),
                    DocumentationLink.WikiLink("Custom-Palette-Hues", "Custom Palettes"),
                    DocumentationLink.WikiLink("Swatches-and-Recommended-Colors", "Swatches"),
                    DocumentationLink.DemoPageLink<ColorTool>("Demo View"),
                    DocumentationLink.DemoPageLink<ColorToolViewModel>("Demo View Model"),
                    DocumentationLink.ApiLink<PaletteHelper>()
                });
            
            yield return new DemoItem(
                "Color Tool",
                typeof(ColorTool),
                new[]
                {
                    DocumentationLink.WikiLink("Brush-Names", "Brushes"),
                    DocumentationLink.WikiLink("Custom-Palette-Hues", "Custom Palettes"),
                    DocumentationLink.WikiLink("Swatches-and-Recommended-Colors", "Swatches"),
                    DocumentationLink.DemoPageLink<ColorTool>("Demo View"),
                    DocumentationLink.DemoPageLink<ColorToolViewModel>("Demo View Model"),
                    DocumentationLink.ApiLink<PaletteHelper>()
                });
        }

        private bool DemoItemsFilter(object obj)
        {
            if (string.IsNullOrWhiteSpace(_searchKeyword))
            {
                return true;
            }

            return obj is DemoItem item
                   && item.Name.ToLower().Contains(_searchKeyword!.ToLower());
        }
    }
}