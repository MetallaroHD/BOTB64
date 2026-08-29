-- Exposed - OnApply: snapshot current Defense/Magic Defense and zero them out.
-- Invoker here is the wearer (the exposed enemy), not Akano.

local auraId = 12
local def = GetDefense(Invoker)
local mdef = GetMagicDefense(Invoker)

SetAuraParam(Invoker, auraId, "Def", def)
SetAuraParam(Invoker, auraId, "MDef", mdef)
ModifyStat(Invoker, "Defense", -def, 0)
ModifyStat(Invoker, "MagicDefense", -mdef, 0)

Success()
