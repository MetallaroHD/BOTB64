-- Cataclysm - toggle. First cast: stuns Rassarang for up to 3 of her own turns,
-- dealing AoE damage to all enemies at the start of each. Cast again while active to
-- end it early.

local cataclysmId = 24

if GetAuraStacks(Invoker, cataclysmId) > 0 then
    DropAura(Invoker, cataclysmId, 1)
else
    ApplyAura(Invoker, Invoker, cataclysmId, 1)
end

Success()
