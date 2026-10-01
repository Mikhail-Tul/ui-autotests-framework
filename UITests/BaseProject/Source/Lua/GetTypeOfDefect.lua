-- Функция для безопасного получения имени (как в вашем примере)
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

-- Получаем все объекты типа DefectKind
local defects = snapshot.GetObjects("DefectKind")

-- Проверка: вернул ли метод что-нибудь
if not defects then
    print("Ошибка: snapshot.GetObjects вернула nil.")
    return
end

-- Проверка: есть ли в списке объекты
if #defects == 0 then
    print("Объекты типа DefectKind не найдены.")
    return
end

-- Определяем, сколько объектов мы можем взять (3 или меньше, если их всего 1-2)
local countToTake = math.min(3, #defects)

print("Обработка первых " .. tostring(countToTake) .. " объектов DefectKind:")

for i = 1, countToTake do
    local currentDefect = defects[i]
    local defectName = getName(currentDefect)

    if defectName ~= nil then
        print("Defect " .. i .. ": " .. defectName)
        -- Добавляем в результат (согласно вашему примеру)
        out.AddRecord(defectName)
    else
        print("Объект под индексом " .. i .. " не имеет имени.")
    end
end