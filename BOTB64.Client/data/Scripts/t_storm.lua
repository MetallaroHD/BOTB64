-- Storm terrain - damages whoever is on it, at turn start or when they step on it.
-- Damage is attributed to whoever cast Storm (stored at placement time), not the
-- character standing on the tile.

local stormTileEffect = 4
local pos = GetPosition(Invoker)
local ownerID = GetTileEffectParam(pos.Q, pos.R, stormTileEffect, "OwnerID")
local dmg = GetTileEffectParam(pos.Q, pos.R, stormTileEffect, "Damage")

if ownerID >= 0 and IsEnemy(ownerID, Invoker) then
    DamageAs(ownerID, Invoker, dmg)
end

Success()
