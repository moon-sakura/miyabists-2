// sts2, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
// MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect.ICharacterSelectButtonDelegate
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

public interface ICharacterSelectButtonDelegate
{
	StartRunLobby Lobby { get; }

	void SelectCharacter(NCharacterSelectButton charSelectButton, CharacterModel characterModel);
}

// sts2, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
// MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect.NActDropdown
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

[ScriptPath("res://src/Core/Nodes/Screens/CharacterSelect/NActDropdown.cs")]
public class NActDropdown : NDropdown
{
	public new class MethodName : NDropdown.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public new static readonly StringName OnFocus = "OnFocus";

		public new static readonly StringName OnUnfocus = "OnUnfocus";

		public static readonly StringName PopulateOptions = "PopulateOptions";

		public static readonly StringName OnDropdownItemSelected = "OnDropdownItemSelected";

		public static readonly StringName GetDropdownContainer = "GetDropdownContainer";
	}

	public new class PropertyName : NDropdown.PropertyName
	{
		public static readonly StringName CurrentOption = "CurrentOption";

		public static readonly StringName _currentOptionIndex = "_currentOptionIndex";
	}

	public new class SignalName : NDropdown.SignalName
	{
	}

	private static readonly string[] _options = new string[3] { "random", "overgrowth", "underdocks" };

	private int _currentOptionIndex = _options.IndexOf("random");

	public string CurrentOption => _options[_currentOptionIndex];

	public override void _Ready()
	{
		ConnectSignals();
		PopulateOptions();
	}

	protected override void OnFocus()
	{
		_currentOptionHighlight.Modulate = new Color("afcdde");
	}

	protected override void OnUnfocus()
	{
		_currentOptionHighlight.Modulate = Colors.White;
	}

	private void PopulateOptions()
	{
		List<NDropdownItem> list = GetDropdownItems().ToList();
		for (int i = 0; i < _options.Length; i++)
		{
			NDropdownItem nDropdownItem = list[i];
			string text = _options[i];
			nDropdownItem.Connect(NDropdownItem.SignalName.Selected, Callable.From<NDropdownItem>(OnDropdownItemSelected));
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 2);
			defaultInterpolatedStringHandler.AppendFormatted(char.ToUpperInvariant(text[0]));
			string text2 = text;
			defaultInterpolatedStringHandler.AppendFormatted(text2.Substring(1, text2.Length - 1));
			nDropdownItem.Text = defaultInterpolatedStringHandler.ToStringAndClear();
		}
		GetDropdownContainer().GetParent<NDropdownContainer>().RefreshLayout();
	}

	private void OnDropdownItemSelected(NDropdownItem item)
	{
		CloseDropdown();
		_currentOptionIndex = GetDropdownItems().ToList().IndexOf(item);
		_currentOptionLabel.SetTextAutoSize(item.Text);
	}

	private Control GetDropdownContainer()
	{
		return GetNode<Control>("DropdownContainer/VBoxContainer");
	}

	private IEnumerable<NDropdownItem> GetDropdownItems()
	{
		return GetDropdownContainer().GetChildren().OfType<NDropdownItem>();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.PopulateOptions, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnDropdownItemSelected, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "item", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.GetDropdownContainer, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false), MethodFlags.Normal, null, null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName._Ready && args.Count == 0)
		{
			_Ready();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnFocus && args.Count == 0)
		{
			OnFocus();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnUnfocus && args.Count == 0)
		{
			OnUnfocus();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.PopulateOptions && args.Count == 0)
		{
			PopulateOptions();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnDropdownItemSelected && args.Count == 1)
		{
			OnDropdownItemSelected(VariantUtils.ConvertTo<NDropdownItem>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.GetDropdownContainer && args.Count == 0)
		{
			ret = VariantUtils.CreateFrom<Control>(GetDropdownContainer());
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName._Ready)
		{
			return true;
		}
		if (method == MethodName.OnFocus)
		{
			return true;
		}
		if (method == MethodName.OnUnfocus)
		{
			return true;
		}
		if (method == MethodName.PopulateOptions)
		{
			return true;
		}
		if (method == MethodName.OnDropdownItemSelected)
		{
			return true;
		}
		if (method == MethodName.GetDropdownContainer)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._currentOptionIndex)
		{
			_currentOptionIndex = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.CurrentOption)
		{
			value = VariantUtils.CreateFrom<string>(CurrentOption);
			return true;
		}
		if (name == PropertyName._currentOptionIndex)
		{
			value = VariantUtils.CreateFrom(in _currentOptionIndex);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName._currentOptionIndex, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.String, PropertyName.CurrentOption, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._currentOptionIndex, Variant.From(in _currentOptionIndex));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._currentOptionIndex, out var value))
		{
			_currentOptionIndex = value.As<int>();
		}
	}
}

// sts2, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
// MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect.NAscensionPanel
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Saves;

[ScriptPath("res://src/Core/Nodes/Screens/CharacterSelect/NAscensionPanel.cs")]
public class NAscensionPanel : Control
{
	[Signal]
	public delegate void AscensionLevelChangedEventHandler();

	public new class MethodName : Control.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public static readonly StringName Initialize = "Initialize";

		public static readonly StringName SetFireBlue = "SetFireBlue";

		public static readonly StringName SetFireRed = "SetFireRed";

		public static readonly StringName Cleanup = "Cleanup";

		public static readonly StringName SetAscensionLevel = "SetAscensionLevel";

		public static readonly StringName IncrementAscension = "IncrementAscension";

		public static readonly StringName DecrementAscension = "DecrementAscension";

		public static readonly StringName RefreshArrowVisibility = "RefreshArrowVisibility";

		public static readonly StringName SetMaxAscension = "SetMaxAscension";

		public static readonly StringName RefreshAscensionText = "RefreshAscensionText";

		public static readonly StringName AnimIn = "AnimIn";

