using System;
using System.Collections.Generic;
using SwinGameSDK;
using Newtonsoft.Json;
using System.IO;

namespace Wormhole
{
	public abstract class ComponentGroup
	{
		protected List<IComponent> components;
		protected string resourcePath;

		public int Mass
		{
			get
			{
				int result = 0;
				foreach (IComponent c in components)
				{
					result += c.Mass;
				}
				return result;
			}
		}

		public ComponentGroup()
		{
			resourcePath = SwinGame.AppPath() + "\\resources";
			components = new List<IComponent>();
		}

		public void InitComponents()
		{
			foreach (IComponent c in components)
			{
				//get json obj and inject it into component for easier testing
				string buffer = File.ReadAllText(resourcePath + c.Path);
				dynamic obj = JsonConvert.DeserializeObject(buffer);

				c.Init(obj);
			}
		}

		public void Update()
		{
			foreach(IComponent c in components)
			{
				c.Update();
			}
		}
		public void Draw()
		{
			foreach(IComponent c in components)
			{
				c.Draw();
			}
		}

		public void AddComponent() { }

		public void RemoveComponent() { }

		public IComponent FetchComponent(string id)
		{
			foreach(IComponent c in components)
			{
				if (c.AreYou(id))
					return c;
			}
			return null;
		}
	}
}
