using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SportCentrum.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SportCentrum
{
    public class BackupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(60);

        public BackupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                await BackupDataAsync();
                await Task.Delay(_interval, cancellationToken);
            }
        }

        private async Task BackupDataAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SportCentrumContext>();

            var users = await context.Users.ToListAsync();
            var coaches = await context.Coaches.ToListAsync();
            var trainings = await context.Trainings.ToListAsync();
            var sessions = await context.Sessions.ToListAsync();

            var backupData = new
            {
                Timestamp = DateTime.UtcNow,
                Users = users,
                Coaches = coaches,
                Trainings = trainings,
                Sessions = sessions
            };

            var backupFolder = Path.Combine(Directory.GetCurrentDirectory(), "Backups");
            if(!Directory.Exists(backupFolder))
            {
                Directory.CreateDirectory(backupFolder);
            }

            var files = Directory.GetFiles(backupFolder, "backup_*.json");
            var expirationDate = DateTime.UtcNow.AddDays(-7);
            foreach (var file in files)
            {
                var creationTime = File.GetLastWriteTime(file);
                if(creationTime < expirationDate)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to delete file {file}: {ex.Message}");
                    }
                }
            }

            var backupFile = Path.Combine(backupFolder, $"backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json");
            var options = new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve };
            await File.WriteAllTextAsync(backupFile, JsonSerializer.Serialize(backupData, options));
        }
    }
}
