using BOTB64.Graphics.G3D;
using BOTB64.Runtime;
using BOTB64.Entities;
using RB = Raylib_cs.Raylib;
using RL = Raylib_cs;
using BOTB64.Graphics.UI;
using BOTB64.Entities.DTOs;
using BOTB64.Engine.Actions;
using BOTB64.Graphics.Animations;
using System.Net.Http.Headers;
using BOTB64.Engine.Net;

namespace BOTB64.Engine.States
{
    public class GameplayState : IGameState
    {
        private bool Enabled = true;

        public GameInitializer Initer = new();
        private Game Game = new();
        IGameCommandChannel Channel; //init depending on game type

        private Viewport Viewport = new();
        private GameOverlayScreen Screen = new();

        private DefaultAction Idle;
        private CharacterMoveAction Move;
        private AutoAttackAction Atk;
        private SpellCastingAction Spell;
        private PauseAction Pause;

        private IAction? CurrentAction;
        public NetSession? Session { get; set; }

        private Character CurrentCharacter => Game.CurrentCharacter;
        private Character? Target;

        public void OnEnter()
        {
            Logger.Init(Screen.Log);
            Game.Initialize(Initer);
            ShaderManager.UpdateWorld();
            Channel = Session == null ? new LocalCommandChannel(Game) : new NetworkedCommandChannel(Game, Session);
            Targeter.SetBoard(Game.GetBoard());
            AuraTriggerManager.Init(Game);
            InitActions();
        }

        public void OnExit()
        {
            Game.Unload();
            AnimationManager.Clear();
            Logger.Unload();
        }

        public void Update(float dt)
        {
            Session?.PumpMainThreadActions();
            bool gameOver = false;
            CurrentAction?.Update();
            Game.Update(dt, out gameOver);
            Viewport.Update(dt);
            Screen.Update(dt);
            Logger.Update();
            FloatingTextManager.Update(dt);
            AnimationManager.Update(dt);

            if (gameOver)
            {
                GameOverState gOver = new GameOverState { Winner = Game.Winner, Session = Session };
                StateManager.ChangeState(gOver);
            }
        }

        public void Render()
        {
            Viewport.Begin();
            ShaderManager.UpdateCameraPosition(Viewport.Camera.Position);
            Game.Render();
            Viewport.End();
            FloatingTextManager.Draw(Viewport);
            Screen.Draw();
        }

        public void ChangeAction(IAction action)
        {
            CurrentAction?.Exit();
            CurrentAction = action;
            CurrentAction?.Enter();
        }

        private void InitActions()
        {
            Idle = new DefaultAction(this);
            Move = new CharacterMoveAction(this);
            Atk = new AutoAttackAction(this);
            Spell = new SpellCastingAction(this);
            Pause = new PauseAction(this);
            //other actions
            InitBindings();
            ChangeAction(Idle);
        }

        private void RegisterBinding(List<ActionBase> addTo, Button? btn, RL.KeyboardKey key, Action action, KeyBindingType type)
        {
            foreach (var item in addTo)
            {
                item.AddBinding(key, action, type);
            }
            if(btn != null)
                btn.OnClick = action;
        }

