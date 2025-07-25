

using Microsoft.Extensions.Configuration;
using NLog;
using RazorEngine.Compilation.ReferenceResolver;
using RazorEngine.Compilation;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using SchoolPilot.Print.Interfaces;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using SchoolPilot.Print.Model;

namespace SchoolPilot.Print
{
    public class PrintEngine : IPrintEngine
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IRazorEngineService _service;
        private TemplateServiceConfiguration _templateServiceConfig;

        public PrintEngine(IConfiguration config)
        {
            var currentDirectory = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            _templateServiceConfig = new TemplateServiceConfiguration
            {
                Debug = false,
                ReferenceResolver = new PrintEngineReferenceResolver(),
                TemplateManager = new ResolvePathTemplateManager(new[]
                {
                    Path.Combine(currentDirectory, "Print"),
                    Path.Combine(currentDirectory, "Templates")
                })
            };

            if (config["App.Environment"] == "Local")
            {
                var searchPath = currentDirectory + "\\..\\..\\..\\Axxess.PalliativeCare.Print";
                var layoutRoots = new[]
                {
                    Path.Combine(searchPath, "Print"),
                    Path.Combine(searchPath, "Templates")
                };
                var invalidatingCachingProvider = new InvalidatingCachingProvider();
                _templateServiceConfig.CachingProvider = invalidatingCachingProvider;
                _templateServiceConfig.TemplateManager = new CustomWatchingResolvePathTemplateManager(layoutRoots, invalidatingCachingProvider);
            }

            _service = RazorEngineService.Create(_templateServiceConfig);
        }

        public string GenerateHeaderHtml(BasicPrintHeader schema)
        {
            var templateKey = _service.GetKey("Header");
            var html = _service.RunCompile(templateKey, schema.GetType(), schema);

            return html;
        }

        public string GenerateBodyHtml<T>(PrintSchema<T> schema, string? templateName = null)
        {
            templateName = templateName ?? typeof(T).Name;
            var templateKey = _service.GetKey(templateName);
            // only need to load templates once so check if they are in cache
            var html = _service.RunCompile(templateKey, typeof(PrintSchema<T>), schema);
            return html;
        }

        public string GenerateFooterHtml(FooterInfo schema)
        {
            var footerHtml = _service.RunCompile(_service.GetKey("Footer"), schema.GetType(), schema);
            return footerHtml;
        }

    }

    internal class PrintEngineReferenceResolver : IReferenceResolver
    {
        public string FindLoaded(IEnumerable<string> refs, string find)
        {
            return refs.First(r => r.EndsWith(System.IO.Path.DirectorySeparatorChar + find));
        }

        public IEnumerable<CompilerReference> GetReferences(TypeContext context, IEnumerable<CompilerReference> includeAssemblies = null)
        {
            IEnumerable<string> loadedAssemblies = (new UseCurrentAssembliesReferenceResolver())
                .GetReferences(context, includeAssemblies)
                .Select(r => r.GetFile())
                .ToArray();

            yield return CompilerReference.From(FindLoaded(loadedAssemblies, "mscorlib.dll"));
            yield return CompilerReference.From(FindLoaded(loadedAssemblies, "System.dll"));
            yield return CompilerReference.From(FindLoaded(loadedAssemblies, "System.Core.dll"));
            yield return CompilerReference.From(FindLoaded(loadedAssemblies, "RazorEngine.dll"));
            yield return CompilerReference.From(typeof(PrintEngineReferenceResolver).Assembly);
            //yield return CompilerReference.From(typeof(PatientHeaderInfo).Assembly);
            //yield return CompilerReference.From(typeof(DrugInteractionResult).Assembly);
            //yield return CompilerReference.From(typeof(EnumExtensions).Assembly);
            //yield return CompilerReference.From(typeof(Markdown).Assembly);
        }
    }

    public class CustomWatchingResolvePathTemplateManager : ITemplateManager, IDisposable
    {
        private readonly ConcurrentQueue<FileSystemEventArgs> queue = new ConcurrentQueue<FileSystemEventArgs>();
        private readonly CancellationTokenSource cancelToken = new CancellationTokenSource();
        private readonly ResolvePathTemplateManager inner;
        private readonly InvalidatingCachingProvider cache;
        private readonly List<FileSystemWatcher> watchers;
        private bool isDisposed;

        /// <summary>Creates a new WatchingResolvePathTemplateManager.</summary>
        /// <param name="layoutRoot">the folders to watch and look for templates.</param>
        /// <param name="cache">the cache to invalidate</param>
        public CustomWatchingResolvePathTemplateManager(
          IEnumerable<string> layoutRoot,
          InvalidatingCachingProvider cache)
        {
            this.cache = cache;
            ReadOnlyCollection<string> readOnlyCollection = new ReadOnlyCollection<string>((IList<string>)new List<string>(layoutRoot));
            this.inner = new ResolvePathTemplateManager(readOnlyCollection);
            this.watchers = readOnlyCollection.Select<string, FileSystemWatcher>((Func<string, FileSystemWatcher>)(path =>
            {
                FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(Path.GetFullPath(path), "*.*");
                fileSystemWatcher.EnableRaisingEvents = true;
                fileSystemWatcher.IncludeSubdirectories = true;
                fileSystemWatcher.Changed += new FileSystemEventHandler(this.watcher_Changed);
                fileSystemWatcher.Created += new FileSystemEventHandler(this.watcher_Changed);
                fileSystemWatcher.Deleted += new FileSystemEventHandler(this.watcher_Changed);
                fileSystemWatcher.Renamed += new RenamedEventHandler(this.watcher_Renamed);
                return fileSystemWatcher;
            })).ToList<FileSystemWatcher>();
        }

        //This is the primary reason this is custom. We nuke the cache anytime something is saved.
        private void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            cache.InvalidateAll();
        }

        private void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            this.watcher_Changed(sender, new FileSystemEventArgs(WatcherChangeTypes.Deleted, e.OldFullPath, e.OldName));
            this.watcher_Changed(sender, new FileSystemEventArgs(WatcherChangeTypes.Created, e.FullPath, e.Name));
        }

        /// <summary>Resolve a template.</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ITemplateSource Resolve(ITemplateKey key)
        {
            return this.inner.Resolve(key);
        }

        /// <summary>Gets a key for the given template.</summary>
        /// <param name="name"></param>
        /// <param name="resolveType"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public ITemplateKey GetKey(
          string name,
          ResolveType resolveType,
          ITemplateKey context)
        {
            return this.inner.GetKey(name, resolveType, context);
        }

        /// <summary>Add a dynamic template (throws an exception)</summary>
        /// <param name="key"></param>
        /// <param name="source"></param>
        public void AddDynamic(ITemplateKey key, ITemplateSource source)
        {
            this.inner.AddDynamic(key, source);
        }

        /// <summary>Dispose the current manager.</summary>
        public void Dispose()
        {
            if (this.isDisposed)
                return;
            this.cancelToken.Cancel();
            foreach (FileSystemWatcher watcher in this.watchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            this.cancelToken.Dispose();
            this.isDisposed = true;
        }
    }

}
