-- Storm - AoE Spell Power damage, and drops Storm terrain (2 turns) in the affected area

local stormTileEffect = 4
local dmg = 20 + 1.0 * GetSpellPower(Invoker)

for i = 1, #Targets do
    local h = Targets[i]
    ApplyTileEffect(Invoker, h.Q, h.R, stormTileEffect, 2)
    SetTileEffectParam(h.Q, h.R, stormTileEffect, "OwnerID", Invoker)
    SetTileEffectParam(h.Q, h.R, stormTileEffect, "Damage", dmg)

    local target = GetCharacterAt(h.Q, h.R)
    if target >= 0 and IsEnemy(Invoker, target) then
        Damage(target, dmg)
    end
end

Success()
