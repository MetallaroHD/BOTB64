-- Putrid Aura (Activate) - inflicts 1 stack of Crab Venom on every enemy that
-- already has at least one stack.

local crabVenomId = 18
local ids = GetAllCharacterIDs()

for i = 1, #ids do
    local id = ids[i]
    if IsAlive(id) and IsEnemy(Invoker, id) and GetAuraStacks(id, crabVenomId) > 0 then
        ApplyAura(Invoker, id, crabVenomId, 1)
    end
end

Success()
