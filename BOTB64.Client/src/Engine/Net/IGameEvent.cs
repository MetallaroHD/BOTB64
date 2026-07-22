using BOTB64.Entities;
using BOTB64.Graphics.Animations;
using BOTB64.Graphics.UI;
using BOTB64.Runtime;
using MessagePack;

namespace BOTB64.Engine.Net
{
    [Union(0, typeof(DamageEvent))]
    [Union(1, typeof(DeathEvent))]
    [Union(2, typeof(MoveEvent))]
    [Union(3, typeof(TurnAdvancedEvent))]
    [Union(4, typeof(CharacterReassignedEvent))]
    [Union(5, typeof(TeamEliminatedEvent))]
    [Union(6, typeof(ActionSpentEvent))]
    [Union(7, typeof(HealEvent))]
    [Union(8, typeof(ApplyAuraEvent))]
    [Union(9, typeof(ApplyTileEffectEvent))]
    [Union(10, typeof(SpellCastEvent))]
    public interface IGameEvent
    {
        void Apply(Game game);
    }

    [MessagePackObject]
    public struct CharacterReassignedEvent : IGameEvent
    {
        [Key(0)] public int CharacterGameID;
        [Key(1)] public int NewOwnerID;
        public void Apply(Game game)
        {
            var character = game.FindCharacter(CharacterGameID);
            if (character != null) character.OwnerID = NewOwnerID;
        }
    }

    [MessagePackObject]
    public struct TeamEliminatedEvent : IGameEvent
    {
        [Key(0)] public Faction Winner;
        public void Apply(Game game) => game.ForceGameOver(Winner);
    }

    [MessagePackObject]
    public struct TurnAdvancedEvent : IGameEvent
    {
        [Key(0)] public int NextCharacterID;
        [Key(1)] public int TurnNumber;

        public void Apply(Game game) => game.ApplyTurnAdvance(NextCharacterID, TurnNumber);
    }

    [MessagePackObject]
    public struct DamageEvent : IGameEvent 
    {
        [Key(0)] public int TargetID;
        [Key(1)] public int Amount;
        [Key(2)] public bool Crit;
        public void Apply(Game game)
        {
            var target = game.FindCharacter(TargetID);
            if (target != null)
            {
                target.CurrentHP -= Amount;
                FloatingTextManager.Add(Amount.ToString(), HexAlgo.HexToWorld(target.Position));
                Logger.Log(target.Name + " receives " + Amount + " damage." + (Crit ? " A critical hit!" : ""));
            }
        }
    }

    [MessagePackObject]
    public struct HealEvent : IGameEvent
    {
        [Key(0)] public int TargetID;
        [Key(1)] public int Amount;
        [Key(2)] public bool Crit;
        public void Apply(Game game)
        {
            var target = game.FindCharacter(TargetID);
            if (target != null)
            {
                target.CurrentHP = Math.Max(target.CurrentHP + Amount, target.MaxHP.GetI());
                FloatingTextManager.Add(Amount.ToString(), HexAlgo.HexToWorld(target.Position), color: Raylib_cs.Color.Green);
                Logger.Log(target.Name + " heals " + Amount + " damage." + (Crit ? " A critical hit!" : ""));
            }
        }
    }

    [MessagePackObject]
    public struct DeathEvent : IGameEvent
    {
        [Key(0)] public int CharacterID;
        public void Apply(Game game) => game.FindCharacter(CharacterID)?.Die();
    }

    [MessagePackObject]
    public struct MoveEvent : IGameEvent
    {
        [Key(0)] public int CharacterID;
        [Key(1)] public Hex Step; // single destination tile, not a path

        public void Apply(Game game)
        {
            var character = game.FindCharacter(CharacterID);
            if (character == null) return;

            var tile = game.GetBoard().GetTile(Step);
            if (tile == null) return;

            character.RemainMovement -= 1;
            var anim = new CharacterMoveAnimation(character, new List<Hex> { character.Position, tile.AxialPosition });
            game.GetBoard().MoveCharacter(character, new List<Hex> { character.Position, tile.AxialPosition });
            AnimationManager.Play(anim);
        }
    }

    [MessagePackObject]
    public struct ActionSpentEvent : IGameEvent
    {
        [Key(0)] public int CharacterID;
        [Key(1)] public bool FastAction;
        public void Apply(Game game)
        {
            var c = game.FindCharacter(CharacterID);
            if (c == null) return;
            if (FastAction) c.RemainFastAction--; else c.RemainAction--;
        }
    }

    [MessagePackObject]
    public struct ApplyAuraEvent : IGameEvent
    {
        [Key(0)] public int OwnerID;
        [Key(1)] public int TargetID;
        [Key(2)] public int AuraID;
        [Key(3)] public int FinalStacks;

        public void Apply(Game game)
        {
            var o = game.FindCharacter(OwnerID);
            var t = game.FindCharacter(TargetID);
            if (o == null || t == null) return;

            int id = AuraID;
            var existing = t.CurrentAuras.FirstOrDefault(a => a.ID == id);
            if (existing != null)
            {
                existing.CurrentStacks = FinalStacks;
                existing.Remaining = existing.Duration;
            }
            else
            {
                var aura = AuraTriggerManager.GetAura(AuraID);
                aura.Owner = o;
                aura.Wearer = t;
                aura.CurrentStacks = FinalStacks;
                aura.Remaining = aura.Duration;
                t.CurrentAuras.Add(aura);
            }
        }
    }

    [MessagePackObject]
    public struct ApplyTileEffectEvent : IGameEvent
    {
        [Key(0)] public int OwnerID;
        [Key(1)] public Hex Position;
        [Key(2)] public int TileEffectID;
        [Key(3)] public int Duration;

        public void Apply(Game game)
        {
            var tile = game.GetBoard().GetTile(Position);
            var owner = game.FindCharacter(OwnerID);
            if (tile == null) return;

            var template = AuraTriggerManager.GetTileEffect(TileEffectID);
            if (template == null) return;

            var instance = new TileEffect
            {
                ID = template.ID,
                Name = template.Name,
                Duration = template.Duration,
                Dispel = template.Dispel,
                TileType = template.TileType,
                Flags = template.Flags,
                Type = template.Type,
                Owner = owner,
                Remaining = Duration
            };
            tile.Effects.Add(instance);
        }
    }

    [MessagePackObject]
    public struct SpellCastEvent : IGameEvent
    {
        [Key(0)] public int CharacterID;
        [Key(1)] public int SpellID;
        [Key(2)] public int CostSpent;

        public void Apply(Game game)
        {
            var character = game.FindCharacter(CharacterID);
            if (character == null) return;

            character.CurrentResource -= CostSpent;

            int spellId = SpellID;
            var spell = character.ActiveSpells.Values.FirstOrDefault(s => s.ID == spellId);
            if (spell != null)
            {
                spell.CurrentCD = spell.Cooldown;
                spell.CurrentCharges--;
                if (spell.Animation != null)
                    AnimationManager.Play(spell.Animation);
            }
        }
    }
}
