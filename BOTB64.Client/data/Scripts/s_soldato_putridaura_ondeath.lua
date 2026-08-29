-- Putrid Aura - on death, Soldatosolitario explodes and creates a Venom Trail in a
-- radius of 3.

local venomTrailId = 7
local pos = GetPosition(Invoker)
local hexes = GetHexesInRadius(pos.Q, pos.R, 3)

for i = 1, #hexes do
    local h = hexes[i]
    local target = GetCharacterAt(h.Q, h.R)
    if target >= 0 and IsEnemy(Invoker, target) then
        Damage(target, 60)
    end
    ApplyTileEffect(Invoker, h.Q, h.R, venomTrailId, 25)
    SetTileEffectParam(h.Q, h.R, venomTrailId, "OwnerID", Invoker)
end

Success()
