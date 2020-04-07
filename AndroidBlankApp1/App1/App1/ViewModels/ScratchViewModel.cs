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
    public class ScratchViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ScratchesViewModel _scratchesViewModel;
        private DbContext _dbContext;

        public bool Tapped { get; set; }

        public MediaSource _audioPath;
        public bool _canSave;
        public bool _canSaveAsPath;

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

            AudioPath = MediaSource.FromUri(new Uri(
                "https://sec.ch9.ms/ch9/5d93/a1eab4bf-3288-4faf-81c4-294402a85d93/XamarinShow_mid.mp4"));

            var tasks = _dbContext.Tasks.GetItems()
                .Where(x => x.PlanId == Plan.Id)
                .Select((x, i) => new TaskViewModel(x, i, null, null));

            tasks.ForEach(t => Tasks.Add(t));
            
            CanSave = !String.IsNullOrEmpty(Name) && Tasks.Count > 0;
            CanSaveAsPlan = CanSave && Plan.Date > DateTime.Now;
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

        public MediaSource AudioPath
        {
            get { return _audioPath; }
            set
            {
                _audioPath = value;
                OnPropertyChanged("AudioPath");
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

        public bool CanSave
        {
            get
            {
                return _canSave && !String.IsNullOrEmpty(Name) && Tasks.Count > 0;
            }
            set
            {
                _canSave = value;
                OnPropertyChanged("CanSave");
            }
        }
        public bool CanSaveAsPlan
        {
            get
            {
                return _canSaveAsPath && CanSave && Plan.Date > DateTime.Now;
            }
            set
            {
                _canSaveAsPath = value;
                OnPropertyChanged("CanSaveAsPlan");
            }
        }

        public void AddTask()
        {
            Tasks.Add(new TaskViewModel(new Task(), Tasks.Count, null, this));
        }
    }
}