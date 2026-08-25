-- Scoper root bonus damage - on pre damage
-- ID 4

local target = Targets[1]
local targetChar = GetCharacterAt(target.Q, target.R)

if IsRooted(targetChar) then
    Damage(targetChar, 55)
end
Success()