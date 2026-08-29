-- Thunder Bolt - basic direct damage to an enemy, scaling on Spell Power

local target = GetCharacterAt(Targets[1].Q, Targets[1].R)

if target < 0 then
    Fail("No target!")
    return
end

Damage(target, 20 + 1.2 * GetSpellPower(Invoker))
Success()
