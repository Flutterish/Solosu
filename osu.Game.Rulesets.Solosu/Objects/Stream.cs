﻿using osu.Game.Rulesets.Objects.Types;

namespace osu.Game.Rulesets.Solosu.Objects {
	public class Stream : SolosuHitObject, IHasDuration {
		public double EndTime { get; set; }

		public double Duration {
			get => EndTime - StartTime;
			set => EndTime = StartTime + value;
		}
	}
}