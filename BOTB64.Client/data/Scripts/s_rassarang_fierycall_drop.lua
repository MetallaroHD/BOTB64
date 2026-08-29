-- Fiery Call Buff - removes the Spell Power/Defense bonus when the buff expires.

local buffId = 23
local perStackSP = 8
local perStackDef = 4

local appliedStacks = GetAuraParam(Invoker, buffId, "AppliedStacks")
ModifyStat(Invoker, "SpellPower", -perStackSP * appliedStacks, 0)
ModifyStat(Invoker, "Defense", -perStackDef * appliedStacks, 0)

Success()
