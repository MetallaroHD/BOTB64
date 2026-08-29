-- Cataclysm - deals Spell Power damage to all enemies at the start of each of
-- Rassarang's turns while active.

local dmg = 20 + 0.8 * GetSpellPower(Invoker)
local ids = GetAllCharacterIDs()

for i = 1, #ids do
    local id = ids[i]
    if IsAlive(id) and IsEnemy(Invoker, id) then
        Damage(id, math.floor(dmg))
    end
end

Success()
