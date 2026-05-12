using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

// 请把这个命名空间改为你自己的 Mod 命名空间
namespace Miyabists2.Scripts.Service
{
    public static class MiyabiAudioService
    {
        // 🛠️ 【修改这里】指向你的音频文件夹路径
        private const string ModAudioRootPath = "res://scenes/audio/";

        // 缓存：防止重复从硬盘读取文件
        private static readonly Dictionary<string, AudioStream> StreamCache = new Dictionary<string, AudioStream>(StringComparer.OrdinalIgnoreCase);
        // 索引：文件名 -> 文件完整路径
        private static readonly Dictionary<string, string> AudioPathToIndex = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private static bool _initialized;
        private static Node? _audioHost;
        private static long _playerCounter;

        // 初始化：遍历文件夹建立索引
        public static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            StreamCache.Clear();
            AudioPathToIndex.Clear();

            IndexDirectory(ModAudioRootPath);
            // 这里用你自己的 Logger，如果没有 Logger 可以把这行删了或者改用 GD.Print
            GD.Print($"[MiyabiAudio] 索引了 {AudioPathToIndex.Count} 个自定义音效。");
        }

        // 核心播放接口：传入文件名（不带后缀）即可
        public static void Play(string key, float linearVolume = 1f)
        {
            if (string.IsNullOrWhiteSpace(key)) return;

            Initialize(); // 确保已初始化

            StopAll();

            string normalizedKey = NormalizeKey(key);

            // 1. 查找路径
            if (!AudioPathToIndex.TryGetValue(normalizedKey, out string? path) || path == null)
            {
                GD.PrintErr($"[MiyabiAudio] 找不到音效 Key: {key}");
                return;
            }

            // 2. 获取音效流（使用缓存）
            AudioStream? stream = GetOrLoadStream(path);
            if (stream == null) return;

            // 3. 获取宿主节点
            Node? host = EnsureHostNode();
            if (host == null) return;

            // 4. 创建临时的播放器
            AudioStreamPlayer player = new AudioStreamPlayer
            {
                // ✨ 修正：Godot 4 标准的 StringName 创建方式
                Name = new StringName($"MiyabiSfxPlayer_{++_playerCounter}"),
                Stream = stream,
                VolumeDb = LinearToDb(linearVolume),
                // ✨ 修正：Godot 4 的 ProcessMode
                ProcessMode = Node.ProcessModeEnum.Always
            };

            host.AddChild(player);

            // 💖 核心白嫖：播放完自动删除自己，清理内存
            player.Finished += () => player.QueueFree();

            // 5. 播放
            player.Play();
        }

        private static string NormalizeKey(string key)
        {
            // ✨ 修正：使用 Godot 的 PathUtil 或 System.IO 规范化
            return Path.GetFileNameWithoutExtension(key.Trim()).ToLowerInvariant();
        }

        private static AudioStream? GetOrLoadStream(string path)
        {
            if (StreamCache.TryGetValue(path, out AudioStream existing))
            {
                return existing;
            }

            // 使用 CacheMode.Replace 确保加载最新资源
            var loadedStream = ResourceLoader.Load<AudioStream>(path, "", ResourceLoader.CacheMode.Replace);
            if (loadedStream != null)
            {
                StreamCache[path] = loadedStream;
            }
            else
            {
                GD.PrintErr($"[MiyabiAudio] 无法加载音频资源: {path}");
            }
            return loadedStream;
        }

        private static Node? EnsureHostNode()
        {
            if (_audioHost != null && GodotObject.IsInstanceValid(_audioHost) && _audioHost.IsInsideTree())
            {
                return _audioHost;
            }

            // ✨ 修正：Godot 4 获取 SceneTree 的标准方式
            SceneTree tree = (SceneTree)Engine.GetMainLoop();
            if (tree?.Root == null) return null;

            // 尝试查找现有的宿主
            const string hostName = "MiyabiAudioHost";
            _audioHost = tree.Root.GetNodeOrNull<Node>(hostName);

            if (_audioHost == null)
            {
                // 如果没有，创建一个
                _audioHost = new Node
                {
                    Name = hostName,
                    ProcessMode = Node.ProcessModeEnum.Always
                };
                tree.Root.AddChild(_audioHost);
            }

            return _audioHost;
        }

