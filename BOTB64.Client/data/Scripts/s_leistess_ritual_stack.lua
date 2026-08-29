-- Thunder Ritual - every autoattack permanently gains 4 Spell Power (stacks indefinitely)

local auraId = 11
local stacks = GetAuraParam(Invoker, auraId, "Stacks") + 1
SetAuraParam(Invoker, auraId, "Stacks", stacks)
ModifyStat(Invoker, "SpellPower", 4, 0)

Success()
