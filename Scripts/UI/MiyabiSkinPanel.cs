using Godot;
using System;
using System.Collections.Generic;

namespace Miyabists2.Scripts.UI;

public enum SkinType
{
    Combat,
    Rest,
    Shop
}

/// <summary>
/// 选人界面皮肤切换面板。支持 Combat / Rest / Shop 三种皮肤类型切换。
/// 纯代码构建，不依赖 tscn 文件。
/// </summary>
public partial class MiyabiSkinPanel : Control
{
    // ===== 子节点 =====
    private Panel _background;
    private Button _combatTab;
    private Button _restTab;
    private Button _shopTab;
    private TextureRect _previewImage;
    private Button _prevButton;
    private Button _nextButton;
    private Label _skinNameLabel;

    // ===== 数据 =====
    private List<string> _skinDisplayNames = new();
    private List<string> _previewImagePaths = new();
    private int _currentIndex;
    private SkinType _currentSkinType = SkinType.Combat;
    private string _language = "eng";

    public string Language
    {
        get => _language;
        set
        {
            _language = MiyabiUILoc.NormalizeLang(value);
            RefreshUILoc();
        }
    }

    // ===== 可配置属性 =====

    public List<string> PreviewImagePaths
    {
        get => _previewImagePaths;
        set
        {
            _previewImagePaths = value ?? new List<string>();
            RefreshDisplay();
        }
    }

    public List<string> SkinDisplayNames
    {
        get => _skinDisplayNames;
        set
        {
            _skinDisplayNames = value ?? new List<string>();
            RefreshDisplay();
        }
    }

    public int CurrentIndex
    {
        get => _currentIndex;
        set
        {
            if (value >= 0 && value < _skinDisplayNames.Count)
            {
                _currentIndex = value;
                RefreshDisplay();
            }
        }
    }

    public SkinType CurrentSkinType
    {
        get => _currentSkinType;
        set
        {
            if (_currentSkinType != value)
            {
                _currentSkinType = value;
                RefreshTabStyles();
                SkinTypeChanged?.Invoke(value);
            }
        }
    }

    // ===== 事件 =====

    /// <summary>用户切换皮肤槽位（上一页/下一页）。参数为新索引。</summary>
    public event Action<int> SkinChanged;

    /// <summary>用户切换皮肤类型标签（Combat/Rest/Shop）。参数为新类型。</summary>
    public event Action<SkinType> SkinTypeChanged;

    // ===== Godot 生命周期 =====

    public override void _Ready()
    {
        GD.Print("[MiyabiSkinPanel] _Ready 调用，开始构建 UI...");
        BuildUI();
        GD.Print("[MiyabiSkinPanel] UI 构建完成");
    }

    // ===== UI 构建 =====

