using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using SwinGameSDK;

namespace Wormhole
{
	public abstract class Component : IComponent
	{
		//TODO
		//add an Idraweable interface?

		protected ComponentGroup ChildComponents;
		protected string id;
		protected Point2D pos;
		protected float scale;
		protected Shape shape;
		public string Path { get; set; }
		public int Mass { get; protected set; }

		/*public Component()
		{
			path = jObj.Path;
			id = jObj.id;

			Init(resourcePath);
		}*/

		public void Update() { }
		public void Draw()
		{
			//Shape.Draw();
		}

		public abstract void Init(dynamic obj);

		public bool AreYou(string id)
		{
			return this.id == id;
		}
	}
}
