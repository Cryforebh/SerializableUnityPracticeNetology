using UnityEngine;

namespace DefaultNamespace
{
    // Этот класс реализует систему воспроизведения записанного движения:
    // 1. PositionSaver хранит массив записей (время + позиция).
    // 2. ReplayMover последовательно переключается между записями и интерполирует позицию объекта между ними, создавая плавное движение.

    [RequireComponent(typeof(PositionSaver))]
    public class ReplayMover : MonoBehaviour
    {
        private PositionSaver _save;

        private int _index; // Текущий индекс записи в списке Records
        private PositionSaver.Data _prev; // Предыдущая запись данных
        private float _duration;

        private void Start()
        {
            ////todo comment: зачем нужны эти проверки?
            /// Эта проверка выполняется для того, чтобы убедиться в наличии необходимого компонента и данных для корректной работы скрипта.
			/// !TryGetComponent(out _save): пытается получить компонент PositionSaver и сохранить его в переменную _save.
			/// _save.Records.Count == 0: проверяет, что количество записей в списке Records компонента PositionSaver равно нулю.

            if (!TryGetComponent(out _save) || _save.Records.Count == 0)
            {
                Debug.LogError("Records incorrect value", this);

                //todo comment: Для чего выключается этот компонент?
                // Если проверки не пройдены, компонент перестаёт обновляться в Update(), чтобы избежать ошибок (например, обращения к пустому списку).

                enabled = false; // Отключает компонент
            }
        }

        private void Update()
        {
            var curr = _save.Records[_index];

            //todo comment: Что проверяет это условие (с какой целью)? 
            // Наступило ли время для перехода к следующей записи.
            // Если текущее игровое время (Time.time) превышает время записи curr.Time, происходит переход к следующему элементу списка Records.

            if (Time.time > curr.Time)
            {
                _prev = curr;
                _index++;

                //todo comment: Для чего нужна эта проверка?
                // Если все записи воспроизведены, компонент отключается, чтобы прекратить выполнение Update().

                if (_index >= _save.Records.Count)
                {
                    enabled = false;
                    Debug.Log($"<b>{name}</b> finished", this);
                }
            }

            //todo comment: Для чего производятся эти вычисления (как в дальнейшем они применяются)?
            // Рассчитывает нормализованное время (0-1) между двумя записями. Например:
            // Если delta = 0, объект находится в позиции _prev.Position.
            // Если delta = 1, объект достигает curr.Position.

            var delta = (Time.time - _prev.Time) / (curr.Time - _prev.Time);

            //todo comment: Зачем нужна эта проверка?
            // Если curr.Time == _prev.Time, возникает деление на ноль, и delta становится NaN (не число)
            // Принудительно устанавливает delta = 0, чтобы избежать ошибок в вычислениях.

            if (float.IsNaN(delta)) delta = 0f;

            //todo comment: Опишите, что происходит в этой строчке так подробно, насколько это возможно
            // Плавно формирует позицию обьекта между двумя показателями на основе значения delta, это создает эфект движения обьекта по записанной траектории.

            transform.position = Vector3.Lerp(_prev.Position, curr.Position, delta);
        }
    }
}