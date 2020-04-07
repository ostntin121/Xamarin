using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using App1.Data;
using App1.Models;
using Xamarin.Forms.Internals;
using Task = App1.Models.Task;

namespace App1.ViewModels
{
    public class DailyPlanViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private CurrentWeekViewModel _weekViewModel;
        private int _completedTasks;
        private string _status;
        private bool _isSuccessful;
        private bool _highlight;
        
        private DbContext _dbContext;

        public ObservableCollection<TaskViewModel> Tasks { get; set; }

        public DailyPlan Plan { get; set; }

        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public DailyPlanViewModel(DbContext dbContext, DailyPlan plan, CurrentWeekViewModel weekViewModel)
        {
            _dbContext = dbContext;
            WeekViewModel = weekViewModel;
            Plan = plan;
            
            Tasks = new ObservableCollection<TaskViewModel>();
            
            var tasks = _dbContext.Tasks.GetItems()
                .Where(x => x.PlanId == Plan.Id)
                .Select((x, i) => new TaskViewModel(x, i, this, null));

            tasks.ForEach(t => Tasks.Add(t));

            AllTasks = Tasks.Count;
            CompletedTasks = Tasks.Count(t => t.IsCompleted);
            Status = GetStatus();
            IsSuccessful = GetSuccess();
            Highlight = Date == DateTime.Now.ToShortDateString();
        }

        public CurrentWeekViewModel WeekViewModel
        {
            get { return _weekViewModel; }
            set
            {
                if (_weekViewModel != value)
                {
                    _weekViewModel = value;
                    OnPropertyChanged("WeekViewModel");
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
        
        public string Date
        {
            get { return Plan.Date.ToShortDateString(); }
            set
            {
                if (Plan.Date != DateTime.Parse(value))
                {
                    Plan.Date = DateTime.Parse(value);
                    OnPropertyChanged("Date");
                }
            }
        }

        public int CompletedTasks
        {
            get { return _completedTasks; }
            set
            {
                _completedTasks = value; 
                OnPropertyChanged("CompletedTasks");
            }
        }

        private int _allTasks;
        
        public int AllTasks
        {
            get => _allTasks;
            set => _allTasks = value;
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }
        
        public string GetStatus()
        {
            return $"Выполнено {CompletedTasks} из {AllTasks} задач";
        }

        public bool IsSuccessful
        {
            get { return _isSuccessful; }
            set
            {
                _isSuccessful = value;
                OnPropertyChanged("IsSuccessful");
            }
        }

        public bool Highlight
        {
            get { return _highlight; }
            set
            {
                _highlight = value && Date == DateTime.Now.ToShortDateString();
                OnPropertyChanged("Highlight");
            }
        }

        public bool GetSuccess()
        {
            return CompletedTasks == AllTasks;
        }
        
        public string Title => $"План на {Date}";

        public bool IsNotExpired => !Plan.IsExpired;
        public void UpdateCompletedTasks()
        {
            CompletedTasks = Tasks.Count(t => t.IsCompleted);
            Status = GetStatus();
            IsSuccessful = GetSuccess();
        }
    }
}