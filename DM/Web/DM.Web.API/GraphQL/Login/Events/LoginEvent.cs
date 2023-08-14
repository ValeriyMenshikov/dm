using System;

namespace DM.Web.API.GraphQL.Login.Events;

/// <summary>
/// Login event data
/// </summary>
public sealed record LoginEvent(string Login, DateTimeOffset Timestamp);
