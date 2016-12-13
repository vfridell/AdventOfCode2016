using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2016
{
    public class Day10Bot
    {
        public Day10Bot(int botNum)
        {
            _botNum = botNum;
        }

        protected int _botNum;

        protected List<int> values  = new List<int>();

        public virtual void AddValue(int value)
        {
            if(values.Count >= 2) throw new Exception("cannot hold more than two chips");
            values.Add(value);
            if (values.Contains(17) && values.Contains(61))
            {
                Console.WriteLine($"Bot {_botNum} is responsible for 17 and 61");
            }
            if (values.Contains(5) && values.Contains(2))
            {
                Console.WriteLine($"Bot {_botNum} is responsible for 5 and 2");
            }
        }

        public int Low => values.Min();
        public int High => values.Max();

        public int ChipCount => values.Count;

        public void ClearValues()
        {
            values.Clear();
        }
    }

    public class Day10Output : Day10Bot
    {
        public Day10Output(int botNum)
            : base(botNum)
        {
            
        }

        public override void AddValue(int value)
        {
            values.Add(value);
        }
    }
}
