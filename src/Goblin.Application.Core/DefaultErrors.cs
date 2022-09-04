namespace Goblin.Application.Core;

public class DefaultErrors
{
    public const string GroupNotSet =
            "Для выполнения этого действия необходимо добавить информацию о группе.\n\n" +
            "✅Напиши \"Установить группу 123456\" без кавычек " +
            "и замени '123456' на номер своей группы (например, 351617)";

    public const string CityNotSet =
            "Для выполнения этого действия необходимо добавить информацию о городе.\n" +
            "✅Напиши \"Установить город Москва\" без кавычек.";

    public const string NarfuSiteIsUnavailable = "Сайт с расписанием (ruz.narfu.ru) временно недоступен. Попробуйте позже.";
    public const string NarfuUnexpectedError = "Непредвиденная ошибка получения расписания. Попробуйте позже.";

    public const string WeatherSiteIsUnavailable = "Сайт с погодой временно недоступен. Попробуйте позже.";
    public const string WeatherUnexpectedError = "Непредвиденная ошибка при получении погоды. Попробуйте позже.";
}