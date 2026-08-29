-- Berserker Rage - upon death, all allies gain Attack Power/Spell Power based on how
-- much Rage Pilaf had when he died.

local legacyBuffId = 30
local rage = GetResource(Invoker)
if rage <= 0 then
    Success()
    return
end

local ids = GetAllCharacterIDs()
for i = 1, #ids do
    local id = ids[i]
    if IsAlive(id) and not IsEnemy(Invoker, id) then
        ApplyAura(Invoker, id, legacyBuffId, rage)
    end
end

Success()
