using LagoVista.ContentManager.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LagoVista.ContentManager.App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        MainViewModel _viewModel;
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        public ObservableCollection<Models.TextResource> TextResources { get; set; }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var settings = Windows.Storage.ApplicationData.Current.RoamingSettings;

            if (settings.Values.ContainsKey("storage_name") && settings.Values.ContainsKey("storage_key"))
            {
                _viewModel.StorageName = settings.Values["storage_name"].ToString();
                _viewModel.StorageKey = settings.Values["storage_key"].ToString();

                try
                {
                    await _viewModel.PopulateResourcesAsync();
                }
                catch(Exception)
                {
                    var dialog = new MessageDialog("There was an error connecting to your storage account, please check your settings and try again.");
                    await dialog.ShowAsync();
                }
            }
            else
            {
                var dialog = new MessageDialog("Please configure your Azure Storage Account Name and Access Key");
                await dialog.ShowAsync();
            }

        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }
    }
}
