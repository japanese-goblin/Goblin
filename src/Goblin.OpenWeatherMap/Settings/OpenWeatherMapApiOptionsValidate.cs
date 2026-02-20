using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;

namespace Goblin.OpenWeatherMap.Settings;

internal class OpenWeatherMapApiOptionsValidate : IValidateOptions<OpenWeatherMapApiOptions>
{
    private static readonly TimeSpan MinimalTimeout = TimeSpan.FromSeconds(1);

    public ValidateOptionsResult Validate(string name, OpenWeatherMapApiOptions options)
    {
        var errors = ValidateInternal(options).ToList();
        if(errors.Count > 0)
        {
            return ValidateOptionsResult.Fail(errors);
        }

        return ValidateOptionsResult.Success;
    }

    private static IEnumerable<string> ValidateInternal(OpenWeatherMapApiOptions options)
    {
        if(string.IsNullOrEmpty(options.HostUrl))
        {
            yield return $"Не задан хост OpenWeatherMap ({nameof(OpenWeatherMapApiOptions.HostUrl)})";
        }

        if(!Uri.TryCreate(options.HostUrl, UriKind.Absolute, out _))
        {
            yield return $"Задан некорректный хост OpenWeatherMap ({nameof(OpenWeatherMapApiOptions.HostUrl)})";
        }

        if(string.IsNullOrEmpty(options.AccessToken))
        {
            yield return $"Не задан токен доступа к OpenWeatherMap ({nameof(OpenWeatherMapApiOptions.AccessToken)})";
        }

        if(options.Timeout < MinimalTimeout)
        {
            yield return
                    $"Задан слишком маленький таймаут получения погоды ({nameof(OpenWeatherMapApiOptions.Timeout)}). Значение должно быть больше {MinimalTimeout}";
        }
    }
}