		public static readonly StringName UpdateControllerButton = "UpdateControllerButton";
	}

	public new class PropertyName : Control.PropertyName
	{
		public static readonly StringName Ascension = "Ascension";

		public static readonly StringName _maxAscension = "_maxAscension";

		public static readonly StringName _leftArrow = "_leftArrow";

		public static readonly StringName _rightArrow = "_rightArrow";

		public static readonly StringName _ascensionLevel = "_ascensionLevel";

		public static readonly StringName _info = "_info";

		public static readonly StringName _leftTriggerIcon = "_leftTriggerIcon";

		public static readonly StringName _rightTriggerIcon = "_rightTriggerIcon";

		public static readonly StringName _iconHsv = "_iconHsv";

		public static readonly StringName _arrowsVisible = "_arrowsVisible";

		public static readonly StringName _mode = "_mode";

		public static readonly StringName _tween = "_tween";
	}

	public new class SignalName : Control.SignalName
	{
		public static readonly StringName AscensionLevelChanged = "AscensionLevelChanged";
	}

	private static readonly StringName _tabLeftHotkey = MegaInput.viewDeckAndTabLeft;

	private static readonly StringName _tabRightHotkey = MegaInput.viewExhaustPileAndTabRight;

	private static readonly StringName _fontOutlineTheme = "font_outline_color";

	private static readonly StringName _h = new StringName("h");

	private static readonly StringName _v = new StringName("v");

	private static readonly Color _redLabelOutline = new Color("593400");

	private static readonly Color _blueLabelOutline = new Color("004759");

	private int _maxAscension;

	private NButton _leftArrow;

	private NButton _rightArrow;

	private MegaLabel _ascensionLevel;

	private MegaRichTextLabel _info;

	private TextureRect _leftTriggerIcon;

	private TextureRect _rightTriggerIcon;

	private ShaderMaterial _iconHsv;

	private bool _arrowsVisible = true;

	private MultiplayerUiMode _mode = MultiplayerUiMode.Singleplayer;

	private Tween? _tween;

	private AscensionLevelChangedEventHandler backing_AscensionLevelChanged;

	public int Ascension { get; private set; }

	public event AscensionLevelChangedEventHandler AscensionLevelChanged
	{
		add
		{
			backing_AscensionLevelChanged = (AscensionLevelChangedEventHandler)Delegate.Combine(backing_AscensionLevelChanged, value);
		}
		remove
		{
			backing_AscensionLevelChanged = (AscensionLevelChangedEventHandler)Delegate.Remove(backing_AscensionLevelChanged, value);
		}
	}

	public override void _Ready()
	{
		_leftTriggerIcon = GetNode<TextureRect>("%LeftTriggerIcon");
		_rightTriggerIcon = GetNode<TextureRect>("%RightTriggerIcon");
		_leftArrow = GetNode<NButton>("HBoxContainer/LeftArrowContainer/LeftArrow");
		_rightArrow = GetNode<NButton>("HBoxContainer/RightArrowContainer/RightArrow");
		_ascensionLevel = GetNode<MegaLabel>("HBoxContainer/AscensionIconContainer/AscensionIcon/AscensionLevel");
		_info = GetNode<MegaRichTextLabel>("HBoxContainer/AscensionDescription/Description");
		_iconHsv = (ShaderMaterial)GetNode<Control>("%AscensionIcon").Material;
		_leftArrow.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(delegate
		{
			DecrementAscension();
		}));
		_rightArrow.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(delegate
		{
			IncrementAscension();
		}));
		NControllerManager.Instance.Connect(NControllerManager.SignalName.MouseDetected, Callable.From(UpdateControllerButton));
		NControllerManager.Instance.Connect(NControllerManager.SignalName.ControllerDetected, Callable.From(UpdateControllerButton));
		NInputManager.Instance.Connect(NInputManager.SignalName.InputRebound, Callable.From(UpdateControllerButton));
		UpdateControllerButton();
	}

	public void Initialize(MultiplayerUiMode mode)
	{
		_mode = mode;
		if (_mode == MultiplayerUiMode.Host)
		{
			SetFireBlue();
			_arrowsVisible = true;
			SetMaxAscension(SaveManager.Instance.Progress.MaxMultiplayerAscension);
			SetAscensionLevel(Math.Min(_maxAscension, SaveManager.Instance.Progress.PreferredMultiplayerAscension));
			NHotkeyManager.Instance.PushHotkeyPressedBinding(_tabLeftHotkey, DecrementAscension);
			NHotkeyManager.Instance.PushHotkeyPressedBinding(_tabRightHotkey, IncrementAscension);
		}
		else if (_mode == MultiplayerUiMode.Singleplayer)
		{
			SetFireRed();
			_arrowsVisible = true;
			SetMaxAscension(0);
			SetAscensionLevel(0);
			NHotkeyManager.Instance.PushHotkeyPressedBinding(_tabLeftHotkey, DecrementAscension);
			NHotkeyManager.Instance.PushHotkeyPressedBinding(_tabRightHotkey, IncrementAscension);
		}
		else
		{
			MultiplayerUiMode mode2 = _mode;
			if ((uint)(mode2 - 3) <= 1u)
			{
				SetFireBlue();
				_arrowsVisible = false;
				SetMaxAscension(0);
			}
		}
	}

	private void SetFireBlue()
	{
		_iconHsv.SetShaderParameter(_h, 0.52f);
		_iconHsv.SetShaderParameter(_v, 1.2f);
		_ascensionLevel.AddThemeColorOverride(_fontOutlineTheme, _blueLabelOutline);
	}

	private void SetFireRed()
	{
		_iconHsv.SetShaderParameter(_h, 1f);
		_iconHsv.SetShaderParameter(_v, 1f);
		_ascensionLevel.AddThemeColorOverride(_fontOutlineTheme, _redLabelOutline);
	}

	public void Cleanup()
	{
		MultiplayerUiMode mode = _mode;
		if ((uint)(mode - 1) <= 1u)
		{
			NHotkeyManager.Instance.RemoveHotkeyPressedBinding(_tabLeftHotkey, DecrementAscension);
			NHotkeyManager.Instance.RemoveHotkeyPressedBinding(_tabRightHotkey, IncrementAscension);
		}
	}

	public void SetAscensionLevel(int ascension)
	{
		if (Ascension != ascension)
		{
			Ascension = ascension;
			EmitSignal(SignalName.AscensionLevelChanged);
		}
		RefreshAscensionText();
		RefreshArrowVisibility();
	}

	private void IncrementAscension()
	{
		if (Ascension < _maxAscension)
		{
			SetAscensionLevel(Ascension + 1);
		}
	}

	private void DecrementAscension()
	{
		if (Ascension > 0)
		{
			SetAscensionLevel(Ascension - 1);
		}
	}

	private void RefreshArrowVisibility()
	{
		_leftArrow.Visible = _arrowsVisible && Ascension != 0;
		_rightArrow.Visible = _arrowsVisible && Ascension != _maxAscension;
	}

	public void SetMaxAscension(int maxAscension)
	{
		Log.Info($"Max ascension changed to {maxAscension}");
		_maxAscension = maxAscension;
		if (Ascension >= _maxAscension)
		{
			SetAscensionLevel(_maxAscension);
		}
		base.Visible = _maxAscension > 0;
		RefreshArrowVisibility();
	}

	private void RefreshAscensionText()
	{
		_ascensionLevel.SetTextAutoSize(Ascension.ToString());
		string formattedText = AscensionHelper.GetTitle(Ascension).GetFormattedText();
		string formattedText2 = AscensionHelper.GetDescription(Ascension).GetFormattedText();
		_info.Text = "[b][gold]" + formattedText + "[/gold][/b]\n" + formattedText2;
	}

	public void AnimIn()
	{
		if (base.Visible)
		{
			Color modulate = base.Modulate;
			modulate.A = 0f;
			base.Modulate = modulate;
			_tween?.FastForwardToCompletion();
			_tween = CreateTween().SetParallel();
			_tween.TweenProperty(this, "modulate:a", 1f, 0.2);
			_tween.TweenProperty(this, "position:y", base.Position.Y, 0.3).From(base.Position.Y + 30f).SetEase(Tween.EaseType.Out)
				.SetTrans(Tween.TransitionType.Back);
		}
	}

	private void UpdateControllerButton()
	{
		MultiplayerUiMode mode = _mode;
		if ((uint)(mode - 1) <= 1u)
		{
			_leftTriggerIcon.Visible = NControllerManager.Instance.IsUsingController;
			_rightTriggerIcon.Visible = NControllerManager.Instance.IsUsingController;
			_leftTriggerIcon.Texture = NInputManager.Instance.GetHotkeyIcon(MegaInput.viewDeckAndTabLeft);
			_rightTriggerIcon.Texture = NInputManager.Instance.GetHotkeyIcon(MegaInput.viewExhaustPileAndTabRight);
		}
		else
		{
			_leftTriggerIcon.Visible = false;
			_rightTriggerIcon.Visible = false;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(13);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.Initialize, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, "mode", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.SetFireBlue, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.SetFireRed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.Cleanup, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.SetAscensionLevel, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, "ascension", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.IncrementAscension, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.DecrementAscension, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.RefreshArrowVisibility, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.SetMaxAscension, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, "maxAscension", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.RefreshAscensionText, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.AnimIn, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.UpdateControllerButton, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName._Ready && args.Count == 0)
		{
			_Ready();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.Initialize && args.Count == 1)
		{
			Initialize(VariantUtils.ConvertTo<MultiplayerUiMode>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetFireBlue && args.Count == 0)
		{
			SetFireBlue();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetFireRed && args.Count == 0)
		{
			SetFireRed();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.Cleanup && args.Count == 0)
		{
			Cleanup();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetAscensionLevel && args.Count == 1)
		{
			SetAscensionLevel(VariantUtils.ConvertTo<int>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.IncrementAscension && args.Count == 0)
		{
			IncrementAscension();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.DecrementAscension && args.Count == 0)
		{
			DecrementAscension();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.RefreshArrowVisibility && args.Count == 0)
		{
			RefreshArrowVisibility();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetMaxAscension && args.Count == 1)
		{
			SetMaxAscension(VariantUtils.ConvertTo<int>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.RefreshAscensionText && args.Count == 0)
		{
			RefreshAscensionText();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.AnimIn && args.Count == 0)
		{
			AnimIn();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateControllerButton && args.Count == 0)
		{
			UpdateControllerButton();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName._Ready)
		{
			return true;
		}
		if (method == MethodName.Initialize)
		{
			return true;
		}
		if (method == MethodName.SetFireBlue)
		{
			return true;
		}
		if (method == MethodName.SetFireRed)
		{
			return true;
		}
		if (method == MethodName.Cleanup)
		{
			return true;
		}
		if (method == MethodName.SetAscensionLevel)
		{
			return true;
		}
		if (method == MethodName.IncrementAscension)
		{
			return true;
		}
		if (method == MethodName.DecrementAscension)
		{
			return true;
		}
		if (method == MethodName.RefreshArrowVisibility)
		{
			return true;
		}
		if (method == MethodName.SetMaxAscension)
		{
			return true;
		}
		if (method == MethodName.RefreshAscensionText)
		{
			return true;
		}
		if (method == MethodName.AnimIn)
		{
			return true;
		}
		if (method == MethodName.UpdateControllerButton)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.Ascension)
		{
			Ascension = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName._maxAscension)
		{
			_maxAscension = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName._leftArrow)
		{
			_leftArrow = VariantUtils.ConvertTo<NButton>(in value);
			return true;
		}
		if (name == PropertyName._rightArrow)
		{
			_rightArrow = VariantUtils.ConvertTo<NButton>(in value);
			return true;
		}
		if (name == PropertyName._ascensionLevel)
		{
			_ascensionLevel = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._info)
		{
			_info = VariantUtils.ConvertTo<MegaRichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName._leftTriggerIcon)
		{
			_leftTriggerIcon = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._rightTriggerIcon)
		{
			_rightTriggerIcon = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._iconHsv)
		{
			_iconHsv = VariantUtils.ConvertTo<ShaderMaterial>(in value);
			return true;
		}
		if (name == PropertyName._arrowsVisible)
		{
			_arrowsVisible = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._mode)
		{
			_mode = VariantUtils.ConvertTo<MultiplayerUiMode>(in value);
			return true;
		}
		if (name == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.Ascension)
		{
			value = VariantUtils.CreateFrom<int>(Ascension);
			return true;
		}
		if (name == PropertyName._maxAscension)
		{
			value = VariantUtils.CreateFrom(in _maxAscension);
			return true;
		}
		if (name == PropertyName._leftArrow)
		{
			value = VariantUtils.CreateFrom(in _leftArrow);
			return true;
		}
		if (name == PropertyName._rightArrow)
		{
			value = VariantUtils.CreateFrom(in _rightArrow);
			return true;
		}
		if (name == PropertyName._ascensionLevel)
		{
			value = VariantUtils.CreateFrom(in _ascensionLevel);
			return true;
		}
		if (name == PropertyName._info)
		{
			value = VariantUtils.CreateFrom(in _info);
			return true;
		}
		if (name == PropertyName._leftTriggerIcon)
		{
			value = VariantUtils.CreateFrom(in _leftTriggerIcon);
			return true;
		}
		if (name == PropertyName._rightTriggerIcon)
		{
			value = VariantUtils.CreateFrom(in _rightTriggerIcon);
			return true;
		}
		if (name == PropertyName._iconHsv)
		{
			value = VariantUtils.CreateFrom(in _iconHsv);
			return true;
		}
		if (name == PropertyName._arrowsVisible)
		{
			value = VariantUtils.CreateFrom(in _arrowsVisible);
			return true;
		}
		if (name == PropertyName._mode)
		{
			value = VariantUtils.CreateFrom(in _mode);
			return true;
		}
		if (name == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom(in _tween);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName.Ascension, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName._maxAscension, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._leftArrow, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._rightArrow, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._ascensionLevel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._info, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._leftTriggerIcon, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._rightTriggerIcon, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._iconHsv, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName._arrowsVisible, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName._mode, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._tween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.Ascension, Variant.From<int>(Ascension));
		info.AddProperty(PropertyName._maxAscension, Variant.From(in _maxAscension));
		info.AddProperty(PropertyName._leftArrow, Variant.From(in _leftArrow));
		info.AddProperty(PropertyName._rightArrow, Variant.From(in _rightArrow));
		info.AddProperty(PropertyName._ascensionLevel, Variant.From(in _ascensionLevel));
		info.AddProperty(PropertyName._info, Variant.From(in _info));
		info.AddProperty(PropertyName._leftTriggerIcon, Variant.From(in _leftTriggerIcon));
		info.AddProperty(PropertyName._rightTriggerIcon, Variant.From(in _rightTriggerIcon));
		info.AddProperty(PropertyName._iconHsv, Variant.From(in _iconHsv));
		info.AddProperty(PropertyName._arrowsVisible, Variant.From(in _arrowsVisible));
		info.AddProperty(PropertyName._mode, Variant.From(in _mode));
		info.AddProperty(PropertyName._tween, Variant.From(in _tween));
		info.AddSignalEventDelegate(SignalName.AscensionLevelChanged, backing_AscensionLevelChanged);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.Ascension, out var value))
		{
			Ascension = value.As<int>();
		}
		if (info.TryGetProperty(PropertyName._maxAscension, out var value2))
		{
			_maxAscension = value2.As<int>();
		}
		if (info.TryGetProperty(PropertyName._leftArrow, out var value3))
		{
			_leftArrow = value3.As<NButton>();
		}
		if (info.TryGetProperty(PropertyName._rightArrow, out var value4))
		{
			_rightArrow = value4.As<NButton>();
		}
		if (info.TryGetProperty(PropertyName._ascensionLevel, out var value5))
		{
			_ascensionLevel = value5.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._info, out var value6))
		{
			_info = value6.As<MegaRichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName._leftTriggerIcon, out var value7))
		{
			_leftTriggerIcon = value7.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName._rightTriggerIcon, out var value8))
		{
			_rightTriggerIcon = value8.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName._iconHsv, out var value9))
		{
			_iconHsv = value9.As<ShaderMaterial>();
		}
		if (info.TryGetProperty(PropertyName._arrowsVisible, out var value10))
		{
			_arrowsVisible = value10.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._mode, out var value11))
		{
			_mode = value11.As<MultiplayerUiMode>();
		}
		if (info.TryGetProperty(PropertyName._tween, out var value12))
		{
			_tween = value12.As<Tween>();
		}
		if (info.TryGetSignalEventDelegate<AscensionLevelChangedEventHandler>(SignalName.AscensionLevelChanged, out var value13))
		{
			backing_AscensionLevelChanged = value13;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.AscensionLevelChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		return list;
	}

	protected void EmitSignalAscensionLevelChanged()
	{
		EmitSignal(SignalName.AscensionLevelChanged);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		if (signal == SignalName.AscensionLevelChanged && args.Count == 0)
		{
			backing_AscensionLevelChanged?.Invoke();
		}
		else
		{
			base.RaiseGodotClassSignalCallbacks(in signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if (signal == SignalName.AscensionLevelChanged)
		{
			return true;
		}
		return base.HasGodotClassSignal(in signal);
	}
}

// sts2, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
// MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect.NCharacterSelectButton
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Unlocks;

[ScriptPath("res://src/Core/Nodes/Screens/CharacterSelect/NCharacterSelectButton.cs")]
public class NCharacterSelectButton : NButton
{
	private enum State
	{
		NotSelected,
		SelectedLocally,
		SelectedRemotely
	}

	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public new static readonly StringName OnFocus = "OnFocus";

		public new static readonly StringName OnPress = "OnPress";

		public new static readonly StringName OnUnfocus = "OnUnfocus";

		public new static readonly StringName _Process = "_Process";

		public static readonly StringName LockForAnimation = "LockForAnimation";

		public static readonly StringName Reset = "Reset";

		public static readonly StringName OnRemotePlayerSelected = "OnRemotePlayerSelected";

		public static readonly StringName OnRemotePlayerDeselected = "OnRemotePlayerDeselected";

		public static readonly StringName Select = "Select";

		public static readonly StringName Deselect = "Deselect";

		public static readonly StringName RefreshState = "RefreshState";

		public static readonly StringName GetSaturationForCurrentState = "GetSaturationForCurrentState";

		public static readonly StringName GetValueForCurrentState = "GetValueForCurrentState";

		public static readonly StringName AnimateSaturationToCurrentState = "AnimateSaturationToCurrentState";

		public static readonly StringName RefreshOutline = "RefreshOutline";

		public static readonly StringName RefreshPlayerIcons = "RefreshPlayerIcons";

		public static readonly StringName DebugUnlock = "DebugUnlock";

		public static readonly StringName UnlockIfPossible = "UnlockIfPossible";

		public static readonly StringName UpdateShaderH = "UpdateShaderH";

		public static readonly StringName UpdateShaderS = "UpdateShaderS";

		public static readonly StringName UpdateShaderV = "UpdateShaderV";
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName IsRandom = "IsRandom";

		public static readonly StringName IsLocked = "IsLocked";

		public static readonly StringName IsSelected = "IsSelected";

		public static readonly StringName _icon = "_icon";

		public static readonly StringName _iconAdd = "_iconAdd";

		public static readonly StringName _lock = "_lock";

		public static readonly StringName _outlineLocal = "_outlineLocal";

		public static readonly StringName _outlineRemote = "_outlineRemote";

		public static readonly StringName _outlineMixed = "_outlineMixed";

		public static readonly StringName _shadow = "_shadow";

		public static readonly StringName _playerIconContainer = "_playerIconContainer";

		public static readonly StringName _hsv = "_hsv";

		public static readonly StringName _isLocked = "_isLocked";

		public static readonly StringName _currentOutline = "_currentOutline";

		public static readonly StringName _isSelected = "_isSelected";

		public static readonly StringName _state = "_state";

		public static readonly StringName _hoverTween = "_hoverTween";

		public static readonly StringName _hsvTween = "_hsvTween";
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private static readonly StringName _v = new StringName("v");

	private static readonly StringName _s = new StringName("s");

	private static readonly StringName _h = new StringName("h");

	private static readonly string _playerIconScenePath = SceneHelper.GetScenePath("screens/char_select/char_select_player_icon");

	private static readonly string _unlockedIconPath = ImageHelper.GetImagePath("packed/character_select/char_select_lock3_unlocked.png");

	private TextureRect _icon;

	private TextureRect _iconAdd;

	private TextureRect _lock;

	private Control _outlineLocal;

	private Control _outlineRemote;

	private Control _outlineMixed;

	private Control _shadow;

	private Control _playerIconContainer;

	private CharacterModel _character;

	private ShaderMaterial _hsv;

	private bool _isLocked;

	private static readonly Vector2 _hoverTipOffset = new Vector2(-90f, -180f);

	private ICharacterSelectButtonDelegate? _delegate;

	private Control? _currentOutline;

	private bool _isSelected;

	private readonly HashSet<ulong> _remoteSelectedPlayers = new HashSet<ulong>();

	private State _state;

	private static readonly Vector2 _hoverScale = Vector2.One * 1.1f;

	private Tween? _hoverTween;

	private Tween? _hsvTween;

	private const float _unhoverDuration = 0.5f;

	private const float _glowSpeed = 1.6f;

	private const float _selectedSaturation = 1f;

	private const float _selectedValue = 1.1f;

	private const float _remotelySelectedSaturation = 0.8f;

	private const float _remotelySelectedValue = 0.4f;

	private const float _notSelectedSaturation = 0.2f;

	private const float _notSelectedValue = 0.4f;

	public static IEnumerable<string> AssetPaths => new global::_003C_003Ez__ReadOnlyArray<string>(new string[2] { _playerIconScenePath, _unlockedIconPath });

	public bool IsRandom { get; private set; }

	public IReadOnlyCollection<ulong> RemoteSelectedPlayers => _remoteSelectedPlayers;

	public CharacterModel Character => _character;

	public bool IsLocked => _isLocked;

	public bool IsSelected => _isSelected;

	public override void _Ready()
	{
		ConnectSignals();
		_icon = GetNode<TextureRect>("%Icon");
		_iconAdd = GetNode<TextureRect>("%IconAdd");
		_lock = GetNode<TextureRect>("%Lock");
		_outlineLocal = GetNode<Control>("%OutlineLocal");
		_outlineRemote = GetNode<Control>("%OutlineRemote");
		_outlineMixed = GetNode<Control>("%OutlineMixed");
		_shadow = GetNode<Control>("%Shadow");
		_playerIconContainer = GetNode<Control>("%PlayerIconContainer");
		_hsv = (ShaderMaterial)_icon.Material;
		_hsv.SetShaderParameter(_s, 0.2f);
		_hsv.SetShaderParameter(_v, 0.4f);
		Connect(Control.SignalName.FocusEntered, Callable.From(Select));
	}

	public void Init(CharacterModel character, ICharacterSelectButtonDelegate del)
	{
		_delegate = del;
		_character = character;
		UnlockState unlockState = SaveManager.Instance.GenerateUnlockStateFromProgress();
		if (character is RandomCharacter)
		{
			IsRandom = true;
			_isLocked = ModelDb.AllCharacters.Any((CharacterModel c) => !unlockState.Characters.Contains(c));
		}
		else
		{
			_isLocked = !unlockState.Characters.Contains(_character);
		}
		if (_isLocked)
		{
			_icon.Texture = character.CharacterSelectLockedIcon;
			_lock.Visible = true;
		}
		else
		{
			_icon.Texture = character.CharacterSelectIcon;
		}
	}

	protected override void OnFocus()
	{
		if (!_isSelected)
		{
			_hoverTween?.Kill();
			base.Scale = _hoverScale;
			_hsv.SetShaderParameter(_s, 1f);
			_hsv.SetShaderParameter(_v, 1.1f);
			if (_isLocked)
			{
				HoverTip hoverTip = new HoverTip(new LocString("main_menu_ui", "CHARACTER_SELECT.locked.title"), _character.GetUnlockText());
				NHoverTipSet.CreateAndShow(this, hoverTip)?.SetGlobalPosition(base.GlobalPosition + _hoverTipOffset);
			}
			SfxCmd.Play("event:/sfx/ui/clicks/ui_hover");
		}
	}

	protected override void OnPress()
	{
	}

	protected override void OnUnfocus()
	{
		_hoverTween?.Kill();
		_hoverTween = CreateTween().SetParallel();
		_hoverTween.TweenProperty(this, "scale", Vector2.One, 0.5).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
		NHoverTipSet.Remove(this);
		AnimateSaturationToCurrentState(_hoverTween);
	}

	public override void _Process(double delta)
	{
		if (_currentOutline != null)
		{
			if (_isSelected)
			{
				float a = Mathf.Lerp(0.35f, 1f, (Mathf.Cos((float)Time.GetTicksMsec() * 0.001f * 1.6f * (float)Math.PI) + 1f) * 0.5f);
				Control? currentOutline = _currentOutline;
				Color modulate = _currentOutline.Modulate;
				modulate.A = a;
				currentOutline.Modulate = modulate;
			}
			else
			{
				Control? currentOutline2 = _currentOutline;
				Color modulate = _currentOutline.Modulate;
				modulate.A = 0.5f;
				currentOutline2.Modulate = modulate;
			}
		}
	}

	public void LockForAnimation()
	{
		_icon.Texture = _character.CharacterSelectLockedIcon;
		_lock.Visible = true;
		base.ZIndex = 1;
		_lock.Modulate = Colors.White;
		Disable();
	}

	public async Task AnimateUnlock()
	{
		GpuParticles2D chargeParticles = GetNode<GpuParticles2D>("%UnlockChargeParticles");
		chargeParticles.Emitting = true;
		float num = 1f;
		Vector2 originalLockPosition = _lock.Position;
		float num2 = 0f;
		NDebugAudioManager.Instance.Play("character_unlock_charge.mp3");
		while (num2 < 1f)
		{
			Vector2 vector = Vector2.Right.Rotated(Rng.Chaotic.NextFloat((float)Math.PI * 2f)) * num;
			_lock.Position = originalLockPosition + vector;
			float num3 = num2;
			num2 = num3 + await this.AwaitProcessFrame();
			num = Mathf.Lerp(1f, 5f, Ease.QuadOut(num2));
		}
		NDebugAudioManager.Instance.Play("character_unlock.mp3");
		_lock.Position = originalLockPosition;
		_lock.Texture = PreloadManager.Cache.GetTexture2D(_unlockedIconPath);
		_icon.Texture = _character.CharacterSelectIcon;
		_iconAdd.Texture = _icon.Texture;
		_iconAdd.Visible = true;
		GpuParticles2D node = GetNode<GpuParticles2D>("%UnlockParticles");
		node.Emitting = true;
		chargeParticles.Emitting = false;
		Tween tween = CreateTween();
		tween.SetParallel();
		tween.TweenProperty(_iconAdd, "scale", Vector2.One * 1.5f, 1.0);
		tween.TweenProperty(_iconAdd, "modulate:a", 0f, 1.0).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Expo);
		tween.TweenProperty(_lock, "modulate:a", 0f, 0.5).SetDelay(0.5);
		base.ZIndex = 0;
		Enable();
	}

	public void Reset()
	{
		foreach (Node child in _playerIconContainer.GetChildren())
		{
			child.QueueFreeSafely();
		}
		_remoteSelectedPlayers.Clear();
		Deselect();
	}

	public void OnRemotePlayerSelected(ulong playerId)
	{
		_remoteSelectedPlayers.Add(playerId);
		RefreshState();
	}

	public void OnRemotePlayerDeselected(ulong playerId)
	{
		_remoteSelectedPlayers.Remove(playerId);
		RefreshState();
	}

	public void Select()
	{
		if (!_isSelected)
		{
			_hoverTween?.Kill();
			_isSelected = true;
			_delegate.SelectCharacter(this, _character);
			RefreshState();
		}
	}

	public void Deselect()
	{
		_isSelected = false;
		RefreshState();
	}

	private void RefreshState()
	{
		State state = (_isSelected ? State.SelectedLocally : ((_remoteSelectedPlayers.Count > 0) ? State.SelectedRemotely : State.NotSelected));
		State state2 = _state;
		if (state2 != state)
		{
			_state = state;
			if (state2 == State.NotSelected)
			{
				_hsv.SetShaderParameter(_s, GetSaturationForCurrentState());
				_hsv.SetShaderParameter(_v, GetValueForCurrentState());
			}
			else
			{
				_hoverTween?.Kill();
				_hoverTween = CreateTween().SetParallel();
				AnimateSaturationToCurrentState(_hoverTween);
			}
		}
		RefreshOutline();
		RefreshPlayerIcons();
	}

	private float GetSaturationForCurrentState()
	{
		return _state switch
		{
			State.SelectedLocally => 1f, 
			State.SelectedRemotely => 0.8f, 
			State.NotSelected => 0.2f, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private float GetValueForCurrentState()
	{
		return _state switch
		{
			State.SelectedLocally => 1.1f, 
			State.SelectedRemotely => 0.4f, 
			State.NotSelected => 0.8f, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private void AnimateSaturationToCurrentState(Tween tween)
	{
		tween.TweenMethod(Callable.From<float>(UpdateShaderS), _hsv.GetShaderParameter(_s), GetSaturationForCurrentState(), 0.5).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
		tween.TweenMethod(Callable.From<float>(UpdateShaderV), _hsv.GetShaderParameter(_v), GetValueForCurrentState(), 0.5).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
	}

	private void RefreshOutline()
	{
		if (_currentOutline != null)
		{
			_currentOutline.Visible = false;
		}
		if (_isSelected && _remoteSelectedPlayers.Count > 0)
		{
			_currentOutline = _outlineMixed;
		}
		else if (_isSelected)
		{
			_currentOutline = _outlineLocal;
		}
		else if (_remoteSelectedPlayers.Count > 0)
		{
			_currentOutline = _outlineRemote;
		}
		else
		{
			_currentOutline = null;
		}
		if (_currentOutline != null)
		{
			_currentOutline.Visible = true;
		}
	}

	private void RefreshPlayerIcons()
	{
		if (_delegate != null && _delegate.Lobby.NetService.Type != NetGameType.Singleplayer)
		{
			int num = _remoteSelectedPlayers.Count + (_isSelected ? 1 : 0);
			for (int i = _playerIconContainer.GetChildCount(); i < num; i++)
			{
				TextureRect child = PreloadManager.Cache.GetScene(_playerIconScenePath).Instantiate<TextureRect>(PackedScene.GenEditState.Disabled);
				_playerIconContainer.AddChildSafely(child);
			}
			while (_playerIconContainer.GetChildCount() > num)
			{
				Control child2 = _playerIconContainer.GetChild<Control>(0);
				_playerIconContainer.RemoveChildSafely(child2);
				child2.QueueFreeSafely();
			}
			for (int j = 0; j < _playerIconContainer.GetChildCount(); j++)
			{
				Control child3 = _playerIconContainer.GetChild<Control>(j);
				child3.Modulate = ((_isSelected && j == 0) ? StsColors.gold : StsColors.blue);
			}
		}
	}

	public void DebugUnlock()
	{
		_icon.Texture = _character.CharacterSelectIcon;
		_isLocked = false;
		_lock.Visible = false;
		Enable();
	}

	public void UnlockIfPossible()
	{
		UnlockState unlockState = SaveManager.Instance.GenerateUnlockStateFromProgress();
		if (unlockState.Characters.Contains(_character))
		{
			_icon.Texture = _character.CharacterSelectIcon;
			_isLocked = false;
			_lock.Visible = false;
			Enable();
		}
	}

	private void UpdateShaderH(float value)
	{
		_hsv.SetShaderParameter(_h, value);
	}

	private void UpdateShaderS(float value)
	{
		_hsv.SetShaderParameter(_s, value);
	}

	private void UpdateShaderV(float value)
	{
		_hsv.SetShaderParameter(_v, value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(22);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.LockForAnimation, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.Reset, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnRemotePlayerSelected, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, "playerId", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.OnRemotePlayerDeselected, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, "playerId", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.Select, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.Deselect, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.RefreshState, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.GetSaturationForCurrentState, new PropertyInfo(Variant.Type.Float, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.GetValueForCurrentState, new PropertyInfo(Variant.Type.Float, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.AnimateSaturationToCurrentState, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "tween", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Tween"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.RefreshOutline, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.RefreshPlayerIcons, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.DebugUnlock, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.UnlockIfPossible, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.UpdateShaderH, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Float, "value", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.UpdateShaderS, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Float, "value", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.UpdateShaderV, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Float, "value", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName._Ready && args.Count == 0)
		{
			_Ready();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnFocus && args.Count == 0)
		{
			OnFocus();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnPress && args.Count == 0)
		{
			OnPress();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnUnfocus && args.Count == 0)
		{
			OnUnfocus();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName._Process && args.Count == 1)
		{
			_Process(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.LockForAnimation && args.Count == 0)
		{
			LockForAnimation();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.Reset && args.Count == 0)
		{
			Reset();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnRemotePlayerSelected && args.Count == 1)
		{
			OnRemotePlayerSelected(VariantUtils.ConvertTo<ulong>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnRemotePlayerDeselected && args.Count == 1)
		{
			OnRemotePlayerDeselected(VariantUtils.ConvertTo<ulong>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.Select && args.Count == 0)
		{
			Select();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.Deselect && args.Count == 0)
		{
			Deselect();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.RefreshState && args.Count == 0)
		{
			RefreshState();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.GetSaturationForCurrentState && args.Count == 0)
		{
			ret = VariantUtils.CreateFrom<float>(GetSaturationForCurrentState());
			return true;
		}
		if (method == MethodName.GetValueForCurrentState && args.Count == 0)
		{
			ret = VariantUtils.CreateFrom<float>(GetValueForCurrentState());
			return true;
		}
		if (method == MethodName.AnimateSaturationToCurrentState && args.Count == 1)
		{
			AnimateSaturationToCurrentState(VariantUtils.ConvertTo<Tween>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.RefreshOutline && args.Count == 0)
		{
			RefreshOutline();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.RefreshPlayerIcons && args.Count == 0)
		{
			RefreshPlayerIcons();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.DebugUnlock && args.Count == 0)
		{
			DebugUnlock();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UnlockIfPossible && args.Count == 0)
		{
			UnlockIfPossible();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateShaderH && args.Count == 1)
		{
			UpdateShaderH(VariantUtils.ConvertTo<float>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateShaderS && args.Count == 1)
		{
			UpdateShaderS(VariantUtils.ConvertTo<float>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateShaderV && args.Count == 1)
		{
			UpdateShaderV(VariantUtils.ConvertTo<float>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName._Ready)
		{
			return true;
		}
		if (method == MethodName.OnFocus)
		{
			return true;
		}
		if (method == MethodName.OnPress)
		{
			return true;
		}
		if (method == MethodName.OnUnfocus)
		{
			return true;
		}
		if (method == MethodName._Process)
		{
			return true;
		}
		if (method == MethodName.LockForAnimation)
		{
			return true;
		}
		if (method == MethodName.Reset)
		{
			return true;
		}
		if (method == MethodName.OnRemotePlayerSelected)
		{
			return true;
		}
		if (method == MethodName.OnRemotePlayerDeselected)
		{
			return true;
		}
		if (method == MethodName.Select)
		{
			return true;
		}
		if (method == MethodName.Deselect)
		{
			return true;
		}
		if (method == MethodName.RefreshState)
		{
			return true;
		}
		if (method == MethodName.GetSaturationForCurrentState)
		{
			return true;
		}
		if (method == MethodName.GetValueForCurrentState)
		{
			return true;
		}
		if (method == MethodName.AnimateSaturationToCurrentState)
		{
			return true;
		}
		if (method == MethodName.RefreshOutline)
		{
			return true;
		}
		if (method == MethodName.RefreshPlayerIcons)
		{
			return true;
		}
		if (method == MethodName.DebugUnlock)
		{
			return true;
		}
		if (method == MethodName.UnlockIfPossible)
		{
			return true;
		}
		if (method == MethodName.UpdateShaderH)
		{
			return true;
		}
		if (method == MethodName.UpdateShaderS)
		{
			return true;
		}
		if (method == MethodName.UpdateShaderV)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.IsRandom)
		{
			IsRandom = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._icon)
		{
			_icon = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._iconAdd)
		{
			_iconAdd = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._lock)
		{
			_lock = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._outlineLocal)
		{
			_outlineLocal = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._outlineRemote)
		{
			_outlineRemote = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._outlineMixed)
		{
			_outlineMixed = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._shadow)
		{
			_shadow = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._playerIconContainer)
		{
			_playerIconContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._hsv)
		{
			_hsv = VariantUtils.ConvertTo<ShaderMaterial>(in value);
			return true;
		}
		if (name == PropertyName._isLocked)
		{
			_isLocked = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._currentOutline)
		{
			_currentOutline = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._isSelected)
		{
			_isSelected = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._state)
		{
			_state = VariantUtils.ConvertTo<State>(in value);
			return true;
		}
		if (name == PropertyName._hoverTween)
		{
			_hoverTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		if (name == PropertyName._hsvTween)
		{
			_hsvTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		bool from;
		if (name == PropertyName.IsRandom)
		{
			from = IsRandom;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName.IsLocked)
		{
			from = IsLocked;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName.IsSelected)
		{
			from = IsSelected;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName._icon)
		{
			value = VariantUtils.CreateFrom(in _icon);
			return true;
		}
		if (name == PropertyName._iconAdd)
		{
			value = VariantUtils.CreateFrom(in _iconAdd);
			return true;
		}
		if (name == PropertyName._lock)
		{
			value = VariantUtils.CreateFrom(in _lock);
			return true;
		}
		if (name == PropertyName._outlineLocal)
		{
			value = VariantUtils.CreateFrom(in _outlineLocal);
			return true;
		}
		if (name == PropertyName._outlineRemote)
		{
			value = VariantUtils.CreateFrom(in _outlineRemote);
			return true;
		}
		if (name == PropertyName._outlineMixed)
		{
			value = VariantUtils.CreateFrom(in _outlineMixed);
			return true;
		}
		if (name == PropertyName._shadow)
		{
			value = VariantUtils.CreateFrom(in _shadow);
			return true;
		}
		if (name == PropertyName._playerIconContainer)
		{
			value = VariantUtils.CreateFrom(in _playerIconContainer);
			return true;
		}
		if (name == PropertyName._hsv)
		{
			value = VariantUtils.CreateFrom(in _hsv);
			return true;
		}
		if (name == PropertyName._isLocked)
		{
			value = VariantUtils.CreateFrom(in _isLocked);
			return true;
		}
		if (name == PropertyName._currentOutline)
		{
			value = VariantUtils.CreateFrom(in _currentOutline);
			return true;
		}
		if (name == PropertyName._isSelected)
		{
			value = VariantUtils.CreateFrom(in _isSelected);
			return true;
		}
		if (name == PropertyName._state)
		{
			value = VariantUtils.CreateFrom(in _state);
			return true;
		}
		if (name == PropertyName._hoverTween)
		{
			value = VariantUtils.CreateFrom(in _hoverTween);
			return true;
		}
		if (name == PropertyName._hsvTween)
		{
			value = VariantUtils.CreateFrom(in _hsvTween);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._icon, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._iconAdd, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._lock, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._outlineLocal, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._outlineRemote, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._outlineMixed, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._shadow, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._playerIconContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._hsv, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName._isLocked, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName.IsRandom, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._currentOutline, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName._isSelected, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName._state, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._hoverTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._hsvTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName.IsLocked, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName.IsSelected, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.IsRandom, Variant.From<bool>(IsRandom));
		info.AddProperty(PropertyName._icon, Variant.From(in _icon));
		info.AddProperty(PropertyName._iconAdd, Variant.From(in _iconAdd));
		info.AddProperty(PropertyName._lock, Variant.From(in _lock));
		info.AddProperty(PropertyName._outlineLocal, Variant.From(in _outlineLocal));
		info.AddProperty(PropertyName._outlineRemote, Variant.From(in _outlineRemote));
		info.AddProperty(PropertyName._outlineMixed, Variant.From(in _outlineMixed));
		info.AddProperty(PropertyName._shadow, Variant.From(in _shadow));
		info.AddProperty(PropertyName._playerIconContainer, Variant.From(in _playerIconContainer));
		info.AddProperty(PropertyName._hsv, Variant.From(in _hsv));
		info.AddProperty(PropertyName._isLocked, Variant.From(in _isLocked));
		info.AddProperty(PropertyName._currentOutline, Variant.From(in _currentOutline));
		info.AddProperty(PropertyName._isSelected, Variant.From(in _isSelected));
		info.AddProperty(PropertyName._state, Variant.From(in _state));
		info.AddProperty(PropertyName._hoverTween, Variant.From(in _hoverTween));
		info.AddProperty(PropertyName._hsvTween, Variant.From(in _hsvTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.IsRandom, out var value))
		{
			IsRandom = value.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._icon, out var value2))
		{
			_icon = value2.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName._iconAdd, out var value3))
		{
			_iconAdd = value3.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName._lock, out var value4))
		{
			_lock = value4.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName._outlineLocal, out var value5))
		{
			_outlineLocal = value5.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._outlineRemote, out var value6))
		{
			_outlineRemote = value6.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._outlineMixed, out var value7))
		{
			_outlineMixed = value7.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._shadow, out var value8))
		{
			_shadow = value8.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._playerIconContainer, out var value9))
		{
			_playerIconContainer = value9.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._hsv, out var value10))
		{
			_hsv = value10.As<ShaderMaterial>();
		}
		if (info.TryGetProperty(PropertyName._isLocked, out var value11))
		{
			_isLocked = value11.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._currentOutline, out var value12))
		{
			_currentOutline = value12.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._isSelected, out var value13))
		{
			_isSelected = value13.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._state, out var value14))
		{
			_state = value14.As<State>();
		}
		if (info.TryGetProperty(PropertyName._hoverTween, out var value15))
		{
			_hoverTween = value15.As<Tween>();
		}
		if (info.TryGetProperty(PropertyName._hsvTween, out var value16))
		{
			_hsvTween = value16.As<Tween>();
		}
	}
}

// sts2, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
// MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect.NCharacterSelectScreen
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Debug;
using MegaCrit.Sts2.Core.Nodes.Ftue;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Unlocks;

[ScriptPath("res://src/Core/Nodes/Screens/CharacterSelect/NCharacterSelectScreen.cs")]
public class NCharacterSelectScreen : NSubmenu, IStartRunLobbyListener, ICharacterSelectButtonDelegate
{
	public new class MethodName : NSubmenu.MethodName
	{
		public static readonly StringName Create = "Create";

		public new static readonly StringName _Ready = "_Ready";

		public static readonly StringName CheckForMultiplayerAscensionPopup = "CheckForMultiplayerAscensionPopup";

		public static readonly StringName InitializeSingleplayer = "InitializeSingleplayer";

		public static readonly StringName InitCharacterButtons = "InitCharacterButtons";

		public static readonly StringName UpdateRandomCharacterVisibility = "UpdateRandomCharacterVisibility";

		public new static readonly StringName _Input = "_Input";

		public static readonly StringName DebugUnlockAllCharacters = "DebugUnlockAllCharacters";

		public new static readonly StringName OnSubmenuOpened = "OnSubmenuOpened";

		public new static readonly StringName OnSubmenuClosed = "OnSubmenuClosed";

		public static readonly StringName OnEmbarkPressed = "OnEmbarkPressed";

		public new static readonly StringName _Process = "_Process";

		public static readonly StringName CleanUpLobby = "CleanUpLobby";

		public static readonly StringName OnAscensionPanelLevelChanged = "OnAscensionPanelLevelChanged";

		public static readonly StringName OnUnreadyPressed = "OnUnreadyPressed";

		public static readonly StringName UpdateRichPresence = "UpdateRichPresence";

		public static readonly StringName MaxAscensionChanged = "MaxAscensionChanged";

		public static readonly StringName AscensionChanged = "AscensionChanged";

		public static readonly StringName SeedChanged = "SeedChanged";

		public static readonly StringName ModifiersChanged = "ModifiersChanged";

		public static readonly StringName AfterInitialized = "AfterInitialized";
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public new static readonly StringName InitialFocusedControl = "InitialFocusedControl";

		public static readonly StringName ShouldShowActDropdown = "ShouldShowActDropdown";

		public static readonly StringName _name = "_name";

		public static readonly StringName _infoPanel = "_infoPanel";

		public static readonly StringName _description = "_description";

		public static readonly StringName _hp = "_hp";

		public static readonly StringName _gold = "_gold";

		public static readonly StringName _relicTitle = "_relicTitle";

		public static readonly StringName _relicDescription = "_relicDescription";

		public static readonly StringName _relicIcon = "_relicIcon";

		public static readonly StringName _relicIconOutline = "_relicIconOutline";

		public static readonly StringName _selectedButton = "_selectedButton";

		public static readonly StringName _charButtonContainer = "_charButtonContainer";

		public static readonly StringName _bgContainer = "_bgContainer";

		public static readonly StringName _readyAndWaitingContainer = "_readyAndWaitingContainer";

		public new static readonly StringName _backButton = "_backButton";

		public static readonly StringName _unreadyButton = "_unreadyButton";

		public static readonly StringName _embarkButton = "_embarkButton";

		public static readonly StringName _ascensionPanel = "_ascensionPanel";

		public static readonly StringName _actDropdown = "_actDropdown";

		public static readonly StringName _actDropdownLabel = "_actDropdownLabel";

		public static readonly StringName _remotePlayerContainer = "_remotePlayerContainer";

		public static readonly StringName _characterUnlockAnimationBackstop = "_characterUnlockAnimationBackstop";

		public static readonly StringName _randomCharacterButton = "_randomCharacterButton";

		public static readonly StringName _infoPanelTween = "_infoPanelTween";

		public static readonly StringName _infoPanelPosFinalVal = "_infoPanelPosFinalVal";

		public static readonly StringName _delayEmbarkForCharacterSelect = "_delayEmbarkForCharacterSelect";

		public static readonly StringName _charSelectButtonScene = "_charSelectButtonScene";
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/character_select_screen");

	private MegaLabel _name;

	private Control _infoPanel;

	private MegaRichTextLabel _description;

	private MegaLabel _hp;

	private MegaLabel _gold;

	private MegaRichTextLabel _relicTitle;

	private MegaRichTextLabel _relicDescription;

	private TextureRect _relicIcon;

	private TextureRect _relicIconOutline;

	private NCharacterSelectButton? _selectedButton;

	private Control _charButtonContainer;

	private Control _bgContainer;

	private Control _readyAndWaitingContainer;

	private NBackButton _backButton;

	private NBackButton _unreadyButton;

	private NConfirmButton _embarkButton;

	private NAscensionPanel _ascensionPanel;

	private NActDropdown _actDropdown;

	private MegaRichTextLabel _actDropdownLabel;

	private NRemoteLobbyPlayerContainer _remotePlayerContainer;

	private Control _characterUnlockAnimationBackstop;

	private NCharacterSelectButton _randomCharacterButton;

	private Tween? _infoPanelTween;

	private Vector2 _infoPanelPosFinalVal;

	private const string _sceneCharSelectButtonPath = "res://scenes/screens/char_select/char_select_button.tscn";

	private bool _delayEmbarkForCharacterSelect;

	[Export(PropertyHint.None, "")]
	private PackedScene _charSelectButtonScene;

	private IBootstrapSettings? _settings;

	private StartRunLobby _lobby;

	public StartRunLobby Lobby => _lobby;

	public static IEnumerable<string> AssetPaths
	{
		get
		{
			List<string> list = new List<string>();
			list.Add(_scenePath);
			list.Add("res://scenes/screens/char_select/char_select_button.tscn");
			list.AddRange(NCharacterSelectButton.AssetPaths);
			return new _003C_003Ez__ReadOnlyList<string>(list);
		}
	}

	protected override Control InitialFocusedControl => _charButtonContainer.GetChild<Control>(0);

	private bool ShouldShowActDropdown => false;

	public static NCharacterSelectScreen? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return ResourceLoader.Load<PackedScene>(_scenePath, null, ResourceLoader.CacheMode.Reuse).Instantiate<NCharacterSelectScreen>(PackedScene.GenEditState.Disabled);
	}

	public override void _Ready()
	{
		ConnectSignals();
		_infoPanel = GetNode<Control>("InfoPanel");
		_name = GetNode<MegaLabel>("InfoPanel/VBoxContainer/Name");
		_description = GetNode<MegaRichTextLabel>("InfoPanel/VBoxContainer/DescriptionLabel");
		_hp = GetNode<MegaLabel>("InfoPanel/VBoxContainer/HpGoldSpacer/HpGold/Hp/Label");
		_gold = GetNode<MegaLabel>("InfoPanel/VBoxContainer/HpGoldSpacer/HpGold/Gold/Label");
		_relicTitle = GetNode<MegaRichTextLabel>("InfoPanel/VBoxContainer/Relic/Name/RichTextLabel");
		_relicDescription = GetNode<MegaRichTextLabel>("InfoPanel/VBoxContainer/Relic/Description");
		_relicIcon = GetNode<TextureRect>("InfoPanel/VBoxContainer/Relic/Icon");
		_relicIconOutline = GetNode<TextureRect>("InfoPanel/VBoxContainer/Relic/Icon/Outline");
		_bgContainer = GetNode<Control>("AnimatedBg");
		_charButtonContainer = GetNode<Control>("CharSelectButtons/ButtonContainer");
		_ascensionPanel = GetNode<NAscensionPanel>("%AscensionPanel");
		_actDropdown = GetNode<NActDropdown>("%ActDropdown");
		_actDropdownLabel = GetNode<MegaRichTextLabel>("ActLabel");
		_remotePlayerContainer = GetNode<NRemoteLobbyPlayerContainer>("RemotePlayerContainer");
		_readyAndWaitingContainer = GetNode<Control>("ReadyAndWaitingPanel");
		GetNode<MegaRichTextLabel>("%WaitingForPlayers").Text = new LocString("main_menu_ui", "CHARACTER_SELECT.waitingForPlayers").GetFormattedText();
		_characterUnlockAnimationBackstop = GetNode<Control>("%CharacterUnlockAnimationBackstop");
		_backButton = GetNode<NBackButton>("BackButton");
		_unreadyButton = GetNode<NBackButton>("UnreadyButton");
		_embarkButton = GetNode<NConfirmButton>("ConfirmButton");
		_embarkButton.OverrideHotkeys(new string[1] { MegaInput.select });
		_embarkButton.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(OnEmbarkPressed));
		_ascensionPanel.Connect(NAscensionPanel.SignalName.AscensionLevelChanged, Callable.From(OnAscensionPanelLevelChanged));
		_unreadyButton.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(OnUnreadyPressed));
		_unreadyButton.Disable();
		base.ProcessMode = ProcessModeEnum.Disabled;
		InitCharacterButtons();
		Type type = BootstrapSettingsUtil.Get();
		if (type != null)
		{
			_settings = (IBootstrapSettings)Activator.CreateInstance(type);
			PreloadManager.Enabled = _settings.DoPreloading;
		}
	}

	public void InitializeMultiplayerAsHost(INetGameService gameService, int maxPlayers)
	{
		if (gameService.Type != NetGameType.Host)
		{
			throw new InvalidOperationException($"Initialized character select screen with GameService of type {gameService.Type} when hosting!");
		}
		_lobby = new StartRunLobby(GameMode.Standard, gameService, this, maxPlayers);
		_ascensionPanel.Initialize(MultiplayerUiMode.Host);
		_lobby.AddLocalHostPlayer(new UnlockState(SaveManager.Instance.Progress), SaveManager.Instance.Progress.MaxMultiplayerAscension);
		OnAscensionPanelLevelChanged();
		CheckForMultiplayerAscensionPopup();
		AfterInitialized();
	}

	public void InitializeMultiplayerAsClient(INetGameService gameService, ClientLobbyJoinResponseMessage message)
	{
		if (gameService.Type != NetGameType.Client)
		{
			throw new InvalidOperationException($"Initialized character select screen with GameService of type {gameService.Type} when joining!");
		}
		_lobby = new StartRunLobby(GameMode.Standard, gameService, this, -1);
		_ascensionPanel.Initialize(MultiplayerUiMode.Client);
		_lobby.InitializeFromMessage(message);
		CheckForMultiplayerAscensionPopup();
		AfterInitialized();
	}

	private void CheckForMultiplayerAscensionPopup()
	{
		if (SaveManager.Instance.Progress.MaxMultiplayerAscension > 0 && !SaveManager.Instance.SeenPopup("ascension_multiplayer_ftue"))
		{
			NAscensionMultiplayerFtue nAscensionMultiplayerFtue = NAscensionMultiplayerFtue.Create();
			if (nAscensionMultiplayerFtue != null)
			{
				NModalContainer.Instance.Add(nAscensionMultiplayerFtue);
			}
		}
	}

	public void InitializeSingleplayer()
	{
		_lobby = new StartRunLobby(GameMode.Standard, new NetSingleplayerGameService(), this, 1);
		_ascensionPanel.Initialize(MultiplayerUiMode.Singleplayer);
		_lobby.AddLocalHostPlayer(new UnlockState(SaveManager.Instance.Progress), 0);
		AfterInitialized();
	}

	private void InitCharacterButtons()
	{
		foreach (CharacterModel allCharacter in ModelDb.AllCharacters)
		{
			NCharacterSelectButton nCharacterSelectButton = _charSelectButtonScene.Instantiate<NCharacterSelectButton>(PackedScene.GenEditState.Disabled);
			nCharacterSelectButton.Name = allCharacter.Id.Entry + "_button";
			_charButtonContainer.AddChildSafely(nCharacterSelectButton);
			nCharacterSelectButton.Init(allCharacter, this);
		}
		_randomCharacterButton = _charSelectButtonScene.Instantiate<NCharacterSelectButton>(PackedScene.GenEditState.Disabled);
		_charButtonContainer.AddChildSafely(_randomCharacterButton);
		_randomCharacterButton.Init(ModelDb.Character<RandomCharacter>(), this);
		UpdateRandomCharacterVisibility();
		List<NCharacterSelectButton> list = (from c in _charButtonContainer.GetChildren().OfType<NCharacterSelectButton>()
			where c.Visible
			select c).ToList();
		for (int num = 0; num < list.Count; num++)
		{
			list[num].FocusNeighborTop = list[num].GetPath();
			list[num].FocusNeighborBottom = list[num].GetPath();
			NCharacterSelectButton nCharacterSelectButton2 = list[num];
			NodePath path;
			if (num <= 0)
			{
				path = list[list.Count - 1].GetPath();
			}
			else
			{
				path = list[num - 1].GetPath();
			}
			nCharacterSelectButton2.FocusNeighborLeft = path;
			list[num].FocusNeighborRight = ((num < list.Count - 1) ? list[num + 1].GetPath() : list[0].GetPath());
		}
	}

	private void UpdateRandomCharacterVisibility()
	{
		if (_lobby == null)
		{
			return;
		}
		bool visible = false;
		foreach (LobbyPlayer player in _lobby.Players)
		{
			UnlockState unlockState = UnlockState.FromSerializable(player.unlockState);
			bool flag = true;
			foreach (CharacterModel allCharacter in ModelDb.AllCharacters)
			{
				if (!unlockState.Characters.Contains(allCharacter))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				visible = true;
				break;
			}
		}
		_randomCharacterButton.Visible = visible;
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionReleased(DebugHotkey.unlockCharacters))
		{
			DebugUnlockAllCharacters();
		}
	}

	private void DebugUnlockAllCharacters()
	{
		foreach (NCharacterSelectButton item in _charButtonContainer.GetChildren().OfType<NCharacterSelectButton>())
		{
			item.DebugUnlock();
		}
	}

	public override void OnSubmenuOpened()
	{
		base.OnSubmenuOpened();
		foreach (NCharacterSelectButton item in _charButtonContainer.GetChildren().OfType<NCharacterSelectButton>())
		{
			if (!item.IsLocked)
			{
				item.Enable();
			}
			else
			{
				item.UnlockIfPossible();
			}
			item.Reset();
		}
		_embarkButton.Enable();
		if (SaveManager.Instance.Progress.PendingCharacterUnlock == ModelId.none)
		{
			_charButtonContainer.GetChild<NCharacterSelectButton>(0).Select();
		}
		else
		{
			TaskHelper.RunSafely(PlayUnlockCharacterAnimation(SaveManager.Instance.Progress.PendingCharacterUnlock));
		}
		_remotePlayerContainer.Visible = _lobby.NetService.Type != NetGameType.Singleplayer;
		_remotePlayerContainer.Initialize(_lobby, displayLocalPlayer: true);
		if (_lobby.NetService.Type == NetGameType.Client)
		{
			_ascensionPanel.SetAscensionLevel(_lobby.Ascension);
		}
		_actDropdown.Visible = ShouldShowActDropdown;
		_actDropdownLabel.Visible = _actDropdown.Visible;
		_readyAndWaitingContainer.Visible = false;
		foreach (LobbyPlayer player in _lobby.Players)
		{
			RefreshButtonSelectionForPlayer(player);
		}
		base.ProcessMode = ProcessModeEnum.Inherit;
	}

	private async Task PlayUnlockCharacterAnimation(ModelId character)
	{
		await this.AwaitProcessFrame();
		_backButton.Disable();
		_embarkButton.Disable();
		_infoPanel.Visible = false;
		_characterUnlockAnimationBackstop.Visible = true;
		foreach (NCharacterSelectButton button in _charButtonContainer.GetChildren().OfType<NCharacterSelectButton>())
		{
			if (button.Character.Id == character)
			{
				button.LockForAnimation();
				await Cmd.Wait(0.3f);
				await button.AnimateUnlock();
				button.Select();
			}
		}
		_infoPanel.Visible = true;
		_characterUnlockAnimationBackstop.Visible = false;
		_backButton.Enable();
		_embarkButton.Enable();
		SaveManager.Instance.Progress.PendingCharacterUnlock = ModelId.none;
	}

	public override void OnSubmenuClosed()
	{
		base.OnSubmenuClosed();
		_embarkButton.Disable();
		_remotePlayerContainer.Cleanup();
		_ascensionPanel.Cleanup();
		if (_lobby.NetService.Type.IsMultiplayer())
		{
			PlatformUtil.SetRichPresence("MAIN_MENU", null, null);
		}
		CleanUpLobby(disconnectSession: true);
	}

	private void OnEmbarkPressed(NButton _)
	{
		_embarkButton.Disable();
		if (!SaveManager.Instance.SeenFtue("accept_tutorials_ftue"))
		{
			NModalContainer.Instance.Add(NAcceptTutorialsFtue.Create(this, delegate
			{
				OnEmbarkPressed(null);
			}));
			return;
		}
		NetGameType type = _lobby.NetService.Type;
		if ((uint)(type - 1) <= 1u)
		{
			_lobby.Act1 = _actDropdown.CurrentOption;
		}
		_lobby.SetReady(ready: true);
		foreach (NCharacterSelectButton item in _charButtonContainer.GetChildren().OfType<NCharacterSelectButton>())
		{
			item.Disable();
		}
		_backButton.Disable();
		if (_lobby.NetService.Type.IsMultiplayer() && !_lobby.IsAboutToBeginGame())
		{
			_readyAndWaitingContainer.Visible = true;
			_unreadyButton.Enable();
		}
	}

	public override void _Process(double delta)
	{
		if (_lobby.NetService.IsConnected)
		{
			_lobby.NetService.Update();
		}
	}

	private void CleanUpLobby(bool disconnectSession)
	{
		_lobby.CleanUp(disconnectSession);
		_lobby = null;
		if (GodotObject.IsInstanceValid(this))
		{
			base.ProcessMode = ProcessModeEnum.Disabled;
		}
	}

	private void OnLocalCharacterChangedForRandom(CharacterModel characterModel)
	{
		NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short, 90f);
		SfxCmd.Play(characterModel.CharacterSelectSfx);
		Control control = PreloadManager.Cache.GetScene(characterModel.CharacterSelectBg).Instantiate<Control>(PackedScene.GenEditState.Disabled);
		control.Name = characterModel.Id.Entry + "_bg";
		_bgContainer.AddChildSafely(control);
		_delayEmbarkForCharacterSelect = true;
	}

	private async Task StartNewSingleplayerRun(string seed, List<ActModel> acts)
	{
		Log.Info($"Embarking on a singleplayer {_lobby.LocalPlayer.character.Id.Entry} run. Ascension: {_lobby.Ascension} Seed: {seed}");
		int ascensionToEmbark = _lobby.Ascension;
		if (_delayEmbarkForCharacterSelect)
		{
			await Cmd.Wait(1f);
			_delayEmbarkForCharacterSelect = false;
		}
		SfxCmd.Play(_lobby.LocalPlayer.character.CharacterTransitionSfx);
		await NGame.Instance.Transition.FadeOut(0.8f, _lobby.LocalPlayer.character.CharacterSelectTransitionPath);
		await NGame.Instance.StartNewSingleplayerRun(_lobby.LocalPlayer.character, shouldSave: true, acts, Array.Empty<ModifierModel>(), seed, GameMode.Standard, ascensionToEmbark);
		CleanUpLobby(disconnectSession: false);
	}

	private async Task StartNewMultiplayerRun(string seed, List<ActModel> acts)
	{
		Log.Info($"Embarking on a multiplayer run. Players: {string.Join(",", _lobby.Players)}. Ascension: {_lobby.Ascension} Seed: {seed}");
		if (_delayEmbarkForCharacterSelect)
		{
			await Cmd.Wait(1f);
			_delayEmbarkForCharacterSelect = false;
		}
		SfxCmd.Play(_lobby.LocalPlayer.character.CharacterTransitionSfx);
		await NGame.Instance.Transition.FadeOut(0.8f, _lobby.LocalPlayer.character.CharacterSelectTransitionPath);
		IBootstrapSettings settings = _settings;
		if (settings != null && settings.BootstrapInMultiplayer)
		{
			using (new NetLoadingHandle(_lobby.NetService))
			{
				acts[0] = _settings.Act;
				RunState runState = RunState.CreateForNewRun(_lobby.Players.Select((LobbyPlayer p) => Player.CreateForNewRun(p.character, UnlockState.FromSerializable(p.unlockState), p.id)).ToList(), acts.Select((ActModel a) => a.ToMutable()).ToList(), _settings.Modifiers, GameMode.Standard, _lobby.Ascension, seed);
				RunManager.Instance.SetUpNewMultiPlayer(runState, _lobby, _settings.SaveRunHistory);
				await PreloadManager.LoadRunAssets(runState.Players.Select((Player p) => p.Character));
				await RunManager.Instance.FinalizeStartingRelics();
				RunManager.Instance.Launch();
				NGame.Instance.RootSceneContainer.SetCurrentScene(NRun.Create(runState));
				await RunManager.Instance.SetActInternal(0);
				await SaveManager.Instance.SaveRun(null);
				CleanUpLobby(disconnectSession: false);
				await _settings.Setup(LocalContext.GetMe(runState));
				switch (_settings.RoomType)
				{
				case RoomType.Unassigned:
					await RunManager.Instance.EnterAct(0);
					break;
				case RoomType.Treasure:
				case RoomType.Shop:
				case RoomType.RestSite:
					await RunManager.Instance.EnterRoomDebug(_settings.RoomType, MapPointType.Unassigned, null, showTransition: false);
					RunManager.Instance.ActionExecutor.Unpause();
					break;
				case RoomType.Event:
					await RunManager.Instance.EnterRoomDebug(_settings.RoomType, MapPointType.Unassigned, _settings.Event, showTransition: false);
					break;
				default:
					await RunManager.Instance.EnterRoomDebug(_settings.RoomType, MapPointType.Unassigned, _settings.RoomType.IsCombatRoom() ? _settings.Encounter.ToMutable() : null, showTransition: false);
					break;
				}
			}
		}
		else
		{
			await NGame.Instance.StartNewMultiplayerRun(_lobby, shouldSave: true, acts, Array.Empty<ModifierModel>(), seed, _lobby.Ascension);
			CleanUpLobby(disconnectSession: false);
		}
	}

	public void SelectCharacter(NCharacterSelectButton charSelectButton, CharacterModel characterModel)
	{
		if (!charSelectButton.IsRandom)
		{
			SfxCmd.Play(characterModel.CharacterSelectSfx);
		}
		NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short, 90f);
		if (_infoPanelTween != null)
		{
			_infoPanel.Position = _infoPanelPosFinalVal;
		}
		_infoPanelPosFinalVal = _infoPanel.Position;
		_infoPanelTween?.Kill();
		_infoPanelTween = CreateTween().SetParallel();
		_infoPanelTween.TweenProperty(_infoPanel, "position", _infoPanel.Position, 0.5).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo)
			.From(_infoPanel.Position - new Vector2(300f, 0f));
		foreach (Node child in _bgContainer.GetChildren())
		{
			_bgContainer.RemoveChildSafely(child);
			child.QueueFreeSafely();
		}
		_selectedButton = charSelectButton;
		if (!charSelectButton.IsLocked)
		{
			_embarkButton.Enable();
			Control control = PreloadManager.Cache.GetScene(characterModel.CharacterSelectBg).Instantiate<Control>(PackedScene.GenEditState.Disabled);
			control.Name = characterModel.Id.Entry + "_bg";
			_bgContainer.AddChildSafely(control);
			string formattedText = new LocString("characters", characterModel.CharacterSelectTitle).GetFormattedText();
			_name.SetTextAutoSize(formattedText);
			_description.Text = new LocString("characters", characterModel.CharacterSelectDesc).GetFormattedText();
			if (!_selectedButton.IsRandom)
			{
				_hp.SetTextAutoSize($"{characterModel.StartingHp}/{characterModel.StartingHp}");
				_gold.SetTextAutoSize($"{characterModel.StartingGold}");
				RelicModel relicModel = characterModel.StartingRelics[0];
				_relicTitle.Text = relicModel.Title.GetFormattedText();
				_relicDescription.Text = relicModel.DynamicDescription.GetFormattedText();
				_relicIcon.Texture = relicModel.Icon;
				_relicIconOutline.Texture = relicModel.IconOutline;
				_relicIcon.SelfModulate = Colors.White;
				_relicIconOutline.SelfModulate = StsColors.halfTransparentBlack;
			}
			else
			{
				_hp.SetTextAutoSize("??/??");
				_gold.SetTextAutoSize("???");
				_relicIcon.SelfModulate = StsColors.transparentBlack;
				_relicIconOutline.SelfModulate = StsColors.transparentBlack;
				_relicTitle.Text = string.Empty;
				_relicDescription.Text = string.Empty;
			}
			_lobby.SetLocalCharacter(characterModel);
			if (!_lobby.NetService.Type.IsMultiplayer())
			{
				_ascensionPanel.AnimIn();
			}
		}
		else
		{
			_embarkButton.Disable();
			string formattedText2 = new LocString("main_menu_ui", "CHARACTER_SELECT.locked.title").GetFormattedText();
			_name.SetTextAutoSize(formattedText2);
			_description.Text = characterModel.GetUnlockText().GetFormattedText();
			_hp.SetTextAutoSize("??/??");
			_gold.SetTextAutoSize("???");
			if (!_selectedButton.IsRandom)
			{
				RelicModel relicModel2 = characterModel.StartingRelics[0];
				_relicTitle.Text = new LocString("main_menu_ui", "CHARACTER_SELECT.lockedRelic.title").GetFormattedText();
				_relicDescription.Text = new LocString("main_menu_ui", "CHARACTER_SELECT.lockedRelic.description").GetFormattedText();
				_relicIcon.Texture = relicModel2.Icon;
				_relicIconOutline.Texture = relicModel2.IconOutline;
				_relicIcon.SelfModulate = StsColors.ninetyPercentBlack;
				_relicIconOutline.SelfModulate = StsColors.halfTransparentWhite;
			}
			else
			{
				_relicIcon.SelfModulate = StsColors.transparentBlack;
				_relicIconOutline.SelfModulate = StsColors.transparentBlack;
				_relicTitle.Text = string.Empty;
				_relicDescription.Text = string.Empty;
			}
			_ascensionPanel.Visible = false;
		}
		foreach (NCharacterSelectButton item in _charButtonContainer.GetChildren().OfType<NCharacterSelectButton>())
		{
			if (item != _selectedButton)
			{
				item.Deselect();
			}
		}
	}

	private void OnAscensionPanelLevelChanged()
	{
		if (_lobby.NetService.Type != NetGameType.Client && _lobby.Ascension != _ascensionPanel.Ascension)
		{
			_lobby.SyncAscensionChange(_ascensionPanel.Ascension);
		}
	}

	private void OnUnreadyPressed(NButton _)
	{
		_lobby.SetReady(ready: false);
		foreach (NCharacterSelectButton item in _charButtonContainer.GetChildren().OfType<NCharacterSelectButton>())
		{
			item.Enable();
		}
		_selectedButton?.TryGrabFocus();
		_readyAndWaitingContainer.Visible = false;
		_embarkButton.Enable();
		_backButton.Enable();
		_unreadyButton.Disable();
	}

	private void UpdateRichPresence()
	{
		if (_lobby.NetService.Type.IsMultiplayer())
		{
			PlatformUtil.SetRichPresence("STANDARD_MP_LOBBY", _lobby.NetService.GetRawLobbyIdentifier(), _lobby.Players.Count);
		}
	}

	public void MaxAscensionChanged()
	{
		_ascensionPanel.SetMaxAscension(_lobby.MaxAscension);
	}

	public void PlayerConnected(LobbyPlayer player)
	{
		_remotePlayerContainer.OnPlayerConnected(player);
		RefreshButtonSelectionForPlayer(player);
		UpdateRichPresence();
		UpdateRandomCharacterVisibility();
	}

	public void PlayerChanged(LobbyPlayer player, bool isRandomCharacterResolution)
	{
		if (player.id == _lobby.LocalPlayer.id && isRandomCharacterResolution)
		{
			OnLocalCharacterChangedForRandom(player.character);
		}
		_remotePlayerContainer.OnPlayerChanged(player);
		RefreshButtonSelectionForPlayer(player);
	}

	private void RefreshButtonSelectionForPlayer(LobbyPlayer player)
	{
		if (player.id == _lobby.LocalPlayer.id)
		{
			return;
		}
		foreach (NCharacterSelectButton item in _charButtonContainer.GetChildren().OfType<NCharacterSelectButton>())
		{
			if (item.RemoteSelectedPlayers.Contains(player.id) && player.character != item.Character)
			{
				item.OnRemotePlayerDeselected(player.id);
			}
			else if (player.character == item.Character)
			{
				item.OnRemotePlayerSelected(player.id);
			}
		}
	}

	public void AscensionChanged()
	{
		if (_lobby.NetService.Type == NetGameType.Client)
		{
			_ascensionPanel.Visible = _lobby.Ascension > 0;
		}
		_ascensionPanel.SetAscensionLevel(_lobby.Ascension);
	}

	public void SeedChanged()
	{
		throw new NotImplementedException("Seed should not be changed in standard mode!");
	}

	public void ModifiersChanged()
	{
		throw new NotImplementedException("Modifiers should not be changed in standard mode!");
	}

	public void RemotePlayerDisconnected(LobbyPlayer player)
	{
		_remotePlayerContainer.OnPlayerDisconnected(player);
		foreach (NCharacterSelectButton item in _charButtonContainer.GetChildren().OfType<NCharacterSelectButton>())
		{
			if (item.RemoteSelectedPlayers.Contains(player.id) && player.character == item.Character)
			{
				item.OnRemotePlayerDeselected(player.id);
			}
		}
		UpdateRichPresence();
		UpdateRandomCharacterVisibility();
	}

	public void BeginRun(string seed, List<ActModel> acts, IReadOnlyList<ModifierModel> modifiers)
	{
		if (modifiers.Count > 0)
		{
			Log.Error("Modifiers list is not empty while starting a standard run, ignoring!");
		}
		NAudioManager.Instance?.StopMusic();
		_ascensionPanel.Cleanup();
		_embarkButton.Disable();
		_unreadyButton.Disable();
		if (_lobby.NetService.Type == NetGameType.Singleplayer)
		{
			TaskHelper.RunSafely(StartNewSingleplayerRun(seed, acts));
		}
		else
		{
			TaskHelper.RunSafely(StartNewMultiplayerRun(seed, acts));
		}
	}

	public void LocalPlayerDisconnected(NetErrorInfo info)
	{
		if (info.SelfInitiated && info.GetReason() == NetError.Quit)
		{
			return;
		}
		if (_stack != null && _stack.Peek() == this)
		{
			_stack.Pop();
		}
		if (TestMode.IsOff)
		{
			NErrorPopup nErrorPopup = NErrorPopup.Create(info);
			if (nErrorPopup != null)
			{
				NModalContainer.Instance.Add(nErrorPopup);
			}
		}
	}

	private void AfterInitialized()
	{
		NGame.Instance.RemoteCursorContainer.Initialize(_lobby.InputSynchronizer, _lobby.Players.Select((LobbyPlayer p) => p.id));
		NGame.Instance.ReactionContainer.InitializeNetworking(_lobby.NetService);
		NGame.Instance.TimeoutOverlay.Initialize(_lobby.NetService, isGameLevel: true);
		UpdateRichPresence();
		UpdateRandomCharacterVisibility();
		MegaCrit.Sts2.Core.Logging.Logger.logLevelTypeMap[LogType.Network] = ((_lobby.NetService.Type == NetGameType.Singleplayer) ? LogLevel.Info : LogLevel.Debug);
		MegaCrit.Sts2.Core.Logging.Logger.logLevelTypeMap[LogType.Actions] = ((_lobby.NetService.Type == NetGameType.Singleplayer) ? LogLevel.Info : LogLevel.VeryDebug);
		MegaCrit.Sts2.Core.Logging.Logger.logLevelTypeMap[LogType.GameSync] = ((_lobby.NetService.Type == NetGameType.Singleplayer) ? LogLevel.Info : LogLevel.VeryDebug);
		if (_lobby.NetService.Type != NetGameType.Singleplayer)
		{
			IBootstrapSettings? settings = _settings;
			if (settings != null && settings.BootstrapInMultiplayer)
			{
				NGame.Instance.DebugSeedOverride = _settings.Seed;
				return;
			}
		}
		NGame.Instance.DebugSeedOverride = null;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(21);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false), MethodFlags.Normal | MethodFlags.Static, null, null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.CheckForMultiplayerAscensionPopup, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.InitializeSingleplayer, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.InitCharacterButtons, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.UpdateRandomCharacterVisibility, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "inputEvent", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("InputEvent"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.DebugUnlockAllCharacters, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnSubmenuClosed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnEmbarkPressed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.CleanUpLobby, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Bool, "disconnectSession", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.OnAscensionPanelLevelChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnUnreadyPressed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.UpdateRichPresence, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.MaxAscensionChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.AscensionChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.SeedChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.ModifiersChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.AfterInitialized, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.Create && args.Count == 0)
		{
			ret = VariantUtils.CreateFrom<NCharacterSelectScreen>(Create());
			return true;
		}
		if (method == MethodName._Ready && args.Count == 0)
		{
			_Ready();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CheckForMultiplayerAscensionPopup && args.Count == 0)
		{
			CheckForMultiplayerAscensionPopup();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.InitializeSingleplayer && args.Count == 0)
		{
			InitializeSingleplayer();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.InitCharacterButtons && args.Count == 0)
		{
			InitCharacterButtons();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateRandomCharacterVisibility && args.Count == 0)
		{
			UpdateRandomCharacterVisibility();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName._Input && args.Count == 1)
		{
			_Input(VariantUtils.ConvertTo<InputEvent>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.DebugUnlockAllCharacters && args.Count == 0)
		{
			DebugUnlockAllCharacters();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnSubmenuOpened && args.Count == 0)
		{
			OnSubmenuOpened();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnSubmenuClosed && args.Count == 0)
		{
			OnSubmenuClosed();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnEmbarkPressed && args.Count == 1)
		{
			OnEmbarkPressed(VariantUtils.ConvertTo<NButton>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName._Process && args.Count == 1)
		{
			_Process(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CleanUpLobby && args.Count == 1)
		{
			CleanUpLobby(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnAscensionPanelLevelChanged && args.Count == 0)
		{
			OnAscensionPanelLevelChanged();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnUnreadyPressed && args.Count == 1)
		{
			OnUnreadyPressed(VariantUtils.ConvertTo<NButton>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateRichPresence && args.Count == 0)
		{
			UpdateRichPresence();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.MaxAscensionChanged && args.Count == 0)
		{
			MaxAscensionChanged();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.AscensionChanged && args.Count == 0)
		{
			AscensionChanged();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SeedChanged && args.Count == 0)
		{
			SeedChanged();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ModifiersChanged && args.Count == 0)
		{
			ModifiersChanged();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.AfterInitialized && args.Count == 0)
		{
			AfterInitialized();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.Create && args.Count == 0)
		{
			ret = VariantUtils.CreateFrom<NCharacterSelectScreen>(Create());
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.Create)
		{
			return true;
		}
		if (method == MethodName._Ready)
		{
			return true;
		}
		if (method == MethodName.CheckForMultiplayerAscensionPopup)
		{
			return true;
		}
		if (method == MethodName.InitializeSingleplayer)
		{
			return true;
		}
		if (method == MethodName.InitCharacterButtons)
		{
			return true;
		}
		if (method == MethodName.UpdateRandomCharacterVisibility)
		{
			return true;
		}
		if (method == MethodName._Input)
		{
			return true;
		}
		if (method == MethodName.DebugUnlockAllCharacters)
		{
			return true;
		}
		if (method == MethodName.OnSubmenuOpened)
		{
			return true;
		}
		if (method == MethodName.OnSubmenuClosed)
		{
			return true;
		}
		if (method == MethodName.OnEmbarkPressed)
		{
			return true;
		}
		if (method == MethodName._Process)
		{
			return true;
		}
		if (method == MethodName.CleanUpLobby)
		{
			return true;
		}
		if (method == MethodName.OnAscensionPanelLevelChanged)
		{
			return true;
		}
		if (method == MethodName.OnUnreadyPressed)
		{
			return true;
		}
		if (method == MethodName.UpdateRichPresence)
		{
			return true;
		}
		if (method == MethodName.MaxAscensionChanged)
		{
			return true;
		}
		if (method == MethodName.AscensionChanged)
		{
			return true;
		}
		if (method == MethodName.SeedChanged)
		{
			return true;
		}
		if (method == MethodName.ModifiersChanged)
		{
			return true;
		}
		if (method == MethodName.AfterInitialized)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._name)
		{
			_name = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._infoPanel)
		{
			_infoPanel = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._description)
		{
			_description = VariantUtils.ConvertTo<MegaRichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName._hp)
		{
			_hp = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._gold)
		{
			_gold = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._relicTitle)
		{
			_relicTitle = VariantUtils.ConvertTo<MegaRichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName._relicDescription)
		{
			_relicDescription = VariantUtils.ConvertTo<MegaRichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName._relicIcon)
		{
			_relicIcon = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._relicIconOutline)
		{
			_relicIconOutline = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._selectedButton)
		{
			_selectedButton = VariantUtils.ConvertTo<NCharacterSelectButton>(in value);
			return true;
		}
		if (name == PropertyName._charButtonContainer)
		{
			_charButtonContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._bgContainer)
		{
			_bgContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._readyAndWaitingContainer)
		{
			_readyAndWaitingContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._backButton)
		{
			_backButton = VariantUtils.ConvertTo<NBackButton>(in value);
			return true;
		}
		if (name == PropertyName._unreadyButton)
		{
			_unreadyButton = VariantUtils.ConvertTo<NBackButton>(in value);
			return true;
		}
		if (name == PropertyName._embarkButton)
		{
			_embarkButton = VariantUtils.ConvertTo<NConfirmButton>(in value);
			return true;
		}
		if (name == PropertyName._ascensionPanel)
		{
			_ascensionPanel = VariantUtils.ConvertTo<NAscensionPanel>(in value);
			return true;
		}
		if (name == PropertyName._actDropdown)
		{
			_actDropdown = VariantUtils.ConvertTo<NActDropdown>(in value);
			return true;
		}
		if (name == PropertyName._actDropdownLabel)
		{
			_actDropdownLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName._remotePlayerContainer)
		{
			_remotePlayerContainer = VariantUtils.ConvertTo<NRemoteLobbyPlayerContainer>(in value);
			return true;
		}
		if (name == PropertyName._characterUnlockAnimationBackstop)
		{
			_characterUnlockAnimationBackstop = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._randomCharacterButton)
		{
			_randomCharacterButton = VariantUtils.ConvertTo<NCharacterSelectButton>(in value);
			return true;
		}
		if (name == PropertyName._infoPanelTween)
		{
			_infoPanelTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		if (name == PropertyName._infoPanelPosFinalVal)
		{
			_infoPanelPosFinalVal = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName._delayEmbarkForCharacterSelect)
		{
			_delayEmbarkForCharacterSelect = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._charSelectButtonScene)
		{
			_charSelectButtonScene = VariantUtils.ConvertTo<PackedScene>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.InitialFocusedControl)
		{
			value = VariantUtils.CreateFrom<Control>(InitialFocusedControl);
			return true;
		}
		if (name == PropertyName.ShouldShowActDropdown)
		{
			value = VariantUtils.CreateFrom<bool>(ShouldShowActDropdown);
			return true;
		}
		if (name == PropertyName._name)
		{
			value = VariantUtils.CreateFrom(in _name);
			return true;
		}
		if (name == PropertyName._infoPanel)
		{
			value = VariantUtils.CreateFrom(in _infoPanel);
			return true;
		}
		if (name == PropertyName._description)
		{
			value = VariantUtils.CreateFrom(in _description);
			return true;
		}
		if (name == PropertyName._hp)
		{
			value = VariantUtils.CreateFrom(in _hp);
			return true;
		}
		if (name == PropertyName._gold)
		{
			value = VariantUtils.CreateFrom(in _gold);
			return true;
		}
		if (name == PropertyName._relicTitle)
		{
			value = VariantUtils.CreateFrom(in _relicTitle);
			return true;
		}
		if (name == PropertyName._relicDescription)
		{
			value = VariantUtils.CreateFrom(in _relicDescription);
			return true;
		}
		if (name == PropertyName._relicIcon)
		{
			value = VariantUtils.CreateFrom(in _relicIcon);
			return true;
		}
		if (name == PropertyName._relicIconOutline)
		{
			value = VariantUtils.CreateFrom(in _relicIconOutline);
			return true;
		}
		if (name == PropertyName._selectedButton)
		{
			value = VariantUtils.CreateFrom(in _selectedButton);
			return true;
		}
		if (name == PropertyName._charButtonContainer)
		{
			value = VariantUtils.CreateFrom(in _charButtonContainer);
			return true;
		}
		if (name == PropertyName._bgContainer)
		{
			value = VariantUtils.CreateFrom(in _bgContainer);
			return true;
		}
		if (name == PropertyName._readyAndWaitingContainer)
		{
			value = VariantUtils.CreateFrom(in _readyAndWaitingContainer);
			return true;
		}
		if (name == PropertyName._backButton)
		{
			value = VariantUtils.CreateFrom(in _backButton);
			return true;
		}
		if (name == PropertyName._unreadyButton)
		{
			value = VariantUtils.CreateFrom(in _unreadyButton);
			return true;
		}
		if (name == PropertyName._embarkButton)
		{
			value = VariantUtils.CreateFrom(in _embarkButton);
			return true;
		}
		if (name == PropertyName._ascensionPanel)
		{
			value = VariantUtils.CreateFrom(in _ascensionPanel);
			return true;
		}
		if (name == PropertyName._actDropdown)
		{
			value = VariantUtils.CreateFrom(in _actDropdown);
			return true;
		}
		if (name == PropertyName._actDropdownLabel)
		{
			value = VariantUtils.CreateFrom(in _actDropdownLabel);
			return true;
		}
		if (name == PropertyName._remotePlayerContainer)
		{
			value = VariantUtils.CreateFrom(in _remotePlayerContainer);
			return true;
		}
		if (name == PropertyName._characterUnlockAnimationBackstop)
		{
			value = VariantUtils.CreateFrom(in _characterUnlockAnimationBackstop);
			return true;
		}
		if (name == PropertyName._randomCharacterButton)
		{
			value = VariantUtils.CreateFrom(in _randomCharacterButton);
			return true;
		}
		if (name == PropertyName._infoPanelTween)
		{
			value = VariantUtils.CreateFrom(in _infoPanelTween);
			return true;
		}
		if (name == PropertyName._infoPanelPosFinalVal)
		{
			value = VariantUtils.CreateFrom(in _infoPanelPosFinalVal);
			return true;
		}
		if (name == PropertyName._delayEmbarkForCharacterSelect)
		{
			value = VariantUtils.CreateFrom(in _delayEmbarkForCharacterSelect);
			return true;
		}
		if (name == PropertyName._charSelectButtonScene)
		{
			value = VariantUtils.CreateFrom(in _charSelectButtonScene);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._name, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._infoPanel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._description, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._hp, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._gold, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._relicTitle, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._relicDescription, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._relicIcon, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._relicIconOutline, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._selectedButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._charButtonContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._bgContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._readyAndWaitingContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._backButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._unreadyButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._embarkButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._ascensionPanel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._actDropdown, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._actDropdownLabel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._remotePlayerContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._characterUnlockAnimationBackstop, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._randomCharacterButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._infoPanelTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Vector2, PropertyName._infoPanelPosFinalVal, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName._delayEmbarkForCharacterSelect, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._charSelectButtonScene, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.InitialFocusedControl, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName.ShouldShowActDropdown, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._name, Variant.From(in _name));
		info.AddProperty(PropertyName._infoPanel, Variant.From(in _infoPanel));
		info.AddProperty(PropertyName._description, Variant.From(in _description));
		info.AddProperty(PropertyName._hp, Variant.From(in _hp));
		info.AddProperty(PropertyName._gold, Variant.From(in _gold));
		info.AddProperty(PropertyName._relicTitle, Variant.From(in _relicTitle));
		info.AddProperty(PropertyName._relicDescription, Variant.From(in _relicDescription));
		info.AddProperty(PropertyName._relicIcon, Variant.From(in _relicIcon));
		info.AddProperty(PropertyName._relicIconOutline, Variant.From(in _relicIconOutline));
		info.AddProperty(PropertyName._selectedButton, Variant.From(in _selectedButton));
		info.AddProperty(PropertyName._charButtonContainer, Variant.From(in _charButtonContainer));
		info.AddProperty(PropertyName._bgContainer, Variant.From(in _bgContainer));
		info.AddProperty(PropertyName._readyAndWaitingContainer, Variant.From(in _readyAndWaitingContainer));
		info.AddProperty(PropertyName._backButton, Variant.From(in _backButton));
		info.AddProperty(PropertyName._unreadyButton, Variant.From(in _unreadyButton));
		info.AddProperty(PropertyName._embarkButton, Variant.From(in _embarkButton));
		info.AddProperty(PropertyName._ascensionPanel, Variant.From(in _ascensionPanel));
		info.AddProperty(PropertyName._actDropdown, Variant.From(in _actDropdown));
		info.AddProperty(PropertyName._actDropdownLabel, Variant.From(in _actDropdownLabel));
		info.AddProperty(PropertyName._remotePlayerContainer, Variant.From(in _remotePlayerContainer));
		info.AddProperty(PropertyName._characterUnlockAnimationBackstop, Variant.From(in _characterUnlockAnimationBackstop));
		info.AddProperty(PropertyName._randomCharacterButton, Variant.From(in _randomCharacterButton));
		info.AddProperty(PropertyName._infoPanelTween, Variant.From(in _infoPanelTween));
		info.AddProperty(PropertyName._infoPanelPosFinalVal, Variant.From(in _infoPanelPosFinalVal));
		info.AddProperty(PropertyName._delayEmbarkForCharacterSelect, Variant.From(in _delayEmbarkForCharacterSelect));
		info.AddProperty(PropertyName._charSelectButtonScene, Variant.From(in _charSelectButtonScene));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._name, out var value))
		{
			_name = value.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._infoPanel, out var value2))
		{
			_infoPanel = value2.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._description, out var value3))
		{
			_description = value3.As<MegaRichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName._hp, out var value4))
		{
			_hp = value4.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._gold, out var value5))
		{
			_gold = value5.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._relicTitle, out var value6))
		{
			_relicTitle = value6.As<MegaRichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName._relicDescription, out var value7))
		{
			_relicDescription = value7.As<MegaRichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName._relicIcon, out var value8))
		{
			_relicIcon = value8.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName._relicIconOutline, out var value9))
		{
			_relicIconOutline = value9.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName._selectedButton, out var value10))
		{
			_selectedButton = value10.As<NCharacterSelectButton>();
		}
		if (info.TryGetProperty(PropertyName._charButtonContainer, out var value11))
		{
			_charButtonContainer = value11.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._bgContainer, out var value12))
		{
			_bgContainer = value12.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._readyAndWaitingContainer, out var value13))
		{
			_readyAndWaitingContainer = value13.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._backButton, out var value14))
		{
			_backButton = value14.As<NBackButton>();
		}
		if (info.TryGetProperty(PropertyName._unreadyButton, out var value15))
		{
			_unreadyButton = value15.As<NBackButton>();
		}
		if (info.TryGetProperty(PropertyName._embarkButton, out var value16))
		{
			_embarkButton = value16.As<NConfirmButton>();
		}
		if (info.TryGetProperty(PropertyName._ascensionPanel, out var value17))
		{
			_ascensionPanel = value17.As<NAscensionPanel>();
		}
		if (info.TryGetProperty(PropertyName._actDropdown, out var value18))
		{
			_actDropdown = value18.As<NActDropdown>();
		}
		if (info.TryGetProperty(PropertyName._actDropdownLabel, out var value19))
		{
			_actDropdownLabel = value19.As<MegaRichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName._remotePlayerContainer, out var value20))
		{
			_remotePlayerContainer = value20.As<NRemoteLobbyPlayerContainer>();
		}
		if (info.TryGetProperty(PropertyName._characterUnlockAnimationBackstop, out var value21))
		{
			_characterUnlockAnimationBackstop = value21.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._randomCharacterButton, out var value22))
		{
			_randomCharacterButton = value22.As<NCharacterSelectButton>();
		}
		if (info.TryGetProperty(PropertyName._infoPanelTween, out var value23))
		{
			_infoPanelTween = value23.As<Tween>();
		}
		if (info.TryGetProperty(PropertyName._infoPanelPosFinalVal, out var value24))
		{
			_infoPanelPosFinalVal = value24.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName._delayEmbarkForCharacterSelect, out var value25))
		{
			_delayEmbarkForCharacterSelect = value25.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._charSelectButtonScene, out var value26))
		{
			_charSelectButtonScene = value26.As<PackedScene>();
		}
	}
}

