using System;
using System.Threading.Tasks;
using Android.Media;
using App1.Android;
using Xamarin.Forms;
using Java.IO;
using Console = System.Console;

[assembly: Dependency(typeof(AndroidPlayer))]
namespace App1.Android
{
    class AndroidPlayer : IPlayable
    {
        AudioManager.IOnAudioFocusChangeListener listener;
        AudioManager audioManager = null;
        MediaPlayer player = null;
        string filePath = null;

        public async Task StartPlayerAsync ()
        {
            try {
                if (player == null) {
                    player = new MediaPlayer ();
                } else {
                    player.Reset ();
                }

                // This method works better than setting the file path in SetDataSource. Don't know why.
                var file = new File (filePath);
                var fis = new FileInputStream (file);
                await player.SetDataSourceAsync (fis.FD);
 
                //player.SetDataSource(filePath);
                player.Prepare ();
                player.Start ();
            } catch (Exception ex) {
                Console.Out.WriteLine (ex.StackTrace);
            }
        }

        public void StopPlayer ()
        {
            if ((player != null)) {
                if (player.IsPlaying) {
                    player.Stop ();
                }
                player.Release ();
                player = null;
            }
        }

        public async Task StartAsync()
        {
            await StartPlayerAsync();
        }

        public void Stop ()
        {
            this.StopPlayer ();
        }
        
        public bool RequestAudioResources(string path)
        {
            filePath = path;
            listener = new FocusChangeListener(this);
            audioManager = Audio.AudioManager;

            var ret = audioManager.RequestAudioFocus(listener, Stream.Music, AudioFocus.Gain);
            if (ret == AudioFocusRequest.Granted) {
                return true;
            }

            if (ret == AudioFocusRequest.Failed) {
                return false;
            }
            
            return false;
        }
        
        public void ReleaseAudioResources()
        {
            if (listener != null)
                audioManager.AbandonAudioFocus(listener);
        }
        
    }
}