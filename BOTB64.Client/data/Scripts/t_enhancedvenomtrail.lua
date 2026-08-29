-- Enhanced Venom Trail - inflicts 3 stacks of Crab Venom on whoever steps on it or
-- starts their turn on it.

local crabVenomId = 18
local enhancedId = 8
local pos = GetPosition(Invoker)
local ownerID = GetTileEffectParam(pos.Q, pos.R, enhancedId, "OwnerID")

if ownerID >= 0 and IsEnemy(ownerID, Invoker) then
    ApplyAura(ownerID, Invoker, crabVenomId, 3)
end

Success()