// sts2, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
// MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect.NCharacterSelectScreenBg
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

[ScriptPath("res://src/Core/Nodes/Screens/CharacterSelect/NCharacterSelectScreenBg.cs")]
public class NCharacterSelectScreenBg : Control
{
	public new class MethodName : Control.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public static readonly StringName OnWindowChange = "OnWindowChange";
	}

	public new class PropertyName : Control.PropertyName
	{
		public static readonly StringName _window = "_window";
	}

	public new class SignalName : Control.SignalName
	{
	}

	private Window _window;

	private const float _sixteenByNine = 1.7777778f;

	private const float _fourByThree = 1.3333334f;

	private static readonly float _defaultBgScale = 1.1f;

	private static readonly float _narrowBgScale = 1.153f;

	public override void _Ready()
	{
		_window = GetTree().Root;
		_window.Connect(Viewport.SignalName.SizeChanged, Callable.From(OnWindowChange));
	}

	private void OnWindowChange()
	{
		float num = Mathf.Max(1.3333334f, (float)_window.Size.X / (float)_window.Size.Y);
		if (num < 1.7777778f)
		{
			float p = (num - 1.3333334f) / 0.44444442f;
			base.Scale = Vector2.One * Mathf.Remap(Ease.CubicOut(p), 0f, 1f, _defaultBgScale * _narrowBgScale, _defaultBgScale);
		}
		else
		{
			base.Scale = Vector2.One * _defaultBgScale;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnWindowChange, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName._Ready && args.Count == 0)
		{
			_Ready();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnWindowChange && args.Count == 0)
		{
			OnWindowChange();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName._Ready)
		{
			return true;
		}
		if (method == MethodName.OnWindowChange)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._window)
		{
			_window = VariantUtils.ConvertTo<Window>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName._window)
		{
			value = VariantUtils.CreateFrom(in _window);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._window, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._window, Variant.From(in _window));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._window, out var value))
		{
			_window = value.As<Window>();
		}
	}
}

