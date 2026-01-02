using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System;

namespace MarioGame.Managers
{
    public class SoundManager
    {
        private static SoundManager _instance;
        public static SoundManager Instance => _instance ??= new SoundManager();

        private ContentManager _content;
        private readonly Dictionary<string, SoundEffect> _sfx = new();
        private readonly Dictionary<string, Song> _music = new();

        public bool IsMusicOn { get; set; } = true;
        public bool IsSfxOn { get; set; } = true;
        public float MasterVolume { get; set; } = 1f;

        private SoundManager() { }

        /* =========================
           INITIALIZATION
           ========================= */

        public void Initialize(ContentManager content)
        {
            _content = content;
            try
            {
                MediaPlayer.Volume = MasterVolume;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: unable to set MediaPlayer volume: {ex.Message}");
            }
        }

        /* =========================
           SOUND EFFECTS
           ========================= */

        public void PlaySound(string name)
        {
            if (!IsSfxOn || _content == null) return;

            try
            {
                if (!_sfx.ContainsKey(name))
                {
                    _sfx[name] = _content.Load<SoundEffect>("Sounds/" + name);
                }

                _sfx[name]?.Play(MasterVolume, 0f, 0f);
            }
            catch (ContentLoadException)
            {
                Console.WriteLine($"Sound asset not found: Sounds/{name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to play sound '{name}': {ex.Message}");
            }
        }

        /* =========================
           MUSIC
           ========================= */

        public void PlayMusic(string name)
        {
            if (!IsMusicOn || _content == null) return;

            try
            {
                if (!_music.ContainsKey(name))
                {
                    _music[name] = _content.Load<Song>("Music/" + name);
                }

                var song = _music[name];
                if (song == null)
                    return;

                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = MasterVolume;
                MediaPlayer.Play(song);
            }
            catch (ContentLoadException)
            {
                Console.WriteLine($"Music asset not found: Music/{name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to play music '{name}': {ex.Message}");
            }
        }
    }
}
