using System;
using System.Collections.Generic;
using System.IO;
using FinchAPI;
using System.Linq;

namespace Project_FinchControl
{

    // **************************************************
    //
    // Title: Finch Control - Menu Starter
    // Description: Starter solution with the helper methods,
    //              opening and closing screens, and the menu
    // Application Type: Console
    // Author: Fink, Kyle
    // Dated Created: 2/26/2020
    // Last Modified: 3/6/2020
    //
    // **************************************************

    class Program
    {
        /// <summary>
        /// first method run when the app starts up
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            SetTheme();

            DisplayWelcomeScreen();
            DisplayMenuScreen();
            DisplayClosingScreen();
        }

        /// <summary>
        /// setup the console theme
        /// </summary>
        static void SetTheme()
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.BackgroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// *****************************************************************
        /// *                     Main Menu                                 *
        /// *****************************************************************
        /// </summary>
        static void DisplayMenuScreen()
        {
            Console.CursorVisible = true;

            bool quitApplication = false;
            string menuChoice;

            Finch finchRobot = new Finch();

            do
            {
                DisplayScreenHeader("Main Menu");

                //
                // get user menu choice
                //
                Console.WriteLine("\ta) Connect Finch Robot");
                Console.WriteLine("\tb) Talent Show");
                Console.WriteLine("\tc) Data Recorder");
                Console.WriteLine("\td) Alarm System");
                Console.WriteLine("\te) User Programming");
                Console.WriteLine("\tf) Disconnect Finch Robot");
                Console.WriteLine("\tq) Quit");
                Console.Write("\t\tEnter Choice:");
                menuChoice = Console.ReadLine().ToLower();

                //
                // process user menu choice
                //
                switch (menuChoice)
                {
                    case "a":
                        DisplayConnectFinchRobot(finchRobot);
                        break;

                    case "b":
                        DisplayTalentShowMenuScreen(finchRobot);
                        break;

                    case "c":
                        DataRecorderMenuScreen(finchRobot);
                        break;

                    case "d":
                        LightAlarmDisplayMenuScreen(finchRobot);
                        break;

                    case "e":

                        break;

                    case "f":
                        DisplayDisconnectFinchRobot(finchRobot);
                        break;

                    case "q":
                        DisplayDisconnectFinchRobot(finchRobot);
                        quitApplication = true;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("\tPlease enter a letter for the menu choice.");
                        DisplayContinuePrompt();
                        break;
                }
            } while (!quitApplication);
        }

        #region ALARM SYSTEM

        static void LightAlarmDisplayMenuScreen(Finch finchRobot)
        {
            Console.CursorVisible = true;

            string sensorsToMonitor = "";
            string rangeType = "";
            int minMaxThresholdValue = 0;
            int timetoMonitor = 0;

            bool quitDataRecorderMenu = false;
            string menuChoice;

            do
            {
                DisplayScreenHeader("Data Recorder Menu");

                //
                // get user menu choice
                //
                Console.WriteLine("\ta) set sensors to monitor");
                Console.WriteLine("\tb) set range type");
                Console.WriteLine("\tc) set Maximum/Minimum Threshold Value");
                Console.WriteLine("\td) Set time to monitor");
                Console.WriteLine("\te) Set Alarm");
                Console.WriteLine("\tq) Return to Main Menu");
                Console.Write("\t\tEnter Choice:");
                menuChoice = Console.ReadLine().ToLower();

                //
                // process user menu choice
                //
                switch (menuChoice)
                {
                    case "a":
                        sensorsToMonitor = LightAlarmDisplaySetSensorsToMonitor();
                        break;

                    case "b":
                        rangeType = LightAlarmDisplaySetRangeType();
                        break;

                    case "c":
                        minMaxThresholdValue = LightAlarmDisplaySetMinMaxThresholdValue(rangeType, finchRobot);
                        break;

                    case "d":
                        timetoMonitor = LightAlarmDisplaySetMaximumTimeToMonitor(rangeType, finchRobot);
                        break;
                    case "e":
                        LightAlarmDisplaySetAlarm(finchRobot, sensorsToMonitor, rangeType, minMaxThresholdValue, timetoMonitor);
                        break;

                    case "q":
                        quitDataRecorderMenu = true;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("\tPlease enter a letter for the menu choice.");
                        DisplayContinuePrompt();
                        break;
                }

            } while (!quitDataRecorderMenu);
        }

