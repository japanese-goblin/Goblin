using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;

namespace Goblin.Narfu.Settings;

internal class NarfuOptionsValidate : IValidateOptions<NarfuApiOptions>
{
    private static readonly TimeSpan MinimalTimeout = TimeSpan.FromSeconds(1);

    public ValidateOptionsResult Validate(string name, NarfuApiOptions options)
    {
        var errors = ValidateInternal(options).ToList();
        if(errors.Count > 0)
        {
            return ValidateOptionsResult.Fail(errors);
        }

        return ValidateOptionsResult.Success;
    }

    private static IEnumerable<string> ValidateInternal(NarfuApiOptions options)
    {
        if(string.IsNullOrEmpty(options.HostUrl))
        {
            yield return $"Не задан хост расписания САФУ ({nameof(NarfuApiOptions.HostUrl)})";
        }

        if(!Uri.TryCreate(options.HostUrl, UriKind.Absolute, out _))
        {
            yield return $"Задан некорректный хост расписания САФУ ({nameof(NarfuApiOptions.HostUrl)})";
        }

        if(string.IsNullOrEmpty(options.NarfuGroupsLink))
        {
            yield return $"Не задана ссылка на скачивание списка групп САФУ ({nameof(NarfuApiOptions.NarfuGroupsLink)})";
        }

        if(!Uri.TryCreate(options.NarfuGroupsLink, UriKind.Absolute, out _))
        {
            yield return $"Задана некорректная ссылка на скачивание списка групп САФУ ({nameof(NarfuApiOptions.NarfuGroupsLink)})";
        }

        if(options.Timeout < MinimalTimeout)
        {
            yield return
                    $"Задан слишком маленький таймаут получения расписания ({nameof(NarfuApiOptions.Timeout)}). Значение должно быть больше {MinimalTimeout}";
        }
    }
}