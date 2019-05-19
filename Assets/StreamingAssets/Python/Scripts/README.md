## Install requirements
Python 3.7 и выше
```
pip install -r requirements.txt
```

## Описание скрипа
```
usage: main.py [-h] [-db DB_PATH] [-xlsx XLSX_PATH] [--create_figures] [--script_m3] [--script_m2] [--script_report]

optional arguments:
  -h, --help        show this help message and exit
  -db DB_PATH       Путь к файлу базы данных
  -xlsx XLSX_PATH   Путь к файлу отчета
  --create_figures  Преобразование плоскостей в фигуры
  --script_m3       Расчет напряженности электрического поля в заданных точках
  --script_m2       Расчет взаимного воздействия кабелей БКС и БА на БКС
  --script_report   Генерация отчета
```

## Run
1: Импорт модели и формирование фигур
```
python main.py -db "./db/ems.bytes" --create_figures
```

2: Расчет напряженности электрического поля в заданных точках
```
python main.py -db "./db/ems.bytes" --script_m3
```

3: Расчет взаимного воздействия кабелей БКС и БА на БКС
```
python main.py -db "./db/ems.bytes" --script_m2
```

4: Генерация отчета
```
python main.py -db "./db/ems.bytes" -xlsx "report.xlsx" --script_report
```