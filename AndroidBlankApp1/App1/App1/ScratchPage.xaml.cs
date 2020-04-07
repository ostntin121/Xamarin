using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App1.Data;
using App1.ViewModels;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScratchPage : ContentPage
    {
        public DbContext _dbContext;
        public ScratchViewModel ViewModel { get; set; }

        public ScratchPage(DbContext dbContext, ScratchViewModel scratch)
        {
            _dbContext = dbContext;
            InitializeComponent();
            
            ViewModel = scratch;
            this.BindingContext = ViewModel;
        }
        
        private void Tasks_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }
        
        void OnPlayPauseButtonClicked(object sender, EventArgs args)
        {
            if (mediaElement.CurrentState == MediaElementState.Stopped ||
                mediaElement.CurrentState == MediaElementState.Paused)
            {
                mediaElement.Play();
            }
            else if (mediaElement.CurrentState == MediaElementState.Playing)
            {
                mediaElement.Pause();
            }
        }

        void OnStopButtonClicked(object sender, EventArgs args)
        {
            mediaElement.Stop();
        }
    }
}