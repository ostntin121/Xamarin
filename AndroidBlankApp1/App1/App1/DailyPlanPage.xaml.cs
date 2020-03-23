﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using App1.Data;
 using App1.Models;
 using App1.ViewModels;
 using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DailyPlanPage : ContentPage
    {
        public DbContext _dbContext;
        public DailyPlanViewModel ViewModel { get; set; }

        public DailyPlanPage(DbContext dbContext, DailyPlanViewModel dailyPlan)
        {
            _dbContext = dbContext;
            InitializeComponent();
            ViewModel = dailyPlan;
            this.BindingContext = ViewModel;
        }
        
        private void Tasks_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }
    }
}