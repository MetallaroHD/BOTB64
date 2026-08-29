-- Berserker Rage - keeps the Attack Power/Crit bonus in sync with current Rage.
-- Called from OnAutoAttack (after gaining Rage), OnSpellCast (after any Rage-spending
-- spell), and OnStartTurn (safety net) - there is no direct "resource changed"
-- trigger, so this diffs against the last-applied amount and re-scales from scratch.

local berserkerRageId = 29
local apPerRage = 0.5
local critPerRage = 0.002

local rage = GetResource(Invoker)
local prevRage = GetAuraParam(Invoker, berserkerRageId, "AppliedRage")

if rage ~= prevRage then
    ModifyStat(Invoker, "AttackPower", -apPerRage * prevRage, 0)
    ModifyStat(Invoker, "Crit", -critPerRage * prevRage, 0)
    ModifyStat(Invoker, "AttackPower", apPerRage * rage, 0)
    ModifyStat(Invoker, "Crit", critPerRage * rage, 0)
    SetAuraParam(Invoker, berserkerRageId, "AppliedRage", rage)
end

Success()