// sts2, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
// MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect.NMultiplayerLoadGameScreen
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.TestSupport;

[ScriptPath("res://src/Core/Nodes/Screens/CharacterSelect/NMultiplayerLoadGameScreen.cs")]
public class NMultiplayerLoadGameScreen : NSubmenu, ILoadRunLobbyListener
{
	public new class MethodName : NSubmenu.MethodName
	{
		public static readonly StringName Create = "Create";

		public new static readonly StringName _Ready = "_Ready";

		public new static readonly StringName OnSubmenuOpened = "OnSubmenuOpened";

		public new static readonly StringName OnSubmenuShown = "OnSubmenuShown";

		public new static readonly StringName OnSubmenuClosed = "OnSubmenuClosed";

		public new static readonly StringName OnSubmenuHidden = "OnSubmenuHidden";

		public static readonly StringName OnEmbarkPressed = "OnEmbarkPressed";

		public static readonly StringName OnUnreadyPressed = "OnUnreadyPressed";

		public static readonly StringName UpdateRichPresence = "UpdateRichPresence";

		public new static readonly StringName _Process = "_Process";

		public static readonly StringName CleanUpLobby = "CleanUpLobby";

		public static readonly StringName PlayerConnected = "PlayerConnected";

		public static readonly StringName PlayerReadyChanged = "PlayerReadyChanged";

		public static readonly StringName RemotePlayerDisconnected = "RemotePlayerDisconnected";

		public static readonly StringName BeginRun = "BeginRun";

		public static readonly StringName AfterMultiplayerStarted = "AfterMultiplayerStarted";
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public new static readonly StringName InitialFocusedControl = "InitialFocusedControl";

		public static readonly StringName _name = "_name";

		public static readonly StringName _infoPanel = "_infoPanel";

		public static readonly StringName _hp = "_hp";

		public static readonly StringName _gold = "_gold";

		public static readonly StringName _selectedButton = "_selectedButton";

		public static readonly StringName _bgContainer = "_bgContainer";

		public static readonly StringName _confirmButton = "_confirmButton";

		public new static readonly StringName _backButton = "_backButton";

		public static readonly StringName _unreadyButton = "_unreadyButton";

		public static readonly StringName _ascensionPanel = "_ascensionPanel";

		public static readonly StringName _floorLabel = "_floorLabel";

		public static readonly StringName _actLabel = "_actLabel";

		public static readonly StringName _remotePlayerContainer = "_remotePlayerContainer";

		public static readonly StringName _infoPanelTween = "_infoPanelTween";

		public static readonly StringName _infoPanelPosFinalVal = "_infoPanelPosFinalVal";
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/multiplayer_load_game_screen");

	private MegaLabel _name;

	private Control _infoPanel;

	private MegaLabel _hp;

	private MegaLabel _gold;

	private NCharacterSelectButton? _selectedButton;

	private Control _bgContainer;

	private NConfirmButton _confirmButton;

	private NBackButton _backButton;

	private NBackButton _unreadyButton;

	private NAscensionPanel _ascensionPanel;

	private MegaRichTextLabel _floorLabel;

	private MegaRichTextLabel _actLabel;

	private NRemoteLoadLobbyPlayerContainer _remotePlayerContainer;

	private Tween? _infoPanelTween;

	private Vector2 _infoPanelPosFinalVal;

	private const string _sceneCharSelectButtonPath = "res://scenes/screens/char_select/char_select_button.tscn";

	private LoadRunLobby _runLobby;

	public static IEnumerable<string> AssetPaths => new global::_003C_003Ez__ReadOnlyArray<string>(new string[2] { _scenePath, "res://scenes/screens/char_select/char_select_button.tscn" });

	protected override Control? InitialFocusedControl => null;

	public static NMultiplayerLoadGameScreen? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NMultiplayerLoadGameScreen>(PackedScene.GenEditState.Disabled);
	}

