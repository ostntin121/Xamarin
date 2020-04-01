using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using App1.Data;
using App1.Models;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Task = System.Threading.Tasks.Task;

namespace App1.ViewModels
{
    public class CurrentWeekViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private DbContext _dbContext;
        private DailyPlanViewModel selectedPlan;
        private bool _highlight;

        public INavigation Navigation { get; set;}
        public ObservableCollection<DailyPlanViewModel> Plans { get; set; }
        public ICommand SavePlanCommand { get; set; }

        public CurrentWeekViewModel(DbContext dbContext, INavigation navigation)
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
                .Where(p => !p.IsScratch && !p.IsExpired)
                .OrderBy(p => p.Date)
                .Select(x => new DailyPlanViewModel(_dbContext, x,this));

            Plans = new ObservableCollection<DailyPlanViewModel>();
            
            dailyPlans.ForEach(d => Plans.Add(d));

            if (!Application.Current.Properties.ContainsKey("highlight"))
            {
                Application.Current.Properties["highlight"] = false;
                Highlight = false;
            }
            else
            {
                Highlight = (bool) Application.Current.Properties["highlight"];
            }

            SavePlanCommand = new Command(SavePlan);
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
        
        private void SavePlan(object planObject)
        {
            if (planObject is DailyPlanViewModel plan)
            {
                _dbContext.DailyPlans.SaveItem(plan.Plan);
                plan.Tasks.ForEach(task => _dbContext.Tasks.SaveItem(task.Task));
            }
            Back();
        }

        public bool Highlight
        {
            get { return _highlight; }
            set
            {
                _highlight = value;
                Application.Current.Properties["highlight"] = value;
                OnPropertyChanged("Highlight");
                Plans?.ForEach(p => { p.Highlight = value; });
            }
        }
    }
}