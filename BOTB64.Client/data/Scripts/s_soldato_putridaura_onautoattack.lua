-- Putrid Aura - Soldatosolitario's autoattacks inflict 1 stack of Crab Venom on the
-- target.

local crabVenomId = 18
local target = GetCharacterAt(Targets[1].Q, Targets[1].R)

if target >= 0 and IsEnemy(Invoker, target) then
    ApplyAura(Invoker, target, crabVenomId, 1)
end

Success()
