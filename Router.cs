using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using DotFeather;

namespace DotFeather.Router
{
    public class Router
    {
        public GameBase Game { get; }

        public Router(GameBase gameBase)
        {
            Game = gameBase;
        }

        public void Update(DFEventArgs e)
        {
            current.OnUpdate(this, e);
            Game.BackgroundColor = current.BackgroundColor;
        }

        public void ChangeScene<T>(Dictionary<string, object> args = default) where T : Scene
        {
            if (current != default)
            {
                current.OnDestroy(this);
                Game.Root.Remove(current.Root);
                current = null;
            }
            current = New<T>.Instance();
            current.OnStart(this, args ?? new Dictionary<string, object>());
            Game.Root.Add(current.Root);
        }

        private Scene current;
    }

    /// <summary>
    /// from https://codeday.me/jp/qa/20190123/149543.html
    /// </summary>
    internal static class New<T>
    {
        public static readonly Func<T> Instance = Creator();

        static Func<T> Creator()
        {
            Type t = typeof(T);
            if (t == typeof(string))
                return Expression.Lambda<Func<T>>(Expression.Constant(string.Empty)).Compile();

            if (t.HasDefaultConstructor())
                return Expression.Lambda<Func<T>>(Expression.New(t)).Compile();

            return () => (T)FormatterServices.GetUninitializedObject(t);
        }
    }

    public static class NewExtension
    {
        public static bool HasDefaultConstructor(this Type t)
        {
            return t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;
        }
    }

}
