-- Death Storm - marks an enemy for death; the payoff happens on expiration, in
-- s_deathmark_expire.lua (run by the Death Mark aura's OnDrop trigger).

local target = GetCharacterAt(Targets[1].Q, Targets[1].R)

if target < 0 or not IsEnemy(Invoker, target) then
    Fail("No valid target!")
    return
end

ApplyAura(Invoker, target, 13, 1)
SetAuraParam(target, 13, "CasterID", Invoker)
Success()
