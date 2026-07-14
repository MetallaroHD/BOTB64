attacker = Invoker
target = Targets[1]

baseDamage = GetAttackPower(attacker) * GetAutoAttackAP(attacker)
           + GetSpellPower(attacker) * GetAutoAttackSP(attacker)

DamageAt(target.Q, target.R, math.floor(baseDamage))