	public override void _Ready()
	{
		ConnectSignals();
		_infoPanel = GetNode<Control>("InfoPanel");
		_name = GetNode<MegaLabel>("InfoPanel/VBoxContainer/Name");
		_hp = GetNode<MegaLabel>("InfoPanel/VBoxContainer/HpGoldSpacer/HpGold/Hp/Label");
		_gold = GetNode<MegaLabel>("InfoPanel/VBoxContainer/HpGoldSpacer/HpGold/Gold/Label");
		_actLabel = GetNode<MegaRichTextLabel>("InfoPanel/VBoxContainer/RunLocation/ActLabel");
		_floorLabel = GetNode<MegaRichTextLabel>("InfoPanel/VBoxContainer/RunLocation/FloorLabel");
		_bgContainer = GetNode<Control>("AnimatedBg");
		_ascensionPanel = GetNode<NAscensionPanel>("%AscensionPanel");
		_remotePlayerContainer = GetNode<NRemoteLoadLobbyPlayerContainer>("RemotePlayerLoadContainer");
		_confirmButton = GetNode<NConfirmButton>("ConfirmButton");
		_backButton = GetNode<NBackButton>("BackButton");
		_unreadyButton = GetNode<NBackButton>("UnreadyButton");
		_confirmButton.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(OnEmbarkPressed));
		_unreadyButton.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(OnUnreadyPressed));
		_unreadyButton.Disable();
		base.ProcessMode = ProcessModeEnum.Disabled;
	}

	public void InitializeAsHost(INetGameService gameService, SerializableRun run)
	{
		if (gameService.Type != NetGameType.Host)
		{
			throw new InvalidOperationException($"Initialized character select screen with GameService of type {gameService.Type} when hosting!");
		}
		_runLobby = new LoadRunLobby(gameService, this, run);
		try
		{
			_runLobby.AddLocalHostPlayer();
			AfterMultiplayerStarted();
		}
		catch
		{
			CleanUpLobby(disconnectSession: true);
			throw;
		}
	}

	public void InitializeAsClient(INetGameService gameService, ClientLoadJoinResponseMessage message)
	{
		if (gameService.Type != NetGameType.Client)
		{
			throw new InvalidOperationException($"Initialized character select screen with GameService of type {gameService.Type} when joining!");
		}
		_runLobby = new LoadRunLobby(gameService, this, message);
		AfterMultiplayerStarted();
	}

	public override void OnSubmenuOpened()
	{
		base.OnSubmenuOpened();
		_confirmButton.Enable();
		_remotePlayerContainer.Initialize(_runLobby, displayLocalPlayer: false);
		_ascensionPanel.Initialize(MultiplayerUiMode.Load);
		_ascensionPanel.SetAscensionLevel(_runLobby.Run.Ascension);
	}

	protected override void OnSubmenuShown()
	{
		base.ProcessMode = ProcessModeEnum.Inherit;
	}

	public override void OnSubmenuClosed()
	{
		base.OnSubmenuClosed();
		_confirmButton.Disable();
		_remotePlayerContainer.Cleanup();
		if (_runLobby.NetService.Type.IsMultiplayer())
		{
			PlatformUtil.SetRichPresence("MAIN_MENU", null, null);
		}
		CleanUpLobby(disconnectSession: true);
	}

	protected override void OnSubmenuHidden()
	{
		base.ProcessMode = ProcessModeEnum.Disabled;
	}

	private void OnEmbarkPressed(NButton _)
	{
		_confirmButton.Disable();
		_backButton.Disable();
		_runLobby.SetReady(ready: true);
		if (!_runLobby.IsAboutToBeginGame())
		{
			_unreadyButton.Enable();
		}
	}

	private void OnUnreadyPressed(NButton _)
	{
		_confirmButton.Enable();
		_backButton.Enable();
		_unreadyButton.Disable();
		_runLobby.SetReady(ready: false);
	}

	private void UpdateRichPresence()
	{
		if (_runLobby.NetService.Type.IsMultiplayer())
		{
			PlatformUtil.SetRichPresence("LOADING_MP_LOBBY", _runLobby.NetService.GetRawLobbyIdentifier(), _runLobby.ConnectedPlayerIds.Count);
		}
	}

	public override void _Process(double delta)
	{
		if (_runLobby.NetService.IsConnected)
		{
			_runLobby.NetService.Update();
		}
	}

	private void CleanUpLobby(bool disconnectSession)
	{
		_runLobby.CleanUp(disconnectSession);
		_runLobby = null;
	}

	public async Task<bool> ShouldAllowRunToBegin()
	{
		if (_runLobby.ConnectedPlayerIds.Count >= _runLobby.Run.Players.Count)
		{
			return true;
		}
		LocString locString = new LocString("gameplay_ui", "CONFIRM_LOAD_SAVE.body");
		locString.Add("MissingCount", _runLobby.Run.Players.Count - _runLobby.ConnectedPlayerIds.Count);
		NGenericPopup nGenericPopup = NGenericPopup.Create();
		NModalContainer.Instance.Add(nGenericPopup);
		return await nGenericPopup.WaitForConfirmation(locString, new LocString("gameplay_ui", "CONFIRM_LOAD_SAVE.header"), new LocString("gameplay_ui", "CONFIRM_LOAD_SAVE.cancel"), new LocString("gameplay_ui", "CONFIRM_LOAD_SAVE.confirm"));
	}

	private async Task StartRun()
	{
		Log.Info("Loading a multiplayer run. Players: " + string.Join(",", _runLobby.ConnectedPlayerIds) + ".");
		SerializablePlayer serializablePlayer = _runLobby.Run.Players.First((SerializablePlayer p) => p.NetId == _runLobby.NetService.NetId);
		SfxCmd.Play(ModelDb.GetById<CharacterModel>(serializablePlayer.CharacterId).CharacterTransitionSfx);
		await NGame.Instance.Transition.FadeOut(0.8f, ModelDb.GetById<CharacterModel>(serializablePlayer.CharacterId).CharacterSelectTransitionPath);
		RunState runState = RunState.FromSerializable(_runLobby.Run);
		await RunManager.Instance.SetUpSavedMultiPlayer(runState, _runLobby);
		await NGame.Instance.LoadRun(runState, _runLobby.Run.PreFinishedRoom);
		CleanUpLobby(disconnectSession: false);
		await NGame.Instance.Transition.FadeIn();
	}

	public void PlayerConnected(ulong playerId)
	{
		Log.Info($"Player connected: {playerId}");
		_remotePlayerContainer.OnPlayerConnected(playerId);
		UpdateRichPresence();
	}

	public void PlayerReadyChanged(ulong playerId)
	{
		Log.Info($"Player ready changed: {playerId}");
		_remotePlayerContainer.OnPlayerChanged(playerId);
		if (playerId == _runLobby.NetService.NetId && !_runLobby.IsPlayerReady(playerId))
		{
			_confirmButton.Enable();
			_backButton.Enable();
			_unreadyButton.Disable();
		}
	}

	public void RemotePlayerDisconnected(ulong playerId)
	{
		Log.Info($"Player disconnected: {playerId}");
		_remotePlayerContainer.OnPlayerDisconnected(playerId);
		UpdateRichPresence();
	}

	public void BeginRun()
	{
		NAudioManager.Instance?.StopMusic();
		_confirmButton.Disable();
		_unreadyButton.Disable();
		TaskHelper.RunSafely(StartRun());
	}

	public void LocalPlayerDisconnected(NetErrorInfo info)
	{
		if (info.SelfInitiated && info.GetReason() == NetError.Quit)
		{
			return;
		}
		if (_stack != null && _stack.Peek() == this)
		{
			_stack.Pop();
		}
		if (TestMode.IsOff)
		{
			NErrorPopup nErrorPopup = NErrorPopup.Create(info);
			if (nErrorPopup != null)
			{
				NModalContainer.Instance.Add(nErrorPopup);
			}
		}
	}

	private void AfterMultiplayerStarted()
	{
		NGame.Instance.RemoteCursorContainer.Initialize(_runLobby.InputSynchronizer, _runLobby.ConnectedPlayerIds);
		NGame.Instance.ReactionContainer.InitializeNetworking(_runLobby.NetService);
		SerializablePlayer serializablePlayer = _runLobby.Run.Players.First((SerializablePlayer p) => p.NetId == _runLobby.NetService.NetId);
		CharacterModel byId = ModelDb.GetById<CharacterModel>(serializablePlayer.CharacterId);
		SfxCmd.Play(byId.CharacterSelectSfx);
		foreach (Node child in _bgContainer.GetChildren())
		{
			_bgContainer.RemoveChildSafely(child);
			child.QueueFreeSafely();
		}
		Control control = PreloadManager.Cache.GetScene(byId.CharacterSelectBg).Instantiate<Control>(PackedScene.GenEditState.Disabled);
		control.Name = byId.Id.Entry + "_bg";
		_bgContainer.AddChildSafely(control);
		_name.SetTextAutoSize(byId.Title.GetFormattedText());
		_hp.SetTextAutoSize($"{serializablePlayer.CurrentHp}/{serializablePlayer.MaxHp}");
		_gold.SetTextAutoSize($"{serializablePlayer.Gold}");
		LocString locString = new LocString("main_menu_ui", "MULTIPLAYER_LOAD_MENU.FLOOR");
		locString.Add("floor", _runLobby.Run.VisitedMapCoords.Count);
		_floorLabel.Text = locString.GetFormattedText();
		LocString locString2 = new LocString("main_menu_ui", "MULTIPLAYER_LOAD_MENU.ACT");
		locString2.Add("act", _runLobby.Run.CurrentActIndex + 1);
		_actLabel.Text = locString2.GetFormattedText();
		UpdateRichPresence();
		MegaCrit.Sts2.Core.Logging.Logger.logLevelTypeMap[LogType.Network] = LogLevel.Debug;
		MegaCrit.Sts2.Core.Logging.Logger.logLevelTypeMap[LogType.Actions] = LogLevel.VeryDebug;
		MegaCrit.Sts2.Core.Logging.Logger.logLevelTypeMap[LogType.GameSync] = LogLevel.VeryDebug;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(16);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false), MethodFlags.Normal | MethodFlags.Static, null, null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnSubmenuShown, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnSubmenuClosed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnSubmenuHidden, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnEmbarkPressed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.OnUnreadyPressed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.UpdateRichPresence, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.CleanUpLobby, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Bool, "disconnectSession", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.PlayerConnected, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, "playerId", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.PlayerReadyChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, "playerId", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.RemotePlayerDisconnected, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, "playerId", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.BeginRun, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.AfterMultiplayerStarted, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.Create && args.Count == 0)
		{
			ret = VariantUtils.CreateFrom<NMultiplayerLoadGameScreen>(Create());
			return true;
		}
		if (method == MethodName._Ready && args.Count == 0)
		{
			_Ready();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnSubmenuOpened && args.Count == 0)
		{
			OnSubmenuOpened();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnSubmenuShown && args.Count == 0)
		{
			OnSubmenuShown();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnSubmenuClosed && args.Count == 0)
		{
			OnSubmenuClosed();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnSubmenuHidden && args.Count == 0)
		{
			OnSubmenuHidden();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnEmbarkPressed && args.Count == 1)
		{
			OnEmbarkPressed(VariantUtils.ConvertTo<NButton>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnUnreadyPressed && args.Count == 1)
		{
			OnUnreadyPressed(VariantUtils.ConvertTo<NButton>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateRichPresence && args.Count == 0)
		{
			UpdateRichPresence();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName._Process && args.Count == 1)
		{
			_Process(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CleanUpLobby && args.Count == 1)
		{
			CleanUpLobby(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.PlayerConnected && args.Count == 1)
		{
			PlayerConnected(VariantUtils.ConvertTo<ulong>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.PlayerReadyChanged && args.Count == 1)
		{
			PlayerReadyChanged(VariantUtils.ConvertTo<ulong>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.RemotePlayerDisconnected && args.Count == 1)
		{
			RemotePlayerDisconnected(VariantUtils.ConvertTo<ulong>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.BeginRun && args.Count == 0)
		{
			BeginRun();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.AfterMultiplayerStarted && args.Count == 0)
		{
			AfterMultiplayerStarted();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.Create && args.Count == 0)
		{
			ret = VariantUtils.CreateFrom<NMultiplayerLoadGameScreen>(Create());
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.Create)
		{
			return true;
		}
		if (method == MethodName._Ready)
		{
			return true;
		}
		if (method == MethodName.OnSubmenuOpened)
		{
			return true;
		}
		if (method == MethodName.OnSubmenuShown)
		{
			return true;
		}
		if (method == MethodName.OnSubmenuClosed)
		{
			return true;
		}
		if (method == MethodName.OnSubmenuHidden)
		{
			return true;
		}
		if (method == MethodName.OnEmbarkPressed)
		{
			return true;
		}
		if (method == MethodName.OnUnreadyPressed)
		{
			return true;
		}
		if (method == MethodName.UpdateRichPresence)
		{
			return true;
		}
		if (method == MethodName._Process)
		{
			return true;
		}
		if (method == MethodName.CleanUpLobby)
		{
			return true;
		}
		if (method == MethodName.PlayerConnected)
		{
			return true;
		}
		if (method == MethodName.PlayerReadyChanged)
		{
			return true;
		}
		if (method == MethodName.RemotePlayerDisconnected)
		{
			return true;
		}
		if (method == MethodName.BeginRun)
		{
			return true;
		}
		if (method == MethodName.AfterMultiplayerStarted)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._name)
		{
			_name = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._infoPanel)
		{
			_infoPanel = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._hp)
		{
			_hp = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._gold)
		{
			_gold = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._selectedButton)
		{
			_selectedButton = VariantUtils.ConvertTo<NCharacterSelectButton>(in value);
			return true;
		}
		if (name == PropertyName._bgContainer)
		{
			_bgContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._confirmButton)
		{
			_confirmButton = VariantUtils.ConvertTo<NConfirmButton>(in value);
			return true;
		}
		if (name == PropertyName._backButton)
		{
			_backButton = VariantUtils.ConvertTo<NBackButton>(in value);
			return true;
		}
		if (name == PropertyName._unreadyButton)
		{
			_unreadyButton = VariantUtils.ConvertTo<NBackButton>(in value);
			return true;
		}
		if (name == PropertyName._ascensionPanel)
		{
			_ascensionPanel = VariantUtils.ConvertTo<NAscensionPanel>(in value);
			return true;
		}
		if (name == PropertyName._floorLabel)
		{
			_floorLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName._actLabel)
		{
			_actLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName._remotePlayerContainer)
		{
			_remotePlayerContainer = VariantUtils.ConvertTo<NRemoteLoadLobbyPlayerContainer>(in value);
			return true;
		}
		if (name == PropertyName._infoPanelTween)
		{
			_infoPanelTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		if (name == PropertyName._infoPanelPosFinalVal)
		{
			_infoPanelPosFinalVal = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.InitialFocusedControl)
		{
			value = VariantUtils.CreateFrom<Control>(InitialFocusedControl);
			return true;
		}
		if (name == PropertyName._name)
		{
			value = VariantUtils.CreateFrom(in _name);
			return true;
		}
		if (name == PropertyName._infoPanel)
		{
			value = VariantUtils.CreateFrom(in _infoPanel);
			return true;
		}
		if (name == PropertyName._hp)
		{
			value = VariantUtils.CreateFrom(in _hp);
			return true;
		}
		if (name == PropertyName._gold)
		{
			value = VariantUtils.CreateFrom(in _gold);
			return true;
		}
		if (name == PropertyName._selectedButton)
		{
			value = VariantUtils.CreateFrom(in _selectedButton);
			return true;
		}
		if (name == PropertyName._bgContainer)
		{
			value = VariantUtils.CreateFrom(in _bgContainer);
			return true;
		}
		if (name == PropertyName._confirmButton)
		{
			value = VariantUtils.CreateFrom(in _confirmButton);
			return true;
		}
		if (name == PropertyName._backButton)
		{
			value = VariantUtils.CreateFrom(in _backButton);
			return true;
		}
		if (name == PropertyName._unreadyButton)
		{
			value = VariantUtils.CreateFrom(in _unreadyButton);
			return true;
		}
		if (name == PropertyName._ascensionPanel)
		{
			value = VariantUtils.CreateFrom(in _ascensionPanel);
			return true;
		}
		if (name == PropertyName._floorLabel)
		{
			value = VariantUtils.CreateFrom(in _floorLabel);
			return true;
		}
		if (name == PropertyName._actLabel)
		{
			value = VariantUtils.CreateFrom(in _actLabel);
			return true;
		}
		if (name == PropertyName._remotePlayerContainer)
		{
			value = VariantUtils.CreateFrom(in _remotePlayerContainer);
			return true;
		}
		if (name == PropertyName._infoPanelTween)
		{
			value = VariantUtils.CreateFrom(in _infoPanelTween);
			return true;
		}
		if (name == PropertyName._infoPanelPosFinalVal)
		{
			value = VariantUtils.CreateFrom(in _infoPanelPosFinalVal);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._name, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._infoPanel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._hp, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._gold, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._selectedButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._bgContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._confirmButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._backButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._unreadyButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._ascensionPanel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._floorLabel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._actLabel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._remotePlayerContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._infoPanelTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Vector2, PropertyName._infoPanelPosFinalVal, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.InitialFocusedControl, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._name, Variant.From(in _name));
		info.AddProperty(PropertyName._infoPanel, Variant.From(in _infoPanel));
		info.AddProperty(PropertyName._hp, Variant.From(in _hp));
		info.AddProperty(PropertyName._gold, Variant.From(in _gold));
		info.AddProperty(PropertyName._selectedButton, Variant.From(in _selectedButton));
		info.AddProperty(PropertyName._bgContainer, Variant.From(in _bgContainer));
		info.AddProperty(PropertyName._confirmButton, Variant.From(in _confirmButton));
		info.AddProperty(PropertyName._backButton, Variant.From(in _backButton));
		info.AddProperty(PropertyName._unreadyButton, Variant.From(in _unreadyButton));
		info.AddProperty(PropertyName._ascensionPanel, Variant.From(in _ascensionPanel));
		info.AddProperty(PropertyName._floorLabel, Variant.From(in _floorLabel));
		info.AddProperty(PropertyName._actLabel, Variant.From(in _actLabel));
		info.AddProperty(PropertyName._remotePlayerContainer, Variant.From(in _remotePlayerContainer));
		info.AddProperty(PropertyName._infoPanelTween, Variant.From(in _infoPanelTween));
		info.AddProperty(PropertyName._infoPanelPosFinalVal, Variant.From(in _infoPanelPosFinalVal));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._name, out var value))
		{
			_name = value.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._infoPanel, out var value2))
		{
			_infoPanel = value2.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._hp, out var value3))
		{
			_hp = value3.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._gold, out var value4))
		{
			_gold = value4.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._selectedButton, out var value5))
		{
			_selectedButton = value5.As<NCharacterSelectButton>();
		}
		if (info.TryGetProperty(PropertyName._bgContainer, out var value6))
		{
			_bgContainer = value6.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._confirmButton, out var value7))
		{
			_confirmButton = value7.As<NConfirmButton>();
		}
		if (info.TryGetProperty(PropertyName._backButton, out var value8))
		{
			_backButton = value8.As<NBackButton>();
		}
		if (info.TryGetProperty(PropertyName._unreadyButton, out var value9))
		{
			_unreadyButton = value9.As<NBackButton>();
		}
		if (info.TryGetProperty(PropertyName._ascensionPanel, out var value10))
		{
			_ascensionPanel = value10.As<NAscensionPanel>();
		}
		if (info.TryGetProperty(PropertyName._floorLabel, out var value11))
		{
			_floorLabel = value11.As<MegaRichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName._actLabel, out var value12))
		{
			_actLabel = value12.As<MegaRichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName._remotePlayerContainer, out var value13))
		{
			_remotePlayerContainer = value13.As<NRemoteLoadLobbyPlayerContainer>();
		}
		if (info.TryGetProperty(PropertyName._infoPanelTween, out var value14))
		{
			_infoPanelTween = value14.As<Tween>();
		}
		if (info.TryGetProperty(PropertyName._infoPanelPosFinalVal, out var value15))
		{
			_infoPanelPosFinalVal = value15.As<Vector2>();
		}
	}
}

