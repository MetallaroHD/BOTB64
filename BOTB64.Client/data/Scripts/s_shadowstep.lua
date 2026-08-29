-- Shadow Step - teleports Akano to the closest available tile next to the targeted
-- enemy, then generates 2 combo points.

local target = GetCharacterAt(Targets[1].Q, Targets[1].R)

if target < 0 or not IsEnemy(Invoker, target) then
    Fail("No valid target!")
    return
end

local targetPos = GetPosition(target)
local myPos = GetPosition(Invoker)
local directions = {{0,1},{1,0},{1,-1},{0,-1},{-1,0},{-1,1}}

local candidates = {}
for i = 1, 6 do
    local nq = targetPos.Q + directions[i][1]
    local nr = targetPos.R + directions[i][2]
    if GetCharacterAt(nq, nr) < 0 then
        local dist = HexDistance(myPos.Q, myPos.R, nq, nr)
        table.insert(candidates, {nq, nr, dist})
    end
end

table.sort(candidates, function(a, b) return a[3] < b[3] end)

local moved = false
for i = 1, #candidates do
    if ForceMove(Invoker, candidates[i][1], candidates[i][2]) then
        moved = true
        break
    end
end

if not moved then
    Fail("No available tile near target!")
    return
end

ModifyResource(Invoker, 2)
Success()
