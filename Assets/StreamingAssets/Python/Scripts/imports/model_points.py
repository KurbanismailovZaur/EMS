from settings import DB_PATH

import sqlite3

CRITERION = 10 ** 6

# Создаем соединение с нашей базой данных
conn = sqlite3.connect(DB_PATH)
# Создаем курсор - это специальный объект который делает запросы и получает их результаты
cursor = conn.cursor()

cursor.execute("DELETE FROM ModelPoint")
conn.commit()

print('=> чтение данных...')

with open('../data_import/PointsAndMaterials.txt', 'r') as f:
    data = {}
    for i, line in enumerate(f):
        if i == 0:
            continue
        plane = line.strip().replace(',', '.').split(';')
        points = (float(x) for x in plane[:-1])
        material_id = int(plane[-1])
        # таким способом мы избавляемся от дубликатов плоскостей - запишется последняя плоскость
        data[points] = material_id

        if i % CRITERION == 0:
            print(f'==> Обработанно {i // CRITERION} мил.')

    for key, value in data.items():
        item = list(key) + [value]
        # Делаем INSERT запрос к базе данных, используя обычный SQL-синтаксис
        cursor.execute(f"INSERT into ModelPoint values ({', '.join(str(x) for x in item)}) ")

    # Сохранение транзакции
    conn.commit()

conn.close()

print('=> данные успешно импортированы!')
