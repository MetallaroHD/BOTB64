-- Death Mark expired - all shurikens fly toward the marked target (Invoker here is
-- the marked character, since this runs on OnDrop from the wearer's perspective),
-- stopping if their path is blocked by a tile that blocks line of sight, then
-- detonate for damage scaled by how many actually reached the target.

local shurikenId = 5
local casterID = GetAuraParam(Invoker, 13, "CasterID")
local targetPos = GetPosition(Invoker)
local positions = FindTileEffectPositions(shurikenId)
local hitCount = 0

for i = 1, #positions do
    local h = positions[i]
    local line = GetLine(h.Q, h.R, targetPos.Q, targetPos.R)
    local blocked = false

    for j = 2, #line do
        if TileBlocksLos(line[j].Q, line[j].R) then
            blocked = true
            break
        end
    end

    RemoveTileEffect(h.Q, h.R, shurikenId)
    if not blocked then
        hitCount = hitCount + 1
    end
end

if hitCount > 0 and casterID >= 0 then
    DamageAs(casterID, Invoker, 15 * hitCount)
end

Success()
