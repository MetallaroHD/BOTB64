-- Venom Trail - walking leaves behind a Venom Trail tile; while the "wide" activation
-- is running, it also spreads to all adjacent tiles.

local venomTrailId = 7
local soldatoAuraId = 20
local pos = GetPosition(Invoker)

ApplyTileEffect(Invoker, pos.Q, pos.R, venomTrailId, 25)
SetTileEffectParam(pos.Q, pos.R, venomTrailId, "OwnerID", Invoker)

local wideTurns = GetAuraParam(Invoker, soldatoAuraId, "WideTurns")
if wideTurns > 0 then
    local directions = {{0,1},{1,0},{1,-1},{0,-1},{-1,0},{-1,1}}
    for i = 1, 6 do
        local nq = pos.Q + directions[i][1]
        local nr = pos.R + directions[i][2]
        ApplyTileEffect(Invoker, nq, nr, venomTrailId, 25)
        SetTileEffectParam(nq, nr, venomTrailId, "OwnerID", Invoker)
    end
end

Success()