        private void InitBindings()
        {
            RegisterBinding([Idle], null, RL.KeyboardKey.Escape, () => { ChangeAction(Pause); }, KeyBindingType.Press);
            RegisterBinding([Idle], Screen.MoveButton, RL.KeyboardKey.M, () => { if (!IsMyCharacter(Game.CurrentCharacter)) return; Move.SetCurrentCharacter(Game.CurrentCharacter); ChangeAction(Move); }, KeyBindingType.Press);
            RegisterBinding([Idle], Screen.AttackButton, RL.KeyboardKey.K, () => { if (!IsMyCharacter(Game.CurrentCharacter)) return; Atk.SetCurrentCharacter(Game.CurrentCharacter); ChangeAction(Atk); }, KeyBindingType.Press);
            RegisterBinding([Idle], Screen.TurnButton, RL.KeyboardKey.Space, () => { if (!IsMyCharacter(Game.CurrentCharacter)) return; Channel.Submit(new EndTurnCommand { ActingCharacterID = Game.CurrentCharacter.GameID }); Console.WriteLine("New Turn: " + Game.CurrentCharacter.Name); }, KeyBindingType.Press);
            RegisterBinding([Move], null, RL.KeyboardKey.Tab, () => { Move.CycleToNextPath(); }, KeyBindingType.Press);
            RegisterBinding([Move, Atk], null, RL.KeyboardKey.Escape, () => { ChangeAction(Idle); }, KeyBindingType.Press);

            Idle.SetLMBinding(() => { if (!Enabled) return; if (Screen.IsMouseBlocked()) return; Target = Idle.GetTarget(); InputManager.UseClick(); UpdateTargetGUI(); });
            Move.SetLMBinding(() => { if (!Enabled) return; if (Screen.IsMouseBlocked()) return; Channel.Submit(new MoveCommand { ActingCharacterID = Game.CurrentCharacter.GameID, Path = Move.GetPath() }); InputManager.UseClick(); ChangeAction(Idle); });
            Atk.SetLMBinding(() => { if (!Enabled) return; if (Screen.IsMouseBlocked()) return; Character? tg = Atk.ConfirmTarget(); if(tg != null) Channel.Submit(new AutoAttackCommand { ActingCharacterID = Game.CurrentCharacter.GameID, TargetID = tg.GameID }); InputManager.UseClick(); ChangeAction(Idle); });
            Screen.ResumeButton.OnClick = () => { ChangeAction(Idle); };
        }

        public Hex GetMouseAxial(out bool valid)
        {             
            Hex ret = HexAlgo.WorldToHex(Viewport.GetMouseXZ());
            valid = Game.GetBoard().IsValidHex(ret);
            return ret;
        }

        public void TogglePauseOverlay(bool active)
        {
            if (active)
                Screen.Pause();
            else
                Screen.UnPause();
        }

        public void ToggleCameraControl(bool active)
        {
            if (active)
                Viewport.Camera.Enable();
            else
                Viewport.Camera.Disable();
        }

        private bool IsMyCharacter(Character c) => Session == null || c.OwnerID == Session.LocalPlayerID;

        private void UpdateGUI()
        {
            UpdateSpellButtons();

        }

        private void UpdatePlayerGUI()
        {

        }

        private void UpdateTargetGUI()
        {
            if (Target == null)
            {
                Screen.TargetStatus.Visible = false;
                return;
            }
            Screen.TargetStatus.Visible = true;
            Screen.TargetStatus.SetHealth(Target.CurrentHP, Target.MaxHP);
            Screen.TargetStatus.SetResource(Target.CurrentResource, Target.MaxRes);
            Screen.TargetStatus.SetName(Target.Name);
        }

        private void UpdateSpellButtons()
        {
            if (CurrentCharacter.ActiveSpells.TryGetValue(1, out Spell spell1))
            {
                Screen.Spell1Button.SetIcon(spell1.Icon);
                Screen.Spell1Button.SetTooltip(spell1.Tooltip);
            }
            else
            {
                Screen.Spell1Button.Empty();
            }
            if (CurrentCharacter.ActiveSpells.TryGetValue(2, out Spell spell2))
            {
                Screen.Spell2Button.SetIcon(spell2.Icon);
                Screen.Spell2Button.SetTooltip(spell2.Tooltip);
            }
            else
            {
                Screen.Spell2Button.Empty();
            }
            if (CurrentCharacter.ActiveSpells.TryGetValue(3, out Spell spell3))
            {
                Screen.Spell3Button.SetIcon(spell3.Icon);
                Screen.Spell3Button.SetTooltip(spell3.Tooltip);
            }
            else
            {
                Screen.Spell3Button.Empty();
            }
            if (CurrentCharacter.ActiveSpells.TryGetValue(4, out Spell spell4))
            {
                Screen.Spell4Button.SetIcon(spell4.Icon);
                Screen.Spell4Button.SetTooltip(spell4.Tooltip);
            }
            else
            {
                Screen.Spell4Button.Empty();
            }
            if (CurrentCharacter.ActiveSpells.TryGetValue(5, out Spell spell5))
            {
                Screen.Spell5Button.SetIcon(spell5.Icon);
                Screen.Spell5Button.SetTooltip(spell5.Tooltip);
            }
            else
            {
                Screen.Spell5Button.Empty();
            }
        }
    }
}