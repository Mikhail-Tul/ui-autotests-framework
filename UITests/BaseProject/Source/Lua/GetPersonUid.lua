local targetPersonName = "${personName}"

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

local function getUid(obj)
    if obj == nil then
        return nil
    end
    
    -- Проверяем возможные варианты названия поля с UID
    if obj.uid ~= nil then
        return tostring(obj.uid)
    end
    
    if obj.Id ~= nil then
        return tostring(obj.Id)
    end
    
    if obj.UID ~= nil then
        return tostring(obj.UID)
    end

    return nil
end

local persons = snapshot.GetObjects("Person")

if not persons then
    print("Ошибка: snapshot.GetObjects вернула nil.")
    return
end

if #persons == 0 then
    print("Нет объектов типа Person.")
    return
end

local targetPerson = nil

for i = 1, #persons do
    if getName(persons[i]) == targetPersonName then
        targetPerson = persons[i]
        break
    end
end

if targetPerson == nil then
    print("Не найден Person с name = '" .. tostring(targetPersonName) .. "'.")
    return
end

-- Получаем UID найденного Person
local personUid = getUid(targetPerson)

if personUid == nil then
    print("Ошибка: Не удалось извлечь UID из найденного объекта Person.")
    return
end

print("Найден Person: " .. tostring(getName(targetPerson)))
print("UID Person: " .. personUid)

-- Возвращаем только UID
out.AddRecord(personUid)