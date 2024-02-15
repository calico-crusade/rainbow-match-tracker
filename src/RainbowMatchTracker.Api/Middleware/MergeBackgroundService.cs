namespace RainbowMatchTracker.Api.Middleware;

using Services;

public class MergeBackgroundService(
    IMatchMergeService _merge,
    IConfiguration _config) : BackgroundService
{
    public double DelayMin => double.TryParse(_config["Matches:DelayMin"], out var v) ? v : 3;

    public int DelayMs => (int)(DelayMin * 60 * 1000);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            await _merge.Merge().ToArrayAsync(stoppingToken);
            await Task.Delay(DelayMs, stoppingToken);
        }
    }
}
