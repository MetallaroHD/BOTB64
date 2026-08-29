-- Firelord - toggles a flat Spell Power buff on/off depending on whether Rassarang is
-- currently standing on Scorched Earth. Checked both when she moves and at the start
-- of her own turn (covers standing still while a zone appears/expires under her).

local firelordId = 21
local scorchedEarthId = 9
local buffAmount = 25

local pos = GetPosition(Invoker)
local standing = HasTileEffect(pos.Q, pos.R, scorchedEarthId)
local active = GetAuraParam(Invoker, firelordId, "Active")

if standing and active <= 0 then
    ModifyStat(Invoker, "SpellPower", buffAmount, 0)
    SetAuraParam(Invoker, firelordId, "Active", 1)
elseif not standing and active > 0 then
    ModifyStat(Invoker, "SpellPower", -buffAmount, 0)
    SetAuraParam(Invoker, firelordId, "Active", 0)
end

Success()
