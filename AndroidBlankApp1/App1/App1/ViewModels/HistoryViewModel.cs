using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using App1.Data;
using App1.Models;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace App1.ViewModels
{
    public class HistoryViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private DbContext _dbContext;
        private DailyPlanViewModel selectedPlan;
        
        public INavigation Navigation { get; set;}
        public ObservableCollection<DailyPlanViewModel> Plans { get; set; }
        public HistoryViewModel(DbContext dbContext, INavigation navigation)
        {
            _dbContext = dbContext;
            Navigation = navigation;
            
            var now = DateTime.Now;

            var expiredPlans = _dbContext.DailyPlans.GetItems()
                .Where(p => 
                    !p.IsExpired && p.Date <= now && 
                    p.Date.StartOfWeek() < now.StartOfWeek())
                .ToList();

            foreach (var plan in expiredPlans)
            {
                plan.IsExpired = true;
                _dbContext.DailyPlans.SaveItem(plan);
            }

            var dailyPlans = _dbContext.DailyPlans.GetItems()
                .Where(p => !p.IsScratch && p.IsExpired)
                .OrderBy(p => p.Date)
                .Select(x => new DailyPlanViewModel(_dbContext, x, null));

            Plans = new ObservableCollection<DailyPlanViewModel>();
            
            dailyPlans.ForEach(d => Plans.Add(d));
        }
        
        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        
        public DailyPlanViewModel SelectedPlan
        {
            get { return selectedPlan; }
            set
            {
                if (selectedPlan != value)
                {
                    var tempPlan = value;
                    selectedPlan = null;
                    OnPropertyChanged("SelectedPlan");
                    Navigation.PushAsync(new DailyPlanPage(_dbContext, tempPlan));
                }
            }
        }

        private void Back()
        {
            Navigation.PopAsync();
        }
    }
}