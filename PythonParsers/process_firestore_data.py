import json
from datetime import datetime

def process_firestore_data(input_file, output_file):
    try:
        # Abrir y cargar el archivo JSON
        with open(input_file, 'r', encoding='utf-8') as f:
            data = json.load(f)

        # Array para almacenar los documentos procesados
        processed_data = []

        # Iterar sobre los documentos de nivel superior
        for doc_id, doc_content in data.items():
            # Agregar el campo "_id"
            doc_content["_id"] = doc_id

            # Convertir el campo "date" a "drawingAt" en formato UTC
            if "date" in doc_content:
                try:
                    doc_content["drawingAt"] = datetime.strptime(doc_content["date"], "%Y/%m/%d").strftime("%Y-%m-%dT%H:%M:%SZ")
                    del doc_content["date"]
                except ValueError:
                    print(f"Error: El formato de la fecha '{doc_content['date']}' no es válido.")
                    continue

            # Procesar cualquier campo con "__datatype__"
            def process_datatype(obj):
                # Si es un diccionario, buscar "__datatype__"
                if isinstance(obj, dict):
                    if "__datatype__" in obj:
                        if obj["__datatype__"] == "documentReference" and "value" in obj:
                            # Tomar la última parte del valor después del '/'
                            return obj["value"].split("/")[-1]
                        elif "value" in obj:
                            return obj["value"]  # Reemplazar con el valor de "value"
                    else:
                        # Recorrer recursivamente el diccionario
                        return {key: process_datatype(value) for key, value in obj.items()}
                # Si es una lista, procesar cada elemento
                elif isinstance(obj, list):
                    return [process_datatype(item) for item in obj]
                else:
                    return obj  # Devolver tal cual si no aplica

            # Aplicar la función de procesamiento al documento
            doc_content = process_datatype(doc_content)

            # Eliminar el campo "__collections__" si existe
            if "__collections__" in doc_content:
                del doc_content["__collections__"]

            # Añadir el documento procesado al array
            processed_data.append(doc_content)

        # Guardar los datos procesados en el archivo de salida
        with open(output_file, 'w', encoding='utf-8') as f:
            json.dump(processed_data, f, indent=4, ensure_ascii=False)

        print(f"Datos procesados y guardados en: {output_file}")

    except FileNotFoundError:
        print(f"Error: No se encontró el archivo de entrada '{input_file}'.")
    except json.JSONDecodeError:
        print("Error: El archivo de entrada no contiene un JSON válido.")
    except Exception as e:
        print(f"Error inesperado: {e}")

# Archivo de entrada y salida
input_file = "inspirations_firestore.json"
output_file = "inspirations_mongo.json"

# Ejecutar la función de procesamiento
process_firestore_data(input_file, output_file)