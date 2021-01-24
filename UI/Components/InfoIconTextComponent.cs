using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LiveSplit.UI.Components
{
	public class InfoIconTextComponent : IComponent
	{
		public string InformationName { get { return NameLabel.Text; } set { NameLabel.Text = value; } }
		public string InformationValue { get { return ValueLabel.Text; } set { ValueLabel.Text = value; } }

		public GraphicsCache Cache { get; set; }

		public ICollection<string> AlternateNameText { get { return NameLabel.AlternateText; } set { NameLabel.AlternateText = value; } }

		public SimpleLabel NameLabel { get; protected set; }
		public SimpleLabel ValueLabel { get; protected set; }

		public string LongestString { get; set; }
		protected SimpleLabel NameMeasureLabel { get; set; }

		public Image Icon { get; set; }
		public Image ShadowImage { get; set; }
		protected Image OldImage { get; set; }
		protected int IconWidth => Icon != null ? Icon.Width : 0;
		protected float IconSize => 24f;

		public float PaddingTop { get; set; }
		public float PaddingLeft => 7f;
		public float PaddingBottom { get; set; }
		public float PaddingRight => 7f;

		public float VerticalHeight { get; set; }
		public float MinimumWidth => 20 + IconWidth;
		public float HorizontalWidth
			=> Math.Max(NameMeasureLabel.ActualWidth, ValueLabel.ActualWidth) + IconWidth + 10;
		public float MinimumHeight { get; set; }

		public InfoIconTextComponent(string informationName, string informationValue, Image icon = null)
		{
			Cache = new GraphicsCache();
			NameLabel = new SimpleLabel()
			{
				HorizontalAlignment = StringAlignment.Near,
				Text = informationName
			};
			ValueLabel = new SimpleLabel()
			{
				HorizontalAlignment = StringAlignment.Far,
				Text = informationValue
			};
			Icon = icon;
			NameMeasureLabel = new SimpleLabel();
			MinimumHeight = 25;
			VerticalHeight = 31;
			LongestString = "";
		}

		public virtual void PrepareDraw(LiveSplitState state, LayoutMode mode)
		{
			NameMeasureLabel.Font = state.LayoutSettings.TextFont;
			ValueLabel.Font = state.LayoutSettings.TextFont;
			NameLabel.Font = state.LayoutSettings.TextFont;
			if (mode == LayoutMode.Vertical)
			{
				NameLabel.VerticalAlignment = StringAlignment.Center;
				ValueLabel.VerticalAlignment = StringAlignment.Center;
			}
			else
			{
				NameLabel.VerticalAlignment = StringAlignment.Near;
				ValueLabel.VerticalAlignment = StringAlignment.Far;
			}
		}

		public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
		{
			VerticalHeight = 31;
			NameLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
			NameLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
			ValueLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
			ValueLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;

			var textHeight = 0.75f * Math.Max(g.MeasureString("A", ValueLabel.Font).Height, g.MeasureString("A", NameLabel.Font).Height);
			PaddingTop = Math.Max(0, (VerticalHeight - textHeight) / 2f);
			PaddingBottom = PaddingTop;

			NameMeasureLabel.Text = LongestString;
			NameMeasureLabel.SetActualWidth(g);
			ValueLabel.SetActualWidth(g);

			NameLabel.Width = width - ValueLabel.ActualWidth - 10;
			NameLabel.Height = VerticalHeight;
			NameLabel.X = 5 + IconWidth;
			NameLabel.Y = 0;

			ValueLabel.Width = ValueLabel.IsMonospaced ? width - 12 : width - 10;
			ValueLabel.Height = VerticalHeight;
			ValueLabel.Y = 0;
			ValueLabel.X = 5;

			PrepareDraw(state, LayoutMode.Vertical);

			if (Icon != null)
			{
				var shadow = ShadowImage;

				if (OldImage != Icon)
				{
					ImageAnimator.Animate(Icon, (s, o) => { });
					ImageAnimator.Animate(shadow, (s, o) => { });
					OldImage = Icon;
				}

				var drawWidth = IconSize;
				var drawHeight = IconSize;
				var shadowWidth = IconSize * (5 / 4f);
				var shadowHeight = IconSize * (5 / 4f);
				if (Icon.Width > Icon.Height)
				{
					var ratio = Icon.Height / (float)Icon.Width;
					drawHeight *= ratio;
					shadowHeight *= ratio;
				}
				else
				{
					var ratio = Icon.Width / (float)Icon.Height;
					drawWidth *= ratio;
					shadowWidth *= ratio;
				}

				ImageAnimator.UpdateFrames(shadow);

				ImageAnimator.UpdateFrames(Icon);

				g.DrawImage(
					Icon,
					7 + (IconSize - drawWidth) / 2,
					(VerticalHeight - IconSize) / 2.0f + (IconSize - drawHeight) / 2,
					drawWidth,
					drawHeight);
			}

			NameLabel.Draw(g);
			ValueLabel.Draw(g);
		}

		public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
		{
			DrawTwoRows(g, state, HorizontalWidth, height);
		}

		protected void DrawTwoRows(Graphics g, LiveSplitState state, float width, float height)
		{
			NameLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
			NameLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
			ValueLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
			ValueLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;

			if (InformationName != null && LongestString != null && InformationName.Length > LongestString.Length)
			{
				LongestString = InformationName;
				NameMeasureLabel.Text = LongestString;
			}
			NameMeasureLabel.Text = LongestString;
			NameMeasureLabel.Font = state.LayoutSettings.TextFont;
			NameMeasureLabel.SetActualWidth(g);

			MinimumHeight = 0.85f * (g.MeasureString("A", ValueLabel.Font).Height + g.MeasureString("A", NameLabel.Font).Height);
			NameLabel.Width = width - 10;
			NameLabel.Height = height;
			NameLabel.X = 5;
			NameLabel.Y = 0;

			ValueLabel.Width = ValueLabel.IsMonospaced ? width - 12 : width - 10;
			ValueLabel.Height = height;
			ValueLabel.Y = 0;
			ValueLabel.X = 5;

			PrepareDraw(state, LayoutMode.Horizontal);

			if (Icon != null)
			{
				var shadow = ShadowImage;

				if (OldImage != Icon)
				{
					ImageAnimator.Animate(Icon, (s, o) => { });
					ImageAnimator.Animate(shadow, (s, o) => { });
					OldImage = Icon;
				}

				var drawWidth = IconSize;
				var drawHeight = IconSize;
				var shadowWidth = IconSize * (5 / 4f);
				var shadowHeight = IconSize * (5 / 4f);
				if (Icon.Width > Icon.Height)
				{
					var ratio = Icon.Height / (float)Icon.Width;
					drawHeight *= ratio;
					shadowHeight *= ratio;
				}
				else
				{
					var ratio = Icon.Width / (float)Icon.Height;
					drawWidth *= ratio;
					shadowWidth *= ratio;
				}

				ImageAnimator.UpdateFrames(shadow);

				ImageAnimator.UpdateFrames(Icon);

				g.DrawImage(
					Icon,
					7 + (IconSize - drawWidth) / 2,
					(height - IconSize) / 2.0f + (IconSize - drawHeight) / 2,
					drawWidth,
					drawHeight);
			}

			NameLabel.Draw(g);
			ValueLabel.Draw(g);
		}

		public string ComponentName
		{
			get { throw new NotSupportedException(); }
		}

		public Control GetSettingsControl(LayoutMode mode)
		{
			throw new NotImplementedException();
		}

		public void SetSettings(System.Xml.XmlNode settings)
		{
			throw new NotImplementedException();
		}

		public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
		{
			throw new NotImplementedException();
		}

		public string UpdateName
		{
			get { throw new NotSupportedException(); }
		}

		public string XMLURL
		{
			get { throw new NotSupportedException(); }
		}

		public string UpdateURL
		{
			get { throw new NotSupportedException(); }
		}

		public Version Version
		{
			get { throw new NotSupportedException(); }
		}

		public IDictionary<string, Action> ContextMenuControls => throw new NotImplementedException();

		public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
		{
			Cache.Restart();
			Cache["NameText"] = InformationName;
			Cache["ValueText"] = InformationValue;
			Cache["NameColor"] = NameLabel.ForeColor.ToArgb();
			Cache["ValueColor"] = ValueLabel.ForeColor.ToArgb();
			Cache["Icon"] = Icon;

			if (invalidator != null && Cache.HasChanged)
			{
				invalidator.Invalidate(0, 0, width, height);
			}
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}