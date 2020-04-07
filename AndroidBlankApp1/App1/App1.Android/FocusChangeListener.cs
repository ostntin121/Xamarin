using Android.App;
using Android.Media;
using Xamarin.Forms;

namespace App1.Android
{ 
    class FocusChangeListener : Java.Lang.Object, AudioManager.IOnAudioFocusChangeListener
    {
        public IAudio parent = null;

        public FocusChangeListener (IAudio parent)
        {
            this.parent = parent;
        }

        public void OnAudioFocusChange (AudioFocus focusChange)
        {    
            switch (focusChange) {
                // We will take any flavor of AudioFocusgain that the system gives us and use it.
                case AudioFocus.GainTransient:
                case AudioFocus.GainTransientMayDuck:
                case AudioFocus.Gain:
                    parent.StartAsync();
                    break;
                // If we get any notificationthat removes focus - just terminate what we were doing.
                case AudioFocus.LossTransientCanDuck:          
                case AudioFocus.LossTransient:
                case AudioFocus.Loss:
                    parent.Stop ();
                    break;
                default:
                    break;
            }
        }
    }
}