-- Survival Instinct - damages and knocks back all adjacent enemies.

local pos = GetPosition(Invoker)
local dmg = 20 + 0.8 * GetAttackPower(Invoker)
local directions = {{0,1},{1,0},{1,-1},{0,-1},{-1,0},{-1,1}}

for i = 1, 6 do
    local nq = pos.Q + directions[i][1]
    local nr = pos.R + directions[i][2]
    local target = GetCharacterAt(nq, nr)
    if target >= 0 and IsEnemy(Invoker, target) then
        Damage(target, math.floor(dmg))
        local destQ = nq + directions[i][1]
        local destR = nr + directions[i][2]
        ForceMove(target, destQ, destR)
    end
end

Success()
