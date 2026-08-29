-- Magma Boulder - costs HP. Hurls a boulder in a line up to 6 tiles (stopping at a
-- wall), damaging anyone it passes and leaving Scorched Earth in its path. If it hits
-- a wall, Scorched Earth also spreads to all tiles adjacent to that wall.

local scorchedEarthId = 9
local hpCost = 50
local dmg = 30 + 1.2 * GetSpellPower(Invoker)

PayHealthCost(Invoker, hpCost)

local pos = GetPosition(Invoker)
local target = Targets[1]
local line = GetLine(pos.Q, pos.R, target.Q, target.R)

local wallQ, wallR = nil, nil
local dist = 0

for i = 2, #line do
    if dist >= 6 then
        break
    end
    local h = line[i]
    dist = dist + 1

    if IsWall(h.Q, h.R) then
        wallQ, wallR = h.Q, h.R
        break
    end

    ApplyTileEffect(Invoker, h.Q, h.R, scorchedEarthId, 3)
    SetTileEffectParam(h.Q, h.R, scorchedEarthId, "OwnerID", Invoker)
    SetTileEffectParam(h.Q, h.R, scorchedEarthId, "Damage", dmg)

    local occupant = GetCharacterAt(h.Q, h.R)
    if occupant >= 0 and IsEnemy(Invoker, occupant) then
        Damage(occupant, math.floor(dmg))
    end
end

if wallQ ~= nil then
    local directions = {{0,1},{1,0},{1,-1},{0,-1},{-1,0},{-1,1}}
    for i = 1, 6 do
        local nq = wallQ + directions[i][1]
        local nr = wallR + directions[i][2]
        ApplyTileEffect(Invoker, nq, nr, scorchedEarthId, 3)
        SetTileEffectParam(nq, nr, scorchedEarthId, "OwnerID", Invoker)
        SetTileEffectParam(nq, nr, scorchedEarthId, "Damage", dmg)
    end
end

Success()
