-- ПЕРЕМЕННЫЕ-ЗАПОЛНИТЕЛИ (подставляются системой)
local targetPersonName = "${personName}"
local targetShiftRoleName = "${shiftRoleName}"
local targetShiftName = "${shiftName}"

-- Функция для безопасного получения имени объекта
local function getName(obj)
    if obj == nil then return nil end
    if obj.name ~= nil then return tostring(obj.name) end
    if obj.Name ~= nil then return tostring(obj.Name) end
    return nil
end

-- Функция для поиска UID объекта по имени
local function findUidByName(objectType, targetName)
    local objects = snapshot.GetObjects(objectType)
    
    if not objects then
        print("Ошибка: snapshot.GetObjects вернула nil для типа " .. objectType)
        return nil
    end

    for i = 1, #objects do
        local obj = objects[i]
        if getName(obj) == targetName then
            return obj.uid
        end
    end
    
    print("Объект типа " .. objectType .. " с именем '" .. tostring(targetName) .. "' не найден.")
    return nil
end

-- Поиск UID в строго заданном порядке
local shiftRoleUid = findUidByName("ShiftRole", targetShiftRoleName)
local shiftUid     = findUidByName("Shift", targetShiftName)
local personUid    = findUidByName("Person", targetPersonName)

-- Проверяем, что все три UID найдены
if shiftRoleUid and shiftUid and personUid then
    
    -- Выводим каждый UID как отдельный объект/запись в систему
    -- Используем tostring(), чтобы избежать ошибки userdata
    
    out.AddRecord(tostring(shiftRoleUid))
    out.AddRecord(tostring(shiftUid))
    out.AddRecord(tostring(personUid))
    
    print("Все UID успешно добавлены в записи по порядку: ShiftRole, Shift, Person")
    return true
else
    print("Ошибка: один или несколько объектов не найдены. Записи не созданы.")
    return nil
end