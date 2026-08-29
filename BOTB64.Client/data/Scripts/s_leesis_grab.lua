-- Grab - pulls a distant enemy to exactly range 2 from Lee Sis and slows them. If
-- they're already at range 2 or closer, only the slow applies (no reposition).

local grabSlowId = 26
local pos = GetPosition(Invoker)
local targetPos = Targets[1]
local target = GetCharacterAt(targetPos.Q, targetPos.R)

if target < 0 or not IsEnemy(Invoker, target) then
    Fail("Must target an enemy!")
    return
end

local dist = HexDistance(pos.Q, pos.R, targetPos.Q, targetPos.R)
if dist > 2 then
    local line = GetLine(pos.Q, pos.R, targetPos.Q, targetPos.R)
    local dest = line[3]
    ForceMove(target, dest.Q, dest.R)
end

ApplyAura(Invoker, target, grabSlowId, 1)

Success()
