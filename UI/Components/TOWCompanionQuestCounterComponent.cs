using LiveSplit.ComponentUtil;
using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
	class TOWQuestCounterComponent : IComponent
	{
		public string ComponentName
		{
			get { return "TOW Companion Quest Counter"; }
		}

		public IDictionary<string, Action> ContextMenuControls { get; protected set; }
		protected ComponentRendererComponent InternalComponent;

		private LiveSplitState _state;
		private int _count;
		private bool IsHooked = false;
		private int UpdateTime = 0;

		private Process Game;
		private int QuestOffset, FoundationOffset;

		private List<Quest> Quests;

		private const string checkMark = "\u2713";

		private List<InfoIconTextComponent> CreateComponentList()
		{
			var assembly = Assembly.GetExecutingAssembly();
			return new List<InfoIconTextComponent>
			{
				new InfoIconTextComponent("ellie", "X"),
				new InfoIconTextComponent("sam", "X"),
				new InfoIconTextComponent("max", "X"),
				new InfoIconTextComponent("nyoka", "X"),
				new InfoIconTextComponent("felix", "X"),
				new InfoIconTextComponent("parvati", "X")
			};
		}

		public TOWQuestCounterComponent(LiveSplitState state)
		{
			this.InternalComponent = new ComponentRendererComponent();
			this.InternalComponent.VisibleComponents = CreateComponentList();

			_state = state;
			_state.OnReset += state_OnReset;
		}

		public void Dispose()
		{
			_state.OnReset -= state_OnReset;
		}

		public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
		{
			UpdateTime++;
			if (UpdateTime > 30)
			{
				UpdateTime = 0;
				if (Game != null && Game.HasExited)
				{
					Game = null;
					IsHooked = false;
				}
				if (!IsHooked)
				{
					List<Process> GameProcesses = Process.GetProcesses().ToList().FindAll(x => x.ProcessName.StartsWith("Indiana"));
					if (GameProcesses.Count > 0)
					{
						Game = GameProcesses.First();
						string version = "";
						switch (Game.MainModule.ModuleMemorySize)
						{
							case 71692288:
								version = "v1.0 (EGS)";
								QuestOffset = 0x03D9C7F8;
								FoundationOffset = 0x03FF7408;
								break;
							case 71729152:
								version = "v1.1 (EGS)";
								QuestOffset = 0x03DA3978;
								FoundationOffset = 0x03FFE788;
								break;
							case 71880704:
								version = "v1.2 (EGS)";
								break;
							case 74125312:
								version = "v1.1 (MS)";
								QuestOffset = 0x03FF0078;
								FoundationOffset = 0x0424AD78;
								break;
						}
						IsHooked = new DeepPointer(QuestOffset, 0x20, 0x0, 0x8, 0x18, 0x8, 0x0).Deref<ulong>(Game, out var ignore);
						if (IsHooked)
						{
							Debug.WriteLine($"TOW {version} found.");
							Debug.WriteLine("Found Quest Offsets");

							Quests = new List<Quest>()
							{
                                //Ellie
							   new Quest("low_crusade",
									new MultiPointer[] {
										new MultiPointer(new DeepPointer(QuestOffset, 0x20, 0x0, 0x8, 0x18, 0x8, 0x0, 0xBF10),
											new Comparison[] {
												new Comparison(5, Comparison.EQUALS)
											})
									},
									Companion.Ellie),
                                //Vicar Max
								new Quest("empty_man",
									new MultiPointer[] {
										new MultiPointer(new DeepPointer(QuestOffset, 0x20, 0x0, 0x8, 0x18, 0x8, 0x0, 0xB7B0),
											new Comparison[] {
												new Comparison(10, Comparison.EQUALS)
											})
									},
									Companion.Max),                              
                                //Felix
                                new Quest("friendships_due",
									new MultiPointer[] {
										new MultiPointer(new DeepPointer(QuestOffset, 0x20, 0x0, 0x8, 0x18, 0x8, 0x0, 0xAF70),
											new Comparison[] {
												new Comparison(6, Comparison.EQUALS)
											})
									},
									Companion.Felix),
                                //S.A.M.
                                new Quest("cleaning_machine",
									new MultiPointer[] {
										new MultiPointer(new DeepPointer(QuestOffset, 0x20, 0x0, 0x8, 0x18, 0x8, 0x0, 0x41F0),
											new Comparison[] {
												new Comparison(1, Comparison.EQUALS)
											})
									},
									Companion.SAM),
                                //Nyoka
                                new Quest("star_crossed_troopers",
									new MultiPointer[] {
										new MultiPointer(new DeepPointer(QuestOffset, 0x20, 0x0, 0x8, 0x18, 0x8, 0x0, 0x3F0),
											new Comparison[] {
												new Comparison(1, Comparison.EQUALS)
											})
									},
									Companion.Nyoka),
                                //Parvati
								new Quest("bite_the_sun",
									new MultiPointer[] {
										new MultiPointer(new DeepPointer(QuestOffset, 0x20, 0x0, 0x8, 0x18, 0x8, 0x0, 0x8370),
											new Comparison[] {
												new Comparison(6, Comparison.EQUALS)
											})
									},
									Companion.Parvati),
							};
						}
					}
				}
				else
				{
					for (int i = 0; i < Quests.Count; i++)
					{
						for (int x = 0; x < Quests[i].Pointers.Length; x++)
						{
							int completionState = 0;
							Quests[i].Pointers[x].Pointer.Deref<int>(Game, out completionState);
							for (int z = 0; z < Quests[i].Pointers[x].Comparisons.Length; z++)
							{
								Quests[i].Pointers[x].Comparisons[z].Completed = true;
								switch (Quests[i].Pointers[x].Comparisons[z].Comparator)
								{
									case Comparison.EQUALS:
										Companion companion = Quests[i].Companion;
										InfoIconTextComponent companionComponent = InternalComponent.VisibleComponents.Cast<InfoIconTextComponent>()
																					.First(component => component.InformationName == companion.ToString().ToLower());
										if (!Quests[i].Pointers[x].Comparisons[z].CompletionStates.Contains(completionState))
										{
											companionComponent.InformationValue = $"X";

											Quests[i].Pointers[x].Comparisons[z].Completed = false;
										}
										else
										{
											companionComponent.InformationValue = "\u2713";
										}
										break;
									case Comparison.NOT_EQUALS:
										if (Quests[i].Pointers[x].Comparisons[z].CompletionStates.Contains(completionState))
											Quests[i].Pointers[x].Comparisons[z].Completed = false;
										break;
									case Comparison.GREATER_THAN:
										if (!(completionState > Quests[i].Pointers[x].Comparisons[z].CompletionState))
											Quests[i].Pointers[x].Comparisons[z].Completed = false;
										break;
									case Comparison.GREATER_THAN_OR_EQUAL:
										if (!(completionState >= Quests[i].Pointers[x].Comparisons[z].CompletionState))
											Quests[i].Pointers[x].Comparisons[z].Completed = false;
										break;
									case Comparison.LESS_THAN:
										if (!(completionState < Quests[i].Pointers[x].Comparisons[z].CompletionState))
											Quests[i].Pointers[x].Comparisons[z].Completed = false;
										break;
									case Comparison.LESS_THAN_OR_EQUAL:
										if (!(completionState <= Quests[i].Pointers[x].Comparisons[z].CompletionState))
											Quests[i].Pointers[x].Comparisons[z].Completed = false;
										break;
								}
							}
							Quests[i].Pointers[x].Completed = Array.FindAll(Quests[i].Pointers[x].Comparisons, k => k.Completed == true).Length == Quests[i].Pointers[x].Comparisons.Length;
						}
						Quests[i].Completed = Quests[i].Both ? Array.FindAll(Quests[i].Pointers, k => k.Completed == true).Length == Quests[i].Pointers.Length : Array.FindAll(Quests[i].Pointers, k => k.Completed == true).Length != 0;
					}

					_count = Quests.FindAll(x => x.Completed == true).Count;
				}
			}

			if (invalidator != null)
			{
				invalidator.Invalidate(0f, 0f, width, height);
			}
		}

		public void DrawVertical(Graphics g, LiveSplitState state, float width, Region region)
		{
			PrepareDraw(state);
			this.InternalComponent.DrawVertical(g, state, width, region);
		}

		public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region region)
		{
			PrepareDraw(state);
			this.InternalComponent.DrawHorizontal(g, state, height, region);
		}

		void state_OnReset(object sender, TimerPhase t)
		{
			_count = 0;
		}

		void PrepareDraw(LiveSplitState state)
		{
			foreach (InfoIconTextComponent component in this.InternalComponent.VisibleComponents)
			{
				component.NameLabel.ForeColor = component.ValueLabel.ForeColor = state.LayoutSettings.TextColor;
				component.NameLabel.HasShadow = component.ValueLabel.HasShadow = state.LayoutSettings.DropShadows;
			}
		}

		public XmlNode GetSettings(XmlDocument document) { return document.CreateElement("Settings"); }
		public Control GetSettingsControl(LayoutMode mode) { return null; }
		public void SetSettings(XmlNode settings) { }
		public void RenameComparison(string oldName, string newName) { }
		public float MinimumWidth { get { return this.InternalComponent.MinimumWidth; } }
		public float MinimumHeight { get { return this.InternalComponent.MinimumHeight; } }
		public float VerticalHeight { get { return this.InternalComponent.VerticalHeight; } }
		public float HorizontalWidth { get { return this.InternalComponent.HorizontalWidth; } }
		public float PaddingLeft { get { return this.InternalComponent.PaddingLeft; } }
		public float PaddingRight { get { return this.InternalComponent.PaddingRight; } }
		public float PaddingTop { get { return this.InternalComponent.PaddingTop; } }
		public float PaddingBottom { get { return this.InternalComponent.PaddingBottom; } }
	}
}