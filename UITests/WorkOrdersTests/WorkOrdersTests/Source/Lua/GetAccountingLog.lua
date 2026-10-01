-- Функция для безопасного получения имени объекта (учитывает и name, и Name)
local function getName(obj)
    if obj == nil then
        return nil
    end
    -- Сначала проверяем строчное 'name', как в вашем примере
    if obj.name ~= nil then
        return tostring(obj.name)
    end
    -- Затем заглавное 'Name', если строчного нет
    if obj.Name ~= nil then
        return tostring(obj.Name)
    end
    return nil
end

-- Получаем все объекты типа WorkOrderExecutionJournal
local workOrders = snapshot.GetObjects("WorkOrderExecutionJournal")

-- Проверка: если ошибка или nil
if not workOrders then
    print("Ошибка: snapshot.GetObjects вернула nil.")
    return
end

-- Проверка: если массив пустой
if #workOrders == 0 then
    print("Нет объектов типа WorkOrderExecutionJournal.")
    return
end

-- Выбираем случайный индекс от 1 до количества объектов
local randomIndex = math.random(1, #workOrders)
local randomWorkOrder = workOrders[randomIndex]

-- Получаем имя случайного объекта
local randomName = getName(randomWorkOrder)

if randomName ~= nil then
    print("Случайный WorkOrderExecutionJournal с именем: " .. randomName)
    out.AddRecord(randomName)
    return randomName
else
    print("Объект WorkOrderExecutionJournal найден, но у него нет поля name/Name.")
    return nil
end