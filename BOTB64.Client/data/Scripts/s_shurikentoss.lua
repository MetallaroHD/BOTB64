-- Shuriken Toss - throws 3 shurikens (targeter already resolved the center + 2
-- flanking tiles), each dealing Attack Power damage and granting 1 combo point per
-- enemy hit, then leaving a shuriken statue behind.

local shurikenId = 5
local dmg = 1.0 * GetAttackPower(Invoker)

for i = 1, #Targets do
    local h = Targets[i]
    local target = GetCharacterAt(h.Q, h.R)
    if target >= 0 and IsEnemy(Invoker, target) then
        Damage(target, dmg)
        ModifyResource(Invoker, 1)
    end
    ApplyTileEffect(Invoker, h.Q, h.R, shurikenId, 20)
end

Success()
