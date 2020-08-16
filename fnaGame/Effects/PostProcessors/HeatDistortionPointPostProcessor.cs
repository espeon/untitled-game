using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;

namespace HeatWaveShader.Effects
{
    public class HeatDistortionPointPostProcessor : PostProcessor
    {
        public HeatDistortionPointPostProcessor(int executionOrder, Effect effect = null) : base(executionOrder, effect) { }

        #region Params

        private bool enabled;

        private Vector2 strongPoint;
        private float totalTimeInMs = 1000f;
        private float timerMs = 0f;
        private float frequency = 0.01f;
        private float amplitude = 0.01f;

        EffectParameter strongPointParam;
        EffectParameter maxEffectiveDistanceParam;
        EffectParameter totalTimeInMsParam;
        EffectParameter timerMsParam;
        EffectParameter frequencyParam;
        EffectParameter amplitudeParam;

        public Vector2 StrongPoint
        {
            get { return strongPoint; }
            set
            {
                
                strongPoint = value;
                strongPointParam?.SetValue(value);
            }
        }

        public float TotalTimeInMs
        {
            get { return totalTimeInMs; }
            set
            {
                float val = value;
                if (val < 0f) val = 0f;
                totalTimeInMs = val;
                totalTimeInMsParam?.SetValue(val);
            }
        }

        public float TimerMs
        {
            set
            {
                float val = value;
                timerMs = val;
                timerMsParam?.SetValue(val);
            }
        }
        
        #endregion

        public override void OnAddedToScene(Scene scene)
        {
            base.OnAddedToScene(scene);
            strongPointParam = Effect.Parameters["strongPoint"];
            totalTimeInMsParam = Effect.Parameters["totalTimeMs"];
            timerMsParam = Effect.Parameters["timerMs"];
            frequencyParam = Effect.Parameters["frequency"];
            amplitudeParam = Effect.Parameters["amplitude"];
            this.enabled = false;
        }

        public void play(Vector2 strongPoint, float totalTimeInMs)
        {
            StrongPoint = strongPoint;
            TotalTimeInMs = totalTimeInMs;
            timerMs = 0f;
            frequencyParam?.SetValue(frequency);
            amplitudeParam?.SetValue(amplitude);
            this.enabled = true;
        }

        public void update()
        {
            if (timerMs > totalTimeInMs)
            {
                this.enabled = false;
            }
            else
            {
                TimerMs = timerMs + (Time.DeltaTime * 1000f);
            }
            
        }
    }
}