using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2016
{
    public abstract class Day10Instruction
    {
        public string Text { get; set; }

        public abstract bool Execute(Dictionary<int, Day10Bot> bots);

        protected Day10Bot GetBot(int botNum, bool isBot, Dictionary<int, Day10Bot> bots )
        {
            if (!isBot)
            {
                if (botNum == 0) botNum = -9999;
                else botNum = -botNum;
            }

            if (bots.ContainsKey(botNum))
            {
                return bots[botNum];
            }
            else
            {
                bots[botNum] = isBot ? new Day10Bot(botNum) : new Day10Output(botNum);
                return bots[botNum];
            }
        }
    }

    public class ValueAddInstruction : Day10Instruction
    {
        private int _botNum;
        private int _value;

        public ValueAddInstruction(int botNum, int value)
        {
            _botNum = botNum;
            _value = value;
        }

        public override bool Execute(Dictionary<int, Day10Bot> bots)
        {
            Day10Bot bot = GetBot(_botNum, true, bots);
            bot.AddValue(_value);
            return true;
        }
    }

    public class GiveInstruction : Day10Instruction
    {
        private int _fromBot;
        private int _lowNum;
        private int _highNum;
        private bool _targetLowBot;
        private bool _targetHighBot;

        public GiveInstruction(int fromBot, int lowNum, bool targetLowBot, int highNum, bool targetHighBot)
        {
            _fromBot = fromBot;
            _lowNum = lowNum;
            _highNum = highNum;
            _targetLowBot = targetLowBot;
            _targetHighBot = targetHighBot;
        }

        public override bool Execute(Dictionary<int, Day10Bot> bots)
        {
            Day10Bot lowBot = GetBot(_lowNum, _targetLowBot, bots);
            Day10Bot highBot = GetBot(_highNum, _targetHighBot, bots);
            Day10Bot fromBot = GetBot(_fromBot, true, bots);
            if (fromBot.ChipCount == 2)
            {
                lowBot.AddValue(fromBot.Low);
                highBot.AddValue(fromBot.High);
                fromBot.ClearValues();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
