-- Singularity Collapse - pays HP to pull every enemy on the board toward Gravitus by
-- several tiles (stopping at walls via PullToward), then damages each based on how
-- close they end up - the closer to Gravitus, the more damage.

local hpCost = 80
local pullAmount = 5
local maxDmg = 40 + 1.2 * GetSpellPower(Invoker)
local minDmg = 10
local falloffPerTile = 8

PayHealthCost(Invoker, hpCost)

local pos = GetPosition(Invoker)
local ids = GetAllCharacterIDs()

for i = 1, #ids do
    local id = ids[i]
    if IsAlive(id) and IsEnemy(Invoker, id) then
        PullToward(id, pos.Q, pos.R, pullAmount)

        local enemyPos = GetPosition(id)
        local dist = HexDistance(pos.Q, pos.R, enemyPos.Q, enemyPos.R)
        local dmg = math.max(minDmg, maxDmg - dist * falloffPerTile)
        Damage(id, math.floor(dmg))
    end
end

Success()
