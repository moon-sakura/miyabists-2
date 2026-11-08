using Godot;
using System;

namespace Miyabists2.Scripts.Service
{
    /// <summary>
    /// 战斗胜利时的「WIPEOUT」风格演出：
    /// 全屏灰黄滤镜，快速闪两下。纯视觉效果，不拦截输入、不暂停游戏。
    ///
    /// 实现方式：在高层 CanvasLayer 上盖一个全屏 ColorRect，
    /// 用 BackBufferCopy 抓取下层画面，再用 shader 做「去饱和 + 黄调」，
    /// 通过 uniform flash 控制滤镜强度完成两次闪光。
    /// </summary>
    public partial class MiyabiVictoryFxOverlay : Control
    {
        // 整个演出的总时长（秒），到点自动删除自己
        private const float TotalDuration = 0.55f;

        // 两次闪光的起止时间（秒）
        private static readonly (float Start, float End)[] Pulses =
        {
            (0.00f, 0.16f),
            (0.22f, 0.44f),
        };

        private const string ShaderCode = @"shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, filter_linear;
uniform float flash : hint_range(0.0, 1.0) = 0.0;
uniform float desaturation : hint_range(0.0, 1.0) = 0.85;
uniform vec3 tint : source_color = vec3(1.0, 0.86, 0.36);
uniform float tint_strength : hint_range(0.0, 1.0) = 0.8;

void fragment() {
    vec3 src = textureLod(screen_texture, SCREEN_UV, 0.0).rgb;

    // 先去饱和得到灰调
    float lum = dot(src, vec3(0.299, 0.587, 0.114));
    vec3 filtered = mix(src, vec3(lum), desaturation);

    // 再压上一层灰黄色
    filtered *= mix(vec3(1.0), tint, tint_strength);
    filtered += vec3(0.04, 0.035, 0.0);

    // flash 0 = 原画面，1 = 完整滤镜
    COLOR = vec4(mix(src, filtered, flash), 1.0);
}";

        private ShaderMaterial? _material;
        private float _elapsed;

        public override void _Ready()
        {
            ProcessMode = ProcessModeEnum.Always;
            SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
            MouseFilter = MouseFilterEnum.Ignore; // 纯视觉，不挡输入
            SetProcess(true);

            // 2D 下读取屏幕纹理前需要先把后缓冲拷贝出来
            BackBufferCopy copy = new()
            {
                CopyMode = BackBufferCopy.CopyModeEnum.Viewport
            };
            AddChild(copy);

            Shader shader = new() { Code = ShaderCode };
            _material = new ShaderMaterial { Shader = shader };
            _material.SetShaderParameter("flash", 0f);

            ColorRect rect = new()
            {
                Color = Colors.White,
                Material = _material,
                MouseFilter = MouseFilterEnum.Ignore
            };
            rect.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
            AddChild(rect);
        }

        public override void _Process(double delta)
        {
            _elapsed += (float)delta;

            float flash = 0f;
            foreach ((float start, float end) in Pulses)
            {
                if (_elapsed >= start && _elapsed <= end)
                {
                    float phase = (_elapsed - start) / (end - start);
                    // sin 曲线：0 → 1 → 0，pow 让闪光更“脆”
                    flash = Mathf.Pow(Mathf.Sin(phase * Mathf.Pi), 1.4f);
                }
            }

            _material?.SetShaderParameter("flash", flash);

            if (_elapsed >= TotalDuration)
            {
                SetProcess(false);
                QueueFree();
            }
        }
    }

    /// <summary>
    /// 战斗胜利滤镜的播放入口。一般由 <c>MiyabiVictoryFxPatch</c> 自动调用，
    /// 也可以手动 <c>MiyabiVictoryFxService.Play()</c> 触发一次。
    /// </summary>
    public static class MiyabiVictoryFxService
    {
        private const string HostNodeName = "MiyabiVictoryFxHost";

        // 比视频层（2000）低一点，两者同时出现时视频优先
        private const int HostCanvasLayer = 1990;

        private static CanvasLayer? _host;

        /// <summary>
        /// 总开关。当前效果未达预期（闪得太快、缺少 WIPEOUT 字样、没有音频放慢），
        /// 暂时搁置，默认关闭。以后素材和节奏调整好，把这里改成 true 即可重新启用。
        /// </summary>
        public static bool Enabled { get; set; } = false;

        // 胜利收尾在极端情况下可能被重复调用（过期的回合状态恢复等），做个简单去重
        private static ulong _lastPlayMsec;

        public static void Play()
        {
            // 搁置期间直接跳过；同时跟随「播放技能视频」这一设置
            if (!Enabled || !MiyabiModConfig.PlayVideo)
                return;

            ulong now = Time.GetTicksMsec();
            if (now - _lastPlayMsec < 1000)
                return;
            _lastPlayMsec = now;

            CanvasLayer? host = EnsureHost();
            if (host == null)
                return;

            // 同一时间只保留一个特效
            foreach (Node child in host.GetChildren())
            {
                host.RemoveChild(child);
                child.QueueFree();
            }

            MiyabiVictoryFxOverlay overlay = new() { Name = "MiyabiVictoryFx" };
            host.AddChild(overlay);
        }

        private static CanvasLayer? EnsureHost()
        {
            if (_host != null && GodotObject.IsInstanceValid(_host) && _host.IsInsideTree())
                return _host;

            SceneTree? tree = Engine.GetMainLoop() as SceneTree;
            if (tree?.Root == null)
                return null;

            _host = tree.Root.GetNodeOrNull<CanvasLayer>(HostNodeName);
            if (_host == null)
            {
                _host = new CanvasLayer
                {
                    Name = HostNodeName,
                    Layer = HostCanvasLayer,
                    ProcessMode = Node.ProcessModeEnum.Always
                };
                tree.Root.AddChild(_host);
            }

            return _host;
        }
    }
}
