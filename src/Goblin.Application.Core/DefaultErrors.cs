﻿namespace Goblin.Application.Core
{
    public class DefaultErrors
    {
        public const string GroupNotSet =
                "Для выполнения этого действия необходимо установить группу.\n\n" +
                "Напиши \"Установить группу 123456\" без кавычек.\n" +
                "где 123456 - номер Вашей группы";

        public const string CityNotSet =
                "Для выполнения этого действия необходимо установить город для выполнения данного действия.\n" +
                "Напиши \"Установить город Москва\" без кавычек.\n";

        public const string NarfuSiteIsUnavailable = "Сайт с расписанием временно недоступен. Попробуйте позже.";
        public const string NarfuUnexpectedError = "Непредвиденная ошибка получения расписания с сайта. Попробуйте позже.";

        public const string WeatherSiteIsUnavailable = "Сайт с погодой временно недоступен. Попробуйте позже.";
        public const string WeatherUnexpectedError = "Непредвиденная ошибка при получении погоды. Попробуйте позже.";
    }
}