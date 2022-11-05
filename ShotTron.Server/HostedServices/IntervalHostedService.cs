using System.Diagnostics;

namespace ShotTron.Server.HostedServices;

public abstract class IntervalHostedService : IHostedService
{
    private readonly TimeSpan _interval;
    private readonly TimeSpan _startDelay;
    private bool _firstExec = true;
    
    public IntervalHostedService(TimeSpan interval) : this(TimeSpan.Zero, interval)
    {
    }
    
    public IntervalHostedService(TimeSpan startDelay, TimeSpan interval)
    {
        _startDelay = startDelay;
        _interval = interval;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_firstExec)
        {
            await Task.Delay(_startDelay, cancellationToken);
            _firstExec = false;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var sw = Stopwatch.StartNew();
        
        try
        {
            var result = await DoWorkAsync();
            
            if (!result)
            {
                await StopAsync(cancellationToken);
                return;
            }
        }
        catch (Exception ex)
        {
            // todo
        }
        
        sw.Stop();
        var waitTime = _interval - sw.Elapsed;
        waitTime = waitTime < TimeSpan.Zero ? TimeSpan.Zero : waitTime;
        await Task.Delay(waitTime, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public abstract Task<bool> DoWorkAsync();
}