using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Game {

    public static InputActions Input = new _InputActions();

    public static GameManager Manager;
    public static UIManager UI;






    // Modified controls class to enable on construction
    private class _InputActions : InputActions {
        public _InputActions() : base() {
            Enable();
        }
    }
}
