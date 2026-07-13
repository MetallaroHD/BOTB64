attacker = Attacker
target = Target

baseDamage = GetAttackPower(attacker) * GetAutoAttackAP(attacker)
           + GetSpellPower(attacker) * GetAutoAttackSP(attacker)
           - GetAutoAttackDef(attacker) * GetDefense(target)
           - GetAutoAttackMDef(attacker) * GetAutoAttackMDef(target)

crit = RollChance(GetCritChance(attacker))
if crit then
    baseDamage = baseDamage * 1.5
end

Damage(target, math.floor(baseDamage), crit)

if GetHP(target) <= 0 then
    Die(target)
end