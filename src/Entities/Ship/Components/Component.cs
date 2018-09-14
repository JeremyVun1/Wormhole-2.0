using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using SwinGameSDK;
using Newtonsoft.Json;

namespace Wormhole
{
	public abstract class Component : IComponent
	{
		//TODO
		//add an Idraweable interface?

		protected ComponentGroup ChildComponents;

		protected List<Color> colors;
		public string Id { get; protected set; }
		public Point2D Pos { get; set; }
		public Vector Dir { get; set; }
		protected Vector TargetDir { get; set; }
		protected float turnRate;
		public string Path { get; set; }
		public int Mass { get; protected set; }
		protected float scale;
		protected Shape shape;
		protected CooldownHandler cdHandler;
		float length;
		public bool Dead = false;

		public virtual void Update() { }
		public virtual void Draw(Point2D parentPos, Color parentColor)
		{
			shape.Draw(Pos.Add(parentPos), parentColor);

			ChildComponents?.Draw(Pos.Add(parentPos), parentColor);
		}
		//public virtual void Draw() { }

		public virtual void Init(JObject obj)
		{
			Id = obj.Value<string>("id");
			Mass = obj.Value<int>("mass");
			scale = obj.Value<float>("scale");
			shape = new Shape(obj.Value<JObject>("shape"), scale);
			length = 10 * scale;
			Pos = Pos.Multiply(length);

			//colors
			colors = new List<Color>();
			if (obj.GetValue("colors") != null)
			{
				foreach (var c in obj.GetValue("colors"))
				{
					colors.Add(SwinGame.RGBColor(c.Value<byte>("R"), c.Value<byte>("G"), c.Value<byte>("B")));
				}
			}

			Dir = SwinGame.VectorTo(0, -1);
			TargetDir = Dir;
		}

		public bool AreYou(string id)
		{
			return Id == id;
		}

		public virtual void Activate() { }
	}
}
