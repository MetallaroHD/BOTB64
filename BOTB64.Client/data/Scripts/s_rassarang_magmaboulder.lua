-- Magma Boulder - costs HP. Hurls a boulder in a line up to 6 tiles (stopping at a
-- wall), damaging anyone it passes and leaving Scorched Earth in its path. If it hits
-- a wall, Scorched Earth also spreads to all tiles adjacent to the last passable tile
-- before the wall (not the wall tile itself, which can't hold a tile effect anyway).
--
-- TargetingType.BeamWall already walked this exact path (wall-stopped, range-clamped)
-- to build the preview the player saw, so Targets *is* the path - consume it directly
-- rather than re-deriving a line from Targets[1] (which is just the nearest tile, not
-- an aim endpoint, since BeamWall fills Targets with the whole route).

local scorchedEarthId = 9
local hpCost = 50
local dmg = 30 + 1.2 * GetSpellPower(Invoker)

PayHealthCost(Invoker, hpCost)

for i = 1, #Targets do
    local h = Targets[i]

    ApplyTileEffect(Invoker, h.Q, h.R, scorchedEarthId, 3)
    SetTileEffectParam(h.Q, h.R, scorchedEarthId, "OwnerID", Invoker)
    SetTileEffectParam(h.Q, h.R, scorchedEarthId, "Damage", dmg)

    local occupant = GetCharacterAt(h.Q, h.R)
    if occupant >= 0 and IsEnemy(Invoker, occupant) then
        Damage(occupant, math.floor(dmg))
    end
end

-- Figure out whether the path actually ended at a wall (as opposed to just running out
-- of range) by checking one more step past the last tile, in the same direction the
-- path was already travelling.
if #Targets > 0 then
    local pos = GetPosition(Invoker)
    local last = Targets[#Targets]
    local prev = (#Targets >= 2) and Targets[#Targets - 1] or pos
    local dq = last.Q - prev.Q
    local dr = last.R - prev.R
    local wallQ = last.Q + dq
    local wallR = last.R + dr

    if IsWall(wallQ, wallR) then
        local directions = {{0,1},{1,0},{1,-1},{0,-1},{-1,0},{-1,1}}
        for i = 1, 6 do
            local nq = last.Q + directions[i][1]
            local nr = last.R + directions[i][2]
            ApplyTileEffect(Invoker, nq, nr, scorchedEarthId, 3)
            SetTileEffectParam(nq, nr, scorchedEarthId, "OwnerID", Invoker)
            SetTileEffectParam(nq, nr, scorchedEarthId, "Damage", dmg)
        end
    end
end

Success()