// sts2, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
// MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect.NRegentCharacterSelectBg
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

[ScriptPath("res://src/Core/Nodes/Screens/CharacterSelect/NRegentCharacterSelectBg.cs")]
public class NRegentCharacterSelectBg : Control
{
	public new class MethodName : Control.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public static readonly StringName SetSkin = "SetSkin";
	}

	public new class PropertyName : Control.PropertyName
	{
		public static readonly StringName _sphereGuardianHover = "_sphereGuardianHover";

		public static readonly StringName _decaHover = "_decaHover";

		public static readonly StringName _sentryHover = "_sentryHover";

		public static readonly StringName _sneckoHover = "_sneckoHover";

		public static readonly StringName _cultistHover = "_cultistHover";

		public static readonly StringName _shapesHover = "_shapesHover";

		public static readonly StringName _amogusHover = "_amogusHover";
	}

	public new class SignalName : Control.SignalName
	{
	}

	private MegaSprite _spineController;

	private Control _sphereGuardianHover;

	private Control _decaHover;

	private Control _sentryHover;

	private Control _sneckoHover;

	private Control _cultistHover;

	private Control _shapesHover;

	private Control _amogusHover;

	public override void _Ready()
	{
		_spineController = new MegaSprite(GetNode("SpineSprite"));
		_sphereGuardianHover = GetNode<Control>("SphereGuardianHover");
		_sphereGuardianHover.Connect(Control.SignalName.MouseEntered, Callable.From(delegate
		{
			SetSkin("spheric guardian constellation");
		}));
		_sphereGuardianHover.Connect(Control.SignalName.MouseExited, Callable.From(delegate
		{
			SetSkin("normal");
		}));
		_decaHover = GetNode<Control>("DecaHover");
		_decaHover.Connect(Control.SignalName.MouseEntered, Callable.From(delegate
		{
			SetSkin("deca outline");
		}));
		_decaHover.Connect(Control.SignalName.MouseExited, Callable.From(delegate
		{
			SetSkin("normal");
		}));
		_sentryHover = GetNode<Control>("SentryHover");
		_sentryHover.Connect(Control.SignalName.MouseEntered, Callable.From(delegate
		{
			SetSkin("sentry constellation");
		}));
		_sentryHover.Connect(Control.SignalName.MouseExited, Callable.From(delegate
		{
			SetSkin("normal");
		}));
		_sneckoHover = GetNode<Control>("SneckoHover");
		_sneckoHover.Connect(Control.SignalName.MouseEntered, Callable.From(delegate
		{
			SetSkin("snecko constellation");
		}));
		_sneckoHover.Connect(Control.SignalName.MouseExited, Callable.From(delegate
		{
			SetSkin("normal");
		}));
		_cultistHover = GetNode<Control>("CultistHover");
		_cultistHover.Connect(Control.SignalName.MouseEntered, Callable.From(delegate
		{
			SetSkin("cultist constellation");
		}));
		_cultistHover.Connect(Control.SignalName.MouseExited, Callable.From(delegate
		{
			SetSkin("normal");
		}));
		_shapesHover = GetNode<Control>("ShapesHover");
		_shapesHover.Connect(Control.SignalName.MouseEntered, Callable.From(delegate
		{
			SetSkin("shapes constellation");
		}));
		_shapesHover.Connect(Control.SignalName.MouseExited, Callable.From(delegate
		{
			SetSkin("normal");
		}));
		_amogusHover = GetNode<Control>("AmogusHover");
		_amogusHover.Connect(Control.SignalName.MouseEntered, Callable.From(delegate
		{
			SetSkin("amogus constellation");
		}));
		_amogusHover.Connect(Control.SignalName.MouseExited, Callable.From(delegate
		{
			SetSkin("normal");
		}));
	}

	private void SetSkin(string skinName)
	{
		MegaSkeleton skeleton = _spineController.GetSkeleton();
		if (skeleton != null)
		{
			skeleton.SetSkin(skeleton.GetData().FindSkin(skinName));
			skeleton.SetSlotsToSetupPose();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.SetSkin, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.String, "skinName", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName._Ready && args.Count == 0)
		{
			_Ready();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetSkin && args.Count == 1)
		{
			SetSkin(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName._Ready)
		{
			return true;
		}
		if (method == MethodName.SetSkin)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._sphereGuardianHover)
		{
			_sphereGuardianHover = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._decaHover)
		{
			_decaHover = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._sentryHover)
		{
			_sentryHover = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._sneckoHover)
		{
			_sneckoHover = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._cultistHover)
		{
			_cultistHover = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._shapesHover)
		{
			_shapesHover = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._amogusHover)
		{
			_amogusHover = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName._sphereGuardianHover)
		{
			value = VariantUtils.CreateFrom(in _sphereGuardianHover);
			return true;
		}
		if (name == PropertyName._decaHover)
		{
			value = VariantUtils.CreateFrom(in _decaHover);
			return true;
		}
		if (name == PropertyName._sentryHover)
		{
			value = VariantUtils.CreateFrom(in _sentryHover);
			return true;
		}
		if (name == PropertyName._sneckoHover)
		{
			value = VariantUtils.CreateFrom(in _sneckoHover);
			return true;
		}
		if (name == PropertyName._cultistHover)
		{
			value = VariantUtils.CreateFrom(in _cultistHover);
			return true;
		}
		if (name == PropertyName._shapesHover)
		{
			value = VariantUtils.CreateFrom(in _shapesHover);
			return true;
		}
		if (name == PropertyName._amogusHover)
		{
			value = VariantUtils.CreateFrom(in _amogusHover);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._sphereGuardianHover, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._decaHover, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._sentryHover, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._sneckoHover, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._cultistHover, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._shapesHover, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._amogusHover, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._sphereGuardianHover, Variant.From(in _sphereGuardianHover));
		info.AddProperty(PropertyName._decaHover, Variant.From(in _decaHover));
		info.AddProperty(PropertyName._sentryHover, Variant.From(in _sentryHover));
		info.AddProperty(PropertyName._sneckoHover, Variant.From(in _sneckoHover));
		info.AddProperty(PropertyName._cultistHover, Variant.From(in _cultistHover));
		info.AddProperty(PropertyName._shapesHover, Variant.From(in _shapesHover));
		info.AddProperty(PropertyName._amogusHover, Variant.From(in _amogusHover));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._sphereGuardianHover, out var value))
		{
			_sphereGuardianHover = value.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._decaHover, out var value2))
		{
			_decaHover = value2.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._sentryHover, out var value3))
		{
			_sentryHover = value3.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._sneckoHover, out var value4))
		{
			_sneckoHover = value4.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._cultistHover, out var value5))
		{
			_cultistHover = value5.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._shapesHover, out var value6))
		{
			_shapesHover = value6.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._amogusHover, out var value7))
		{
			_amogusHover = value7.As<Control>();
		}
	}
}
