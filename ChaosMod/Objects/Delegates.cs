namespace FrootLuips.ChaosMod.Objects;

public delegate bool TryFunc<in T, TResult>(T input, out TResult result);

public delegate void Callback(string message);

public delegate void StatusCallback(List<string> issues, bool success);

internal delegate void OnEffectEnd(ActiveEffect sender);
