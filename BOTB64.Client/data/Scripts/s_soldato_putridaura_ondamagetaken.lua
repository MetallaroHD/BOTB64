-- Putrid Aura - physical attacks against Soldatosolitario inflict 2 stacks of Crab
-- Venom on the attacker. Invoker here is Soldato (the defender); Attacker is who hit him.

local crabVenomId = 18

if DamageType == EffectDamageType.Physical and Attacker ~= nil then
    ApplyAura(Invoker, Attacker, crabVenomId, 2)
end

Success()
