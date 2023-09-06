﻿using Dalamud.Game.ClientState.JobGauge;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using DelvCD.Helpers;
using System.Collections.Generic;

namespace DelvCD.Config.JobGaugeDataSources
{
    public class BardJobGaugeDataSource : JobGaugeDataSource
    {
        public BardJobGaugeDataSource(string rawData) : base(rawData)
        {
        }

        public override Job Job => Job.BRD;

        protected override void InitializeConditions()
        {
            _names = new List<string>() {
                "Active Song",
                "Last Active Song",
                "Song Time (milliseconds)",
                "Repertoire Stacks",
                "Soul Voice",
                "Coda"
            };

            _types = new List<TriggerConditionType>() {
                TriggerConditionType.Combo,
                TriggerConditionType.Combo,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Numeric,
                TriggerConditionType.Combo
            };

            string[] songs = new string[] { "None", "Mage's Ballad", "Army's Paeon", "The Wanderer's Minute" };
            _comboOptions = new Dictionary<int, string[]>()
            {
                [0] = songs,
                [1] = songs,
                [5] = songs
            };
        }

        public override bool IsTriggered(bool preview, DataSource data)
        {
            BRDGauge gauge = Singletons.Get<JobGauges>().Get<BRDGauge>();

            data.Value = 0;
            data.MaxValue = 100;

            return
                EvaluateCondition(0, (int)gauge.Song) &&
                EvaluateCondition(1, (int)gauge.LastSong) &&
                EvaluateCondition(2, gauge.SongTimer) &&
                EvaluateCondition(3, gauge.Repertoire) &&
                EvaluateCondition(4, gauge.SoulVoice) &&
                EvaluateCodaCondition(gauge);
        }

        private bool EvaluateCodaCondition(BRDGauge gauge)
        {
            if (!ConditionEnabledforIndex(5)) { return true; }

            int value = _values[5];

            // none?
            if (value == 0)
            {
                return gauge.Coda[0] == Song.NONE && gauge.Coda[1] == Song.NONE && gauge.Coda[2] == Song.NONE;
            }
            else if (value < 4)
            {
                return gauge.Coda[value - 1] != Song.NONE;
            }

            return true;
        }
    }
}
