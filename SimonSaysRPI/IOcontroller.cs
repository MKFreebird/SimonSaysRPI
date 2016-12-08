using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace SimonSaysRPI
{
    class IOcontroller
    {
        private int[] ledPins = new int[] { 5, 6, 13, 26 }; // red, yellow, blue, green
        private int[] buttonPins = new int[] { 23, 24, 25, 8 };
        private const int buzzerPin = 21;
        public int[] userInput = new int[10];
        public int buttonCount;

        private GpioController gpio;
        private GpioPin gpioPinRed;
        private GpioPin gpioPinYellow;
        private GpioPin gpioPinBlue;
        private GpioPin gpioPinGreen;
        private GpioPin gpioPinButtonRed;
        private GpioPin gpioPinButtonYellow;
        private GpioPin gpioPinButtonBlue;
        private GpioPin gpioPinButtonGreen;
        private GpioPin gpioBuzzerPin;

        Stopwatch stopwatch = new Stopwatch();

        public void Init()
        {
            gpio = GpioController.GetDefault();
            // initialiseer LEDs. Drivemode set to output
            gpioPinRed = gpio.OpenPin(ledPins[0]);
            gpioPinRed.SetDriveMode(GpioPinDriveMode.Output);

            gpioPinYellow = gpio.OpenPin(ledPins[1]);
            gpioPinYellow.SetDriveMode(GpioPinDriveMode.Output);

            gpioPinBlue = gpio.OpenPin(ledPins[2]);
            gpioPinBlue.SetDriveMode(GpioPinDriveMode.Output);

            gpioPinGreen = gpio.OpenPin(ledPins[3]);
            gpioPinGreen.SetDriveMode(GpioPinDriveMode.Output);

            // initialiseer buttons. Drivemode set to inputpullup
            gpioPinButtonRed = gpio.OpenPin(buttonPins[0]);
            gpioPinButtonRed.SetDriveMode(GpioPinDriveMode.InputPullUp);
            gpioPinButtonRed.DebounceTimeout = TimeSpan.FromMilliseconds(50);

            gpioPinButtonYellow = gpio.OpenPin(buttonPins[1]);
            gpioPinButtonYellow.SetDriveMode(GpioPinDriveMode.InputPullUp);
            gpioPinButtonYellow.DebounceTimeout = TimeSpan.FromMilliseconds(50);

            gpioPinButtonBlue = gpio.OpenPin(buttonPins[2]);
            gpioPinButtonBlue.SetDriveMode(GpioPinDriveMode.InputPullUp);
            gpioPinButtonBlue.DebounceTimeout = TimeSpan.FromMilliseconds(50);

            gpioPinButtonGreen = gpio.OpenPin(buttonPins[3]);
            gpioPinButtonGreen.SetDriveMode(GpioPinDriveMode.InputPullUp);
            gpioPinButtonGreen.DebounceTimeout = TimeSpan.FromMilliseconds(50);

            // initialise buzzer. Drivemode set to output
            gpioBuzzerPin = gpio.OpenPin(buzzerPin);
            gpioBuzzerPin.SetDriveMode(GpioPinDriveMode.Output);
            gpioBuzzerPin.Write(GpioPinValue.Low);

            gpioPinButtonRed.ValueChanged += gpioPinButton_ValueChanged;
            gpioPinButtonYellow.ValueChanged += gpioPinButton_ValueChanged;
            gpioPinButtonBlue.ValueChanged += gpioPinButton_ValueChanged;
            gpioPinButtonGreen.ValueChanged += gpioPinButton_ValueChanged;
        }

        public void gpioPinButton_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.FallingEdge && buttonCount <= userInput.Length - 1)
            {
                if (sender == gpioPinButtonRed)
                {
                    userInput[buttonCount] = 1;
                    gpioPinRed.Write(GpioPinValue.Low);
                    Task.Delay(100).Wait();
                    gpioPinRed.Write(GpioPinValue.High);
                }
                else if (sender == gpioPinButtonYellow)
                {
                    userInput[buttonCount] = 2;
                    gpioPinYellow.Write(GpioPinValue.Low);
                    Task.Delay(100).Wait();
                    gpioPinYellow.Write(GpioPinValue.High);
                }
                else if (sender == gpioPinButtonBlue)
                {
                    userInput[buttonCount] = 3;
                    gpioPinBlue.Write(GpioPinValue.Low);
                    Task.Delay(100).Wait();
                    gpioPinBlue.Write(GpioPinValue.High);
                }
                else if (sender == gpioPinButtonGreen)
                {
                    userInput[buttonCount] = 4;
                    gpioPinGreen.Write(GpioPinValue.Low);
                    Task.Delay(100).Wait();
                    gpioPinGreen.Write(GpioPinValue.High);
                }
                buttonCount++;
            }
        }

        public void FlashCode(int[] arr, int _count)
        {
            for (int i = 0; i < _count; i++)
            {
                int value = arr[i];
                switch (value)
                {
                    case 1:
                        gpioPinRed.Write(GpioPinValue.Low);
                        Task.Delay(500).Wait();
                        gpioPinRed.Write(GpioPinValue.High);
                        Task.Delay(250).Wait();
                        break;
                    case 2:
                        gpioPinYellow.Write(GpioPinValue.Low);
                        Task.Delay(500).Wait();
                        gpioPinYellow.Write(GpioPinValue.High);
                        Task.Delay(250).Wait();
                        break;
                    case 3:
                        gpioPinBlue.Write(GpioPinValue.Low);
                        Task.Delay(500).Wait();
                        gpioPinBlue.Write(GpioPinValue.High);
                        Task.Delay(250).Wait();
                        break;
                    case 4:
                        gpioPinGreen.Write(GpioPinValue.Low);
                        Task.Delay(500).Wait();
                        gpioPinGreen.Write(GpioPinValue.High);
                        Task.Delay(250).Wait();
                        break;
                }
            }
        }

        public void UserInput(int _count, int _timelimit)
        {
            int lastSeconds = 2000;
            bool timeLeft = true;
            stopwatch.Start();
            while (buttonCount < _count && timeLeft == true)
            {
                if (stopwatch.ElapsedMilliseconds == _timelimit-lastSeconds)
                {
                    gpioBuzzerPin.Write(GpioPinValue.High);
                    Task.Delay(75).Wait();
                    gpioBuzzerPin.Write(GpioPinValue.Low);
                    lastSeconds -= 1000;
                }
                if (stopwatch.ElapsedMilliseconds >= _timelimit)
                {
                    timeLeft = false;                  
                }
            }
            stopwatch.Stop();
            stopwatch.Reset();
        }

        public void LedAnimation(string gamestate)
        {
            switch (gamestate)
            {
                case "newgame":
                    NewGameAnimation();
                    break;
                case "win":
                    WinAnimation();
                    break;
                case "lose":
                    LoseAnimation();
                    break;
                case "newround":
                    NewroundAnimation();
                    break;
                case "idle":
                    IdleAnimation();
                    break;
            }
        }

        private void IdleAnimation()
        {
            bool idle = true;
            bool tempoFaster = true;
            int delay = 200;
            while (idle)
            {
                gpioPinRed.Write(GpioPinValue.Low);
                Task.Delay(delay).Wait();
                gpioPinRed.Write(GpioPinValue.High);
                gpioPinYellow.Write(GpioPinValue.Low);
                Task.Delay(delay).Wait();
                gpioPinYellow.Write(GpioPinValue.High);
                gpioPinBlue.Write(GpioPinValue.Low);
                Task.Delay(delay).Wait();
                gpioPinBlue.Write(GpioPinValue.High);
                gpioPinGreen.Write(GpioPinValue.Low);
                Task.Delay(delay).Wait();
                gpioPinGreen.Write(GpioPinValue.High);
                gpioPinBlue.Write(GpioPinValue.Low);
                Task.Delay(delay).Wait();
                gpioPinBlue.Write(GpioPinValue.High);
                gpioPinYellow.Write(GpioPinValue.Low);
                Task.Delay(delay).Wait();
                gpioPinYellow.Write(GpioPinValue.High);
                if (tempoFaster)
                {
                    delay -= 5;
                    if (delay < 15)
                    {
                        tempoFaster = false;
                    }
                }
                else
                {
                    delay += 5;
                    if (delay > 200)
                    {
                        tempoFaster = true;
                    }
                }
                if (userInput[0] != 0)
                {
                    idle = false;
                }
            }
            LedAnimation("newgame");
        }

        private void WinAnimation()
        {
            Task.Delay(500).Wait();
            for (int i = 200; i > 0; i -= 10)
            {
                gpioPinGreen.Write(GpioPinValue.Low);
                Task.Delay(i).Wait();
                gpioPinGreen.Write(GpioPinValue.High);
                Task.Delay(i).Wait();
            }
        }

        private void LoseAnimation()
        {
            gpioBuzzerPin.Write(GpioPinValue.High);
            for (int i = 200; i > 0; i -= 10)
            {
                if (i==190)
                {
                    gpioBuzzerPin.Write(GpioPinValue.Low);
                }
                gpioPinRed.Write(GpioPinValue.Low);
                Task.Delay(i).Wait();
                gpioPinRed.Write(GpioPinValue.High);
                Task.Delay(i).Wait();
            }
        }

        private void NewroundAnimation()
        {
            Task.Delay(500).Wait();
            for (int x = 0; x < 3; x++)
            {
                gpioPinRed.Write(GpioPinValue.Low);
                gpioPinYellow.Write(GpioPinValue.Low);
                gpioPinBlue.Write(GpioPinValue.Low);
                gpioPinGreen.Write(GpioPinValue.Low);
                Task.Delay(200).Wait();
                gpioPinRed.Write(GpioPinValue.High);
                gpioPinYellow.Write(GpioPinValue.High);
                gpioPinBlue.Write(GpioPinValue.High);
                gpioPinGreen.Write(GpioPinValue.High);
                Task.Delay(50).Wait();
            }
            Task.Delay(300).Wait();
        }

        private void NewGameAnimation()
        {
            int delay = 1000;
            gpioPinRed.Write(GpioPinValue.Low);
            gpioPinYellow.Write(GpioPinValue.Low);
            gpioPinBlue.Write(GpioPinValue.Low);
            gpioPinGreen.Write(GpioPinValue.Low);
            Task.Delay(delay).Wait();
            gpioPinRed.Write(GpioPinValue.High);
            Task.Delay(delay).Wait();
            gpioPinYellow.Write(GpioPinValue.High);
            Task.Delay(delay).Wait();
            gpioPinBlue.Write(GpioPinValue.High);
            Task.Delay(delay).Wait();
            gpioPinGreen.Write(GpioPinValue.High);
            Task.Delay(1500).Wait();
        }
    }
}