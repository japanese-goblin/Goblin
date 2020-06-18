namespace Goblin.Application.Core
{
    public class DefaultErrors
    {
        public const string GroupNotSet =
                "Необходимо установить группу (в которой вы учитесь) для выполнения данного действия (нужно написать следующее - установить группу 123456).";

        public const string CityNotSet =
                "Необходимо установить город для выполнения данного действия (нужно написать следующее - установить город Москва).";

        public const string NarfuSiteIsUnavailable = "Сайт с расписанием временно недоступен. Попробуйте позже.";
        public const string NarfuUnexpectedError = "Непредвиденная ошибка получения расписания с сайта. Попробуйте позже.";
    }
}