using System;
using System.Collections.ObjectModel;
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
        private DbContext _dbContext;

        public ObservableCollection<TaskViewModel> Tasks { get; set; }

        public DailyPlan Plan { get; set; }

        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public DailyPlanViewModel(DbContext dbContext, DailyPlan plan)
        {
            _dbContext = dbContext;
            Plan = plan;
            
            Tasks = new ObservableCollection<TaskViewModel>();
            
            var tasks = _dbContext.Tasks.GetItems()
                .Where(x => x.PlanId == Plan.Id)
                .Select((x, i) => new TaskViewModel(x, i));

            tasks.ForEach(t => Tasks.Add(t));
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
            get
            {
                return Tasks.Count(x => x.IsCompleted);
            }
        }
        
        public int AllTasks
        {
            get
            {
                return Tasks.Count;
            }
        }

        public string Status => $"Выполнено {CompletedTasks} из {AllTasks} задач";
        public string Title => $"План на {Date}";
    }
}