        static void LightAlarmDisplaySetAlarm(
            Finch finchRobot,
            string sensorsToMonitor,
            string rangeType,
            int minMaxThresholdValue,
            int timetoMonitor)
        {
            bool thresholdExceeded = false;

            DisplayScreenHeader("Set Alarm");

            Console.WriteLine($"\tSensor(s) to monitor: {sensorsToMonitor}");
            Console.WriteLine($"\tRange Type: {rangeType}");
            Console.WriteLine($"\t{rangeType} Threshold Value: {minMaxThresholdValue}");
            Console.WriteLine($"\tTime to Monitor: {timetoMonitor}");
            Console.WriteLine();

            Console.WriteLine("Press any key to begin monitoring.");
            Console.ReadKey();

            thresholdExceeded = LightAlarmMonitorLightSensors(finchRobot, sensorsToMonitor, rangeType, minMaxThresholdValue, timetoMonitor);          

            if (thresholdExceeded)
            {
                Console.WriteLine($"The {rangeType} threshold value of {minMaxThresholdValue} was exceeded.");
            }
            else
            {
                Console.WriteLine("The treshold value was not exceeded.");
            }


            DisplayMenuPrompt("Light Alarm");
        }

        static void LightAlarmDisplayElapsedTime(int elapsedTime) 
        {
            Console.SetCursorPosition(15, 10);
            Console.WriteLine($"Elapsed Time: {elapsedTime}");
        }

        static bool LightAlarmMonitorLightSensors(Finch finchRobot, string sensorsToMonitor, string  rangeType, int minMaxThresholdValue, int timetoMonitor) 
        {
            bool thresholdExceeded = false;
            int elapsedTime = 0;
            int currentLightSensorValue = 0;

            while (!thresholdExceeded && elapsedTime < timetoMonitor)
            {
                currentLightSensorValue = LightAlarmGetCurrentLightSensorValue(finchRobot, sensorsToMonitor);

                switch (rangeType)
                {
                    case "minimum":
                        if (currentLightSensorValue < minMaxThresholdValue)
                        {
                            thresholdExceeded = true;
                        }
                        break;
                    case "maximum":
                        if (currentLightSensorValue > minMaxThresholdValue)
                        {
                            thresholdExceeded = true;
                        }
                        break;
                }
                finchRobot.wait(1000);
                elapsedTime++;
                LightAlarmDisplayElapsedTime(elapsedTime);
            }
                return thresholdExceeded;
        }

        static int LightAlarmGetCurrentLightSensorValue(Finch finchRobot, string sensorsToMonitor) 
        {
            int currentLightSensorValue = 0;
            switch (sensorsToMonitor)
            {
                case "left":
                    currentLightSensorValue = finchRobot.getLeftLightSensor();
                    break;
                case "right":
                    currentLightSensorValue = finchRobot.getRightLightSensor();
                    break;
                case "both":
                    currentLightSensorValue = (finchRobot.getLeftLightSensor() + finchRobot.getRightLightSensor()) / 2;
                    currentLightSensorValue = (int)finchRobot.getLightSensors().Average();
                    break;
            }
            return currentLightSensorValue;
        }

        static string LightAlarmDisplaySetSensorsToMonitor()
        {
            string sensorsToMonitor;

            DisplayScreenHeader("Sensors To Monitor");

            Console.WriteLine("Sensors to monitor:");
            sensorsToMonitor = Console.ReadLine();

            DisplayContinuePrompt();

            return sensorsToMonitor;
        }

        static string LightAlarmDisplaySetRangeType()
        {
            string rangeType;

            DisplayScreenHeader("Range Type");

            Console.WriteLine("Range Type:");
            rangeType = Console.ReadLine();

            DisplayContinuePrompt();

            return rangeType;
        }

