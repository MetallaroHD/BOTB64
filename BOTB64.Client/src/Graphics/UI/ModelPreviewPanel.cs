using BOTB64.Entities.DTOs;
using BOTB64.Graphics.G3D;
using BOTB64.Runtime;
using System.Numerics;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;

namespace BOTB64.Graphics.UI
{
    public class ModelPreviewPanel : UIElement
    {
        public RL.Rectangle Bounds { get; set; }
        private readonly RL.RenderTexture2D RenderTarget;
        private RL.Camera3D Camera;

        private const float RotationSpeed = 35f;

        private readonly Dictionary<int, ModelInstance> ModelCache = new();
        private int CurrentCharacterIndex = -1;
        private float RotationDegrees = 0f;

        public ModelPreviewPanel(int width, int height)
        {
            RenderTarget = RB.LoadRenderTexture(width, height);
            if (RenderTarget.Id == 0)
                Console.WriteLine("ModelPreviewPanel: LoadRenderTexture failed to create a valid FBO!");
            Camera = new RL.Camera3D
            {
                Position = new Vector3(0f, 1.4f, 3.2f),
                Target = new Vector3(0f, 1.0f, 0f),
                Up = new Vector3(0f, 1f, 0f),
                FovY = 40f,
                Projection = RL.CameraProjection.Perspective
            };
        }

        public void SetCharacter(CharacterDTO character, int characterIndex)
        {
            if (characterIndex == CurrentCharacterIndex) return;

            CurrentCharacterIndex = characterIndex;
            RotationDegrees = 0f;

            if (!ModelCache.ContainsKey(characterIndex))
            {
                string path = CommonURIs.GetCharacterModel(character);
                ModelCache[characterIndex] = new ModelInstance(AssetManager.GetModel(path, ModelPurpose.CharSelect));
            }
        }

        public override void Update(float dt)
        {
            if (CurrentCharacterIndex < 0) return;
            RotationDegrees += RotationSpeed * dt;
            if (RotationDegrees >= 360f) RotationDegrees -= 360f;

            var model = ModelCache[CurrentCharacterIndex];
            model.Transform.RotationAxis = Vector3.UnitY;
            model.Transform.RotationAngle = RotationDegrees;
        }

        public void RenderToTexture()
        {
            if (RenderTarget.Id == 0) return;
            if (CurrentCharacterIndex < 0 || !ModelCache.TryGetValue(CurrentCharacterIndex, out var model))
                return;

            RB.BeginTextureMode(RenderTarget);
            RB.ClearBackground(new RL.Color(20, 20, 28, 255));
            RB.BeginMode3D(Camera);
            ShaderManager.UpdateCharSelect(Camera.Position);
            RB.BeginShaderMode(ShaderManager.CS.GetShader());
            model.Draw();
            RB.EndShaderMode();
            RB.EndMode3D();
            RB.EndTextureMode();
        }

        public override void Draw()
        {
            if (RenderTarget.Id == 0 || CurrentCharacterIndex < 0) return;

            RL.Rectangle src = new RL.Rectangle(0, 0, RenderTarget.Texture.Width, -RenderTarget.Texture.Height);
            RB.DrawTexturePro(RenderTarget.Texture, src, Bounds, Vector2.Zero, 0f, RL.Color.White);
        }

        public void Unload()
        {
            ModelCache.Clear();
            AssetManager.UnloadAll();
            RB.UnloadRenderTexture(RenderTarget);
        }
    }
}