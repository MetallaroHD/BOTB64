-- Berserker's Legacy - removes the Attack Power/Spell Power bonus.

local legacyBuffId = 30
local perStackAP = 0.3
local perStackSP = 0.3

local appliedStacks = GetAuraParam(Invoker, legacyBuffId, "AppliedStacks")
ModifyStat(Invoker, "AttackPower", -perStackAP * appliedStacks, 0)
ModifyStat(Invoker, "SpellPower", -perStackSP * appliedStacks, 0)

Success()
