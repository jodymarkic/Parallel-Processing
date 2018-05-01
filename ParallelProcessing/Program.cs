/*
 *  FILENAME        : Program.cs
 *  PROJECT         : ParallelProcessing
 *  PROGRAMMER      : Jody Markic
 *  FIRST VERSION   : 2018-04-03
 *  DESCRIPTION     : This project is made with the purpose of comparing computation speeds between Sequential and Parallel Processing.
 *                    The project calculates and measures the ticks elapsed to compute all prime number to a given ceiling.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelProcessing
{
    /*
     * CLASS        : Program
     * DESCRIPTION  : Class that holds the main method.
     * 
     */
    class Program
    {
        /*
         *  METHOD      : Main()
         *  DESCRIPTION : Entry point of the program
         *  PARAMETERS  : string[] args
         *  RETURNS     : n/a
         */
        static void Main(string[] args)
        {
            //begin program
            StartProgram();
        }

        /*
        *  METHOD      : StartProgram()
        *  DESCRIPTION : Starts the program
        *  PARAMETERS  : n/a
        *  RETURNS     : n/a
        */
        static void StartProgram()
        {
            //constant to indicate modes of operation
            const int sequential = 1;
            const int parallel = 2;

            //additional local variables
            int upperBound;
            int mode;

            //Continue to run until user selection of exit, will result in evalution being false.
            while (!StartMenu(out upperBound, out mode))
            {
                //seed HashSet of number upto upperbound
                HashSet<int> sequence = SeedHashSet(upperBound);
                HashSet<int> results = new HashSet<int>();

                //evaluate mode of operation
                switch (mode)
                {
                    case sequential: //compute prime number sequentially.
                        results = SequentialComputation(sequence);
                        break;
                    case parallel: //compute prime number in parallel
                        results = ParallelComputation(sequence);
                        break;
                    default:
                        break;
                }
                //write prime numbers to an output file.
                WriteToFile(results);

                //continue program
                Console.WriteLine("\nPress Any Key to Continue");
                Console.ReadLine();
            }
        }

        /*
         *  METHOD      : StartMenu()
         *  DESCRIPTION : Requests Input from User, & sets up the mode of operation
         *  PARAMETERS  : out int upperBound, out int mode
         *  RETURNS     : bool exitFlag
         */
        static bool StartMenu(out int upperBound, out int mode)
            {
                upperBound = 0;
                mode = 0;

                int computation_mode = 0;
                string upperBoundBuffer;
                string userInput;
                bool exitFlag = false;

                //prompt type of computation or exit program from user
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("Please Select a Mode of Operation!\n");
                    Console.WriteLine("1. Sequential Computation of Prime Numbers.");
                    Console.WriteLine("2. Parallel Computation of Prime Numbers.");
                    Console.WriteLine("3. Exit Program.");
                    Console.Write("Your Input >> ");
                    userInput = Console.ReadLine();

                    if (!userInput.Equals("3")) //if not exit
                    {
                        if (userInput.Equals("1") || userInput.Equals("2")) //check for computation type
                        {
                            Int32.TryParse(userInput, out computation_mode); //store computation request
                            mode = computation_mode;

                            //prompt for an upperbound limit
                            while (true)
                            {
                                Console.Clear();
                                Console.WriteLine("Calculate all Prime Numbers to an Upperbound Limit!\n");
                                Console.WriteLine("Please Provide An Upperbound!");
                                Console.Write("Your Input >> ");
                                upperBoundBuffer = Console.ReadLine();

                                if (Int32.TryParse(upperBoundBuffer, out upperBound)) //if upperbound is a number store and exit while true
                                {
                                    break; 
                                }
                                else //if invalid input
                                {
                                    Console.Clear();
                                    Console.WriteLine("\nThat was an Invalid Upperbound...");
                                    Console.WriteLine("Please Try Again!");
                                    Console.WriteLine("Press Enter to Continue...");
                                    Console.ReadLine();
                                }
                            }
                            break;
                        }
                        else //if invalid input
                        {
                            Console.Clear();
                            Console.WriteLine("\nThat was an Invalid Menu Option...");
                            Console.WriteLine("Please Try Again!");
                            Console.WriteLine("Press Enter to Continue...");
                            Console.ReadLine();
                        }
                    }
                    else //if exit set flag and break out of while true
                    {
                        exitFlag = true;
                        break;
                    }
                }
                return exitFlag; //return exit or run flag.
            }

        /*
         *  METHOD      : SeedHashSet()
         *  DESCRIPTION : seeds a hashset from 0 to an upperbound
         *  PARAMETERS  : int upperBound
         *  RETURNS     : HashSet<int> sequence
         */
        static HashSet<int> SeedHashSet(int upperBound)
        {
            HashSet<int> sequence = new HashSet<int>();

            //seed hash set.
            for (int i = 2; i < upperBound; i++)
            {
                sequence.Add(i);
            }

            return sequence;
        }

        /*
         *  METHOD      : SequentialComputation()
         *  DESCRIPTION : Calculate all prime number upto an ceiling sequentially and returns found primes in a Hashset
         *  PARAMETERS  : HashSet<int> sequence
         *  RETURNS     : HashSet<int> PrimeNumbers
         */
        static HashSet<int> SequentialComputation(HashSet<int> sequence)
        {
            Stopwatch stopWatch = new Stopwatch();
            // add prime number 2 to hashset results
            HashSet<int> PrimeNumbers = new HashSet<int>();
            PrimeNumbers.Add(2);
            //start stopwatch
            stopWatch.Start();
            foreach (int number in sequence) //for each number
            {
                bool isPrime = true;
                for (long j = 2; j < number; j++)  //check if it is divisible by all number before it
                {
                    if (number % j == 0) //if divisible with no remainder
                    {
                        isPrime = false; //it is not a prime number
                        break;
                    }
                }
                if (isPrime) //if it is a prime number add it to the hashset results.
                {
                    PrimeNumbers.Add(number);
                }
            }
            stopWatch.Stop(); //stop the watch

            long duration = stopWatch.ElapsedTicks; //get and display elapsed ticks
            Console.WriteLine("\nUsing Sequential Computation,");
            Console.WriteLine("Time Elapsed to Calculate: " + duration.ToString() + " ticks.");

            return PrimeNumbers;
        }

        /*
         *  METHOD      : ParallelComputation()
         *  DESCRIPTION : Calculate all prime number upto an ceiling in parallel and returns found primes in a Hashset
         *  PARAMETERS  : HashSet<int> sequence
         *  RETURNS     : HashSet<int> PrimeNumbers
         */
        static HashSet<int> ParallelComputation(HashSet<int> sequence)
        {
            Stopwatch stopWatch = new Stopwatch();
            // add prime number 2 to hashset results
            HashSet<int> PrimeNumbers = new HashSet<int>();
            PrimeNumbers.Add(2);
            //start stopwatch
            stopWatch.Start();
            Parallel.ForEach(sequence, number => { //for each number

                bool isPrime = true; 
                for (long j = 2; j < number; j++) //check if it is divisible by all number before it
                {
                    if (number % j == 0) //if divisible with no remainder
                    {
                        isPrime = false; //it is not a prime number
                        break;
                    }
                }
                if (isPrime)//if it is a prime number add it to the hashset results.
                {
                    PrimeNumbers.Add(number);
                }
            });
            stopWatch.Stop(); //stop the watch

            long duration = stopWatch.ElapsedTicks; //get and display elapsed ticks
            Console.WriteLine("\nUsing Parallel Computation,");
            Console.WriteLine("Time Elapsed to Calculate: " + duration.ToString() + " ticks");

            return PrimeNumbers;
        }

        /*
         *  METHOD      : WriteToFile()
         *  DESCRIPTION : Writes primenumbers to PrimeNumbers.txt
         *  PARAMETERS  : HashSet<int> results
         *  RETURNS     : void
         */
        static void WriteToFile(HashSet<int> results)
        {
            int counter = 1;

            //delete file if exists
            if (File.Exists(@"PrimeNumbers.txt"))
            {
                File.Delete(@"PrimeNumbers.txt");
            }

           Console.WriteLine("\nWriting Results to PrimeNumbers.txt");

           //write prime numbers to file.
           using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(@"PrimeNumbers.txt", true))
           {
                foreach (int number in results)
                {
                    file.WriteLine("Counter: " + counter.ToString() + " | Prime: " + number.ToString());
                    counter++;
                }
           }

           Console.WriteLine("Finished Writing, Please Check PrimeNumbers.txt!");
        }
    }
}
