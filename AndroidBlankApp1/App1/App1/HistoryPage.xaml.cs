using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App1.Data;
using App1.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryPage : ContentPage
    {
        public DbContext _dbContext;

        public HistoryPage(DbContext dbContext)
        {
            _dbContext = dbContext;
            InitializeComponent();
            BindingContext = new HistoryViewModel(_dbContext, Navigation);
        }

        private void Tasks_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView) sender).SelectedItem = null;
        }
    }
}