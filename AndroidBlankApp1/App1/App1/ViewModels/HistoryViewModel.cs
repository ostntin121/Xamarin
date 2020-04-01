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
    public class CurrentWeekViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private DbContext _dbContext;
        private DailyPlanViewModel selectedPlan;
        
        public INavigation Navigation { get; set;}
        public ObservableCollection<DailyPlanViewModel> Plans { get; set; }
        public ICommand BackCommand { protected set; get; }
        public ICommand GoToScratchesCommand { protected set; get; }

        public ICommand GoToHistoryCommand { get; set; }
        public ICommand SavePlanCommand { get; set; }

        public CurrentWeekViewModel(DbContext dbContext, INavigation navigation)
        {
            _dbContext = dbContext;
            Navigation = navigation;

            var dailyPlans = _dbContext.DailyPlans.GetItems()
                .Where(p => !p.IsScratch && !p.IsExpired)
                .Select(x => new DailyPlanViewModel(_dbContext, x) {WeekViewModel = this});

            Plans = new ObservableCollection<DailyPlanViewModel>();
            
            dailyPlans.ForEach(d => Plans.Add(d));

            BackCommand = new Command(Back);
            GoToScratchesCommand = new Command(GoToScratches);
            GoToHistoryCommand = new Command(GoToHistory);
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

        private void GoToScratches()
        {
            Navigation.PushAsync(new ScratchesPage(_dbContext));
        }
        
        private void GoToHistory()
        {
            //Navigation.PushAsync(new HistoryPage(_dbContext));
        }

        private void SavePlan(object planObject)
        {
            if (planObject is DailyPlanViewModel plan)
            {
                _dbContext.DailyPlans.SaveItem(plan.Plan);
            }
        }
    }
}