using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.WPF
{
    public static class DependencyResolver
    {
        static DependencyResolver()
        {
            _typesDictionary = new Dictionary<TypeEntry, object>();
            _lifeStylesDictionary = new Dictionary<Type, LifeStyle>();
        }
        private static Dictionary<TypeEntry, object> _typesDictionary;
        private static Dictionary<Type, LifeStyle> _lifeStylesDictionary;

        public static void RegisterSingleton<T, TImpl>()
            where TImpl: T, new()
        {
            if (!_typesDictionary.ContainsKey(new TypeEntry(typeof(T), typeof(TImpl))))
            {
                _typesDictionary.Add(new TypeEntry(typeof(T), typeof(TImpl)), new TImpl());
                _lifeStylesDictionary.Add(typeof(T), LifeStyle.Singleton);
            }

        }

        public static void Register<T, TImpl>()
            where TImpl : T, new()
        {
            var abstractAlreadyRegistered = _typesDictionary.Any(x => x.Key.Abstract == typeof(T));
            if (abstractAlreadyRegistered)
                throw new Exception($"The abstract type {nameof(T)} has more than one registered implementations");

            if (!_typesDictionary.ContainsKey(new TypeEntry(typeof(T), typeof(TImpl))))
            {
                _typesDictionary.Add(new TypeEntry(typeof(T), typeof(TImpl)), new TImpl());
                _lifeStylesDictionary.Add(typeof(T), LifeStyle.Transient);
            }
        }

        public static T Resolve<T>()
        {
            if(!_lifeStylesDictionary.TryGetValue(typeof(T), out var lifeStyle))
                throw new Exception($"Resolve error: can't find registration of type {nameof(T)}");

            var entry = _typesDictionary.Where(x => x.Key.Abstract == typeof(T)).SingleOrDefault();

            return lifeStyle == LifeStyle.Singleton ?
                    (T)entry.Value :
                    (T)Activator.CreateInstance(entry.Key.Implementation);
        }

        enum LifeStyle
        {
            Transient,
            Singleton
        }
    }

    internal struct TypeEntry
    {
        public TypeEntry(Type a, Type i)
        {
            Abstract = a;
            Implementation = i;
        }
        public Type Abstract { get; set; }
        public Type Implementation { get; set; }
    }
}
