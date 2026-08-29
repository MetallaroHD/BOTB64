-- Fiery Call - clears nearby Scorched Earth, gaining a Spell Power/Defense buff for
-- 2 turns that scales with how many zones were removed.

local scorchedEarthId = 9
local fierycallBuffId = 23
local radius = 3

local pos = GetPosition(Invoker)
local hexes = GetHexesInRadius(pos.Q, pos.R, radius)
local removed = 0

for i = 1, #hexes do
    local h = hexes[i]
    if HasTileEffect(h.Q, h.R, scorchedEarthId) then
        RemoveTileEffect(h.Q, h.R, scorchedEarthId)
        removed = removed + 1
    end
end

if removed > 0 then
    ApplyAura(Invoker, Invoker, fierycallBuffId, removed)
end

Success()
