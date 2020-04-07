using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using App1.Models;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace App1.ViewModels
{
    public class TaskViewModel: INotifyPropertyChanged
    {
        public Task Task { get; set; }

        public DailyPlanViewModel dailyPlanViewModel;
        public ScratchViewModel scratchViewModel;

        public int Position { get; set; }

        public string Number => $"{Position + 1}.";

        private bool _hasImage;
        private bool _hasAudio;

        private IRecordable _recorder;
        private IPlayable _player;
        private string _audioPath;

        private bool _isRecording;
        private bool _isPlaying;

        public ICommand CreatePhotoCommand { get; set; }
        public ICommand ChooseImageCommand { get; set; }

        public ICommand RecordAudioCommand { get; set; }
        public ICommand StopRecordAudioCommand { get; set; }

        public ICommand PlayAudioCommand { get; set; }
        public ICommand StopPlayAudioCommand { get; set; }

        public ICommand LoadAudioCommand { get; set; }

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

        public ImageSource ImagePath
        {
            get { return Task.ImagePath; }
            set
            {
                var imagePath = ((FileImageSource) value).File;
                
                if (Task.ImagePath != imagePath)
                {
                    Task.ImagePath = imagePath;
                    HasImage = !String.IsNullOrEmpty(Task.ImagePath);
                    OnPropertyChanged("ImagePath");
                }
            }
        }
        
        public MediaSource AudioPath
        {
            get => String.IsNullOrEmpty(Task.AudioPath)
                    ? MediaSource.FromUri(new Uri(
                        "https://sec.ch9.ms/ch9/5d93/a1eab4bf-3288-4faf-81c4-294402a85d93/XamarinShow_mid.mp4"))
                    : MediaSource.FromFile(Task.AudioPath);
            set
            {
                var filePath = ((FileMediaSource)value)?.File;
                
                if (Task.AudioPath != filePath)
                {
                    Task.AudioPath = filePath;
                    HasAudio = !String.IsNullOrEmpty(Task.AudioPath);
                    OnPropertyChanged("AudioPath");
                }
            }
        }

        public bool HasImage
        {
            get { return _hasImage;  }
            set
            {
                _hasImage = value;
                OnPropertyChanged("HasImage");
            }
        }
        
        public bool HasAudio
        {
            get { return _hasAudio;  }
            set
            {
                _hasAudio = value;
                OnPropertyChanged("HasAudio");
            }
        }

        public bool CanRecordAudio
        {
            get { return !_isRecording && !_isPlaying; }
            set
            {
                _isRecording = !value;
                OnPropertyChanged("CanRecordAudio");
                OnPropertyChanged("CanStopRecording");
            }
        }

        public bool CanPlayAudio
        {
            get { return !_isPlaying && !_isRecording; }
            set
            {
                _isPlaying = !value;
                OnPropertyChanged("CanPlayAudio");
                OnPropertyChanged("CanStopPlaying");
            }
        }

        public bool CanStopRecording =>_isRecording;
        public bool CanStopPlaying => _isPlaying;

        public TaskViewModel(Task task, int position, DailyPlanViewModel viewModel, ScratchViewModel viewModel2)
        {
            Task = task;
            Position = position;
            dailyPlanViewModel = viewModel;
            scratchViewModel = viewModel2;
            
            CreatePhotoCommand = new Command(CreatePhoto);
            ChooseImageCommand = new Command(ChooseImage);
            
            RecordAudioCommand = new Command(RecordAudio);
            PlayAudioCommand = new Command(PlayAudio);
            
            StopRecordAudioCommand = new Command(StopRecordAudio);
            StopPlayAudioCommand = new Command(StopPlayAudio);
            
            LoadAudioCommand = new Command(LoadAudio);
            
            HasImage = !String.IsNullOrEmpty(Task.ImagePath);
            HasAudio = !String.IsNullOrEmpty(Task.AudioPath);
            
            _recorder = DependencyService.Get<IRecordable>();
            _player = DependencyService.Get<IPlayable>();
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

        public async void ChooseImage()
        {
            if (CrossMedia.Current.IsPickPhotoSupported)
            {
                var photo = await CrossMedia.Current.PickPhotoAsync();

                if (photo == null)
                    return;
                
                ImagePath = ImageSource.FromFile(photo.Path);
            }
        }

        public async void CreatePhoto()
        {
            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    SaveToAlbum = true,
                    Directory = "Sample",
                    Name = $"{Guid.NewGuid()}.jpg"
                });
 
                if (file == null)
                    return;
 
                ImagePath = ImageSource.FromFile(file.Path);
            }
        }

        public async void RecordAudio()
        {
            if (!CanRecordAudio)
                return;

            _audioPath = Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, $"Audio/{Guid.NewGuid()}.ogg")
                .Replace("data/user/0", "storage/self/primary/Android/data");

            if (_recorder.RequestAudioResources(_audioPath))
            {
                await _recorder.StartAsync();
                CanRecordAudio = false;
            }
        }

        public void StopRecordAudio()
        {
            if (CanRecordAudio)
                return;
            
            _recorder.Stop();
            _recorder.ReleaseAudioResources();

            AudioPath = new FileMediaSource() {File = _audioPath};
            CanRecordAudio = true;
        }

        public async void PlayAudio()
        {
            if (!HasAudio || !CanPlayAudio)
                return;

            if (_player.RequestAudioResources(Task.AudioPath))
            {
                await _player.StartAsync();
                CanPlayAudio = false;    
            }
        }

        public void StopPlayAudio()
        {
            if (CanPlayAudio)
                return;
            
            _player.Stop();
            _player.ReleaseAudioResources();
            
            CanPlayAudio = true;
        }

        public void LoadAudio()
        {
            if (scratchViewModel != null)
            {
                scratchViewModel.AudioPath = AudioPath;
            }
            
            if (dailyPlanViewModel != null)
            {
                
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}