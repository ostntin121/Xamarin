using System;
using System.Linq;
using App1.Data;
using App1.Models;
using App1.ViewModels;
using Xamarin.Forms;

namespace App1
{
    public partial class CurrentWeekPage : ContentPage
    {
        public DbContext _dbContext;
        public CurrentWeekPage(DbContext dbContext)
        {
            _dbContext = dbContext;
            InitializeComponent();

            BindingContext = new CurrentWeekViewModel(_dbContext, Navigation);
        }

        private void Tasks_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }
    }
}