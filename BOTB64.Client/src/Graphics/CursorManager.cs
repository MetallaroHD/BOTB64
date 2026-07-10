using BOTB64.Graphics.G3D;
using System.Collections.Generic;
using System.Numerics;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Runtime
{
    // Hides the OS cursor and draws a custom PNG in its place.
    // Load one or more named cursors up front, then swap the active one with SetCursor(name).
    //
    // Usage:
    //   CursorManager.Init();
    //   CursorManager.LoadCursor("default", "assets/cursor_default.png");
    //   CursorManager.LoadCursor("hover", "assets/cursor_hover.png", new Vector2(2, 2));
    //   ...
    //   CursorManager.SetCursor("hover");
    //   ...
    // Call CursorManager.Draw() LAST in your frame, after everything else, so it renders on top.
    public static class CursorManager
    {
        private class CursorDef
        {
            public RL.Texture2D Texture;
            public Vector2 Hotspot; // offset from texture top-left to the cursor's "point"
        }

        private static readonly Dictionary<string, CursorDef> _cursors = new();
        private static string _activeName;
        private static bool _initialized = false;

        public static bool Visible = true;

        public static void Init()
        {
            if (_initialized) return;

            RB.HideCursor();
            _initialized = true;
        }

        public static void Shutdown()
        {
            RB.ShowCursor();

            foreach (var cursor in _cursors.Values)
                RB.UnloadTexture(cursor.Texture);

            _cursors.Clear();
            _activeName = null;
            _initialized = false;
        }

        // hotspot: pixel offset within the image that represents the actual "tip" of the
        // cursor (e.g. an arrow's point). Defaults to (0,0) - the top-left corner.
        public static void LoadCursor(string name, string pngPath, Vector2 hotspot = default)
        {
            if (_cursors.TryGetValue(name, out var existing))
                RB.UnloadTexture(existing.Texture);

            RL.Texture2D texture = ResourceManager.LoadTexture(pngPath);

            _cursors[name] = new CursorDef
            {
                Texture = texture,
                Hotspot = hotspot
            };

            // First cursor loaded becomes active by default.
            _activeName ??= name;
        }

        public static void SetCursor(string name)
        {
            if (_cursors.ContainsKey(name))
                _activeName = name;
        }

        public static string ActiveCursor => _activeName;

        public static void Draw()
        {
            if (!Visible || _activeName == null) return;
            if (!_cursors.TryGetValue(_activeName, out var cursor)) return;

            Vector2 mouse = InputManager.MousePosition;
            Vector2 drawPos = mouse - cursor.Hotspot;

            RB.DrawTextureV(cursor.Texture, drawPos, RL.Color.White);
        }
    }
}