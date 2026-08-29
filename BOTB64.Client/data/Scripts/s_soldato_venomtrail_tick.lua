-- Venom Trail - counts down the "wide trail" activation window at the start of
-- Soldato's own turn.

local soldatoAuraId = 20
local wideTurns = GetAuraParam(Invoker, soldatoAuraId, "WideTurns")

if wideTurns > 0 then
    SetAuraParam(Invoker, soldatoAuraId, "WideTurns", wideTurns - 1)
end

Success()
