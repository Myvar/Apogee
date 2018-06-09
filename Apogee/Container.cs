using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Apogee.Components;
using Apogee.Ecs;
using Apogee.Files;
using Nett;
using Newtonsoft.Json;

namespace Apogee
{
    public class Container
    {
        public static string AssetsPath { get; set; }

        public static Camera Camera { get; set; }
        public static object _locker = new object();

        public Manifest Manifest { get; set; }

        public List<Component> Components { get; set; } = new List<Component>();

        public List<Ecs.System> Systems { get; set; } = new List<Ecs.System>();

        public Container(string path)
        {
            Reload(path);
        }

        public void Reload(string path)
        {
            Components = new List<Component>();
            Systems = new List<Ecs.System>();
            var p = Path.GetFullPath(path);
            AssetsPath = p;
            Manifest = Toml.ReadFile<Manifest>(Path.Combine(p, "manifest.tml"));

            //neded for reflections
            var assmbly = Assembly.GetExecutingAssembly();

            //build system index
            foreach (var type in assmbly.GetTypes())
            {
                if (type.BaseType == typeof(Ecs.System))
                {
                    Systems.Add((Ecs.System) Activator.CreateInstance(type, new object[] { }));
                }
            }

            Camera = new Camera(0.1f, 1000, GameEngine.Window.Width, GameEngine.Window.Height, 70, 10);
        }

        public void Update(bool load = false)
        {
            lock (_locker)
            {
                var entitys = BuildEntityIndex();

                foreach (var system in Systems)
                {
                    if (system.RunOnce != load) continue;
                    //    Parallel.ForEach(entitys, (entity, state) =>
                    foreach (var entity in entitys)
                    {
                        var meths = new List<(int args, MethodInfo info)>();
                        foreach (var meth in system.GetType().GetTypeInfo().GetMethods())
                        {
                            var infos = meth.GetParameters();
                            if (infos.Length <= entity.components.Count)
                            {
                                var flag = true;
                                for (var i = 0; i < infos.Length; i++)
                                {
                                    var parameter = infos[i];
                                    var flag2 = true;
                                    foreach (var tuple in entity.components)
                                    {
                                        if (tuple.t == parameter.ParameterType)
                                        {
                                            flag2 = false;
                                            break;
                                        }
                                    }

                                    if (flag2)
                                    {
                                        flag = false;
                                        break;
                                    }
                                }

                                if (flag && meth != null && meth.GetParameters().Length != 0)
                                    meths.Add((meth.GetParameters().Length, meth));
                            }
                        }

                        if (meths.Count != 0)
                        {
                            var ordered = meths.OrderBy(x => x.args).Reverse();
                            var args = new object[ordered.First().args];

                            var parameters = ordered.First().info.GetParameters();
                            for (var i = 0; i < parameters.Length; i++)
                            {
                                var parameter = parameters[i];
                                args[i] = Components[
                                    entity.components.First(x => x.t == parameter.ParameterType).index];
                            }

                            ordered.First().info.Invoke(system, args);
                        }
                    } //);
                }
            }
        }

        public List<(Guid id, List<(int index, Type t)> components)> BuildEntityIndex()
        {
            var re = new List<(Guid id, List<(int index, Type t)> components)>();

            for (var i = 0; i < Components.Count; i++)
            {
                var component = Components[i];

                if (re.All(x => component.Entity != x.id))
                {
                    re.Add((component.Entity, new List<(int index, Type t)>()));
                }

                var entity = re.Single(x => x.id == component.Entity);
                entity.components.Add((i, component.GetType()));
            }


            return re;
        }

        public T GetComponent<T>(Guid entity) where T : Component, new()
        {
            foreach (var component in Components)
            {
                if (component.Entity == entity && component.GetType() == typeof(T))
                {
                    return (T) component;
                }
            }

            return new T();
        }

        public List<Component> GetEntity(Guid id)
        {
            var re = new List<Component>();

            foreach (var component in Components)
            {
                if (component.Entity == id)
                {
                    re.Add(component);
                }
            }

            return re;
        }

        private JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public void SaveFile(string s)
        {
            lock (_locker)
            {
                File.WriteAllText(s, JsonConvert.SerializeObject(Components, _jsonSerializerSettings));
            }
        }

        public void LoadFile(string s)
        {
            lock (_locker)
            {
                Reload(AssetsPath);
                Components =
                    JsonConvert.DeserializeObject<List<Component>>(File.ReadAllText(s), _jsonSerializerSettings);
                Update(true);
            }
        }

        public void DeleteEntity(Guid selectedEntity)
        {
            for (var i = 0; i < Components.Count; i++)
            {
                var component = Components[i];
                if (component.Entity == selectedEntity)
                {
                    Components.RemoveAt(i);
                    DeleteEntity(selectedEntity);
                    break;
                }
            }
        }
    }
}