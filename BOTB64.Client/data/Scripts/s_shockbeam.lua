-- Shock Beam - hits all enemies in a beam between Oliver and the Hotspot (inclusive)

local shockwaveAura = 9
local hq = GetAuraParam(Invoker, shockwaveAura, "Q")
local hr = GetAuraParam(Invoker, shockwaveAura, "R")
local pos = GetPosition(Invoker)

local line = GetLine(pos.Q, pos.R, hq, hr)

for i = 1, #line do
    local h = line[i]
    local target = GetCharacterAt(h.Q, h.R)
    if target >= 0 and IsEnemy(Invoker, target) then
        Damage(target, 65)
    end
end

Success()
