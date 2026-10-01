local function getName(obj)
    if obj == nil then return nil end
    if obj.name ~= nil then return tostring(obj.name) end
    if obj.Name ~= nil then return tostring(obj.Name) end
    return nil
end

-- 1. Получаем объекты
local categories = snapshot.GetObjects("DjCategoryType")

if not categories or #categories == 0 then
    print("Объекты не найдены")
    return
end

local total = #categories
local countToTake = 3
if total < 3 then 
    countToTake = total 
end

-- 2. Создаем список индексов (чтобы не было повторов)
local indices = {}
for i = 1, total do
    indices[i] = i
end

-- 3. Выбираем случайные
for i = 1, countToTake do
    -- Используем только базовый math.random
    local randPos = math.random(1, #indices)
    local objIdx = indices[randPos]
    
    local obj = categories[objIdx]
    local name = getName(obj)
    
    if name ~= nil then
        print("Random " .. i .. ": " .. name)
        out.AddRecord(name)
    end
    
    -- Удаляем индекс, чтобы не выбрать его снова
    table.remove(indices, randPos)
end