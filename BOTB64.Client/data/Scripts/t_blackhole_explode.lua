-- Black Hole - explodes when an enemy touches it, dealing damage in a radius, then
-- collapses.

local blackHoleId = 13
local pos = GetPosition(Invoker)
local ownerID = GetTileEffectParam(pos.Q, pos.R, blackHoleId, "OwnerID")

if ownerID < 0 or not IsEnemy(ownerID, Invoker) then
    Success()
    return
end

local dmg = GetTileEffectParam(pos.Q, pos.R, blackHoleId, "Damage")
local hexes = GetHexesInRadius(pos.Q, pos.R, 2)

RemoveTileEffect(pos.Q, pos.R, blackHoleId)

for i = 1, #hexes do
    local h = hexes[i]
    local target = GetCharacterAt(h.Q, h.R)
    if target >= 0 and IsAlive(target) and IsEnemy(ownerID, target) then
        DamageAs(ownerID, target, dmg)
    end
end

Success()
