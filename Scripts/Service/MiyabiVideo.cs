using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Service
{
    /// <summary>
    /// 全屏视频覆盖层（内部使用，一般不需要直接 new）。
    ///
    /// 结构：
    ///   MiyabiVideoOverlay (Control，铺满屏幕，吃掉所有输入)
    ///   ├─ ColorRect        （黑色背景，兼作黑边）
    ///   └─ AspectRatioContainer
    ///      └─ VideoStreamPlayer（按视频原始宽高比铺放）
    ///
    /// 播放结束或被点击跳过时，它会恢复游戏暂停状态、删除自己，
    /// 底层画面从未被切换过，所以自然“回到之前的画面”。
    /// </summary>
    public partial class MiyabiVideoOverlay : Control
    {
        private VideoStream? _stream;
        private bool _cover = true;
        private bool _loop;
        private bool _pauseGame = true;
        private bool _mute;

        private SceneTree? _tree;
        private AspectRatioContainer? _aspect;
        private VideoStreamPlayer? _player;

        private bool _closed;
        private bool _pausedByUs;
        private bool _ratioApplied;

        /// <summary>
        /// 播放结束回调。true = 自然播放完毕；false = 被点击跳过 / 被中断 / 加载失败。
        /// </summary>
        public event Action<bool>? Completed;

        /// <summary>
        /// 配置参数，必须在加入场景树之前调用。
        /// </summary>
        public void Setup(VideoStream stream, bool cover, bool loop, bool pauseGame, bool mute = false)
        {
            _stream = stream;
            _cover = cover;
            _loop = loop;
            _pauseGame = pauseGame;
            _mute = mute;
        }

        public override void _Ready()
        {
            // Always：即使游戏树被暂停，视频解码、_Process、_Input 仍然工作
            ProcessMode = ProcessModeEnum.Always;
            SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
            MouseFilter = MouseFilterEnum.Stop; // 防止点击穿透到下层 UI
            FocusMode = FocusModeEnum.All;
            SetProcessInput(true);
            SetProcess(true);

            _tree = GetTree();
            PauseGameIfNeeded();

            BuildNodes();
            _player!.Play();
        }

        private void BuildNodes()
        {
            // 黑色背景：既是幕布，也是视频宽高比不匹配时的黑边
            ColorRect background = new()
            {
                Color = Colors.Black,
                MouseFilter = MouseFilterEnum.Ignore,
                ProcessMode = ProcessModeEnum.Always
            };
            background.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
            AddChild(background);

            // 用 AspectRatioContainer 自动处理任意分辨率 / 宽高比，
            // 不需要自己算屏幕尺寸，且不受游戏 stretch 设置影响。
            _aspect = new AspectRatioContainer
            {
                StretchMode = _cover
                    ? AspectRatioContainer.StretchModeEnum.Cover
                    : AspectRatioContainer.StretchModeEnum.Fit,
                AlignmentHorizontal = AspectRatioContainer.AlignmentMode.Center,
                AlignmentVertical = AspectRatioContainer.AlignmentMode.Center,
                Ratio = 16f / 9f, // 首帧解码出来后会改成视频真实比例
                ClipContents = true,
                MouseFilter = MouseFilterEnum.Ignore,
                ProcessMode = ProcessModeEnum.Always
            };
            _aspect.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
            AddChild(_aspect);

            _player = new VideoStreamPlayer
            {
                Name = "MiyabiVideoPlayer",
                Stream = _stream,
                Expand = true, // 铺满 AspectRatioContainer 给出的矩形
                Loop = _loop,
                Autoplay = false,
                MouseFilter = MouseFilterEnum.Ignore,
                ProcessMode = ProcessModeEnum.Always,
                // 静音时视频只出画面，声音交给 MiyabiAudioPlay 播放的语音
                VolumeDb = _mute ? -80f : 0f
            };
            _player.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
            _aspect.AddChild(_player);
            _player.Finished += OnVideoFinished;
        }

        public override void _Process(double delta)
        {
            if (_ratioApplied)
            {
                SetProcess(false);
                return;
            }

            // 首帧解码出来之前拿不到视频分辨率，拿到后立即修正比例
            if (_player == null || _aspect == null)
                return;

            Texture2D? texture = _player.GetVideoTexture();
            if (texture == null)
                return;

            int width = texture.GetWidth();
            int height = texture.GetHeight();
            if (width <= 0 || height <= 0)
                return;

            _aspect.Ratio = (float)width / height;
            _ratioApplied = true;
            SetProcess(false);
        }

        public override void _Input(InputEvent @event)
        {
            if (_closed)
                return;

            bool clicked = @event switch
            {
                InputEventMouseButton mouseButton => mouseButton.Pressed,
                InputEventScreenTouch screenTouch => screenTouch.Pressed,
                _ => false
            };

            // 点击屏幕 / 手柄确认 / Esc 都直接跳过
            if (clicked
                || @event.IsActionPressed("ui_cancel")
                || @event.IsActionPressed("ui_accept"))
            {
                RequestStop();
            }

            // 播放期间吞掉所有输入，避免点穿到下面的游戏界面
            GetViewport()?.SetInputAsHandled();
        }

        /// <summary>提前结束播放（等同于点击跳过）。</summary>
        public void RequestStop()
        {
            Close(skipped: true);
        }

        private void OnVideoFinished()
        {
            Close(skipped: false);
        }

        private void Close(bool skipped)
        {
            if (_closed)
                return;
            _closed = true;

            if (_player != null && GodotObject.IsInstanceValid(_player))
            {
                _player.Finished -= OnVideoFinished;
                _player.Stop();
            }

            RestorePause();

            Visible = false;
            MouseFilter = MouseFilterEnum.Ignore;
            SetProcess(false);
            SetProcessInput(false);

            // 先通知调用方（可能是 await 的后续逻辑），再删除自己
            Completed?.Invoke(!skipped);
            QueueFree();
        }

        public override void _ExitTree()
        {
            // 兜底：如果节点被外部移除（切场景、Mod 卸载等），也要恢复暂停并通知等待者
            if (_closed)
                return;
            _closed = true;

            if (_player != null && GodotObject.IsInstanceValid(_player))
                _player.Finished -= OnVideoFinished;

            RestorePause();
            SetProcessInput(false);
            Completed?.Invoke(false);
        }

        // ============================================================
        // 暂停控制
        // ============================================================

        private void PauseGameIfNeeded()
        {
            // 只在“游戏当前没暂停”时才由我们接管暂停，
            // 已经暂停的界面（菜单等）保持原状。
            if (!_pauseGame || _tree == null || _tree.Paused)
                return;

            _tree.Paused = true;
            _pausedByUs = true;
        }

        private void RestorePause()
        {
            if (!_pausedByUs)
                return;

            _pausedByUs = false;
            if (_tree != null && GodotObject.IsInstanceValid(_tree))
                _tree.Paused = false;
        }
    }

    /// <summary>
    /// 全屏视频播放服务。用法：
    ///
    /// <code>
    /// // 1) 最简单：播完自动回到原画面，点击屏幕可跳过
    /// MiyabiVideoService.Play("guang_awaken");
    ///
    /// // 2) 播放完后继续游戏逻辑（推荐在 async 流程里用）
    /// public override async Task SomeGameFlow()
    /// {
    ///     bool finished = await MiyabiVideoService.PlayAsync("guang_awaken");
    ///     // finished == true 自然播完；false 被点击跳过
    ///     await DoSomethingAfterVideo();
    /// }
    ///
    /// // 3) 不暂停游戏 / 保持原比例不裁切
    /// MiyabiVideoService.Play("guang_awaken", pauseGame: false, cover: false);
    ///
    /// // 4) 视频静音只出画面，语音用自己的系统同时播（音画不同步是预期）
    /// MiyabiAudioPlay.Random(YixuanFufaVoices);
    /// await MiyabiVideoService.PlayIfEnabledAsync("yixuan_end1", mute: true);
    /// </code>
    ///
    /// 视频文件放在 res://scenes/video/ 下，扩展名必须是 .ogv。
    /// 也可以直接传完整路径：MiyabiVideoService.Play("res://scenes/video/xxx.ogv")。
    /// </summary>
    public static class MiyabiVideoService
    {
        // 🛠️ 【修改这里】视频文件夹路径（与 scenes/audio 同级）
        private const string ModVideoRootPath = "res://scenes/video/";

        private const string HostNodeName = "MiyabiVideoHost";

        // 高于 Godot 内嵌 Window 的 1024 层，保证盖住所有游戏 UI
        private const int HostCanvasLayer = 2000;

        private static readonly Dictionary<string, string> VideoPathIndex =
            new(StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<string, VideoStream> StreamCache =
            new(StringComparer.OrdinalIgnoreCase);

        private static bool _indexed;
        private static CanvasLayer? _host;
        private static MiyabiVideoOverlay? _current;

        /// <summary>当前是否有视频正在播放。</summary>
        public static bool IsPlaying =>
            _current != null && GodotObject.IsInstanceValid(_current) && _current.IsInsideTree();

        /// <summary>
        /// 播放一个全屏视频。同一时间只允许一个视频，新视频会顶掉正在播放的旧视频。
        /// </summary>
        /// <param name="keyOrPath">
        /// 视频文件名（不带扩展名，如 "guang_awaken"）或完整 res:// 路径。
        /// </param>
        /// <param name="onCompleted">播放结束回调，参数 true 表示自然播完，false 表示被跳过/中断。</param>
        /// <param name="pauseGame">播放期间是否暂停游戏（默认 true）。</param>
        /// <param name="cover">true = 铺满全屏并裁切边缘；false = 完整显示并留黑边（默认 true）。</param>
        /// <param name="loop">是否循环播放（循环时只能点击跳过）。</param>
        /// <param name="mute">
        /// true = 视频静音，只播画面，声音自己用 MiyabiAudioPlay 播。
        /// 两者同时开始，音画不同步属于预期。
        /// </param>
        public static void Play(
            string keyOrPath,
            Action<bool>? onCompleted = null,
            bool pauseGame = true,
            bool cover = true,
            bool loop = false,
            bool mute = false)
        {
            // 同一时间只允许一个视频
            Stop();

            string? path = ResolvePath(keyOrPath);
            if (path == null)
            {
                onCompleted?.Invoke(false);
                return;
            }

            VideoStream? stream = LoadStream(path);
            if (stream == null)
            {
                onCompleted?.Invoke(false);
                return;
            }

            CanvasLayer? host = EnsureHost();
            if (host == null)
            {
                GD.PrintErr("[MiyabiVideo] 找不到可用于挂载视频的 SceneTree.Root。");
                onCompleted?.Invoke(false);
                return;
            }

            MiyabiVideoOverlay overlay = new() { Name = "MiyabiVideoOverlay" };
            overlay.Setup(stream, cover, loop, pauseGame, mute);

            // 先登记再 AddChild：_Ready 里如果立即出错关闭，也能正确清理。
            _current = overlay;
            overlay.Completed += finished =>
            {
                if (ReferenceEquals(_current, overlay))
                    _current = null;
                onCompleted?.Invoke(finished);
            };

            host.AddChild(overlay);
        }

        /// <summary>
        /// 播放视频并等待结束。返回 true 表示自然播放完毕，false 表示被跳过/中断。
        /// </summary>
        public static Task<bool> PlayAsync(
            string keyOrPath,
            bool pauseGame = true,
            bool cover = true,
            bool loop = false,
            bool mute = false)
        {
            TaskCompletionSource<bool> tcs = new();
            Play(keyOrPath, result => tcs.TrySetResult(result), pauseGame, cover, loop, mute);
            return tcs.Task;
        }

        /// <summary>
        /// 设置里开启了「播放视频」时才播放，否则什么也不做。
        /// 适合直接塞进卡牌 / 能力流程里，让玩家自行决定是否看演出。
        /// </summary>
        public static void PlayIfEnabled(
            string keyOrPath,
            Action<bool>? onCompleted = null,
            bool pauseGame = true,
            bool cover = true,
            bool loop = false,
            bool mute = false)
        {
            if (!MiyabiModConfig.PlayVideo)
            {
                onCompleted?.Invoke(false);
                return;
            }

            Play(keyOrPath, onCompleted, pauseGame, cover, loop, mute);
        }

        /// <summary>
        /// 设置里开启了「播放视频」时播放并等待结束；否则立即返回 false。
        /// </summary>
        public static async Task<bool> PlayIfEnabledAsync(
            string keyOrPath,
            bool pauseGame = true,
            bool cover = true,
            bool loop = false,
            bool mute = false)
        {
            if (!MiyabiModConfig.PlayVideo)
                return false;

            return await PlayAsync(keyOrPath, pauseGame, cover, loop, mute);
        }

        /// <summary>立即跳过并关闭当前视频；没有视频时什么也不做。</summary>
        public static void Stop()
        {
            MiyabiVideoOverlay? current = _current;
            _current = null;

            if (current != null && GodotObject.IsInstanceValid(current))
                current.RequestStop();
        }

        // ============================================================
        // 资源解析 / 加载
        // ============================================================

        private static string? ResolvePath(string keyOrPath)
        {
            if (string.IsNullOrWhiteSpace(keyOrPath))
                return null;

            string input = keyOrPath.Trim();

            if (input.StartsWith("res://", StringComparison.OrdinalIgnoreCase)
                || input.StartsWith("user://", StringComparison.OrdinalIgnoreCase))
            {
                if (ResourceLoader.Exists(input))
                    return input;

                GD.PrintErr($"[MiyabiVideo] 视频资源不存在: {input}");
                return null;
            }

            EnsureIndex();

            string key = NormalizeKey(input);
            if (VideoPathIndex.TryGetValue(key, out string? indexed) && !string.IsNullOrEmpty(indexed))
                return indexed;

            // 索引没命中时，退化为按默认目录拼路径
            string guess = $"{ModVideoRootPath}{input}";
            if (!guess.EndsWith(".ogv", StringComparison.OrdinalIgnoreCase))
                guess += ".ogv";

            if (ResourceLoader.Exists(guess))
                return guess;

            GD.PrintErr($"[MiyabiVideo] 找不到视频 Key: {keyOrPath}（应放在 {ModVideoRootPath} 下）");
            return null;
        }

        private static VideoStream? LoadStream(string path)
        {
            if (StreamCache.TryGetValue(path, out VideoStream? cached)
                && cached != null
                && GodotObject.IsInstanceValid(cached))
            {
                return cached;
            }

            VideoStream? stream = ResourceLoader.Load<VideoStream>(
                path, "", ResourceLoader.CacheMode.Replace);

            if (stream == null)
            {
                GD.PrintErr($"[MiyabiVideo] 无法加载视频资源: {path}");
                return null;
            }

            StreamCache[path] = stream;
            return stream;
        }

        private static CanvasLayer? EnsureHost()
        {
            if (_host != null && GodotObject.IsInstanceValid(_host) && _host.IsInsideTree())
                return _host;

            SceneTree tree = (SceneTree)Engine.GetMainLoop();
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
            else
            {
                // 保证层号始终在最上面
                _host.Layer = HostCanvasLayer;
                _host.ProcessMode = Node.ProcessModeEnum.Always;
            }

            return _host;
        }

        // ============================================================
        // 目录索引（和 MiyabiAudioService 同一套思路）
        // ============================================================

        private static void EnsureIndex()
        {
            if (_indexed)
                return;

            _indexed = true;
            VideoPathIndex.Clear();
            IndexDirectory(ModVideoRootPath);
            GD.Print($"[MiyabiVideo] 索引了 {VideoPathIndex.Count} 个视频。");
        }

        private static void IndexDirectory(string path)
        {
            using DirAccess? dir = DirAccess.Open(path);
            if (dir == null)
                return;

            dir.ListDirBegin();
            string fileName = dir.GetNext();

            while (!string.IsNullOrEmpty(fileName))
            {
                if (dir.CurrentIsDir())
                {
                    if (fileName != "." && fileName != "..")
                        IndexDirectory($"{path}{fileName}/");
                }
                else
                {
                    string lower = fileName.ToLowerInvariant();

                    // 导出后的 PCK 里可能只有 .ogv.import，也可能只有 .ogv，
                    // 两种都索引，ResourceLoader 会自己处理 remap。
                    if (lower.EndsWith(".ogv") || lower.EndsWith(".ogv.import"))
                    {
                        string realFileName = lower.EndsWith(".ogv.import")
                            ? fileName[..^".import".Length]
                            : fileName;

                        string key = NormalizeKey(realFileName);
                        if (!VideoPathIndex.ContainsKey(key))
                        {
                            VideoPathIndex[key] = $"{path}{realFileName}";
                            GD.Print($"[MiyabiVideo] 成功索引视频: {key} -> {VideoPathIndex[key]}");
                        }
                    }
                }

                fileName = dir.GetNext();
            }
        }

        private static string NormalizeKey(string name)
        {
            string file = name.Trim();

            if (file.EndsWith(".import", StringComparison.OrdinalIgnoreCase))
                file = file[..^".import".Length];

            return Path.GetFileNameWithoutExtension(file).ToLowerInvariant();
        }
    }
}
