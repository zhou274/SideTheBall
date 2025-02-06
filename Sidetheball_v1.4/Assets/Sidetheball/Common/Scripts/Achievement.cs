﻿namespace dotmob {
    public class Achievement
    {
        public string id;
        public string description;
        public string icon;
        public bool isUnlocked = false;
        public string type;

        public int totalStep;
        public int currentStep;

        public Reward reward;
    }
}