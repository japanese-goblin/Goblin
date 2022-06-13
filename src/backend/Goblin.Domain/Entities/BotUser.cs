using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goblin.Domain.Entities;

public class BotUser
{
    public long Id { get; private set; }

    public string WeatherCity { get; private set; }
    public int NarfuGroup { get; private set; }

    public bool IsErrorsEnabled { get; private set; }
    public bool IsAdmin { get; private set; }

    public bool HasWeatherSubscription { get; private set; }
    public bool HasScheduleSubscription { get; private set; }

    public ConsumerType ConsumerType { get; set; }

    protected BotUser()
    {
    }

    public BotUser(long id, string city = "", int group = 0, bool isAdmin = false,
                   bool isErrorsEnabled = true, bool hasWeather = false, bool hasSchedule = false)
    {
        SetId(id);
        SetCity(city);
        SetNarfuGroup(group);
        SetAdmin(isAdmin);
        SetErrorNotification(isErrorsEnabled);
        SetHasWeather(hasWeather);
        SetHasSchedule(hasSchedule);
    }

    private void SetId(long id)
    {
        if(id <= 0)
        {
            throw new ArgumentException("Параметр должен быть больше 0", nameof(id));
        }

        Id = id;
    }

    public void SetCity(string city)
    {
        WeatherCity = city;
    }

    public void SetNarfuGroup(int group)
    {
        if(group < 0)
        {
            throw new ArgumentException("Параметр должен быть больше 0", nameof(group));
        }

        NarfuGroup = group;
    }

    public void SetErrorNotification(bool enable = true)
    {
        IsErrorsEnabled = enable;
    }

    public void SetAdmin(bool isAdmin = false)
    {
        IsAdmin = isAdmin;
    }

    public void SetHasWeather(bool isWeather)
    {
        HasWeatherSubscription = isWeather;
    }

    public void SetHasSchedule(bool isSchedule)
    {
        HasScheduleSubscription = isSchedule;
    }
}