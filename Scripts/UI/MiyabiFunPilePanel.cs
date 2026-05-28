using Godot;
using System;

namespace Miyabists2.Scripts.UI;

/// <summary>
/// 特殊挑战面板。位于皮肤面板下方，控制 MiyabiModConfig.FunPileSelected。
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

    // ===== 特殊模式数据 =====

    private static readonly (string name, string desc)[] _funPileOptions = new[]
    {
        ("默认", "无变化"),
        ("邦布当家", "初始卡组变为邦布相关卡组"),
        ("蜂群集结", "初始卡组中添加1张升级后的[color=#FFD700]蜂群集结[/color]"),
    };

    public static int MaxIndex => _funPileOptions.Length - 1;

    // ===== 属性 =====

    public int CurrentIndex
    {
        get => _currentIndex;
        set
        {
            if (value >= 0 && value <= MaxIndex)
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
        BuildUI();
    }

    // ===== UI 构建 =====

    private void BuildUI()
    {
        float panelW = 280f;
        float panelH = 140f;
        SetSize(new Vector2(panelW, panelH));

        // 背景
        _background = new Panel();
        _background.SetSize(new Vector2(panelW, panelH));
        _background.Position = Vector2.Zero;
        AddChild(_background);

        // 标题
        _titleLabel = new Label();
        _titleLabel.Text = "特殊挑战";
        _titleLabel.Position = new Vector2(10, 8);
        _titleLabel.AddThemeColorOverride("font_color", Colors.White);
        _titleLabel.AddThemeFontSizeOverride("font_size", 16);
        AddChild(_titleLabel);

        // 上一个
        _prevButton = new Button();
        _prevButton.Text = "<";
        _prevButton.Position = new Vector2(20, 36);
        _prevButton.Size = new Vector2(60, 28);
        _prevButton.Connect(Button.SignalName.Pressed, Callable.From(OnPrevPressed));
        AddChild(_prevButton);

        // 选项名称
        _optionLabel = new Label();
        _optionLabel.Position = new Vector2(90, 36);
        _optionLabel.Size = new Vector2(100, 28);
        _optionLabel.HorizontalAlignment = HorizontalAlignment.Center;
        _optionLabel.VerticalAlignment = VerticalAlignment.Center;
        _optionLabel.AddThemeColorOverride("font_color", new Color("afcdde"));
        _optionLabel.AddThemeFontSizeOverride("font_size", 14);
        AddChild(_optionLabel);

        // 下一个
        _nextButton = new Button();
        _nextButton.Text = ">";
        _nextButton.Position = new Vector2(200, 36);
        _nextButton.Size = new Vector2(60, 28);
        _nextButton.Connect(Button.SignalName.Pressed, Callable.From(OnNextPressed));
        AddChild(_nextButton);

        // 描述
        _descriptionLabel = new RichTextLabel();
        _descriptionLabel.Position = new Vector2(10, 74);
        _descriptionLabel.Size = new Vector2(260, 56);
        _descriptionLabel.BbcodeEnabled = true;
        _descriptionLabel.FitContent = false;
        _descriptionLabel.ScrollActive = true;
        AddChild(_descriptionLabel);
    }

    // ===== 按钮回调 =====

    private void OnPrevPressed()
    {
        if (_currentIndex <= 0) return;
        _currentIndex--;
        RefreshDisplay();
        FunPileChanged?.Invoke(_currentIndex);
    }

    private void OnNextPressed()
    {
        if (_currentIndex >= MaxIndex) return;
        _currentIndex++;
        RefreshDisplay();
        FunPileChanged?.Invoke(_currentIndex);
    }

    // ===== 显示刷新 =====

    private void RefreshDisplay()
    {
        if (_optionLabel == null) return;

        var (name, desc) = _funPileOptions[_currentIndex];
        _optionLabel.Text = name;

        _prevButton.Disabled = _currentIndex <= 0;
        _nextButton.Disabled = _currentIndex >= MaxIndex;

        _descriptionLabel.Text = $"[color=#FFD700]{name}[/color]：{desc}";
    }
}