    private void BuildUI()
    {
        float panelW = 280f;
        float panelH = 380f;
        SetSize(new Vector2(panelW, panelH));

        // 背景
        _background = new Panel();
        _background.SetSize(new Vector2(panelW, panelH));
        _background.Position = Vector2.Zero;
        AddChild(_background);

        // ---- 皮肤类型标签 ----
        float tabY = 8f;
        float tabW = 80f;
        float tabH = 28f;

        _combatTab = new Button();
        _combatTab.Text = "Combat";
        _combatTab.Position = new Vector2(14, tabY);
        _combatTab.Size = new Vector2(tabW, tabH);
        _combatTab.Connect(Button.SignalName.Pressed, Callable.From(() => { CurrentSkinType = SkinType.Combat; }));
        AddChild(_combatTab);

        _restTab = new Button();
        _restTab.Text = "Rest";
        _restTab.Position = new Vector2(100, tabY);
        _restTab.Size = new Vector2(tabW, tabH);
        _restTab.Connect(Button.SignalName.Pressed, Callable.From(() => { CurrentSkinType = SkinType.Rest; }));
        AddChild(_restTab);

        _shopTab = new Button();
        _shopTab.Text = "Shop";
        _shopTab.Position = new Vector2(186, tabY);
        _shopTab.Size = new Vector2(tabW, tabH);
        _shopTab.Connect(Button.SignalName.Pressed, Callable.From(() => { CurrentSkinType = SkinType.Shop; }));
        AddChild(_shopTab);

        RefreshTabStyles();

        // ---- 预览图框 ----
        _previewImage = new TextureRect();
        _previewImage.Position = new Vector2(50, 50);
        _previewImage.Size = new Vector2(180, 200);
        _previewImage.ExpandMode = TextureRect.ExpandModeEnum.FitWidthProportional;
        _previewImage.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        AddChild(_previewImage);

        // ---- 上一页 / 下一页 ----
        float navY = 262f;
        _prevButton = new Button();
        _prevButton.Text = "<";
        _prevButton.Position = new Vector2(50, navY);
        _prevButton.Size = new Vector2(80, 32);
        _prevButton.Connect(Button.SignalName.Pressed, Callable.From(OnPrevPressed));
        AddChild(_prevButton);

        _nextButton = new Button();
        _nextButton.Text = ">";
        _nextButton.Position = new Vector2(150, navY);
        _nextButton.Size = new Vector2(80, 32);
        _nextButton.Connect(Button.SignalName.Pressed, Callable.From(OnNextPressed));
        AddChild(_nextButton);

        // ---- 皮肤名称 ----
        _skinNameLabel = new Label();
        _skinNameLabel.Position = new Vector2(10, 306);
        _skinNameLabel.Size = new Vector2(260, 40);
        _skinNameLabel.HorizontalAlignment = HorizontalAlignment.Center;
        _skinNameLabel.VerticalAlignment = VerticalAlignment.Center;
        _skinNameLabel.AddThemeColorOverride("font_color", new Color("afcdde"));
        _skinNameLabel.AddThemeFontSizeOverride("font_size", 14);
        AddChild(_skinNameLabel);
    }

    private void RefreshUILoc()
    {
        if (_combatTab != null)
        {
            _combatTab.Text = MiyabiUILoc.Get("skin_combat", _language);
            _restTab.Text = MiyabiUILoc.Get("skin_rest", _language);
            _shopTab.Text = MiyabiUILoc.Get("skin_shop", _language);
        }
    }

    // ===== 标签样式 =====

    private void RefreshTabStyles()
    {
        if (_combatTab == null) return;

        Color activeColor = new Color("4682B4");
        Color inactiveColor = new Color("4a4a4a");

        _combatTab.Modulate = _currentSkinType == SkinType.Combat ? activeColor : inactiveColor;
        _restTab.Modulate = _currentSkinType == SkinType.Rest ? activeColor : inactiveColor;
        _shopTab.Modulate = _currentSkinType == SkinType.Shop ? activeColor : inactiveColor;
    }

    // ===== 按钮回调 =====

    private void OnPrevPressed()
    {
        if (_skinDisplayNames.Count <= 1) return;
        _currentIndex = (_currentIndex - 1 + _skinDisplayNames.Count) % _skinDisplayNames.Count;
        RefreshDisplay();
        SkinChanged?.Invoke(_currentIndex);
    }

    private void OnNextPressed()
    {
        if (_skinDisplayNames.Count <= 1) return;
        _currentIndex = (_currentIndex + 1) % _skinDisplayNames.Count;
        RefreshDisplay();
        SkinChanged?.Invoke(_currentIndex);
    }

    // ===== 显示刷新 =====

    private void RefreshDisplay()
    {
        if (_previewImage == null) return;

        if (_currentIndex >= 0 && _currentIndex < _previewImagePaths.Count)
        {
            string path = _previewImagePaths[_currentIndex];
            if (!string.IsNullOrEmpty(path) && ResourceLoader.Exists(path))
            {
                _previewImage.Texture = ResourceLoader.Load<Texture2D>(path);
            }
            else
            {
                _previewImage.Texture = null;
            }
        }
        else
        {
            _previewImage.Texture = null;
        }

        if (_currentIndex >= 0 && _currentIndex < _skinDisplayNames.Count)
        {
            _skinNameLabel.Text = _skinDisplayNames[_currentIndex];
        }
        else
        {
            _skinNameLabel.Text = "";
        }

        bool hasMultiple = _skinDisplayNames.Count > 1;
        if (_prevButton != null) _prevButton.Disabled = !hasMultiple;
        if (_nextButton != null) _nextButton.Disabled = !hasMultiple;
    }
}
