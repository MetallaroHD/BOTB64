-- Ashen Earth - refreshes a short Ashen Slow debuff on whoever is standing on it, at
-- turn start or when they step on it. The debuff's own short duration means it fades
-- naturally a couple of turns after they leave.

local ashenEarthId = 10
local ashenSlowId = 22
local pos = GetPosition(Invoker)
local ownerID = GetTileEffectParam(pos.Q, pos.R, ashenEarthId, "OwnerID")

if ownerID >= 0 and IsEnemy(ownerID, Invoker) then
    ApplyAura(ownerID, Invoker, ashenSlowId, 1)
end

Success()
