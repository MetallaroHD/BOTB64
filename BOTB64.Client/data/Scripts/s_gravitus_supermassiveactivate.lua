-- Supermassive - grants a target ally increased Defense and Magic Defense for
-- 3 turns.

local buffId = 38
local target = GetCharacterAt(Targets[1].Q, Targets[1].R)

if target < 0 or IsEnemy(Invoker, target) then
    Fail("Must target an ally!")
    return
end

ApplyAura(Invoker, target, buffId, 1)

Success()
