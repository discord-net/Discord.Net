using Discord.Overrides;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents an override that can be loaded.
    /// </summary>
    public sealed class Override
    {
        /// <summary>
        ///     Gets the ID of the override.
        /// </summary>
        public Guid Id { get; internal set; }

        /// <summary>
        ///     Gets the name of the override.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        ///     Gets the description of the override.
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        ///     Gets the date this override was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; internal set; }

        /// <summary>
        ///     Gets the date the override was last modified.
        /// </summary>
        public DateTimeOffset LastUpdated { get; internal set; }

        internal static Override FromJson(string json)
        {
            var result = new Override();

            using (var textReader = new StringReader(json))
            using (var reader = new JsonTextReader(textReader))
            {
                var obj = JObject.ReadFrom(reader);
                result.Id = obj["id"].ToObject<Guid>();
                result.Name = obj["name"].ToObject<string>();
                result.Description = obj["description"].ToObject<string>();
                result.CreatedAt = obj["created_at"].ToObject<DateTimeOffset>();
                result.LastUpdated = obj["last_updated"].ToObject<DateTimeOffset>();
            }

            return result;
        }
    }

    /// <summary>
    ///     Represents a loaded override instance.
    /// </summary>
    public sealed class LoadedOverride
    {
        /// <summary>
        ///     Gets the assembly containing the overrides definition.
        /// </summary>
        public Assembly Assembly { get; internal set; }

        /// <summary>
        ///     Gets an instance of the override.
        /// </summary>
        public IOverride Instance { get; internal set; }

        /// <summary>
        ///     Gets the overrides type.
        /// </summary>
        public Type Type { get; internal set; }
    }

    public sealed class BuildOverrides
    {
        /// <summary>
        ///     Fired when an override logs a message.
        /// </summary>
        public static event Func<Override, string, Task> Log
        {
            add => _logEvents.Add(value);
            remove => _logEvents.Remove(value);

        }

        /// <summary>
        ///     Gets a read-only dictionary containing the currently loaded overrides.
        /// </summary>
        public IReadOnlyDictionary<Override, IReadOnlyCollection<LoadedOverride>> LoadedOverrides
            => _loadedOverrides.Select(x => new KeyValuePair<Override, IReadOnlyCollection<LoadedOverride>>(x.Key, x.Value)).ToDictionary(x => x.Key, x => x.Value);

        private static AssemblyLoadContext _overrideDomain;
        private static List<Func<Override, string, Task>> _logEvents = new();
        private static ConcurrentDictionary<Override, List<LoadedOverride>> _loadedOverrides = new ConcurrentDictionary<Override, List<LoadedOverride>>();

        private const string ApiUrl = "https://overrides.discordnet.dev";

        static BuildOverrides()
        {
            _overrideDomain = new AssemblyLoadContext("Discord.Net.Overrides.Runtime");

            _overrideDomain.Resolving += _overrideDomain_Resolving;
        }

        /// <summary>
        ///     Gets details about a specific override.
        /// </summary>
        /// <remarks>
        ///     <b>Note:</b> This method does not load an override, it simply retrieves the info about it.
        /// </remarks>
        /// <param name="name">The name of the override to get.</param>
        /// <returns>
        ///     A task representing the asynchronous get operation. The tasks result is an <see cref="Override"/>
        ///     if it exists; otherwise <see langword="null"/>.
        /// </returns>
        public static async Task<Override> GetOverrideAsync(string name)
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync($"{ApiUrl}/overrides/{name}");

                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();

                    return Override.FromJson(content);
                }
                else
                    return null;
            }
        }

        /// <summary>
        ///     Adds an override to the current Discord.Net instance.
        /// </summary>
        /// <remarks>
        ///     The override initialization is non-blocking, any errors that occur within
        ///     the overrides initialization procedure will be sent in the <see cref="Log"/> event.
        /// </remarks>
        /// <param name="name">The name of the override to add.</param>
        /// <returns>
        ///     A task representing the asynchronous add operation. The tasks result is a boolean
        ///     determining if the add operation was successful.
        /// </returns>
        public static async Task<bool> AddOverrideAsync(string name)
        {
            var ovrride = await GetOverrideAsync(name);

            if (ovrride == null)
                return false;

            return await AddOverrideAsync(ovrride);
        }

        /// <summary>
        ///     Adds an override to the current Discord.Net instance.
        /// </summary>
        /// <remarks>
        ///     The override initialization is non-blocking, any errors that occur within
        ///     the overrides initialization procedure will be sent in the <see cref="Log"/> event.
        /// </remarks>
        /// <param name="ovrride">The override to add.</param>
        /// <returns>
        ///     A task representing the asynchronous add operation. The tasks result is a boolean
        ///     determining if the add operation was successful.
        /// </returns>
        public static async Task<bool> AddOverrideAsync(Override ovrride)
        {
            // download it
            var ms = new MemoryStream();

            using (var client = new HttpClient())
            {
                var result = await client.GetAsync($"{ApiUrl}/overrides/download/{ovrride.Id}");

                if (!result.IsSuccessStatusCode)
                    return false;

                await (await result.Content.ReadAsStreamAsync()).CopyToAsync(ms);
            }

            ms.Position = 0;

            // load the assembly
            //var test = Assembly.Load(ms.ToArray());
            var asm = _overrideDomain.LoadFromStream(ms);

            // find out IOverride
            var overrides = asm.GetTypes().Where(x => x.GetInterfaces().Any(x => x == typeof(IOverride)));

            List<LoadedOverride> loaded = new();

            var context = new OverrideContext((m) => HandleLog(ovrride, m), ovrride);

            foreach (var ovr in overrides)
            {
                var inst = (IOverride)Activator.CreateInstance(ovr);

                inst.RegisterPackageLookupHandler((s) =>
                {
                    return GetDependencyAsync(ovrride.Id, s);
                });

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await inst.InitializeAsync(context);
                    }
                    catch (Exception x)
                    {
                        HandleLog(ovrride, $"Failed to initialize build override: {x}");
                    }
                });

                loaded.Add(new LoadedOverride()
                {
                    Assembly = asm,
                    Instance = inst,
                    Type = ovr
                });
            }

            return _loadedOverrides.AddOrUpdate(ovrride, loaded, (_, __) => loaded) != null;
        }

        internal static void HandleLog(Override ovr, string msg)
        {
            _ = Task.Run(async () =>
            {
                foreach (var item in _logEvents)
                {
                    await item.Invoke(ovr, msg).ConfigureAwait(false);
                }
            });
        }

        private static Assembly _overrideDomain_Resolving(AssemblyLoadContext arg1, AssemblyName arg2)
        {
            // resolve the override id
            var v = _loadedOverrides.FirstOrDefault(x => x.Value.Any(x => x.Assembly.FullName == arg1.Assemblies.First().FullName));

            return GetDependencyAsync(v.Key.Id, $"{arg2}").GetAwaiter().GetResult();
        }

        private static async Task<Assembly> GetDependencyAsync(Guid id, string name)
        {
            using (var client = new HttpClient())
            {
                var result = await client.PostAsync($"{ApiUrl}/overrides/{id}/dependency", new StringContent($"{{ \"info\": \"{name}\"}}", Encoding.UTF8, "application/json"));

                if (!result.IsSuccessStatusCode)
                    throw new HttpRequestException("Failed to get dependency");

                using (var ms = new MemoryStream())
                {
                    var innerStream = await result.Content.ReadAsStreamAsync();
                    await innerStream.CopyToAsync(ms);
                    ms.Position = 0;
                    return _overrideDomain.LoadFromStream(ms);
                }
            }
        }
    }
}
