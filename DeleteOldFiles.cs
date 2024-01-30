public class DeleteOldFiles : IHostedLifecycleService
{
    private Task? executingTask;
    private CancellationTokenSource stoppingCts = new CancellationTokenSource();
    private readonly Settings settings;

    public DeleteOldFiles(Settings settings)
    {
        this.settings = settings;
    }

    public Task StartingAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Starting Service");
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Start Service");
        return Task.CompletedTask;
    }

    public async Task StartedAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Started Service");
        
        executingTask = ExecuteAsync(stoppingCts.Token);

        // If the task is completed, return it, otherwise it's still running
        await (executingTask.IsCompleted ? executingTask : Task.CompletedTask);
    }

    protected async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine($"Delete file {DateTime.Now:HH:mm:ss}");

            foreach (string file in Directory.GetFiles(settings.GetUserTempPath()))
            {
                var fi = new FileInfo(file);
                if (fi.CreationTimeUtc < DateTime.UtcNow.AddMinutes(-settings.DeleteFilesOlderThanMinutes))
                    fi.Delete();
            }

            await Task.Delay(settings.DeleteFilesOlderThanMinutes * 60 * 1000, stoppingToken);
        }
         Console.WriteLine("Execution stopped");
    }

    public Task StoppingAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Stopping Service");
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Stop Service");

        stoppingCts.Cancel();

        if (executingTask is not null)
            await Task.WhenAny(executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
    }

    public Task StoppedAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Stopped Service");
        return Task.CompletedTask;
    }

}