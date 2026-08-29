-- Flame Pillar - invisible trap: explodes on the first step, dealing damage and
-- covering the tile and all neighbors in Scorched Earth. Single-use - removes itself
-- once triggered.

local flamePillarId = 11
local scorchedEarthId = 9
local pos = GetPosition(Invoker)
local ownerID = GetTileEffectParam(pos.Q, pos.R, flamePillarId, "OwnerID")
local dmg = GetTileEffectParam(pos.Q, pos.R, flamePillarId, "Damage")

if ownerID >= 0 and IsEnemy(ownerID, Invoker) then
    DamageAs(ownerID, Invoker, dmg)
end

RemoveTileEffect(pos.Q, pos.R, flamePillarId)

ApplyTileEffect(ownerID, pos.Q, pos.R, scorchedEarthId, 3)
SetTileEffectParam(pos.Q, pos.R, scorchedEarthId, "OwnerID", ownerID)
SetTileEffectParam(pos.Q, pos.R, scorchedEarthId, "Damage", dmg)

local directions = {{0,1},{1,0},{1,-1},{0,-1},{-1,0},{-1,1}}
for i = 1, 6 do
    local nq = pos.Q + directions[i][1]
    local nr = pos.R + directions[i][2]
    ApplyTileEffect(ownerID, nq, nr, scorchedEarthId, 3)
    SetTileEffectParam(nq, nr, scorchedEarthId, "OwnerID", ownerID)
    SetTileEffectParam(nq, nr, scorchedEarthId, "Damage", dmg)
end

Success()
