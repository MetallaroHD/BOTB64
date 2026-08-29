-- Exposed - OnDrop: restore the Defense/Magic Defense snapshotted on apply.

local auraId = 12
local def = GetAuraParam(Invoker, auraId, "Def")
local mdef = GetAuraParam(Invoker, auraId, "MDef")

ModifyStat(Invoker, "Defense", def, 0)
ModifyStat(Invoker, "MagicDefense", mdef, 0)

Success()
