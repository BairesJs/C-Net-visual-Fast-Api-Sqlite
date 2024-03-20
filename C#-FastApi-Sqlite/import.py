import csv
import sqlite3

# Conexión a la base de datos SQLite
conn = sqlite3.connect('db1.sqlite')
cursor = conn.cursor()

# Crear tabla si no existe
cursor.execute('''CREATE TABLE IF NOT EXISTS clientes (
                    id INTEGER PRIMARY KEY,
                    nombre TEXT,
                    direccion TEXT,
                    telefono TEXT,
                    email TEXT
                )''')

# Abrir el archivo CSV y leer los datos
with open('db.csv', 'r') as file:
    csv_reader = csv.reader(file)
    next(csv_reader)  # Omitir la primera fila si contiene encabezados
    # Insertar cada fila en la base de datos
    for row in csv_reader:
        cursor.execute('INSERT INTO clientes (id, nombre, direccion, telefono, email) VALUES (?, ?, ?, ?, ?)', row)

# Confirmar los cambios y cerrar la conexión
conn.commit()
conn.close()
