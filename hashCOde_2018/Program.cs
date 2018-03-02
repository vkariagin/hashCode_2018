using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;

namespace hashCOde_2018
{
    class MainClass
    {
        static string inputPath = "/Users/vitaliy/Downloads/hash_code_in";
        static string outPath = "/Users/vitaliy/Downloads/hash_code_res";
        static string inputFileName = "a_example";


        public static void Main(string[] args)
        {
            readSetting();

        }

        public static void readSetting()
        {
            List<Ride> listOfRides = new List<Ride>();
            Settings settings;

            using (StreamReader fs = new StreamReader(Path.Combine(inputPath, inputFileName + ".in")))
            {
                var line = fs.ReadLine();
                var setting = line.Split(' ');
                // parse settings

                settings = new Settings
                {
                    Row = Int32.Parse(setting[0]),
                    Col = Int32.Parse(setting[1]),
                    VehiclesNum = Int32.Parse(setting[2]),
                    Rides = Int32.Parse(setting[3]),
                    BonusPoints = Int32.Parse(setting[4]),
                    SimulationSteps = Int32.Parse(setting[5])
                };

                List<Car> cars = new List<Car>();
                int temp = 0;
                while (temp < settings.VehiclesNum)
                {
                    cars.Add(new Car());
                    temp++;
                }


                var rideCounter = 0;
                while (fs.Peek() >= 0)
                {
                    var rideSetting = fs.ReadLine().Split(' ');
                    listOfRides.Add(
                        new Ride
                        {
                            startPoint = new Point { x = Int32.Parse(rideSetting[0]), y = Int32.Parse(rideSetting[1]) },
                            endPoint = new Point { x = Int32.Parse(rideSetting[2]), y = Int32.Parse(rideSetting[3]) },
                            earliestStart = Int32.Parse(rideSetting[4]),
                            latestFinish = Int32.Parse(rideSetting[5]),
                            Index = rideCounter
                        });

                    rideCounter++;
                }

                startSimulation(settings, listOfRides, cars);
            }
        }

        static void startSimulation(Settings settings, List<Ride> rides, List<Car> cars)
        {
            int step = 0;

            while (step < settings.SimulationSteps)
            {

                step++;

                foreach (Car car in cars)
                {

                    if (car.Available)
                    {
                        var closestRide = rides.Where(r => !r.IsAssigned).OrderBy(x => ComputeHScore(car.Destination.x, car.Destination.y, x.startPoint.x, x.startPoint.y)).FirstOrDefault();
                        if (closestRide != null){
                            closestRide.IsAssigned = true;
                            car.assignRide(closestRide, step);    
                        }
                    }

                    car.makeMove();
                }
            }
            writeOutPut(cars);
        }

        public static void writeOutPut(List<Car> cars)
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(outPath, inputFileName + ".out")))
            {
                foreach (Car car in cars)
                {
                    var str = car.RoutsList.Count().ToString() + " " + string.Join(" ", car.RoutsList);
                    sw.WriteLine(str);
                }
            }
        }

        static int ComputeHScore(int x, int y, int targetX, int targetY)
        {
            return Math.Abs(targetX - x) + Math.Abs(targetY - y);
        }
    }

    class Settings {
        public int Row;
        public int Col;
        public int VehiclesNum;
        public int Rides;
        public int BonusPoints;
        public int SimulationSteps;
    }

    class Ride
    {
        public Point startPoint;
        public Point endPoint;
        public int earliestStart;
        public int latestFinish;
        public int Index;
        public bool IsAssigned;
    }

    class Car
    {
        public Point Destination;
        public bool Available;

        public int BonusPoints;
        public List<int> RoutsList;

        int stepsCount;

        public Car() 
        {
            stepsCount = 0;
            RoutsList = new List<int>();

        }

        int ComputeHScore(int x, int y, int targetX, int targetY)
        {
            return Math.Abs(targetX - x) + Math.Abs(targetY - y);
        }

        public void addBonus(int points) {
            this.BonusPoints+=points;
        }

        public void assignRide (Ride rideInfo, int currentSystemStep)
        {
            var costToRidePoint = ComputeHScore(Destination.x, Destination.y, rideInfo.startPoint.x, rideInfo.startPoint.y);
            var stepsToWait = rideInfo.earliestStart - currentSystemStep + costToRidePoint;

            var rideCost = ComputeHScore(rideInfo.startPoint.x, rideInfo.startPoint.y, rideInfo.endPoint.x, rideInfo.endPoint.y);

            this.stepsCount = costToRidePoint + (stepsToWait < 0 ? 0 : stepsToWait) + rideCost;
            this.Available = false;

            Destination = rideInfo.endPoint;
            RoutsList.Add(rideInfo.Index);
        }

        public void makeMove()
        {
            if(stepsCount>0){
                stepsCount -= 1;    
            }
            else{
                this.Available = true;
                 
            }
        }
    }


    struct Point {
        public int x;
        public int y;
    }
}
