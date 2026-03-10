using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Codexus.Interceptors;
using Codexus.Game.Launcher.Services.Java;
using EastSide.Entities.Web.NEL;

namespace EastSide.Manager;

internal class GameManager
{
    private readonly Lock _lock = new Lock();
    static readonly Dictionary<Guid, LauncherService> Launchers = new();
    static readonly Dictionary<Guid, Interceptor> Interceptors = new();
    static readonly object Lock = new object();
    public static GameManager Instance { get; } = new GameManager();

    public sealed class LockScope : IDisposable
    {
        readonly object _Lock;
        public LockScope(object o){_Lock=o; Monitor.Enter(_Lock);}
        public void Dispose(){ Monitor.Exit(_Lock);}
    }
    public static LockScope EnterScope(object o)=>new LockScope(o);

    public List<EntityQueryInterceptors> GetQueryInterceptors()
    {
        var list = Interceptors.Values.Select((interceptor, index) => new EntityQueryInterceptors
        {
            Id = index.ToString(),
            Name = interceptor.Identifier,
            Address = $"{interceptor.ForwardAddress}:{interceptor.ForwardPort}",
            Role = interceptor.NickName,
            Server = interceptor.ServerName,
            Version = interceptor.ServerVersion,
            LocalAddress = $"{interceptor.LocalAddress}:{interceptor.LocalPort}"
        }).ToList();

        return list;
    }

    public List<EntityQueryLaunchers> GetQueryLaunchers()
    {
        return Launchers.Values.Select((launcher, index) => new EntityQueryLaunchers
        {
            Id = index.ToString(),
            Name = launcher.Identifier,
            Role = launcher.Entity.RoleName,
            Server = launcher.Entity.GameName,
            Version = launcher.Entity.GameVersion,
            StatusMessage = launcher.LastProgress.Message,
            Progress = launcher.LastProgress.Percent,
            ProcessId = launcher.GetProcess()?.Id ?? -1
        }).ToList();
    }

    public void ShutdownInterceptor(Guid identifier)
    {
        Interceptor? value = null;
        var has = false;
        using (EnterScope(Lock))
        {
            if (Interceptors.TryGetValue(identifier, out value))
            {
                Interceptors.Remove(identifier);
                has = true;
            }
        }
        if (has && value != null)
        {
            value.ShutdownAsync();
            Packet.SSynchronizePlayerPosition.ClearDetection(identifier);
        }
    }

    public void ShutdownLauncher(Guid identifier)
    {
        LauncherService? value = null;
        var has = false;
        using (EnterScope(Lock))
        {
            if (Launchers.TryGetValue(identifier, out value))
            {
                Launchers.Remove(identifier);
                has = true;
            }
        }
        if (has && value != null)
        {
            value.ShutdownAsync();
        }
    }

    public void AddInterceptor(Interceptor interceptor)
    {
        using (_lock.EnterScope())
        {
            Interceptors.Add(interceptor.Identifier, interceptor);
        }
    }

    public void AddLauncher(LauncherService launcher)
    {
        using (_lock.EnterScope())
        {
            Launchers.Add(launcher.Identifier, launcher);
            launcher.Exited += id => ShutdownLauncher(id);
        }
    }

    public Interceptor? GetInterceptor(Guid identifier)
    {
        using (EnterScope(Lock))
        {
            return Interceptors.TryGetValue(identifier, out var interceptor) ? interceptor : null;
        }
    }

    public LauncherService? GetLauncher(Guid identifier)
    {
        using (EnterScope(Lock))
        {
            return Launchers.TryGetValue(identifier, out var launcher) ? launcher : null;
        }
    }
}
