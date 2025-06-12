using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

// Это наш "Стикер - Не вскрывать!" (Атрибут) для "Почтовых писем" (Переменных)
// Атрибуты — это такие “стикеры” для кода. Они не меняют логику класса или переменной, но дают подсказки движку, как с ними работать.

/// <summary>
/// Атрибут, для блокирования модификации сериализуемых полей через инспектор ( МОЙ!)) )
/// </summary>

public class ReadOnlyAttribute : PropertyAttribute { }

// Создаем доступ к редактору атрибута через #if UNITY_EDITOR,
// чтобы не создавать отдельный скрипт в кастомной папке Editor.
// Код редактора должен лежать в папке Editor, иначе Unity его проигнорирует. Но мы это обошли.

#if UNITY_EDITOR

// Говорим Unity, что это редактор для атрибута ReadOnly
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    // Для старой отрисовки через IMGUI
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Сохраняем предыдущее состояние GUI
        bool wasEnabled = GUI.enabled;

        // Отключаем редактирование
        GUI.enabled = false;

        // Рисуем поле как обычно, но теперь оно неактивно
        EditorGUI.PropertyField(position, property, label);

        // Восстанавливаем состояние GUI
        GUI.enabled = wasEnabled;
    }

    // Для новой отрисовки через UIElements
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // 1. Создаём базовый элемент (как обычное поле в инспекторе)
        var element = base.CreatePropertyGUI(property)

        // 2. Если базовый элемент не создался (null), создаём новый PropertyField
        ?? new UnityEditor.UIElements.PropertyField(property);

        // 3. Делаем элемент неактивным
        element.SetEnabled(false);

        // 4. Возвращаем элемент, чтобы Unity его отрисовал
        return element;
    }
}

#endif