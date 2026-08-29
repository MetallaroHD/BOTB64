-- Thunder Beam - big Spell Power scaling damage in a straight line

local dmg = 40 + 1.8 * GetSpellPower(Invoker)

for i = 1, #Targets do
    local h = Targets[i]
    local target = GetCharacterAt(h.Q, h.R)
    if target >= 0 and IsEnemy(Invoker, target) then
        Damage(target, dmg)
    end
end

Success()
