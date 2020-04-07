using System.Threading.Tasks;

namespace App1
{
    public interface IAudio
    {
        bool RequestAudioResources(string filePath);
        void ReleaseAudioResources();
        
        Task StartAsync();
        void Stop();
    }
    public interface IRecordable: IAudio
    {
        void StartRecorder();
        void StopRecorder();
    }

    public interface IPlayable: IAudio
    {
        Task StartPlayerAsync();
        void StopPlayer();
    }
}