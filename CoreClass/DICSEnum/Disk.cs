using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreClass.DICSEnum
{
    public enum Disk
    {
        Null,
        E_Drive,
        F_Drive,
        G_Drive,
        H_Drive,
        I_Drive,
        J_Drive,
        K_Drive,
        L_Drive,
        M_Drive,
        N_Drive,
        O_Drive,
        P_Drive,
        Q_Drive,
        R_Drive,
        S_Drive,
    }
    public enum DiskStatus
    {
        Unchecked,
        NotExist,
        ConnectError,
        ConnectOverTime,
        OK,
        OnSearch,
    }
}
