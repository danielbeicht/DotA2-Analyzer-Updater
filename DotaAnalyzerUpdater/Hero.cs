using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaAnalyzerUpdater
{
    public class Hero
    {
        public string name;
        public string fullName;
        public int index;

        public float[] advantage = new float[Form1.heroCount];
        public float[] winrate = new float[Form1.heroCount];
        public float[] advantage_zeroAsAvg = new float[Form1.heroCount];

        public Hero(string name, string fullName, int index)
        {
            this.name = name;
            this.fullName = fullName;
            this.index = index;
        }

        public void CalculateExtendedAdvantage()
        {
            float advantageAvg = 0.0f;

            for (int i = 0; i < Form1.heroCount; i++)
            {
                advantageAvg += advantage[i];
            }
            advantageAvg /= (Form1.heroCount - 1); //-1, because the hero itself shall not be counted.

            for (int i = 0; i < Form1.heroCount; i++)
            {
                if (i != index)
                {
                    advantage_zeroAsAvg[i] -= advantageAvg;
                }
            }
        }
    }
}
