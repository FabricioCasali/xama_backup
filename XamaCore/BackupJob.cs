using ICSharpCode.SharpZipLib.Zip;
using NLog;
using Quartz;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using XamaCore.Configs;

namespace XamaCore
{
    [DisallowConcurrentExecution]
    public class BackupJob : IJob
    {
        private Logger _logger;

        public BackupJob(Logger logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var data = context.MergedJobDataMap;
            var task = data["task"] as ConfigTask;

            _logger.Info($"Starting backup job for {task.Name}");
            var outuputPath = Path.Combine(task.Target.Path, task.Target.FileName);

            var start = DateTime.Now;

            using (var os = new ZipOutputStream(File.Create(outuputPath)))
            {
                os.SetLevel(9);
                foreach (var p in task.Paths)
                {
                    _logger.Debug($"Backuping {p.Path}");
                    var di = new DirectoryInfo(p.Path);
                    di.GetFiles("*", p.IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToList().ForEach(f =>
                    {
                        // todo incluir validações de máscara
                        var entry = new ZipEntry(f.FullName.Substring(di.FullName.Length + 1));
                        entry.DateTime = DateTime.Now;
                        _logger.Debug($"Adding {entry.Name}");
                        os.PutNextEntry(entry);
                        os.WriteAsync(File.ReadAllBytes(f.FullName)).GetAwaiter().GetResult();
                    });
                }
                os.Finish();
                os.Close();
                _logger.Info($"Backup job for {task.Name} finished in {(DateTime.Now - start).TotalSeconds} seconds");
            }
            return Task.CompletedTask;
        }
    }
}
