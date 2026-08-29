-- Firelord - on death, every Scorched Earth zone on the board transforms into
-- Ashen Earth.

local scorchedEarthId = 9
local ashenEarthId = 10

local positions = FindTileEffectPositions(scorchedEarthId)
for i = 1, #positions do
    local h = positions[i]
    RemoveTileEffect(h.Q, h.R, scorchedEarthId)
    ApplyTileEffect(Invoker, h.Q, h.R, ashenEarthId, 6)
end

Success()
