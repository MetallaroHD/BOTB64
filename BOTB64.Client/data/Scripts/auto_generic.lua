local generic = {}

function generic.run()
    local attacker = Invoker
    local target = Targets[1]

    local baseDamage = GetAttackPower(attacker) * GetAutoAttackAP(attacker)
            + GetSpellPower(attacker) * GetAutoAttackSP(attacker)

    DamageAt(target.Q, target.R, math.floor(baseDamage))
end

return generic