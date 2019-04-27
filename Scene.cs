using System;
using System.Collections.Generic;
using System.Drawing;
using DotFeather;

namespace DotFeather.Router
{
    public abstract class Scene
    {
        public Container Root { get; } = new Container();
        public Random Random { get; private set; } = new Random();
        public Color BackgroundColor { get; set; }

        public void Randomize(int? seed = null)
        {
            Random = seed is int s ? new Random(s) : new Random();
        }
        public virtual void OnStart(Router router, Dictionary<string, object> args) { }
        public virtual void OnUpdate(Router router, DFEventArgs e) { }
        public virtual void OnDestroy(Router router) { }
    }
}