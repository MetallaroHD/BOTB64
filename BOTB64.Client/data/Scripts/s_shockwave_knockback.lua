-- Shockwave (passive) - at the start of every turn, knock back adjacent enemies by 1 tile

local directions = {{0,1},{1,0},{1,-1},{0,-1},{-1,0},{-1,1}}
local pos = GetPosition(Invoker)

for i = 1, 6 do
    local dq, dr = directions[i][1], directions[i][2]
    local nq, nr = pos.Q + dq, pos.R + dr
    local target = GetCharacterAt(nq, nr)
    if target >= 0 and IsEnemy(Invoker, target) then
        ForceMove(target, nq + dq, nr + dr)
    end
end

Success()
