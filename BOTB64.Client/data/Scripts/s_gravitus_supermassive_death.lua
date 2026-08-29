-- Supermassive - upon death, attracts all enemies toward Gravitus's position. Anyone
-- who is stopped by a wall while being pulled takes damage.

local pos = GetPosition(Invoker)
local dmg = 25 + 0.8 * GetSpellPower(Invoker)
local pullAmount = 5

local ids = GetAllCharacterIDs()
for i = 1, #ids do
    local id = ids[i]
    if IsAlive(id) and IsEnemy(Invoker, id) then
        local hitWall = PullToward(id, pos.Q, pos.R, pullAmount)
        if hitWall then
            Damage(id, math.floor(dmg))
        end
    end
end

Success()
