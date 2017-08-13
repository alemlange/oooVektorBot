using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Enums
{
    public enum SessionState
    {
        Unknown,
        Queue,
        Sitted,
        //DishChoosing,
        OrderingDish,
        Closed,
        Deactivated
    }
}
