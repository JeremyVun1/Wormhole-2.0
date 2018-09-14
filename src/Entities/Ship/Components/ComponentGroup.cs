using System;
using System.Linq;
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

		public void Activate<T>()
		{
			foreach(IComponent component in components.OfType<T>())
			{
				component.Activate();
			}
		}

		public void Activate<T>(string id)
		{
			foreach(IComponent component in components.OfType<T>())
			{
				if (component.AreYou(id))
					component.Activate();
			}
		}

		public void Update()
		{
			foreach(IComponent c in components)
			{
				c.Update();
			}
		}
		public void Draw(Point2D pos, Color color)
		{
			foreach(IComponent c in components)
			{
				c.Draw(pos, color);
			}
		}

		public void AddComponent() { }

		public void RemoveComponent() { }

		public IComponent FetchComponent<T>(string id)
		{
			foreach(IComponent c in components.OfType<T>())
			{
				if (c.AreYou(id))
					return c;
			}
			return null;
		}

		public IComponent FetchComponent(string id)
		{
			return FetchComponent<IComponent>(id);
		}
	}
}
