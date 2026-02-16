using System;
using System.IO;
using NAudio.Wave;
using NLayer.NAudioSupport;

namespace LangGuard.Core
{
    public sealed class SoundService : IDisposable
    {
        private AppConfig _cfg;

        private readonly object _lock = new object();
        private IWavePlayer? _output;
        private WaveStream? _reader;

        public SoundService(AppConfig cfg)
        {
            _cfg = cfg;
        }

        public void UpdateConfig(AppConfig cfg)
        {
            _cfg = cfg;
        }

        public void Dispose()
        {
            Stop();
        }

        public void PlayForLanguage(ushort langId)
        {
            if (langId == LanguageService.LangHebrew && TryPlayAudio(_cfg.SoundHebrewPath, out _))
                return;

            if (langId == LanguageService.LangEnglish && TryPlayAudio(_cfg.SoundEnglishPath, out _))
                return;

            // Fallback: system beeps
            if (langId == LanguageService.LangHebrew)
            {
                System.Media.SystemSounds.Asterisk.Play();
            }
            else if (langId == LanguageService.LangEnglish)
            {
                System.Media.SystemSounds.Exclamation.Play();
            }
            else
            {
                System.Media.SystemSounds.Beep.Play();
            }
        }

        public bool TestPlay(string? path, bool fallbackHebrew, out string? error)
        {
            if (TryPlayAudio(path, out error))
                return true;

            if (fallbackHebrew)
            {
                System.Media.SystemSounds.Asterisk.Play();
            }
            else
            {
                System.Media.SystemSounds.Exclamation.Play();
            }

            return false;
        }

        private bool TryPlayAudio(string? path, out string? error)
        {
            error = null;

            if (string.IsNullOrWhiteSpace(path))
            {
                error = "Path is empty.";
                return false;
            }

            if (!File.Exists(path))
            {
                error = "File does not exist.";
                return false;
            }

            try
            {
                lock (_lock)
                {
                    Stop_NoLock();

                    _reader = CreateReader(path);
                    _output = new WaveOutEvent();
                    _output.Init(_reader);
                    _output.Play();
                }

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                Stop();
                return false;
            }
        }

        private static WaveStream CreateReader(string path)
        {
            string ext = Path.GetExtension(path).ToLowerInvariant();

            if (ext == ".mp3")
            {
                // Fully managed MP3 decoding (no Windows Media / codecs required)
                return new Mp3FileReader(path);
            }

            // WAV (and others supported by AudioFileReader)
            return new AudioFileReader(path);
        }

        private void Stop()
        {
            lock (_lock)
            {
                Stop_NoLock();
            }
        }

        private void Stop_NoLock()
        {
            try
            {
                _output?.Stop();
            }
            catch
            {
            }

            _output?.Dispose();
            _output = null;

            _reader?.Dispose();
            _reader = null;
        }
    }
}
