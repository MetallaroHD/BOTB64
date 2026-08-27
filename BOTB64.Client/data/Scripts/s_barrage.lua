-- Barrage - fires two side beams flanking the Oliver-Hotspot line (no center beam).
-- Each side beam is offset by one hex-neighbor to either side of Oliver and traced
-- parallel to the main line's direction (snapped to the nearest of the 6 canonical
-- hex directions, since hex grids have no exact perpendicular). Side beams stop at
-- the first enemy they hit, or a tile that blocks line of sight.

local directions = {{0,1},{1,0},{1,-1},{0,-1},{-1,0},{-1,1}}

local function dirIndexOf(dq, dr)
    for i = 1, 6 do
        if directions[i][1] == dq and directions[i][2] == dr then
            return i
        end
    end
    return nil
end

local shockwaveAura = 9
local hq = GetAuraParam(Invoker, shockwaveAura, "Q")
local hr = GetAuraParam(Invoker, shockwaveAura, "R")
local pos = GetPosition(Invoker)

local line = GetLine(pos.Q, pos.R, hq, hr)

-- side beams
if #line >= 2 then
    local dq = line[2].Q - line[1].Q
    local dr = line[2].R - line[1].R
    local dist = #line - 1
    local idx = dirIndexOf(dq, dr)

    if idx ~= nil then
        local left = directions[(idx % 6) + 1]
        local right = directions[((idx - 2) % 6 + 6) % 6 + 1]

        local function traceSide(offset)
            local sq = pos.Q + offset[1]
            local sr = pos.R + offset[2]
            for step = 0, dist - 1 do
                local q = sq + dq * step
                local r = sr + dr * step
                if TileBlocksLos(q, r) then
                    break
                end
                local target = GetCharacterAt(q, r)
                if target >= 0 and IsEnemy(Invoker, target) then
                    Damage(target, 45)
                    break
                end
            end
        end

        traceSide(left)
        traceSide(right)
    end
end

Success()
