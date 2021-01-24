using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LiveSplit.UI.Components
{
	public class TOWCompanionQuestCounterComponent : IComponent
	{
		public string ComponentName => throw new NotImplementedException();

		public float HorizontalWidth => throw new NotImplementedException();

		public float MinimumHeight => throw new NotImplementedException();

		public float VerticalHeight => throw new NotImplementedException();

		public float MinimumWidth => throw new NotImplementedException();

		public float PaddingTop => throw new NotImplementedException();

		public float PaddingBottom => throw new NotImplementedException();

		public float PaddingLeft => throw new NotImplementedException();

		public float PaddingRight => throw new NotImplementedException();

		public IDictionary<string, Action> ContextMenuControls => throw new NotImplementedException();

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public void DrawHorizontal(System.Drawing.Graphics g, LiveSplitState state, float height, System.Drawing.Region clipRegion)
		{
			throw new NotImplementedException();
		}

		public void DrawVertical(System.Drawing.Graphics g, LiveSplitState state, float width, System.Drawing.Region clipRegion)
		{
			throw new NotImplementedException();
		}

		public XmlNode GetSettings(XmlDocument document)
		{
			throw new NotImplementedException();
		}

		public System.Windows.Forms.Control GetSettingsControl(LayoutMode mode)
		{
			throw new NotImplementedException();
		}

		public void SetSettings(XmlNode settings)
		{
			throw new NotImplementedException();
		}

		public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
		{
			throw new NotImplementedException();
		}
	}
}
