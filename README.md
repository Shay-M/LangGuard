# 🔤 LangGuard

LangGuard is a lightweight Windows tray application that helps prevent typing in the wrong keyboard language.

It plays a configurable sound when you start typing after an idle period — so you immediately know whether you're typing in Hebrew or English.

---

## ✨ Features

- 🔔 Sound notification when typing session starts
- 🇮🇱 🇺🇸 Supports Hebrew and English
- ⏱ Configurable idle timeout
- 🔁 Optional sound on language change
- 🎵 Custom WAV / MP3 sounds (no Windows Media dependency)
- ⌨ Custom global hotkey (convert clipboard EN ↔ HE)
- 🖥 Runs in system tray
- 🚀 Optional "Start with Windows"
- 📋 Clipboard layout conversion (EN ↔ HE)

---

## 🧠 How It Works

LangGuard:

1. Hooks into low-level keyboard events
2. Detects typing activity
3. Checks current keyboard layout
4. Plays the corresponding sound when a new typing session starts

It ignores modifier-based shortcuts like Ctrl+C.

---

## ⚙ Settings

You can configure:

- Idle timeout (ms)
- Play sound on every key press
- Play sound when language changes
- Custom WAV/MP3 per language
- Global hotkey
- Start with Windows

---

## 🚀 Build & Run

```bash
dotnet restore
dotnet run
```
