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

        public bool IsMuted { get; private set; } = false;

        private SoundManager()
        {
            _songs = new Dictionary<string, Song>();
            _sounds = new Dictionary<string, SoundEffect>();
        }

        // Hàm load nhạc nền (MP3) dùng Song.FromUri
        public void LoadSong(string name, string assetName)
        {
            if (_songs.ContainsKey(name)) return;

            // Lấy ContentManager từ GameManager (đã gán ở Game1)
            var content = GameManager.Instance.Content;

            // Cách chuẩn: Load qua Content Pipeline
            // assetName ví dụ: "audio/titleMusic" (không có đuôi .mp3)
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

        // Hàm load âm thanh ngắn (WAV) - nếu sau này bạn dùng
            /*
            public void LoadSound(string name, string filePath, Microsoft.Xna.Framework.Content.ContentManager content)
            {
                 // SoundEffect thường phải load qua Content Pipeline hoặc Stream
                 // Tạm thời để trống hoặc dùng Content.Load nếu bạn add vào MGCB
            }
            */

        public void PlayMusic(string name, bool isLooping = true)
        {
            if (IsMuted) return;

            if (_songs.ContainsKey(name))
            {
                MediaPlayer.IsRepeating = isLooping;
                MediaPlayer.Play(_songs[name]);
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
    }
}
