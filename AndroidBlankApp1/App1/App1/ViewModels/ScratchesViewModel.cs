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
    public class ScratchesViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private DbContext _dbContext;
        private ScratchViewModel selectedScratch;
        public INavigation Navigation { get; set;}
        
        public ObservableCollection<ScratchViewModel> Scratches { get; set; }
        public ICommand CreateScratchCommand { protected set; get; }
        public ICommand DeleteScratchCommand { protected set; get; }
        public ICommand SaveScratchCommand { protected set; get; }

        public ICommand SavePlanCommand { protected set; get; }

        public ICommand GoToHistoryCommand { get; set; }

        public ICommand GoToCurrentWeekCommand { get; set; }

        public ScratchesViewModel(DbContext dbContext, INavigation navigation)
        {
            _dbContext = dbContext;
            Navigation = navigation;
            
            var scratches = _dbContext.DailyPlans.GetItems()
                .Where(p => p.IsScratch)
                .Select(x => new ScratchViewModel(_dbContext, x) {ScratchesViewModel = this});

            Scratches = new ObservableCollection<ScratchViewModel>();
            
            scratches.ForEach(x => Scratches.Add(x));

            CreateScratchCommand = new Command(CreateScratch);
            DeleteScratchCommand = new Command(DeleteScratch);
            SaveScratchCommand = new Command(SaveScratch);
            SavePlanCommand = new Command(SavePlan);
            GoToCurrentWeekCommand = new Command(GoToCurrentWeek);
        }
        
        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        
        public ScratchViewModel SelectedScratch
        {
            get { return selectedScratch; }
            set
            {
                if (selectedScratch != value)
                {
                    var tempScratch = value;

                    selectedScratch = null;
                    OnPropertyChanged("selectedScratch");
                    Navigation.PushAsync(new ScratchPage(_dbContext, tempScratch));
                }
            }
        }
        
        private async void CreateScratch()
        {
            var plan = new ScratchViewModel(_dbContext, new DailyPlan()) {ScratchesViewModel = this};
            var scratchPage = new ScratchPage(_dbContext, plan);
            scratchPage.BindingContext = plan;
            await Navigation.PushAsync(scratchPage);
        }
        
        private void DeleteScratch(object scratchObject)
        {
            if (scratchObject is ScratchViewModel scratch)
            {
                Scratches.Remove(scratch);
                scratch.Tasks.ForEach(t => _dbContext.Tasks.DeleteItem(t.Task.Id));
                _dbContext.DailyPlans.DeleteItem(scratch.Plan.Id);
            }
            Back();
        }

        private void SavePlan(object scratchObject)
        {
            if (scratchObject is ScratchViewModel scratch)
            {
                if (Scratches.Contains(scratch))
                {
                    Scratches.Remove(scratch);   
                }
                
                scratch.Plan.IsScratch = false;
                _dbContext.DailyPlans.SaveItem(scratch.Plan);
            }
            Back();
        }

        private void Back()
        {
            Navigation.PopAsync();
        }
        
        private void SaveScratch(object scratchObject)
        {
            if (scratchObject is ScratchViewModel scratch)
            {
                var id = _dbContext.DailyPlans.SaveItem(scratch.Plan);

                if (Scratches.All(s => s.Plan.Id != id))
                {
                    Scratches.Add(scratch);    
                }

                scratch.Tasks.ForEach(t =>
                {
                    t.Task.PlanId = id;
                    _dbContext.Tasks.SaveItem(t.Task);
                });
                
            }
            Back();
        }
        
        private void GoToCurrentWeek()
        {
            Navigation.PushAsync(new CurrentWeekPage(_dbContext));
        }

        private void GoToHistory()
        {
            //Navigation.PushAsync(new HistoryPage(_dbContext));
        }
    }
}