-- 1. Исправляем функцию: переименовываем в getGuid, так как мы ищем ID
local function getGuid(obj)
    if obj == nil then
        return nil
    end

    -- Проверяем разные варианты написания поля UID
    if obj.uid ~= nil then
        return tostring(obj.uid)
    end

    if obj.UID ~= nil then -- Исправлено на UID (обычно в системах либо uid, либо UID)
        return tostring(obj.UID)
    end

    if obj.Uid ~= nil then
        return tostring(obj.Uid)
    end

    return nil
end

local substations = snapshot.GetObjects("Substation")

if not substations then
    print("Ошибка: snapshot.GetObjects вернула nil.")
    return
end

if #substations == 0 then
    print("Нет объектов типа Substation.")
    return
end

-- Выбираем случайный индекс
local randomIndex = math.random(1, #substations)
local randomSubstation = substations[randomIndex]

if randomSubstation == nil then
    print("Не удалось получить случайный Substation.")
    return
end

-- 2. Теперь вызываем функцию getGuid, которая определена выше
local substationUid = getGuid(randomSubstation)

if substationUid == nil then
    print("У случайного Substation не найдено поле uid или UID.")
    return
end

print("Случайный Substation UID: " .. substationUid)

-- Добавляем в запись и возвращаем значение
out.AddRecord(substationUid)

return substationUid