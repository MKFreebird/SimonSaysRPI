using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimonSaysRPI
{
    class Simon
    {
        private const int timelimit = 7000; // milliseconds
        private int[] randomCode = new int[10];
        private bool nextRound;
        private int count = 0; // position in randomCode array

        IOcontroller IO = new IOcontroller();
        Random rand = new Random();

        public Simon()
        {
            IO.Init();
        }

        public void Main()
        {
            while (true)
            {
                nextRound = true;
                IO.LedAnimation("idle");

                while (nextRound)
                {
                    ResetUserInput();
                    GenerateRandomCode();
                    IO.UserInput(count, timelimit);
                    ValidateInput();
                }
            }
        }

        public void GenerateRandomCode()
        {
            randomCode[count] = rand.Next(1, 5);
            count++;
            IO.FlashCode(randomCode, count);
        }

        public void ValidateInput()
        {
            bool correctInput = true;
            for (int x = 0; x < count; x++)
            {
                if (IO.userInput[x] != randomCode[x])
                {
                    correctInput = false;
                }
            }
            if (correctInput)
            {
                if (count == randomCode.Length)
                {
                    IO.LedAnimation("win");
                    ResetGame();
                    nextRound = false;
                }
                else
                {
                    IO.LedAnimation("newround");
                }
            }
            else
            {
                IO.LedAnimation("lose");
                ResetGame();
                nextRound = false;
            }
            ResetUserInput();
        }

        public void ResetGame()
        {
            for (int i = 0; i < randomCode.Length ; i++)
            {
                randomCode[i] = 0;
            }
            count = 0;
        }

        public void ResetUserInput()
        {
            for (int i = 0; i < randomCode.Length ; i++)
            {
                IO.userInput[i] = 0;
            }
            IO.buttonCount = 0;
        }
    }
}