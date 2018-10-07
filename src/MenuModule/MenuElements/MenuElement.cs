using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TaskForceUltra.src.MenuModule
{
	public abstract class MenuElement {
		protected Color hoverColor { get; private set; }
		private Color fillColor;
		private Color borderColor;
		protected Color fontColor { get; private set; }

		private int borderWidth;
		[JsonProperty("text")]
		public string text;
		protected string fontId { get; private set; }
		protected FontAlignment alignment { get; private set; }
		protected Rectangle bounds { get; private set; }

		[JsonProperty("pos")]
		private Point2D pos {
			get {
				return SwinGame.PointAt(bounds.X / SwinGame.ScreenWidth(), bounds.Y / SwinGame.ScreenHeight());
			}
		}
		[JsonProperty("size")]
		private Size2D<float> size {
			get {
				return new Size2D<float>(bounds.Width / SwinGame.ScreenWidth(), bounds.Height / SwinGame.ScreenHeight());
			}
		}

		[JsonProperty("id")]
		public string id { get; private set; }

		public MenuElement(string id, Rectangle bounds, Color hover, Color fill, Color border,
			Color font, string text, string fontId, FontAlignment alignment
		) {
			this.id = id;
			hoverColor = hover;
			fillColor = fill;
			borderColor = border;
			fontColor = font;
			this.text = text;
			this.bounds = bounds;
			this.fontId = fontId;
			this.alignment = alignment;
		}

		public virtual void Draw() {
			SwinGame.FillRectangle(fillColor, bounds);
			DrawOutline();
			SwinGame.DrawText(text, fontColor, Color.Transparent, fontId, alignment, bounds);
		}

		protected void DrawOutline() {
			if (borderWidth > 0) {
				SwinGame.DrawRectangle(borderColor, bounds);
			}
		}
	}

	/// <summary>
	/// Menu Element Factory
	/// </summary>
	public class MenuElementFactory
	{

		public List<MenuElement> Create(JArray textBoxesObj, JArray buttonsObj, JArray colorsObj, IMenuModule menuModule) {
			List<MenuElement> result = new List<MenuElement>();

			Color hoverColor = Util.DeserializeKeyedColor(colorsObj, "hoverColor");
			Color fillColor = Util.DeserializeKeyedColor(colorsObj, "fillColor");
			Color borderColor = Util.DeserializeKeyedColor(colorsObj, "borderColor");
			Color fontColor = Util.DeserializeKeyedColor(colorsObj, "fontColor");

			foreach (JObject obj in textBoxesObj) {
				result.Add(CreateTextBox(obj, hoverColor, fillColor, borderColor, fontColor));
			}

			foreach (JObject obj in buttonsObj) {
				result.Add(CreateButton(obj, hoverColor, fillColor, borderColor, fontColor, menuModule));
			}

			return result;
		}

		public TextBox CreateTextBox(JObject textObj, Color hover, Color fill, Color border, Color font) {
			Rectangle bounds = CreateElementBounds(textObj);
			string text = textObj.Value<string>("text");
			string id = textObj.Value<string>("id");

			return new TextBox(id, bounds, hover, fill, border, font, text);
		}

		public Button CreateButton(JObject buttonObj, Color hover, Color fill, Color border, Color font, IMenuModule menuModule) {
			Rectangle bounds = CreateElementBounds(buttonObj);
			string text = buttonObj.Value<string>("label");
			string action = buttonObj.Value<string>("action");
			string payload = buttonObj.Value<string>("payload");
			string id = buttonObj.Value<string>("id");
			string type = buttonObj.Value<string>("type");

			//build command for button
			MenuCommandFactory menuCommandFac = new MenuCommandFactory(menuModule);
			ICommand command = menuCommandFac.Create(action, payload);

			switch(type.ToLower()) {
				case "nonstick":
					return new NonStickButton(id, command, bounds, hover, fill, border, font, text);
				default:
					return new Button(id, command, bounds, hover, fill, border, font, text);
			}
		}

		/// <summary>
		/// built on runtime to allow the plaeyr to choose different data selections
		/// </summary>
		/// <returns></returns>
		public SelectButton CreateSelectButton(string id, string label, Rectangle bounds, SelectionType type, string payload, SelectionGroup parent, IMenuModule menuModule)
		{
			MenuCommandFactory menuCommandFac = new MenuCommandFactory(menuModule);
			ICommand command;

			switch (type) {
				case SelectionType.Difficulty:
					command = new SelectDifficultyCommand(menuModule, payload);
					break;
				case SelectionType.Level:
					command = new SelectLevelCommand(menuModule, payload);
					break;
				case SelectionType.Ship:
					command = new SelectShipCommand(menuModule, payload);
					break;
				default:
					command = null;
					break;
			}
			Color hover = Color.Orange;
			Color fill = Color.Grey;
			Color border = Color.White;
			Color font = Color.White;

			return new SelectButton(id, command, bounds, hover, fill, border, font, payload, parent, type);
		}

		private Rectangle CreateElementBounds(JObject obj) {
			Point2D pos = obj["pos"].ToObject<Point2D>();
			Size2D<float> size = obj["size"].ToObject<Size2D<float>>();
			int w = SwinGame.ScreenWidth();
			int h = SwinGame.ScreenHeight();

			return SwinGame.CreateRectangle(pos.X * w, pos.Y * h, size.W * w, size.H * h);
		}
	}
}
