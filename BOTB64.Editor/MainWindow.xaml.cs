using System.Windows;
using BOTB64.Editor.ViewModels;

namespace BOTB64.Editor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is not MainViewModel vm)
                return;

            DbPanel.OpenScriptRequested += (kind, path) =>
            {
                vm.OpenOrCreate(kind, path);
                MainTabs.SelectedIndex = 0; // jump back to the Editor tab
            };

            if (string.IsNullOrEmpty(vm.Database.DataRoot))
                vm.ChooseDataRootCommand.Execute(null);
        }
    }
}
