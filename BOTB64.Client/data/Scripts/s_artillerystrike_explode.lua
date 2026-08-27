-- Artillery Charging - detonates around the Hotspot's current position

local shockwaveAura = 9
local hq = GetAuraParam(Invoker, shockwaveAura, "Q")
local hr = GetAuraParam(Invoker, shockwaveAura, "R")

local hexes = GetHexesInRadius(hq, hr, 3)
for i = 1, #hexes do
    local h = hexes[i]
    local target = GetCharacterAt(h.Q, h.R)
    if target >= 0 and IsEnemy(Invoker, target) then
        Damage(target, 130)
    end
end

Success()
