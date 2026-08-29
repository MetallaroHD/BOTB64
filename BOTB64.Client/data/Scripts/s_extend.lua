-- Extend - selects a Venom Trail tile and extends it to all adjacent tiles

local venomTrailId = 7
local pos = Targets[1]

if not HasTileEffect(pos.Q, pos.R, venomTrailId) then
    Fail("Must target a Venom Trail tile!")
    return
end

local directions = {{0,1},{1,0},{1,-1},{0,-1},{-1,0},{-1,1}}
for i = 1, 6 do
    local nq = pos.Q + directions[i][1]
    local nr = pos.R + directions[i][2]
    ApplyTileEffect(Invoker, nq, nr, venomTrailId, 25)
    SetTileEffectParam(nq, nr, venomTrailId, "OwnerID", Invoker)
end

Success()
