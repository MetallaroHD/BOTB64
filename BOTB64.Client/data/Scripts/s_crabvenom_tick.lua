-- Crab Venom - deals damage at the start of the wearer's turn, scaling with stacks

local crabVenomId = 18
local stacks = GetAuraStacks(Invoker, crabVenomId)

if stacks > 0 then
    Damage(Invoker, stacks * 4)
end

Success()
