-- Unstable Gravity - when a tethered target takes damage, the other end is healed if
-- it's Gravitus's ally, or damaged if it's his enemy. Same logic as Gravity Tether,
-- kept as a separate script so it reads its own aura ID's parameters.

local unstableTetherId = 37
local partnerId = GetAuraParam(Invoker, unstableTetherId, "PartnerID")
local gravitusId = GetAuraParam(Invoker, unstableTetherId, "GravitusID")

if partnerId >= 0 and IsAlive(partnerId) then
    if IsEnemy(gravitusId, partnerId) then
        Damage(partnerId, DamageAmount)
    else
        Heal(partnerId, DamageAmount)
    end
end

Success()
