using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using DotFeather;

namespace DotFeather.Router
{
    /// <summary>
    /// シーン遷移などを管理するクラスです。
    /// </summary>
    public class Router
    {
        /// <summary>
        /// 親のゲームクラスを取得します。
        /// </summary>
        /// <value></value>
        public GameBase Game { get; }

        /// <summary>
        /// 親のゲームクラスを指定して <see cref="Router"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="gameBase"></param>
        public Router(GameBase gameBase)
        {
            Game = gameBase;
        }

        /// <summary>
        /// ゲームクラスのアップデート時に呼び出してください。
        /// </summary>
        public void Update(DFEventArgs e)
        {
            current.OnUpdate(this, e);
            Game.BackgroundColor = current.BackgroundColor;
        }

        /// <summary>
        /// シーンを遷移します。
        /// </summary>
        /// <param name="args">遷移先のシーンに渡す引数の辞書。</param>
        /// <typeparam name="T">遷移する対象のシーン。</typeparam>
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
    /// リフレクションを用いつつも高速に、動的にインスタンスを生成します。
    /// Code from https://codeday.me/jp/qa/20190123/149543.html
    /// </summary>
    internal static class New<T>
    {
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <returns></returns>
        internal static readonly Func<T> Instance = Creator();

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

    /// <summary>
    /// <see cref="New{T}"/> クラスの為の拡張メソッドを提供します。
    /// from https://codeday.me/jp/qa/20190123/149543.html
    /// </summary>
    internal static class NewExtension
    {
        /// <summary>
        /// 指定した型がデフォルトコンストラクターを持っているかどうかを判断します。
        /// </summary>
        /// <param name="t">判断する対象の型。</param>
        /// <returns>デフォルトコンストラクターを持っている場合は <c>true</c>。それ以外の場合は <c>false</c>。</returns>
        public static bool HasDefaultConstructor(this Type t)
        {
            return t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;
        }
    }

}
