-- Thunder Ritual - on death, transfers all stacks (and their SP) to the nearest ally.
-- If she has 0 stacks, nothing transfers.

local auraId = 11
local stacks = GetAuraParam(Invoker, auraId, "Stacks")

if stacks > 0 then
    local pos = GetPosition(Invoker)
    local ids = GetAllCharacterIDs()
    local bestId = -1
    local bestDist = -1

    for i = 1, #ids do
        local id = ids[i]
        if id ~= Invoker and IsAlive(id) and not IsEnemy(Invoker, id) then
            local otherPos = GetPosition(id)
            local dist = HexDistance(pos.Q, pos.R, otherPos.Q, otherPos.R)
            if bestId == -1 or dist < bestDist then
                bestId = id
                bestDist = dist
            end
        end
    end

    if bestId >= 0 then
        ApplyAura(Invoker, bestId, auraId, 1)
        local allyStacks = GetAuraParam(bestId, auraId, "Stacks") + stacks
        SetAuraParam(bestId, auraId, "Stacks", allyStacks)
        ModifyStat(bestId, "SpellPower", 4 * stacks, 0)
    end
end

Success()
