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

local chain = {
    organisation,
    headDepartment,
    department
}

print("Найден Person: " .. tostring(getName(targetPerson)))
print("Вывод от последнего к начальному:")

for i = 1, #chain do
    local objectName = getName(chain[i])

    if objectName ~= nil then
        print(objectName)
        out.AddRecord(objectName)
    end
end