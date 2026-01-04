using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioGame.src._Core
{
    public class SoundManager
    {
        private static SoundManager _instance;
        public static SoundManager Instance => _instance ??= new SoundManager();

        private Dictionary<string, Song> _songs;
        private Dictionary<string, SoundEffect> _sounds;
        private float _musicVolume = 0.8f;

        public bool IsMuted { get; private set; } = false;
        public float MusicVolume => _musicVolume;

        private SoundManager()
        {
            _songs = new Dictionary<string, Song>();
            _sounds = new Dictionary<string, SoundEffect>();
        }

        // Load music
        public void LoadSong(string name, string assetName)
        {
            if (_songs.ContainsKey(name)) return;

            var content = GameManager.Instance.Content;

            try
            {
                var song = content.Load<Song>(assetName);
                _songs.Add(name, song);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi load nhạc: {ex.Message}");
            }
        }

        public void PlayMusic(string name, bool isLooping = true)
        {
            if (IsMuted) return;

            if (_songs.ContainsKey(name))
            {
                MediaPlayer.IsRepeating = isLooping;
                MediaPlayer.Play(_songs[name]);
                MediaPlayer.Volume = _musicVolume;
            }
        }

        public void StopMusic()
        {
            MediaPlayer.Stop();
        }

        public void ToggleMute()
        {
            IsMuted = !IsMuted;
            MediaPlayer.IsMuted = IsMuted;
        }

        /// <summary>
        /// Set music volume (0.0 to 1.0)
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            _musicVolume = System.Math.Clamp(volume, 0f, 1f);
            MediaPlayer.Volume = _musicVolume;
            System.Diagnostics.Debug.WriteLine($"[SOUND] Volume set to {_musicVolume * 100:F0}%");
        }

        /// <summary>
        /// Get current music volume
        /// </summary>
        public float GetMusicVolume()
        {
            return _musicVolume;
        }
    }
}
