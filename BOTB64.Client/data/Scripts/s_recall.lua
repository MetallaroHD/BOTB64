-- Recall - removes all shuriken statues from the field; each hits everything in a
-- line back to Akano, scaling on Attack Power, and grants 1 combo point per enemy hit.

local shurikenId = 5
local dmg = 1.0 * GetAttackPower(Invoker)
local pos = GetPosition(Invoker)
local positions = FindTileEffectPositions(shurikenId)

for i = 1, #positions do
    local h = positions[i]
    RemoveTileEffect(h.Q, h.R, shurikenId)

    local line = GetLine(h.Q, h.R, pos.Q, pos.R)
    for j = 1, #line do
        local t = line[j]
        local target = GetCharacterAt(t.Q, t.R)
        if target >= 0 and IsEnemy(Invoker, target) then
            Damage(target, dmg)
            ModifyResource(Invoker, 1)
        end
    end
end

Success()