        static int LightAlarmDisplaySetMinMaxThresholdValue(string rangeType, Finch finchRobot) 
        {
            int minMaxThresholdValue;

            // validation
            bool validResponse;

            do
            {

                DisplayScreenHeader("Min/Max Threshold Value");

                Console.WriteLine($"Current Left light sensor: {finchRobot.getLeftLightSensor()}");
                Console.WriteLine($"Current right light sensor value: {finchRobot.getRightLightSensor()}");
                Console.WriteLine();

                Console.Write($"{rangeType} light sensor value: ");
                validResponse = int.TryParse(Console.ReadLine(), out minMaxThresholdValue);

                if (!validResponse)
                {
                    Console.WriteLine();
                    Console.WriteLine("Please enter an interger.");
                    DisplayContinuePrompt();
                }

            } while (!validResponse);

            //echo value back to user

            DisplayContinuePrompt();

            return minMaxThresholdValue;
        }

        static int LightAlarmDisplaySetMaximumTimeToMonitor(string rangeType, Finch finchRobot)
        {
            int timeToMonitor;

            // validation
            bool validResponse;

            do
            {

                DisplayScreenHeader("Time to Monitor");

                Console.WriteLine($"Time to Monitor:");
                Console.WriteLine("Time to Monitor [seconds]");
                Console.WriteLine();

                Console.Write($"{rangeType} light sensor value: ");
                validResponse = int.TryParse(Console.ReadLine(), out timeToMonitor);

            } while (!validResponse);

            //echo value back to user

            DisplayContinuePrompt();

            return timeToMonitor;
        }

        #endregion

        #region DATA RECORDER

        static void DataRecorderMenuScreen(Finch myFinch) 
        {
            Console.CursorVisible = true;

            int numberOfPoints = 0;
            double dataPointFrequency = 0;
            double[] temperatures = null;

            bool quitDataRecorderMenu = false;
            string menuChoice;

            do
            {
                DisplayScreenHeader("Data Recorder Menu");

                //
                // get user menu choice
                //
                Console.WriteLine("\ta) Number of Data points");
                Console.WriteLine("\tb) Frequency of Data Points");
                Console.WriteLine("\tc) Get Data");
                Console.WriteLine("\td) Show Data");
                Console.WriteLine("\tq) Return to Main Menu");
                Console.Write("\t\tEnter Choice:");
                menuChoice = Console.ReadLine().ToLower();

                //
                // process user menu choice
                //
                switch (menuChoice)
                {
                    case "a":
                        numberOfPoints = DataRecorderDisplayGetNumberOfDataPoints();
                        break;

                    case "b":
                        dataPointFrequency = DataRecorderDisplayGetDataPointFrequency();
                        break;

                    case "c":
                        temperatures = DataRecorderDisplayGetData(numberOfPoints, dataPointFrequency, myFinch);
                        break;

                    case "d":
                        DataRecorderDisplayData(temperatures);
                        break;

                    case "q":
                        quitDataRecorderMenu = true;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("\tPlease enter a letter for the menu choice.");
                        DisplayContinuePrompt();
                        break;
                }

            } while (!quitDataRecorderMenu);
        }

        static void DataRecorderDisplayData(double[] temperatures)
        {
            DisplayScreenHeader("Data");

            DataRecorderDisplayTable(temperatures);

            DisplayContinuePrompt();
        }

        static void DataRecorderDisplayTable(double[] temperatures) 
        {
            //
            // table headers
            //
            Console.WriteLine(
                "DataPoint".PadLeft(12) +
                "Temp".PadLeft(10)
                );
            Console.WriteLine(
                "----------".PadLeft(12) +
                "----".PadLeft(10)
                );
            //
            // table data
            //
            for (int index = 0; index < temperatures.Length; index++)
            {
                Console.WriteLine(
                (index + 1).ToString().PadLeft(12) +
                (temperatures[index] * 9/5 + 32).ToString("n2").PadLeft(10)
                );
            }
        }

