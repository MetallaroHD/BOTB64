-- Shockwave (passive) - on death, damages every enemy within range 3

local pos = GetPosition(Invoker)
local hexes = GetHexesInRadius(pos.Q, pos.R, 3)

for i = 1, #hexes do
    local h = hexes[i]
    local target = GetCharacterAt(h.Q, h.R)
    if target >= 0 and IsEnemy(Invoker, target) then
        Damage(target, 90)
    end
end

Success()
