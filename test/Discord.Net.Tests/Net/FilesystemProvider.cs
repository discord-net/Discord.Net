//From https://github.com/akavache/Akavache
//Copyright (c) 2012 GitHub
//TODO: Remove once netstandard support is added

using Akavache;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;

namespace Discord
{
    public class FilesystemProvider : IFilesystemProvider
    {
        public IObservable<Stream> OpenFileForReadAsync(string path, IScheduler scheduler)
        {
            return SafeOpenFileAsync(path, FileMode.Open, FileAccess.Read, FileShare.Read, scheduler);
        }

        public IObservable<Stream> OpenFileForWriteAsync(string path, IScheduler scheduler)
        {
            return SafeOpenFileAsync(path, FileMode.Create, FileAccess.Write, FileShare.None, scheduler);
        }

        public IObservable<Unit> CreateRecursive(string path)
        {
            CreateRecursive(new DirectoryInfo(path));
            return Observable.Return(Unit.Default);
        }

        public IObservable<Unit> Delete(string path)
        {
            return Observable.Start(() => File.Delete(path), Scheduler.Default);
        }
                
        public string GetDefaultRoamingCacheDirectory()
        {
            throw new NotSupportedException();
        }

        public string GetDefaultSecretCacheDirectory()
        {
            throw new NotSupportedException();
        }

        public string GetDefaultLocalMachineCacheDirectory()
        {
            throw new NotSupportedException();
        }

        protected static string GetAssemblyDirectoryName()
        {
            var assemblyDirectoryName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Debug.Assert(assemblyDirectoryName != null, "The directory name of the assembly location is null");
            return assemblyDirectoryName;
        }

        private static IObservable<Stream> SafeOpenFileAsync(string path, FileMode mode, FileAccess access, FileShare share, IScheduler scheduler = null)
        {
            scheduler = scheduler ?? Scheduler.Default;
            var ret = new AsyncSubject<Stream>();

            Observable.Start(() =>
            {
                try
                {
                    var createModes = new[]
                    {
                        FileMode.Create,
                        FileMode.CreateNew,
                        FileMode.OpenOrCreate,
                    };


                    // NB: We do this (even though it's incorrect!) because
                    // throwing lots of 1st chance exceptions makes debugging
                    // obnoxious, as well as a bug in VS where it detects
                    // exceptions caught by Observable.Start as Unhandled.
                    if (!createModes.Contains(mode) && !File.Exists(path))
                    {
                        ret.OnError(new FileNotFoundException());
                        return;
                    }

                    Observable.Start(() => new FileStream(path, mode, access, share, 4096, false), scheduler).Cast<Stream>().Subscribe(ret);
                }
                catch (Exception ex)
                {
                    ret.OnError(ex);
                }
            }, scheduler);

            return ret;
        }
        private static void CreateRecursive(DirectoryInfo info)
        {
            SplitFullPath(info).Aggregate((parent, dir) =>
            {
                var path = Path.Combine(parent, dir);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            });
        }

        private static IEnumerable<string> SplitFullPath(DirectoryInfo info)
        {
            var root = Path.GetPathRoot(info.FullName);
            var components = new List<string>();
            for (var path = info.FullName; path != root && path != null; path = Path.GetDirectoryName(path))
            {
                var filename = Path.GetFileName(path);
                if (String.IsNullOrEmpty(filename))
                    continue;
                components.Add(filename);
            }
            components.Add(root);
            components.Reverse();
            return components;
        }
    }
}