using Godot;
using System;
using System.Text;

namespace Miyabists2.Scripts.UI;

/// <summary>
/// 难度选择面板。位于皮肤面板上方，控制 MiyabiModConfig.CombatHardSelected。
/// 切换难度时显示当前等级及所有已激活难度的累积描述。
/// </summary>
public partial class MiyabiDifficultyPanel : Control
{
    // ===== 子节点 =====
    private Panel _background;
    private Label _titleLabel;
    private Button _prevButton;
    private Label _levelLabel;
    private Button _nextButton;
    private CheckBox _enemyStrongerCheck;
    private RichTextLabel _descriptionLabel;

    private int _currentLevel;
    private bool _enemyStronger;

    // ===== 难度数据 =====

    private static readonly (string name, string desc)[] _difficultyLevels = new[]
    {
        ("无进阶", ""),
        ("锑级", "敌人+10%生命值"),
        ("锌级", "每一幕（第一幕除外）开始时，随机[color=red]降级[/color]一张卡牌"),
        ("锡级", "战斗开始后，使用的第一张牌[color=red]费用+1[/color]"),
        ("锰级", "战斗开始时，敌人获得1点力量，自己失去1点力量"),
        ("镉级", "敌人改为+20%生命"),
        ("镍级", "前三个回合[color=red]少抽1张牌[/color]"),
        ("铜级", "敌人在死亡时造成6点伤害"),
        ("铋级", "敌人改为+30%生命，额外获得1点力量"),
        ("铅级", "进入新的一幕后会向卡组里加一张随机[color=red]诅咒[/color]"),
        ("汞级", "敌人第一次受到致命伤害前会获得1层[color=#FFD700]缓冲[/color]"
            + "与2点[color=#FFD700]力量[/color]并恢复30%生命"),
    };

    public static int MaxLevel => _difficultyLevels.Length - 1;

    // ===== 属性 =====

    public int CurrentLevel
    {
        get => _currentLevel;
        set
        {
            if (value >= 0 && value <= MaxLevel)
            {
                _currentLevel = value;
                RefreshDisplay();
            }
        }
    }

    // ===== 事件 =====

    public event Action<int> LevelChanged;

    public bool EnemyStronger
    {
        get => _enemyStronger;
        set
        {
            _enemyStronger = value;
            if (_enemyStrongerCheck != null)
                _enemyStrongerCheck.ButtonPressed = value;
        }
    }

    public event Action<bool> EnemyStrongerChanged;

    // ===== Godot =====

    public override void _Ready()
    {
        BuildUI();
    }

    // ===== UI 构建 =====

    private void BuildUI()
    {
        float panelW = 280f;
        float panelH = 330f;
        SetSize(new Vector2(panelW, panelH));

        // 背景
        _background = new Panel();
        _background.SetSize(new Vector2(panelW, panelH));
        _background.Position = Vector2.Zero;
        AddChild(_background);

        // 标题
        _titleLabel = new Label();
        _titleLabel.Text = "进阶选项";
        _titleLabel.Position = new Vector2(10, 8);
        _titleLabel.AddThemeColorOverride("font_color", Colors.White);
        _titleLabel.AddThemeFontSizeOverride("font_size", 16);
        AddChild(_titleLabel);

        // 上一级
        _prevButton = new Button();
        _prevButton.Text = "<";
        _prevButton.Position = new Vector2(20, 36);
        _prevButton.Size = new Vector2(60, 28);
        _prevButton.Connect(Button.SignalName.Pressed, Callable.From(OnPrevPressed));
        AddChild(_prevButton);

        // 等级标签
        _levelLabel = new Label();
        _levelLabel.Position = new Vector2(90, 36);
        _levelLabel.Size = new Vector2(100, 28);
        _levelLabel.HorizontalAlignment = HorizontalAlignment.Center;
        _levelLabel.VerticalAlignment = VerticalAlignment.Center;
        _levelLabel.AddThemeColorOverride("font_color", new Color("afcdde"));
        _levelLabel.AddThemeFontSizeOverride("font_size", 14);
        AddChild(_levelLabel);

        // 下一级
        _nextButton = new Button();
        _nextButton.Text = ">";
        _nextButton.Position = new Vector2(200, 36);
        _nextButton.Size = new Vector2(60, 28);
        _nextButton.Connect(Button.SignalName.Pressed, Callable.From(OnNextPressed));
        AddChild(_nextButton);

        // 加强模组敌人
        _enemyStrongerCheck = new CheckBox();
        _enemyStrongerCheck.Text = "加强模组敌人";
        _enemyStrongerCheck.Position = new Vector2(14, 72);
        _enemyStrongerCheck.Size = new Vector2(250, 24);
        _enemyStrongerCheck.AddThemeColorOverride("font_color", Colors.White);
        _enemyStrongerCheck.AddThemeFontSizeOverride("font_size", 13);
        _enemyStrongerCheck.Connect(CheckBox.SignalName.Toggled, Callable.From<bool>(OnEnemyStrongerToggled));
        AddChild(_enemyStrongerCheck);

        // 累积描述
        _descriptionLabel = new RichTextLabel();
        _descriptionLabel.Position = new Vector2(10, 104);
        _descriptionLabel.Size = new Vector2(260, 216);
        _descriptionLabel.BbcodeEnabled = true;
        _descriptionLabel.ScrollFollowing = true;
        _descriptionLabel.FitContent = false;
        _descriptionLabel.ScrollActive = true;
        AddChild(_descriptionLabel);
    }

    // ===== 按钮回调 =====

    private void OnPrevPressed()
    {
        if (_currentLevel <= 0) return;
        _currentLevel--;
        RefreshDisplay();
        LevelChanged?.Invoke(_currentLevel);
    }

    private void OnNextPressed()
    {
        if (_currentLevel >= MaxLevel) return;
        _currentLevel++;
        RefreshDisplay();
        LevelChanged?.Invoke(_currentLevel);
    }

    private void OnEnemyStrongerToggled(bool toggled)
    {
        _enemyStronger = toggled;
        EnemyStrongerChanged?.Invoke(toggled);
    }

    // ===== 显示刷新 =====

    private void RefreshDisplay()
    {
        if (_levelLabel == null) return;

        var (name, _) = _difficultyLevels[_currentLevel];
        _levelLabel.Text = _currentLevel == 0 ? "无进阶" : $"Lv.{_currentLevel} {name}";

        _prevButton.Disabled = _currentLevel <= 0;
        _nextButton.Disabled = _currentLevel >= MaxLevel;

        _descriptionLabel.Text = BuildCumulativeDescription(_currentLevel);
    }

    // ===== 累积描述生成 =====

    private static string BuildCumulativeDescription(int level)
    {
        if (level <= 0) return "[color=#888888]未启用任何难度修正[/color]";

        var sb = new StringBuilder();
        for (int i = 1; i <= level; i++)
        {
            var (name, desc) = _difficultyLevels[i];
            sb.Append("[color=#FFD700]");
            sb.Append(i);
            sb.Append('.');
            sb.Append(name);
            sb.Append("[/color]：");
            sb.Append(desc);
            if (i < level) sb.Append('\n');
        }
        return sb.ToString();
    }
}
