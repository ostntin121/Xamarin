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
    public class ScratchViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ScratchesViewModel _scratchesViewModel;
        private DbContext _dbContext;
        
        public bool Tapped { get; set; }

        public ObservableCollection<TaskViewModel> Tasks { get; set; }

        public DailyPlan Plan { get; set; }

        public ICommand AddTaskCommand { get; set; }

        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public ScratchViewModel(DbContext dbContext, DailyPlan plan)
        {
            _dbContext = dbContext;
            Plan = plan;
            Tasks = new ObservableCollection<TaskViewModel>();
            
            AddTaskCommand = new Command(AddTask);
            
            var tasks = _dbContext.Tasks.GetItems()
                .Where(x => x.PlanId == Plan.Id)
                .Select((x, i) => new TaskViewModel(x, i));

            tasks.ForEach(t => Tasks.Add(t));
        }

        public ScratchesViewModel ScratchesViewModel
        {
            get { return _scratchesViewModel; }
            set
            {
                if (_scratchesViewModel != value)
                {
                    _scratchesViewModel = value;
                    OnPropertyChanged("ScratchesViewModel");
                }
            }
        }
        
        public string Name
        {
            get { return Plan.Name; }
            set
            {
                if (Plan.Name != value)
                {
                    Plan.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        
        public DateTime Date
        {
            get { return Plan.Date; }
            set
            {
                if (Plan.Date != value)
                {
                    Plan.Date = value;
                    OnPropertyChanged("Date");
                }
            }
        }
        
        public bool CanSave => Tasks.Count > 0;
        public bool CanSaveAsPlan => CanSave && Plan.Date > DateTime.Now;

        public void AddTask()
        {
            Tasks.ForEach(t => t.Tapped = false);
            Tasks.Add(new TaskViewModel(new Task { Name = "Новая задача" }, Tasks.Count)
            {
                Tapped = true
            });
        }

    }
}