        static double[] DataRecorderDisplayGetData(int numberOfPoints, double dataPointFrequency, Finch finchRobot)
        {
            double[] temperatures = new double[numberOfPoints];
            int frequencyInSeconds;

            DisplayScreenHeader("Get Data");

            // echo number of data points

            Console.WriteLine("The Finch Robot is Ready to Record Tempuratures");
            DisplayContinuePrompt();

            for (int index = 0; index < numberOfPoints; index++)
            {
                temperatures[index] = finchRobot.getTemperature();
                Console.WriteLine($"Data #{index + 1}: {temperatures[index] * 9/5 + 32} farenheit");
                frequencyInSeconds = (int)(dataPointFrequency * 500);
                finchRobot.wait(frequencyInSeconds);
            }

            Console.WriteLine();
            Console.WriteLine("Current Data");
            DataRecorderDisplayTable(temperatures);

            Console.WriteLine();
            Console.WriteLine($"Average Tempurature: {temperatures.Average()}");

            DisplayContinuePrompt();

            return temperatures;
        }

        static double DataRecorderDisplayGetDataPointFrequency()
        {
            int dataPointFrequency;
            bool validResponse;
            string userResponse;

            do
            {
            DisplayScreenHeader("Data Point Frequency");

            Console.WriteLine("Data Point Frequency:");

            userResponse = Console.ReadLine();

            // validate response
            validResponse = int.TryParse(userResponse, out dataPointFrequency);

            if (!validResponse)
            {
                Console.WriteLine("Please enter the Data Point Frequency.");
                Console.WriteLine();
            }

                Console.WriteLine();
            Console.WriteLine($"Data Point Frequency: {dataPointFrequency}");
            } while (!validResponse);


            DisplayContinuePrompt();

            return dataPointFrequency;
        }

        static int DataRecorderDisplayGetNumberOfDataPoints()
        {
            int numberOfDataPoints;
            bool validResponse;
            string userResponse;

            do
            {
            DisplayScreenHeader("Number of Data Points");
            Console.WriteLine("Number of data points:");
            userResponse = Console.ReadLine();

            // validate response
            validResponse = int.TryParse(userResponse, out numberOfDataPoints);

                if (!validResponse)
                {
                    Console.WriteLine("Please enter the Number of data points.");
                    Console.WriteLine();
                }

            Console.WriteLine();
            Console.WriteLine($"Number of Data Points: {numberOfDataPoints}");

            } while (!validResponse);




            DisplayContinuePrompt();

            return numberOfDataPoints;
        }

        #endregion

        #region TALENT SHOW

        /// 
        /// *****************************************************************
        /// *                     Talent Show Menu                          *
        /// *****************************************************************
        /// 
        static void DisplayTalentShowMenuScreen(Finch myFinch)
        {
            Console.CursorVisible = true;

            bool quitTalentShowMenu = false;
            string menuChoice;

            do
            {
                DisplayScreenHeader("Talent Show Menu");

                //
                // get user menu choice
                //
                Console.WriteLine("\ta) Light and Sound");
                Console.WriteLine("\tb) Dance");
                Console.WriteLine("\tc) Mix it up");
                Console.WriteLine("\tq) Main Menu");
                Console.Write("\t\tEnter Choice:");
                menuChoice = Console.ReadLine().ToLower();

                //
                // process user menu choice
                //
                switch (menuChoice)
                {
                    case "a":
                        DisplayLightAndSound(myFinch);
                        break;

                    case "b":
                        DisplayDance(myFinch);
                        break;

                    case "c":
                        DisplayMixingItUp(myFinch);
                        break;

                    case "q":
                        quitTalentShowMenu = true;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("\tPlease enter a letter for the menu choice.");
                        DisplayContinuePrompt();
                        break;
                }

            } while (!quitTalentShowMenu);
        }

        ///
        /// *****************************************************************
        /// *               Talent Show > Light and Sound                   *
        /// *****************************************************************
        /// 
        /// <param name="finchRobot">finch robot object</param>
        /// 
        static void DisplayLightAndSound(Finch finchRobot)
        {
            Console.CursorVisible = false;

            DisplayScreenHeader("Light and Sound");

            Console.WriteLine("\tThe Finch robot will not show off its glowing talent!");
            DisplayContinuePrompt();

            for (int lightSoundLevel = 255; lightSoundLevel > 0; lightSoundLevel -= 2)
            {
                finchRobot.setLED(255, lightSoundLevel, lightSoundLevel);
                finchRobot.noteOn(lightSoundLevel * 100);
                finchRobot.noteOff();
                finchRobot.setLED(0, 0, 0);
            }

            DisplayMenuPrompt("Talent Show Menu");
        }

