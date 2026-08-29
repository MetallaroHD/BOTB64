-- Long Staff Statue - attacks all enemies within range 2 every round. Runs on
-- OnRoundStart, which fires once per tile effect regardless of tile occupancy, with
-- Invoker set to the statue's owner (Lee Sis, even though she's dead) - so position
-- must come from the Position global (the tile's own position), not GetPosition(Invoker).

local statueId = 14
local pos = Position
local ownerID = GetTileEffectParam(pos.Q, pos.R, statueId, "OwnerID")
local dmg = GetTileEffectParam(pos.Q, pos.R, statueId, "Damage")

local hexes = GetHexesInRadius(pos.Q, pos.R, 2)
for i = 1, #hexes do
    local h = hexes[i]
    local target = GetCharacterAt(h.Q, h.R)
    if target >= 0 and IsAlive(target) and IsEnemy(ownerID, target) then
        DamageAs(ownerID, target, dmg)
    end
end

Success()
