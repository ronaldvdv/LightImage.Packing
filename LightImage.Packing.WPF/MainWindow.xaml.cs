using ReactiveUI;
using System.ComponentModel;
using System.Windows;

namespace LightImage.Packing.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MainWindowBase
    {
        public MainWindow()
        {
            ViewModel = new ViewModel();
            ViewModel.RunWeighted.Execute(null);
            this.WhenActivated(d =>
            {
                d(this.OneWayBind(ViewModel, vm => vm.Packing.Bins, v => v.BinTabs.ItemsSource));
                d(this.Bind(ViewModel, vm => vm.Width, v => v.WidthTextBox.Text));
                d(this.Bind(ViewModel, vm => vm.Height, v => v.HeightTextBox.Text));
                d(this.Bind(ViewModel, vm => vm.Count, v => v.CountTextBox.Text));
                d(this.Bind(ViewModel, vm => vm.Options.Padding, v => v.PaddingTextBox.Text));
                d(this.Bind(ViewModel, vm => vm.Options.Border, v => v.BorderTextBox.Text));
                d(this.Bind(ViewModel, vm => vm.Options.Smart, v => v.SmartCheckBox.IsChecked));
                d(this.Bind(ViewModel, vm => vm.Options.Square, v => v.SquareCheckBox.IsChecked));
                d(this.Bind(ViewModel, vm => vm.Options.PowerOfTwo, v => v.PowerOfTwoCheckBox.IsChecked));
                d(this.OneWayBind(ViewModel, vm => vm.Packing.Width, v => v.BinWidthTextBox.Text));
                d(this.OneWayBind(ViewModel, vm => vm.Packing.Height, v => v.BinHeightTextBox.Text));
                d(this.OneWayBind(ViewModel, vm => vm.Options.Smart, v => v.SquareCheckBox.IsEnabled));
                d(this.OneWayBind(ViewModel, vm => vm.Options.Smart, v => v.PowerOfTwoCheckBox.IsEnabled));
                d(this.Bind(ViewModel, vm => vm.SelectedBin, v => v.BinTabs.SelectedItem));
                d(this.BindCommand(ViewModel, vm => vm.RunWeighted, v => v.RunWeightedButton));
                d(this.BindCommand(ViewModel, vm => vm.RunUnweighted, v => v.RunUnweightedButton));
            });
            InitializeComponent();
        }

        private void FitButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Width = MainPanel.ActualWidth;
            ViewModel.Height = MainPanel.ActualHeight;
        }
    }

    public abstract class MainWindowBase : ReactiveWindow<ViewModel>
    {
    }
}