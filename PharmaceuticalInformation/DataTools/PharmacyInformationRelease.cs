using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test_pharm_server.PharmaceuticalInformation.Interfaces;

namespace Test_pharm_server.PharmaceuticalInformation.DataTools
{
    class PharmacyInformationRelease
    {
        public IPharmacyInformation PhrmInf;

        public PharmacyInformationRelease(IPharmacyInformation _PhrmInf)
        {
            PhrmInf = _PhrmInf;
        }

    }
}
