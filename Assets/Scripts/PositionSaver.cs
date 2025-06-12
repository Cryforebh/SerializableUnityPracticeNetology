using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DefaultNamespace
{
    public class PositionSaver : MonoBehaviour
    {
        // Структура Data хранит позицию и время. Это позволяет сохранять историю позиций объекта.
        [System.Serializable]
        public struct Data
        {
            public Vector3 Position;
            public float Time;
        }

        // Объявляется переменная _json типа TextAsset, которая будет использоваться для хранения JSON данных.
        [SerializeField, ReadOnly, Tooltip("Для заполнения используйте контекстное меню → Create File")]
        private TextAsset _json;							// _json — это ссылка на текстовый ассет, который содержит JSON-данные с сохранёнными позициями.

        [field: SerializeField, HideInInspector]
        public List<Data> Records { get; private set; }		// Список Records хранит все сохранённые позиции. Он инициализируется в методе Awake.

        private void Awake()
        {
            //todo comment: Что будет, если в теле этого условия не сделать выход из метода?
            // Если условие не выполнится и метод продолжится, Unity попытается загрузить JSON данные, что может привести к ошибке, если _json действительно null.

            if (_json == null)                              // Если _json не установлен, объект деактивируется и выводится ошибка.
            {
                gameObject.SetActive(false);
                Debug.LogError("Please, create TextAsset and add in field _json");
                return;
            }

            // Происходит десерилизация JSON в объект. Если Records равен null, создаётся новый список на 10 элементов.
            JsonUtility.FromJsonOverwrite(_json.text, this);

            //todo comment: Для чего нужна эта проверка (что она позволяет избежать)?
            // Проверка if (Records == null) позволяет избежать попытки работы с нулевым списком, что может вызвать исключение.

            if (Records == null)
                Records = new List<Data>(10);
        }

        private void OnDrawGizmos()
        {
            //todo comment: Зачем нужны эти проверки (что они позволляют избежать)?
            // Проверки в OnDrawGizmos позволяют избежать рисования объектов, если данные отсутствуют или их недостаточно,
            // что предотвращает ошибки и улучшает производительность.

            if (Records == null || Records.Count == 0) return;	// Проверяется наличие и количество записей в Records. Если их нет, метод завершает работу.
            var data = Records;
            var prev = data[0].Position;                        // prev инициализируется первой позицией из списка.
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(prev, 0.3f);					// Рисуется первая позиция как сфера зеленого цвета.

            //todo comment: Почему итерация начинается не с нулевого элемента?
            // Итерация начинается с 1, потому что первая позиция уже была обработана и отображена как отдельная сфера.

            for (int i = 1; i < data.Count; i++)                // Для каждой последующей позиции рисуется сфера и линия, соединяющая текущую и предыдущую позиции.
            {
                var curr = data[i].Position;
                Gizmos.DrawWireSphere(curr, 0.3f);
                Gizmos.DrawLine(prev, curr);
                prev = curr;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Create File")]
        private void CreateFile()
        {
            //todo comment: Что происходит в этой строке?
            // Создается файл с именем “Path.txt” в папке проекта.

            var stream = File.Create(Path.Combine(Application.dataPath, "Path.txt"));

            //todo comment: Подумайте для чего нужна эта строка? (а потом проверьте догадку, закомментировав) 
            // Dispose()
            // Для освобождения всех ресурсов.
            // Что означает завершение любых текущих операций чтения или записи.
            // Все занятые ресурсы освобождаются, что позволяет системе перераспределить их для других целей.
            // Другими словами: чтобы избежать утечек памяти и обеспечить корректное освобождение системных ресурсов, stream будет отчищен, а созданный файл Path.txt останется.

            stream.Dispose();

            // UnityEditor.AssetDatabase.Refresh();
            // Это метод в Unity, который используется для обновления базы данных ассетов. Этот метод инициирует процесс обновления, который включает в себя несколько шагов:
            // Поиск изменений: Unity проверяет наличие изменений в файлах ассетов.
            // Обновление базы данных: Обновляются записи в базе данных ассетов для новых или изменённых файлов. Удаляются записи для удалённых файлов.
            // Импорт и компиляция: Импортируются и компилируются связанные с кодом файлы, такие как .dll, .asmdef, .asmref, .rsp и .cs файлы.
            // Перезагрузка домена: Если были изменения в скриптах, Unity перезагружает C# домен, чтобы учесть новые имплементации импортеров.
            // Пост-обработка ассетов: Происходит пост-обработка импортированных ассетов, особенно связанных с кодом.
            // Импорт не-кодовых ассетов: Импортируются и обрабатываются оставшиеся ассеты, такие как текстуры, модели и другие ресурсы.
            // Горячая перезагрузка: Изменения в скриптах и ассетах применяются без необходимости перезапуска редактора или приложения.
            // Этот метод является мощным инструментом для управления изменениями в проекте Unity, обеспечивая актуальность всех ассетов и их корректную обработку.
            UnityEditor.AssetDatabase.Refresh();

            //В Unity можно искать объекты по их типу, для этого используется префикс "t:"
            //После нахождения, Юнити возвращает массив гуидов (которые в мета-файлах задаются, например)

            var guids = UnityEditor.AssetDatabase.FindAssets("t:TextAsset");
            foreach (var guid in guids)
            {
                //Этой командой можно получить путь к ассету через его гуид
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                //Этой командой можно загрузить сам ассет
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);

                //todo comment: Для чего нужны эти проверки?
                // Убедится что ассет был успешно загружен, после чего обеспечить правильное сохранение и отображение изменений

                if (asset != null && asset.name == "Path")
                {
                    _json = asset;
                    UnityEditor.EditorUtility.SetDirty(this);
                    UnityEditor.AssetDatabase.SaveAssets();
                    UnityEditor.AssetDatabase.Refresh();

                    //todo comment: Почему мы здесь выходим, а не продолжаем итерироваться?
                    // Выход из метода происходит, чтобы предотвратить дальнейшие проверки и обновления, если файл с нужным именем уже был найден и загружен.

                    return;
                }
            }
        }

        private void OnDestroy()
        {
            // Сериализуем данные
            string jsonData = JsonUtility.ToJson(this, true);

            // Получаем путь к файлу _json 
            string path = UnityEditor.AssetDatabase.GetAssetPath(_json);

            // Записываем данные в файл
            System.IO.File.WriteAllText(path, jsonData);

            // Обновляем AssetDatabase, чтобы Unity увидел изменения
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }
}