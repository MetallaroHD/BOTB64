-- Berserker's Legacy - grants Attack Power/Spell Power scaling with stacks (Pilaf's
-- Rage at the time of his death).

local legacyBuffId = 30
local perStackAP = 0.3
local perStackSP = 0.3

local prevStacks = GetAuraParam(Invoker, legacyBuffId, "AppliedStacks")
if prevStacks > 0 then
    ModifyStat(Invoker, "AttackPower", -perStackAP * prevStacks, 0)
    ModifyStat(Invoker, "SpellPower", -perStackSP * prevStacks, 0)
end

local stacks = GetAuraStacks(Invoker, legacyBuffId)
ModifyStat(Invoker, "AttackPower", perStackAP * stacks, 0)
ModifyStat(Invoker, "SpellPower", perStackSP * stacks, 0)
SetAuraParam(Invoker, legacyBuffId, "AppliedStacks", stacks)

Success()
