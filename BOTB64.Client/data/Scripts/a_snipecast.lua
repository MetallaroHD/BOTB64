-- Scoper snipe cast - on start turn
-- ID 7

local target = GetAuraParam(Invoker, 5, "CastTarget")
local shouldFire = IsAlive(target) and not IsStunned(Invoker) and not IsSilenced(Invoker) and HasLineOfSight(Invoker, target)

if shouldFire then
    Damage(target, 2.3 * GetAttackPower(Invoker))
    SpendAction(Invoker, false)
end

Success()