using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI {
    // ReSharper disable once InconsistentNaming
    public class ShapeShiftHUD : MonoBehaviour {

        [SerializeField] private List<RectTransform> shapeIcons;
        
        public IEnumerator Rotate(bool rightDirection, float duration, Action onRotationComplete) {
            var time = 0f;
            var totalDuration = duration * Time.timeScale;
            
            var start = transform.localRotation;
            var target = start * Quaternion.Euler(0, 0, rightDirection ? 120 : -120);
            
            var icons = new List<ShapeIcon>();
            foreach (var s in shapeIcons)
                icons.Add(new ShapeIcon(s, rightDirection));

            while (time <= totalDuration) {
                // Root
                transform.rotation = Quaternion.Lerp(start, target, time / totalDuration);
                
                // Elements
                foreach (var i in icons)
                    i.T.localRotation = Quaternion.Lerp(i.Start, i.End, time / totalDuration);
                
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            
            transform.rotation = target;
            foreach (var i in icons)
                i.T.localRotation = i.End;
            
            onRotationComplete?.Invoke();
        }

        private class ShapeIcon {
            public RectTransform T { get; }
            public Quaternion Start { get; }
            public Quaternion End { get; }
            
            public ShapeIcon(RectTransform t, bool rightDirection) {
                T = t;
                Start = t.localRotation;
                var e = Start.eulerAngles;
                End = Quaternion.Euler(e.x, e.y, e.z + (rightDirection ? -120 : 120));
            }
        }
    }
}