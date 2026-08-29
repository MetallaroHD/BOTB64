-- Scorched Earth - damages whoever is on it, at turn start or when they step on it.
-- Damage is attributed to whoever placed it (stored at placement time), not the
-- character standing on the tile.

local scorchedEarthId = 9
local pos = GetPosition(Invoker)
local ownerID = GetTileEffectParam(pos.Q, pos.R, scorchedEarthId, "OwnerID")
local dmg = GetTileEffectParam(pos.Q, pos.R, scorchedEarthId, "Damage")

if ownerID >= 0 and IsEnemy(ownerID, Invoker) then
    DamageAs(ownerID, Invoker, dmg)
end

Success()
