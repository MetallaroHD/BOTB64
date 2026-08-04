---@meta
-- API reference for LuaEffectRunner bindings. Not loaded at runtime — for editor
-- intellisense only. Keep this in sync manually whenever LuaEffectRunner.cs changes.

--- ACTIONS ---

---Deals damage to a target character (by GameID).
---@param targetId integer
---@param amount integer
---@return boolean success
function Damage(targetId, amount) end

---Deals damage to whatever character is standing at hex (q, r).
---@param q integer
---@param r integer
---@param amount integer
---@return boolean success
function DamageAt(q, r, amount) end

---Kills the given character immediately.
---@param charId integer
function Die(charId) end

---Applies (or refreshes/stacks) an aura on a target.
---@param ownerId integer
---@param targetId integer
---@param auraId integer
---@param stacks integer
---@return boolean success
function ApplyAura(ownerId, targetId, auraId, stacks) end

---Drops stacks of (or whole) aura on a target.
---@param charId integer
---@param auraId integer
---@param stacks integer
function DropAura(charId, auraId, stacks) end

---Applies a tile effect to the tile at hex (q, r).
---@param ownerId integer
---@param q integer
---@param r integer
---@param tileEffectId integer
---@param duration integer
---@return boolean success
function ApplyTileEffect(ownerId, q, r, tileEffectId, duration) end

---Modifies a character's stat by an additive and/or multiplicative delta.
---@param charId integer
---@param statName string  -- must match a StatType name, e.g. "AttackPower", "AutoAttackAP"
---@param addDelta number
---@param mulDelta number
---@return boolean success  -- false if statName is unrecognized
function ModifyStat(charId, statName, addDelta, mulDelta) end

---Stores a numeric value on an aura instance, keyed by name — useful for
---snapshotting values applied on OnApply so OnDrop can reverse them exactly.
---@param wearerId integer
---@param auraId integer
---@param key string
---@param value number
function SetAuraParam(wearerId, auraId, key, value) end

---Reads back a previously-stored aura parameter. Returns 0 if not set.
---@param wearerId integer
---@param auraId integer
---@param key string
---@return number
function GetAuraParam(wearerId, auraId, key) end

---Spends a normal or fast action.
---@param charId integer
---@param fast boolean
function SpendAction(wearerId, auraId, key) end

--- OTHER ---

---Returns a uniform random float in [min, max].
---@param min number
---@param max number
---@return number
function Random(min, max) end

---Returns true with probability `chance` (0..1).
---@param chance number
---@return boolean
function Roll(chance) end

---Writes a line to the game log.
---@param text string
function Log(text) end

--- GETTERS ---

---True if the currently executing effect is a direct (non-triggered) effect.
---@type boolean
IsDirect = nil

---Checks whether the currently executing effect's Trigger flags include the given trigger.
---@param trigger EffectTrigger
---@return boolean
function HasTrigger(trigger) end

---Returns the GameID of the character standing at hex (q, r), or -1 if none.
---@param q integer
---@param r integer
---@return integer
function GetCharacterAt(q, r) end

---@param charId integer
---@return integer
function GetHP(charId) end

---@param charId integer
---@return number
function GetAttackPower(charId) end

---@param charId integer
---@return number
function GetSpellPower(charId) end

---@param charId integer
---@return number
function GetAutoAttackAP(charId) end

---@param charId integer
---@return number
function GetAutoAttackSP(charId) end

---@param charId integer
---@return number
function GetDefense(charId) end

---@param charId integer
---@return number
function GetCritChance(charId) end

---@param charId integer
---@return Hex
function GetPosition(charId) end

---@param charId integer
---@return boolean
function IsAlive(charId) end

---@param charId integer
---@return boolean
function IsRooted(charId) end

---@param charId integer
---@return boolean
function IsStunned(charId) end

---@param charId integer
---@return boolean
function IsDisarmed(charId) end

---@param charId integer
---@return boolean
function IsSilenced(charId) end

---@param fromId integer
---@param toId integer
---@return boolean
function HasLineOfSight(fromId, toId) end

--- TYPES ---

---@class EffectTrigger
---@field OnApply EffectTrigger
---@field OnDrop EffectTrigger
---@field OnStartTurn EffectTrigger
---@field OnEndTurn EffectTrigger
---@field OnDeath EffectTrigger
---@field OnDamageDone EffectTrigger
---@field OnDamageTaken EffectTrigger
---@field OnHealingDone EffectTrigger
---@field OnHealingTaken EffectTrigger
---@field OnSpellCast EffectTrigger
---@field OnMove EffectTrigger
---@field OnDropStack EffectTrigger
---@field OnApplyOtherAura EffectTrigger
---@field OnApplyTileEffect EffectTrigger
---@field OnOtherAuraApplied EffectTrigger
---@field OnMoveFirstTime EffectTrigger
---@field OnCrit EffectTrigger
---@field OnPreDamageDealt EffectTrigger
---@field OnAutoAttack EffectTrigger
EffectTrigger = {}

--- CONTEXT GLOBALS ---
-- Set fresh before each script run by LuaEffectRunner.Run(). Not all are present
-- every run — Caster/Targets only exist when the context is a SpellCastContext.

---GameID of the character that triggered this effect.
---@type integer
Invoker = nil

---GameID of the spell's caster (SpellCastContext only).
---@type integer
Caster = nil

---Explicit target hexes for a spell cast (SpellCastContext only).
---@type Hex[]
Targets = nil

--- RESULT REPORTING ---
-- Call exactly one of these at the end of your script.

---Marks this script run as successful.
function Success() end

---Marks this script run as failed, with a message (surfaced to the acting client).
---@param message string
function Fail(message) end