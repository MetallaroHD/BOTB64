-- Generic apply/drop routine

local applydrop = {}

function applydrop.run(charID, statName, add, mul)
    if HasTrigger(EffectTrigger.OnApply) then
        if not ModifyStat(charID, statName, add, mul) then
            return false
        end
    else
        if not ModifyStat(charID, statName, add, mul) then
            return false
        end
    end
    return true
end

return applydrop