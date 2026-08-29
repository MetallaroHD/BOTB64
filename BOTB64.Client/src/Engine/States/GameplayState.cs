using System.Numerics;
using BOTB64.Graphics.G3D;
using BOTB64.Runtime;
using BOTB64.Entities;
using RL = Raylib_cs;
using BOTB64.Graphics.UI;
using BOTB64.Engine;
using BOTB64.Engine.Actions;
using BOTB64.Graphics.Animations;
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
            FloatingMessageManager.Init(Screen);
            AuraTriggerManager.Init(Game);
            Game.Initialize(Initer);
            ShaderManager.UpdateWorld();
            Channel = Session == null ? new LocalCommandChannel(Game) : new NetworkedCommandChannel(Game, Session);
            Targeter.SetBoard(Game.GetBoard());
            Screen.ShowSecretsAvailable = Session == null;
            Screen.ShowSecretsButton.Visible = Screen.ShowSecretsAvailable;
            InitActions();
        }

        public void OnExit()
        {
            Game.Unload();
            AnimationManager.Clear();
            Logger.Unload();
            AuraTriggerManager.ClearCache();
        }

        public void Update(float dt)
        {
            Session?.PumpMainThreadActions();
            bool gameOver = false;
            CurrentAction?.Update();
            Game.Update(dt, out gameOver);
            UpdateGUI();
            Viewport.Update(dt);
            Screen.Update(dt);
            Logger.Update();
            FloatingTextManager.Update(dt);
            FloatingMessageManager.Update(dt);
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
            Game.Render(VisibleSecretFaction());
            Viewport.End();
            FloatingTextManager.Draw(Viewport);
            Screen.Draw();
            Screen.TileTooltip.Draw();
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
            RegisterBinding([Idle], null, RL.KeyboardKey.Escape, () => { Pause.Mode = PauseMode.Esc;  ChangeAction(Pause); }, KeyBindingType.Press);
            RegisterBinding([Idle], Screen.MoveButton, RL.KeyboardKey.M, () => { if (!IsMyCharacter(Game.CurrentCharacter)) return; Move.SetCurrentCharacter(Game.CurrentCharacter); ChangeAction(Move); }, KeyBindingType.Press);
            RegisterBinding([Idle], Screen.AttackButton, RL.KeyboardKey.K, () => { if (!IsMyCharacter(Game.CurrentCharacter)) return; Atk.SetCurrentCharacter(Game.CurrentCharacter); ChangeAction(Atk); }, KeyBindingType.Press);
            RegisterBinding([Idle], Screen.Spell1Button, RL.KeyboardKey.One, () => TryEnterSpellCast(1), KeyBindingType.Press);
            RegisterBinding([Idle], Screen.Spell2Button, RL.KeyboardKey.Two, () => TryEnterSpellCast(2), KeyBindingType.Press);
            RegisterBinding([Idle], Screen.Spell3Button, RL.KeyboardKey.Three, () => TryEnterSpellCast(3), KeyBindingType.Press);
            RegisterBinding([Idle], Screen.Spell4Button, RL.KeyboardKey.Four, () => TryEnterSpellCast(4), KeyBindingType.Press);
            RegisterBinding([Idle], Screen.Spell5Button, RL.KeyboardKey.Five, () => TryEnterSpellCast(5), KeyBindingType.Press);
            RegisterBinding([Idle], Screen.TurnButton, RL.KeyboardKey.Space, () => { if (!IsMyCharacter(Game.CurrentCharacter)) return; if (!Settings.AskEndTurn) { SubmitEndTurn(); } else { Pause.Mode = PauseMode.Turn; ChangeAction(Pause); } }, KeyBindingType.Press);
            RegisterBinding([Move], null, RL.KeyboardKey.Tab, () => { Move.CycleToNextPath(); }, KeyBindingType.Press);
            RegisterBinding([Move, Atk, Spell], null, RL.KeyboardKey.Escape, () => { InputManager.UseKey(); ChangeAction(Idle); }, KeyBindingType.Press);

            Idle.SetLMBinding(SetTarget);
            Move.SetLMBinding(SubmitMove);
            Atk.SetLMBinding(SubmitAttack);
            Spell.SetLMBinding(SubmitSpellCast);
            Move.SetRMBinding(() => ChangeAction(Idle));
            Atk.SetRMBinding(() => ChangeAction(Idle));
            Spell.SetRMBinding(() => ChangeAction(Idle));
            Screen.ResumeButton.OnClick = () => { ChangeAction(Idle); };
            Screen.NoButton.OnClick = () => { ChangeAction(Idle); };
            Screen.YesButton.OnClick = () => { SubmitEndTurn(); InputManager.UseClick(); ChangeAction(Idle); };
        }

        private void TryEnterSpellCast(int slot)
        {
            if (!IsMyCharacter(Game.CurrentCharacter))
                return;
            if (!Game.CurrentCharacter.ActiveSpells.TryGetValue(slot, out Spell spell))
                return;

            string? reason = GetSpellCastBlockReason(Game.CurrentCharacter, spell);
            if (reason != null)
            {
                FloatingMessageManager.AddMessage(reason);
                return;
            }

            // No real aim needed (e.g. a self-cast beam/nova) - skip the targeter and the
            // extra click-to-confirm entirely, and fire immediately now that validation passed.
            if (spell.ExplicitTarget == TargetingType.None)
            {
                if (!Enabled)
                    return;
                Channel.Submit(new SpellCastCommand { ActingCharacterID = Game.CurrentCharacter.GameID, ExplicitTarget = new List<Hex>(), SpellID = spell.ID });
                return;
            }

            Spell.SetCurrentCharacter(Game.CurrentCharacter);
            Spell.SpellBind = slot;
            ChangeAction(Spell);
        }

        // Client-side only pre-check so the targeter never even opens for a spell that
        // SpellCastCommand.Validate would reject server/authoritative-side anyway.
        private string? GetSpellCastBlockReason(Character caster, Spell spell)
        {
            if (spell.IsPassive)
                return $"{spell.Name} is passive.";
            if (Game.RoundNumber < spell.Preparation)
                return $"{spell.Name} is not available until round {spell.Preparation}.";
            if (spell.Charges == 0)
            {
                if (spell.CurrentCD > 0)
                    return $"{spell.Name} is on cooldown ({spell.CurrentCD}).";
            }
            else if (spell.CurrentCharges <= 0)
            {
                return $"{spell.Name} has no charges left ({spell.CurrentCD} until next).";
            }
            if (caster.CurrentResource < spell.Cost)
                return $"Not enough resource to cast {spell.Name}.";
            if (spell.CastTime == 0 && caster.RemainAction <= 0)
                return "No action remaining.";
            if (spell.CastTime == -1 && caster.RemainFastAction <= 0)
                return "No fast action remaining.";
            return null;
        }

        public void SubmitEndTurn()
        {
            Channel.Submit(new EndTurnCommand { ActingCharacterID = Game.CurrentCharacter.GameID });
            Console.WriteLine("New Turn: " + Game.CurrentCharacter.Name);
        }

        public void SubmitMove()
        {
            if (!Enabled) 
                return; 
            if (Screen.IsMouseBlocked()) 
                return; 
            Channel.Submit(new MoveCommand { ActingCharacterID = Game.CurrentCharacter.GameID, Path = Move.GetPath() }); 
            InputManager.UseClick(); 
            ChangeAction(Idle);
        }

        public void SetTarget()
        {
            if (!Enabled) 
                return; 
            if (Screen.IsMouseBlocked()) 
                return; 
            Target = Idle.GetTarget(); 
            InputManager.UseClick();
        }

        public void SubmitAttack()
        {
            if (!Enabled) 
                return; 
            if (Screen.IsMouseBlocked()) 
                return; 
            Character? tg = Atk.ConfirmTarget(); 
            if (tg != null) 
                Channel.Submit(new AutoAttackCommand { ActingCharacterID = Game.CurrentCharacter.GameID, TargetID = tg.GameID }); 
            InputManager.UseClick(); 
            ChangeAction(Idle);
        }

        public void SubmitSpellCast()
        {
            if (!Enabled)
                return;
            if (Screen.IsMouseBlocked())
                return;
            if (!Game.CurrentCharacter.ActiveSpells.TryGetValue(Spell.SpellBind, out Spell sp))
                return;

            // DualDirect spells need two confirm-clicks - the first one just locks in the
            // first pick and stays in the Spell action for the second, instead of submitting.
            if (Spell.NeedsFirstPick(sp))
            {
                Spell.ConfirmFirstPick();
                InputManager.UseClick();
                return;
            }

            List<Hex>? tg = sp.ExplicitTarget == TargetingType.DualDirect ? Spell.GetDualExplicitTarget() : Spell.GetExplicitTarget();
            if (tg != null)
                Channel.Submit(new SpellCastCommand { ActingCharacterID = Game.CurrentCharacter.GameID, ExplicitTarget = tg, SpellID = sp.ID });
            InputManager.UseClick();
            ChangeAction(Idle);
        }

        public Hex GetMouseAxial(out bool valid)
        {
            Hex ret = HexAlgo.WorldToHex(Viewport.GetMouseXZ());
            valid = !Screen.IsMouseBlocked() && Game.GetBoard().IsValidHex(ret);
            return ret;
        }

        public Game GetGame() => Game;

        public void TogglePauseOverlay(bool active)
        {
            if (active)
                Screen.Pause();
            else
                Screen.UnPause();
        }

        public void ToggleAskEndTurn(bool active)
        {
            if (active)
                Screen.ShowEndTurn();
            else
                Screen.HideEndTurn();
        }

        public void ToggleCameraControl(bool active)
        {
            if (active)
                Viewport.Camera.Enable();
            else
                Viewport.Camera.Disable();
        }

        private bool IsMyCharacter(Character c) => Session == null || c.OwnerID == Session.LocalPlayerID;

        // null in local (hot-seat) play, where everyone shares one screen and secret tile
        // effects should never render for anyone; the local player's Faction online.
        private Faction? LocalViewerFaction() => Session?.LocalPlayer.Faction;

        // Online play always defers to the local player's own team. Local (hot-seat) play has
        // no fixed "local team" since everyone shares the screen, so secrets stay hidden
        // unless the Show Secrets button is being held - in which case they're revealed for
        // whichever team is currently taking its turn.
        private Faction? VisibleSecretFaction()
        {
            if (Session != null)
                return LocalViewerFaction();

            return Screen.ShowSecretsButton.IsHeld ? Game.CurrentCharacter.Faction : null;
        }

        private void UpdateGUI()
        {
            UpdateSpellButtons();
            UpdatePlayerGUI();
            UpdateTargetGUI();
            UpdateTileTooltip();
        }

        // Lists the name of every tile effect on the hovered board tile, vertically,
        // one per line - hidden entirely when there are none. Respects the same
        // secret-effect visibility rule as rendering (Board.Draw/IsVisibleTo).
        private void UpdateTileTooltip()
        {
            var names = new List<string>();

            if (!Screen.IsMouseBlocked())
            {
                Hex hex = GetMouseAxial(out bool valid);
                Tile? tile = valid ? Game.GetBoard().GetTile(hex) : null;

                if (tile != null)
                {
                    Faction? viewerFaction = VisibleSecretFaction();
                    foreach (var fx in tile.Effects.ToList())
                    {
                        if (fx.Secret && (viewerFaction == null || fx.Owner == null || fx.Owner.Faction != viewerFaction))
                            continue;
                        names.Add(fx.Name);
                    }
                }
            }

            Screen.TileTooltip.SetContent(names);
            if (names.Count > 0)
                Screen.TileTooltip.SetPosition(UIRenderer.ScreenToUI(InputManager.MousePosition) + new Vector2(16, 16));
        }

        private void UpdatePlayerGUI()
        {
            var current = CurrentCharacter;
            Screen.PlayerStatus.SetHealth(current.CurrentHP, current.MaxHP.GetI());
            Screen.PlayerStatus.SetResource(current.CurrentResource, current.MaxRes.GetI());
            Screen.PlayerStatus.SetResourceColor(current.ResourceColor());
            Screen.PlayerStatus.SetName(current.Name);
            Screen.PlayerStatus.Effects.Sync(current.CurrentAuras, a => new EffectDisplayInfo(a.ID, a.Name, a.Tooltip, a.CurrentStacks, a.Remaining, a.Icon));
            UpdateSpellButtons();
        }

        private void UpdateTargetGUI()
        {
            if (Target == null || !Target.Alive)
            {
                Target = null;
                Screen.TargetStatus.Visible = false;
                return;
            }
            var target = Target;
            Screen.TargetStatus.Visible = true;
            Screen.TargetStatus.SetHealth(target.CurrentHP, target.MaxHP.GetI());
            Screen.TargetStatus.SetResource(target.CurrentResource, target.MaxRes.GetI());
            Screen.TargetStatus.SetResourceColor(target.ResourceColor());
            Screen.TargetStatus.SetName(target.Name);
            Screen.TargetStatus.Effects.Sync(target.CurrentAuras, a => new EffectDisplayInfo(a.ID, a.Name, a.Tooltip, a.CurrentStacks, a.Remaining, a.Icon));
        }

        public List<string> BuildSpellTooltip(Spell spell)
        {
            var lines = new List<string>();

            if (!string.IsNullOrEmpty(spell.Name))
                lines.Add(spell.Name);

            if (spell.IsPassive)
            {
                lines.Add("Passive");
            }
            else
            {
                if (spell.Cost > 0)
                    lines.Add($"Cost: {spell.Cost}");
                if (spell.Cooldown > 0)
                    lines.Add($"Cooldown: {spell.Cooldown}");
            }

            if (!string.IsNullOrEmpty(spell.Tooltip))
                lines.Add(spell.Tooltip);

            if (spell.CurrentCD > 0)
                lines.Add($"Remaining: {spell.CurrentCD}");

            if (spell.Charges > 1)
                lines.Add($"Charges: {spell.CurrentCharges}/{spell.Charges}");

            if (spell.Preparation > Game.RoundNumber)
                lines.Add($"Unlocks at Round {spell.Preparation}");

            return lines;
        }

        private void UpdateSpellButtons()
        {
            var current = CurrentCharacter;
            if (current.ActiveSpells.TryGetValue(1, out Spell spell1))
            {
                Screen.Spell1Button.SetIcon(spell1.Icon);
                Screen.Spell1Button.SetTooltip(BuildSpellTooltip(spell1));
            }
            else
            {
                Screen.Spell1Button.Empty();
            }
            if (current.ActiveSpells.TryGetValue(2, out Spell spell2))
            {
                Screen.Spell2Button.SetIcon(spell2.Icon);
                Screen.Spell2Button.SetTooltip(BuildSpellTooltip(spell2));
            }
            else
            {
                Screen.Spell2Button.Empty();
            }
            if (current.ActiveSpells.TryGetValue(3, out Spell spell3))
            {
                Screen.Spell3Button.SetIcon(spell3.Icon);
                Screen.Spell3Button.SetTooltip(BuildSpellTooltip(spell3));
            }
            else
            {
                Screen.Spell3Button.Empty();
            }
            if (current.ActiveSpells.TryGetValue(4, out Spell spell4))
            {
                Screen.Spell4Button.SetIcon(spell4.Icon);
                Screen.Spell4Button.SetTooltip(BuildSpellTooltip(spell4));
            }
            else
            {
                Screen.Spell4Button.Empty();
            }
            if (current.ActiveSpells.TryGetValue(5, out Spell spell5))
            {
                Screen.Spell5Button.SetIcon(spell5.Icon);
                Screen.Spell5Button.SetTooltip(BuildSpellTooltip(spell5));
            }
            else
            {
                Screen.Spell5Button.Empty();
            }
        }
    }
}