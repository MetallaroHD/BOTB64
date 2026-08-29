-- Gravity Tether - when a tethered target takes damage, the other end is healed if
-- it's Gravitus's ally, or damaged if it's his enemy.

local tetherId = 33
local partnerId = GetAuraParam(Invoker, tetherId, "PartnerID")
local gravitusId = GetAuraParam(Invoker, tetherId, "GravitusID")

if partnerId >= 0 and IsAlive(partnerId) then
    if IsEnemy(gravitusId, partnerId) then
        Damage(partnerId, DamageAmount)
    else
        Heal(partnerId, DamageAmount)
    end
end

Success()