        /// *****************************************************************
        /// *               Talent Show > Dance                             *
        /// *****************************************************************
        /// 
        /// <param name="myFinch"></param>
        static void DisplayDance(Finch myFinch)
        {
            string shape;

            Console.CursorVisible = false;

            DisplayScreenHeader("Dance");

            Console.WriteLine("\tWhich shape would you like the finch to go in?");
            Console.WriteLine("a) Circle");
            Console.WriteLine("b) Square");
            shape = Console.ReadLine();

            switch (shape)
            {
                case "a":
                    Console.WriteLine("The finch will now move in a circle");
                    DisplayContinuePrompt();
                    for (int Pattern = 0; Pattern < 4; Pattern++)
                    {
                        //myFinch.setMotors(100, 100);
                        //myFinch.wait(200);
                        //myFinch.setMotors(-100, -100);
                        //myFinch.wait(200);
                        myFinch.setMotors(255, 50);
                        myFinch.wait(1000);
                    }
                    break;

                case "b":
                    Console.WriteLine("The finch will now move in a square");
                    DisplayContinuePrompt();
                    for (int pattern = 0; pattern < 1; pattern++)
                    {
                        myFinch.setMotors(255, 255);
                        myFinch.wait(500);
                        myFinch.setMotors(255,50);
                        myFinch.wait(550);
                        myFinch.setMotors(255, 255);
                        myFinch.wait(500);
                        myFinch.setMotors(255, 50);
                        myFinch.wait(550);
                        myFinch.setMotors(255, 255);
                        myFinch.wait(500);
                        myFinch.setMotors(255, 50);
                        myFinch.wait(550);
                        myFinch.setMotors(255, 255);
                        myFinch.wait(500);
                        myFinch.setMotors(255, 50);
                        myFinch.wait(550);
                    }
                    break;

                default:
                    Console.WriteLine("Please enter (a) or (b)");
                    break;
            }
            myFinch.setMotors(0, 0);
            DisplayMenuPrompt("Talent Show Menu");
        }

        /// *****************************************************************
        /// *               Talent Show > Mixing It Up                      *
        /// *****************************************************************
        /// 
        /// <param name="myFinch"></param>
        static void DisplayMixingItUp(Finch myFinch)
        {
            Console.CursorVisible = false;

            DisplayScreenHeader("Mix It Up");

            Console.WriteLine("\tThe Finch robot will now do a little bit of everything!");
            DisplayContinuePrompt();

            for (int Mix = 0; Mix < 1; Mix++)
            {

                DisplayMusic(myFinch);
                myFinch.setLED(Mix, 255, Mix);
                myFinch.setMotors(255, 0);
                myFinch.wait(200);
                myFinch.setMotors(0, 255);
                myFinch.wait(200);
                myFinch.setMotors(-255, 0);
                myFinch.wait(200);
                myFinch.setMotors(0, -255);
                myFinch.wait(200);
            }
            myFinch.setMotors(0, 0);

            DisplayMenuPrompt("Talent Show Menu");
        }

