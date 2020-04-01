using System;
using System.ComponentModel;
using App1.Models;

namespace App1.ViewModels
{
    public class TaskViewModel: INotifyPropertyChanged
    {
        public Task Task { get; set; }

        public DailyPlanViewModel dailyPlanViewModel;

        public int Position { get; set; }

        public string Number => $"{Position + 1}.";

        public bool IsCompleted
        {
            get { return Task.IsCompleted; }
            set
            {
                if (Task.IsCompleted != value)
                {
                    Task.IsCompleted = value;
                    
                    if (dailyPlanViewModel != null)
                    {
                        dailyPlanViewModel.UpdateCompletedTasks();
                    }
                    
                    OnPropertyChanged("IsCompleted");
                }
            }
        }
        
        public string Name
        {
            get { return Task.Name; }
            set
            {
                if (Task.Name != value)
                {
                    Task.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public TaskViewModel(Task task, int position, DailyPlanViewModel viewModel)
        {
            Task = task;
            Position = position;
            dailyPlanViewModel = viewModel;
        }
        
        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public void UpdateView()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Tapped"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}