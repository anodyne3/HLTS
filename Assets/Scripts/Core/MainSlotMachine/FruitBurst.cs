using System;

namespace Core.MainSlotMachine
{
    [Serializable]
    public class FruitBurst
    {
        public float burstTime;
        public int burstAmountMin;
        public int burstAmountMax;
        public int cycles;
        public float burstInterval;

        public FruitBurst(float burstTime, int burstAmountMin, int burstAmountMax, int cycles, float burstInterval)
        {
            this.burstTime = burstTime;
            this.burstAmountMin = burstAmountMin;
            this.burstAmountMax = burstAmountMax;
            this.cycles = cycles;
            this.burstInterval = burstInterval;
        }
    }
}