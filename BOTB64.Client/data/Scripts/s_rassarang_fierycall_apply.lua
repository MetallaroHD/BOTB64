-- Fiery Call Buff - grants Spell Power/Defense scaling with stacks (number of
-- Scorched Earth zones removed). Re-applying refreshes duration and re-scales from
-- the new total stack count.

local buffId = 23
local perStackSP = 8
local perStackDef = 4

local prevStacks = GetAuraParam(Invoker, buffId, "AppliedStacks")
if prevStacks > 0 then
    ModifyStat(Invoker, "SpellPower", -perStackSP * prevStacks, 0)
    ModifyStat(Invoker, "Defense", -perStackDef * prevStacks, 0)
end

local stacks = GetAuraStacks(Invoker, buffId)
ModifyStat(Invoker, "SpellPower", perStackSP * stacks, 0)
ModifyStat(Invoker, "Defense", perStackDef * stacks, 0)
SetAuraParam(Invoker, buffId, "AppliedStacks", stacks)

Success()
