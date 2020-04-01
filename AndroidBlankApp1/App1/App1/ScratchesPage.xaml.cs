using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App1.Data;
using App1.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Task = App1.Models.Task;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScratchesPage : ContentPage
    {
        public DbContext _dbContext;
        private ViewCell lastCell;
        public ScratchesPage(DbContext dbContext, object currentWeekViewModel)
        {
            _dbContext = dbContext;
            InitializeComponent();
            BindingContext = new ScratchesViewModel(_dbContext, Navigation, currentWeekViewModel as CurrentWeekViewModel);
        }
        
        private void Tasks_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var listView = (ListView) sender;
            listView.SelectedItem = null;
        }

        private async void Button_OnPressed(object sender, EventArgs e)
        {
            var button = (Button) sender;
            await button.ScaleTo(1.1, 100, Easing.Linear);
            await button.ScaleTo(1, 100, Easing.Linear);
        }
    }
}