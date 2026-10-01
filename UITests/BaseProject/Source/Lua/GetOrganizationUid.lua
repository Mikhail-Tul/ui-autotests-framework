-- Входной параметр (имя человека)
local targetPersonName = "${personName}"

-- Функция для получения имени объекта (универсальная)
local function getName(obj)
    if obj == nil then return nil end
    if obj.name ~= nil then return tostring(obj.name) end
    if obj.Name ~= nil then return tostring(obj.Name) end
    return nil
end

-- 1. Получаем всех людей
local persons = snapshot.GetObjects("Person")

if not persons or #persons == 0 then
    print("Ошибка: Объекты типа Person не найдены.")
    return
end

-- 2. Ищем конкретного человека по имени
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

-- 3. Идем по цепочке связей до Организации
local department = targetPerson.Department
if department == nil then
    print("У Person не найдена связь Department.")
    return
end

local headDepartment = department.HeadDepartment
if headDepartment == nil then
    print("У Department не найдена связь HeadDepartment.")
    return
end

local organisation = headDepartment.Organisation
if organisation == nil then
    print("У HeadDepartment не найдена связь Organisation.")
    return
end

-- 4. Получаем UID организации
-- Проверяем оба варианта написания поля: uid или UID
local orgUid = organisation.uid or organisation.UID

if orgUid == nil then
    print("У объекта Organisation не найден UID.")
    return
end

-- Результат
print("Для человека " .. targetPersonName .. " найден Organization UID: " .. tostring(orgUid))
out.AddRecord(tostring(orgUid))