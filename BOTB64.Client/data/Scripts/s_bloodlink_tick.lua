-- Bloodlink - at the start of Trappus's own turn, recompute the Bloodwell threshold
-- buff (10% AP/SP at 50+, 25% at 100+) for both Trappus and the linked ally. Only
-- runs from Trappus's own copy of the aura (Invoker check below).

local bloodlinkId = 16
local trappusId = GetAuraParam(Invoker, bloodlinkId, "TrappusID")

if Invoker ~= trappusId then
    Success()
    return
end

local allyId = GetAuraParam(Invoker, bloodlinkId, "AllyID")
local bloodwell = GetResource(Invoker)

local targetTier = 0
if bloodwell >= 100 then
    targetTier = 2
elseif bloodwell >= 50 then
    targetTier = 1
end

local currentTier = GetAuraParam(Invoker, bloodlinkId, "Tier")

if targetTier ~= currentTier then
    local oldPct = 0
    if currentTier == 1 then oldPct = 0.10 elseif currentTier == 2 then oldPct = 0.25 end
    local newPct = 0
    if targetTier == 1 then newPct = 0.10 elseif targetTier == 2 then newPct = 0.25 end
    local delta = newPct - oldPct

    ModifyStat(Invoker, "AttackPower", 0, delta)
    ModifyStat(Invoker, "SpellPower", 0, delta)
    ModifyStat(allyId, "AttackPower", 0, delta)
    ModifyStat(allyId, "SpellPower", 0, delta)

    SetAuraParam(Invoker, bloodlinkId, "Tier", targetTier)
end

Success()
