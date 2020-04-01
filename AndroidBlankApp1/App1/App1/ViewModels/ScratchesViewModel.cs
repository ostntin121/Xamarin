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
        private CurrentWeekViewModel _currentWeekViewModel;
        public INavigation Navigation { get; set;}
        
        public ObservableCollection<ScratchViewModel> Scratches { get; set; }
        public ICommand CreateScratchCommand { protected set; get; }
        public ICommand DeleteScratchCommand { protected set; get; }
        public ICommand SaveScratchCommand { protected set; get; }
        public ICommand SavePlanCommand { protected set; get; }

        public ScratchesViewModel(DbContext dbContext, INavigation navigation, CurrentWeekViewModel currentWeekViewModel)
        {
            _dbContext = dbContext;
            _currentWeekViewModel = currentWeekViewModel;
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
                scratch.Plan.IsScratch = false;
                _dbContext.DailyPlans.SaveItem(scratch.Plan);
                
                scratch.Tasks.ForEach(t =>
                {
                    t.Task.PlanId = scratch.Plan.Id;
                    _dbContext.Tasks.SaveItem(t.Task);
                });

                if (_currentWeekViewModel.Plans.All(p => p.Plan.Id != scratch.Plan.Id))
                {
                    var viewModel = new DailyPlanViewModel(_dbContext, scratch.Plan, _currentWeekViewModel);
                    _currentWeekViewModel.Plans.Add(viewModel);
                }
                
                if (Scratches.Contains(scratch))
                {
                    Scratches.Remove(scratch);
                }
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

                if (!Scratches.Contains(scratch))
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
    }
}