        private static void IndexDirectory(string path)
        {
            using var dir = DirAccess.Open(path);
            if (dir == null) return;

            dir.ListDirBegin();
            string fileName = dir.GetNext();
            while (fileName != "")
            {
                if (dir.CurrentIsDir())
                {
                    if (fileName != "." && fileName != "..")
                        IndexDirectory($"{path}/{fileName}");
                }
                else
                {
                    // 💡 关键修改：检查是否是 .import 文件，或者是原始音频格式
                    string lowerName = fileName.ToLowerInvariant();
                    if (lowerName.EndsWith(".mp3.import") || lowerName.EndsWith(".wav.import") || lowerName.EndsWith(".ogg.import") ||
                        lowerName.EndsWith(".mp3") || lowerName.EndsWith(".wav") || lowerName.EndsWith(".ogg"))
                    {
                        // 还原出原始文件名（去掉 .import）
                        string realFileName = fileName.EndsWith(".import") ? fileName.Replace(".import", "") : fileName;

                        string key = NormalizeKey(realFileName);
                        string fullPath = $"{path}/{realFileName}"; // 映射到原始资源的 res:// 路径

                        if (!AudioPathToIndex.ContainsKey(key))
                        {
                            AudioPathToIndex[key] = fullPath;
                            GD.Print($"[MiyabiAudio] 成功索引资源: {key} -> {fullPath}");
                        }
                    }
                }
                fileName = dir.GetNext();
            }
        }

        private static readonly List<AudioStreamPlayer> ActivePlayers = new List<AudioStreamPlayer>();
        public static void StopAll()
        {
            lock (ActivePlayers)
            {
                foreach (var player in ActivePlayers)
                {
                    if (GodotObject.IsInstanceValid(player))
                    {
                        player.Stop();
                        player.QueueFree();
                    }
                }
                ActivePlayers.Clear();
            }
            GD.Print("[MiyabiAudio] 所有正在播放的语音已停止并清理。");
        }

        //private static void IndexDirectory(string path)
        //{
        //    // ✨ 修正：Godot 4 的 DirAccess
        //    using var dir = DirAccess.Open(path);
        //    if (dir == null)
        //    {
        //        GD.PrintErr($"[MiyabiAudio] 找不到音频目录: {path}");
        //        return;
        //    }

        //    dir.ListDirBegin();
        //    string fileName = dir.GetNext();
        //    while (fileName != "")
        //    {
        //        GD.PrintErr($"[MiyabiAudio] 当前文件: {fileName}");
        //        if (dir.CurrentIsDir())
        //        {
        //            if (fileName != "." && fileName != "..")
        //            {
        //                IndexDirectory($"{path}/{fileName}"); // 递归搜索子文件夹
        //            }
        //        }
        //        else
        //        {
        //            // 判断后缀，目前 Godot 处理短音效建议用 .wav，长音效可以用 .ogg
        //            string ext = Path.GetExtension(fileName).ToLowerInvariant();
        //            if (ext == ".wav" || ext == ".ogg" || ext == ".mp3")
        //            {
        //                string key = NormalizeKey(fileName);
        //                string fullPath = $"{path}/{fileName}";
        //                if (!AudioPathToIndex.ContainsKey(key))
        //                {
        //                    AudioPathToIndex[key] = fullPath;
        //                }
        //            }
        //        }
        //        fileName = dir.GetNext();
        //    }
        //}

        private static float LinearToDb(float linearVolume)
        {
            return linearVolume <= 0f ? -80f : Mathf.LinearToDb(Mathf.Max(linearVolume, 0.0001f));
        }
    }
}