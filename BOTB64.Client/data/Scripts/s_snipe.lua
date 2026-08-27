-- Snipe - Scoper takes aim; the shot fires at the start of her next turn
-- (see a_snipecast.lua, run by the "Taking Aim" aura, ID 7)

local target = GetCharacterAt(Targets[1].Q, Targets[1].R)

if target < 0 then
    Fail("No target!")
    return
end

ApplyAura(Invoker, Invoker, 7, 1)
SetAuraParam(Invoker, 7, "CastTarget", target)
Success()
