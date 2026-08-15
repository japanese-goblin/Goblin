namespace Goblin.Narfu.Models;

/// <summary>
///     Модель группы
/// </summary>
/// <param name="Name">Название</param>
/// <param name="RealId">Номер группы</param>
/// <param name="SiteId">Идентификатор на сайте</param>
public record Group(string Name, int RealId, int SiteId);