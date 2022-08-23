using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CoreClass.Model;
using CoreClass.Service;
using EyeOfSauron.ViewModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Windows.Threading;
using CoreClass;
using MaterialDesignThemes.Wpf;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace EyeOfSauron.MyUserControl
{
    /// <summary>
    /// Interaction logic for PanelIdInput.xaml
    /// </summary>
    public partial class InformationView : UserControl
    {
        //public readonly InformationViewModel viewModel;

        public InformationView()
        {
            InitializeComponent();
            //viewModel = new();
            //DataContext = viewModel;
        }
    }
}
