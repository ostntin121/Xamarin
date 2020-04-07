using System;
using Android.Media;
using System.Threading.Tasks;
using Android.Content;
using App1.Android;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidRecorder))]
namespace App1.Android
{
    class AndroidRecorder : IRecordable
    {
        AudioManager.IOnAudioFocusChangeListener listener;
        MediaRecorder recorder = null;
        AudioManager audioManager = null;
        string filePath = null;

        public void StartRecorder()
        {
            try
            {
                var file = new Java.IO.File(filePath);
                file.CreateNewFile();

                if (recorder == null)
                    recorder = new MediaRecorder (); // Initial state.
                else
                    recorder.Reset ();

                recorder.SetAudioSource (AudioSource.Mic);
                recorder.SetOutputFormat (OutputFormat.Default);
                recorder.SetAudioEncoder (AudioEncoder.Default); // Initialized state.
                recorder.SetOutputFile (filePath); // DataSourceConfigured state.
                recorder.Prepare (); // Prepared state
                recorder.Start (); // Recording state.
 
            } catch (Exception ex) {
                Console.Out.WriteLine (ex.StackTrace);
            }
        }

        public void StopRecorder ()
        {
            if (recorder != null) {
                recorder.Stop ();
                recorder.Release ();
                recorder = null;
            }
        }

        public Task StartAsync()
        {
            StartRecorder();

            var tcs = new TaskCompletionSource<object> ();
            tcs.SetResult (null);
            return tcs.Task;
        }

        public void Stop ()
        {
            StopRecorder ();
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