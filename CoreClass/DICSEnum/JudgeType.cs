using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreClass.DICSEnum
{
    public enum JudgeGrade
    {
        S,
        A,
        T,
        Q,
        J,
        W,  // W级会在sorter进行重投，因此当无法预料的情况发生的时候应该默认判定该等级；
        D,
        K,  // TSP judge
        E,
        F,
        G,  //LotGrade
        N,  //LotGrade
        U,  //unfinish
        PASS, //SKIP
    }
}
