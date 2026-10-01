local function getName(obj)
    if obj == nil then
        return nil
    end

    if obj.name ~= nil then
        return tostring(obj.name)
    end

    if obj.Name ~= nil then
        return tostring(obj.Name)
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

local randomIndex = math.random(#substations)

local randomSubstation = substations[randomIndex]

if randomSubstation == nil then
    print("Не удалось получить случайный Substation.")
    return
end

local substationName = getName(randomSubstation)

if substationName == nil then
    print("У случайного Substation не найдено поле name или Name.")
    return
end

print("Случайный Substation: " .. substationName)

out.AddRecord(substationName)

return substationName