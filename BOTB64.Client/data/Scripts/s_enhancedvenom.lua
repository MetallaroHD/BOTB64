-- Enhanced Venom - converts existing Venom Trail tiles in the area into Enhanced
-- Venom Trail (applies 3 stacks instead of 1). Terrain-type tile effects are
-- singleton-per-tile, so applying the enhanced version automatically replaces the
-- regular one.

local venomTrailId = 7
local enhancedId = 8

for i = 1, #Targets do
    local h = Targets[i]
    if HasTileEffect(h.Q, h.R, venomTrailId) then
        ApplyTileEffect(Invoker, h.Q, h.R, enhancedId, 25)
        SetTileEffectParam(h.Q, h.R, enhancedId, "OwnerID", Invoker)
    end
end

Success()
