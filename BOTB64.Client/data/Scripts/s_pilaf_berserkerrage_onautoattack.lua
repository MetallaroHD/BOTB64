-- Berserker Rage - gains Rage based on the target's missing health when Pilaf
-- attacks.

local target = GetCharacterAt(Targets[1].Q, Targets[1].R)
if target >= 0 and IsEnemy(Invoker, target) then
    local maxHp = GetMaxHP(target)
    local curHp = GetHP(target)
    if maxHp > 0 then
        local missingFrac = (maxHp - curHp) / maxHp
        local gain = math.floor(20 * missingFrac)
        if gain > 0 then
            ModifyResource(Invoker, gain)
        end
    end
end

Success()
