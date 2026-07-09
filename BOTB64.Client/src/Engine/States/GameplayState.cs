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

        private IAction? CurrentAction;

        public NetSession? Session { get; set; }

        public void OnEnter()
        {
            Logger.Init(Screen.Log);
            Game.Initialize(Initer);
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
            ShaderManager.UpdateWorld();
            Game.Render();
            Viewport.End();
            FloatingTextManager.Draw(Viewport);
            UIRenderer.Begin();
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
            RegisterBinding([Idle], null, RL.KeyboardKey.M, () => { if (!IsMyCharacter(Game.CurrentCharacter)) return; Move.SetCurrentCharacter(Game.CurrentCharacter); ChangeAction(Move); }, KeyBindingType.Press);
            RegisterBinding([Idle], null, RL.KeyboardKey.K, () => { if (!IsMyCharacter(Game.CurrentCharacter)) return; Atk.SetCurrentCharacter(Game.CurrentCharacter); ChangeAction(Atk); }, KeyBindingType.Press);
            RegisterBinding([Idle], null, RL.KeyboardKey.Space, () => { if (!IsMyCharacter(Game.CurrentCharacter)) return; Channel.Submit(new EndTurnCommand { ActingCharacterID = Game.CurrentCharacter.GameID }); Console.WriteLine("New Turn: " + Game.CurrentCharacter.Name); }, KeyBindingType.Press);
            RegisterBinding([Move], null, RL.KeyboardKey.Tab, () => { Move.CycleToNextPath(); }, KeyBindingType.Press);
            RegisterBinding([Move, Atk], null, RL.KeyboardKey.Escape, () => { ChangeAction(Idle); }, KeyBindingType.Press);

            Move.SetLMBinding(() => { if (!Enabled) return; if (Screen.IsMouseBlocked()) return; Channel.Submit(new MoveCommand { ActingCharacterID = Game.CurrentCharacter.GameID, Path = Move.GetPath() }); ChangeAction(Idle); });
            Atk.SetLMBinding(() => { if (!Enabled) return; if (Screen.IsMouseBlocked()) return; Character? tg = Atk.ConfirmTarget(); if(tg != null) Channel.Submit(new AutoAttackCommand { ActingCharacterID = Game.CurrentCharacter.GameID, TargetID = tg.GameID }); ChangeAction(Idle); });
        }

        public Hex GetMouseAxial(out bool valid)
        {             
            Hex ret = HexAlgo.WorldToHex(Viewport.GetMouseXZ());
            valid = Game.GetBoard().IsValidHex(ret);
            return ret;
        }

        private bool IsMyCharacter(Character c) => Session == null || c.OwnerID == Session.LocalPlayerID;
    }
}