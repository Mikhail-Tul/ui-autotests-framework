-- Имя типа категории, которое вы передаете (замените на нужное или используйте переменную системы)
local targetCategoryTypeName = "${CategoryTypeName}" 

local function getName(obj)
    if obj == nil then return nil end
    if obj.name ~= nil then return tostring(obj.name) end
    if obj.Name ~= nil then return tostring(obj.Name) end
    return nil
end

-- 1. Находим нужный DjCategoryType
local allTypes = snapshot.GetObjects("DjCategoryType")
local targetType = nil

if allTypes then
    for i = 1, #allTypes do
        if getName(allTypes[i]) == targetCategoryTypeName then
            targetType = allTypes[i]
            break
        end
    end
end

if targetType == nil then
    print("DjCategoryType с именем '" .. tostring(targetCategoryTypeName) .. "' не найден.")
    return
end

-- 2. Получаем список связанных категорий через связь 'Categories'
local categories = targetType.Categories

if not categories or #categories == 0 then
    print("У данного типа нет связанных категорий (DjCategory).")
    return
end

-- 3. Выбираем 2 случайных имени из списка
local total = #categories
local countToTake = 2
if total < 2 then 
    countToTake = total 
end

-- Создаем список индексов, чтобы избежать повторов
local indices = {}
for i = 1, total do
    indices[i] = i
end

for i = 1, countToTake do
    -- Базовый случайный выбор
    local randPos = math.random(1, #indices)
    local objIdx = indices[randPos]
    
    local categoryObj = categories[objIdx]
    local categoryName = getName(categoryObj)
    
    if categoryName ~= nil then
        print("Random Category " .. i .. ": " .. categoryName)
        out.AddRecord(categoryName)
    end
    
    -- Удаляем индекс, чтобы не выбрать одну и ту же категорию дважды
    table.remove(indices, randPos)
end