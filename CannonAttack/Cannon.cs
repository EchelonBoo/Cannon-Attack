using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CannonAttack
{
    public sealed class Cannon //seales so the class isn't inherited by anything
    {


        private readonly string CANNONID = "Human"; //stores default ID in case user doesn't //readonly - runtime constant
        private string CannonID;
        public static readonly int MAXANGLE = 90;
        public static readonly int MINANGLE = 0;
        public static readonly int MAXVELOCITY = 300000000;
        private readonly int MAXDISTANCEOFTARGET = 20000;
        private readonly int BURSTRADIUS = 50;
        private int shots;

        private int distanceOfTarget;
        private int distanceOfPerson;
        private readonly double GRAVITY = 9.8;

        public int DistanceOfTarget
        {
            get { return distanceOfTarget; }
            set { distanceOfTarget = value; }
        }
        public int DistanceOfPerson
        {
            get { return distanceOfPerson; }
            set { distanceOfPerson = value; }
        }


        public string ID {

            get
            {
                return (String.IsNullOrWhiteSpace(CannonID)) ? CANNONID : CannonID;
            }

            set {

                CannonID = value;

            }
        }


        private static Cannon cannonSingletonInstance;

        static readonly object padlock = new object();

        private Cannon()
        {
            Random r = new Random();
            SetTarget(r.Next((int)MAXDISTANCEOFTARGET));
            SetPerson(r.Next((int)MAXDISTANCEOFTARGET));
        }

        public static Cannon GetInstance()
        {
            lock (padlock) //this lock ensures that only one thread enters this block at any given time.
            {
                if (cannonSingletonInstance == null)
                {
                    cannonSingletonInstance = new Cannon();
                }

                return cannonSingletonInstance;
            }
        }

        public void SetTarget(int distanceOfTarget)
        {
            if (!distanceOfTarget.Between(0, MAXDISTANCEOFTARGET))
            {
                throw new ApplicationException(String.Format("Target distance must be between 1 and {0} meters", MAXDISTANCEOFTARGET));
            }
            this.distanceOfTarget = distanceOfTarget;
        }

        public Tuple<bool, string> Shoot(int angle, int velocity)
        {
           if(angle > MAXANGLE || angle < MINANGLE) //angle must be between 90 and 0
            {
                return Tuple.Create(false, "Angle incorrect");
            }

            if (velocity > MAXVELOCITY)
            {
                return Tuple.Create(false, "Velocity of the cannon can't travel faster than the speed of light!");
            }
            shots++;
            string message;
            bool hit;
            int distanceOfShot = CalculateDistanceOfCannonShot(angle, velocity);
            if (distanceOfShot.WithinRange(this.distanceOfTarget, BURSTRADIUS))
            {
                message = String.Format("Hit - {0} Shot(s)", shots);
                hit = true;
            }
            else if(distanceOfShot.WithinRange(this.distanceOfPerson, BURSTRADIUS)){

                message = String.Format("You hit a person and killed them. You lose.");
                hit = true;
            }
            else
            {
                message = String.Format("Missed cannonball landed at {0} meters", distanceOfShot);
                hit = false;
            }
            return Tuple.Create(hit, message);
        }

        public void SetPerson(int distanceOfPerson)
        {
            if (!distanceOfPerson.Between(0, MAXDISTANCEOFTARGET))
            {
                throw new ApplicationException(String.Format("Person distance must be between 1 and {0} meters", MAXDISTANCEOFTARGET));
            }
            this.distanceOfPerson = distanceOfPerson;
        }

        public int CalculateDistanceOfCannonShot(int angle, int velocity)
        {
            int time = 0;
            double height = 0;
            double distance = 0;
            double angleInRadians = (3.1415926536 / 180) * angle;
            while (height >= 0)
            {
                time++;
                distance = velocity * Math.Cos(angleInRadians) * time;
                height = (velocity * Math.Sin(angleInRadians) * time) - (GRAVITY * Math.Pow(time, 2)) / 2;
            }
            return (int)distance;
        }
        public int Shots
        {
            get { return shots; }

        }

        public void Reset()
        {
            shots = 0;
        }

    }

}
