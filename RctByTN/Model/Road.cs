﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RctByTN.Model
{
    class Road : ParkElement
    {
        public Road(int x, int y, int buildcost, ElementStatus status) : base(x, y, buildcost, status)
        {
        }

        public override void ModifyGuest(Guest guest)
        {
            throw new NotImplementedException();
        }
    }
}