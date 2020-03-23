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
    public partial class ScratchesPage : ContentPage
    {
        public DbContext _dbContext;
        public ScratchesPage(DbContext dbContext)
        {
            _dbContext = dbContext;
            InitializeComponent();
            BindingContext = new ScratchesViewModel(_dbContext, Navigation);
        }
    }
}