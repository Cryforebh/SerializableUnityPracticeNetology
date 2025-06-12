using UnityEngine;

namespace DefaultNamespace
{

    // Этот скрипт EditorMover предназначен для отслеживания и сохранения позиций объекта
	// в течение определенного времени с задержкой. Он использует компонент PositionSaver для хранения данных о позиции.

    [RequireComponent(typeof(PositionSaver))]

    // Атрибут [RequireComponent(typeof(PositionSaver))] в Unity используется для автоматического добавления необходимых компонентов в GameObject.
    // Когда вы применяете этот атрибут к скрипту, Unity гарантирует, что указанный компонент (PositionSaver в данном случае) будет добавлен к тому же GameObject,
    // к которому прикреплён скрипт. Это помогает избежать ошибок при настройке, особенно когда определённый компонент должен присутствовать для корректной работы скрипта.

    public class EditorMover : MonoBehaviour
	{
		private PositionSaver _save;	// Ссылка на компонент PositionSaver.
        private float _currentDelay;	// Текущая задержка перед записью позиции.

        //todo comment: Что произойдёт, если _delay > _duration?
        // Если _delay больше, чем _duration, скрипт будет записывать позиции с задержкой,
        // но общая продолжительность работы будет меньше, что может привести к некорректной работе.

        [SerializeField, Range(0.2f, 1.0f)]
        private float _delay = 0.5f;	// Задержка перед первой записью (0.5f).
        [SerializeField, Min(0.2f)]
        private float _duration = 5f;   // Общая продолжительность работы скрипта (5f).

        private void Start()
		{
            //todo comment: Почему этот поиск производится здесь, а не в начале метода Update?
            // Поиск компонента выполняется в Start(), чтобы гарантировать, что компонент доступен до начала работы скрипта.
			// Это более эффективно и избегает ошибок, если компонент отсутствует.

            _save = GetComponent<PositionSaver>();	// Инициализация: скрипт ищет компонент PositionSaver через GetComponent<PositionSaver>().
            _save.Records.Clear();                  // Очищает список записей компонента PositionSaver с помощью _save.Records.Clear().
        }

		private void Update()
		{
			_duration -= Time.deltaTime;                        // Уменьшение _duration на величину Time.deltaTime.
            if (_duration <= 0f)                                // Если _duration достигает 0, скрипт отключается (enabled = false) и выводит сообщение в консоль.
            {
				enabled = false;
				Debug.Log($"<b>{name}</b> finished", this);
				return;
			}

            //todo comment: Почему не написать (_delay -= Time.deltaTime;) по аналогии с полем _duration?
            // _delay используется как начальная задержка перед первой записью. Уменьшение его значения не имеет смысла,
			// так как оно должно оставаться постоянным между записями.

            _currentDelay -= Time.deltaTime;                    // Уменьшение _currentDelay
            if (_currentDelay <= 0f)                            // Если _currentDelay достигает 0, происходит запись позиции
            {
				_currentDelay = _delay;                         // _currentDelay восстанавливается до значения _delay.
                _save.Records.Add(new PositionSaver.Data        // В список записей добавляется новая запись с текущей позицией объекта и временем.
                {
					Position = transform.position,

                    //todo comment: Для чего сохраняется значение игрового времени?
                    // Время сохраняется вместе с позицией для отслеживания временных меток,
					// что может быть полезно для анализа или воспроизведения последовательности движений объекта.

                    Time = Time.time,
				});
			}
		}
	}
}