local targetSubstationName = "${substationName}"

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

if targetSubstationName == nil or targetSubstationName == "" then
    print("Не задано имя Substation.")
    return
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

local targetSubstation = nil

for i = 1, #substations do
    if getName(substations[i]) == targetSubstationName then
        targetSubstation = substations[i]
        break
    end
end

if targetSubstation == nil then
    print("Не найден Substation с name = '" .. tostring(targetSubstationName) .. "'.")
    return
end

local voltageLevels = targetSubstation.VoltageLevels

if not voltageLevels then
    print("У Substation не найдена связь VoltageLevels.")
    return
end

if #voltageLevels == 0 then
    print("Список VoltageLevels пуст.")
    return
end

local randomIndex = math.random(#voltageLevels)

local randomVoltageLevel = voltageLevels[randomIndex]

if randomVoltageLevel == nil then
    print("Не удалось получить случайную VoltageLevel.")
    return
end

local voltageLevelName = getName(randomVoltageLevel)

if voltageLevelName == nil then
    print("У случайной VoltageLevel не найдено поле name или Name.")
    return
end

print(voltageLevelName)

out.AddRecord(voltageLevelName)

return voltageLevelName