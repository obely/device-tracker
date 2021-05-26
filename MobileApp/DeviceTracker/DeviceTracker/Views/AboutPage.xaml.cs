using DeviceTracker.ViewModels;
using Xamarin.Forms;

namespace DeviceTracker.Views
{
    public partial class AboutPage : ContentPage
    {
        private readonly AboutViewModel _viewModel;

        public AboutPage()
        {
            InitializeComponent();
            _viewModel = (AboutViewModel)BindingContext;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}