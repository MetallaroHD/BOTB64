-- Helicopter Move - damages all enemies at exactly range 2 and applies Head Trauma.

local headTraumaId = 28
local pos = GetPosition(Invoker)
local dmg = 15 + 0.7 * GetAttackPower(Invoker)
local hexes = GetHexesInRadius(pos.Q, pos.R, 2)

for i = 1, #hexes do
    local h = hexes[i]
    if HexDistance(pos.Q, pos.R, h.Q, h.R) == 2 then
        local target = GetCharacterAt(h.Q, h.R)
        if target >= 0 and IsEnemy(Invoker, target) then
            Damage(target, math.floor(dmg))
            ApplyAura(Invoker, target, headTraumaId, 1)
        end
    end
end

Success()
