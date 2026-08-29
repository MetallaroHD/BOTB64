-- Blood Blossom - every round, heals allies within 2 tiles by the amount of Bloodwell
-- Trappus had when he died (Invoker here is the blossom's Owner, since this fires
-- OnRoundStart via ProcessWorldTick).

local bloodBlossomId = 6
local pos = Position
local healAmt = GetTileEffectParam(pos.Q, pos.R, bloodBlossomId, "HealAmount")

if healAmt > 0 then
    local hexes = GetHexesInRadius(pos.Q, pos.R, 2)
    for i = 1, #hexes do
        local h = hexes[i]
        local target = GetCharacterAt(h.Q, h.R)
        if target >= 0 and not IsEnemy(Invoker, target) then
            Heal(target, healAmt)
        end
    end
end

Success()
