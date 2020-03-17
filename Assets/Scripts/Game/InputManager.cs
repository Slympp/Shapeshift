using UnityEngine;

namespace Game {
    public static class InputManager {

        public static bool ShapeShiftPrevious => Input.GetKey(KeyCode.A) || 
                                                 Input.GetKey(KeyCode.Q) || 
                                                 Input.GetKey(KeyCode.LeftArrow) || 
                                                 Input.GetMouseButton(0);

        public static bool ShapeShiftNext => Input.GetKey(KeyCode.D) || 
                                             Input.GetKey(KeyCode.E) || 
                                             Input.GetKey(KeyCode.RightArrow) ||
                                             Input.GetMouseButton(1);
        
        public static bool Pause => Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape);
    }
}
