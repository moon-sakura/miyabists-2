using Godot;
using System;

namespace Miyabists2.Scripts.UI;

/// <summary>
/// 特殊挑战面板。选项数据由外部通过 SetOptions 注入，支持不同角色不同挑战。
/// </summary>
public partial class MiyabiFunPilePanel : Control
{
    // ===== 子节点 =====
    private Panel _background;
    private Label _titleLabel;
    private Button _prevButton;
    private Label _optionLabel;
    private Button _nextButton;
    private RichTextLabel _descriptionLabel;

    private int _currentIndex;
    private (string name, string desc)[] _options = Array.Empty<(string, string)>();

    private int MaxIndex => _options.Length - 1;

    // ===== 属性 =====

    public int CurrentIndex
    {
        get => _currentIndex;
        set
        {
            if (_options.Length > 0 && value >= 0 && value <= MaxIndex)
            {
                _currentIndex = value;
                RefreshDisplay();
            }
        }
    }

    // ===== 事件 =====

    public event Action<int> FunPileChanged;

    // ===== Godot =====

    public override void _Ready()
    {
        GD.Print("[MiyabiFunPilePanel] _Ready 调用，开始构建 UI...");
        BuildUI();
        GD.Print("[MiyabiFunPilePanel] UI 构建完成");
    }

    // ===== UI 构建 =====

    private void BuildUI()
    {
        float panelW = 280f;
        float panelH = 140f;
        SetSize(new Vector2(panelW, panelH));

        _background = new Panel();
        _background.SetSize(new Vector2(panelW, panelH));
        _background.Position = Vector2.Zero;
        AddChild(_background);

        _titleLabel = new Label();
        _titleLabel.Text = "特殊挑战";
        _titleLabel.Position = new Vector2(10, 8);
        _titleLabel.AddThemeColorOverride("font_color", Colors.White);
        _titleLabel.AddThemeFontSizeOverride("font_size", 16);
        AddChild(_titleLabel);

        _prevButton = new Button();
        _prevButton.Text = "<";
        _prevButton.Position = new Vector2(20, 36);
        _prevButton.Size = new Vector2(60, 28);
        _prevButton.Connect(Button.SignalName.Pressed, Callable.From(OnPrevPressed));
        AddChild(_prevButton);

        _optionLabel = new Label();
        _optionLabel.Position = new Vector2(90, 36);
        _optionLabel.Size = new Vector2(100, 28);
        _optionLabel.HorizontalAlignment = HorizontalAlignment.Center;
        _optionLabel.VerticalAlignment = VerticalAlignment.Center;
        _optionLabel.AddThemeColorOverride("font_color", new Color("afcdde"));
        _optionLabel.AddThemeFontSizeOverride("font_size", 14);
        AddChild(_optionLabel);

        _nextButton = new Button();
        _nextButton.Text = ">";
        _nextButton.Position = new Vector2(200, 36);
        _nextButton.Size = new Vector2(60, 28);
        _nextButton.Connect(Button.SignalName.Pressed, Callable.From(OnNextPressed));
        AddChild(_nextButton);

        _descriptionLabel = new RichTextLabel();
        _descriptionLabel.Position = new Vector2(10, 74);
        _descriptionLabel.Size = new Vector2(260, 56);
        _descriptionLabel.BbcodeEnabled = true;
        _descriptionLabel.FitContent = false;
        _descriptionLabel.ScrollActive = true;
        AddChild(_descriptionLabel);
    }

    // ===== 外部注入 =====

    /// <summary>
    /// 设置当前角色的挑战选项列表和选中索引。
    /// </summary>
    public void SetOptions((string name, string desc)[] options, int currentIndex)
    {
        _options = options ?? Array.Empty<(string, string)>();
        if (_options.Length == 0) return;

        if (currentIndex < 0) currentIndex = 0;
        if (currentIndex > MaxIndex) currentIndex = 0;

        _currentIndex = currentIndex;
        RefreshDisplay();
    }

    // ===== 按钮回调 =====

    private void OnPrevPressed()
    {
        if (_options.Length <= 1 || _currentIndex <= 0) return;
        _currentIndex--;
        RefreshDisplay();
        FunPileChanged?.Invoke(_currentIndex);
    }

    private void OnNextPressed()
    {
        if (_options.Length <= 1 || _currentIndex >= MaxIndex) return;
        _currentIndex++;
        RefreshDisplay();
        FunPileChanged?.Invoke(_currentIndex);
    }

    // ===== 显示刷新 =====

    private void RefreshDisplay()
    {
        if (_optionLabel == null || _options.Length == 0) return;

        var (name, desc) = _options[_currentIndex];
        _optionLabel.Text = name;

        _prevButton.Disabled = _options.Length <= 1 || _currentIndex <= 0;
        _nextButton.Disabled = _options.Length <= 1 || _currentIndex >= MaxIndex;

        _descriptionLabel.Text = $"[color=#FFD700]{name}[/color]：{desc}";
    }
}
