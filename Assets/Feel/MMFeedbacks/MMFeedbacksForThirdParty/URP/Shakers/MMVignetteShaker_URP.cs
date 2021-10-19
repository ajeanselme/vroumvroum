﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// Add this class to a Camera with a URP vignette post processing and it'll be able to "shake" its values by getting events
    /// </summary>
    [RequireComponent(typeof(Volume))]
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMVignetteShaker_URP")]
    public class MMVignetteShaker_URP : MMShaker
    {
        [Header("Intensity")]
        /// whether or not to add to the initial value
        [Tooltip("TODO")]
        public bool RelativeIntensity = false;
        /// the curve used to animate the intensity value on
        [Tooltip("TODO")]
        public AnimationCurve ShakeIntensity = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the value to remap the curve's 0 to
        [Tooltip("TODO")]
        [Range(0f, 1f)]
        public float RemapIntensityZero = 0f;
        /// the value to remap the curve's 1 to
        [Tooltip("TODO")]
        [Range(0f, 1f)]
        public float RemapIntensityOne = 1f;

        protected Volume _volume;
        protected Vignette _vignette;
        protected float _initialIntensity;
        protected float _originalShakeDuration;
        protected AnimationCurve _originalShakeIntensity;
        protected float _originalRemapIntensityZero;
        protected float _originalRemapIntensityOne;
        protected bool _originalRelativeIntensity;

        /// <summary>
        /// On init we initialize our values
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            _volume = this.gameObject.GetComponent<Volume>();
            _volume.profile.TryGet(out _vignette);
        }

        /// <summary>
        /// Shakes values over time
        /// </summary>
        protected override void Shake()
        {
            float newValue = ShakeFloat(ShakeIntensity, RemapIntensityZero, RemapIntensityOne, RelativeIntensity, _initialIntensity);
            _vignette.intensity.Override(newValue);
        }

        /// <summary>
        /// Collects initial values on the target
        /// </summary>
        protected override void GrabInitialValues()
        {
            _initialIntensity = _vignette.intensity.value;
        }

        /// <summary>
        /// When we get the appropriate event, we trigger a shake
        /// </summary>
        /// <param name="intensity"></param>
        /// <param name="duration"></param>
        /// <param name="amplitude"></param>
        /// <param name="relativeIntensity"></param>
        /// <param name="attenuation"></param>
        /// <param name="channel"></param>
        public virtual void OnVignetteShakeEvent(AnimationCurve intensity, float duration, float remapMin, float remapMax, bool relativeIntensity = false,
            float attenuation = 1.0f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true)
        {
            if (!CheckEventAllowed(channel) || (!Interruptible && Shaking))
            {
                return;
            }
            
            _resetShakerValuesAfterShake = resetShakerValuesAfterShake;
            _resetTargetValuesAfterShake = resetTargetValuesAfterShake;

            if (resetShakerValuesAfterShake)
            {
                _originalShakeDuration = ShakeDuration;
                _originalShakeIntensity = ShakeIntensity;
                _originalRemapIntensityZero = RemapIntensityZero;
                _originalRemapIntensityOne = RemapIntensityOne;
                _originalRelativeIntensity = RelativeIntensity;
            }

            ShakeDuration = duration;
            ShakeIntensity = intensity;
            RemapIntensityZero = remapMin * attenuation;
            RemapIntensityOne = remapMax * attenuation;
            RelativeIntensity = relativeIntensity;
            ForwardDirection = forwardDirection;

            Play();
        }

        /// <summary>
        /// Resets the target's values
        /// </summary>
        protected override void ResetTargetValues()
        {
            base.ResetTargetValues();
            _vignette.intensity.Override(_initialIntensity);
        }

        /// <summary>
        /// Resets the shaker's values
        /// </summary>
        protected override void ResetShakerValues()
        {
            base.ResetShakerValues();
            ShakeDuration = _originalShakeDuration;
            ShakeIntensity = _originalShakeIntensity;
            RemapIntensityZero = _originalRemapIntensityZero;
            RemapIntensityOne = _originalRemapIntensityOne;
            RelativeIntensity = _originalRelativeIntensity;
        }

        /// <summary>
        /// Starts listening for events
        /// </summary>
        public override void StartListening()
        {
            base.StartListening();
            MMVignetteShakeEvent_URP.Register(OnVignetteShakeEvent);
        }

        /// <summary>
        /// Stops listening for events
        /// </summary>
        public override void StopListening()
        {
            base.StopListening();
            MMVignetteShakeEvent_URP.Unregister(OnVignetteShakeEvent);
        }
    }

    /// <summary>
    /// An event used to trigger vignette shakes
    /// </summary>
    public struct MMVignetteShakeEvent_URP
    {
        public delegate void Delegate(AnimationCurve intensity, float duration, float remapMin, float remapMax, bool relativeIntensity = false,
            float attenuation = 1.0f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true);
        static private event Delegate OnEvent;
        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }
        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }
        static public void Trigger(AnimationCurve intensity, float duration, float remapMin, float remapMax, bool relativeIntensity = false,
            float attenuation = 1.0f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true)
        {
            OnEvent?.Invoke(intensity, duration, remapMin, remapMax, relativeIntensity, attenuation, channel, resetShakerValuesAfterShake, resetTargetValuesAfterShake, forwardDirection);
        }
    }
}
