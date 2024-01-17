using System;
using System.Collections.Generic;

namespace StarForce {
    public class Controller {
        public PuzzleForgeController PuzzleForgeController;

        public void OnInit() {
            PuzzleForgeController = new PuzzleForgeController();
            PuzzleForgeController.OnInit();
        }

        public void Clear() {
            PuzzleForgeController = null;
        }
    }
}