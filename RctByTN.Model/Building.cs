using System;
using System.Collections.Generic;
using System.Text;

namespace RctByTN.Model
{
    public abstract class Building : ParkElement
    {
        protected List<Guest> _waitingList;
        protected List<Guest> _userList;
        protected int _minCapacity;
        protected int _maxCapacity;
        protected int _useCost;
        protected int _useTime;
        protected int _serviceCost;

        public List<Guest> WaitingList { get => _waitingList; set => _waitingList = value; }
        public List<Guest> UserList { get => _userList; set => _userList = value; }
        public int MinCapacity { get => _minCapacity; set => _minCapacity = value < 0 ? 0 : value; }
        public int MaxCapacity { get => _maxCapacity; set => _maxCapacity = value < 0 ? 0 : value; }
        public int UseCost { get => _useCost; set => _useCost = value < 0 ? 0 : value; }
        public int UseTime { get => _useTime; set => _useTime = value < 0 ? 0 : value; }
        public int ServiceCost { get => _serviceCost; set => _serviceCost = value < 0 ? 0 : value; }


        public Building(int x, int y, int minCapacity, int maxCapacity, int buildcost, int usecost, int usetime, int maintainCost, int serviceCost) : base(x,y,buildcost,maintainCost)
        {
            this.MinCapacity = minCapacity;
            this.MaxCapacity = maxCapacity;
            this.UseCost = usecost;
            this.UseTime = usetime;
            this.ServiceCost = serviceCost;
            this.WaitingList = new List<Guest>();
            this.UserList = new List<Guest>();
        }
    }
}