        static void DisplayMusic(Finch myFinch)
        {
            //
            // oh say can you see
            //
            myFinch.setLED(255, 0, 0);
            myFinch.noteOn(700);
            myFinch.wait(500);
            myFinch.noteOn(500);
            myFinch.wait(500);
            myFinch.noteOn(400);
            myFinch.wait(1200);
            myFinch.noteOn(500);
            myFinch.wait(700);
            myFinch.noteOn(600);
            myFinch.wait(700);
            myFinch.noteOn(800);
            myFinch.wait(500);
            myFinch.noteOff();

            //
            // by the dawns early light
            //
            myFinch.setLED(255, 255, 255);
            myFinch.wait(500);
            myFinch.noteOn(1000);
            myFinch.wait(700);
            myFinch.noteOn(900);
            myFinch.wait(500);
            myFinch.noteOn(800);
            myFinch.wait(700);
            myFinch.noteOn(500);
            myFinch.wait(600);
            myFinch.noteOn(550);
            myFinch.wait(600);
            myFinch.noteOn(600);
            myFinch.wait(700);
            myFinch.noteOff();
            myFinch.wait(500);

            //
            // whats so proudly we hailed
            //
            myFinch.setLED(0, 0, 255);
            myFinch.noteOn(600);
            myFinch.wait(700);
            myFinch.noteOn(550);
            myFinch.noteOn(1000);
            myFinch.wait(900);
            myFinch.noteOn(900);
            myFinch.wait(500);
            myFinch.noteOn(800);
            myFinch.wait(500);
            myFinch.noteOn(750);
            myFinch.wait(700);
            myFinch.noteOff();
            myFinch.wait(200);
            myFinch.noteOff();
        }

        #endregion

        #region FINCH ROBOT MANAGEMENT

        /// <summary>
        /// *****************************************************************
        /// *               Disconnect the Finch Robot                      *
        /// *****************************************************************
        /// </summary>
        /// <param name="finchRobot">finch robot object</param>
        static void DisplayDisconnectFinchRobot(Finch finchRobot)
        {
            Console.CursorVisible = false;

            DisplayScreenHeader("Disconnect Finch Robot");

            Console.WriteLine("\tAbout to disconnect from the Finch robot.");
            DisplayContinuePrompt();

            finchRobot.disConnect();

            Console.WriteLine("\tThe Finch robot is now disconnect.");

            DisplayMenuPrompt("Main Menu");
        }

        /// <summary>
        /// *****************************************************************
        /// *                  Connect the Finch Robot                      *
        /// *****************************************************************
        /// </summary>
        /// <param name="finchRobot">finch robot object</param>
        /// <returns>notify if the robot is connected</returns>
        static bool DisplayConnectFinchRobot(Finch finchRobot)
        {
            Console.CursorVisible = false;

            bool robotConnected;

            DisplayScreenHeader("Connect Finch Robot");

            Console.WriteLine("\tAbout to connect to Finch robot. Please be sure the USB cable is connected to the robot and computer now.");
            DisplayContinuePrompt();

            robotConnected = finchRobot.connect();

            // TODO test connection and provide user feedback - text, lights, sounds

            DisplayMenuPrompt("Main Menu");

            //
            // reset finch robot
            //
            finchRobot.setLED(0, 0, 0);
            finchRobot.noteOff();

            return robotConnected;
        }

        #endregion

        #region USER INTERFACE

        /// <summary>
        /// *****************************************************************
        /// *                     Welcome Screen                            *
        /// *****************************************************************
        /// </summary>
        static void DisplayWelcomeScreen()
        {
            Console.CursorVisible = false;

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\tFinch Control");
            Console.WriteLine();

            DisplayContinuePrompt();
        }

        /// <summary>
        /// *****************************************************************
        /// *                     Closing Screen                            *
        /// *****************************************************************
        /// </summary>
        static void DisplayClosingScreen()
        {
            Console.CursorVisible = false;

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\tThank you for using Finch Control!");
            Console.WriteLine();

            DisplayContinuePrompt();
        }

        /// <summary>
        /// display continue prompt
        /// </summary>
        static void DisplayContinuePrompt()
        {
            Console.WriteLine();
            Console.WriteLine("\tPress any key to continue.");
            Console.ReadKey();
        }

        /// <summary>
        /// display menu prompt
        /// </summary>
        static void DisplayMenuPrompt(string menuName)
        {
            Console.WriteLine();
            Console.WriteLine($"\tPress any key to return to the {menuName} Menu.");
            Console.ReadKey();
        }

        /// <summary>
        /// display screen header
        /// </summary>
        static void DisplayScreenHeader(string headerText)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\t" + headerText);
            Console.WriteLine();
        }

        #endregion
    }
}
