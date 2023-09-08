using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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

namespace LazyApiPack.XamlPreview
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ResourceManager _manager;
        IEnumerator<string> _enumerator;
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                _manager = new ResourceManager(Environment.GetCommandLineArgs()[1]);
            }
        }


        RelayCommand _nextCommand;
        int x = 0;
        public RelayCommand NextCommand { get => _nextCommand ??= new RelayCommand(OnNextCommand_Execute); }
        protected void OnNextCommand_Execute(object? parameter)
        {
            if (x == 0)
            {
                CurrentResource = (UIElement)App.Current.Resources["Material.Design.Icons.Filled.10k"];
                x++;
            } else if (x == 1)
            {
                CurrentResource = (UIElement)App.Current.Resources["Material.Design.Icons.Filled.10mp"];
                x--;
            }
            return;
            if(_enumerator != null && _enumerator.MoveNext())
            {
                if (_manager.GetResource(_enumerator.Current) is UIElement ui)
                {
                    CurrentResource = (UIElement)_manager.GetResource(_enumerator.Current);
                    CurrentName = _enumerator.Current;
                } else
                {
                    CurrentResource= null;
                    CurrentName = "Not a valid resource.";
                }
            } else
            {
                _enumerator = _manager.ResourceKeys.GetEnumerator();
            }
        }
     



        public static readonly DependencyProperty CurrentResourceProperty = DependencyProperty.Register(nameof(CurrentResource), typeof(UIElement), typeof(MainWindow), new PropertyMetadata(null));
        public UIElement CurrentResource
        {
            get { return (UIElement)GetValue(CurrentResourceProperty); }
            set { SetValue(CurrentResourceProperty, value); }
        }


        public static readonly DependencyProperty CurrentNameProperty = DependencyProperty.Register(nameof(CurrentName), typeof(string), typeof(MainWindow), new PropertyMetadata(null));
        public string CurrentName
        {
            get { return (string)GetValue(CurrentNameProperty); }
            set { SetValue(CurrentNameProperty, value); }
        }


    }
}
