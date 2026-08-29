-- Bloodlink - links Trappus to an ally (max 1 at a time). Casting on a different
-- ally moves the link, reverting any active tier buff on the old pairing first.

local bloodlinkId = 16
local newAlly = GetCharacterAt(Targets[1].Q, Targets[1].R)

if newAlly < 0 or IsEnemy(Invoker, newAlly) then
    Fail("Must target an ally!")
    return
end

local hasAlly = GetAuraParam(Invoker, bloodlinkId, "HasAlly")
if hasAlly > 0 then
    local oldAlly = GetAuraParam(Invoker, bloodlinkId, "AllyID")
    if oldAlly ~= newAlly then
        local tier = GetAuraParam(Invoker, bloodlinkId, "Tier")
        if tier > 0 then
            local pct = 0
            if tier == 1 then pct = 0.10 elseif tier == 2 then pct = 0.25 end
            ModifyStat(Invoker, "AttackPower", 0, -pct)
            ModifyStat(Invoker, "SpellPower", 0, -pct)
            ModifyStat(oldAlly, "AttackPower", 0, -pct)
            ModifyStat(oldAlly, "SpellPower", 0, -pct)
        end
        DropAura(oldAlly, bloodlinkId, 1)
        DropAura(Invoker, bloodlinkId, 1)
        SetAuraParam(Invoker, bloodlinkId, "Tier", 0)
    end
end

ApplyAura(Invoker, Invoker, bloodlinkId, 1)
ApplyAura(Invoker, newAlly, bloodlinkId, 1)
SetAuraParam(Invoker, bloodlinkId, "TrappusID", Invoker)
SetAuraParam(Invoker, bloodlinkId, "AllyID", newAlly)
SetAuraParam(Invoker, bloodlinkId, "HasAlly", 1)
SetAuraParam(newAlly, bloodlinkId, "TrappusID", Invoker)
SetAuraParam(newAlly, bloodlinkId, "AllyID", newAlly)
SetAuraParam(newAlly, bloodlinkId, "HasAlly", 1)

Success()
