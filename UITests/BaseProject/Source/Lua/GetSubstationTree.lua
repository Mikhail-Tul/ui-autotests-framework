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

local names = {}
local visited = {}
local current = targetSubstation
local depth = 0
local maxDepth = 1000

while current ~= nil and depth < maxDepth do
    if visited[current] then
        break
    end

    visited[current] = true

    local objectName = getName(current)

    if objectName ~= nil then
        table.insert(names, objectName)
    end

    current = current.ParentObject
    depth = depth + 1
end

local reversedNames = {}

for i = #names, 1, -1 do
    table.insert(reversedNames, names[i])
    print(names[i])
    out.AddRecord(names[i])
end

return table.concat(reversedNames, ", ")