-- Black Hole - pulls all enemies within range toward its position every round.
-- Runs on OnRoundStart, which fires once per tile effect regardless of occupancy,
-- with Invoker set to the black hole's owner - so position comes from the Position
-- global (the tile's own position), not GetPosition(Invoker).

local blackHoleId = 13
local pos = Position
local ownerID = GetTileEffectParam(pos.Q, pos.R, blackHoleId, "OwnerID")
local pullRadius = 4
local pullAmount = 1

local hexes = GetHexesInRadius(pos.Q, pos.R, pullRadius)
for i = 1, #hexes do
    local h = hexes[i]
    local target = GetCharacterAt(h.Q, h.R)
    if target >= 0 and IsAlive(target) and IsEnemy(ownerID, target) then
        PullToward(target, pos.Q, pos.R, pullAmount)
    end
end

Success()
