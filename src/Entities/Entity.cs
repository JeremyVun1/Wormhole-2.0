using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace Wormhole
{
	public abstract class Entity
	{
		public string Id { get; set; }
		protected Point2D Pos { get; set; }

		protected float Scale { get; set; }
		protected Shape Shape { get; set; }
		protected Color Clr { get; set; }

		public Entity(dynamic id, dynamic rgb, dynamic shape, dynamic scale)
		{
			Id = id;
			Clr = SwinGame.RGBColor((byte)rgb[0], (byte)rgb[1], (byte)rgb[2]);
			Scale = scale;
			Shape = new Shape(shape, scale);
		}

		public virtual void Update() { }

		public virtual void Draw()
		{
			Shape.Draw(Pos, Clr);
		}
	}
}
