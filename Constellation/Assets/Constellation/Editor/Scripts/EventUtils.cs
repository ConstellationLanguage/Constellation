using System;
using UnityEngine;

    public static class EventUtils {
        private static int[] ValidMouseButton = {
            0, //Left
            1, //Right
            2  //Mousewheel
        };

        public static bool IsLayout (this Event current) {
            return current.type == EventType.Layout;
        }

        public static bool IsRepaint (this Event current) {
            return current.type == EventType.Repaint;
        }

        public static bool IsLayoutOrRepaint (this Event current) {
            return current.IsLayout() || current.IsRepaint();
        }

        public static bool IsUsed (this Event current) {
            return current.type == EventType.Used;
        }

        public static bool MouseDrag (this Event current) {
            return current.type == EventType.MouseDrag;
        }

        public static bool MouseButtonDrag (this Event current, int button) {
            return CheckButton(button) 
                ? (current.MouseDrag() && current.button == button)
                : false;
        }

        public static bool MouseDown (this Event current) {
            return current.type == EventType.MouseDown;
        }

        public static bool MouseButtonDown (this Event current, int button) {
            return CheckButton(button) 
                ? (current.MouseDown() && current.button == button)
                : false;
        }

        public static bool MouseUp (this Event current) {
            return current.type == EventType.MouseUp;
        }

        public static bool MouseButtonUp (this Event current, int button) {
            return CheckButton(button) 
                ? (current.MouseUp() && current.button == button)
                : false;
        }

        private static bool CheckButton (int button) {
            if (Array.IndexOf(ValidMouseButton, button) >= 0)
                return true;

            Debug.LogError(string.Format("Invalid Mouse Button: {0}", button));
            return false;
        }
    }
