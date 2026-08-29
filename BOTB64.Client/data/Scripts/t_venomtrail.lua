-- Venom Trail - inflicts 1 stack of Crab Venom on whoever steps on it or starts
-- their turn on it.

local crabVenomId = 18
local venomTrailId = 7
local pos = GetPosition(Invoker)
local ownerID = GetTileEffectParam(pos.Q, pos.R, venomTrailId, "OwnerID")

if ownerID >= 0 and IsEnemy(ownerID, Invoker) then
    ApplyAura(ownerID, Invoker, crabVenomId, 1)
end

Success()
