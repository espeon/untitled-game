using Microsoft.Xna.Framework;
using Nez.Particles;
using Nez;
using Microsoft.Xna.Framework.Graphics;
using System.IO;


namespace fnaGame.Shared
{
    /// <summary>
    /// Main particle stuff. Mainly used in the playercontroller.
    /// </summary>
    public class Particles : Component
    {
        string[] _particleConfigs;
        // the ParticleEmitter component
        ParticleEmitter _particleEmitter;
        // The currently loaded particle emitter configuration
        ParticleEmitterConfig _particleEmitterConfig;

        public static void LoadParticleSystem(int ParticleNumber, Entity thisEntity, float angle, float angleVariance)
        {
            Particles P = new Particles();
            P._particleConfigs = Directory.GetFiles("Content/Particles", "*.pex");
            Debug.Log(P._particleConfigs[0]);
            // load up the config then add a ParticleEmitter
            P._particleEmitterConfig = Core.Content.LoadParticleEmitterConfig(P._particleConfigs[0]);
            Debug.Log(P._particleEmitterConfig);
            P.ResetEmitter(thisEntity, ParticleNumber, angle, angleVariance);
        }
        public static void Play(float angle, float angleVariance)
        {
            Particles P = new Particles();
            Debug.Log(P._particleEmitterConfig);
            P._particleEmitterConfig.Angle = -angle;
            P._particleEmitterConfig.AngleVariance = angleVariance;
            P._particleEmitter.Play();
        }
        void ResetEmitter(Entity thisEntity, int ParticleNumber, float angle, float angleVariance)
        {
            // kill the ParticleEmitter if we already have one
            if (_particleEmitter != null)
                thisEntity.RemoveComponent(_particleEmitter);

            if(ParticleNumber == 1){
            _particleEmitterConfig.EmissionRate = 10000;
            _particleEmitterConfig.MaxParticles = 16;
            _particleEmitterConfig.BlendFuncSource = Blend.One;
            _particleEmitterConfig.BlendFuncDestination = Blend.One;
            _particleEmitterConfig.ParticleLifespan = 0.3f;
            }

            _particleEmitter = thisEntity.AddComponent(new ParticleEmitter(_particleEmitterConfig));
            _particleEmitter.CollisionConfig.Enabled = true;
            _particleEmitter.CollisionConfig.Elasticity = 0.5f;
            _particleEmitter.CollisionConfig.CollidesWithLayers = 1;
            _particleEmitter.CollisionConfig.Gravity = new Vector2(0, 1300f);
            _particleEmitter.UpdateOrder = 0;
            _particleEmitter.SimulateInWorldSpace = true;
            _particleEmitterConfig.Angle = -angle;
            _particleEmitterConfig.AngleVariance = angleVariance;
            _particleEmitter.Play();
        }
    }
}