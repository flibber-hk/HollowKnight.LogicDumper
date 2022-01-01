using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Modding;
using RandomizerCore.Logic;
using RandomizerMod.Settings;
using UnityEngine;

namespace LogicDumper
{
    public class LogicDumper : Mod
    {
        internal static LogicDumper instance;
        
        public LogicDumper() : base(null)
        {
            instance = this;
        }
        
        public override string GetVersion()
        {
            return GetType().Assembly.GetName().Version.ToString();;
        }
        
        public override void Initialize()
        {
            Log("Initializing Mod...");

            RandomizerMod.RC.RCData.RuntimeLogicOverride.Subscribe(float.MinValue, GetLogicDumper("RawLogicBeforeConnections"));
            ModHooks.FinishedLoadingModsHook += () => RandomizerMod.RC.RCData.RuntimeLogicOverride.Subscribe(float.MaxValue, GetLogicDumper("RawLogicAfterConnections"));
        }

        private Action<GenerationSettings, LogicManagerBuilder> GetLogicDumper(string filename)
        {
            void DumpLogic(GenerationSettings _, LogicManagerBuilder lmb)
            {
                JsonSerializer js = new()
                {
                    DefaultValueHandling = DefaultValueHandling.Include,
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Auto,
                };

                js.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

                using StreamWriter sw = new(Path.Combine(Path.GetDirectoryName(typeof(LogicDumper).Assembly.Location), $"{filename}.json"));
                {
                    js.Serialize(sw, new LogicManager(lmb));
                }
            }

            return DumpLogic;
        }
    }
}